using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using D2RLAN.Models;
using D2RLAN.ViewModels.Drawers;
using D2RLAN.Views.Drawers;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.NavigationDrawer;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLAN.ViewModels.Dialogs
{
    #region Metadata Class
    public class FilterMetadata
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string RepoPath { get; set; }
    }


    public static class LuaFilterParser
    {
        public static FilterMetadata ParseFilterMetadata(string filePath)
        {
            var metadata = new FilterMetadata
            {
                FilePath = filePath,
                Title = Path.GetFileNameWithoutExtension(filePath)
            };

            if (!File.Exists(filePath))
                return metadata;

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("--- Filter Title:"))
                    metadata.Title = line[17..].Trim();
                else if (line.StartsWith("--- Filter Type:"))
                    metadata.Type = line[16..].Trim();
                else if (line.StartsWith("--- Filter Description:"))
                    metadata.Description = line[23..].Trim();
                else if (line.StartsWith("--- Filter Link:"))
                    metadata.RepoPath = line[16..].Trim();
            }

            return metadata;
        }
    }




    #endregion

    public class LootFilterViewModel : Screen
    {
        #region ---Static Members---

        public ShellViewModel ShellViewModel { get; }

        private FilterMetadata _selectedFilter;
        public FilterMetadata SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    NotifyOfPropertyChange(() => SelectedFilter);
                    NotifyOfPropertyChange(() => SelectedFilterTitle);
                    NotifyOfPropertyChange(() => SelectedFilterType);
                    NotifyOfPropertyChange(() => SelectedFilterDescription);
                    _ = UpdateSelectedFilterAsync();
                }
            }
        }

        public string SelectedFilterTitle => SelectedFilter?.Title ?? "";
        public string SelectedFilterType => SelectedFilter?.Type ?? "";
        public string SelectedFilterDescription => SelectedFilter?.Description?.Replace("\\n", Environment.NewLine) ?? "";
        public ObservableCollection<FilterMetadata> FilterList { get; } = new ObservableCollection<FilterMetadata>();

        private int _selectedFilterIndex;
        public int SelectedFilterIndex
        {
            get => _selectedFilterIndex;
            set
            {
                if (_selectedFilterIndex != value)
                {
                    _selectedFilterIndex = value;
                    ShellViewModel.UserSettings.LootFilter = value;
                    NotifyOfPropertyChange(() => SelectedFilterIndex);
                    SaveSettingsAsync();
                }
            }
        }

        private string _lootFilterVersion;
        public string LootFilterVersion
        {
            get => _lootFilterVersion;
            set
            {
                if (_lootFilterVersion != value)
                {
                    _lootFilterVersion = value;
                    NotifyOfPropertyChange(() => LootFilterVersion);
                }
            }
        }

        private bool _filterUpdates;
        public bool FilterUpdates
        {
            get => _filterUpdates;
            set
            {
                if (_filterUpdates != value)
                {
                    _filterUpdates = value;
                    ShellViewModel.UserSettings.FilterUpdates = value;
                    NotifyOfPropertyChange(() => FilterUpdates);
                    SaveSettingsAsync();
                }
            }
        }

        private Task _saveTask = Task.CompletedTask;
        private void SaveSettingsAsync()
        {
            _saveTask = _saveTask.ContinueWith(async _ =>
            {
                try
                {
                    await ShellViewModel.SaveUserSettings();
                }
                catch (Exception ex)
                {
                    _logger.Error("Error saving user settings", ex);
                }
            }).Unwrap();
        }

        #endregion

        #region ---Window/Loaded Handlers---

        private ILog _logger = LogManager.GetLogger(typeof(LootFilterViewModel));

        public LootFilterViewModel(ShellViewModel shellViewModel)
        {
            DisplayName = "Active Loot Filter Settings";
            ShellViewModel = shellViewModel;

            InitializeAsync();
        }

        private bool _isInitialized = false;
        public async Task InitializeAsync()
        {
            if (_isInitialized) return; // Prevent double initialization
            _isInitialized = true;

            LoadFilterTitlesFromFolder(Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN\Filters"));

            // Move "No Filter" to the top
            if (FilterList != null)
            {
                var noFilter = FilterList.FirstOrDefault(f => f?.Title?.Equals("No Filter", StringComparison.OrdinalIgnoreCase) == true);
                if (noFilter != null)
                {
                    FilterList.Remove(noFilter);
                    FilterList.Insert(0, noFilter);
                }
            }

            SelectedFilterIndex = ShellViewModel.UserSettings.LootFilter;
            FilterUpdates = ShellViewModel.UserSettings.FilterUpdates;

            string lootFilterPath = Path.Combine(ShellViewModel.GamePath, "lootfilter.lua");
            string configPath = Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua");
            string configBlankPath = Path.Combine(Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN\Filters"), "lootfilter_config_blank.lua");
            string guidePath = Path.Combine(ShellViewModel.GamePath, "lootfilter_guide.pdf");
            string remoteLootFilterUrl = "https://drive.google.com/uc?export=download&id=157sEJn8LSpNWwlwuEnrSPo23UULzJmNs";
            string guideUrl = "https://drive.google.com/uc?export=download&id=1Rtypc8FRRn14rNtTpeXGEEYXaUoiV5lb";
            string remoteVersion = "0.0.0";
            string currentVersion = null;
            bool shouldReplace = true;
            string tempLootFilterPath = Path.GetTempFileName();
            using var client = new HttpClient();

            try
            {
                // --- Get remote version ---
                await DownloadFileAsync(client, remoteLootFilterUrl, tempLootFilterPath);

                var tempVersionLine = File.ReadLines(tempLootFilterPath).FirstOrDefault(line => Regex.IsMatch(line, @"local\s+version\s*=\s*""([^""]+)"""));
                if (!string.IsNullOrWhiteSpace(tempVersionLine))
                {
                    var match = Regex.Match(tempVersionLine, @"local\s+version\s*=\s*""([^""]+)""");
                    if (match.Success)
                        remoteVersion = match.Groups[1].Value;
                }

                // --- Get local version (if exists) ---
                if (File.Exists(lootFilterPath))
                {
                    var currentVersionLine = File.ReadLines(lootFilterPath)
                        .FirstOrDefault(line => Regex.IsMatch(line, @"local\s+version\s*=\s*""([^""]+)"""));
                    if (!string.IsNullOrWhiteSpace(currentVersionLine))
                    {
                        var match = Regex.Match(currentVersionLine, @"local\s+version\s*=\s*""([^""]+)""");
                        if (match.Success)
                        {
                            currentVersion = match.Groups[1].Value;
                            if (currentVersion == remoteVersion)
                                shouldReplace = false;
                        }
                    }
                }

                // --- Update label binding ---
                if (currentVersion != null)
                    LootFilterVersion = currentVersion;
                else
                    LootFilterVersion = "N/A";
            }
            catch (Exception ex)
            {
                LootFilterVersion = "Loot Filter version check failed";
                MessageBox.Show($"Failed to check remote loot filter version:\n{ex.Message}", "Version Check Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (!File.Exists(configBlankPath))
                shouldReplace = true;

            if (shouldReplace)
            {
                string filterFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN\Filters");
                Directory.CreateDirectory(filterFolder);

                try
                {
                    await DownloadFileAsync(client, remoteLootFilterUrl, lootFilterPath);
                    await DownloadFileAsync(client, guideUrl, guidePath);
                    await File.WriteAllBytesAsync(configBlankPath, Helper.GetResourceByteArray2("lootfilter_config_blank.lua"));

                    // since we replaced, update LootFilterVersion
                    LootFilterVersion = remoteVersion;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to download one or more loot filter files:\n{ex.Message}", "Download Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                FilterMetadata copiedFilterMetadata = null;
                try
                {
                    copiedFilterMetadata = LuaFilterParser.ParseFilterMetadata(configBlankPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to parse metadata:\n" + ex.Message);
                }

                LoadFilterTitlesFromFolder(filterFolder);

                if (!string.IsNullOrWhiteSpace(copiedFilterMetadata?.Title))
                {
                    var match = FilterList.FirstOrDefault(f => f?.Title?.Equals(copiedFilterMetadata.Title, StringComparison.OrdinalIgnoreCase) == true);
                    if (match != null)
                        SelectedFilter = match;
                }
            }

            try
            {
                if (File.Exists(tempLootFilterPath))
                    File.Delete(tempLootFilterPath);
            }
            catch { }
        }

        private static async Task DownloadFileAsync(HttpClient client, string url, string destinationPath)
        {
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var file = File.Create(destinationPath);
            await stream.CopyToAsync(file);
        }


        #endregion

        #region ---Filter Loading---

        private void LoadFilterTitlesFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return;

            FilterList.Clear();

            var luaFiles = Directory.GetFiles(folderPath, "*.lua");

            foreach (var file in luaFiles)
            {
                if (Path.GetFileName(file).Equals("lootfilter_config.lua", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    var metadata = LuaFilterParser.ParseFilterMetadata(file);
                    if (!string.IsNullOrWhiteSpace(metadata?.Title))
                    {
                        metadata.FilePath = file;
                        FilterList.Add(metadata);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public async Task OnLoadFilter()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Lua files (*.lua)|*.lua",
                Title = "Select a Loot Filter File",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(selectedFilePath);
                string renamedFileName = fileName.Equals("lootfilter_config.lua", StringComparison.OrdinalIgnoreCase) ? "custom_lootfilter_config.lua" : fileName;
                string filterFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN\Filters");
                Directory.CreateDirectory(filterFolder);
                string destinationPath = Path.Combine(filterFolder, renamedFileName);
                File.Copy(selectedFilePath, destinationPath, true);
                FilterMetadata copiedFilterMetadata = null;

                try
                {
                    copiedFilterMetadata = LuaFilterParser.ParseFilterMetadata(destinationPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to parse metadata:\n" + ex.Message);
                }

                LoadFilterTitlesFromFolder(filterFolder);

                if (!string.IsNullOrWhiteSpace(copiedFilterMetadata?.Title))
                {
                    var match = FilterList.FirstOrDefault(f => f?.Title?.Equals(copiedFilterMetadata.Title, StringComparison.OrdinalIgnoreCase) == true);
                    if (match != null)
                        SelectedFilter = match;
                }

                MessageBox.Show($"The specified loot filter:\n{selectedFilePath}\nhas been loaded!", "Filter Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void OnApplyFilter()
        {
            if (SelectedFilter == null)
            {
                MessageBox.Show("Please select a filter before applying.", "No Filter Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(SelectedFilter.FilePath) && File.Exists(SelectedFilter.FilePath))
                {
                    string destinationPath = Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua");
                    File.Copy(SelectedFilter.FilePath, destinationPath, true);

                    MessageBox.Show($"\"{SelectedFilter.Title}\" has been applied!", "Filter Applied", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to apply selected filter:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void OnOpenFilter()
        {
            var path = Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua");

            if (!File.Exists(path))
            {
                MessageBox.Show($"{ShellViewModel.SelectedModDataFolder} Directory does not exist!");
                return;
            }

            // Common install locations for VS Code
            string[] possiblePaths =
            {
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Microsoft VS Code\Code.exe"),
        @"C:\Program Files\Microsoft VS Code\Code.exe",
        @"C:\Program Files (x86)\Microsoft VS Code\Code.exe"
    };

            string? codeExe = possiblePaths.FirstOrDefault(File.Exists);

            ProcessStartInfo startInfo;

            if (codeExe != null)
            {
                // Launch VS Code directly
                startInfo = new ProcessStartInfo
                {
                    FileName = codeExe,
                    Arguments = $"\"{path}\"",
                    UseShellExecute = false
                };
            }
            else
            {
                // Fallback: open with default app (likely VS Code if user associated it)
                startInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{path}\"",
                    UseShellExecute = true
                };
            }

            Process.Start(startInfo);
        }


        private async Task UpdateSelectedFilterAsync()
        {
            if (SelectedFilter == null)
            {
                _logger.Warn("Update skipped: SelectedFilter is null.");
                return;
            }

            // Remember the old title
            string oldFilterTitle = SelectedFilter.Title ?? "Unknown";
            string repoUrl = SelectedFilter.RepoPath;
            string localFilterPath = SelectedFilter.FilePath;

            if (string.IsNullOrWhiteSpace(repoUrl))
            {
                _logger.Warn($"Skipped update for '{oldFilterTitle}' — no RepoPath defined.");
                return;
            }

            if (string.IsNullOrWhiteSpace(localFilterPath) || !File.Exists(localFilterPath))
            {
                string filterFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN\Filters");
                Directory.CreateDirectory(filterFolder);
                localFilterPath = Path.Combine(filterFolder, Path.GetFileName(new Uri(repoUrl).LocalPath));
                SelectedFilter.FilePath = localFilterPath; // fix lost mapping
            }

            using var client = new HttpClient();

            try
            {
                // Download remote filter to temp file
                string tempPath = Path.GetTempFileName();
                await DownloadFileAsync(client, repoUrl, tempPath);

                bool shouldReplace = true;

                if (File.Exists(localFilterPath))
                {
                    shouldReplace = !await FilesAreEqualAsync(localFilterPath, tempPath);
                }

                if (!shouldReplace)
                {
                    File.Delete(tempPath);
                    _logger.Info($"'{oldFilterTitle}' is already up to date.");
                    return;
                }

                File.Copy(tempPath, localFilterPath, overwrite: true);
                File.Delete(tempPath);

                _logger.Info($"'{oldFilterTitle}' successfully updated from remote.");

                if (ShellViewModel.UserSettings.FilterUpdates)
                {
                    string gameConfigPath = Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua");
                    File.Copy(localFilterPath, gameConfigPath, overwrite: true);
                    _logger.Info($"'{oldFilterTitle}' applied automatically to game folder.");
                }

                // Refresh UI: reload all filters from folder
                LoadFilterTitlesFromFolder(Path.GetDirectoryName(localFilterPath));

                // Try exact match first
                var matchFilter = FilterList.FirstOrDefault(f =>
                    f?.Title?.Equals(oldFilterTitle, StringComparison.OrdinalIgnoreCase) == true);

                if (matchFilter == null)
                {
                    // Fuzzy match
                    string baseName = Regex.Replace(oldFilterTitle, @"[_\- ]?v?\d+(\.\d+)?[a-zA-Z]*$", "", RegexOptions.IgnoreCase);

                    matchFilter = FilterList
                        .OrderByDescending(f => f?.Title?.Length ?? 0) // pick longest matching name (usually newest)
                        .FirstOrDefault(f => f?.Title?.IndexOf(baseName, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (matchFilter != null)
                {
                    SelectedFilter = matchFilter;
                    _logger.Info($"Reselected updated filter: {matchFilter.Title}");
                }
                else
                {
                    _logger.Warn($"Could not find updated filter similar to '{oldFilterTitle}' after update.");
                }

                if (!ShellViewModel.UserSettings.FilterUpdates)
                {
                    MessageBox.Show($"An update for '{oldFilterTitle}' was downloaded.\nPress Apply to activate it.", "Filter Update Available", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating filter '{oldFilterTitle}': {ex}");
                MessageBox.Show($"Failed to update filter '{oldFilterTitle}':\n{ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private static async Task<bool> FilesAreEqualAsync(string path1, string path2)
        {
            using var hash = System.Security.Cryptography.SHA256.Create();

            await using var fs1 = File.OpenRead(path1);
            await using var fs2 = File.OpenRead(path2);

            byte[] hash1 = await hash.ComputeHashAsync(fs1);
            byte[] hash2 = await hash.ComputeHashAsync(fs2);

            return hash1.SequenceEqual(hash2);
        }

        public async Task CheckForUpdatesAsync()
        {
            FilterMetadata copiedFilterMetadata = null;
            copiedFilterMetadata = LuaFilterParser.ParseFilterMetadata(Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua"));


            if (FilterUpdates)
            {
                var match = FilterList.FirstOrDefault(f => f?.Title?.Equals(copiedFilterMetadata.Title, StringComparison.OrdinalIgnoreCase) == true);
                if (match != null)
                    SelectedFilter = match;

                await InitializeAsync();
            }
        }

        #endregion
    }
}
