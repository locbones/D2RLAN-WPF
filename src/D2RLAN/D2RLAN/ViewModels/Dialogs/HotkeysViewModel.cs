using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Caliburn.Micro;

namespace D2RLAN.ViewModels.Dialogs
{
    public class HotkeysViewModel : Screen
    {
        #region ---Static Members---

        private const string ConfigPath = "D2RLAN_Config.txt";
        public string Transmute { get; set; }
        public string IdentifyItems { get; set; }
        public string ForceSave { get; set; }
        public string ResetStats { get; set; }
        public string ResetSkills { get; set; }
        public string RemoveGroundItems { get; set; }
        public string OpenCubePanel { get; set; }
        public string TZForwardPanel { get; set; }
        public string TZBackwardPanel { get; set; }
        public string TZStatTogglePanel { get; set; }
        public string CustomCommand1 { get; set; }
        public string CustomCommand2 { get; set; }
        public string CustomCommand3 { get; set; }
        public string CustomCommand4 { get; set; }
        public string CustomCommand5 { get; set; }
        public string CustomCommand6 { get; set; }
        public string CustomCommandC1 { get; set; }
        public string CustomCommandC2 { get; set; }
        public string CustomCommandC3 { get; set; }
        public string CustomCommandC4 { get; set; }
        public string CustomCommandC5 { get; set; }
        public string CustomCommandC6 { get; set; }
        public string StartupCommands { get; set; }
        public ICommand SaveCommand { get; private set; }
        public ShellViewModel ShellViewModel { get; }

        #endregion

        #region ---Window/Loaded Handlers---

        public HotkeysViewModel(ShellViewModel shellViewModel)
        {
            DisplayName = "Custom Keybind Commands";
            ShellViewModel = shellViewModel;

            LoadConfig();
        }

        #endregion

        #region ---Load Config---

        private void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
                return;

            var lines = File.ReadAllLines(ConfigPath);

            // Regex for standard commands (key + optional command in quotes)
            var regex = new Regex(@"^(?<name>[\w\s]+):\s(?<key>VK_\w+|NaN)(,\s""(?<command>.*)"")?$");

            // Regex for startup commands
            var startupRegex = new Regex(@"^Startup Commands:\s(?<command>.*)$");

            foreach (var line in lines)
            {
                var startupMatch = startupRegex.Match(line);
                if (startupMatch.Success)
                {
                    StartupCommands = startupMatch.Groups["command"].Value;
                    NotifyOfPropertyChange(nameof(StartupCommands));
                    continue;
                }

                // Special handling for Toggle Stat Adjustments Display
                if (line.StartsWith("Toggle Stat Adjustments Display:"))
                {
                    // Split at the first comma
                    int commaIndex = line.IndexOf(',');
                    if (commaIndex > 0)
                    {
                        // Extract boolean and key
                        string beforeComma = line.Substring(0, commaIndex).Trim(); // "Toggle Stat Adjustments Display: true"
                        string afterComma = line.Substring(commaIndex + 1).Trim(); // "VK_F7"

                        // Extract the boolean value
                        int colonIndex = beforeComma.IndexOf(':');
                        string boolText = colonIndex >= 0 ? beforeComma.Substring(colonIndex + 1).Trim() : "false";
                        bool showStatAdjusts = boolText.Equals("true", StringComparison.OrdinalIgnoreCase);

                        // Store key
                        TZStatTogglePanel = afterComma;
                        continue;
                    }
                }

                // Standard regex match
                var match = regex.Match(line);
                if (match.Success)
                {
                    var name = match.Groups["name"].Value;
                    var key = match.Groups["key"].Value;
                    var command = match.Groups["command"].Value;

                    switch (name)
                    {
                        case "Transmute":
                            Transmute = key == "NaN" ? "NaN" : key;
                            break;
                        case "Identify Items":
                            IdentifyItems = key == "NaN" ? "NaN" : key;
                            break;
                        case "Force Save":
                            ForceSave = key == "NaN" ? "NaN" : key;
                            break;
                        case "Reset Stats":
                            ResetStats = key == "NaN" ? "NaN" : key;
                            break;
                        case "Reset Skills":
                            ResetSkills = key == "NaN" ? "NaN" : key;
                            break;
                        case "Remove Ground Items":
                            RemoveGroundItems = key == "NaN" ? "NaN" : key;
                            break;
                        case "Custom Command 1":
                            CustomCommand1 = key == "NaN" ? "NaN" : key;
                            CustomCommandC1 = command;
                            break;
                        case "Custom Command 2":
                            CustomCommand2 = key == "NaN" ? "NaN" : key;
                            CustomCommandC2 = command;
                            break;
                        case "Custom Command 3":
                            CustomCommand3 = key == "NaN" ? "NaN" : key;
                            CustomCommandC3 = command;
                            break;
                        case "Custom Command 4":
                            CustomCommand4 = key == "NaN" ? "NaN" : key;
                            CustomCommandC4 = command;
                            break;
                        case "Custom Command 5":
                            CustomCommand5 = key == "NaN" ? "NaN" : key;
                            CustomCommandC5 = command;
                            break;
                        case "Custom Command 6":
                            CustomCommand6 = key == "NaN" ? "NaN" : key;
                            CustomCommandC6 = command;
                            break;
                        case "Open Cube Panel":
                            OpenCubePanel = key == "NaN" ? "NaN" : key;
                            break;
                        case "Cycle TZ Forward":
                            TZForwardPanel = key == "NaN" ? "NaN" : key;
                            break;
                        case "Cycle TZ Backward":
                            TZBackwardPanel = key == "NaN" ? "NaN" : key;
                            break;
                    }
                }
            }

            NotifyOfPropertyChange(string.Empty);
        }


        #endregion
    }

    //Hotkey Relay
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
