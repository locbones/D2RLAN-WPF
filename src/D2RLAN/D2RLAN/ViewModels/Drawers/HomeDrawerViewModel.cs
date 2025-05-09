using Caliburn.Micro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using D2RLAN.Models.Enums;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using D2RLAN.Properties;
using Syncfusion.Licensing;
using System.ComponentModel.DataAnnotations;
using System;
using System.Dynamic;
using D2RLAN.Culture;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using JetBrains.Annotations;
using System.Threading;
using System.Windows;
using D2RLAN.Extensions;
using D2RLAN.Models;
using D2RLAN.ViewModels.Dialogs;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;
using System.Text;

namespace D2RLAN.ViewModels.Drawers;

public class HomeDrawerViewModel : INotifyPropertyChanged
{
    #region ---Static Members---
    private const string TAB_BYTE_CODE = "55AA55AA0100000062000000000000004400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000004A4D0000";
    private const string TAB_BYTE_CODE_FULL = "55AA55AA010000006200000000000000D407000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000004A4D3300100080000500F4B02BDCC5DA5E8D0DDBA392389959EA938529173BD0D070200A8418277052E46C81E257FC07100080000508D4FC419039A8821B1BB75913D4263933104E060878604C7860C4D022294B5923F815FF01100080000510F49E15FA440AC8C7C6AE55991445494006302E24588120C349E13F100080000518D4AA09D4772CE563C396AD623E64245AE435A00084830536104AA4D052123E91F51F10008000052054B10DD2A29DE06383F4E7239A6661C2257FE2C40B502840609022274BBA69B99381570C4092112630FE03100080000528F4AA09400E65676343B46D96F2E3353EA44D6800030201A404430B704EF02819FF03100880000538149A199E865C17C686ED534ACCC3997C4BD244880BF1C490042E109E8818C6B0DC18FD07100080000530D4E80D01DC762B63C3152E67FA64350002120C0E3C5940D3E2E83F10008000057014FE4C1073AE181E1BA1596572281E0497B0FA71A045029631A5FF1000800005F8546036586A3A018C0D57A38F68909CD12658A18543822308125A2C8DF1FC071000800005C0548637305A55C28F8DDEA6905033DA0456188D48600401438B9EFF100080000580951B0A555C1DEA6323F7EA50CA264CCA45402003D48A042323841338297056E0B4C0D902956C8041F11F1000800005A8743C2EEC6428418C0DCF97911894F119939A691062A098108074200404142D7EC67878A500E83F1000800005A0743C4CC04059DF181BC141393528E153260B132E34400121C0E90009089C52ECACC117372DBC5200F21F10008000058016C25F985E8B741D1BBB4CE5922FCA06BAA1580020E58451B20106C57F10008000059874036BD8946B911B1BBD75637236239116AFB12E7E80D1000049029505371040D8083020938C8081F11F1000800005C8941B1C380FB54DC7463053C37CC848EC8B79A0931B18005620A01031A4105232FE071000800005D094630DF39B9C136323B62C247A64613EE4571A0410302A40C8900267C58F12114ADB18CF7F1000800005D8743C7258D7C1281E1BAC3D37B428395332C2C52128261C10D2E268CD3CB9690927F01BFE0310008000053015DE1508E48B1AC686AF584E1DD5B0B4C5C2748824302080E02905CE0A252562178F9DFF01100080000528753C1A264FEF55C6862DD04A8C49AF844BB99C0C4E505008508230E644CF0A9D35F5E2A6855FF11F100080000588753D1EC60EC0F4C78627C2138DD078D4C58320032242972E139681E1C04246F00AB380068485FF10008000058856BE2FC06B80118F0D5C8802C9971E028F0B00522268D030E1041B0C2B64FE031000800005205528086D267B866343F4254212632F6A07241F2250A234811B0C026606042834FF01100080000550D5910DD2BD0D206323D520911A232F8203160B229C385161B06098C2FC071000800005B815BF1B046517ADC6462F415F54C715158B6B72E322C28C19396EB8A10F8F0433C38F8691FF10008000053CF73F6F0060A0241E0B04025CF80F10008000055475D917A2A35B86C746A9E57A9C8592D12E784A00409910918D030C96FC86FF1000800005B0F53E1EC88C06FDC6062AC44E4DC58104C6B9D4058F0D1100029C80C30922180A3364085974E1FD071000800005A8F5F12178BF064E8F0DDE84C1989601B91B6030883068D004097C24860D3520FE0310008000055C15DDDB85BB2038F0B171ABD920C1911ED7212A0804B01281099309E5542072D86003992EB24FC47F1000C0000510D69186659F3CB0B1518A92178FD11D6551207D212242041F3E26B86041212358C006C102B2FF1008800005E0D55917EE561525C746F042D24C45604CC67954079E151261CC98096960A0E4C00083255D78FF01100080000514D6930D8063833663E396F28952E48402E640B0A000802402081013689011905227A0CDF80F100080000518D6492754BF10D78D8DDC930259A7BC988BF5006342041E3C262404E300023362110C8FFF10008000051C96A413B693CC0CC6C668CD7A34C5291D8660413463422F16023C0A5D3C7A68F019E53F100080000568D62912F869A872C6062AC399444577AC4C0F01A180508298090ED8781021D345D6AFF80F1000800005BC760136CCAC4CEE8C8DD89A9BA9A40D99163C2468E1E323B1064BE07C9815567C84FD0710008000057076FD285C9D82EF8D0D4FCB071997214A9A27FA0D32252278F132A1910AC1850086C50D19012C5B8C86112428FE030000800005741611147EFDE980C7066DCD877CC5D498224628C8081C4A8382F3CF43674F41FF010000800005D0F6AF4C18DFF4511D9B0C08051C9AB0199CAF800787032DD2A0D6EC98A200AF0400191B6050FC070000800005B856EF185893C677C70668C15DB4D41E9DC5CC140B0C0B113C78687163030185250FC414C9582044E13F1000800005D4F6AFFBA003BB4B3D361D105C33480514451FE22B503038204446A08F12714A9406B5868798212D38F0FC071000800005D856582638DD98BF8C299C242B0EE84880F21F1000800005DC764A9FC0CEE3463016FC0710008000052C17DD0082FD4D58FDD892E860C6880B9181850486448A22390CA8C47F1000A0000528F7304C00100080000524F7FCD8A02F56E83BB680FAF51DF93023FB22520060398062820923022869FF011000A00005E4764D101000A0000520F730FB001000A00005E076090D";
    private ILog _logger = LogManager.GetLogger(typeof(HomeDrawerViewModel));
    private IWindowManager _windowManager;
    private string _launcherDescription = "This application is used to download and configure mods for D2R.";
    private string _launcherTitle = "D2RLAN";
    private string _modDescription = "Please create a blank mod or download a new mod using the options below.";
    private string _modTitle = "No Mods Detected!";
    private ObservableCollection<KeyValuePair<string, eLanguage>> _languages = new ObservableCollection<KeyValuePair<string, eLanguage>>();
    private KeyValuePair<string, eLanguage> _selectedAppLanguage;
    private ObservableCollection<string> _installedMods;
    private string _selectedMod;
    private bool _mapsComboBoxEnabled;
    private bool _uiComboBoxEnabled;
    private bool _checkingForUpdates;
    private double _downloadProgress;
    private bool _progressBarIsIndeterminate;
    private string _progressStatus;
    private string _downloadProgressString;
    private bool _directTxtEnabled;
    private bool _hdrOpacityFixEnabled;
    private bool _mapRegenEnabled;
    private bool _respecEnabled;
    private ObservableCollection<KeyValuePair<string, eMapLayouts>> _mapLayouts = new ObservableCollection<KeyValuePair<string, eMapLayouts>>();
    private ObservableCollection<KeyValuePair<string, eWindowMode>> _windowMode = new ObservableCollection<KeyValuePair<string, eWindowMode>>();
    private ObservableCollection<KeyValuePair<string, eUiThemes>> _uiThemes = new ObservableCollection<KeyValuePair<string, eUiThemes>>();
    private DispatcherTimer _monsterStatsDispatcherTimer;
    private bool _uiThemeEnabled = true;
    private bool _cheatsEnabled = true;
    private string _d2rArgs;

    // P/Invoke declarations
    private const int GWL_STYLE = -16;
    private const int GWL_EXSTYLE = -20;
    private const uint WS_CAPTION = 0x00C00000;
    private const uint WS_THICKFRAME = 0x00040000;
    private const uint WS_EX_CLIENTEDGE = 0x00000200;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    #endregion

    #region ---Window/Loaded Handlers---

    public HomeDrawerViewModel()
    {
        if (Execute.InDesignMode)
        {
            DownloadProgressString = "70%";
            ProgressStatus = "Test Progress Status...";
        }
    }
    public HomeDrawerViewModel(ShellViewModel shellViewModel, IWindowManager windowManager)
    {
        ShellViewModel = shellViewModel;
        _windowManager = windowManager;


        //_monsterStatsDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        //_monsterStatsDispatcherTimer.Tick += (sender, args) => MonsterStatsDispatcherTimerOnTick(ShellViewModel.UserSettings);
        //_monsterStatsDispatcherTimer.Interval = TimeSpan.FromSeconds(15);
    }
    public async Task Initialize()
    {
        foreach (eMapLayouts mapLayout in Enum.GetValues<eMapLayouts>())
        {
            MapLayouts.Add(new KeyValuePair<string, eMapLayouts>(mapLayout.GetAttributeOfType<DisplayAttribute>().Name, mapLayout));
        }

        foreach (eWindowMode windowMode in Enum.GetValues<eWindowMode>())
        {
            WindowMode.Add(new KeyValuePair<string, eWindowMode>(windowMode.GetAttributeOfType<DisplayAttribute>().Name, windowMode));
        }

        foreach (eUiThemes uiTheme in Enum.GetValues<eUiThemes>())
        {
            UiThemes.Add(new KeyValuePair<string, eUiThemes>(uiTheme.GetAttributeOfType<DisplayAttribute>().Name, uiTheme));
        }

        ImageSource = "pack://application:,,,/Resources/Images/ChatGem1b.png";
        ImageRNG = 0;

        await InitializeLanguage();
        await InitializeMods();
        GetD2RArgs();
        if (ShellViewModel.UserSettings != null)
        {
            ShellViewModel.UserSettings.MapSeed = "";
            ShellViewModel.UserSettings.MapSeedName = "An Evil Force's Seed: ";
        }

        if (SelectedMod == "RMD-MP")
        {
            ShellViewModel.UserSettings.Cheats = false;
            ShellViewModel.UserSettings.CheatsActive = false;
            CheatsEnabled = false;
        }


    }
    public async Task InitializeLanguage()
    {
        eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);
        SelectedAppLanguage = new KeyValuePair<string, eLanguage>(appLanguage.GetAttributeOfType<DisplayAttribute>().Name, appLanguage);

        foreach (eLanguage language in Enum.GetValues<eLanguage>())
        {
            Languages.Add(new KeyValuePair<string, eLanguage>(language.GetAttributeOfType<DisplayAttribute>().Name, language));
        }

        await Translate();
    }
    public async Task InitializeMods()
    {
        string[] modFolders = null;

        if (Directory.Exists(ShellViewModel.BaseModsFolder))
        {
            modFolders = Directory.GetDirectories(ShellViewModel.BaseModsFolder);

            InstalledMods = new ObservableCollection<string>(modFolders.Where(m => !m.ToUpperInvariant().Contains("Backup".ToUpperInvariant())).Select(Path.GetFileName));

            if (Directory.Exists(ShellViewModel.BaseSelectedModFolder))
            {
                if (!string.IsNullOrEmpty(Settings.Default.SelectedMod))
                {
                    SelectedMod = Settings.Default.SelectedMod;
                    ShellViewModel.ModInfo = await Helper.ParseModInfo(ShellViewModel.SelectedModInfoFilePath);

                    await Translate();

                    if (ShellViewModel.ModInfo == null)
                        return;

                    MapsComboBoxEnabled = ShellViewModel.ModInfo.MapLayouts;
                    UiComboBoxEnabled = ShellViewModel.ModInfo.UIThemes && (ShellViewModel.ModInfo.Name == "VNP-MP" || ShellViewModel.ModInfo.Name == "RMD-MP");
                    ShellViewModel.CustomizationsEnabled = ShellViewModel.ModInfo.Customizations;

                    //Disable RW Sort+Merged HUD if author enabled without providing template files
                    if (!Directory.Exists(System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/HUD Design")))
                        ShellViewModel.ModInfo.HudDisplay = false;

                    if (!Directory.Exists(System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/Runeword Sort")))
                        ShellViewModel.ModInfo.RunewordSorting = false;

                    string logoPath = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/Logo.png");
                    if (File.Exists(logoPath))
                    {
                        string tempPath = System.IO.Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                        File.Copy(logoPath, tempPath, true);
                        ShellViewModel.ModLogo = tempPath;
                    }
                    else
                        ShellViewModel.ModLogo = "pack://application:,,,/Resources/Images/D2RLAN_Logo.png";

                    await LoadUserSettings();

                    await ShellViewModel.StartAutoBackup();

                    if (ShellViewModel.ModInfo.Name == "RMD-MP")
                    {
                        UiThemeEnabled = true;
                        ShellViewModel.WikiEnabled = true;
                        ShellViewModel.ShowItemLevelsEnabled = true;
                        ShellViewModel.UserSettings.SuperTelekinesis = 1;
                        ShellViewModel.SuperTelekinesisEnabled = false;
                        ShellViewModel.SkillBuffIconsEnabled = false;
                        ShellViewModel.UserSettings.SkillIcons = 1;
                        ShellViewModel.SkillIconPackEnabled = false;
                        ShellViewModel.ItemIconDisplayEnabled = false;
                        //ShellViewModel.UserSettings.ItemIlvls = 1;
                        ShellViewModel.ExpandedInventoryEnabled = false;
                        ShellViewModel.ExpandedStashEnabled = false;
                        ShellViewModel.ExpandedCubeEnabled = false;
                        ShellViewModel.ExpandedMercEnabled = false;
                        ShellViewModel.ColorDyesEnabled = false;
                    }
                    else
                    {
                        UiThemeEnabled = false;
                        ShellViewModel.WikiEnabled = true;
                        ShellViewModel.UserSettings.UiTheme = 1;

                        ShellViewModel.ShowItemLevelsEnabled = true;
                        ShellViewModel.SuperTelekinesisEnabled = true;
                        ShellViewModel.SkillBuffIconsEnabled = true;
                        ShellViewModel.SkillIconPackEnabled = true;
                        ShellViewModel.ItemIconDisplayEnabled = true;
                        ShellViewModel.ExpandedInventoryEnabled = true;
                        ShellViewModel.ExpandedStashEnabled = true;
                        ShellViewModel.ExpandedCubeEnabled = true;
                        ShellViewModel.ExpandedMercEnabled = true;
                        ShellViewModel.ColorDyesEnabled = true;
                    }

                    GetD2RArgs();
                    
                    //await ApplyUiTheme();
                }
            }
        }

    }
    private async Task LoadUserSettings()
    {
        //Protected
        if (Directory.Exists(ShellViewModel.SelectedModDataFolder))
        {
            if (!File.Exists(ShellViewModel.SelectedUserSettingsFilePath))
            {
                if (!File.Exists(ShellViewModel.OldSelectedUserSettingsFilePath))
                    ShellViewModel.UserSettings = await Helper.GetDefaultUserSettings();
                else
                {
                    string[] oldUserSettings = await File.ReadAllLinesAsync(ShellViewModel.OldSelectedUserSettingsFilePath);
                    ShellViewModel.UserSettings = await Helper.ConvertUserSettings(oldUserSettings);
                }
            }
            else
                ShellViewModel.UserSettings = JsonConvert.DeserializeObject<UserSettings>(await File.ReadAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath));
        }
        else //Unprotected
        {
            if (!File.Exists(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
            {
                if (!File.Exists(ShellViewModel.OldSelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
                    ShellViewModel.UserSettings = await Helper.GetDefaultUserSettings();
                else
                {
                    string[] oldUserSettings = await File.ReadAllLinesAsync(ShellViewModel.OldSelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", ""));
                    ShellViewModel.UserSettings = await Helper.ConvertUserSettings(oldUserSettings);
                }
            }
            else
                ShellViewModel.UserSettings = JsonConvert.DeserializeObject<UserSettings>(await File.ReadAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")));
        }

        //TODO: Should the autoback up timer be configured here?
        //TODO:_profilehd.json be setup here?
    }

    #endregion

    #region ---Properties---

    public ShellViewModel ShellViewModel { get; }
    public bool CheatsEnabled
    {
        get => _cheatsEnabled;
        set
        {
            if (value == _cheatsEnabled) return;
            _cheatsEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool UiThemeEnabled
    {
        get => _uiThemeEnabled;
        set
        {
            if (value == _uiThemeEnabled) return;
            _uiThemeEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool DirectTxtEnabled
    {
        get => _directTxtEnabled;
        set
        {
            if (value == _directTxtEnabled) return;
            _directTxtEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool HdrOpacityFixEnabled
    {
        get => _hdrOpacityFixEnabled;
        set
        {
            if (value == _hdrOpacityFixEnabled) return;
            _hdrOpacityFixEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool MapRegenEnabled
    {
        get => _mapRegenEnabled;
        set
        {
            if (value == _mapRegenEnabled) return;
            _mapRegenEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool RespecEnabled
    {
        get => _respecEnabled;
        set
        {
            if (value == _respecEnabled) return;
            _respecEnabled = value;
            OnPropertyChanged();
        }
    }
    public string ProgressStatus
    {
        get => _progressStatus;
        set
        {
            if (value == _progressStatus) return;
            _progressStatus = value;
            OnPropertyChanged();
        }
    }
    public bool ProgressBarIsIndeterminate
    {
        get => _progressBarIsIndeterminate;
        set
        {
            if (value == _progressBarIsIndeterminate) return;
            _progressBarIsIndeterminate = value;
            OnPropertyChanged();
        }
    }
    public string DownloadProgressString
    {
        get => _downloadProgressString;
        set
        {
            if (value == _downloadProgressString) return;
            _downloadProgressString = value;
            OnPropertyChanged();
        }
    }
    public double DownloadProgress
    {
        get => _downloadProgress;
        set
        {
            if (value.Equals(_downloadProgress)) return;
            _downloadProgress = value;
            OnPropertyChanged();
        }
    }
    public bool CheckingForUpdates
    {
        get => _checkingForUpdates;
        set
        {
            if (value == _checkingForUpdates) return;
            _checkingForUpdates = value;
            OnPropertyChanged();
        }
    }
    public bool UiComboBoxEnabled
    {
        get => _uiComboBoxEnabled;
        set
        {
            if (value == _uiComboBoxEnabled) return;
            _uiComboBoxEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool MapsComboBoxEnabled
    {
        get => _mapsComboBoxEnabled;
        set
        {
            if (value == _mapsComboBoxEnabled) return;
            _mapsComboBoxEnabled = value;
            OnPropertyChanged();
        }
    }
    public string SelectedMod
    {
        get => _selectedMod;
        set
        {
            if (value == _selectedMod) return;
            _selectedMod = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> InstalledMods
    {
        get => _installedMods;
        set
        {
            if (Equals(value, _installedMods)) return;
            _installedMods = value;
            OnPropertyChanged();
        }
    }
    public KeyValuePair<string, eLanguage> SelectedAppLanguage
    {
        get => _selectedAppLanguage;
        set
        {
            if (value.Equals(_selectedAppLanguage)) return;
            _selectedAppLanguage = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<KeyValuePair<string, eUiThemes>> UiThemes
    {
        get => _uiThemes;
        set
        {
            if (Equals(value, _uiThemes)) return;
            _uiThemes = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<KeyValuePair<string, eMapLayouts>> MapLayouts
    {
        get => _mapLayouts;
        set
        {
            if (Equals(value, _mapLayouts)) return;
            _mapLayouts = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<KeyValuePair<string, eWindowMode>> WindowMode
    {
        get => _windowMode;
        set
        {
            if (Equals(value, _windowMode)) return;
            _windowMode = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<KeyValuePair<string, eLanguage>> Languages
    {
        get => _languages;
        set
        {
            if (Equals(value, _languages)) return;
            _languages = value;
            OnPropertyChanged();
        }
    }
    public string ModTitle
    {
        get => _modTitle;
        set
        {
            if (value == _modTitle)
            {
                return;
            }
            _modTitle = value;
            OnPropertyChanged();
        }
    }
    public string ModDescription
    {
        get => _modDescription;
        set
        {
            if (value == _modDescription)
            {
                return;
            }
            _modDescription = value;
            OnPropertyChanged();
        }
    }
    public string LauncherTitle
    {
        get => _launcherTitle;
        set
        {
            if (value == _launcherTitle)
            {
                return;
            }
            _launcherTitle = value;
            OnPropertyChanged();
        }
    }
    public string LauncherDescription
    {
        get => _launcherDescription;
        set
        {
            if (value == _launcherDescription)
            {
                return;
            }
            _launcherDescription = value;
            OnPropertyChanged();
        }
    }
    public string D2RArgsText
    {
        get => _d2rArgs;
        set
        {
            if (value == _d2rArgs) return;
            _d2rArgs = value;
            OnPropertyChanged();
        }
    }
    public static string _imageSource;
    private string _imageHint;
    public static int _imageRNG;
    public string ImageSource
    {
        get => _imageSource;
        set
        {
            if (value == _imageSource) return;
            _imageSource = value;
            OnPropertyChanged();
        }
    }
    public string ImageHint
    {
        get => _imageHint;
        set
        {
            if (value == _imageHint) return;
            _imageHint = value;
            OnPropertyChanged();
        }
    }
    public static int ImageRNG
    {
        get => _imageRNG;
        set
        {
            if (value == _imageRNG) return;
            _imageRNG = value;
            OnStaticPropertyChanged(nameof(ImageRNG));
        }
    }
    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
    private static void OnStaticPropertyChanged(string propertyName)
    {
        StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

    #endregion

    #region ---Launch Arguments/Game Start---

    [UsedImplicitly]
    public async void OnPlayMod()
    {
        if (!File.Exists("D2RHUD.dll"))
            DownloadD2RHUDZip();

        if (ShellViewModel.ModInfo == null)
            return;

        await ApplyHdrFix();
        await ApplyCinematicSkip();
        await ShellViewModel.ApplyModSettings();
        GetD2RArgs();
        StashMigration();

        ShellViewModel.DisableBNetConnection();

        //Add Exocet Font to D2R base Folder for Monster Stat Display (mod agnostic)
        if (!File.Exists(ShellViewModel.GamePath + "/Exocet.otf"))
        {
            byte[] font = await Helper.GetResourceByteArray($"Fonts.0.otf");
            await File.WriteAllBytesAsync(ShellViewModel.GamePath + "/Exocet.otf", font);
        }
    }
    public string GetD2RArgs()
    {
        string args = string.Empty;
        string regenArg = ShellViewModel?.UserSettings?.ResetMaps ?? false ? " -resetofflinemaps" : string.Empty;
        string respecArg = ShellViewModel?.UserSettings?.InfiniteRespec ?? false ? " -enablerespec" : string.Empty;
        string windowedArg = ShellViewModel?.UserSettings?.WindowMode >= 1 ? " -windowed" : string.Empty;
        string cheatsArg = ShellViewModel?.UserSettings?.Cheats ?? false ? " -cheats" : string.Empty;
        string mapLayoutArg = GetMapLayoutArg();

        string excelDir = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel");

        if (Directory.Exists(excelDir))
        {
            int binFileCount = Directory.GetFiles(excelDir, "*.bin").Length;
            int txtFileCount = Directory.GetFiles(excelDir, "*.txt").Length;

            if (binFileCount >= 81 && txtFileCount >= 10)
                args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
            else if (binFileCount >= 81 && txtFileCount < 10)
                args = $"-mod {ShellViewModel.ModInfo.Name}";
            else if (binFileCount < 81 && txtFileCount >= 1)
                args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
        }
        else
        {
            if (ShellViewModel.ModInfo != null)
                args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
            else
                args = "";
        }

        string mArgs = args;

        args = $"{mArgs}{regenArg}{respecArg}{mapLayoutArg}{windowedArg}{cheatsArg}";

        if (ShellViewModel?.UserSettings == null)
            return string.Empty;

        ShellViewModel.UserSettings.CurrentD2RArgs = args;
        return args;
    }
    private string GetMapLayoutArg()
    {
        if (ShellViewModel?.UserSettings == null)
            return string.Empty;

        string arg = string.Empty;

        switch ((eMapLayouts)ShellViewModel.UserSettings.MapLayout)
        {
            case eMapLayouts.Default:
                return "";
            case eMapLayouts.Tower:
                return " -seed 1112";
            case eMapLayouts.Catacombs:
                return " -seed 348294647";
            case eMapLayouts.AncientTunnels:
                return " -seed 1111";
            case eMapLayouts.LowerKurast:
                return " -seed 1460994795";
            case eMapLayouts.DuranceOfHate:
                return " -seed 1113";
            case eMapLayouts.Hellforge:
                return " -seed 100";
            case eMapLayouts.WorldstoneKeep:
                return " -seed 1104";
            case eMapLayouts.Cheater:
                return " -seed 1056279548";
            default:
                return "";
        }
    }
    private void ExtendDiabloWindowToFullScreen()
    {
        Process[] processes = Process.GetProcesses();
        bool foundDiabloWindow = false;

        foreach (Process process in processes)
        {
            IntPtr mainWindowHandle = process.MainWindowHandle;
            if (mainWindowHandle != IntPtr.Zero)
            {
                string windowTitle = process.MainWindowTitle.ToLower();
                if (windowTitle.Contains("diablo"))
                {
                    foundDiabloWindow = true;

                    // Remove title bar, border, and client edge
                    uint style = GetWindowLong(mainWindowHandle, GWL_STYLE);
                    uint exStyle = GetWindowLong(mainWindowHandle, GWL_EXSTYLE);

                    style &= ~(WS_CAPTION | WS_THICKFRAME); // Remove title bar and resizable border
                    exStyle &= ~WS_EX_CLIENTEDGE; // Remove client edge

                    SetWindowLong(mainWindowHandle, GWL_STYLE, style);
                    SetWindowLong(mainWindowHandle, GWL_EXSTYLE, exStyle);

                    // Update window position and size to cover the entire screen
                    IntPtr desktopHandle = GetDesktopWindow();
                    RECT desktopRect;
                    GetWindowRect(desktopHandle, out desktopRect);

                    SetWindowPos(mainWindowHandle, IntPtr.Zero, desktopRect.Left, desktopRect.Top, desktopRect.Right - desktopRect.Left, desktopRect.Bottom - desktopRect.Top, SWP_FRAMECHANGED);

                    //MessageBox.Show("Diablo window extended to full screen resolution.");
                    break; // Once we find a Diablo window, no need to continue looping
                }
            }
        }
    }
    private void RemoveDiabloWindowTitleBarAndBorder()
    {
        Process[] processes = Process.GetProcesses();
        bool foundDiabloWindow = false;

        foreach (Process process in processes)
        {
            IntPtr mainWindowHandle = process.MainWindowHandle;
            if (mainWindowHandle != IntPtr.Zero)
            {
                string windowTitle = process.MainWindowTitle.ToLower();
                if (windowTitle.Contains("diablo"))
                {
                    foundDiabloWindow = true;

                    // Modify the window style to remove the title bar and border
                    uint style = GetWindowLong(mainWindowHandle, GWL_STYLE);
                    uint exStyle = GetWindowLong(mainWindowHandle, GWL_EXSTYLE);

                    style &= ~(WS_CAPTION | WS_THICKFRAME); // Remove title bar and resizable border
                    exStyle &= ~WS_EX_CLIENTEDGE; // Remove client edge

                    SetWindowLong(mainWindowHandle, GWL_STYLE, style);
                    SetWindowLong(mainWindowHandle, GWL_EXSTYLE, exStyle);

                    // Update window position to refresh the appearance
                    SetWindowPos(mainWindowHandle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);

                    //MessageBox.Show("Title bar, border, and client edge removed from Diablo window.");
                    break; // Once we find a Diablo window, no need to continue looping
                }
            }
        }
    }
    public async void DownloadD2RHUDZip() //Download Expanded File Package
    {
        string url = "https://github.com/locbones/D2RHUD-2.4/archive/refs/heads/main.zip";
        string savePath = "D2RHUD.zip";
        string extractPathTemp = "./";

        if (File.Exists(savePath))
            File.Delete(savePath);

        using (WebClient client = new WebClient())
        {
            client.DownloadFileCompleted += (sender, e) =>
            {
                try
                {
                    if (e.Error == null)
                    {
                        if (Directory.Exists(extractPathTemp + "D2RHud-2.4-main"))
                            Directory.Delete(extractPathTemp + "D2RHud-2.4-main", true);

                        ZipFile.ExtractToDirectory(savePath, extractPathTemp);
                        _logger.Error("Monster Stats: D2RHUD Downloaded and Extracted");

                        File.Copy(extractPathTemp + "D2RHUD-2.4-main/x64/Release/D2RHUD.dll", "D2RHUD.dll", true);
                        _logger.Error($"Monster Stats: D2RHUD.dll copied to Launcher folder");

                        File.Delete(savePath);
                        Directory.Delete(extractPathTemp + "/D2Rhud-2.4-main", true);
                        _logger.Error($"Monster Stats: D2RHUD Cleanup Completed");
                    }
                    else
                        MessageBox.Show($"An error occurred during download: {e.Error.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            };

            try
            {
                client.DownloadFileAsync(new Uri(url), savePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }

    public void StashMigration()
    {
        string savePath = Path.Combine(GetSavePath(), $@"Diablo II Resurrected\Mods\{SelectedMod}");
        string saveFileSC = Path.Combine(savePath, "SharedStashSoftCoreV2.d2i");
        string saveFileHC = Path.Combine(savePath, "SharedStashHardCoreV2.d2i");
        string prefixSC = Path.Combine(savePath, "Stash_SC");
        string prefixHC = Path.Combine(savePath, "Stash_HC");

        ProcessStashFile(saveFileSC, prefixSC);
        ProcessStashFile(saveFileHC, prefixHC);
    }

    private void ProcessStashFile(string inputFilePath, string outputFilePrefix)
    {
        if (!File.Exists(inputFilePath))
            return;

        string firstPagePath = $"{outputFilePrefix}_Page1.d2i";
        if (File.Exists(firstPagePath))
            return;

        byte[] data = File.ReadAllBytes(inputFilePath);
        byte[] pattern = new byte[] { 0x55, 0xAA, 0x55, 0xAA };
        List<int> markerIndices = new List<int>();

        for (int i = 0; i <= data.Length - pattern.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (data[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                markerIndices.Add(i);
            }
        }

        int page = 1;
        for (int i = 0; i < markerIndices.Count; i += 7)
        {
            int start = markerIndices[i];
            int end = (i + 7 < markerIndices.Count) ? markerIndices[i + 7] : data.Length;

            int length = end - start;
            if (length <= 0) continue;

            byte[] output = new byte[length];
            Array.Copy(data, start, output, 0, length);

            string outputPath = $"{outputFilePrefix}_Page{page}.d2i";
            File.WriteAllBytes(outputPath, output);
            page++;
        }

        _logger.Error("Stash Migration completed!");
    }


    #endregion

    #region ---Translation Functions---

    private async Task Translate()
    {
        if (ShellViewModel.ModInfo != null)
        {
            if (string.IsNullOrEmpty(ShellViewModel.ModInfo.ModTitle))
            {
                ModTitle = "No News Found!";
                ModDescription = "The mod author has not specified any news messages for this mod";
                LauncherTitle = "Add D2RLAN Support";
                LauncherDescription = "Unlock one-click mod updates, additional QoL controls, live news display and more. It's as easy as editing a single already included file to add D2RLAN support to your mod. Visit the D2RModding Discord for more info.";

                return;
            }

            if (SelectedAppLanguage.Value == eLanguage.English)
            {
                ModTitle = ShellViewModel.ModInfo.ModTitle.Trim().Replace("\"", "");
                ModDescription = ShellViewModel.ModInfo.ModDescription.Trim().Replace("|| ", ".").Replace(@"\u0026", ". ").Replace("\\n", Environment.NewLine);

                LauncherTitle = ShellViewModel.ModInfo.NewsTitle.Trim().Replace("\"", "");
                //LauncherDescription = ShellViewModel.ModInfo.NewsDescription.Trim().Replace("\"", "");
                LauncherDescription = ShellViewModel.ModInfo.NewsDescription.Trim().Replace("|| ", ".").Replace(@"\u0026", ". ").Replace("\\n", Environment.NewLine);

                return;
            }
            string pattern = @"(?<![0-9])\.(?![0-9])"; // Matches a period not surrounded by digits

            string modTitle = Regex.Replace(ShellViewModel.ModInfo.ModTitle.Trim().Replace("\"", ""), pattern, "||");
            string modDescription = Regex.Replace(ShellViewModel.ModInfo.ModDescription.Trim().Replace("\"", ""), pattern, "||");
            string launcherTitle = Regex.Replace(ShellViewModel.ModInfo.NewsTitle.Trim().Replace("\"", ""), pattern, "||");
            string launcherDescription = Regex.Replace(ShellViewModel.ModInfo.NewsDescription.Trim().Replace("\"", ""), pattern, "||");

            try
            {
                ModTitle = await TranslateGoogleAsync(modTitle);
                ModDescription = await TranslateGoogleAsync(modDescription.Replace("|| ", ".").Replace(@"\u0026", ". "));
                LauncherTitle = await TranslateGoogleAsync(launcherTitle.Replace("|| ", ".").Replace(@"\u0026", ". ").Replace("\\n", Environment.NewLine));
                //LauncherDescription = await TranslateGoogleAsync(launcherDescription.Replace("|| ", ".").Replace(@"\u0026", ". "));
                LauncherDescription = await TranslateGoogleAsync(launcherDescription.Replace("|| ", ".").Replace(@"\u0026", ". ").Replace("\\n", Environment.NewLine));
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to translate test with google translate.");
                _logger.Error(ex);
            }
        }
        else
        {
            try
            {
                ModTitle = await TranslateGoogleAsync(ModTitle);
                ModDescription = await TranslateGoogleAsync(ModDescription);
                LauncherTitle = await TranslateGoogleAsync(LauncherTitle);
                LauncherDescription = await TranslateGoogleAsync(LauncherDescription);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to translate test with google translate.");
                _logger.Error(ex);
            }
        }
    }
    private async Task<string> TranslateGoogleAsync(string text)
    {
        try
        {
            text = Uri.EscapeDataString(text);
            string uri = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={Resources.Culture.Name}&dt=t&q={text}";

            using HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            string translation = responseBody.Split('"')[1].Trim();
            return translation;
        }
        catch (Exception ex)
        {
            _logger.Error($"Translation failed for: {text}");
            _logger.Error(ex);
            return null;
        }
    }
    [UsedImplicitly]
    public async void OnTextLanguageSelectionChanged()
    {
        if (ShellViewModel.UserSettings != null)
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\OSI", true) ?? throw new Exception("Failed to find registry key");

            switch (ShellViewModel.UserSettings.TextLanguage)
            {
                case 0:
                    key.SetValue("LOCALE", "enUS");
                    break;
                case 1:
                    key.SetValue("LOCALE", "deDE");
                    break;
                case 2:
                    key.SetValue("LOCALE", "esES");
                    break;
                case 3:
                    key.SetValue("LOCALE", "esMX");
                    break;
                case 4:
                    key.SetValue("LOCALE", "frFR");
                    break;
                case 5:
                    key.SetValue("LOCALE", "itIT");
                    break;
                case 6:
                    key.SetValue("LOCALE", "jaJP");
                    break;
                case 7:
                    key.SetValue("LOCALE", "koKR");
                    break;
                case 8:
                    key.SetValue("LOCALE", "plPL");
                    break;
                case 9:
                    key.SetValue("LOCALE", "ptBR");
                    break;
                case 10:
                    key.SetValue("LOCALE", "ruRU");
                    break;
                case 11:
                    key.SetValue("LOCALE", "zhCN");
                    break;
                case 12:
                    key.SetValue("LOCALE", "zhTW");
                    break;
                default:
                    key.SetValue("LOCALE", "enUS");
                    break;
            }
            key.Close();
        }
    }
    [UsedImplicitly]
    public async void OnAudioLanguageSelectionChanged()
    {
        if (ShellViewModel.UserSettings != null)
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\OSI", true) ?? throw new Exception("Failed to find registry key");

            switch (ShellViewModel.UserSettings.AudioLanguage)
            {
                case 0:
                    key.SetValue("LOCALE_AUDIO", "enUS");
                    break;
                case 1:
                    key.SetValue("LOCALE_AUDIO", "deDE");
                    break;
                case 2:
                    key.SetValue("LOCALE_AUDIO", "esES");
                    break;
                case 3:
                    key.SetValue("LOCALE_AUDIO", "esMX");
                    break;
                case 4:
                    key.SetValue("LOCALE_AUDIO", "frFR");
                    break;
                case 5:
                    key.SetValue("LOCALE_AUDIO", "itIT");
                    break;
                case 6:
                    key.SetValue("LOCALE_AUDIO", "jaJP");
                    break;
                case 7:
                    key.SetValue("LOCALE_AUDIO", "koKR");
                    break;
                case 8:
                    key.SetValue("LOCALE_AUDIO", "plPL");
                    break;
                case 9:
                    key.SetValue("LOCALE_AUDIO", "ptBR");
                    break;
                case 10:
                    key.SetValue("LOCALE_AUDIO", "ruRU");
                    break;
                case 11:
                    key.SetValue("LOCALE_AUDIO", "zhCN");
                    break;
                case 12:
                    key.SetValue("LOCALE_AUDIO", "zhTW");
                    break;
                default:
                    key.SetValue("LOCALE_AUDIO", "enUS");
                    break;
            }
            key.Close();
        }
    }
    [UsedImplicitly]
    public async void OnAppLanguageSelectionChanged()
    {
        Settings.Default.AppLanguage = (int)SelectedAppLanguage.Value;
        Settings.Default.Save();

        if (!string.IsNullOrEmpty(SelectedAppLanguage.Key))
        {
            CultureInfo culture = new CultureInfo(SelectedAppLanguage.Key.Split(' ')[1].Trim(new[] { '(', ')' }));
            CultureResources.ChangeCulture(culture);
        }

        await Translate();
    }

    #endregion

    #region ---Button/Checkbox Controls---

    private async Task ApplyHdrFix()
    {
        string profileHdJsonPath = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/_profilehd.json");

        // Ensure the directory exists
        string? directory = Path.GetDirectoryName(profileHdJsonPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        if (!File.Exists(profileHdJsonPath))
            await File.WriteAllBytesAsync(profileHdJsonPath, await Helper.GetResourceByteArray("CASC.profilehd.json"));


        try
        {
            string backgroundColorNormal = "\"backgroundColor\": [ 0, 0, 0, 0.75 ],";
            string backgroundColorFix = "\"backgroundColor\": [ 0, 0, 0, 0.95 ],";
            string inGameBackgroundColorNormal = "\"inGameBackgroundColor\": [ 0, 0, 0, 0.6 ],";
            string inGameBackgroundColorFix = "\"inGameBackgroundColor\": [ 0, 0, 0, 0.95 ],";
            string fileContent = File.ReadAllText(profileHdJsonPath);

            if (ShellViewModel.UserSettings.HdrFix)
            {
                fileContent = fileContent.Replace(backgroundColorNormal, backgroundColorFix);
                fileContent = fileContent.Replace(inGameBackgroundColorNormal, inGameBackgroundColorFix);
                await File.WriteAllTextAsync(profileHdJsonPath, fileContent);
            }
            else
            {
                fileContent = fileContent.Replace(backgroundColorFix, backgroundColorNormal);
                fileContent = fileContent.Replace(inGameBackgroundColorFix, inGameBackgroundColorNormal);
                await File.WriteAllTextAsync(profileHdJsonPath, fileContent);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            MessageBox.Show(ex.Message);
        }
    }
    private async Task ApplyCinematicSkip()
    {
        string videoPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd/global/video");

        if (!File.Exists(videoPath + "/act2/act02start.webm"))
        {
            if (ShellViewModel.UserSettings.skipCinematics)
            {
                if (!Directory.Exists(videoPath))
                    Directory.CreateDirectory(videoPath);
                if (!Directory.Exists(videoPath + "/act2"))
                    Directory.CreateDirectory(videoPath + "/act2");
                if (!Directory.Exists(videoPath + "/act3"))
                    Directory.CreateDirectory(videoPath + "/act3");
                if (!Directory.Exists(videoPath + "/act4"))
                    Directory.CreateDirectory(videoPath + "/act4");
                if (!Directory.Exists(videoPath + "/act5"))
                    Directory.CreateDirectory(videoPath + "/act5");

                File.Create(videoPath + "/act2/act02start.webm").Close();
                File.Create(videoPath + "/act3/act03start.webm").Close();
                File.Create(videoPath + "/act4/act04start.webm").Close();
                File.Create(videoPath + "/act4/act04end.webm").Close();
                File.Create(videoPath + "/act5/d2x_out.webm").Close();
            }
        }
    }
    private async Task ApplyUiTheme()
    {
        string layoutPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts");
        string layoutExpandedPath = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN/UI Theme/expanded/layouts");
        string layoutRemoddedPath = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN/UI Theme/remodded/layouts");
        string uiPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui");
        string uiExpandedPath = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLAN/UI Theme/Expanded/ui");

        if (Directory.Exists(layoutRemoddedPath) || Directory.Exists(layoutExpandedPath))
        {
            if (Settings.Default.SelectedMod != "MyCustomMod")
            {
                switch ((eUiThemes)ShellViewModel.UserSettings.UiTheme)
                {
                    case eUiThemes.Standard:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutExpandedPath, layoutPath);
                            }
                            if (Directory.Exists(uiPath + "controller"))
                            {
                                Directory.Delete(uiPath + "controller", true);
                                await Helper.CloneDirectory(uiExpandedPath + "controller", uiPath + "controller");
                            }
                            if (Directory.Exists(uiPath + "panel"))
                            {
                                Directory.Delete(uiPath + "panel", true);
                                await Helper.CloneDirectory(uiExpandedPath + "panel", uiExpandedPath + "panel");
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD1:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_B\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_R\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD2:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_B\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD3:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_B\"", "_Y\"", "_G\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_P\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD4:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_B\"", "_P\"", "_G\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_Y\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD5:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_B\"", "_P\"", "_Y\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_G\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD6:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_B\"", "_P\"", "_Y\"", "_G\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_D\"");
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }
    }

    static void ReplaceStringsInFile(string filePath, string[] searchStrings, string replacementString)
    {
        try
        {
            string content = File.ReadAllText(filePath, Encoding.UTF8);
            bool modified = false;

            foreach (string searchString in searchStrings)
            {
                if (content.Contains(searchString))
                {
                    content = content.Replace(searchString, replacementString);
                    modified = true;
                }
            }

            if (modified)
            {
                File.WriteAllText(filePath, content, Encoding.UTF8);
                Console.WriteLine($"Updated: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
        }
    }

    [UsedImplicitly]
    public async void OnMapsHelp()
    {
        MessageBox.Show("These options let you force specific map layouts so you can roll that 'perfect' map anytime you want. Details explained below:\n\nTower: The tower entrance is on the same screen as your waypoint.\n\nCatacombs: Levels 3 and 4 are less than 3 screens away\n\nAncient Tunnels: Entrance is 1 screen away from your waypoint\n\nLower Kurast: Very favorable super chest pattern near your waypoint\n\nDurance of Hate: Level 3 entrance is one teleport away from waypoint.\n\nHellforge: Forge is at closest spawn to your waypoint\n\nWorldstone Keep: Level 3 and 4 are right next to each other\n\nI'm a Cheater: Almost all entrances are absurdly close with a perfect LK pattern by the waypoint. You're basically just cheating now.\n\n\nNOTE: Lower Kurast and I'm a Cheater options are only available on Vanilla++.");
    }
    [UsedImplicitly]
    public async void OnMSIFixHelp()
    {
        MessageBox.Show("Use this feature if you meet these conditions:\n- Using either of the 'Advanced' Monster Stats Display Options\n- Using MSI Afterburner (Riva Tuner) for in-game overlays\n\nThis will restart the apps to avoid loading conflicts; requires:\n- D2RLAN must be ran as Administrator\n- Change MSI Settings to 'Start App Minimized' (for QoL purposes)");
    }
    [UsedImplicitly]
    public async Task OnCheckForUpdates()
    {
        if (string.IsNullOrEmpty(Settings.Default.SelectedMod))
        {
            MessageBox.Show("Please select a mod first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        CheckingForUpdates = true;
        ProgressBarIsIndeterminate = true;
        ProgressStatus = "Checking for updates...";


        string tempPath = Path.GetTempPath();
        string tempModInfoPath = System.IO.Path.Combine(tempPath, "modinfo.json");
        string version = string.Empty;

        if (!File.Exists(ShellViewModel.SelectedModVersionFilePath))
        {
            if (ShellViewModel.ModInfo == null)
            {
                MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckingForUpdates = false;
                return;
            }

            version = ShellViewModel.ModInfo.ModVersion;
        }
        else
        {
            version = await File.ReadAllTextAsync(ShellViewModel.SelectedModVersionFilePath);
        }

        // Seting up the http client used to download the data
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5);

        //Download remote modinfo.json
        try
        {
            // Create a file stream to store the downloaded data.
            // This really can be any type of writeable stream.
            await using FileStream file = new FileStream(tempModInfoPath, FileMode.Create, FileAccess.Write, FileShare.None);

            await Execute.OnUIThreadAsync(async () => { await client.DownloadAsync(ShellViewModel.ModInfo.ModConfigDownloadLink, file, null, CancellationToken.None); });

            file.Close();
            await file.DisposeAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        ModInfo tempModInfo = await Helper.ParseModInfo(tempModInfoPath);

        File.Delete(tempModInfoPath);

        if (tempModInfo != null && SelectedMod != "MyCustomMod")
        {
            if (version == tempModInfo.ModVersion)
            {
                MessageBox.Show("No updates available.", "Update", MessageBoxButton.OK);
                CheckingForUpdates = false;
                return;
            }

            MessageBox.Show($"{Helper.GetCultureString("Version1")} {version}\n {Helper.GetCultureString("Version2")} {tempModInfo.ModVersion}",
                            Resources.ResourceManager.GetString("VersionRdy"), MessageBoxButton.OK);

            //Backup
            if (MessageBox.Show($"{Helper.GetCultureString("ModUpdateRdy").Replace("\\n", Environment.NewLine)}", "Backup Option", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                ProgressStatus = Helper.GetCultureString("UpdateBackup");

                if (Directory.Exists(ShellViewModel.BaseSelectedModFolder))
                {
                    try
                    {
                        string backupPath = System.IO.Path.Combine(ShellViewModel.BaseModsFolder, $"{Settings.Default.SelectedMod}(Backup-{ShellViewModel.ModInfo.ModVersion.Replace(".", "-")})");
                        if (Directory.Exists(backupPath))
                            Directory.Delete(backupPath, true);

                        await Task.Run(async () => { await Helper.CloneDirectory(ShellViewModel.BaseSelectedModFolder, backupPath); });
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        MessageBox.Show(Helper.GetCultureString("UpdateBackupError"));
                    }

                }
                else
                {
                    MessageBox.Show(Helper.GetCultureString("UpdateBackupError"));
                }
            }

            //Download Update
            string tempUpdatePath = System.IO.Path.Combine(tempPath, "Update.zip");
            ProgressBarIsIndeterminate = false;
            ProgressStatus = Helper.GetCultureString("UpdateBegin");

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
                            ProgressStatus = Helper.GetCultureString("UpdateProgressGHSize");
                        }
                        else
                        {
                            DownloadProgress = Math.Round(args, MidpointRounding.AwayFromZero);
                            DownloadProgressString = $"{DownloadProgress}%";
                        }
                    });
                };

                if (File.Exists(tempUpdatePath))
                    File.Delete(tempUpdatePath);

                // Create a file stream to store the downloaded data.
                // This really can be any type of writeable stream.
                await using FileStream file = new FileStream(tempUpdatePath, FileMode.Create, FileAccess.Write, FileShare.None);

                //TODO: Add cancellation token
                await Execute.OnUIThreadAsync(async () => { await client.DownloadAsync(tempModInfo.ModDownloadLink, file, progress, CancellationToken.None); });

                file.Close();
                await file.DisposeAsync();

                ProgressBarIsIndeterminate = true;
                ProgressStatus = Helper.GetCultureString("UpdateProgress1");
                DownloadProgressString = string.Empty;

                string tempExtractedModFolderPath = System.IO.Path.Combine(tempPath, "UpdateDownload");

                if (Directory.Exists(tempExtractedModFolderPath))
                    Directory.Delete(tempExtractedModFolderPath, true);

                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(tempUpdatePath, tempExtractedModFolderPath);
                    return Task.CompletedTask;
                });

                string tempModDirPath = await Helper.FindFolderWithMpq(tempExtractedModFolderPath);
                string tempParentDir = Path.GetDirectoryName(tempModDirPath);
                string modInstallPath = System.IO.Path.Combine(ShellViewModel.BaseModsFolder, tempModInfo.Name);

                string[] userSettings = null;

                if (File.Exists(ShellViewModel.SelectedUserSettingsFilePath))
                    userSettings = await File.ReadAllLinesAsync(ShellViewModel.SelectedUserSettingsFilePath);
                else if (File.Exists(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
                    userSettings = await File.ReadAllLinesAsync(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", ""));

                //Delete current Mod folder if it exists
                if (Directory.Exists(modInstallPath))
                    Directory.Delete(modInstallPath, true);

                //Clone mod into base mods folder.
                await Task.Run(async () => { await Helper.CloneDirectory(tempParentDir, modInstallPath); });

                if (userSettings != null)
                {
                    File.Create(ShellViewModel.SelectedUserSettingsFilePath).Close();
                    await File.WriteAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath, string.Join("\n", userSettings));
                }

                string versionPath = System.IO.Path.Combine(modInstallPath, "version.txt");
                if (!File.Exists(versionPath))
                    File.Create(versionPath).Close();

                tempModInfoPath = System.IO.Path.Combine(tempModDirPath, "modinfo.json");

                ModInfo modInfo = await Helper.ParseModInfo(tempModInfoPath);

                if (modInfo != null)
                    await File.WriteAllTextAsync(versionPath, modInfo.ModVersion);
                else
                    MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                File.Delete(tempUpdatePath);
                Directory.Delete(tempExtractedModFolderPath, true);

                ProgressBarIsIndeterminate = false;
                DownloadProgress = 100;
                ProgressStatus = Helper.GetCultureString("UpdateProgressDone");
                CheckingForUpdates = false;

                await InitializeMods();

                MessageBox.Show(Helper.GetCultureString("UpdateProgressDone"), "Update", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckingForUpdates = false;
                return;
            }
        }
        else
        {
            MessageBox.Show("Mod Author hasn't added support for auto-updates yet!");
        }
        CheckingForUpdates = false;

    }
    [UsedImplicitly]
    public async void OnDownloadMod()
    {
        dynamic options = new ExpandoObject();
        options.ResizeMode = ResizeMode.NoResize;
        options.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        DownloadNewModViewModel vm = new DownloadNewModViewModel(ShellViewModel);

        if (await _windowManager.ShowDialogAsync(vm, null, options))
        {
            Settings.Default.SelectedMod = vm.SelectedMod.Key;
            Settings.Default.Save();

            await InitializeMods();
        }
    }
    [UsedImplicitly]
    public async void OnCASCSettings()
    {
        dynamic options = new ExpandoObject();
        options.ResizeMode = ResizeMode.NoResize;
        options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        CASCExtractorViewModel vm = new CASCExtractorViewModel(ShellViewModel);
        await _windowManager.ShowDialogAsync(vm, null, options);
    }
    [UsedImplicitly]
    public async void OnCreateMod()
    {
        string createModDesc = Helper.GetCultureString("CreateModDesc").Replace("\\n", Environment.NewLine);

        if (MessageBox.Show(createModDesc, Helper.GetCultureString("Create"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            if (!Directory.Exists(System.IO.Path.Combine(ShellViewModel.BaseModsFolder, "MyCustomMod/MyCustomMod.mpq/data/global")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(ShellViewModel.BaseModsFolder, "MyCustomMod/MyCustomMod.mpq/data/global"));
                await File.WriteAllBytesAsync(System.IO.Path.Combine(ShellViewModel.BaseModsFolder, "MyCustomMod/MyCustomMod.mpq/modinfo.json"), await Helper.GetResourceByteArray("modinfo_blank.json"));
                Settings.Default.SelectedMod = "MyCustomMod";
                Settings.Default.Save();
                CloneDirectory(System.IO.Path.Combine(GetSavePath(), @"Diablo II Resurrected"), System.IO.Path.Combine(GetSavePath(), @"Diablo II Resurrected\Mods\MyCustomMod"));
                await InitializeMods();
            }
            else
                MessageBox.Show("A custom mod has already been created!", "Error", MessageBoxButton.OK);
        }
    }
    [UsedImplicitly]
    public async void OnDirectTxtChecked()
    {
        string sourceDirectory = ShellViewModel.SelectedModDataFolder;

        if (!File.Exists(System.IO.Path.Combine(sourceDirectory, @"local\macui\d2logo.pcx")))
        {
            ShellViewModel.UserSettings.DirectTxt = false;

            if (MessageBox.Show("You must first extract all ~40GB of game data to use this mode\nWould you like to extract them now?", "Missing Files!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                dynamic options = new ExpandoObject();
                options.ResizeMode = ResizeMode.NoResize;
                options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                CASCExtractorViewModel vm = new CASCExtractorViewModel(ShellViewModel);

                if (await _windowManager.ShowDialogAsync(vm, null, options))
                {
                }
            }
            else
                ShellViewModel.UserSettings.DirectTxt = false;
        }

        GetD2RArgs();
    }
    [UsedImplicitly]
    public async void OnMapRegenChecked()
    {
        GetD2RArgs();
    }
    public void OnImageClick() //Chat Gem RNG
    {
        Random random = new Random();
        int randomNumber = random.Next(1, 11);
        ImageRNG = randomNumber;

        // Toggle the image on click
        if (ImageRNG <= 8)
        {
            if (ImageSource == "pack://application:,,,/Resources/Images/ChatGem1b.png")
            {
                ImageSource = "pack://application:,,,/Resources/Images/ChatGem1c.png";
                ImageHint = "Gem Activated";
            }
            else
            {
                ImageSource = "pack://application:,,,/Resources/Images/ChatGem1b.png";
                ImageHint = "Gem Deactivated";
            }
        }
        else if (ImageRNG == 9)
        {
            if (ImageSource == "pack://application:,,,/Resources/Images/ChatGem1b.png")
            {
                ImageSource = "pack://application:,,,/Resources/Images/ChatGem1c.png";
                ImageHint = "Moooooooo!";
            }
            else
            {
                ImageSource = "pack://application:,,,/Resources/Images/ChatGem1b.png";
                ImageHint = "Gem Deactivated";
            }
        }
        else if (ImageRNG == 10)
        {
            if (ImageSource == "pack://application:,,,/Resources/Images/ChatGem1b.png")
            {
                ImageSource = "pack://application:,,,/Resources/Images/ChatGem1c.png";
                ImageHint = "Perfect Gem Activated";
            }
            else
            {
                ImageSource = "pack://application:,,,/Resources/Images/ChatGem1b.png";
                ImageHint = "Gem Deactivated";
            }
        }

        GetD2RArgs();
    }
    static void CloneDirectory(string sourceDirectory, string targetDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
            return;

        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        string[] files = Directory.GetFiles(sourceDirectory);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string targetPath = System.IO.Path.Combine(targetDirectory, fileName);
            File.Copy(file, targetPath, true);
        }
    }

    #endregion

    #region ---Change Handlers---

    [UsedImplicitly]
    public async void OnModSelectionChanged()
    {
        Settings.Default.SelectedMod = SelectedMod;
        Settings.Default.Save();

        await InitializeMods();
        GetD2RArgs();
    }
    [UsedImplicitly]
    public async void OnUIThemeSelectionChanged()
    {
        if (ShellViewModel.ModInfo == null)
            return;

        await ApplyUiTheme();
    }
    [UsedImplicitly]
    public async void OnMapLayoutSelectionChanged()
    {
        if ((eMapLayouts)ShellViewModel.UserSettings.MapLayout != eMapLayouts.Default)
            MessageBox.Show("WARNING: These options are meant for a fun experience or two, but will feel like cheating. Use at your own risk.\nIf you would like to proceed, please read these instructions:\n\nStep 1: Start the game with your selected layout\nStep 2: Once loaded into the game with your character fully, EXIT the game.\nStep 3: After exiting the game, you should see your layout dropdown on launcher changed back to Default. This is normal; Start the game again.\n\nIf you do not exit the game after changing your map layout...you will be stuck with a small drop pool of deterministic outcomes.\nThis does not need to be done every game; only if you change map layouts the normal ways; such as changing difficulty.");

        GetD2RArgs();
    }

    #endregion

    #region ---Map Seed Functions---

    public string GetSavePath()
    {
        string savePath = null;

        // Get all SIDs
        string[] userSIDs = Registry.Users.GetSubKeyNames()
            .Where(name => Regex.IsMatch(name, @"S-1-5-21-\d+-\d+-\d+-\d+$"))
            .ToArray();

        // GUID for Saved Games folder
        string valueName = "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}";

        foreach (string SID in userSIDs)
        {
            // Find the location of the registry key under the current user's hive
            string keyPath = $"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";
            using (RegistryKey key = Registry.Users.OpenSubKey($"{SID}\\{keyPath}"))
            {
                if (key != null)
                {
                    object value = key.GetValue(valueName);
                    if (value != null)
                    {
                        savePath = value.ToString();
                        break;
                    }
                }
            }
        }

        // If not found under specific user SID, check under HKEY_CURRENT_USER
        if (savePath == null)
        {
            string currentUserKeyPath = $"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(currentUserKeyPath))
            {
                if (key != null)
                {
                    object value = key.GetValue(valueName);
                    if (value != null)
                    {
                        savePath = value.ToString();
                    }
                }
            }
        }

        return savePath;
    }
    [UsedImplicitly]
    public async void OnCharSelect()
    {
        OpenFileDialog ofd = new OpenFileDialog();
        {
            ofd.InitialDirectory = ShellViewModel.SaveFilesFilePath;
            ofd.Filter = "D2R Character Files (*.d2s)|*.d2s";
        };

        ofd.ShowDialog();

        if (ofd.FileName != "")
        {
            string seedID = ParseD2SSeed(ofd.FileName).ToString();
            ShellViewModel.UserSettings.MapSeed = seedID;
            ShellViewModel.UserSettings.MapSeedName = Path.GetFileNameWithoutExtension(ofd.FileName) + "'s Seed: ";
            ShellViewModel.UserSettings.MapSeedLoc = ofd.FileName;
        }
        else
        {
            ShellViewModel.UserSettings.MapSeed = "";
            ShellViewModel.UserSettings.MapSeedName = "An Evil Force's Seed: ";
            ShellViewModel.UserSettings.MapSeedLoc = "";
        }
    }
    public async void OnCharMapSeed()
    {
        try
        {
            // Read the character file and parse the saved map seed ID
            byte[] bytes = await File.ReadAllBytesAsync(ShellViewModel.UserSettings.MapSeedLoc);
            byte[] newSeedBytes = BitConverter.GetBytes(uint.Parse(ShellViewModel.UserSettings.MapSeed));

            //Apply the saved map seed ID and update the D2S checksum
            Array.Copy(newSeedBytes, 0, bytes, 171, newSeedBytes.Length);
            int checksum = FixChecksum(bytes);
            byte[] checksumBytes = BitConverter.GetBytes(checksum);
            Array.Copy(checksumBytes, 0, bytes, 12, checksumBytes.Length);

            // Write the modified content back to the file
            await File.WriteAllBytesAsync(ShellViewModel.UserSettings.MapSeedLoc, bytes);
            MessageBox.Show($"{ShellViewModel.UserSettings.MapSeedName.Replace(":", "")}Updated!");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }
    public static int ParseD2SSeed(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file at {filePath} was not found.");

        byte[] fileData = File.ReadAllBytes(filePath);

        // Read 4 bytes starting at byte 171 for seed ID
        int startIndex = 171;
        byte[] data = new byte[4];
        Array.Copy(fileData, startIndex, data, 0, 4);

        // Convert the 4 bytes to an integer and return the result
        int result = BitConverter.ToInt32(data, 0);
        return result;
    }
    private int FixChecksum(byte[] bytes)
    {
        //Update save file checksum data to match edited content
        new byte[4].CopyTo(bytes, 0xc);
        int checksum = 0;

        for (int i = 0; i < bytes.Length; i++)
        {
            checksum = bytes[i] + (checksum * 2) + (checksum < 0 ? 1 : 0);
        }

        return checksum;
    }

    #endregion
}