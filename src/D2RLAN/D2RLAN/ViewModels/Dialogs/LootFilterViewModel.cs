﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using D2RLAN.Models;
using D2RLAN.ViewModels.Drawers;
using Microsoft.Win32;

namespace D2RLAN.ViewModels.Dialogs
{
    #region Metadata Class
    public class FilterMetadata
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
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

            if (!string.IsNullOrEmpty(metadata.Title) &&
                !string.IsNullOrEmpty(metadata.Type) &&
                !string.IsNullOrEmpty(metadata.Description))
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

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(_selectedFilter?.FilePath) && File.Exists(_selectedFilter?.FilePath))
                        {
                            string destinationPath = Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua");
                            File.Copy(_selectedFilter?.FilePath, destinationPath, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to apply selected filter:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    NotifyOfPropertyChange(() => SelectedFilter);
                    NotifyOfPropertyChange(() => SelectedFilterTitle);
                    NotifyOfPropertyChange(() => SelectedFilterType);
                    NotifyOfPropertyChange(() => SelectedFilterDescription);
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

        private async void SaveSettingsAsync()
        {
            await ShellViewModel.SaveUserSettings();
        }

        #endregion

        #region ---Window/Loaded Handlers---

        public LootFilterViewModel(ShellViewModel shellViewModel)
        {
            DisplayName = "Active Loot Filter Settings";
            ShellViewModel = shellViewModel;

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

            string lootFilterPath = Path.Combine(ShellViewModel.GamePath, "lootfilter.lua");
            string configPath = Path.Combine(ShellViewModel.GamePath, "lootfilter_config.lua");
            string configBlankPath = Path.Combine(Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN\Filters"), "lootfilter_config_blank.lua");
            string guidePath = Path.Combine(ShellViewModel.GamePath, "lootfilter_guide.pdf");

            string expectedVersion = "0.0.0";
            bool shouldReplace = true;

            try
            {
                var embeddedBytes = Helper.GetResourceByteArray2("lootfilter.lua");
                var embeddedText = System.Text.Encoding.UTF8.GetString(embeddedBytes);
                var embeddedLines = embeddedText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                if (embeddedLines.Length >= 4)
                {
                    var match = Regex.Match(embeddedLines[3], @"local\s+version\s*=\s*""([^""]+)""");
                    if (match.Success)
                    {
                        expectedVersion = match.Groups[1].Value;
                    }
                }
            }
            catch
            {
                // Keep default version on error
            }

            if (File.Exists(lootFilterPath))
            {
                try
                {
                    var versionLine = File.ReadLines(lootFilterPath).Skip(3).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(versionLine))
                    {
                        var match = Regex.Match(versionLine, @"local\s+version\s*=\s*""([^""]+)""");
                        if (match.Success)
                        {
                            string currentVersion = match.Groups[1].Value;
                            if (currentVersion == expectedVersion)
                            {
                                shouldReplace = false;
                            }
                        }
                    }
                }
                catch
                {
                    shouldReplace = true; // Assume overwrite on read error
                }
            }

            if (shouldReplace)
            {
                File.WriteAllBytesAsync(lootFilterPath, Helper.GetResourceByteArray2("lootfilter.lua"));
                File.WriteAllBytesAsync(configPath, Helper.GetResourceByteArray2("lootfilter_config.lua"));
                File.WriteAllBytesAsync(configBlankPath, Helper.GetResourceByteArray2("lootfilter_config_blank.lua"));
                File.WriteAllBytesAsync(guidePath, Helper.GetResourceByteArray2("D2R_LootFilter_Guide.pdf"));

                string filterFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN\Filters");
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
                string renamedFileName = fileName.Equals("lootfilter_config.lua", StringComparison.OrdinalIgnoreCase)
                    ? "custom_lootfilter_config.lua"
                    : fileName;

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


        #endregion
    }
}
