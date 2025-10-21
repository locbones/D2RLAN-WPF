using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using D2RLAN.Extensions;
using D2RLAN.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using JetBrains.Annotations;
using SevenZip;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLAN.ViewModels.Dialogs;

public class DownloadNewModViewModel : Caliburn.Micro.Screen
{
    #region ---Static Members---

    private ILog _logger = LogManager.GetLogger(typeof(DownloadNewModViewModel));
    private ObservableCollection<KeyValuePair<string, string>> _mods = new ObservableCollection<KeyValuePair<string, string>>();
    private string _serviceAccountEmail;
    private string _privateKey; 
    private KeyValuePair<string, string> _selectedMod;
    private string _modDownloadLink;
    private double _downloadProgress;
    private bool _progressBarIsIndeterminate;
    private string _progressStatus;
    private string _downloadProgressString;

    #endregion

    #region ---Window/Loaded Handlers---

    public DownloadNewModViewModel()
    {
        if (Execute.InDesignMode)
        {
            DownloadProgressString = "70%";
            ProgressStatus = "Test Progress Status...";
            SelectedMod = new KeyValuePair<string, string>("Text Mod", "This is a test string");
            ModDownloadLink = "This is a test string";
        }
    }
    public DownloadNewModViewModel(ShellViewModel shellViewModel)
    {
        DisplayName = "Download A New Mod";
        ShellViewModel = shellViewModel;

        _serviceAccountEmail = ShellViewModel.Configuration["ServiceAccountEmail"] ?? string.Empty;
        _privateKey = ShellViewModel.Configuration["PrivateKey"] ?? string.Empty;

        if (string.IsNullOrEmpty(_serviceAccountEmail) || string.IsNullOrEmpty(_privateKey))
        {
            MessageBox.Show("Please make sure appSettings.json has been properly setup!");
            return;
        }

        Execute.OnUIThread(async () =>
        {
            await GetAvailableMods();

            if (ShellViewModel.UserSettings.DataHashPass == false && SelectedMod.Key == "TCP Files (Install First)")
                OnInstallMod();
        });
    }

    #endregion

    #region ---Properties---

    public string ProgressStatus
    {
        get => _progressStatus;
        set
        {
            if (value == _progressStatus) return;
            _progressStatus = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ProgressBarIsIndeterminate
    {
        get => _progressBarIsIndeterminate;
        set
        {
            if (value == _progressBarIsIndeterminate) return;
            _progressBarIsIndeterminate = value;
            NotifyOfPropertyChange();
        }
    }
    public string DownloadProgressString
    {
        get => _downloadProgressString;
        set
        {
            if (value == _downloadProgressString) return;
            _downloadProgressString = value;
            NotifyOfPropertyChange();
        }
    }
    public double DownloadProgress
    {
        get => _downloadProgress;
        set
        {
            if (value.Equals(_downloadProgress)) return;
            _downloadProgress = value;
            NotifyOfPropertyChange();
        }
    }
    public string ModDownloadLink
    {
        get => _modDownloadLink;
        set
        {
            if (value == _modDownloadLink) return;
            _modDownloadLink = value;
            NotifyOfPropertyChange();
        }
    }
    public KeyValuePair<string, string> SelectedMod
    {
        get => _selectedMod;
        set
        {
            if (value.Equals(_selectedMod)) return;
            _selectedMod = value;
            NotifyOfPropertyChange();
        }
    }
    public ObservableCollection<KeyValuePair<string, string>> Mods
    {
        get => _mods;
        set
        {
            if (Equals(value, _mods))
            {
                return;
            }
            _mods = value;
            NotifyOfPropertyChange();
        }
    }
    public ShellViewModel ShellViewModel { get; }

    #endregion

    #region ---Download Mod Functions---

    private async Task GetAvailableMods()
    {
        Mods.Clear();

        if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.003") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.008"))
        {
            var tcpEntry = new KeyValuePair<string, string>("TCP Files (Install First)", "https://www.dropbox.com/scl/fi/d0swzfshayfkvf077yw2c/0927_idx_p2.zip?rlkey=yx1w7ats6mie9a7nbj8rjcp6k&st=xymgblqq&dl=1");
            _logger.Info("TCP FILES: Part 1 files found in game folder, skipping to part 2");

            Mods.Add(tcpEntry);
            SelectedMod = tcpEntry;
            return;
        }
        else if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.008") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.013"))
        {
            var tcpEntry = new KeyValuePair<string, string>("TCP Files (Install First)", "https://www.dropbox.com/scl/fi/kekirhb9iupq32yoxyyvi/0927_idx_p3.zip?rlkey=3ld3fegp5j6budwhx1vw6a14x&st=o9422biw&dl=1");
            _logger.Info("TCP FILES: Parts 1&2 files found in game folder, skipping to part 3");

            Mods.Add(tcpEntry);
            SelectedMod = tcpEntry;
            return;
        }
        else if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.013") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.018"))
        {
            var tcpEntry = new KeyValuePair<string, string>("TCP Files (Install First)", "https://www.dropbox.com/scl/fi/2sd571yc429h169fmjzwa/0927_idx_p4.zip?rlkey=ff8daf427acq0mk4wda01036y&st=f4moofu0&dl=1");
            _logger.Info("TCP FILES: Parts 1-3 files found in game folder, skipping to part 4");

            Mods.Add(tcpEntry);
            SelectedMod = tcpEntry;
            return;
        }
        else if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.018") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.023"))
        {
            var tcpEntry = new KeyValuePair<string, string>("TCP Files (Install First)", "https://www.dropbox.com/scl/fi/ixp1dizsevtl6b60cn8bw/0927_idx_p5.zip?rlkey=xai0pttu75qc9iu8iq1mwj74s&st=hb4zyml7&dl=1");
            _logger.Info("TCP FILES: Parts 1-4 files found in game folder, skipping to part 5");

            Mods.Add(tcpEntry);
            SelectedMod = tcpEntry;
            return;
        }
        else if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.023") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.027"))
        {
            var tcpEntry = new KeyValuePair<string, string>("TCP Files (Install First)", "https://www.dropbox.com/scl/fi/h6jgqqkdi2tg94ua4w8sk/0927_idx_p6.zip?rlkey=av43wdttqhiyu9orribogvff8&st=6cs4sji3&dl=1");
            _logger.Info("TCP FILES: Parts 1-5 files found in game folder, skipping to part 6");

            Mods.Add(tcpEntry);
            SelectedMod = tcpEntry;
            return;
        }
        else if (!File.Exists($@"{ShellViewModel.GamePath}data\data\data.001"))
        {
            var tcpEntry = new KeyValuePair<string, string>("TCP Files (Install First)", string.Join(",", new[]
            {
                "https://www.dropbox.com/scl/fi/h8kj7w67d9wux1fqii1br/0927_idx_p1.zip?rlkey=02s8y2s3x2s2sdtqfx55ehg67&st=1qspfvqk&dl=1",
                "https://www.dropbox.com/scl/fi/d0swzfshayfkvf077yw2c/0927_idx_p2.zip?rlkey=yx1w7ats6mie9a7nbj8rjcp6k&st=je9a3maa&dl=1",
                "https://www.dropbox.com/scl/fi/kekirhb9iupq32yoxyyvi/0927_idx_p3.zip?rlkey=3ld3fegp5j6budwhx1vw6a14x&st=o9422biw&dl=1",
                "https://www.dropbox.com/scl/fi/2sd571yc429h169fmjzwa/0927_idx_p4.zip?rlkey=ff8daf427acq0mk4wda01036y&st=f4moofu0&dl=1",
                "https://www.dropbox.com/scl/fi/ixp1dizsevtl6b60cn8bw/0927_idx_p5.zip?rlkey=xai0pttu75qc9iu8iq1mwj74s&st=hb4zyml7&dl=1",
                "https://www.dropbox.com/scl/fi/h6jgqqkdi2tg94ua4w8sk/0927_idx_p6.zip?rlkey=av43wdttqhiyu9orribogvff8&st=6cs4sji3&dl=1"
            }));
            _logger.Info("TCP FILES: Part 1 files not found, downloading both parts...");
            Mods.Add(tcpEntry);
            SelectedMod = tcpEntry;

            return;
        }
        else
        {
            try
            {
                // Create credentials
                ServiceAccountCredential serviceAccountCredential = new(new ServiceAccountCredential.Initializer(_serviceAccountEmail)
                {
                    Scopes = new[] { SheetsService.Scope.Spreadsheets }
                }.FromPrivateKey(_privateKey));

                // Create Google Sheets service
                SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = serviceAccountCredential,
                    ApplicationName = "D2RLaunch"
                });

                // Define spreadsheetId and ranges
                string spreadsheetId = "1ICm2wxCTrQrgRxPJshj1WPA10-slATymYLm7WYkmkis";
                string columnDRange = "Sheet1!D10:D";
                string columnGRange = "Sheet1!G10:G";

                // Fetch values from Google Sheets for column D
                SpreadsheetsResource.ValuesResource.GetRequest request =
                    sheetsService.Spreadsheets.Values.Get(spreadsheetId, columnDRange);

                ValueRange response = await request.ExecuteAsync();
                IList<IList<object>> dValues = response.Values;

                // Fetch values from Google Sheets for column G
                SpreadsheetsResource.ValuesResource.GetRequest request2 =
                    sheetsService.Spreadsheets.Values.Get(spreadsheetId, columnGRange);

                response = await request2.ExecuteAsync();
                IList<IList<object>> gValues = response.Values;

                if (dValues.Count != gValues.Count)
                {
                    System.Windows.MessageBox.Show(
                        "The number of items in column D does not match the number of items in column G.\nPlease notify an admin.",
                        "Column Mismatch!", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.Error("The number of items in column D does not match the number of items in column G.");
                    return;
                }

                Mods.Clear();
                for (int i = 0; i < dValues.Count; i++)
                {
                    Mods.Add(new KeyValuePair<string, string>(
                        dValues[i][0].ToString(),
                        gValues[i][0].ToString()));
                }

                // Automatically assign first entry to SelectedMod
                if (Mods.Count > 0)
                    SelectedMod = Mods[0];
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(ex);
            }
        }
    }

    [UsedImplicitly]
    public async void OnInstallMod()
    {
        if (SelectedMod.Key != "TCP Files (Install First)")
            ModDownloadLink = ModDownloadLink.TrimEnd();

        string tempPath = Path.GetTempPath();
        string tempExtractedModFolderPath = Path.Combine(tempPath, "NewModDownload");
        SevenZipExtractor.SetLibraryPath("7z.dll");

        try
        {
            if (Directory.Exists(tempExtractedModFolderPath))
                Directory.Delete(tempExtractedModFolderPath, true);

            // === Branch: TCP special handling ===
            if (SelectedMod.Key == "TCP Files (Install First)")
            {
                var links = SelectedMod.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                int fileIndex = 1;

                foreach (var link in links)
                {
                    string tempFile = Path.Combine(ShellViewModel.GamePath, $"BaseTCPFiles_Part{fileIndex}.zip");

                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = Timeout.InfiniteTimeSpan;

                        var response = await client.GetAsync(link.Trim(), HttpCompletionOption.ResponseHeadersRead);
                        response.EnsureSuccessStatusCode();
                        var contentLength = response.Content.Headers.ContentLength ?? -1L;

                        await using var httpStream = await response.Content.ReadAsStreamAsync();
                        await using var file = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);

                        byte[] buffer = new byte[81920];
                        long totalRead = 0;
                        int read;
                        var sw = Stopwatch.StartNew();

                        if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.003") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.008"))
                            fileIndex = 2;
                        if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.008") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.013"))
                            fileIndex = 3;
                        if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.013") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.018"))
                            fileIndex = 4;
                        if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.018") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.023"))
                            fileIndex = 5;
                        if (File.Exists($@"{ShellViewModel.GamePath}data\data\data.023") && !File.Exists($@"{ShellViewModel.GamePath}data\data\data.027"))
                            fileIndex = 6;

                        ProgressBarIsIndeterminate = false;
                        ProgressStatus = $"Downloading part {fileIndex} of 6...";

                        while ((read = await httpStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await file.WriteAsync(buffer, 0, read);
                            totalRead += read;

                            double speed = totalRead / Math.Max(0.0001, sw.Elapsed.TotalSeconds);
                            string speedStr = $"{speed / 1024d / 1024d:0.00} MB/s";

                            if (contentLength > 0)
                            {
                                double progress = (double)totalRead / contentLength * 100.0;
                                double remainingSeconds = (contentLength - totalRead) / Math.Max(1, speed);
                                string timeRemaining = remainingSeconds > 0 ? $"{TimeSpan.FromSeconds(remainingSeconds):mm\\:ss}" : "--:--";

                                Execute.OnUIThread(() =>
                                {
                                    DownloadProgress = Math.Round(progress, MidpointRounding.AwayFromZero);
                                    DownloadProgressString = $"{DownloadProgress}%  " + $"({totalRead / 1024d / 1024d:0} / {contentLength / 1024d / 1024d:0} MB)  " + $"{speedStr}  ETA: {timeRemaining}";
                                });
                            }
                            else
                            {
                                // Unknown content-length: show bytes + speed
                                Execute.OnUIThread(() =>
                                {
                                    DownloadProgressString = $"{totalRead / 1024d / 1024d:0} MB downloaded  {speedStr}";
                                });
                            }
                        }

                        file.Close();
                        sw.Stop();
                    }

                    ProgressStatus = $"Extracting part {fileIndex} of 6...";
                    ProgressBarIsIndeterminate = true;

                    // Special extraction: strip root folder
                    await Task.Run(() =>
                    {
                        if (tempFile.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var archive = ZipFile.OpenRead(tempFile))
                            {
                                foreach (var entry in archive.Entries)
                                {
                                    if (string.IsNullOrEmpty(entry.FullName))
                                        continue;

                                    string relativePath = entry.FullName;
                                    int slashIndex = relativePath.IndexOf('/');
                                    if (slashIndex >= 0)
                                        relativePath = relativePath.Substring(slashIndex + 1);

                                    if (string.IsNullOrWhiteSpace(relativePath))
                                        continue;

                                    string destinationPath = Path.Combine(ShellViewModel.GamePath, relativePath);
                                    string destDir = Path.GetDirectoryName(destinationPath);
                                    if (!string.IsNullOrEmpty(destDir))
                                        Directory.CreateDirectory(destDir);

                                    // Skip directory entries
                                    if (!entry.FullName.EndsWith("/"))
                                        entry.ExtractToFile(destinationPath, true);
                                }
                            }
                        }
                    });

                    File.Delete(tempFile);
                    fileIndex++;
                }

                // mark as fully downloaded for UI
                Execute.OnUIThread(() =>
                {
                    DownloadProgress = 100;
                    DownloadProgressString = "Download complete.";
                });
            }
            // === Branch: Normal mods (single file, but with progress) ===
            else
            {
                string tempFile = Path.Combine(tempPath, "NewModDownload.zip");

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = Timeout.InfiniteTimeSpan;

                    var response = await client.GetAsync(SelectedMod.Value.Trim(), HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();
                    var contentLength = response.Content.Headers.ContentLength ?? -1L;

                    await using var httpStream = await response.Content.ReadAsStreamAsync();
                    await using var file = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);

                    byte[] buffer = new byte[81920];
                    long totalRead = 0;
                    int read;
                    var sw = Stopwatch.StartNew();

                    ProgressBarIsIndeterminate = false;
                    ProgressStatus = "Downloading mod...";

                    while ((read = await httpStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await file.WriteAsync(buffer, 0, read);
                        totalRead += read;

                        double speed = totalRead / Math.Max(0.0001, sw.Elapsed.TotalSeconds);
                        string speedStr = $"{speed / 1024d / 1024d:0.00} MB/s";

                        if (contentLength > 0)
                        {
                            double progress = (double)totalRead / contentLength * 100.0;
                            double remainingSeconds = (contentLength - totalRead) / Math.Max(1, speed);
                            string timeRemaining = remainingSeconds > 0
                                ? $"{TimeSpan.FromSeconds(remainingSeconds):mm\\:ss}"
                                : "--:--";

                            Execute.OnUIThread(() =>
                            {
                                DownloadProgress = Math.Round(progress, MidpointRounding.AwayFromZero);
                                DownloadProgressString =
                                    $"{DownloadProgress}%  " +
                                    $"({totalRead / 1024d / 1024d:0} / {contentLength / 1024d / 1024d:0} MB)  " +
                                    $"{speedStr}  ETA: {timeRemaining}";
                            });
                        }
                        else
                        {
                            Execute.OnUIThread(() =>
                            {
                                DownloadProgressString =
                                    $"{totalRead / 1024d / 1024d:0} MB downloaded  {speedStr}";
                            });
                        }
                    }

                    file.Close();
                    sw.Stop();
                    Execute.OnUIThread(() =>
                    {
                        DownloadProgress = 100;
                        DownloadProgressString = "Download complete.";
                    });
                }

                ProgressStatus = "Extracting mod...";
                ProgressBarIsIndeterminate = true;

                if (tempFile.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    ZipFile.ExtractToDirectory(tempFile, tempExtractedModFolderPath, true);
                else
                {
                    using var extractor = new SevenZipExtractor(tempFile);
                    extractor.ExtractArchive(tempExtractedModFolderPath);
                }

                File.Delete(tempFile);
            }

            if (Directory.Exists(tempExtractedModFolderPath))
            {
                // === Remainder of function (install and cleanup) ===
                string tempModDirPath = await Helper.FindFolderWithMpq(tempExtractedModFolderPath);
                string tempModDir = Path.GetFileName(tempModDirPath);
                string tempParentDir = Path.GetDirectoryName(tempModDirPath);
                string modName = string.Empty;

                if (tempModDir != null)
                    modName = tempModDir.Replace(".mpq", "");
                else
                {
                    MessageBox.Show("Mod download was unsuccessful", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string modInstallPath = Path.Combine(ShellViewModel.BaseModsFolder, modName);

                if (File.Exists(Path.Combine(ShellViewModel.SelectedModDataFolder, @"global\ui\layouts\bankexpansionlayouthd.json")))
                    File.Copy(Path.Combine(ShellViewModel.SelectedModDataFolder, @"global\ui\layouts\bankexpansionlayouthd.json"), Path.Combine(ShellViewModel.BaseModsFolder, "temp_bankexpansionlayouthd.json"), true);

                //Delete current Mod folder if it exists
                if (Directory.Exists(modInstallPath))
                {
                    if (File.Exists(Path.Combine(modInstallPath, $@"{modName}.mpq\MyUserSettings.json")))
                        File.Move(Path.Combine(modInstallPath, $@"{modName}.mpq\MyUserSettings.json"), Path.Combine(ShellViewModel.BaseModsFolder, "MyUserSettings.json"));
                    Directory.Delete(modInstallPath, true);
                }

                ProgressStatus = "Installing mod...";

                await Task.Run(async () =>
                {
                    await Helper.CloneDirectory(tempParentDir, modInstallPath);
                });

                string versionPath = Path.Combine(modInstallPath, "version.txt");

                if (!File.Exists(versionPath))
                    File.Create(versionPath).Close();

                string tempModInfoPath = Path.Combine(tempModDirPath, "modinfo.json");
                ModInfo modInfo = await Helper.ParseModInfo(tempModInfoPath);

                if (modInfo != null)
                    await File.WriteAllTextAsync(versionPath, modInfo.ModVersion);
                else
                    MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Clean up temp files
                if (Directory.Exists(tempExtractedModFolderPath))
                    Directory.Delete(tempExtractedModFolderPath, true);
                if (File.Exists(Path.Combine(ShellViewModel.BaseModsFolder, "MyUserSettings.json")))
                    File.Move(Path.Combine(ShellViewModel.BaseModsFolder, "MyUserSettings.json"), Path.Combine(modInstallPath, $@"{modName}.mpq\MyUserSettings.json"));
                ProgressStatus = "Installing Complete!";

                if (File.Exists(Path.Combine(ShellViewModel.BaseModsFolder, "temp_bankexpansionlayouthd.json")))
                {
                    File.Copy(Path.Combine(ShellViewModel.BaseModsFolder, "temp_bankexpansionlayouthd.json"), Path.Combine(ShellViewModel.SelectedModDataFolder, @"global\ui\layouts\bankexpansionlayouthd.json"), true);
                    File.Delete(Path.Combine(ShellViewModel.BaseModsFolder, "temp_bankexpansionlayouthd.json"));
                }

                MessageBox.Show($"{modName} has been installed!", "Mod Installed!", MessageBoxButton.OK, MessageBoxImage.None);

                // We installed a custom mod from a direct link 
                if (string.IsNullOrEmpty(SelectedMod.Key))
                    SelectedMod = new KeyValuePair<string, string>(modName, "DirectDownload");

                await TryCloseAsync(true);
            }
            else
            {
                ProgressStatus = "Install Complete!";
                MessageBox.Show($"TCP Base Files have been installed!", "Base Files Installed!", MessageBoxButton.OK, MessageBoxImage.None);

                // We installed a custom mod from a direct link 
                if (string.IsNullOrEmpty(SelectedMod.Key))
                    SelectedMod = new KeyValuePair<string, string>("TCP", "DirectDownload");

                await TryCloseAsync(true);
            }
            
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _logger.Error(ex);

            if (Directory.Exists(tempExtractedModFolderPath))
                Directory.Delete(tempExtractedModFolderPath, true);

            await TryCloseAsync(false);
        }
    }

    [UsedImplicitly]
    public async void OnModInstallSelectionChanged()
    {
        if (!string.IsNullOrEmpty(SelectedMod.Value))
        {
            if (SelectedMod.Key != "TCP Files (Install First)")
                ModDownloadLink = SelectedMod.Value;
        }
            
    }

    #endregion
}