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
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Lua file not found.", filePath);

            string[] lines = File.ReadAllLines(filePath);
            var metadata = new FilterMetadata();

            foreach (var line in lines)
            {
                if (line.StartsWith("--- Filter Title:"))
                    metadata.Title = line.Substring(17).Trim();
                else if (line.StartsWith("--- Filter Type:"))
                    metadata.Type = line.Substring(16).Trim();
                else if (line.StartsWith("--- Filter Description:"))
                    metadata.Description = line.Substring(23).Trim();
                else if (line.StartsWith("--- Filter Link:"))
                    metadata.RepoPath = line.Substring(16).Trim();

                if (!string.IsNullOrEmpty(metadata.Title) &&
                    !string.IsNullOrEmpty(metadata.Type) &&
                    !string.IsNullOrEmpty(metadata.Description) &&
                    !string.IsNullOrEmpty(metadata.RepoPath))
                {
                    break;
                }
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
                    NotifyOfPropertyChange(() => FilterUpdates);
                }
            }
        }

        private async void SaveSettingsAsync()
        {
            await ShellViewModel.SaveUserSettings();
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

        private async Task DownloadFileAsync(HttpClient client, string url, string destinationPath)
        {
            var bytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(destinationPath, bytes);
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

            if (File.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            }
            else
                MessageBox.Show($"{ShellViewModel.SelectedModDataFolder} Directory does not exist!");
        }

        private async Task UpdateSelectedFilterAsync()
        {
            if (SelectedFilter == null || string.IsNullOrWhiteSpace(SelectedFilter.RepoPath))
                return;

            string filterFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN\Filters");
            Directory.CreateDirectory(filterFolder);
            string localFilterPath = Path.Combine(filterFolder, Path.GetFileName(SelectedFilter.FilePath));
            using var client = new HttpClient();

            try
            {
                // Download remote filter to temp file
                string tempPath = Path.GetTempFileName();
                var remoteBytes = await client.GetByteArrayAsync(SelectedFilter.RepoPath);
                await File.WriteAllBytesAsync(tempPath, remoteBytes);

                // Determine if replacement is needed
                bool shouldReplace = true;
                if (File.Exists(localFilterPath))
                {
                    if (ShellViewModel.UserSettings.FilterUpdates)
                        localFilterPath = Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua");

                    var localBytes = await File.ReadAllBytesAsync(localFilterPath);
                    if (localBytes.Length == remoteBytes.Length && localBytes.SequenceEqual(remoteBytes))
                        shouldReplace = false;
                }

                if (!shouldReplace)
                {
                    File.Delete(tempPath);
                    return; // No update needed
                }

                // Replace local file with remote version
                File.Copy(tempPath, localFilterPath, overwrite: true);
                if (ShellViewModel.UserSettings.FilterUpdates)
                    File.Copy(localFilterPath, Path.Combine(filterFolder, Path.GetFileName(SelectedFilter.FilePath)), overwrite: true);
                File.Delete(tempPath);

                _logger.Info($"{SelectedFilter.Title} has been updated from Github!");

                // Remember previous selection
                string previousTitle = SelectedFilter.Title;
                LoadFilterTitlesFromFolder(filterFolder);

                // Try to re-select the previous filter or the newly added one
                var matchFilter = FilterList.FirstOrDefault(f => f?.Title.Equals(previousTitle, StringComparison.OrdinalIgnoreCase) == true);

                if (matchFilter == null)
                    matchFilter = FilterList.FirstOrDefault(f => f?.FilePath == localFilterPath);

                if (matchFilter != null)
                    SelectedFilter = matchFilter;

                if (ShellViewModel.UserSettings.FilterUpdates)
                    File.Copy(SelectedFilter.FilePath, Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua"), true);

                // Inform the player
                if (!ShellViewModel.UserSettings.FilterUpdates)
                    MessageBox.Show($"A more recent version of '{SelectedFilter.Title}' is available!\nPress the Apply button to use it.", "Filter Update Available", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update filter '{SelectedFilter?.Title ?? "Unknown"}':\n{ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
