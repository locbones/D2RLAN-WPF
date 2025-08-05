using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using D2RLAN.Models;
using JetBrains.Annotations;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLAN.ViewModels.Dialogs
{
    public class StashTabSettingsViewModel : Screen
    {
        #region ---Static Members---

        private ILog _logger = LogManager.GetLogger(typeof(RestoreBackupViewModel));
        private List<string> _originalStashTabNames = new List<string>();
        private List<string> _originalStashPageNames = new List<string>();
        private ObservableCollection<string> _stashTabNames = new ObservableCollection<string>();
        private ObservableCollection<string> _stashPageNames = new ObservableCollection<string>();

        #endregion

        #region ---Window/Loaded Handlers---

        public StashTabSettingsViewModel()
        {
            if (Execute.InDesignMode)
            {
                StashTabNames = new ObservableCollection<string>
                                {
                                    "Personal",
                                    "Shared",
                                    "Shared",
                                    "Shared",
                                    "Shared",
                                    "Shared",
                                    "Shared",
                                    "Shared"
                                };

                StashPageNames = new ObservableCollection<string>
                                {
                                    "Page 1", "Page 2", "Page 3", "Page 4", "Page 5", "Page 6",
                                    "Page 7", "Page 8", "Page 9", "Page 10", "Page 11", "Page 12",
                                    "Page 13", "Page 14", "Page 15", "Page 16", "Page 17", "Page 18",
                                    "Page 19", "Page 20", "Page 21", "Page 22", "Page 23", "Page 24",
                                    "Page 25", "Page 26", "Page 27", "Page 28", "Page 29", "Page 30",
                                    "Page 31", "Page 32", "Page 33", "Page 34", "Page 35", "Page 36",
                                    "Page 37", "Page 38", "Page 39", "Page 40", "Page 41", "Page 42",
                                    "Page 43", "Page 44", "Page 45", "Page 46", "Page 47", "Page 48",
                                    "Page 49", "Page 50", "Page 51", "Page 52", "Page 53", "Page 54",
                                    "Page 55", "Page 56", "Page 57", "Page 58", "Page 59", "Page 60",
                                    "Page 61", "Page 62", "Page 63", "Page 64"
                                };
            }
        }
        public StashTabSettingsViewModel(ShellViewModel shellViewModel)
        {
            DisplayName = "Stash Tab Settings";
            ShellViewModel = shellViewModel;

            Execute.OnUIThread(async () => { await GetStashTabNames(); });
        }

        #endregion

        #region ---Properties---

        public ShellViewModel ShellViewModel { get; }
        public ObservableCollection<string> StashTabNames
        {
            get => _stashTabNames;
            set
            {
                if (Equals(value, _stashTabNames))
                {
                    return;
                }
                _stashTabNames = value;
                NotifyOfPropertyChange();
            }
        }
        public ObservableCollection<string> StashPageNames
        {
            get => _stashPageNames;
            set
            {
                if (Equals(value, _stashPageNames))
                {
                    return;
                }
                _stashPageNames = value;
                NotifyOfPropertyChange();
            }
        }
        public List<string> OriginalStashTabNames
        {
            get => _originalStashTabNames;
            set
            {
                if (Equals(value, _originalStashTabNames))
                {
                    return;
                }
                _originalStashTabNames = value;
                NotifyOfPropertyChange();
            }
        }
        public List<string> OriginalStashPageNames
        {
            get => _originalStashPageNames;
            set
            {
                if (Equals(value, _originalStashPageNames))
                {
                    return;
                }
                _originalStashPageNames = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region ---Stash Tab Functions---

        public async Task GetStashTabNames()
        {
            string bankExpansionLayoutHdJsonPath = ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/bankexpansionlayouthd.json";

            if (!File.Exists(bankExpansionLayoutHdJsonPath))
            {
                if (!Directory.Exists(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts"))
                    Directory.CreateDirectory(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts");

                await File.WriteAllBytesAsync(bankExpansionLayoutHdJsonPath,
                await Helper.GetResourceByteArray("Options.PersonalizedTabs.bankexpansionlayouthd.json"));
            }

            string jsonString = await File.ReadAllTextAsync(bankExpansionLayoutHdJsonPath);

            // Only insert if it doesn't already exist
            if (!jsonString.Contains("\"name\": \"BankPages\""))
            {
                // Trim leading whitespace to avoid blank line
                string bankPagesJson = @"{
            ""type"": ""DropdownListWidget"", ""name"": ""BankPages"",
            ""fields"": {
                ""anchor"": { ""x"": 0.15, ""y"": 0.05 },
                ""rect"": { ""width"": 408, ""height"": 75 },
                ""background/rect"": { ""x"": 20, ""width"": 408, ""height"": 75 },
                ""background/leftCapOffset"": ""$OptionsDropDownLeftCapOffset2"",
                ""background/rightCapOffset"": ""$OptionsDropDownRightCapOffset2"",
                ""pressedFrame"": 1,
                ""disabledFrame"": 2,
                ""hoveredFrame"": 3,
                ""focusIndicatorFilename"": ""$OptionsDropDownFocusIndicatorFilename"",
                ""states"": [ ""Page 1"", ""Page 2"", ""Page 3"", ""Page 4"", ""Page 5"", ""Page 6"", ""Page 7"", ""Page 8"", ""Page 9"", ""Page 10"", ""Page 11"", ""Page 12"", ""Page 13"", ""Page 14"", ""Page 15"", ""Page 16"", ""Page 17"", ""Page 18"", ""Page 19"", ""Page 20"", ""Page 21"", ""Page 22"", ""Page 23"", ""Page 24"", ""Page 25"", ""Page 26"", ""Page 27"", ""Page 28"", ""Page 29"", ""Page 30"", ""Page 31"", ""Page 32"", ""Page 33"", ""Page 34"", ""Page 35"", ""Page 36"", ""Page 37"", ""Page 38"", ""Page 39"", ""Page 40"", ""Page 41"", ""Page 42"", ""Page 43"", ""Page 44"", ""Page 45"", ""Page 46"", ""Page 47"", ""Page 48"", ""Page 49"", ""Page 50"", ""Page 51"", ""Page 52"", ""Page 53"", ""Page 54"", ""Page 55"", ""Page 56"", ""Page 57"", ""Page 58"", ""Page 59"", ""Page 60"", ""Page 61"", ""Page 62"", ""Page 63"", ""Page 64""],
                ""onUpdateMessage"": ""BankPanelMessage:SelectPage"",
                ""text/style"": { ""pointSize"": ""$MediumFontSize"" },
                ""textColor"": ""$FontColorLightGold"",
                ""tooltipString"": ""This allows you to switch between multiple pages of shared stash tabs"",
                ""tooltipStyle"": {
                ""showAfterDelay"": true
                }
            }
        }";

                int bankTabsIndex = jsonString.IndexOf("\"name\": \"BankTabs\"");
                if (bankTabsIndex >= 0)
                {
                    int insertIndex = jsonString.LastIndexOf('}', bankTabsIndex);
                    if (insertIndex >= 0)
                    {
                        jsonString = jsonString.Insert(insertIndex + 1, ",\n\t\t" + bankPagesJson.Trim());
                        await File.WriteAllTextAsync(bankExpansionLayoutHdJsonPath, jsonString);
                    }
                }
            }

            if (File.Exists(bankExpansionLayoutHdJsonPath))
            {
                jsonString = await File.ReadAllTextAsync(bankExpansionLayoutHdJsonPath);
                jsonString = Regex.Replace(jsonString, @"(\s*,\s*)([\}\]])", "$2");
                JsonDocument jsonDoc = JsonDocument.Parse(jsonString.Replace("@", ""));
                JsonElement children = jsonDoc.RootElement.GetProperty("children");
                JsonElement bankTabs = default;

                foreach (JsonElement child in children.EnumerateArray())
                {
                    if (child.TryGetProperty("name", out JsonElement name) && name.GetString() == "BankTabs")
                    {
                        bankTabs = child;
                        break;
                    }
                }

                JsonElement textStrings = bankTabs.GetProperty("fields").GetProperty("textStrings");

                foreach (JsonElement element in textStrings.EnumerateArray())
                {
                    StashTabNames.Add(element.GetString());
                    OriginalStashTabNames.Add(element.GetString());
                }

                while (StashTabNames.Count < 8)
                {
                    StashTabNames.Add("JustUnlocked");
                    OriginalStashTabNames.Add("JustUnlocked");
                }

                jsonString = await File.ReadAllTextAsync(bankExpansionLayoutHdJsonPath);
                jsonString = Regex.Replace(jsonString, @"(\s*,\s*)([\}\]])", "$2");
                jsonDoc = JsonDocument.Parse(jsonString.Replace("@", ""));
                children = jsonDoc.RootElement.GetProperty("children");
                bankTabs = default(JsonElement);
                foreach (JsonElement child in children.EnumerateArray())
                {
                    if (child.TryGetProperty("name", out JsonElement name) && name.GetString() == "BankPages")
                    {
                        bankTabs = child;
                        break;
                    }
                }

                textStrings = bankTabs.GetProperty("fields").GetProperty("states");

                foreach (JsonElement element in textStrings.EnumerateArray())
                {
                    StashPageNames.Add(element.GetString());
                    OriginalStashPageNames.Add(element.GetString());
                }

                while (StashPageNames.Count < 64)
                {
                    string newPageName = $"Page {StashPageNames.Count + 1}";
                    StashPageNames.Add(newPageName);
                    OriginalStashPageNames.Add(newPageName);
                }
            }
        }

        [UsedImplicitly]
        public async void OnApply() //Apply User-Chosen Settings
        {
            string bankExpansionLayoutHdJsonPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/bankexpansionlayouthd.json");
            string jsonString = await File.ReadAllTextAsync(bankExpansionLayoutHdJsonPath);

            jsonString = Regex.Replace(jsonString, @"(\s*,\s*)([\}\]])", "$2");
            JsonDocument jsonDoc = JsonDocument.Parse(jsonString);
            JsonElement bankTabs;
            JsonElement bankPages;

            if (jsonString.Contains("LeftSprite"))
            {
                bankTabs = jsonDoc.RootElement.GetProperty("children")[10];
                bankPages = jsonDoc.RootElement.GetProperty("children")[9];
            }
            else
            {
                bankTabs = jsonDoc.RootElement.GetProperty("children")[9];
                bankPages = jsonDoc.RootElement.GetProperty("children")[8];
            }

            JsonElement textStrings = bankTabs.GetProperty("fields").GetProperty("textStrings");
            JsonElement tabCount = bankTabs.GetProperty("fields").GetProperty("tabCount");
            JsonElement inactiveFrames = bankTabs.GetProperty("fields").GetProperty("inactiveFrames");
            JsonElement activeFrames = bankTabs.GetProperty("fields").GetProperty("activeFrames");
            JsonElement disabledFrames = bankTabs.GetProperty("fields").GetProperty("disabledFrames");
            JsonElement states = bankPages.GetProperty("fields").GetProperty("states");

            

            jsonString = ReplaceFirst(jsonString, $"\"tabCount\": {tabCount},", "\"tabCount\": 8,");
            jsonString = ReplaceFirst(jsonString, textStrings.ToString(), "[ \"" + StashTabNames[0] + "\", " + "\"" + StashTabNames[1] + "\", " + "\"" + StashTabNames[2] + "\", " + "\"" + StashTabNames[3] + "\", " + "\"" + StashTabNames[4] + "\", " + "\"" + StashTabNames[5] + "\", " + "\"" + StashTabNames[6] + "\", " + "\"" + StashTabNames[7] + "\" ]");
            jsonString = ReplaceFirst(jsonString, inactiveFrames.ToString(), "[ 0, 0, 0, 0, 0, 0, 0, 0 ]");
            jsonString = ReplaceFirst(jsonString, activeFrames.ToString(), "[ 1, 1, 1, 1, 1, 1, 1, 1 ]");
            jsonString = ReplaceFirst(jsonString, disabledFrames.ToString(), "[ 0, 0, 0, 0, 0, 0, 0, 0 ]");

            // Dynamically create the page names string
            string stashPagesString = "[ " + string.Join(", ", StashPageNames.Select(page => $"\"{page}\"")) + " ]";

            // Replace the 'states' property with the dynamically generated string
            jsonString = ReplaceFirst(jsonString, states.ToString(), stashPagesString);

            // Write the updated JSON string back to the file
            await File.WriteAllTextAsync(bankExpansionLayoutHdJsonPath, jsonString);

            string remoddedThemePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/UI Theme/ReMoDDeD");
            string remoddedBankExpansionLayoutHdJsonPath = Path.Combine(remoddedThemePath, "layouts/bankexpansionlayouthd.json");
            string remoddedBankExpansionLayoutHdJsonExpandedPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/UI Theme/expanded/layouts/bankexpansionlayouthd.json");

            if (Directory.Exists(remoddedThemePath))
            {
                if (File.Exists(remoddedBankExpansionLayoutHdJsonPath))
                    File.Delete(remoddedBankExpansionLayoutHdJsonPath);

                File.Copy(bankExpansionLayoutHdJsonPath, remoddedBankExpansionLayoutHdJsonPath);

                if (File.Exists(remoddedBankExpansionLayoutHdJsonExpandedPath))
                    File.Delete(remoddedBankExpansionLayoutHdJsonExpandedPath);

                File.Copy(bankExpansionLayoutHdJsonPath, remoddedBankExpansionLayoutHdJsonExpandedPath);
            }

            // Success Message
            MessageBox.Show("Stash Panel Names have been updated successfully!");

            await TryCloseAsync(true);
        }

        private string ReplaceFirst(string original, string oldValue, string newValue) //Helper method to replace only the first occurrence of a substring in a string
        {
            int index = original.IndexOf(oldValue);

            if (index == -1)
                return original; // Not found, return the original string

            string result = original.Remove(index, oldValue.Length).Insert(index, newValue);
            return result;
        }

        #endregion
    }
}