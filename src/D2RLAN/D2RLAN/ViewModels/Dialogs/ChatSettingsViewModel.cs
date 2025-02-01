using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Caliburn.Micro;

namespace D2RLAN.ViewModels.Dialogs
{
    public class ChatSettingsViewModel : Screen
    {
        #region ---Static Members---

        private const string ConfigFilePath = "config.json";
        private ICommand _saveConfigCommand;
        private string _channelColor;
        private string _playerColor;
        private string _messageColor;
        private static readonly Dictionary<string, string> ColorCodeMapping = new Dictionary<string, string> //Mapping of color codes to color names
        {
            { "ÿc0", "White" },
            { "ÿc1", "Red" },
            { "ÿc2", "Green" },
            { "ÿc3", "Blue" },
            { "ÿc4", "Gold" },
            { "ÿc5", "Grey" },
            { "ÿc6", "Black" },
            { "ÿc7", "Tan" },
            { "ÿc8", "Orange" },
            { "ÿc9", "Yellow" },
            { "ÿc;", "Purple" },
            { "ÿcA", "Dark Green" },
            { "ÿcN", "Turquoise" },
            { "ÿcO", "Pink" }
        };

        #endregion

        #region ---Window/Loaded Handlers---

        public ChatSettingsViewModel(ShellViewModel shellViewModel)
        {
            DisplayName = "Custom Keybind Commands";
            ShellViewModel = shellViewModel;

            LoadConfig();
        }

        #endregion

        #region ---Properties---

        public ICommand SaveConfigCommand
        {
            get
            {
                if (_saveConfigCommand == null)
                {
                    _saveConfigCommand = new RelayCommand(param => SaveConfig(), param => true);
                }
                return _saveConfigCommand;
            }
        }
        public List<string> Options { get; set; }
        public ShellViewModel ShellViewModel { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public string ChannelColor
        {
            get => _channelColor;
            set
            {
                if (_channelColor != value)
                {
                    _channelColor = value;
                    OnPropertyChanged(nameof(ChannelColor));
                }
            }
        }
        public string PlayerColor
        {
            get => _playerColor;
            set
            {
                if (_playerColor != value)
                {
                    _playerColor = value;
                    OnPropertyChanged(nameof(PlayerColor));
                }
            }
        }
        public string MessageColor
        {
            get => _messageColor;
            set
            {
                if (_messageColor != value)
                {
                    _messageColor = value;
                    OnPropertyChanged(nameof(MessageColor));
                }
            }
        }

        #endregion

        #region ---Chat Controls---

        public void LoadConfig()
        {
            Options = new List<string>
        {
            "White", "Red", "Green", "Blue", "Gold", "Grey", "Black", "Tan",
            "Orange", "Yellow", "Purple", "Dark Green", "Turquoise", "Pink"
        };

            // Load from config file if exists
            if (File.Exists(ConfigFilePath))
            {
                var configContent = File.ReadAllText(ConfigFilePath);

                ChannelColor = GetColorFromConfig(configContent, "Channel Color", "ÿcO");
                PlayerColor = GetColorFromConfig(configContent, "Player Name Color", "ÿc3");
                MessageColor = GetColorFromConfig(configContent, "Message Color", "ÿc2");
            }
            else
            {
                // Set defaults
                ChannelColor = "ÿcO"; // Pink
                PlayerColor = "ÿc3"; // Blue
                MessageColor = "ÿc2"; // Green
            }

            // Ensure UI updates after loading the configuration
            OnPropertyChanged(nameof(ChannelColor));
            OnPropertyChanged(nameof(PlayerColor));
            OnPropertyChanged(nameof(MessageColor));
        }
        public void SaveConfig()
        {
            var configContent = File.Exists(ConfigFilePath) ? File.ReadAllText(ConfigFilePath) : string.Empty;

            configContent = UpdateColorInConfig(configContent, "Channel Color", ChannelColor);
            configContent = UpdateColorInConfig(configContent, "Player Name Color", PlayerColor);
            configContent = UpdateColorInConfig(configContent, "Message Color", MessageColor);

            if (string.IsNullOrEmpty(configContent))
            {
                configContent = "{\n  \"MonsterStatsDisplay\": true,\n}";
            }

            File.WriteAllText(ConfigFilePath, configContent);
        }
        private string GetColorFromConfig(string configContent, string key, string defaultColorCode)
        {
            var pattern = $"\"{key}\": \"(ÿc[0-9A-Za-z])\"";
            var match = Regex.Match(configContent, pattern);

            if (match.Success)
            {
                var colorCode = match.Groups[1].Value;
                return ColorCodeMapping.ContainsKey(colorCode) ? ColorCodeMapping[colorCode] : "White";
            }

            return ColorCodeMapping.ContainsKey(defaultColorCode) ? ColorCodeMapping[defaultColorCode] : "White";
        }
        private string UpdateColorInConfig(string configContent, string key, string colorName)
        {
            var colorCode = GetColorCodeFromName(colorName);
            var pattern = $"\"{key}\": \"(ÿc[0-9A-Za-z])\"";
            var replacement = $"\"{key}\": \"{colorCode}\"";

            return Regex.Replace(configContent, pattern, replacement);
        }
        private string GetColorCodeFromName(string colorName)
        {
            foreach (var kvp in ColorCodeMapping)
            {
                if (kvp.Value.Equals(colorName, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Key;
                }
            }

            return "ÿcO"; // Default to Pink if not found
        }

        #endregion
    }
}
