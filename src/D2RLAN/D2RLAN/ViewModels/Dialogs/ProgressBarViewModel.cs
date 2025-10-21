using Caliburn.Micro;
using D2RLAN.ViewModels;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;

namespace D2RLAN.ViewModels.Dialogs
{
    public class ProgressBarViewModel : Screen
    {
        private double _progressValue;
        private string _progressText = "Preparing...";
        private CancellationTokenSource _cts;

        public ProgressBarViewModel(ShellViewModel shellViewModel)
        {
            ShellViewModel = shellViewModel;
            DisplayName = "Verifying Data Integrity";
            _cts = new CancellationTokenSource();
            RunVerificationAsync();
        }

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                NotifyOfPropertyChange(() => ProgressValue);
            }
        }
        public string ProgressText
        {
            get => _progressText;
            set
            {
                _progressText = value;
                NotifyOfPropertyChange(() => ProgressText);
            }
        }

        #region ---Properties---

        public ShellViewModel ShellViewModel { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        #endregion

        public void Cancel()
        {
            _cts.Cancel();
            TryCloseAsync(false);
        }

        public async Task RunVerificationAsync()
        {
            try
            {
                var progress = new Progress<double>(value =>
                {
                    ProgressValue = value;
                    ProgressText = $"Verifying files... {value}%";
                });

                string dataHash = await GetFolderMd5("../D2R/data/data", true, progress, _cts.Token
                );

                ShellViewModel.UserSettings.DataHash = dataHash;
                ProgressText = "Verification complete.";
                ProgressValue = 100;

                await TryCloseAsync(true);
            }
            catch (OperationCanceledException)
            {
                ProgressText = "Operation canceled.";
                await TryCloseAsync(false);
            }
            catch (Exception ex)
            {
                ProgressText = $"Error: {ex.Message}";
                await TryCloseAsync(false);
            }
        }



        public async Task<string> GetFolderMd5(string folderPath, bool recursive = true, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"The folder '{folderPath}' does not exist.");

            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(folderPath, "*", searchOption);
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);

            using (var md5 = MD5.Create())
            {
                byte[] buffer = new byte[1024 * 1024];
                long totalSize = files.Where(f => !string.Equals(Path.GetFileName(f), "shmem", StringComparison.OrdinalIgnoreCase)).Sum(f => new FileInfo(f).Length);

                long processedBytes = 0;
                int lastPercent = -1;

                foreach (var file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (string.Equals(Path.GetFileName(file), "shmem", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string relativePath = Path.GetRelativePath(folderPath, file);
                    byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLowerInvariant());
                    md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, buffer.Length, FileOptions.SequentialScan))
                    {
                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                            processedBytes += bytesRead;

                            int percent = (int)(processedBytes * 100L / totalSize);
                            if (percent != lastPercent)
                            {
                                progress?.Report(percent);
                                lastPercent = percent;
                            }
                        }
                    }
                }

                md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                progress?.Report(100);

                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
