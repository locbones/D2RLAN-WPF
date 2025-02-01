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
                System.Windows.MessageBox.Show("The number of items in column D does not match the number of items in column G.\nPlease notify an admin.", "Column Mismatch!", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error("The number of items in column D does not match the number of items in column G.");
                return;
            }

            for (int i = 0; i < dValues.Count; i++)
            {
                Mods.Add(new KeyValuePair<string, string>(dValues[i][0].ToString(), gValues[i][0].ToString()));
            }

        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _logger.Error(ex);
        }
    }
    [UsedImplicitly]
    public async void OnInstallMod()
    {
        ModDownloadLink = ModDownloadLink.TrimEnd();
        string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "NewModDownload.zip");
        string tempExtractedModFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "NewModDownload");

        if (File.Exists(ShellViewModel.GamePath + "D2R_Installer.exe") && File.Exists(ShellViewModel.GamePath + "data/data/data.027"))
        {
            try
            {
                var progress = new Progress<double>(value =>
                {
                    Execute.OnUIThread(() =>
                    {
                        if (value == -1)
                        {
                            DownloadProgress = 0;
                            DownloadProgressString = string.Empty;
                            ProgressBarIsIndeterminate = true;
                        }
                        else
                        {
                            DownloadProgress = Math.Round(value, MidpointRounding.AwayFromZero);
                            DownloadProgressString = $"{DownloadProgress}%";
                        }
                    });
                });

                using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
                ProgressStatus = "Downloading mod...";

                await Execute.OnUIThreadAsync(async () =>
                {
                    await using var file = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
                    await client.DownloadAsync(ModDownloadLink, file, progress, CancellationToken.None);
                });

                ProgressStatus = "Extracting mod...";
                ProgressBarIsIndeterminate = true;

                if (Directory.Exists(tempExtractedModFolderPath))
                    Directory.Delete(tempExtractedModFolderPath, true);

                await Task.Run(() => ZipFile.ExtractToDirectory(tempFile, tempExtractedModFolderPath, true));

                string tempModDirPath = await Helper.FindFolderWithMpq(tempExtractedModFolderPath);
                string modName = System.IO.Path.GetFileName(tempModDirPath)?.Replace(".mpq", "") ?? string.Empty;

                if (string.IsNullOrEmpty(modName))
                {
                    System.Windows.MessageBox.Show("Mod download was unsuccessful", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string modInstallPath = System.IO.Path.Combine(ShellViewModel.BaseModsFolder, modName);

                if (Directory.Exists(modInstallPath))
                    Directory.Delete(modInstallPath, true);

                ProgressStatus = "Installing mod...";
                await Task.Run(async () => await Helper.CloneDirectory(System.IO.Path.GetDirectoryName(tempModDirPath), modInstallPath));

                string versionPath = System.IO.Path.Combine(modInstallPath, "version.txt");
                if (!File.Exists(versionPath))
                    File.Create(versionPath).Close();

                var modInfo = await Helper.ParseModInfo(System.IO.Path.Combine(tempModDirPath, "modinfo.json"));
                if (modInfo != null)
                    await File.WriteAllTextAsync(versionPath, modInfo.ModVersion);
                else
                    System.Windows.MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                CleanupTempFiles(tempFile, tempExtractedModFolderPath);

                ProgressStatus = "Installation Complete!";
                System.Windows.MessageBox.Show($"{modName} has been installed!", "Mod Installed!", MessageBoxButton.OK, MessageBoxImage.None);

                if (string.IsNullOrEmpty(SelectedMod.Key))
                    SelectedMod = new KeyValuePair<string, string>(modName, "DirectDownload");

                await TryCloseAsync(true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(ex);
                CleanupTempFiles(tempFile, tempExtractedModFolderPath);
                await TryCloseAsync(false);
            }

        }
        else if (!File.Exists(ShellViewModel.GamePath + "D2R_Installer.exe") && !File.Exists(ShellViewModel.GamePath + "data/data/data.027"))
        {
            try
            {
                var progress = new Progress<double>(value =>
                {
                    Execute.OnUIThread(() =>
                    {
                        if (value == -1)
                        {
                            DownloadProgress = 0;
                            DownloadProgressString = string.Empty;
                            ProgressBarIsIndeterminate = true;
                        }
                        else
                        {
                            DownloadProgress = Math.Round(value, MidpointRounding.AwayFromZero);
                            DownloadProgressString = $"{DownloadProgress}%";
                        }
                    });
                });

                using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
                ProgressStatus = "Downloading core files...";

                await Execute.OnUIThreadAsync(async () =>
                {
                    await using var file = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
                    await client.DownloadAsync(ModDownloadLink, file, progress, CancellationToken.None);
                });

                ProgressStatus = "Extracting core files...";
                ProgressBarIsIndeterminate = true;

                if (Directory.Exists(tempExtractedModFolderPath))
                    Directory.Delete(tempExtractedModFolderPath, true);

                await Task.Run(() => ZipFile.ExtractToDirectory(tempFile, tempExtractedModFolderPath, true));
                string tempModDirPath = await Helper.FindFolderWithMpq(tempExtractedModFolderPath);
                string modName = System.IO.Path.GetFileName(tempModDirPath)?.Replace(".mpq", "") ?? string.Empty;

                if (string.IsNullOrEmpty(modName))
                {
                    System.Windows.MessageBox.Show("Core files download was unsuccessful", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string modInstallPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                ProgressStatus = "Extracting core files...";
                await Task.Run(async () => await Helper.CloneDirectory(tempExtractedModFolderPath, modInstallPath));
                CleanupTempFiles(tempFile, tempExtractedModFolderPath);
                ProgressStatus = "Download Complete!";
                System.Windows.MessageBox.Show($"Core file package has been extracted!\nNow Proceeding with game file download...\n\nWarning: This process may take some time to complete\nRestarting download from launcher will resume download if needed", "Mod Installed!", MessageBoxButton.OK, MessageBoxImage.None);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = ShellViewModel.GamePath + "D2R_Installer.exe",
                    WorkingDirectory = ShellViewModel.GamePath
                };
                Process.Start(psi);

                if (string.IsNullOrEmpty(SelectedMod.Key))
                    SelectedMod = new KeyValuePair<string, string>(modName, "DirectDownload");

                await TryCloseAsync(true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(ex);
                CleanupTempFiles(tempFile, tempExtractedModFolderPath);
                await TryCloseAsync(false);
            }
        }
        else
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = ShellViewModel.GamePath + "D2R_Installer.exe",
                WorkingDirectory = ShellViewModel.GamePath
            };
            Process.Start(psi);
        }
    }
    [UsedImplicitly]
    public async void OnModInstallSelectionChanged()
    {
        if (!string.IsNullOrEmpty(SelectedMod.Value))
            ModDownloadLink = SelectedMod.Value;
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