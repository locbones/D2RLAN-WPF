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
        if (!File.Exists(ShellViewModel.GamePath + "/D2R_Installer.exe"))
        {
            Mods.Clear();

            var tcpEntry = new KeyValuePair<string, string>("TCP Files (Install First)", "https://www.dropbox.com/scl/fi/qbvgssix2s2jrlnq5jxzc/0828_270idx.zip?rlkey=fclmo9v31do993d933s5yifhk&st=lrcsoo1z&dl=1");
            Mods.Add(tcpEntry);

            // Assign as the first and only selection
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
        string tempFile = Path.Combine(tempPath, "NewModDownload.zip");
        string tempExtractedModFolderPath = Path.Combine(tempPath, "NewModDownload");
        SevenZipExtractor.SetLibraryPath("7z.dll");

      //  if (tempFile.Contains(".zip"))
           // tempFile = "NewModDownload.7z";

        try
        {
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(30);

            // Get file size from headers
            var response = await client.GetAsync(SelectedMod.Value, HttpCompletionOption.ResponseHeadersRead);
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

                if (contentLength > 0)
                {
                    double progress = (double)totalRead / contentLength * 100.0;

                    // Speed (bytes/sec)
                    double speed = totalRead / sw.Elapsed.TotalSeconds;
                    string speedStr = $"{speed / 1024d / 1024d:0.00} MB/s";

                    // Time remaining
                    double remainingSeconds = (contentLength - totalRead) / speed;
                    string timeRemaining = remainingSeconds > 0
                        ? $"{TimeSpan.FromSeconds(remainingSeconds):mm\\:ss}"
                        : "--:--";

                    // Update UI
                    Execute.OnUIThread(() =>
                    {
                        DownloadProgress = Math.Round(progress, MidpointRounding.AwayFromZero);
                        DownloadProgressString =
                            $"{DownloadProgress}%  " +
                            $"({totalRead / 1024d / 1024d:0} / {contentLength / 1024d / 1024d:0} MB)  " +
                            $"{speedStr}  ETA: {timeRemaining}";
                    });
                }
            }

            file.Close();
            client.Dispose();
            sw.Stop();

            ProgressStatus = "Extracting mod...";
            DownloadProgressString = string.Empty;
            ProgressBarIsIndeterminate = true;

            if (Directory.Exists(tempExtractedModFolderPath))
                Directory.Delete(tempExtractedModFolderPath, true);

            await Task.Run(() =>
            {
                if (tempFile.Contains(".zip"))
                    ZipFile.ExtractToDirectory(tempFile, tempExtractedModFolderPath, true);
                else
                {
                    using (var extractor = new SevenZipExtractor(tempFile))
                    {
                        extractor.ExtractArchive(tempExtractedModFolderPath);
                    }
                }
                return Task.CompletedTask;
            });

            string tempModDirPath = await Helper.FindFolderWithMpq(tempExtractedModFolderPath);
            string tempModDir = Path.GetFileName(tempModDirPath);
            string tempParentDir = Path.GetDirectoryName(tempModDirPath);
            string modName = string.Empty;

            if (tempModDir.Replace(".mpq", "") == "TCP" && Directory.Exists(ShellViewModel.GamePath + "data"))
            {
                Process.Start(ShellViewModel.GamePath + "/D2R_Installer.exe");
                MessageBox.Show("Previous game files detected, restarting the installer...");

                if (File.Exists(tempFile))
                    File.Delete(tempFile);
                if (Directory.Exists(tempExtractedModFolderPath))
                    Directory.Delete(tempExtractedModFolderPath, true);

                await TryCloseAsync(true);
                return;
            }

            if (tempModDir != null)
                modName = tempModDir.Replace(".mpq", "");
            else
            {
                MessageBox.Show("Mod download was unsuccessful", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string modInstallPath = Path.Combine(ShellViewModel.BaseModsFolder, modName);


            if (File.Exists(ShellViewModel.SelectedModDataFolder + @"\global\ui\layouts\bankexpansionlayouthd.json"))
                File.Copy(ShellViewModel.SelectedModDataFolder + @"\global\ui\layouts\bankexpansionlayouthd.json", ShellViewModel.BaseModsFolder + "temp_bankexpansionlayouthd.json", true);

            //Delete current Mod folder if it exists
            if (Directory.Exists(modInstallPath))
            {
                if (File.Exists(Path.Combine(modInstallPath, $@"{modName}.mpq\MyUserSettings.json")))
                    File.Move(Path.Combine(modInstallPath, $@"{modName}.mpq\MyUserSettings.json"), Path.Combine(ShellViewModel.BaseModsFolder, "MyUserSettings.json"));
                Directory.Delete(modInstallPath, true);
            }

            ProgressStatus = "Installing mod...";

            if (modName == "TCP")
            {
                await Task.Run(async () =>
                {
                    await Helper.CloneDirectory(tempExtractedModFolderPath, ShellViewModel.GamePath);
                });
            }
            else
            {
                await Task.Run(async () =>
                {
                    await Helper.CloneDirectory(tempParentDir, modInstallPath);
                });
            }

            string versionPath = Path.Combine(modInstallPath, "version.txt");

            if (!File.Exists(versionPath))
                File.Create(versionPath).Close();

            string tempModInfoPath = Path.Combine(tempModDirPath, "modinfo.json");

            ModInfo modInfo = await Helper.ParseModInfo(tempModInfoPath);

            if (modInfo != null)
                await File.WriteAllTextAsync(versionPath, modInfo.ModVersion);
            else
                MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            //Always clean up temp files.
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            if (Directory.Exists(tempExtractedModFolderPath))
                Directory.Delete(tempExtractedModFolderPath, true);
            if (File.Exists(Path.Combine(ShellViewModel.BaseModsFolder, "MyUserSettings.json")))
                File.Move(Path.Combine(ShellViewModel.BaseModsFolder, "MyUserSettings.json"), Path.Combine(modInstallPath, $@"{modName}.mpq\MyUserSettings.json"));
            ProgressStatus = "Installing Complete!";

            if (File.Exists(ShellViewModel.BaseModsFolder + "temp_bankexpansionlayouthd.json"))
            {
                File.Copy(ShellViewModel.BaseModsFolder + "temp_bankexpansionlayouthd.json", ShellViewModel.SelectedModDataFolder + @"\global\ui\layouts\bankexpansionlayouthd.json", true);
                File.Delete(ShellViewModel.BaseModsFolder + "temp_bankexpansionlayouthd.json");
            }

            MessageBox.Show($"{modName} has been installed!", "Mod Installed!", MessageBoxButton.OK, MessageBoxImage.None);


            //We installed a custom mod from a direct link. 
            if (string.IsNullOrEmpty(SelectedMod.Key))
                SelectedMod = new KeyValuePair<string, string>(modName, "DirectDownload");

            await TryCloseAsync(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _logger.Error(ex);

            //Always clean up temp files.
            if (File.Exists(tempFile))
                File.Delete(tempFile);
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
    void CleanupTempFiles(string filePath, string dirPath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        if (Directory.Exists(dirPath))
            Directory.Delete(dirPath, true);
    }

    #endregion
}