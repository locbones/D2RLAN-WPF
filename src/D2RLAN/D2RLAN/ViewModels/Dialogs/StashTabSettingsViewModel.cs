using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
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
        private ObservableCollection<string> _stashTabNames = new ObservableCollection<string>();

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

        #endregion

        #region ---Stash Tab Functions---

        public async Task GetStashTabNames()
        {
            string bankExpansionLayoutHdJsonPath = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/bankexpansionlayouthd.json");

            if (File.Exists(bankExpansionLayoutHdJsonPath))
            {
                string jsonString = await File.ReadAllTextAsync(bankExpansionLayoutHdJsonPath);
                JsonDocument jsonDoc = JsonDocument.Parse(jsonString.Replace("@", ""));
                JsonElement bankTabs = jsonDoc.RootElement.GetProperty("children")[8];
                JsonElement textStrings = bankTabs.GetProperty("fields").GetProperty("textStrings");

                foreach (JsonElement element in textStrings.EnumerateArray())
                {
                    StashTabNames.Add(element.GetString());
                    OriginalStashTabNames.Add(element.GetString());
                }
            }
        }
        [UsedImplicitly]
        public async void OnApply()
        {
            string bankExpansionLayoutHdJsonPath = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/bankexpansionlayouthd.json");

            string jsonString = await File.ReadAllTextAsync(bankExpansionLayoutHdJsonPath);

            jsonString = ReplaceFirst(jsonString, "\"@" + OriginalStashTabNames[0] + "\"", "\"" + StashTabNames[0] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"@" + OriginalStashTabNames[1] + "\"", "\"" + StashTabNames[1] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"@" + OriginalStashTabNames[2] + "\"", "\"" + StashTabNames[2] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"@" + OriginalStashTabNames[3] + "\"", "\"" + StashTabNames[3] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"@" + OriginalStashTabNames[4] + "\"", "\"" + StashTabNames[4] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"@" + OriginalStashTabNames[5] + "\"", "\"" + StashTabNames[5] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"@" + OriginalStashTabNames[6] + "\"", "\"" + StashTabNames[6] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"@" + OriginalStashTabNames[7] + "\"", "\"" + StashTabNames[7] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"" + OriginalStashTabNames[0] + "\"", "\"" + StashTabNames[0] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"" + OriginalStashTabNames[1] + "\"", "\"" + StashTabNames[1] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"" + OriginalStashTabNames[2] + "\"", "\"" + StashTabNames[2] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"" + OriginalStashTabNames[3] + "\"", "\"" + StashTabNames[3] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"" + OriginalStashTabNames[4] + "\"", "\"" + StashTabNames[4] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"" + OriginalStashTabNames[5] + "\"", "\"" + StashTabNames[5] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"" + OriginalStashTabNames[6] + "\"", "\"" + StashTabNames[6] + "\"");
            jsonString = ReplaceFirst(jsonString, "\"" + OriginalStashTabNames[7] + "\"", "\"" + StashTabNames[7] + "\"");

            //Write updated values back to JSON file
            await File.WriteAllTextAsync(bankExpansionLayoutHdJsonPath, jsonString);

            string remoddedThemePath = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/UI Theme/ReMoDDeD");
            string remoddedBankExpansionLayoutHdJsonPath = System.IO.Path.Combine(remoddedThemePath, "layouts/bankexpansionlayouthd.json");
            string remoddedBankExpansionLayoutHdJsonExpandedPath = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/UI Theme/expanded/layouts/bankexpansionlayouthd.json");
            if (Directory.Exists(remoddedThemePath))
            {
                if (File.Exists(remoddedBankExpansionLayoutHdJsonPath))
                    File.Delete(remoddedBankExpansionLayoutHdJsonPath);

                File.Copy(bankExpansionLayoutHdJsonPath, remoddedBankExpansionLayoutHdJsonPath);

                if (File.Exists(remoddedBankExpansionLayoutHdJsonExpandedPath))
                    File.Delete(remoddedBankExpansionLayoutHdJsonExpandedPath);

                File.Copy(bankExpansionLayoutHdJsonPath, remoddedBankExpansionLayoutHdJsonExpandedPath);
            }

            //Success Message
            MessageBox.Show("Stash Tab Names have been updated successfully!");

            await TryCloseAsync(true);
        }
        private string ReplaceFirst(string original, string oldValue, string newValue)
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