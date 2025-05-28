using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using D2RLAN.Models;
using D2RLAN.Models.Enums;
using JetBrains.Annotations;
using Syncfusion.Licensing;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using D2RLAN.Properties;

namespace D2RLAN.ViewModels.Drawers
{
    public class CustomizationsDrawerViewModel : INotifyPropertyChanged
    {
        #region ---Static Members---

        private ILog _logger = LogManager.GetLogger(typeof(CustomizationsDrawerViewModel));
        private DifficultyCustomizations _selectedDifficulty;
        private bool _normal = true;
        private bool _nightmare;
        private bool _hell;
        private string _actOneString;
        private string _actTwoString;
        private string _actThreeString;
        private string _actFourString;
        private string _actFiveString;
        private ObservableCollection<KeyValuePair<string, eChampionPacks>> _championPacks;
        private ObservableCollection<KeyValuePair<string, eGroupSizes>> _groupSizes;
        private ObservableCollection<KeyValuePair<string, eExpRate>> _expRates;
        private ObservableCollection<KeyValuePair<string, eMonsterItemDrops>> _monsterItemDrops;
        private ObservableCollection<KeyValuePair<string, eShortenedLevels>> _shortenedLevels;
        private string _customizationsPath;
        private string _globalTreasureClassExTxtPath;
        private string _globalLevelsTxtPath;
        private string _globalMonStatsTxtPath;
        private string _customizationsTreasureClassExTxtPath;
        private string _customizationLevelsTxtPath;
        private string _customizationMonStatsTxtPath;
        private bool _shortenedLevelsEnabled = true;
        public ShellViewModel ShellViewModel { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        #endregion

        #region ---Window/Loaded Handlers---

        public void Initialize()
        {
            ChampionPacks = new ObservableCollection<KeyValuePair<string, eChampionPacks>>();
            GroupSizes = new ObservableCollection<KeyValuePair<string, eGroupSizes>>();
            ExpRates = new ObservableCollection<KeyValuePair<string, eExpRate>>();
            MonsterItemDrops = new ObservableCollection<KeyValuePair<string, eMonsterItemDrops>>();
            ShortenedLevels = new ObservableCollection<KeyValuePair<string, eShortenedLevels>>();

            foreach (eChampionPacks championPacks in Enum.GetValues<eChampionPacks>())
            {
                ChampionPacks.Add(new KeyValuePair<string, eChampionPacks>(championPacks.GetAttributeOfType<DisplayAttribute>().Name, championPacks));
            }

            foreach (eGroupSizes groupSizes in Enum.GetValues<eGroupSizes>())
            {
                GroupSizes.Add(new KeyValuePair<string, eGroupSizes>(groupSizes.GetAttributeOfType<DisplayAttribute>().Name, groupSizes));
            }

            foreach (eExpRate expRate in Enum.GetValues<eExpRate>())
            {
                ExpRates.Add(new KeyValuePair<string, eExpRate>(expRate.GetAttributeOfType<DisplayAttribute>().Name, expRate));
            }

            foreach (eMonsterItemDrops monsterItemDrops in Enum.GetValues<eMonsterItemDrops>())
            {
                MonsterItemDrops.Add(new KeyValuePair<string, eMonsterItemDrops>(Helper.GetCultureString(monsterItemDrops.GetAttributeOfType<DisplayAttribute>().Name), monsterItemDrops));
            }

            foreach (eShortenedLevels shortenedLevels in Enum.GetValues<eShortenedLevels>())
            {
                ShortenedLevels.Add(new KeyValuePair<string, eShortenedLevels>(Helper.GetCultureString(shortenedLevels.GetAttributeOfType<DisplayAttribute>().Name), shortenedLevels));
            }

            if (ShellViewModel.ModInfo.Name != "VNP-MP")
            {
                ShortenedLevelsEnabled = false;
            }

            if (!Directory.Exists(_customizationsPath))
                Directory.CreateDirectory(_customizationsPath);

            if (!File.Exists(_globalLevelsTxtPath))
                File.WriteAllBytesAsync(ShellViewModel.SelectedModDataFolder + "/global/excel/levels.txt", Helper.GetResourceByteArray2("CASC.levels.txt"));
            if (!File.Exists(_globalMonStatsTxtPath))
                File.WriteAllBytesAsync(ShellViewModel.SelectedModDataFolder + "/global/excel/monstats.txt", Helper.GetResourceByteArray2("CASC.monstats.txt"));
            if (!File.Exists(_globalTreasureClassExTxtPath))
                File.WriteAllBytesAsync(ShellViewModel.SelectedModDataFolder + "/global/excel/treasureclassex.txt", Helper.GetResourceByteArray2("CASC.treasureclassex.txt"));

            if (!File.Exists(_customizationLevelsTxtPath))
                File.Copy(_globalLevelsTxtPath, _customizationLevelsTxtPath);
            if (!File.Exists(_customizationMonStatsTxtPath))
                File.Copy(_globalMonStatsTxtPath, _customizationMonStatsTxtPath);
            if (!File.Exists(_customizationsTreasureClassExTxtPath))
                File.Copy(_globalTreasureClassExTxtPath, _customizationsTreasureClassExTxtPath);

            ChangeDifficulty();

        }
        public CustomizationsDrawerViewModel()
        {
            if (Execute.InDesignMode)
            {
                if (Execute.InDesignMode)
                {
                    ActOneString = $"{Helper.GetCultureString("ActX")} 1";
                    ActTwoString = $"{Helper.GetCultureString("ActX")} 2";
                    ActThreeString = $"{Helper.GetCultureString("ActX")} 3";
                    ActFourString = $"{Helper.GetCultureString("ActX")} 4";
                    ActFiveString = $"{Helper.GetCultureString("ActX")} 5";
                }

                SelectedDifficulty = new DifficultyCustomizations();
            }
        }
        public CustomizationsDrawerViewModel(ShellViewModel shellViewModel)
        {
            ShellViewModel = shellViewModel;

            _customizationsPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/Customizations");

            _globalTreasureClassExTxtPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/treasureclassex.txt");
            _globalLevelsTxtPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/levels.txt");
            _globalMonStatsTxtPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/monstats.txt");

            _customizationsTreasureClassExTxtPath = Path.Combine(_customizationsPath, "treasureclassex.txt");
            _customizationLevelsTxtPath = Path.Combine(_customizationsPath, "levels.txt");
            _customizationMonStatsTxtPath = Path.Combine(_customizationsPath, "monstats.txt");

            Initialize();

            ActOneString = $"{Helper.GetCultureString("ActX")} 1";
            ActTwoString = $"{Helper.GetCultureString("ActX")} 2";
            ActThreeString = $"{Helper.GetCultureString("ActX")} 3";
            ActFourString = $"{Helper.GetCultureString("ActX")} 4";
            ActFiveString = $"{Helper.GetCultureString("ActX")} 5";

            ShellViewModel.UserSettings.DifficultyCustomizations ??= new Dictionary<string, DifficultyCustomizations>()
                                                                     {
                                                                         {"Normal", new DifficultyCustomizations()},
                                                                         {"Nightmare", new DifficultyCustomizations()},
                                                                         {"Hell", new DifficultyCustomizations()}
                                                                     };

            foreach (KeyValuePair<string, DifficultyCustomizations> userSettingsDifficultyCustomization in ShellViewModel.UserSettings.DifficultyCustomizations)
            {
                userSettingsDifficultyCustomization.Value.PropertyChanged += async (sender, args) => { await OnDifficultyPropertyChanged(sender, args); };
            }

            ChangeDifficulty();
        }

        #endregion

        #region ---Properties---

        public bool ShortenedLevelsEnabled
        {
            get => _shortenedLevelsEnabled;
            set
            {
                if (value == _shortenedLevelsEnabled)
                {
                    return;
                }
                _shortenedLevelsEnabled = value;
                OnPropertyChanged();
            }
        }
        public string ActOneString
        {
            get => _actOneString;
            set
            {
                if (value == _actOneString)
                {
                    return;
                }
                _actOneString = value;
                OnPropertyChanged();
            }
        }
        public string ActTwoString
        {
            get => _actTwoString;
            set
            {
                if (value == _actTwoString)
                {
                    return;
                }
                _actTwoString = value;
                OnPropertyChanged();
            }
        }
        public string ActThreeString
        {
            get => _actThreeString;
            set
            {
                if (value == _actThreeString)
                {
                    return;
                }
                _actThreeString = value;
                OnPropertyChanged();
            }
        }
        public string ActFourString
        {
            get => _actFourString;
            set
            {
                if (value == _actFourString)
                {
                    return;
                }
                _actFourString = value;
                OnPropertyChanged();
            }
        }
        public string ActFiveString
        {
            get => _actFiveString;
            set
            {
                if (value == _actFiveString)
                {
                    return;
                }
                _actFiveString = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<KeyValuePair<string, eChampionPacks>> ChampionPacks
        {
            get => _championPacks;
            set
            {
                if (Equals(value, _championPacks))
                {
                    return;
                }
                _championPacks = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<KeyValuePair<string, eGroupSizes>> GroupSizes
        {
            get => _groupSizes;
            set
            {
                if (Equals(value, _groupSizes))
                {
                    return;
                }
                _groupSizes = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<KeyValuePair<string, eExpRate>> ExpRates
        {
            get => _expRates;
            set
            {
                if (Equals(value, _expRates))
                {
                    return;
                }
                _expRates = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<KeyValuePair<string, eMonsterItemDrops>> MonsterItemDrops
        {
            get => _monsterItemDrops;
            set
            {
                if (Equals(value, _monsterItemDrops))
                {
                    return;
                }
                _monsterItemDrops = value;
                OnPropertyChanged();
                OnPropertyChanged();
            }
        }
        public ObservableCollection<KeyValuePair<string, eShortenedLevels>> ShortenedLevels
        {
            get => _shortenedLevels;
            set
            {
                if (Equals(value, _shortenedLevels))
                {
                    return;
                }
                _shortenedLevels = value;
                OnPropertyChanged();
            }
        }
        public DifficultyCustomizations SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                if (value == _selectedDifficulty)
                {
                    return;
                }
                _selectedDifficulty = value;
                OnPropertyChanged();
            }
        }
        public bool Normal
        {
            get => _normal;
            set
            {
                if (value == _normal)
                {
                    return;
                }
                _normal = value;
                OnPropertyChanged();

                ChangeDifficulty();
            }
        }
        public bool Nightmare
        {
            get => _nightmare;
            set
            {
                if (value == _nightmare)
                {
                    return;
                }
                _nightmare = value;
                OnPropertyChanged();

                ChangeDifficulty();
            }
        }
        public bool Hell
        {
            get => _hell;
            set
            {
                if (value == _hell)
                {
                    return;
                }
                _hell = value;
                OnPropertyChanged();

                ChangeDifficulty();
            }
        }

        #endregion

        #region ---Control Functions---
        [UsedImplicitly]
        public async void OnMonsterItemDropChange()
        {
            if (Directory.Exists(_customizationsPath))
            {
                if (!File.Exists(_globalTreasureClassExTxtPath))
                    await File.WriteAllBytesAsync(ShellViewModel.SelectedModDataFolder + "/global/excel/treasureclassex.txt", await Helper.GetResourceByteArray("CASC.treasureclassex.txt"));

                string[] lines = await File.ReadAllLinesAsync(_customizationsTreasureClassExTxtPath);
                string contents = "";
                int index = 0;

                foreach (string line in lines)
                {
                    string[] splitContent = line.Split('\t');

                    switch ((eMonsterItemDrops)ShellViewModel.UserSettings.SelectedMonsterItemDrops)
                    {
                        case eMonsterItemDrops.Standard:
                            {
                                contents += line + "\n";
                                index += 1;
                                break;
                            }
                        case eMonsterItemDrops.SuperUniques:
                            {
                                if (index is > 736 and < 740 or > 793 and < 795 or > 801 and < 805 or > 819 and < 822)
                                {
                                    splitContent[8] = 0.ToString();
                                    contents += string.Join("\t", splitContent) + "\n";
                                }
                                else
                                {
                                    contents += line + "\n";
                                }
                                index += 1;
                                break;
                            }
                        case eMonsterItemDrops.AllMonsters:
                            {
                                if (index > 42)
                                {
                                    splitContent[8] = 0.ToString();
                                    contents += string.Join("\t", splitContent) + "\n";
                                }
                                else
                                {
                                    contents += line + "\n";
                                }
                                index += 1;
                                break;
                            }
                    }

                    await File.WriteAllTextAsync(_globalTreasureClassExTxtPath, contents);
                }
            }
        }
        private async Task OnDifficultyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int difficulty = Normal ? 0 : Nightmare ? 1 : Hell ? 2 : 0;

            switch (e.PropertyName)
            {
                case nameof(SelectedDifficulty.ActOneMultiplier):
                    {
                        (int density, double spawnChance) value = await CalculateMultiplier(SelectedDifficulty.ActOneMultiplier, difficulty, 1, 3, 40);
                        SelectedDifficulty.ActOneDensity = value.density;
                        SelectedDifficulty.ActOneSpawnChance = value.spawnChance;
                        SelectedDifficulty.ActOneMultiplierString = $"({SelectedDifficulty.ActOneMultiplier}x)";
                        break;
                    }
                case nameof(SelectedDifficulty.ActTwoMultiplier):
                    {
                        (int density, double spawnChance) value = await CalculateMultiplier(SelectedDifficulty.ActTwoMultiplier, difficulty, 2, 42, 75);
                        SelectedDifficulty.ActTwoDensity = value.density;
                        SelectedDifficulty.ActTwoSpawnChance = value.spawnChance;
                        SelectedDifficulty.ActTwoMultiplierString = $"({SelectedDifficulty.ActTwoMultiplier}x)";
                        break;
                    }
                case nameof(SelectedDifficulty.ActThreeMultiplier):
                    {
                        (int density, double spawnChance) value = await CalculateMultiplier(SelectedDifficulty.ActThreeMultiplier, difficulty, 3, 77, 103);
                        SelectedDifficulty.ActThreeDensity = value.density;
                        SelectedDifficulty.ActThreeSpawnChance = value.spawnChance;
                        SelectedDifficulty.ActThreeMultiplierString = $"({SelectedDifficulty.ActThreeMultiplier}x)";
                        break;
                    }
                case nameof(SelectedDifficulty.ActFourMultiplier):
                    {
                        (int density, double spawnChance) value = await CalculateMultiplier(SelectedDifficulty.ActFourMultiplier, difficulty, 4, 105, 109);
                        SelectedDifficulty.ActFourDensity = value.density;
                        SelectedDifficulty.ActFourSpawnChance = value.spawnChance;
                        SelectedDifficulty.ActFourMultiplierString = $"({SelectedDifficulty.ActFourMultiplier}x)";
                        break;
                    }
                case nameof(SelectedDifficulty.ActFiveMultiplier):
                    {
                        (int density, double spawnChance) value = await CalculateMultiplier(SelectedDifficulty.ActFiveMultiplier, difficulty, 5, 112, 132);
                        SelectedDifficulty.ActFiveDensity = value.density;
                        SelectedDifficulty.ActFiveSpawnChance = value.spawnChance;
                        SelectedDifficulty.ActFiveMultiplierString = $"({SelectedDifficulty.ActFiveMultiplier}x)";
                        break;
                    }
            }
        }
        private async Task<(int density, double spawnChance)> CalculateMultiplier(double multiplier, int difficulty, int act, int startIndex, int endIndex)
        {
            int density = 0;
            int densityAverage = 0;
            if (Directory.Exists(_customizationsPath))
            {
                string[] lines = await File.ReadAllLinesAsync(Path.Combine(_customizationsPath, "levels.txt"));

                for (int i = 0; i < lines.Length; i++)
                {
                    if (i >= startIndex && i <= endIndex)
                    {
                        string[] splitContent = lines[i].Split('\t');
                        int columnCount = 63 + difficulty; // Adjust the color index offset

                        //if (multiplier >= 10)
                        //{
                        //    density += int.Parse(splitContent[columnCount]) * 20;
                        //    splitContent[columnCount] = (int.Parse(splitContent[columnCount]) * 20).ToString();
                        //}
                        //else
                        {
                            density += int.Parse(splitContent[columnCount]) * (int)multiplier;
                            splitContent[columnCount] = (int.Parse(splitContent[columnCount]) * multiplier).ToString();
                        }

                        switch (act)
                        {
                            case 1:
                                {
                                    densityAverage = density / 38;
                                    break;
                                }
                            case 2:
                                {
                                    densityAverage = density / 34;
                                    break;
                                }
                            case 3:
                                {
                                    densityAverage = density / 27;
                                    break;
                                }
                            case 4:
                                {
                                    densityAverage = density / 5;
                                    break;
                                }
                            case 5:
                                {
                                    densityAverage = density / 20;
                                    break;
                                }
                        }
                    }
                }
            }

            if ((Convert.ToDouble(densityAverage) / 1000.0) < 10.0)
                return (densityAverage, Math.Round(Convert.ToDouble(densityAverage) / 1000, 1));

            return (10000, 10.0);
        }
        private async void ChangeDifficulty()
        {
            string selectedDifficultyKey = null;

            if (Normal)
                selectedDifficultyKey = "Normal";
            else if (Nightmare)
                selectedDifficultyKey = "Nightmare";
            else if (Hell)
                selectedDifficultyKey = "Hell";

            if (selectedDifficultyKey != null && ShellViewModel.UserSettings.DifficultyCustomizations.TryGetValue(selectedDifficultyKey, out var difficultySettings))
            {
                SelectedDifficulty = difficultySettings;
                await UpdateDifficultyPropertiesAsync();
            }
            else
                _logger.Error("\nCustomizations: Settings not found");
        }
        private async Task UpdateDifficultyPropertiesAsync()
        {
            var properties = new[]
            {
        nameof(SelectedDifficulty.ActOneMultiplier),
        nameof(SelectedDifficulty.ActTwoMultiplier),
        nameof(SelectedDifficulty.ActThreeMultiplier),
        nameof(SelectedDifficulty.ActFourMultiplier),
        nameof(SelectedDifficulty.ActFiveMultiplier)
    };

            foreach (var property in properties)
            {
                await OnDifficultyPropertyChanged(null, new PropertyChangedEventArgs(property));
            }
        }
        [UsedImplicitly]
        public async void OnApply()
        {
            if (Directory.Exists(_customizationsPath))
            {
                string[] levelsContent = await File.ReadAllLinesAsync(_customizationLevelsTxtPath);
                string[] monStatsContent = await File.ReadAllLinesAsync(_customizationMonStatsTxtPath);

                string contents = "";
                int index = 0;

                #region Level Mods
                foreach (string line in levelsContent)
                {
                    string[] splitContent = line.Split('\t');
                    #region Levels - Act 1

                    if (index < 3)
                    {
                        contents += line + "\n";
                    }

                    else if (index is >= 3 and <= 40)
                    {

                        int[] multiplierValues = new int[] { (int)ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].ActOneMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].ActOneMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].ActOneMultiplier };
                        int[] champValues = new int[] { ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].SelectedChampionPack };

                        for (int i = 0; i < multiplierValues.Length; i++)
                        {
                            int densityIndex = 63 + i;
                            int champIndex = 66 + i;

                            if (!string.IsNullOrEmpty(splitContent[densityIndex]))
                            {
                                int newValue = int.Parse(splitContent[densityIndex]) * multiplierValues[i];
                                splitContent[densityIndex] = (newValue >= 10000) ? "10000" : newValue.ToString();
                            }

                            if (!string.IsNullOrEmpty(splitContent[champIndex]))
                            {
                                splitContent[champIndex] = (int.Parse(splitContent[champIndex]) + champValues[i]).ToString();
                                splitContent[champIndex + 1] = (int.Parse(splitContent[champIndex + 1]) + champValues[i]).ToString();
                            }
                        }

                        contents += String.Join("\t", splitContent) + "\n";
                    }
                    #endregion
                    #region Levels - Act 2
                    if (index == 41)
                    {
                        contents += line + "\n";
                    }
                    else if (index is >= 42 and <= 75)
                    {
                        int[] multiplierValues = new int[] { (int)ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].ActTwoMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].ActTwoMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].ActTwoMultiplier };
                        int[] champValues = new int[] { ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].SelectedChampionPack };

                        for (int i = 0; i < multiplierValues.Length; i++)
                        {
                            int densityIndex = 63 + i;
                            int champIndex = 66 + i;

                            if (!string.IsNullOrEmpty(splitContent[densityIndex]))
                            {
                                int newValue = Int32.Parse(splitContent[densityIndex]) * multiplierValues[i];
                                splitContent[densityIndex] = (newValue >= 10000) ? "10000" : newValue.ToString();
                            }

                            if (!string.IsNullOrEmpty(splitContent[champIndex]))
                            {
                                splitContent[champIndex] = (int.Parse(splitContent[champIndex]) + champValues[i]).ToString();
                                splitContent[champIndex + 1] = (int.Parse(splitContent[champIndex + 1]) + champValues[i]).ToString();
                            }
                        }

                        contents += String.Join("\t", splitContent) + "\n";
                    }
                    #endregion
                    #region Levels - Act 3
                    if (index == 76)
                    {
                        contents += line + "\n";
                    }
                    else if (index is >= 77 and <= 103)
                    {
                        int[] multiplierValues = new int[] { (int)ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].ActThreeMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].ActThreeMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].ActThreeMultiplier };
                        int[] champValues = new int[] { ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].SelectedChampionPack };

                        for (int i = 0; i < multiplierValues.Length; i++)
                        {
                            int densityIndex = 63 + i;
                            int champIndex = 66 + i;

                            if (!string.IsNullOrEmpty(splitContent[densityIndex]))
                            {
                                int newValue = int.Parse(splitContent[densityIndex]) * multiplierValues[i];
                                splitContent[densityIndex] = (newValue >= 10000) ? "10000" : newValue.ToString();
                            }

                            if (!string.IsNullOrEmpty(splitContent[champIndex]))
                            {
                                splitContent[champIndex] = (int.Parse(splitContent[champIndex]) + champValues[i]).ToString();
                                splitContent[champIndex + 1] = (int.Parse(splitContent[champIndex + 1]) + champValues[i]).ToString();
                            }
                        }

                        contents += String.Join("\t", splitContent) + "\n";
                    }
                    #endregion
                    #region Levels - Act 4
                    if (index == 104)
                    {
                        contents += line + "\n";
                    }
                    else if (index is >= 105 and <= 109)
                    {
                        int[] multiplierValues = new int[] { (int)ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].ActFourMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].ActFourMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].ActFourMultiplier };
                        int[] champValues = new int[] { ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].SelectedChampionPack };

                        for (int i = 0; i < multiplierValues.Length; i++)
                        {
                            int densityIndex = 63 + i;
                            int champIndex = 66 + i;

                            if (!string.IsNullOrEmpty(splitContent[densityIndex]))
                            {
                                int newValue = int.Parse(splitContent[densityIndex]) * (multiplierValues[i] + 1);
                                splitContent[densityIndex] = (newValue >= 10000) ? "10000" : newValue.ToString();
                            }

                            if (!string.IsNullOrEmpty(splitContent[champIndex]))
                            {
                                splitContent[champIndex] = (int.Parse(splitContent[champIndex]) + champValues[i]).ToString();
                                splitContent[champIndex + 1] = (int.Parse(splitContent[champIndex + 1]) + champValues[i]).ToString();
                            }
                        }

                        contents += String.Join("\t", splitContent) + "\n";
                    }

                    #endregion
                    #region Levels - Act 5
                    if (index is 110 or 111)
                    {
                        contents += line + "\n";
                    }
                    else if (index >= 112)
                    {
                        int[] multiplierValues = new int[] { (int)ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].ActFiveMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].ActFiveMultiplier, (int)ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].ActFiveMultiplier };
                        int[] champValues = new int[] { ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].SelectedChampionPack, ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].SelectedChampionPack };

                        for (int i = 0; i < multiplierValues.Length; i++)
                        {
                            int densityIndex = 63 + i;
                            int champIndex = 66 + i;

                            if (!string.IsNullOrEmpty(splitContent[densityIndex]))
                            {
                                int newValue = int.Parse(splitContent[densityIndex]) * multiplierValues[i];
                                splitContent[densityIndex] = (newValue >= 10000) ? "10000" : newValue.ToString();
                            }

                            if (!string.IsNullOrEmpty(splitContent[champIndex]))
                            {
                                splitContent[champIndex] = (int.Parse(splitContent[champIndex]) + champValues[i]).ToString();
                                splitContent[67 + i] = (int.Parse(splitContent[67 + i]) + champValues[i]).ToString();
                            }
                        }

                        contents += String.Join("\t", splitContent) + "\n";
                    }
                    index += 1;
                    #endregion
                }
                await File.WriteAllTextAsync(_globalLevelsTxtPath, contents);
                #endregion


                #region Monstat Mods
                contents = "";
                index = 0;
                foreach (string line in monStatsContent)
                {
                    string[] splitContent = line.Split('\t');

                    if (index < 1) { contents += line + "\n"; }
                    if (index > 0)
                    {
                        if (splitContent[25] != "")
                        {
                            splitContent[25] = (int.Parse(splitContent[25]) + ShellViewModel.UserSettings.SelectedGroupSize).ToString();
                            splitContent[26] = (int.Parse(splitContent[26]) + ShellViewModel.UserSettings.SelectedGroupSize).ToString();
                        }
                        if (splitContent[162] != "" && splitContent[175] != "" && splitContent[188] != "")
                        {
                            splitContent[162] = (int.Parse(splitContent[162]) * (ShellViewModel.UserSettings.DifficultyCustomizations["Normal"].SelectedExpRate + 1)).ToString();
                            splitContent[175] = (int.Parse(splitContent[175]) * (ShellViewModel.UserSettings.DifficultyCustomizations["Nightmare"].SelectedExpRate + 1)).ToString();
                            splitContent[188] = (int.Parse(splitContent[188]) * (ShellViewModel.UserSettings.DifficultyCustomizations["Hell"].SelectedExpRate + 1)).ToString();
                        }
                        contents += string.Join("\t", splitContent) + "\n";
                    }
                    index += 1;
                }
                await File.WriteAllTextAsync(_globalMonStatsTxtPath, contents);

                #endregion
                MessageBox.Show("Customizations applied successfully!");
                await ShellViewModel.SaveUserSettings();
            }

            if (Settings.Default.SelectedMod == "VNP-MP")
            {
                if (SelectedDifficulty.SelectedShortenedLevel == 1)
                    await File.WriteAllBytesAsync(ShellViewModel.SelectedModDataFolder + "/global/excel/levels.txt", Helper.GetResourceByteArray2("CASC.levels-vnp.txt"));
                else
                    await File.WriteAllBytesAsync(ShellViewModel.SelectedModDataFolder + "/global/excel/levels.txt", Helper.GetResourceByteArray2("CASC.levels.txt"));
            }
        }

        #endregion
    }

    public class DifficultyCustomizations : INotifyPropertyChanged
    {
        #region ---Static Members---

        private int _selectedChampionPack = 0;
        private int _selectedExpRate = 0;
        private int _selectedShortenedLevel;
        private int _actOneDensity;
        private int _actTwoDensity;
        private int _actThreeDensity;
        private int _actFourDensity;
        private int _actFiveDensity;
        private double _actOneSpawnChance;
        private double _actTwoSpawnChance;
        private double _actThreeSpawnChance;
        private double _actFourSpawnChance;
        private double _actFiveSpawnChance;
        private string _actOneMultiplierString;
        private string _actTwoMultiplierString;
        private string _actThreeMultiplierString;
        private string _actFourMultiplierString;
        private string _actFiveMultiplierString;
        private double _actOneMultiplier = 1;
        private double _actTwoMultiplier = 1;
        private double _actThreeMultiplier = 1;
        private double _actFourMultiplier = 1;
        private double _actFiveMultiplier = 1;
        public DifficultyCustomizations()
        {
            if (Execute.InDesignMode)
            {
                ActOneMultiplierString = "(20x)";
                ActTwoMultiplierString = "(20x)";
                ActThreeMultiplierString = "(20x)";
                ActFourMultiplierString = "(20x)";
                ActFiveMultiplierString = "(20x)";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        #endregion

        #region ---Properties---

        public int SelectedShortenedLevel
        {
            get => _selectedShortenedLevel;
            set
            {
                if (value == _selectedShortenedLevel)
                {
                    return;
                }
                _selectedShortenedLevel = value;
                OnPropertyChanged();
            }
        }
        public int SelectedChampionPack
        {
            get => _selectedChampionPack;
            set
            {
                if (value == _selectedChampionPack)
                {
                    return;
                }
                _selectedChampionPack = value;
                OnPropertyChanged();
            }
        }
        public int SelectedExpRate
        {
            get => _selectedExpRate;
            set
            {
                if (value == _selectedExpRate)
                {
                    return;
                }
                _selectedExpRate = value;
                OnPropertyChanged();
            }
        }
        public string ActOneMultiplierString
        {
            get => _actOneMultiplierString;
            set
            {
                if (value == _actOneMultiplierString)
                {
                    return;
                }
                _actOneMultiplierString = value;
                OnPropertyChanged();
            }
        }
        public string ActTwoMultiplierString
        {
            get => _actTwoMultiplierString;
            set
            {
                if (value == _actTwoMultiplierString)
                {
                    return;
                }
                _actTwoMultiplierString = value;
                OnPropertyChanged();
            }
        }
        public string ActThreeMultiplierString
        {
            get => _actThreeMultiplierString;
            set
            {
                if (value == _actThreeMultiplierString)
                {
                    return;
                }
                _actThreeMultiplierString = value;
                OnPropertyChanged();
            }
        }
        public string ActFourMultiplierString
        {
            get => _actFourMultiplierString;
            set
            {
                if (value == _actFourMultiplierString)
                {
                    return;
                }
                _actFourMultiplierString = value;
                OnPropertyChanged();
            }
        }
        public string ActFiveMultiplierString
        {
            get => _actFiveMultiplierString;
            set
            {
                if (value == _actFiveMultiplierString)
                {
                    return;
                }
                _actFiveMultiplierString = value;
                OnPropertyChanged();
            }
        }
        public double ActOneMultiplier
        {
            get => _actOneMultiplier;
            set
            {
                if (value.Equals(_actOneMultiplier))
                {
                    return;
                }
                _actOneMultiplier = value;
                OnPropertyChanged();
            }
        }
        public double ActTwoMultiplier
        {
            get => _actTwoMultiplier;
            set
            {
                if (value.Equals(_actTwoMultiplier))
                {
                    return;
                }
                _actTwoMultiplier = value;
                OnPropertyChanged();
            }
        }
        public double ActThreeMultiplier
        {
            get => _actThreeMultiplier;
            set
            {
                if (value.Equals(_actThreeMultiplier))
                {
                    return;
                }
                _actThreeMultiplier = value;
                OnPropertyChanged();
            }
        }
        public double ActFourMultiplier
        {
            get => _actFourMultiplier;
            set
            {
                if (value.Equals(_actFourMultiplier))
                {
                    return;
                }
                _actFourMultiplier = value;
                OnPropertyChanged();
            }
        }
        public double ActFiveMultiplier
        {
            get => _actFiveMultiplier;
            set
            {
                if (value.Equals(_actFiveMultiplier))
                {
                    return;
                }
                _actFiveMultiplier = value;
                OnPropertyChanged();
            }
        }
        public int ActOneDensity
        {
            get => _actOneDensity;
            set
            {
                if (value.Equals(_actOneDensity))
                {
                    return;
                }
                _actOneDensity = value;
                OnPropertyChanged();
            }
        }
        public int ActTwoDensity
        {
            get => _actTwoDensity;
            set
            {
                if (value.Equals(_actTwoDensity))
                {
                    return;
                }
                _actTwoDensity = value;
                OnPropertyChanged();
            }
        }
        public int ActThreeDensity
        {
            get => _actThreeDensity;
            set
            {
                if (value.Equals(_actThreeDensity))
                {
                    return;
                }
                _actThreeDensity = value;
                OnPropertyChanged();
            }
        }
        public int ActFourDensity
        {
            get => _actFourDensity;
            set
            {
                if (value.Equals(_actFourDensity))
                {
                    return;
                }
                _actFourDensity = value;
                OnPropertyChanged();
            }
        }
        public int ActFiveDensity
        {
            get => _actFiveDensity;
            set
            {
                if (value.Equals(_actFiveDensity))
                {
                    return;
                }
                _actFiveDensity = value;
                OnPropertyChanged();
            }
        }
        public double ActOneSpawnChance
        {
            get => _actOneSpawnChance;
            set
            {
                if (value.Equals(_actOneSpawnChance))
                {
                    return;
                }
                _actOneSpawnChance = value;
                OnPropertyChanged();
            }
        }
        public double ActTwoSpawnChance
        {
            get => _actTwoSpawnChance;
            set
            {
                if (value.Equals(_actTwoSpawnChance))
                {
                    return;
                }
                _actTwoSpawnChance = value;
                OnPropertyChanged();
            }
        }
        public double ActThreeSpawnChance
        {
            get => _actThreeSpawnChance;
            set
            {
                if (value.Equals(_actThreeSpawnChance))
                {
                    return;
                }
                _actThreeSpawnChance = value;
                OnPropertyChanged();
            }
        }
        public double ActFourSpawnChance
        {
            get => _actFourSpawnChance;
            set
            {
                if (value.Equals(_actFourSpawnChance))
                {
                    return;
                }
                _actFourSpawnChance = value;
                OnPropertyChanged();
            }
        }
        public double ActFiveSpawnChance
        {
            get => _actFiveSpawnChance;
            set
            {
                if (value.Equals(_actFiveSpawnChance))
                {
                    return;
                }
                _actFiveSpawnChance = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}