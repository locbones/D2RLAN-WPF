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
        string tempPath = Path.GetTempPath();
        string tempFile = Path.Combine(tempPath, "NewModDownload.zip");
        string tempExtractedModFolderPath = Path.Combine(tempPath, "NewModDownload");
        SevenZipExtractor.SetLibraryPath("7z.dll");

        if (tempFile.Contains(".zip"))
            tempFile = "NewModDownload.7z";

        try
        {
            Progress<double> progress = new Progress<double>();

            progress.ProgressChanged += (sender, args) =>
            {
                Execute.OnUIThread(() =>
                {
                    if (args == -1)
                    {
                        DownloadProgress = 0;
                        DownloadProgressString = string.Empty;
                        ProgressBarIsIndeterminate = true;
                    }
                    else
                    {
                        DownloadProgress = Math.Round(args, MidpointRounding.AwayFromZero);
                        DownloadProgressString = $"{DownloadProgress}%";
                    }
                });
            };

            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            await using FileStream file = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
            ProgressStatus = "Downloading mod...";

            await Execute.OnUIThreadAsync(async () =>
            {
                await client.DownloadAsync(ModDownloadLink, file, progress, CancellationToken.None);
            });

            file.Close();
            client.Dispose();

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

            if (modName == "TCP")
            {
                MessageBox.Show($"Base Files have been downloaded!\nGame Installer will launch in a few seconds...", "Mod Installed!", MessageBoxButton.OK, MessageBoxImage.None);
                Process.Start(ShellViewModel.GamePath + "/D2R_Installer.exe");
            }
            else
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