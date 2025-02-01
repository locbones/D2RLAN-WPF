using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using CASCLibNET;
using D2RLAN.Properties;
using JetBrains.Annotations;

namespace D2RLAN.ViewModels.Dialogs;

public class CASCExtractorViewModel : Screen
{
    //This feature is largely unused due to the debug nature of the build
    #region ---Static Members---

    private string _diskSpaceString;
    private ObservableCollection<string> _installedMods = new ObservableCollection<string>();
    private string _selectedMod;
    private double _downloadProgress;
    private bool _progressBarIsIndeterminate;
    private string _downloadProgressString;
    private string _totalFiles;
    private string _filesLeft;
    private string _extracted;
    private string _fileName;
    private string _status;

    #endregion

    #region ---Window/Loaded Handlers---

    public CASCExtractorViewModel(ShellViewModel shellViewModel)
    {
        ShellViewModel = shellViewModel;
        DisplayName = "CASC Extractor";

        DriveInfo driveInfo = new(Path.GetPathRoot(ShellViewModel.GamePath));

        if (driveInfo.IsReady)
        {
            long availableSpaceBytes = driveInfo.AvailableFreeSpace;
            double availableSpace = availableSpaceBytes / (1024.0 * 1024.0) / 1024.0;

            DiskSpaceString = $"(You have {availableSpace:F0} GB of free space available)";
        }

        string[] baseFolders = Directory.GetDirectories(ShellViewModel.BaseModsFolder);

        InstalledMods.Add("Bin Generation Only");

        foreach (string folder in baseFolders)
        {
            if (!folder.Contains("Backup"))
                InstalledMods.Add(Path.GetFileName(folder));
        }

        SelectedMod = Settings.Default.SelectedMod;
    }
    public CASCExtractorViewModel()
    {
        if (Execute.InDesignMode)
        {
            DownloadProgressString = "70%";
            DiskSpaceString = "(You have 123 GB available on the drive where your game is installed)";
        }
    }

    #endregion

    #region ---Properties---

    public string FileName
    {
        get => _fileName;
        set
        {
            if (value == _fileName) return;
            _fileName = value;
            NotifyOfPropertyChange();
        }
    }
    public string TotalFiles
    {
        get => _totalFiles;
        set
        {
            if (value == _totalFiles) return;
            _totalFiles = value;
            NotifyOfPropertyChange();
        }
    }
    public string FilesLeft
    {
        get => _filesLeft;
        set
        {
            if (value == _filesLeft) return;
            _filesLeft = value;
            NotifyOfPropertyChange();
        }
    }
    public string Extracted
    {
        get => _extracted;
        set
        {
            if (value == _extracted) return;
            _extracted = value;
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
    public string SelectedMod
    {
        get => _selectedMod;
        set
        {
            if (value == _selectedMod)
            {
                return;
            }
            _selectedMod = value;
            NotifyOfPropertyChange();
        }
    }
    public ObservableCollection<string> InstalledMods
    {
        get => _installedMods;
        set
        {
            if (Equals(value, _installedMods))
            {
                return;
            }
            _installedMods = value;
            NotifyOfPropertyChange();
        }
    }
    public ShellViewModel ShellViewModel { get; }
    public string DiskSpaceString
    {
        get => _diskSpaceString;
        set
        {
            if (value == _diskSpaceString)
            {
                return;
            }
            _diskSpaceString = value;
            NotifyOfPropertyChange();
        }
    }
    public string Status
    {
        get => _status;
        set
        {
            if (value == _status)
            {
                return;
            }
            _status = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    #region ---CASC Extraction---

    [UsedImplicitly]
    public async void OnExtract()
    {
        string extractionDirectory = ShellViewModel.GamePath;

        if (SelectedMod == "Bin Generation Only")
        {
            extractionDirectory = ShellViewModel.GamePath;
        }
        else
        {
            Status = "Backing up mod files...";

            extractionDirectory = System.IO.Path.Combine(ShellViewModel.BaseModsFolder, SelectedMod, $"{SelectedMod}.mpq");
            string[] modFiles = Directory.GetFiles(System.IO.Path.Combine(extractionDirectory, "data"), "*.*", SearchOption.AllDirectories);

            foreach (string file in modFiles)
            {
                string relativePath = Path.GetRelativePath(System.IO.Path.Combine(extractionDirectory, "data"), file);
                string destinationFilePath = System.IO.Path.Combine(System.IO.Path.Combine(extractionDirectory, "data_temp"), relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                await Task.Run(() => File.Copy(file, destinationFilePath, true));

                string fileNameFormatted = file.Replace($"{System.IO.Path.Combine(extractionDirectory, @"data\")}", "");

                await Execute.OnUIThreadAsync(() =>
                {
                    FileName = fileNameFormatted;
                    return Task.CompletedTask;
                });

            }
        }

        Status = "Performing CASC Extraction...";
        using CASCStorage storage = new CASCStorage(ShellViewModel.GamePath);
        int extractedFiles = 0;
        long totalSize = 0;
        int totalFiles = storage.Files.Count(file => !file.FileName.Contains("locales") && !file.FileName.Contains("binaries") && Path.HasExtension(file.FileName));
        TotalFiles = totalFiles.ToString();

        // Iterate through all files in the storage asynchronously
        foreach (CASCFileInfo file in storage.Files)
        {
            // Check if the file path contains "locales" or "binaries," or if it doesn't have an extension; skip it if any condition is true
            if (file.FileName.Contains("locales") || file.FileName.Contains("binaries") || !Path.HasExtension(file.FileName))
                continue;

            string modifiedFileName = file.FileName.Replace("data:data", "Data");
            string extractedFilePath = System.IO.Path.Combine(extractionDirectory, modifiedFileName);
            string extractedFileDirectory = Path.GetDirectoryName(extractedFilePath);

            if (!Directory.Exists(extractedFileDirectory))
                Directory.CreateDirectory(extractedFileDirectory);

            await using (CASCFileStream reader = storage.OpenFile(file.FileName))
            {
                await using FileStream writer = new FileStream(extractedFilePath, FileMode.Create);
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await writer.WriteAsync(buffer, 0, bytesRead);
                    totalSize += bytesRead; // Increment the total size as bytes are read and written
                }
            }

            double totalSizeGB = (double)totalSize / 1073741824.0;

            // Update labels and progress bar on the UI thread
            await Execute.OnUIThreadAsync(() =>
            {
                extractedFiles++;
                FilesLeft = (totalFiles - extractedFiles).ToString();
                FileName = modifiedFileName;
                Extracted = $"{totalSizeGB:F2} GB / 40.41 GB"; // Format to X.XX GB

                // Update progress bar
                DownloadProgress = (int)((totalSizeGB / 40.41) * 100);
                DownloadProgressString = Math.Round(DownloadProgress).ToString(CultureInfo.InvariantCulture) + "%";

                return Task.CompletedTask;
            });
        }


        //Extraction Complete - Trans
        if (totalFiles - extractedFiles == 0)
        {
            if (SelectedMod != "Bin Generation Only")
            {
                Status = "Restoring mod files to mod folder...";
                extractionDirectory = System.IO.Path.Combine(ShellViewModel.BaseModsFolder, SelectedMod, $"{SelectedMod}.mpq");
                string[] modFiles = Directory.GetFiles(System.IO.Path.Combine(extractionDirectory, "data_temp"), "*.*", SearchOption.AllDirectories);

                foreach (string file in modFiles)
                {
                    string relativePath = Path.GetRelativePath(System.IO.Path.Combine(extractionDirectory, "data_temp"), file);
                    string destinationFilePath = System.IO.Path.Combine(System.IO.Path.Combine(extractionDirectory, "data"), relativePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                    await Task.Run(() => File.Copy(file, destinationFilePath, true));

                    string fileNameFormatted = file.Replace($"{System.IO.Path.Combine(extractionDirectory, @"data_temp\")}", "");

                    await Execute.OnUIThreadAsync(async () =>
                    {
                        FileName = fileNameFormatted;
                        await Task.CompletedTask;
                    });

                }

                await Execute.OnUIThreadAsync(async () =>
                {
                    FileName = "";
                    FilesLeft = "";
                    Extracted = "";
                    DownloadProgress = 0;
                    DownloadProgressString = "";
                    await Task.CompletedTask;
                });

                Directory.Delete(System.IO.Path.Combine(extractionDirectory, "data_temp"), true);
                Status = "Extraction Process Complete! Now ready to Play";
                ShellViewModel.UserSettings.FastLoad = "On";
                await this.TryCloseAsync();
            }
        }

    }
    [UsedImplicitly]
    public async Task OnUpdate()
    {
        string sourceDirectory = System.IO.Path.Combine(ShellViewModel.BaseModsFolder, SelectedMod, $"{SelectedMod}.mpq/data");
        string destinationDirectory = System.IO.Path.Combine(ShellViewModel.GamePath, "data");

        try
        {
            if (Directory.Exists(sourceDirectory))
            {
                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                string[] files = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    string destinationFilePath = file.Replace(sourceDirectory, destinationDirectory);
                    string destinationFileDirectory = Path.GetDirectoryName(destinationFilePath);

                    if (!Directory.Exists(destinationFileDirectory))
                        Directory.CreateDirectory(destinationFileDirectory);

                    File.Copy(file, destinationFilePath, true);
                }
            }
            MessageBox.Show("All processes completed successfully!\n\nYou may now use the -direct -txt option to generate bins or to speed up loading times");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    [UsedImplicitly]
    public async void OnCascHelp()
    {
        MessageBox.Show("D2RLAN will first backup your mod's data, then extract CASC Data.\nAfter CASC Data has been extracted; it will restore your mod files.\n\nIf you wish to generate bins for your mod, please choose the 'Bin Generation Only' option, then you may run the game using -direct -txt.", "WARNING!!");
    }

    #endregion
}