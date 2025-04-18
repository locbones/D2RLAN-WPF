using System.Windows.Controls;
using Caliburn.Micro;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using Syncfusion.UI.Xaml.NavigationDrawer;
using System.Windows;
using System;
using System.Threading.Tasks;
using D2RLAN.ViewModels.Drawers;
using D2RLAN.Views.Drawers;
using JetBrains.Annotations;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using D2RLAN.Properties;
using D2RLAN.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using Syncfusion.Licensing;
using D2RLAN.Culture;
using D2RLAN.Models;
using Newtonsoft.Json;
using System.Windows.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Net;
using D2RLAN.ViewModels.Dialogs;
using System.Dynamic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using MemoryEditor;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;


namespace D2RLAN.ViewModels;

public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
{
    #region ---Static/Public Members---

    private ILog _logger = LogManager.GetLogger(typeof(ShellViewModel));
    private UserControl _userControl;
    private IWindowManager _windowManager;
    private string _title = "D2RLAN";
    private string appVersion = "1.3.0";
    private string _gamePath;
    private bool _diabloInstallDetected;
    private bool _customizationsEnabled;
    private bool _wikiEnabled = true;
    private ModInfo _modInfo;
    private UserSettings _userSettings;
    private string _modLogo = "pack://application:,,,/Resources/Images/D2RLAN_Logo.png";
    private DispatcherTimer _autoBackupDispatcherTimer;
    private bool _skillIconPackEnabled = true;
    private bool _skillBuffIconsEnabled = true;
    private bool _showItemLevelsEnabled = true;
    private bool _superTelekinesisEnabled = true;
    private bool _itemIconDisplayEnabled;
    private bool _launcherHasUpdate;
    private string _launcherUpdateString = "D2RLAN Update Ready!";
    private const string TAB_BYTE_CODE = "55AA55AA0100000062000000000000004400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000004A4D0000";
    private bool _ColorDyesEnabled = true;
    private bool _ExpandedInventoryEnabled = true;
    private bool _ExpandedStashEnabled = true;
    private bool _ExpandedCubeEnabled = true;
    private bool _ExpandedMercEnabled = true;
    private readonly IConfigurationRoot _configuration;

    const int PROCESS_VM_READ = 0x0010;
    const int PROCESS_VM_WRITE = 0x0020;
    const int PROCESS_VM_OPERATION = 0x0008;
    const int PROCESS_QUERY_INFORMATION = 0x0400;
    const int PROCESS_CREATE_THREAD = 0x0002;
    const int MEM_COMMIT = 0x1000;
    const int PAGE_EXECUTE_READWRITE = 0x40;

    public static bool debugLogging = false;

    public string BaseModsFolder => System.IO.Path.Combine(GamePath, "Mods");
    public string BaseSelectedModFolder => System.IO.Path.Combine(BaseModsFolder, Settings.Default.SelectedMod);
    public string SelectedModVersionFilePath => System.IO.Path.Combine(BaseSelectedModFolder, "version.txt");
    public string SelectedModDataFolder => System.IO.Path.Combine(BaseSelectedModFolder, $"{Settings.Default.SelectedMod}.mpq", "data");
    public string SelectedModInfoFilePath => System.IO.Path.Combine(BaseSelectedModFolder, $"{Settings.Default.SelectedMod}.mpq", "modinfo.json");
    public string OldSelectedUserSettingsFilePath => System.IO.Path.Combine(BaseSelectedModFolder, $"{Settings.Default.SelectedMod}.mpq", "MyUserSettings.txt");
    public string SelectedUserSettingsFilePath => System.IO.Path.Combine(BaseSelectedModFolder, $"{Settings.Default.SelectedMod}.mpq", "MyUserSettings.json");
    public string BaseSaveFilesFilePath => System.IO.Path.Combine(GetSavePath(), @$"Diablo II Resurrected");
    public string SaveFilesFilePath => System.IO.Path.Combine(GetSavePath(), @$"Diablo II Resurrected\Mods\{Settings.Default.SelectedMod}");
    public string BackupFolder => System.IO.Path.Combine(SaveFilesFilePath, "Backups");
    public string StasherPath => $@"..\Stasher";

    #endregion

    #region ---Imports---
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, out int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string lpModuleName);
    #endregion

    #region ---Window/Loaded Handlers---

    public ShellViewModel() //Main Window
    {
        if (Execute.InDesignMode)
        {
            ModLogo = "pack://application:,,,/Resources/Images/D2RLAN_Logo.png";
            Title = "D2RLAN";
            DiabloInstallDetected = true;
            HomeDrawerViewModel vm = new HomeDrawerViewModel();
            UserControl = new HomeDrawerView() {DataContext = vm};
            Injector injector = new Injector(_gamePath);
        }
    }
    public ShellViewModel(IWindowManager windowManager, IConfigurationRoot configuration) //Main Window Logger
    {
        _windowManager = windowManager;
        _configuration = configuration;
        _logger.Error("Shell view model being created..");
    }
    public async Task ApplyModSettings()
    {
        await StartAutoBackup();
        await ConfigureBuffIcons();
        await ConfigureSkillIcons();
        await ConfigureMercIcons();
        await ConfigureItemILvls();
        await ConfigureRuneDisplay();
        await ConfigureHideHelmets();
        await ConfigureMonsterStatsDisplay();
        await ConfigureItemIcons();
        await ConfigureSuperTelekinesis();
        await ConfigureRunewordSorting();
        await ConfigureHudDesign();
        await ConfigureColorDyes();
        await ConfigureCinematicSubs();

        await ApplyTCPPatch();
    } //Apply User-Defined QoL Options
    [UsedImplicitly]
    public async void OnLoaded(object args) //Functions to perform after UI has been loaded
    {
        eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);
        CultureInfo culture = new CultureInfo(appLanguage.GetAttributeOfType<DisplayAttribute>().Name.Split(' ')[1].Trim(new[] { '(', ')' })/*.Insert(2, "-")*/);
        CultureResources.ChangeCulture(culture);

        GamePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\D2R\";
        Settings.Default.InstallPath = GamePath;
        DiabloInstallDetected = true;

        HomeDrawerViewModel vm = new HomeDrawerViewModel(this, _windowManager);
        await vm.Initialize();
        UserControl = new HomeDrawerView() { DataContext = vm };
        await SaveUserSettings();

        await Task.Run(CheckForLauncherUpdates);
    }
    [UsedImplicitly]
    public async void OnItemClicked(NavigationItemClickedEventArgs args) //Side Menu Controls
    {

        switch (((string)args.Item.Tag).ToUpperInvariant())
        {
            case "HOME":
                {
                    HomeDrawerViewModel vm = new HomeDrawerViewModel(this, _windowManager);
                    await vm.Initialize();
                    UserControl = new HomeDrawerView() { DataContext = vm };
                    break;
                }
            case "QOL OPTIONS":
                {
                    QoLOptionsDrawerViewModel vm = new QoLOptionsDrawerViewModel(this, _windowManager);
                    await vm.Initialize();
                    UserControl = new QoLOptionsDrawerView() { DataContext = vm };
                    break;
                }
            case "CUSTOMIZATIONS":
                {
                    CustomizationsDrawerViewModel vm = new CustomizationsDrawerViewModel(this);
                    UserControl = new CustomizationsDrawerView() { DataContext = vm };
                    break;
                }
            case "HOTKEYS":
                {
                    dynamic options = new ExpandoObject();
                    options.ResizeMode = ResizeMode.NoResize;
                    options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    HotkeysViewModel vm = new HotkeysViewModel(this);

                    if (await _windowManager.ShowDialogAsync(vm, null, options))
                    {
                    }
                    break;
                }
            case "CHAT":
                {
                    dynamic options = new ExpandoObject();
                    options.ResizeMode = ResizeMode.NoResize;
                    options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    ChatSettingsViewModel vm = new ChatSettingsViewModel(this);

                    if (await _windowManager.ShowDialogAsync(vm, null, options))
                    {
                    }
                    break;
                }
            case "RENAME CHARACTER":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    await RenameCharacter();
                    break;
                }
            case "COMMUNITY DISCORD":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    if (!string.IsNullOrEmpty(ModInfo.Discord))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(ModInfo.Discord);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    else
                        MessageBox.Show(Helper.GetCultureString("NoDiscord"));
                    break;
                }
            case "WIKI":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;
                    if (!string.IsNullOrEmpty(ModInfo.Wiki))
                    {

                        ProcessStartInfo psi = new ProcessStartInfo(ModInfo.Wiki);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    else
                        MessageBox.Show(Helper.GetCultureString("NoWiki"));
                    break;
                }
            case "COMMUNITY PATREON":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;
                    if (!string.IsNullOrEmpty(ModInfo.Patreon))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(ModInfo.Patreon);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    else
                        MessageBox.Show(Helper.GetCultureString("NoPatreon"));

                    break;
                }
            case "MOD FILES":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    if (Directory.Exists(SelectedModDataFolder))
                    {
                        string smdf = SelectedModDataFolder.Replace("/", @"\");

                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = smdf,
                            FileName = "explorer.exe",
                        };
                        Process.Start(startInfo);
                    }
                    else
                        MessageBox.Show($"{SelectedModDataFolder} Directory does not exist!");
                    break;
                }
            case "SAVE FILES":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    string modPath;

                    //Check for Default Save Path
                    if (ModInfo.SavePath.Contains("\"../\""))
                        modPath = BaseSaveFilesFilePath;
                    else
                        modPath = SaveFilesFilePath;

                    if (Directory.Exists(modPath))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = modPath,
                            FileName = "explorer.exe"
                        };
                        Process.Start(startInfo);
                    }
                    else
                        MessageBox.Show($"{modPath} Directory does not exist!");

                    break;
                }
            case "LAUNCH FILES":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    string folderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D2RLAN");
                    if (Directory.Exists(folderPath))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = folderPath,
                            FileName = "explorer.exe"
                        };
                        Process.Start(startInfo);
                    }
                    else
                        MessageBox.Show($"{folderPath} Directory does not exist!");

                    break;
                }
            case "ERROR LOGS":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    string folderPath = "Error Logs";
                    if (Directory.Exists(folderPath))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = folderPath,
                            FileName = "explorer.exe"
                        };
                        Process.Start(startInfo);
                    }
                    else
                        MessageBox.Show($"{folderPath} Directory does not exist!");

                    break;
                }
            case "D2RWEBSITE":
                {
                    ProcessStartInfo psi = new ProcessStartInfo("https://d2rmodding.com") { UseShellExecute = true };
                    Process.Start(psi);
                    break;
                }
            case "D2RDISCORD":
                {
                    ProcessStartInfo psi = new ProcessStartInfo("https://www.discord.gg/pqUWcDcjWF") { UseShellExecute = true };
                    Process.Start(psi);
                    break;
                }
            case "D2RYOUTUBE":
                {
                    ProcessStartInfo psi = new ProcessStartInfo("https://www.youtube.com/locbones1") { UseShellExecute = true };
                    Process.Start(psi);
                    break;
                }
            case "EVENTS":
                {
                    try
                    {
                        await CheckForEvents();
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                        MessageBox.Show(e.Message);
                    }
                    break;
                }
            case "BEACON":
                {
                    if (File.Exists("Beacon.exe"))
                        Process.Start("Beacon.exe");

                    break;
                }
            case "PATREON":
                {
                    ProcessStartInfo psi = new ProcessStartInfo("https://patreon.com/bonesyd2r") { UseShellExecute = true };
                    Process.Start(psi);
                    break;
                }

        }
    }

    #endregion

    #region ---Properties---

    public string LauncherUpdateString
    {
        get => _launcherUpdateString;
        set
        {
            if (value == _launcherUpdateString)
            {
                return;
            }
            _launcherUpdateString = value;
            NotifyOfPropertyChange();
        }
    }
    public bool LauncherHasUpdate
    {
        get => _launcherHasUpdate;
        set
        {
            if (value == _launcherHasUpdate)
            {
                return;
            }
            _launcherHasUpdate = value;
            NotifyOfPropertyChange();
        }
    }
    public string ModLogo
    {
        get => _modLogo;
        set
        {
            if (value == _modLogo) return;
            _modLogo = value;
            NotifyOfPropertyChange();
        }
    }
    public ModInfo ModInfo
    {
        get => _modInfo;
        set
        {
            if (Equals(value, _modInfo)) return;
            _modInfo = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ItemIconDisplayEnabled
    {
        get => _itemIconDisplayEnabled;
        set
        {
            if (value == _itemIconDisplayEnabled) return;
            _itemIconDisplayEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool SuperTelekinesisEnabled
    {
        get => _superTelekinesisEnabled;
        set
        {
            if (value == _superTelekinesisEnabled) return;
            _superTelekinesisEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ShowItemLevelsEnabled
    {
        get => _showItemLevelsEnabled;
        set
        {
            if (value == _showItemLevelsEnabled) return;
            _showItemLevelsEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool SkillIconPackEnabled
    {
        get => _skillIconPackEnabled;
        set
        {
            if (value == _skillIconPackEnabled) return;
            _skillIconPackEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool SkillBuffIconsEnabled
    {
        get => _skillBuffIconsEnabled;
        set
        {
            if (value == _skillBuffIconsEnabled) return;
            _skillBuffIconsEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool CustomizationsEnabled
    {
        get => _customizationsEnabled;
        set
        {
            if (value == _customizationsEnabled) return;
            _customizationsEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool WikiEnabled
    {
        get => _wikiEnabled;
        set
        {
            if (value == _wikiEnabled) return;
            _wikiEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool DiabloInstallDetected
    {
        get => _diabloInstallDetected;
        set
        {
            if (value == _diabloInstallDetected) return;
            _diabloInstallDetected = value;
            NotifyOfPropertyChange();
        }
    }
    public string GamePath
    {
        get => _gamePath;
        set
        {
            if (value == _gamePath) return;
            _gamePath = value;
            NotifyOfPropertyChange();
        }
    }
    public UserSettings UserSettings
    {
        get => _userSettings;
        set
        {
            if (Equals(value, _userSettings)) return;
            _userSettings = value;
            _userSettings.PropertyChanged += (sender, args) =>
                                             {
                                                 Task.Run(SaveUserSettings);
                                             };

            NotifyOfPropertyChange();
        }
    }
    public string Title
    {
        get => _title + " v" + appVersion;
        set
        {
            if (value == _title) return;
            _title = value;
            NotifyOfPropertyChange();
        }
    }
    public UserControl UserControl
    {
        get => _userControl;
        set
        {
            _userControl = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ExpandedInventoryEnabled
    {
        get => _ExpandedInventoryEnabled;
        set
        {
            if (value == _ExpandedInventoryEnabled) return;
            _ExpandedInventoryEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ExpandedStashEnabled
    {
        get => _ExpandedStashEnabled;
        set
        {
            if (value == _ExpandedStashEnabled) return;
            _ExpandedStashEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ExpandedCubeEnabled
    {
        get => _ExpandedCubeEnabled;
        set
        {
            if (value == _ExpandedCubeEnabled) return;
            _ExpandedCubeEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ExpandedMercEnabled
    {
        get => _ExpandedMercEnabled;
        set
        {
            if (value == _ExpandedMercEnabled) return;
            _ExpandedMercEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ColorDyesEnabled
    {
        get => _ColorDyesEnabled;
        set
        {
            if (value == _ColorDyesEnabled) return;
            _ColorDyesEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public IConfigurationRoot Configuration => _configuration;

    #endregion

    #region ---Mod Settings---

    private async Task ConfigureHudDesign() //Merged HUD
    {
        eHudDesign hudDesign = (eHudDesign)UserSettings.HudDesign;


        string mergedHudDirectory = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design");
        string layoutFolder = System.IO.Path.Combine(SelectedModDataFolder, "global/ui/layouts");
        string hudPanelhdJsonFilePath = System.IO.Path.Combine(layoutFolder, "hudpanelhd.json");
        string controllerhudPanelhdJsonFilePath = System.IO.Path.Combine(layoutFolder, "controller/hudpanelhd.json");
        string skillSelecthdJsonFilePath = System.IO.Path.Combine(layoutFolder, "skillselecthd.json");
        string controllerDirectory = System.IO.Path.Combine(layoutFolder, "controller");

        if (Directory.Exists(mergedHudDirectory))
        {

            if (!File.Exists(layoutFolder))
                Directory.CreateDirectory(layoutFolder);

            switch (hudDesign)
            {
                case eHudDesign.Standard:
                    {
                        if (File.Exists(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/standard/hudpanelhd.json")))
                        {
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/standard/hudpanelhd.json"), hudPanelhdJsonFilePath, true);
                            // File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/HUD Design/remodded/Controller/hudpanelhd-merged_controller.json"), controllerhudPanelhdJsonFilePath, true);
                        }

                        if (File.Exists(controllerhudPanelhdJsonFilePath))
                            File.Delete(controllerhudPanelhdJsonFilePath);

                        if (File.Exists(Path.Combine(layoutFolder, "hireablespanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "hireablespanelhd.json"));

                        if (File.Exists(Path.Combine(layoutFolder, "chatpanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "chatpanelhd.json"));

                        if (File.Exists(Path.Combine(layoutFolder, "hudmessagepanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "hudmessagepanelhd.json"));

                        if (File.Exists(Path.Combine(layoutFolder, "messagelogpanel_640x480hd.json")))
                            File.Delete(Path.Combine(layoutFolder, "messagelogpanel_640x480hd.json"));

                        // Update skillselecthd.json if it exists
                        if (File.Exists(skillSelecthdJsonFilePath))
                        {
                            string skillSelect = await File.ReadAllTextAsync(skillSelecthdJsonFilePath);
                            await File.WriteAllTextAsync(skillSelecthdJsonFilePath, skillSelect.Replace("\"centerMirrorGapWidth\": 846,", "\"centerMirrorGapWidth\": 146,"));
                        }

                        string[] searchStrings = null;
                        if (Directory.Exists(layoutFolder))
                        {
                            if (UserSettings.UiTheme == 2)
                            {
                                searchStrings = new string[] { "_B\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_R\"");
                                }
                            }

                            if (UserSettings.UiTheme == 3)
                            {
                                searchStrings = new string[] { "_R\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_B\"");
                                }
                            }

                            if (UserSettings.UiTheme == 4)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_P\"");
                                }
                            }

                            if (UserSettings.UiTheme == 5)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_Y\"");
                                }
                            }

                            if (UserSettings.UiTheme == 6)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_Y\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_G\"");
                                }
                            }

                            if (UserSettings.UiTheme == 7)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_Y\"", "_G\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_D\"");
                                }
                            }
                        }
                        break;
                    }
                case eHudDesign.ReMoDDeD:
                    {
                        if (!Directory.Exists(controllerDirectory))
                            Directory.CreateDirectory(controllerDirectory);

                        if (File.Exists(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/remodded/hudpanelhd.json")))
                        {
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/remodded/hudpanelhd.json"), hudPanelhdJsonFilePath, true);
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/remodded/hireablespanelhd.json"), Path.Combine(layoutFolder, "hireablespanelhd.json"), true);
                            // File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/HUD Design/remodded/Controller/hudpanelhd-merged_controller.json"), controllerhudPanelhdJsonFilePath, true);
                        }

                        if (File.Exists(Path.Combine(layoutFolder, "chatpanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "chatpanelhd.json"));

                        if (File.Exists(Path.Combine(layoutFolder, "hudmessagepanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "hudmessagepanelhd.json"));

                        if (File.Exists(Path.Combine(layoutFolder, "messagelogpanel_640x480hd.json")))
                            File.Delete(Path.Combine(layoutFolder, "messagelogpanel_640x480hd.json"));

                        /*
                        // Update skillselecthd.json if it exists
                        if (!File.Exists(skillSelecthdJsonFilePath))
                        {
                            File.Create(skillSelecthdJsonFilePath).Close();
                            await File.WriteAllBytesAsync(skillSelecthdJsonFilePath, await Helper.GetResourceByteArray("Options.MergedHUD.skillselecthd.json"));
                        }
                        string skillSelect = File.ReadAllText(skillSelecthdJsonFilePath);
                        await File.WriteAllTextAsync(skillSelecthdJsonFilePath, skillSelect.Replace("\"centerMirrorGapWidth\": 146,", "\"centerMirrorGapWidth\": 846,"));
                        */

                        string[] searchStrings = null;
                        if (Directory.Exists(layoutFolder))
                        {
                            if (UserSettings.UiTheme == 2)
                            {
                                searchStrings = new string[] { "_B\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_R\"");
                                }
                            }

                            if (UserSettings.UiTheme == 3)
                            {
                                searchStrings = new string[] { "_R\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_B\"");
                                }
                            }

                            if (UserSettings.UiTheme == 4)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_P\"");
                                }
                            }

                            if (UserSettings.UiTheme == 5)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_Y\"");
                                }
                            }

                            if (UserSettings.UiTheme == 6)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_Y\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_G\"");
                                }
                            }

                            if (UserSettings.UiTheme == 7)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_Y\"", "_G\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_D\"");
                                }
                            }
                        }
                        break;
                    }
                case eHudDesign.Merged:
                    {
                        if (!Directory.Exists(controllerDirectory))
                            Directory.CreateDirectory(controllerDirectory);

                        if (File.Exists(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/merged/hudpanelhd-merged.json")))
                        {
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/merged/hudpanelhd-merged.json"), hudPanelhdJsonFilePath, true);
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/merged/Controller/hudpanelhd-merged_controller.json"), controllerhudPanelhdJsonFilePath, true);
                        }
                        if (File.Exists(Path.Combine(layoutFolder, "hireablespanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "hireablespanelhd.json"));

                        if (File.Exists(Path.Combine(layoutFolder, "chatpanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "chatpanelhd.json"));

                        if (File.Exists(Path.Combine(layoutFolder, "hudmessagepanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "hudmessagepanelhd.json"));

                        if (File.Exists(Path.Combine(layoutFolder, "messagelogpanel_640x480hd.json")))
                            File.Delete(Path.Combine(layoutFolder, "messagelogpanel_640x480hd.json"));

                        // Update skillselecthd.json if it exists
                        if (!File.Exists(skillSelecthdJsonFilePath))
                        {
                            File.Create(skillSelecthdJsonFilePath).Close();
                            await File.WriteAllBytesAsync(skillSelecthdJsonFilePath, await Helper.GetResourceByteArray("Options.MergedHUD.skillselecthd.json"));
                        }
                        string skillSelect = File.ReadAllText(skillSelecthdJsonFilePath);
                        await File.WriteAllTextAsync(skillSelecthdJsonFilePath, skillSelect.Replace("\"centerMirrorGapWidth\": 146,", "\"centerMirrorGapWidth\": 846,"));

                        string[] searchStrings = null;
                        if (Directory.Exists(layoutFolder))
                        {
                            if (UserSettings.UiTheme == 2)
                            {
                                searchStrings = new string[] { "_B\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_R\"");
                                }
                            }

                            if (UserSettings.UiTheme == 3)
                            {
                                searchStrings = new string[] { "_R\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_B\"");
                                }
                            }

                            if (UserSettings.UiTheme == 4)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_P\"");
                                }
                            }

                            if (UserSettings.UiTheme == 5)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_Y\"");
                                }
                            }

                            if (UserSettings.UiTheme == 6)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_Y\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_G\"");
                                }
                            }

                            if (UserSettings.UiTheme == 7)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_Y\"", "_G\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_D\"");
                                }
                            }
                        }
                        break;
                    }
                case eHudDesign.MergedMini:
                    {
                        if (!Directory.Exists(controllerDirectory))
                            Directory.CreateDirectory(controllerDirectory);

                        if (File.Exists(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/merged_v2/hudpanelhd.json")))
                        {
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/merged_v2/hudpanelhd.json"), hudPanelhdJsonFilePath, true);
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/merged_v2/hudmessagepanelhd.json"), Path.Combine(layoutFolder, "hudmessagepanelhd.json"), true);
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/merged_v2/chatpanelhd.json"), Path.Combine(layoutFolder, "chatpanelhd.json"), true);
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLAN/HUD Design/merged_v2/messagelogpanel_640x480hd.json"), Path.Combine(layoutFolder, "messagelogpanel_640x480hd.json"), true);
                        }
                        if (File.Exists(Path.Combine(layoutFolder, "hireablespanelhd.json")))
                            File.Delete(Path.Combine(layoutFolder, "hireablespanelhd.json"));

                        /*
                        // Update skillselecthd.json if it exists
                        if (!File.Exists(skillSelecthdJsonFilePath))
                        {
                            File.Create(skillSelecthdJsonFilePath).Close();
                            await File.WriteAllBytesAsync(skillSelecthdJsonFilePath, await Helper.GetResourceByteArray("Options.MergedHUD.skillselecthd.json"));
                        }
                        string skillSelect = File.ReadAllText(skillSelecthdJsonFilePath);
                        await File.WriteAllTextAsync(skillSelecthdJsonFilePath, skillSelect.Replace("\"centerMirrorGapWidth\": 146,", "\"centerMirrorGapWidth\": 846,"));
                        */

                        string[] searchStrings = null;
                        if (Directory.Exists(layoutFolder))
                        {
                            if (UserSettings.UiTheme == 2)
                            {
                                searchStrings = new string[] { "_B\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_R\"");
                                }
                            }

                            if (UserSettings.UiTheme == 3)
                            {
                                searchStrings = new string[] { "_R\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_B\"");
                                }
                            }

                            if (UserSettings.UiTheme == 4)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_Y\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_P\"");
                                }
                            }

                            if (UserSettings.UiTheme == 5)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_G\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_Y\"");
                                }
                            }

                            if (UserSettings.UiTheme == 6)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_Y\"", "_D\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
                                {
                                    ReplaceStringsInFile(file, searchStrings, "_G\"");
                                }
                            }

                            if (UserSettings.UiTheme == 7)
                            {
                                searchStrings = new string[] { "_R\"", "_B\"", "_P\"", "_Y\"", "_G\"" };

                                foreach (string file in Directory.GetFiles(layoutFolder, "*.json*", SearchOption.AllDirectories))
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
    private async Task ConfigureRunewordSorting() //Runeword Sorting
    {
        eRunewordSorting runewordSorting = (eRunewordSorting)UserSettings.RunewordSorting;

        string abRunewordJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Runeword Sort/runewords-ab.json");
        string itRunewordJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Runeword Sort/runewords-it.json");
        string lvRunewordJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Runeword Sort/runewords-lv.json");

        string abHelpPandelHdJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Runeword Sort/helppanelhd-ab.json");
        string itHelpPandelHdJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Runeword Sort/helppanelhd-it.json");
        string lvHelpPandelHdJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Runeword Sort/helppanelhd-lv.json");

        string helpPandelHdJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "global/ui/layouts/helppanelhd.json");

        if (ModInfo.Name != "RMD-MP" && ModInfo.Name != "VNP-MP")
            return;

        switch (runewordSorting)
        {
            case eRunewordSorting.ByName:
                {
                    if (ModInfo.Name == "RMD-MP")
                    {
                        File.Copy(abHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                        File.Copy(abRunewordJsonFilePath, System.IO.Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                    }
                    else
                    {
                        if (File.Exists(abRunewordJsonFilePath))
                            File.Copy(abRunewordJsonFilePath, System.IO.Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);
                    }

                    break;
                }
            case eRunewordSorting.ByItemtype:
                {
                    if (ModInfo.Name == "RMD-MP")
                    {
                        File.Copy(itHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                        File.Copy(itRunewordJsonFilePath, System.IO.Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                    }
                    else
                        if (File.Exists(itRunewordJsonFilePath))
                            File.Copy(itRunewordJsonFilePath, System.IO.Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);

                    break;
                }
            case eRunewordSorting.ByReqLevel:
                {
                    if (ModInfo.Name == "RMD-MP")
                    {
                        File.Copy(lvHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                        File.Copy(lvRunewordJsonFilePath, System.IO.Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                    }
                    else
                        if (File.Exists(lvRunewordJsonFilePath))
                            File.Copy(lvRunewordJsonFilePath, System.IO.Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);

                    break;
                }
        }
    } 
    private async Task ConfigureMonsterStatsDisplay() //Advanced Monster Stats
    {
        eMonsterHP monsterHP = (eMonsterHP)UserSettings.MonsterHP;

        string uiLayoutsPath = Path.Combine(SelectedModDataFolder, "global/ui/layouts");
        string hudMonsterHealthHdJsonFilePath = Path.Combine(uiLayoutsPath, "hudmonsterhealthhd.json");
        string hudMonsterHealthHdDisabledJsonFilePath = Path.Combine(uiLayoutsPath, "hudmonsterhealthhd_disabled.json");
        string monsterStatsPath = Path.Combine(SelectedModDataFolder, "D2RLAN/Monster Stats");
        string outputPath = SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip";

        if (!Directory.Exists(uiLayoutsPath))
            Directory.CreateDirectory(uiLayoutsPath);

        if (!Directory.Exists(monsterStatsPath))
            Directory.CreateDirectory(monsterStatsPath);

        if (!File.Exists(hudMonsterHealthHdJsonFilePath) && !File.Exists(hudMonsterHealthHdDisabledJsonFilePath))
            await File.WriteAllBytesAsync(hudMonsterHealthHdJsonFilePath, await Helper.GetResourceByteArray("Options.MonsterStats.hudmonsterhealthhd.json"));
        if (!File.Exists(hudMonsterHealthHdJsonFilePath) && File.Exists(hudMonsterHealthHdDisabledJsonFilePath))
            File.Move(hudMonsterHealthHdDisabledJsonFilePath, hudMonsterHealthHdJsonFilePath, true);

        if (File.Exists(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip"))
            File.Delete(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip");
        else
            await File.WriteAllBytesAsync(outputPath, await Helper.GetResourceByteArray("Options.MonsterStats.MS_Assets.zip"));

        switch (monsterHP)
        {
            case eMonsterHP.Retail:
                {
                    if (File.Exists(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd_disabled.json", true);

                    break;
                }
            case eMonsterHP.BasicNoP:
                {
                    ZipFile.ExtractToDirectory(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip", SelectedModDataFolder + "/D2RLAN/Monster Stats/", true);

                    if (File.Exists(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip"))
                        File.Delete(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip");

                    if (!File.Exists(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    string hudContents = File.ReadAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json");

                    if (hudContents.Contains("HB_A\""))
                        File.Move(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    if (hudContents.Contains("MonHPBar_UniFull\"") || hudContents.Contains("MonHPBar_UniFullPer\""))
                    {
                        hudContents = hudContents.Replace("MonHPBar_UniFull\"", "MonHPBar_UniSmall\"").Replace("MonHPBar_NormFull\"", "MonHPBar_NormSmall\"").Replace("MonHPBar_UniFullPer\"", "MonHPBar_UniSmall\"").Replace("MonHPBar_NormFullPer\"", "MonHPBar_NormSmall\"")
                            .Replace("MonHPBar_UniSmallPer\"", "MonHPBar_UniSmall\"").Replace("MonHPBar_NormSmallPer\"", "MonHPBar_NormSmall\"").Replace("\"y\": 115", "\"y\": 65").Replace("\"y\": 150", "\"y\": 100");
                        File.WriteAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", hudContents);
                    }

                    break;
                }
            case eMonsterHP.BasicP:
                {
                    ZipFile.ExtractToDirectory(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip", SelectedModDataFolder + "/D2RLAN/Monster Stats/", true);

                    if (File.Exists(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip"))
                        File.Delete(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip");

                    if (!File.Exists(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    string hudContents = File.ReadAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json");

                    if (hudContents.Contains("HB_A\""))
                        File.Move(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    hudContents = hudContents.Replace("MonHPBar_UniFull\"", "MonHPBar_UniSmallPer\"").Replace("MonHPBar_NormFull\"", "MonHPBar_NormSmallPer\"").Replace("MonHPBar_UniFullPer\"", "MonHPBar_UniSmallPer\"").Replace("MonHPBar_NormFullPer\"", "MonHPBar_NormSmallPer\"")
                            .Replace("MonHPBar_UniSmall\"", "MonHPBar_UniSmallPer\"").Replace("MonHPBar_NormSmall\"", "MonHPBar_NormSmallPer\"").Replace("\"y\": 115", "\"y\": 65").Replace("\"y\": 150", "\"y\": 100");
                    File.WriteAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", hudContents);

                    break;
                }
            case eMonsterHP.AdvancedNoP:
                {
                    ZipFile.ExtractToDirectory(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip", SelectedModDataFolder + "/D2RLAN/Monster Stats/", true);

                    if (File.Exists(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip"))
                        File.Delete(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip");

                    if (!File.Exists(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    string hudContents = File.ReadAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json");

                    if (hudContents.Contains("HB_A\""))
                        File.Move(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    hudContents = hudContents.Replace("MonHPBar_UniFullPer\"", "MonHPBar_UniFull\"").Replace("MonHPBar_NormFullPer\"", "MonHPBar_NormFull\"").Replace("MonHPBar_UniSmallPer\"", "MonHPBar_UniFull\"").Replace("MonHPBar_NormSmallPer\"", "MonHPBar_NormFull\"")
                            .Replace("MonHPBar_UniSmall\"", "MonHPBar_UniFull\"").Replace("MonHPBar_NormSmall\"", "MonHPBar_NormFull\"").Replace("\"y\": 65", "\"y\": 115").Replace("\"y\": 100", "\"y\": 150");
                    File.WriteAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", hudContents);

                    break;
                }
            case eMonsterHP.AdvancedP:
                {
                    ZipFile.ExtractToDirectory(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip", SelectedModDataFolder + "/D2RLAN/Monster Stats/", true);

                    if (File.Exists(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip"))
                        File.Delete(SelectedModDataFolder + "/D2RLAN/Monster Stats/MS_Assets.zip");

                    if (!File.Exists(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    string hudContents = File.ReadAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json");

                    if (hudContents.Contains("HB_A\""))
                        File.Move(SelectedModDataFolder + "/D2RLAN/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    hudContents = hudContents.Replace("MonHPBar_UniFull\"", "MonHPBar_UniFullPer\"").Replace("MonHPBar_NormFull\"", "MonHPBar_NormFullPer\"").Replace("MonHPBar_UniSmallPer\"", "MonHPBar_UniFullPer\"").Replace("MonHPBar_NormSmallPer\"", "MonHPBar_NormFullPer\"")
                            .Replace("MonHPBar_UniSmall\"", "MonHPBar_UniFullPer\"").Replace("MonHPBar_NormSmall\"", "MonHPBar_NormFullPer\"").Replace("\"y\": 65", "\"y\": 115").Replace("\"y\": 100", "\"y\": 150");
                    File.WriteAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", hudContents);

                    break;
                }
        }
    }
    private async Task ConfigureHideHelmets() //Hide Helmets
    {
        eEnabledDisabled helmetDisplay = (eEnabledDisabled)UserSettings.HideHelmets;

        //Define filenames and paths
        string helmetBaseDir1 = System.IO.Path.Combine(SelectedModDataFolder, "hd/items/armor/helmet");
        string helmetBaseDir2 = System.IO.Path.Combine(SelectedModDataFolder, "hd/items/armor/circlet");
        string helmetBaseDir3 = System.IO.Path.Combine(SelectedModDataFolder, "hd/items/armor/pelt");
        string[] helmetFiles1 = new[] { "assault_helmet", "avenger_guard", "bone_helm", "cap_hat", "coif_of_glory", "crown", "crown_of_thieves", "duskdeep", "fanged_helm", "full_helm", "great_helm", "helm", "horned_helm", "jawbone_cap", "mask", "ondals_almighty", "rockstopper", "skull_cap", "war_bonnet", "wormskull" };
        string[] helmetFiles2 = new[] { "circlet", "coronet", "diadem", "tiara" };
        string[] helmetFiles3 = new[] { "antlers", "falcon_mask", "hawk_helm", "spirit_mask", "wolf_head" };

        //Add paths and extension to array
        string[] helmetFilesWithExtension1 = helmetFiles1.Select(x => x + ".json").ToArray();
        string[] helmetFilesWithExtension2 = helmetFiles2.Select(x => x + ".json").ToArray();
        string[] helmetFilesWithExtension3 = helmetFiles3.Select(x => x + ".json").ToArray();
        string[] allHelmetFiles1 = helmetFilesWithExtension1.Select(x => System.IO.Path.Combine(helmetBaseDir1, x)).Concat(helmetFilesWithExtension1.Select(x => System.IO.Path.Combine(helmetBaseDir1, x))).ToArray();
        string[] allHelmetFiles2 = helmetFilesWithExtension2.Select(x => System.IO.Path.Combine(helmetBaseDir2, x)).Concat(helmetFilesWithExtension2.Select(x => System.IO.Path.Combine(helmetBaseDir2, x))).ToArray();
        string[] allHelmetFiles3 = helmetFilesWithExtension3.Select(x => System.IO.Path.Combine(helmetBaseDir3, x)).Concat(helmetFilesWithExtension3.Select(x => System.IO.Path.Combine(helmetBaseDir3, x))).ToArray();

        switch (helmetDisplay)
        {
            case eEnabledDisabled.Disabled:
                {
                    foreach (string filename in allHelmetFiles1)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                    }

                    foreach (string filename in allHelmetFiles2)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                    }

                    foreach (string filename in allHelmetFiles3)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                    }
                    break;
                }
            case eEnabledDisabled.Enabled:
                {
                    //Create directories if they don't exist
                    if (!Directory.Exists(helmetBaseDir1))
                        Directory.CreateDirectory(helmetBaseDir1);
                    if (!Directory.Exists(helmetBaseDir2))
                        Directory.CreateDirectory(helmetBaseDir2);
                    if (!Directory.Exists(helmetBaseDir3))
                        Directory.CreateDirectory(helmetBaseDir3);

                    //Loop through both arrays to create files
                    foreach (string filename in allHelmetFiles1)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                        File.Create(filename).Close();
                        await File.WriteAllBytesAsync(filename, await Helper.GetResourceByteArray("Options.HideHelmets.hide_helmets.json"));
                    }

                    foreach (string filename in allHelmetFiles2)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                        File.Create(filename).Close();
                        await File.WriteAllBytesAsync(filename, await Helper.GetResourceByteArray("Options.HideHelmets.hide_helmets.json"));
                    }

                    foreach (string filename in allHelmetFiles3)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                        File.Create(filename).Close();
                        await File.WriteAllBytesAsync(filename, await Helper.GetResourceByteArray("Options.HideHelmets.hide_helmets.json"));
                    }
                    break;
                }
        }
    }
    private async Task ConfigureRuneDisplay() //Rune Display (Special Rune Visuals)
    {
        eEnabledDisabled runeDisplay = (eEnabledDisabled)UserSettings.RuneDisplay;

        //Define replacement strings
        string runePath = System.IO.Path.Combine(SelectedModDataFolder, "hd/items/misc/rune");
        string runeStringPath = System.IO.Path.Combine(SelectedModDataFolder, "local/lng/strings");
        string runeStringJsonFile = System.IO.Path.Combine(SelectedModDataFolder, "local/lng/strings/item-runes.json");
        string noOverlay = "\"terrainBlendMode\": 1\r\n                }\r\n            ]";
        string overlay1 = "\"terrainBlendMode\": 1\r\n                },\r\n                {\r\n                    \"type\": \"VfxDefinitionComponent\",\r\n                    \"name\": \"item_icon_2\",\r\n                    \"filename\": \"data/hd/vfx/particles/overlays/objects/multigleam/fx_multigleam.particles\",\r\n                    \"hardKillOnDestroy\": false\r\n                },\r\n                {\r\n                    \"type\": \"TransformDefinitionComponent\",\r\n                    \"name\": \"entity_root_TransformDefinition\",\r\n                    \"inheritOnlyPosition\": true\r\n                }\r\n            ]";
        string overlay2 = "\"terrainBlendMode\": 1\r\n                },\r\n                {\r\n                    \"type\": \"VfxDefinitionComponent\",\r\n                    \"name\": \"item_icon_2\",\r\n                    \"filename\": \"data/hd/vfx/particles/overlays/common/impregnated/vfx_impregnated.particles\",\r\n                    \"hardKillOnDestroy\": false\r\n                },\r\n                {\r\n                    \"type\": \"TransformDefinitionComponent\",\r\n                    \"name\": \"entity_root_TransformDefinition\",\r\n                    \"inheritOnlyPosition\": true\r\n                }\r\n            ]";
        string[] fileNames = null;

        string cascItemRuneJsonFileName = @"data:data\local\lng\strings\item-runes.json";

        switch (runeDisplay)
        {
            case eEnabledDisabled.Disabled:
                {
                    if (!Directory.Exists(runePath))
                        Directory.CreateDirectory(runePath);

                    fileNames = Directory.GetFiles(runePath, "*.json");

                    foreach (string fileName in fileNames)
                    {
                        string fileContent = await File.ReadAllTextAsync(fileName);
                        fileContent = fileContent.Replace(overlay1, noOverlay);
                        fileContent = fileContent.Replace(overlay2, noOverlay);
                        await File.WriteAllTextAsync(fileName, fileContent);
                    }

                    //Replace rune string contents to display names
                    if (!Directory.Exists(runeStringPath))
                        Directory.CreateDirectory(runeStringPath);
                    if (!File.Exists(runeStringJsonFile))
                        await File.WriteAllBytesAsync(runeStringJsonFile, await Helper.GetResourceByteArray("CASC.item-runes.json"));

                    if (File.Exists(runeStringJsonFile))
                    {
                        string runeStrings = await File.ReadAllTextAsync(runeStringJsonFile);
                        runeStrings = runeStrings.Replace("\"⅐ Elÿc0\"", "\"El Rune\"");
                        runeStrings = runeStrings.Replace("\"⅑ Eldÿc0\"", "\"Eld Rune\"");
                        runeStrings = runeStrings.Replace("\"⅒ Tirÿc0\"", "\"Tir Rune\"");
                        runeStrings = runeStrings.Replace("\"⅓ Nefÿc0\"", "\"Nef Rune\"");
                        runeStrings = runeStrings.Replace("\"⅔ Ethÿc0\"", "\"Eth Rune\"");
                        runeStrings = runeStrings.Replace("\"⅕ Ithÿc0\"", "\"Ith Rune\"");
                        runeStrings = runeStrings.Replace("\"⅖ Talÿc0\"", "\"Tal Rune\"");
                        runeStrings = runeStrings.Replace("\"⅗ Ralÿc0\"", "\"Ral Rune\"");
                        runeStrings = runeStrings.Replace("\"⅘ Ortÿc0\"", "\"Ort Rune\"");
                        runeStrings = runeStrings.Replace("\"⅙ Thulÿc0\"", "\"Thul Rune\"");
                        runeStrings = runeStrings.Replace("\"⅚ Amnÿc0\"", "\"Amn Rune\"");
                        runeStrings = runeStrings.Replace("\"⅛ Solÿc0\"", "\"Sol Rune\"");
                        runeStrings = runeStrings.Replace("\"⅜ Shaelÿc0\"", "\"Shael Rune\"");
                        runeStrings = runeStrings.Replace("\"⅝ Dolÿc0\"", "\"Dol Rune\"");
                        runeStrings = runeStrings.Replace("\"⅞ Helÿc0\"", "\"Hel Rune\"");
                        runeStrings = runeStrings.Replace("\"⅟ Ioÿc0\"", "\"Io Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅰ Lumÿc0\"", "\"Lum Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅱ Koÿc0\"", "\"Ko Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅲ Falÿc0\"", "\"Fal Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅳ Lemÿc0\"", "\"Lem Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅴ Pulÿc0\"", "\"Pul Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅵ Umÿc0\"", "\"Um Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅶ Malÿc0\"", "\"Mal Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅷ Istÿc0\"", "\"Ist Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅸ Gulÿc0\"", "\"Gul Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅹ Vexÿc0\"", "\"Vex Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅺ Ohmÿc0\"", "\"Ohm Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅻ Loÿc0\"", "\"Lo Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅼ Surÿc0\"", "\"Sur Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅽ Berÿc0\"", "\"Ber Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅾ Jahÿc0\"", "\"Jah Rune\"");
                        runeStrings = runeStrings.Replace("\"Ⅿ Chamÿc0\"", "\"Cham Rune\"");
                        runeStrings = runeStrings.Replace("\"ⅰ Zodÿc0\"", "\"Zod Rune\"");
                        await File.WriteAllTextAsync(runeStringJsonFile, runeStrings);
                    }

                    break;
                }
            case eEnabledDisabled.Enabled:
                {
                    string[] runeFiles1 = { "sol_rune.json", "shael_rune.json", "dol_rune.json", "hel_rune.json", "io_rune.json", "lum_rune.json", "ko_rune.json", "fal_rune.json", "lem_rune.json", "pul_rune.json", "um_rune.json" };
                    string[] runeFiles2 = { "mal_rune.json", "ist_rune.json", "gul_rune.json", "vex_rune.json", "ohm_rune.json", "lo_rune.json", "sur_rune.json", "ber_rune.json", "jah_rune.json", "cham_rune.json", "zod_rune.json" };

                    if (!Directory.Exists(runePath))
                        Directory.CreateDirectory(runePath);

                    //Assign overlay1 to mid runes
                    foreach (string fileName in runeFiles1)
                    {

                        string filePath = System.IO.Path.Combine(runePath, fileName);
                        if (!File.Exists(filePath))
                        {
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "sol_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.sol_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "shael_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.shael_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "dol_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.dol_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "hel_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.hel_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "io_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.io_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "lum_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.lum_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "ko_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.ko_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "fal_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.fal_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "lem_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.lem_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "pul_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.pul_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "um_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.um_rune.json"));
                        }

                        if (File.Exists(filePath))
                        {
                            string fileContent = await File.ReadAllTextAsync(filePath);
                            fileContent = fileContent.Replace(noOverlay, overlay1);
                            await File.WriteAllTextAsync(filePath, fileContent);
                        }
                    }

                    //Assign overlay2 to high runes
                    foreach (string fileName in runeFiles2)
                    {
                        string filePath = System.IO.Path.Combine(runePath, fileName);
                        if (!File.Exists(filePath))
                        {
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "mal_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.mal_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "ist_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.ist_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "gul_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.gul_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "vex_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.vex_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "ohm_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.ohm_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "lo_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.lo_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "sur_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.sur_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "ber_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.ber_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "jah_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.jah_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "cham_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.cham_rune.json"));
                            Helper.CreateFileIfNotExists(System.IO.Path.Combine(runePath, "zod_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.zod_rune.json"));
                        }

                        if (File.Exists(filePath))
                        {
                            string fileContent = await File.ReadAllTextAsync(filePath);
                            fileContent = fileContent.Replace(noOverlay, overlay2);
                            await File.WriteAllTextAsync(filePath, fileContent);
                        }
                    }

                    //Replace rune string contents to display icons
                    if (!File.Exists(runeStringJsonFile))
                        await File.WriteAllBytesAsync(runeStringJsonFile, await Helper.GetResourceByteArray("CASC.item-runes.json"));

                    string runeStrings = await File.ReadAllTextAsync(runeStringJsonFile);
                    runeStrings = runeStrings.Replace("\"El Rune\"", "\"⅐ Elÿc0\"");
                    runeStrings = runeStrings.Replace("\"Eld Rune\"", "\"⅑ Eldÿc0\"");
                    runeStrings = runeStrings.Replace("\"Tir Rune\"", "\"⅒ Tirÿc0\"");
                    runeStrings = runeStrings.Replace("\"Nef Rune\"", "\"⅓ Nefÿc0\"");
                    runeStrings = runeStrings.Replace("\"Eth Rune\"", "\"⅔ Ethÿc0\"");
                    runeStrings = runeStrings.Replace("\"Ith Rune\"", "\"⅕ Ithÿc0\"");
                    runeStrings = runeStrings.Replace("\"Tal Rune\"", "\"⅖ Talÿc0\"");
                    runeStrings = runeStrings.Replace("\"Ral Rune\"", "\"⅗ Ralÿc0\"");
                    runeStrings = runeStrings.Replace("\"Ort Rune\"", "\"⅘ Ortÿc0\"");
                    runeStrings = runeStrings.Replace("\"Thul Rune\"", "\"⅙ Thulÿc0\"");
                    runeStrings = runeStrings.Replace("\"Amn Rune\"", "\"⅚ Amnÿc0\"");
                    runeStrings = runeStrings.Replace("\"Sol Rune\"", "\"⅛ Solÿc0\"");
                    runeStrings = runeStrings.Replace("\"Shael Rune\"", "\"⅜ Shaelÿc0\"");
                    runeStrings = runeStrings.Replace("\"Dol Rune\"", "\"⅝ Dolÿc0\"");
                    runeStrings = runeStrings.Replace("\"Hel Rune\"", "\"⅞ Helÿc0\"");
                    runeStrings = runeStrings.Replace("\"Io Rune\"", "\"⅟ Ioÿc0\"");
                    runeStrings = runeStrings.Replace("\"Lum Rune\"", "\"Ⅰ Lumÿc0\"");
                    runeStrings = runeStrings.Replace("\"Ko Rune\"", "\"Ⅱ Koÿc0\"");
                    runeStrings = runeStrings.Replace("\"Fal Rune\"", "\"Ⅲ Falÿc0\"");
                    runeStrings = runeStrings.Replace("\"Lem Rune\"", "\"Ⅳ Lemÿc0\"");
                    runeStrings = runeStrings.Replace("\"Pul Rune\"", "\"Ⅴ Pulÿc0\"");
                    runeStrings = runeStrings.Replace("\"Um Rune\"", "\"Ⅵ Umÿc0\"");
                    runeStrings = runeStrings.Replace("\"Mal Rune\"", "\"Ⅶ Malÿc0\"");
                    runeStrings = runeStrings.Replace("\"Ist Rune\"", "\"Ⅷ Istÿc0\"");
                    runeStrings = runeStrings.Replace("\"Gul Rune\"", "\"Ⅸ Gulÿc0\"");
                    runeStrings = runeStrings.Replace("\"Vex Rune\"", "\"Ⅹ Vexÿc0\"");
                    runeStrings = runeStrings.Replace("\"Ohm Rune\"", "\"Ⅺ Ohmÿc0\"");
                    runeStrings = runeStrings.Replace("\"Lo Rune\"", "\"Ⅻ Loÿc0\"");
                    runeStrings = runeStrings.Replace("\"Sur Rune\"", "\"Ⅼ Surÿc0\"");
                    runeStrings = runeStrings.Replace("\"Ber Rune\"", "\"Ⅽ Berÿc0\"");
                    runeStrings = runeStrings.Replace("\"Jah Rune\"", "\"Ⅾ Jahÿc0\"");
                    runeStrings = runeStrings.Replace("\"Cham Rune\"", "\"Ⅿ Chamÿc0\"");
                    runeStrings = runeStrings.Replace("\"Zod Rune\"", "\"ⅰ Zodÿc0\"");
                    await File.WriteAllTextAsync(runeStringJsonFile, runeStrings);
                    break;
                }
        }
    }
    private async Task ConfigureItemILvls() //Show Item Levels
    {
        string excelPath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel");
        string armorTxtPath = System.IO.Path.Combine(excelPath, "armor.txt");
        string miscTxtPath = System.IO.Path.Combine(excelPath, "misc.txt");
        string weaponsTxtPath = System.IO.Path.Combine(excelPath, "weapons.txt");
        string[] files = new string[] { "armor.txt", "misc.txt", "weapons.txt" };


        if (ModInfo.Name == "RMD-MP")
        {
            armorTxtPath = System.IO.Path.Combine(excelPath, "armor.bin");
            miscTxtPath = System.IO.Path.Combine(excelPath, "misc.bin");
            weaponsTxtPath = System.IO.Path.Combine(excelPath, "weapons.bin");
            files = new string[] { "armor.bin", "misc.bin", "weapons.bin" };
        }
        else
        {
            armorTxtPath = System.IO.Path.Combine(excelPath, "armor.txt");
            miscTxtPath = System.IO.Path.Combine(excelPath, "misc.txt");
            weaponsTxtPath = System.IO.Path.Combine(excelPath, "weapons.txt");
            files = new string[] { "armor.txt", "misc.txt", "weapons.txt" };
        }
        eEnabledDisabledModify itemLvls = (eEnabledDisabledModify)UserSettings.ItemIlvls;

        switch (itemLvls)
        {
            case eEnabledDisabledModify.NoChange:
                return;

            case eEnabledDisabledModify.Disabled:
                {
                    //search the defined files
                    foreach (string file in files)
                    {
                        if (!Directory.Exists(excelPath))
                            Directory.CreateDirectory(excelPath);

                        if (ModInfo.Name == "RMD-MP")
                        {
                            if (File.Exists(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "Off", "armor.bin")))
                                File.Copy(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "Off", "armor.bin"), System.IO.Path.Combine(SelectedModDataFolder, "global", "excel", "armor.bin"), true);

                            if (File.Exists(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "Off", "misc.bin")))
                                File.Copy(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "Off", "misc.bin"), System.IO.Path.Combine(SelectedModDataFolder, "global", "excel", "misc.bin"), true);

                            if (File.Exists(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "Off", "weapons.bin")))
                                File.Copy(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "Off", "weapons.bin"), System.IO.Path.Combine(SelectedModDataFolder, "global", "excel", "weapons.bin"), true);
                        }
                        else
                        {
                            if (!File.Exists(armorTxtPath))
                                await File.WriteAllBytesAsync(armorTxtPath, await Helper.GetResourceByteArray("CASC.armor.txt"));
                            if (!File.Exists(miscTxtPath))
                                await File.WriteAllBytesAsync(miscTxtPath, await Helper.GetResourceByteArray("CASC.misc.txt"));
                            if (!File.Exists(weaponsTxtPath))
                                await File.WriteAllBytesAsync(weaponsTxtPath, await Helper.GetResourceByteArray("CASC.weapons.txt"));

                            string filePath = System.IO.Path.Combine(excelPath, file);

                            if (!File.Exists(filePath))
                                continue;

                            string[] lines = await File.ReadAllLinesAsync(filePath);

                            if (lines.Length == 0)
                                continue;

                            string[] headers = lines[0].Split('\t'); //split by tab-delimited format
                            int showLevelIndex = Array.IndexOf(headers, "ShowLevel"); //make an array from the 'ShowLevel' entries

                            //search through 'ShowLevel' entries further
                            for (int i = 1; i < lines.Length; i++)
                            {
                                string[] columns = lines[i].Split('\t');
                                //check if entries match the dropdown index of 0 or 1
                                if (columns.Length > showLevelIndex && columns[showLevelIndex] != (UserSettings.ItemIlvls - 1).ToString())
                                {

                                    columns[showLevelIndex] = (UserSettings.ItemIlvls - 1).ToString();
                                    lines[i] = string.Join("\t", columns); //replace the 0 or 1 values as dropdown indicates
                                }
                            }
                            //We done boys
                            File.WriteAllLines(filePath, lines);
                        }
                    }
                    break;
                }
            case eEnabledDisabledModify.Enabled:
                {
                    //search the defined files
                    foreach (string file in files)
                    {
                        if (!Directory.Exists(excelPath))
                            Directory.CreateDirectory(excelPath);

                        if (ModInfo.Name == "RMD-MP")
                        {
                            if (File.Exists(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "On", "armor.bin")))
                                File.Copy(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "On", "armor.bin"), System.IO.Path.Combine(SelectedModDataFolder, "global", "excel", "armor.bin"), true);

                            if (File.Exists(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "On", "misc.bin")))
                                File.Copy(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "On", "misc.bin"), System.IO.Path.Combine(SelectedModDataFolder, "global", "excel", "misc.bin"), true);

                            if (File.Exists(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "On", "weapons.bin")))
                                File.Copy(System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN", "Show iLvls", "On", "weapons.bin"), System.IO.Path.Combine(SelectedModDataFolder, "global", "excel", "weapons.bin"), true);
                        }
                        else
                        {
                            if (!File.Exists(armorTxtPath))
                                await File.WriteAllBytesAsync(armorTxtPath, await Helper.GetResourceByteArray("CASC.armor.txt"));
                            if (!File.Exists(miscTxtPath))
                                await File.WriteAllBytesAsync(miscTxtPath, await Helper.GetResourceByteArray("CASC.misc.txt"));
                            if (!File.Exists(weaponsTxtPath))
                                await File.WriteAllBytesAsync(weaponsTxtPath, await Helper.GetResourceByteArray("CASC.weapons.txt"));

                            string filePath = System.IO.Path.Combine(excelPath, file);

                            if (!File.Exists(filePath))
                                continue;

                            string[] lines = await File.ReadAllLinesAsync(filePath);

                            if (lines.Length == 0)
                                continue;

                            string[] headers = lines[0].Split('\t'); //split by tab-delimited format
                            int showLevelIndex = Array.IndexOf(headers, "ShowLevel"); //make an array from the 'ShowLevel' entries

                            //search through 'ShowLevel' entries further
                            for (int i = 1; i < lines.Length; i++)
                            {
                                string[] columns = lines[i].Split('\t');
                                //check if entries match the dropdown index of 0 or 1
                                if (columns.Length > showLevelIndex && columns[showLevelIndex] != UserSettings.ItemIlvls.ToString())
                                {
                                    columns[showLevelIndex] = UserSettings.ItemIlvls.ToString();
                                    lines[i] = string.Join("\t", columns); //replace the 0 or 1 values as dropdown indicates
                                }
                            }
                            //We done boys
                            File.WriteAllLines(filePath, lines);
                        }
                    }
                    break;
                }
        }
    }
    private async Task ConfigureMercIcons() //Merc Icons
    {
        eMercIdentifier mercIdentifier = (eMercIdentifier)UserSettings.MercIcons;

        string dataHdPath = System.IO.Path.Combine(SelectedModDataFolder, "hd");
        string mercNameTexturePath = System.IO.Path.Combine(dataHdPath, "vfx/textures/MercName.texture");
        string mercNameParticlesPath = System.IO.Path.Combine(dataHdPath, "vfx/particles/MercName.particles");
        string mercNameDimTexturePath = System.IO.Path.Combine(dataHdPath, "vfx/textures/MercNameDim.texture");
        string rogueHireJsonPath = System.IO.Path.Combine(dataHdPath, "character/enemy/roguehire.json");
        string act2HireJsonPath = System.IO.Path.Combine(dataHdPath, "character/enemy/act2hire.json");
        string act3HireJsonPath = System.IO.Path.Combine(dataHdPath, "character/enemy/act3hire.json");
        string act5HireJsonPath = System.IO.Path.Combine(dataHdPath, "character/enemy/act5hire1.json");
        string enemyPath = System.IO.Path.Combine(dataHdPath, "character/enemy");
        string texturesPath = System.IO.Path.Combine(dataHdPath, "vfx/textures");
        string particlesPath = System.IO.Path.Combine(dataHdPath, "vfx/particles");

        switch (mercIdentifier)
        {
            case eMercIdentifier.Disabled:
                {
                    if (File.Exists(mercNameTexturePath))
                    {
                        File.Delete(mercNameTexturePath);
                        File.Delete(mercNameDimTexturePath);
                    }
                    if (File.Exists(rogueHireJsonPath))
                    {
                        File.Delete(rogueHireJsonPath);
                        File.Delete(act2HireJsonPath);
                        File.Delete(act3HireJsonPath);
                        File.Delete(act5HireJsonPath);
                    }
                    break;
                }
            case eMercIdentifier.Enabled:
                {
                    if (!Directory.Exists(enemyPath))
                        Directory.CreateDirectory(enemyPath);
                    if (!Directory.Exists(texturesPath))
                        Directory.CreateDirectory(texturesPath);
                    if (!Directory.Exists(particlesPath))
                        Directory.CreateDirectory(particlesPath);

                    await File.WriteAllBytesAsync(rogueHireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.roguehire.json"));
                    await File.WriteAllBytesAsync(act2HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act2hire.json"));
                    await File.WriteAllBytesAsync(act3HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act3hire.json"));
                    await File.WriteAllBytesAsync(act5HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act5hire1.json"));
                    await File.WriteAllBytesAsync(mercNameTexturePath, await Helper.GetResourceByteArray("Options.MercIcons.MercName1a.texture"));
                    await File.WriteAllBytesAsync(mercNameDimTexturePath, await Helper.GetResourceByteArray("Options.MercIcons.MercName1b.texture"));
                    await File.WriteAllBytesAsync(mercNameParticlesPath, await Helper.GetResourceByteArray("Options.MercIcons.MercName.particles"));
                    break;
                }
            case eMercIdentifier.EnabledMini:
                {
                    if (!Directory.Exists(enemyPath))
                        Directory.CreateDirectory(enemyPath);
                    if (!Directory.Exists(texturesPath))
                        Directory.CreateDirectory(texturesPath);
                    if (!Directory.Exists(particlesPath))
                        Directory.CreateDirectory(particlesPath);

                    await File.WriteAllBytesAsync(rogueHireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.roguehire.json"));
                    await File.WriteAllBytesAsync(act2HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act2hire.json"));
                    await File.WriteAllBytesAsync(act3HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act3hire.json"));
                    await File.WriteAllBytesAsync(act5HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act5hire1.json"));
                    await File.WriteAllBytesAsync(mercNameTexturePath, await Helper.GetResourceByteArray("Options.MercIcons.MercName2a.texture"));
                    await File.WriteAllBytesAsync(mercNameDimTexturePath, await Helper.GetResourceByteArray("Options.MercIcons.MercName2b.texture"));
                    await File.WriteAllBytesAsync(mercNameParticlesPath, await Helper.GetResourceByteArray("Options.MercIcons.MercName.particles"));
                    break;
                }
        }
    }
    private async Task ConfigureSkillIcons() //Skill Icon Packs
    {
        eSkillIconPack skillIconPack = (eSkillIconPack)UserSettings.SkillIcons;

        string globalUiSpellPath = System.IO.Path.Combine(SelectedModDataFolder, "hd/global/ui/spells");
        string amazonSkillIconsPath = System.IO.Path.Combine(SelectedModDataFolder, "hd/global/ui/spells/amazon/amskillicon2.sprite");
        string profileHdJsonPath = System.IO.Path.Combine(SelectedModDataFolder, "global/ui/layouts/_profilehd.json");
        string skillsTreePanelHdJsonPath = System.IO.Path.Combine(SelectedModDataFolder, "global/ui/layouts/skillstreepanelhd.json");
        string ControllerskillsTreePanelHdJsonPath = System.IO.Path.Combine(SelectedModDataFolder, "global/ui/layouts/controller/skillstreepanelhd.json");

        string cascProfileHdJsonFileName = @"data:data\global\ui\layouts\_profilehd.json";
        string cascSkillsTreePanelHdJsonFileName = @"data:data\global\ui\layouts\skillstreepanelhd.json";
        string cascControllerSkillsTreePanelHdJsonFileName = @"data:data\global\ui\layouts\controller\skillstreepanelhd.json";

        //Create Skill Icons if they don't exist
        if (!File.Exists(amazonSkillIconsPath))
        {
            string skillIconsPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "SkillIcons.zip");

            await File.WriteAllBytesAsync(skillIconsPath, await Helper.GetResourceByteArray("Options.D2RL_SkillIcons.zip"));
            ZipFile.ExtractToDirectory(skillIconsPath, globalUiSpellPath);
            File.Delete(skillIconsPath);
        }

        switch (skillIconPack)
        {
            case eSkillIconPack.Disabled:
                {
                    if (File.Exists(profileHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(profileHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon\"").Replace("AmSkillicon3\"", "AmSkillicon\"");
                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon\"").Replace("AsSkillicon3\"", "AsSkillicon\"");
                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon\"").Replace("BaSkillicon3\"", "BaSkillicon\"");
                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon\"").Replace("DrSkillicon3\"", "DrSkillicon\"");
                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon\"").Replace("NeSkillicon3\"", "NeSkillicon\"");
                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon\"").Replace("PaSkillicon3\"", "PaSkillicon\"");
                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon\"").Replace("SoSkillicon3\"", "SoSkillicon\"");
                        await File.WriteAllTextAsync(profileHdJsonPath, profileContents);
                    }

                    if (File.Exists(skillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(skillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon\"").Replace("AmSkillicon3\"", "AmSkillicon\"");
                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon\"").Replace("AsSkillicon3\"", "AsSkillicon\"");
                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon\"").Replace("BaSkillicon3\"", "BaSkillicon\"");
                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon\"").Replace("DrSkillicon3\"", "DrSkillicon\"");
                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon\"").Replace("NeSkillicon3\"", "NeSkillicon\"");
                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon\"").Replace("PaSkillicon3\"", "PaSkillicon\"");
                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon\"").Replace("SoSkillicon3\"", "SoSkillicon\"");
                        await File.WriteAllTextAsync(skillsTreePanelHdJsonPath, profileContents);
                    }

                    if (File.Exists(ControllerskillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(ControllerskillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon\"").Replace("AmSkillicon3\"", "AmSkillicon\"");
                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon\"").Replace("AsSkillicon3\"", "AsSkillicon\"");
                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon\"").Replace("BaSkillicon3\"", "BaSkillicon\"");
                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon\"").Replace("DrSkillicon3\"", "DrSkillicon\"");
                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon\"").Replace("NeSkillicon3\"", "NeSkillicon\"");
                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon\"").Replace("PaSkillicon3\"", "PaSkillicon\"");
                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon\"").Replace("SoSkillicon3\"", "SoSkillicon\"");
                        await File.WriteAllTextAsync(ControllerskillsTreePanelHdJsonPath, profileContents);
                    }
                    break;
                }
            case eSkillIconPack.ReMoDDeD:
                {
                    if (!File.Exists(profileHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascProfileHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(skillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(ControllerskillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascControllerSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (File.Exists(profileHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(profileHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon\"", "AmSkillicon2\"").Replace("AmSkillicon3\"", "AmSkillicon2\"");
                        profileContents = profileContents.Replace("AsSkillicon\"", "AsSkillicon2\"").Replace("AsSkillicon3\"", "AsSkillicon2\"");
                        profileContents = profileContents.Replace("BaSkillicon\"", "BaSkillicon2\"").Replace("BaSkillicon3\"", "BaSkillicon2\"");
                        profileContents = profileContents.Replace("DrSkillicon\"", "DrSkillicon2\"").Replace("DrSkillicon3\"", "DrSkillicon2\"");
                        profileContents = profileContents.Replace("NeSkillicon\"", "NeSkillicon2\"").Replace("NeSkillicon3\"", "NeSkillicon2\"");
                        profileContents = profileContents.Replace("PaSkillicon\"", "PaSkillicon2\"").Replace("PaSkillicon3\"", "PaSkillicon2\"");
                        profileContents = profileContents.Replace("SoSkillicon\"", "SoSkillicon2\"").Replace("SoSkillicon3\"", "SoSkillicon2\"");
                        await File.WriteAllTextAsync(profileHdJsonPath, profileContents);
                    }

                    if (File.Exists(skillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(skillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon\"", "AmSkillicon2\"").Replace("AmSkillicon3\"", "AmSkillicon2\"");
                        profileContents = profileContents.Replace("AsSkillicon\"", "AsSkillicon2\"").Replace("AsSkillicon3\"", "AsSkillicon2\"");
                        profileContents = profileContents.Replace("BaSkillicon\"", "BaSkillicon2\"").Replace("BaSkillicon3\"", "BaSkillicon2\"");
                        profileContents = profileContents.Replace("DrSkillicon\"", "DrSkillicon2\"").Replace("DrSkillicon3\"", "DrSkillicon2\"");
                        profileContents = profileContents.Replace("NeSkillicon\"", "NeSkillicon2\"").Replace("NeSkillicon3\"", "NeSkillicon2\"");
                        profileContents = profileContents.Replace("PaSkillicon\"", "PaSkillicon2\"").Replace("PaSkillicon3\"", "PaSkillicon2\"");
                        profileContents = profileContents.Replace("SoSkillicon\"", "SoSkillicon2\"").Replace("SoSkillicon3\"", "SoSkillicon2\"");
                        await File.WriteAllTextAsync(skillsTreePanelHdJsonPath, profileContents);
                    }

                    if (File.Exists(ControllerskillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(ControllerskillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon\"", "AmSkillicon2\"").Replace("AmSkillicon3\"", "AmSkillicon2\"");
                        profileContents = profileContents.Replace("AsSkillicon\"", "AsSkillicon2\"").Replace("AsSkillicon3\"", "AsSkillicon2\"");
                        profileContents = profileContents.Replace("BaSkillicon\"", "BaSkillicon2\"").Replace("BaSkillicon3\"", "BaSkillicon2\"");
                        profileContents = profileContents.Replace("DrSkillicon\"", "DrSkillicon2\"").Replace("DrSkillicon3\"", "DrSkillicon2\"");
                        profileContents = profileContents.Replace("NeSkillicon\"", "NeSkillicon2\"").Replace("NeSkillicon3\"", "NeSkillicon2\"");
                        profileContents = profileContents.Replace("PaSkillicon\"", "PaSkillicon2\"").Replace("PaSkillicon3\"", "PaSkillicon2\"");
                        profileContents = profileContents.Replace("SoSkillicon\"", "SoSkillicon2\"").Replace("SoSkillicon3\"", "SoSkillicon2\"");
                        await File.WriteAllTextAsync(ControllerskillsTreePanelHdJsonPath, profileContents);
                    }
                    break;
                }
            case eSkillIconPack.Dize:
                {
                    if (!File.Exists(profileHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascProfileHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(skillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(ControllerskillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascControllerSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (File.Exists(profileHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(profileHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon3\"").Replace("AmSkillicon\"", "AmSkillicon3\"");
                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon3\"").Replace("AsSkillicon\"", "AsSkillicon3\"");
                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon3\"").Replace("BaSkillicon\"", "BaSkillicon3\"");
                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon3\"").Replace("DrSkillicon\"", "DrSkillicon3\"");
                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon3\"").Replace("NeSkillicon\"", "NeSkillicon3\"");
                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon3\"").Replace("PaSkillicon\"", "PaSkillicon3\"");
                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon3\"").Replace("SoSkillicon\"", "SoSkillicon3\"");
                        await File.WriteAllTextAsync(profileHdJsonPath, profileContents);
                    }

                    if (File.Exists(skillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(skillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon3\"").Replace("AmSkillicon\"", "AmSkillicon3\"");
                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon3\"").Replace("AsSkillicon\"", "AsSkillicon3\"");
                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon3\"").Replace("BaSkillicon\"", "BaSkillicon3\"");
                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon3\"").Replace("DrSkillicon\"", "DrSkillicon3\"");
                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon3\"").Replace("NeSkillicon\"", "NeSkillicon3\"");
                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon3\"").Replace("PaSkillicon\"", "PaSkillicon3\"");
                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon3\"").Replace("SoSkillicon\"", "SoSkillicon3\"");
                        await File.WriteAllTextAsync(skillsTreePanelHdJsonPath, profileContents);
                    }

                    if (File.Exists(ControllerskillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(ControllerskillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon\"", "AmSkillicon2\"").Replace("AmSkillicon3\"", "AmSkillicon2\"");
                        profileContents = profileContents.Replace("AsSkillicon\"", "AsSkillicon2\"").Replace("AsSkillicon3\"", "AsSkillicon2\"");
                        profileContents = profileContents.Replace("BaSkillicon\"", "BaSkillicon2\"").Replace("BaSkillicon3\"", "BaSkillicon2\"");
                        profileContents = profileContents.Replace("DrSkillicon\"", "DrSkillicon2\"").Replace("DrSkillicon3\"", "DrSkillicon2\"");
                        profileContents = profileContents.Replace("NeSkillicon\"", "NeSkillicon2\"").Replace("NeSkillicon3\"", "NeSkillicon2\"");
                        profileContents = profileContents.Replace("PaSkillicon\"", "PaSkillicon2\"").Replace("PaSkillicon3\"", "PaSkillicon2\"");
                        profileContents = profileContents.Replace("SoSkillicon\"", "SoSkillicon2\"").Replace("SoSkillicon3\"", "SoSkillicon2\"");
                        await File.WriteAllTextAsync(ControllerskillsTreePanelHdJsonPath, profileContents);
                    }
                    break;
                }
        }
    }
    private async Task ConfigureBuffIcons() //Buff Icons
    {
        string buffIconsParticlesPath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Buff Icons/Particles");
        string buffIconsParticlesDisabledPath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Buff Icons/Particles (Disabled)");

        if ((eEnabledDisabled)UserSettings.BuffIcons == eEnabledDisabled.Disabled)
        {
            if (Directory.Exists(buffIconsParticlesPath))
                Directory.Move(buffIconsParticlesPath, buffIconsParticlesDisabledPath);
        }
        if ((eEnabledDisabled)UserSettings.BuffIcons == eEnabledDisabled.Enabled)
        {
            if (Directory.Exists(buffIconsParticlesDisabledPath))
                Directory.Move(buffIconsParticlesDisabledPath, buffIconsParticlesPath);
        }
    }

    #region ---Super TK---

    private async Task ConfigureSuperTelekinesis() //Super TelekinesisL
    {
        eEnabledDisabled superTelekinesis = (eEnabledDisabled)UserSettings.SuperTelekinesis;

        switch (superTelekinesis)
        {
            case eEnabledDisabled.Disabled:
                {
                    if (ModInfo.Name == "RMD-MP")
                        return;

                    RemoveSuperTkSkill();
                    break;
                }
            case eEnabledDisabled.Enabled:
                {
                    if (ModInfo.Name == "RMD-MP")
                        return;

                    CreateSuperTKSkill();
                    string charStatsPath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/charstats.txt"));
                    string itemTypesPath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/itemtypes.txt"));

                    if (File.Exists(charStatsPath) && File.Exists(itemTypesPath))
                    {
                        string[] charStatsLines = await File.ReadAllLinesAsync(charStatsPath);
                        string[] itemTypesLines = await File.ReadAllLinesAsync(itemTypesPath);

                        for (int i = 0; i < charStatsLines.Length; i++)
                        {
                            string line = charStatsLines[i];
                            string[] splitContent = line.Split('\t');

                            if (i > 0 && i != 6)
                            {
                                splitContent[34] = "SuperTK";
                                charStatsLines[i] = string.Join("\t", splitContent);
                            }
                        }

                        for (int i = 0; i < itemTypesLines.Length; i++)
                        {
                            string line = itemTypesLines[i];
                            string[] splitContent = line.Split('\t');

                            if (i == 14 || i == 21 || i == 60 || i == 76) splitContent[3] = "poti";
                            if (i == 46 || i == 51) splitContent[2] = "gold";

                            itemTypesLines[i] = string.Join("\t", splitContent);
                        }

                        // Write the modified content back to the files
                        File.WriteAllLines(charStatsPath, charStatsLines);
                        File.WriteAllLines(itemTypesPath, itemTypesLines);
                    }

                    break;
                }
        }
    }
    private async Task CreateSuperTKSkill()
    {
        string skillTextPath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/skills.txt");
        string itemTypesTextPath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/itemtypes.txt");
        string charStatsPath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/charstats.txt");
        string originalSkillTextPath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Originals/skills-original.txt");
        string originalsDirectoryPath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Originals");

        //Create needed folders and files
        if (!File.Exists(itemTypesTextPath))
            await File.WriteAllBytesAsync(itemTypesTextPath, await Helper.GetResourceByteArray("CASC.itemtypes.txt"));
        if (!File.Exists(charStatsPath))
            await File.WriteAllBytesAsync(charStatsPath, await Helper.GetResourceByteArray("CASC.charstats.txt"));
        if (!Directory.Exists(originalsDirectoryPath))
            Directory.CreateDirectory(originalsDirectoryPath);
        if (!File.Exists(skillTextPath))
            await File.WriteAllBytesAsync(skillTextPath, await Helper.GetResourceByteArray("CASC.skills.txt"));

        File.Copy(skillTextPath, originalSkillTextPath, true);

        //Check to see if we already added the skill previously
        bool superTKExists = false;
        using (StreamReader reader = new StreamReader(skillTextPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split('\t');
                if (columns.Length > 0 && columns[0] == "SuperTK")
                {
                    superTKExists = true;
                    break;
                }
            }
        }

        if (!superTKExists)
        {
            //Skill doesn't exist yet; let's create it
            int tkKSkill = 44; //ID of TK Skill
            string[] lines = File.ReadAllLines(skillTextPath);
            int lineCount = 0; //track ID we added skill to

            // Check if the specified line number is valid
            if (tkKSkill < lines.Length)
            {
                string lineToCopy = lines[tkKSkill];
                lineCount = lines.Length;
                Array.Resize(ref lines, lineCount + 1);
                lines[lineCount] = lineToCopy;
                File.WriteAllLines(skillTextPath, lines);
            }

            //TK has been cloned now; let's edit it
            string[] linesS = File.ReadAllLines(skillTextPath);
            string outstr = "";
            string sep = "\t";
            int index = 0;

            foreach (string line in linesS)
            {
                string[] splitContent = line.Split(sep.ToCharArray());

                if (index == lineCount)
                {
                    splitContent[0] = "SuperTK"; //Change Skill Name
                    splitContent[1] = (lineCount - 2).ToString(); //Update Comment ID
                    splitContent[2] = ""; //Remove sorceress-type skill
                    splitContent[189] = ""; //Remove mana requirement
                    splitContent[214] = "50"; //Increase Range
                    splitContent[216] = "0"; //Remove Knockback Chance
                    splitContent[244] = "13"; //Remove Knockback Layer
                    for (int i = 261; i <= 273; i++)
                    {
                        splitContent[i] = "";
                    }

                    outstr += String.Join("\t", splitContent) + "\n";
                }
                else
                {
                    outstr += line + "\n";
                }
                index += 1;
            }
            File.WriteAllText(skillTextPath, outstr);
        }
    } //Used in ConfigureSuperTelekinesis()
    private void RemoveSuperTkSkill() //Used in ConfigureSuperTelekinesis()
    {
        string skillTextPath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/skills.txt");

        if (File.Exists(skillTextPath))
        {
            string originalSkillTextPath = System.IO.Path.Combine(SelectedModDataFolder, "D2RLAN/Originals/skills-original.txt");

            //Remove SuperTK from charstats and itemtypes
            if (File.Exists(originalSkillTextPath))
                File.Copy(originalSkillTextPath, skillTextPath, true);

            string charStatsPath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/charstats.txt");
            string itemTypesPath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/itemtypes.txt");

            if (File.Exists(charStatsPath) && File.Exists(itemTypesPath))
            {
                string[] charStatsLines = File.ReadAllLines(charStatsPath);
                string[] itemTypesLines = File.ReadAllLines(itemTypesPath);

                for (int i = 0; i < charStatsLines.Length; i++)
                {
                    string line = charStatsLines[i];
                    string[] splitContent = line.Split('\t');

                    //Write blank entries to remove SuperTK Skill reference
                    if (i > 0 && i != 6)
                    {
                        splitContent[34] = "";
                        charStatsLines[i] = string.Join("\t", splitContent);
                    }
                }

                for (int i = 0; i < itemTypesLines.Length; i++)
                {
                    //Write blank entries to remove Equiv2 itemtype modifiers
                    string line = itemTypesLines[i];
                    string[] splitContent = line.Split('\t');

                    if (i == 14 || i == 21 || i == 60 || i == 76)
                        splitContent[3] = "";
                    if (i == 46 || i == 51)
                        splitContent[2] = "";

                    itemTypesLines[i] = string.Join("\t", splitContent);
                }

                // Write the modified content back to the files
                File.WriteAllLines(charStatsPath, charStatsLines);
                File.WriteAllLines(itemTypesPath, itemTypesLines);
            }

            //Remove SuperTK from skills
            bool superTKExists = false;
            List<string> lines = new List<string>();

            //Check the last entry in file for the SuperTK skill; if it exists, flag it
            using (StreamReader reader = new StreamReader(skillTextPath))
            {
                while (reader.ReadLine() is { } line)
                {
                    string[] columns = line.Split('\t');
                    if (columns.Length > 0 && columns[0] == "SuperTK")
                        superTKExists = true;
                    else
                        lines.Add(line);
                }
            }

            //Check for flag and write the modified content back to the file
            if (superTKExists)
            {
                using StreamWriter writer = new StreamWriter(skillTextPath, false);

                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }

    #endregion

    #region ---Item Icons---

    private async Task ConfigureItemIcons() //Item Display (Item/Rune Icons)
    {
        eItemDisplay itemDisplay = (eItemDisplay)UserSettings.ItemIcons;

        string itemNameJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "local/lng/strings/item-names.json");
        string itemNameOriginalJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "local/lng/strings/item-names-original.json");
        string itemRuneJsonFilePath = System.IO.Path.Combine(SelectedModDataFolder, "local/lng/strings/item-runes.json");

        switch (itemDisplay)
        {
            case eItemDisplay.NoIcons:
                {
                    ItemIconsHide(itemNameOriginalJsonFilePath, itemNameJsonFilePath);
                    RuneIconsHide(itemRuneJsonFilePath);
                    break;
                }
            case eItemDisplay.ItemRuneIcons:
                {
                    if (!Directory.Exists(SelectedModDataFolder + "/hd/ui/fonts"))
                    {
                        string fontsFolder = System.IO.Path.Combine(SelectedModDataFolder, "hd/ui/fonts");
                        byte[] font = await Helper.GetResourceByteArray($"Fonts.{UserSettings.Font}.otf");

                        if (!Directory.Exists(fontsFolder))
                        {
                            Directory.CreateDirectory(fontsFolder);
                            File.Create(System.IO.Path.Combine(fontsFolder, "exocetblizzardot-medium.otf")).Close();
                        }

                        await File.WriteAllBytesAsync(System.IO.Path.Combine(fontsFolder, "exocetblizzardot-medium.otf"), font);
                    }

                    if (!File.Exists(itemNameJsonFilePath))
                        await File.WriteAllBytesAsync(itemNameJsonFilePath, await Helper.GetResourceByteArray("CASC.item-names.json"));

                    if (!File.Exists(itemRuneJsonFilePath))
                        await File.WriteAllBytesAsync(itemRuneJsonFilePath, await Helper.GetResourceByteArray("CASC.item-runes.json"));

                    string namesFile = await File.ReadAllTextAsync(itemNameJsonFilePath);

                    if (namesFile.Contains("Chipped Emerald"))
                        await File.WriteAllTextAsync(itemNameOriginalJsonFilePath, namesFile);

                    ItemIconsShow(itemNameJsonFilePath);
                    RuneIconsShow(itemRuneJsonFilePath);
                    break;
                }
            case eItemDisplay.ItemIconsOnly:
                {
                    if (!File.Exists(itemNameJsonFilePath))
                        await File.WriteAllBytesAsync(itemNameJsonFilePath, await Helper.GetResourceByteArray("CASC.item-names.json"));

                    string namesFile = await File.ReadAllTextAsync(itemNameJsonFilePath);

                    if (namesFile.Contains("Chipped Emerald"))
                        await File.WriteAllTextAsync(itemNameOriginalJsonFilePath, namesFile);

                    ItemIconsShow(itemNameJsonFilePath);
                    RuneIconsHide(itemRuneJsonFilePath);
                    break;
                }
            case eItemDisplay.RuneIconsOnly:
                {
                    if (!File.Exists(itemRuneJsonFilePath))
                        await File.WriteAllBytesAsync(itemRuneJsonFilePath, await Helper.GetResourceByteArray("CASC.item-runes.json"));

                    ItemIconsHide(itemNameOriginalJsonFilePath, itemNameJsonFilePath);
                    RuneIconsShow(itemRuneJsonFilePath);
                    break;
                }
        }
    }
    private void ItemIconsShow(string itemNameOriginalJsonFilePath) //Used in ConfigureItemIcons()
    {
        string itemNames = File.ReadAllText(itemNameOriginalJsonFilePath);

        if (ModInfo.Name == "RMD-MP" || ModInfo.Name == "VNP-MP")
        {
            //Replace Potions, Scrolls and Keys
            itemNames = itemNames.Replace("\"Minor Healing Potion\"", "\"ÿc1 ³ ÿc0\"").Replace("\"Light Healing Potion\"", "\"ÿc1 ³ ÿc0\"").Replace("\"Healing Potion\"", "\"ÿc1 ³ ÿc0\"").Replace("\"Greater Healing Potion\"", "\"ÿc1¸ ÿc0\"").Replace("\"Super Healing Potion\"", "\"ÿc1¸ ÿc0\"");
            itemNames = itemNames.Replace("\"Minor Mana Potion\"", "\"ÿc3 ³ ÿc0\"").Replace("\"Light Mana Potion\"", "\"ÿc3 ³ ÿc0\"").Replace("\"Mana Potion\"", "\"ÿc3 ³ ÿc0\"").Replace("\"Greater Mana Potion\"", "\"ÿc3¸ ÿc0\"").Replace("\"Super Mana Potion\"", "\"ÿc3¸ ÿc0\"");
            itemNames = itemNames.Replace("\"Rejuvenation Potion\"", "\"ÿc; ³ ÿc0\"").Replace("\"Full Rejuvenation Potion\"", "\"ÿc; ¸ ÿc0\"").Replace("\"Antidote Potion\"", "\"ÿc5 ³ ÿc0\"").Replace("\"Thawing Potion\"", "\"ÿc9 ³ ÿc0\"").Replace("\"Stamina Potion\"", "\"ÿc0 ³ ÿc0\"");
            itemNames = itemNames.Replace("\"Scroll of Town Portal\"", "\"ÿc3 ¯ ÿc0\"").Replace("\"Scroll of Identify\"", "\"ÿc1 ¯ ÿc0\"").Replace("\"enUS\": \"Key\"", "\"enUS\": \"ÿc4 ±ÿc0\"");
        }
        else
        {
            //Replace Potions, Scrolls and Keys
            itemNames = itemNames.Replace("\"Minor Healing Potion\"", "\"ÿc1 © ÿc0\"").Replace("\"Light Healing Potion\"", "\"ÿc1 ª ÿc0\"").Replace("\"Healing Potion\"", "\"ÿc1 « ÿc0\"").Replace("\"Greater Healing Potion\"", "\"ÿc1 ¬ ÿc0\"").Replace("\"Super Healing Potion\"", "\"ÿc1 ® ÿc0\"");
            itemNames = itemNames.Replace("\"Minor Mana Potion\"", "\"ÿc3 © ÿc0\"").Replace("\"Light Mana Potion\"", "\"ÿc3 ª ÿc0\"").Replace("\"Mana Potion\"", "\"ÿc3 « ÿc0\"").Replace("\"Greater Mana Potion\"", "\"ÿc3 ¬ ÿc0\"").Replace("\"Super Mana Potion\"", "\"ÿc3 ® ÿc0\"");
            itemNames = itemNames.Replace("\"Rejuvenation Potion\"", "\"ÿc; ³ ÿc0\"").Replace("\"Full Rejuvenation Potion\"", "\"ÿc; ¸ ÿc0\"").Replace("\"Antidote Potion\"", "\"ÿc5 ³ ÿc0\"").Replace("\"Thawing Potion\"", "\"ÿc9 ³ ÿc0\"").Replace("\"Stamina Potion\"", "\"ÿc0 ³ ÿc0\"");
            itemNames = itemNames.Replace("\"Scroll of Town Portal\"", "\"ÿc3 ¯ ÿc0\"").Replace("\"Scroll of Identify\"", "\"ÿc1 ¯ ÿc0\"").Replace("\"enUS\": \"Key\"", "\"enUS\": \"ÿc4 ±ÿc0\"");
        }

        //Replace Gems
        itemNames = itemNames.Replace("\"Chipped Amethyst\"", "\"ÿc;¶ ÿc0\"").Replace("\"Flawed Amethyst\"", "\"ÿc;¶ ÿc0\"").Replace("\"Amethyst\"", "\"ÿc;¶ ÿc0\"").Replace("\"Flawless Amethyst\"", "\"ÿc;¶ ÿc0\"").Replace("\"Perfect Amethyst\"", "\"ÿc;¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Topaz\"", "\"ÿc9¶ ÿc0\"").Replace("\"Flawed Topaz\"", "\"ÿc9¶ ÿc0\"").Replace("\"Topaz\"", "\"ÿc9¶ ÿc0\"").Replace("\"Flawless Topaz\"", "\"ÿc9¶ ÿc0\"").Replace("\"Perfect Topaz\"", "\"ÿc9¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Sapphire\"", "\"ÿc3¶ ÿc0\"").Replace("\"Flawed Sapphire\"", "\"ÿc3¶ ÿc0\"").Replace("\"Sapphire\"", "\"ÿc3¶ ÿc0\"").Replace("\"Flawless Sapphire\"", "\"ÿc3¶ ÿc0\"").Replace("\"Perfect Sapphire\"", "\"ÿc3¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Emerald\"", "\"ÿc2¶ ÿc0\"").Replace("\"Flawed Emerald\"", "\"ÿc2¶ ÿc0\"").Replace("\"Emerald\"", "\"ÿc2¶ ÿc0\"").Replace("\"Flawless Emerald\"", "\"ÿc2¶ ÿc0\"").Replace("\"Perfect Emerald\"", "\"ÿc2¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Ruby\"", "\"ÿc1¶ ÿc0\"").Replace("\"Flawed Ruby\"", "\"ÿc1¶ ÿc0\"").Replace("\"Ruby\"", "\"ÿc1¶ ÿc0\"").Replace("\"Flawless Ruby\"", "\"ÿc1¶ ÿc0\"").Replace("\"Perfect Ruby\"", "\"ÿc1¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Diamond\"", "\"¶ \"").Replace("\"Flawed Diamond\"", "\"¶ \"").Replace("\"Diamond\"", "\"¶ \"").Replace("\"Flawless Diamond\"", "\"¶ \"").Replace("\"Perfect Diamond\"", "\"¶ \"");
        itemNames = itemNames.Replace("\"Chipped Skull\"", "\"ÿc0 ¹ ÿc0\"").Replace("\"Flawed Skull\"", "\"ÿc0 ¹ ÿc0\"").Replace("\"Skull\"", "\"ÿc0 ¹ ÿc0\"").Replace("\"Flawless Skull\"", "\"ÿc0 ¹ ÿc0\"").Replace("\"Perfect Skull\"", "\"ÿc0 ¹ ÿc0\"");

        File.WriteAllText(itemNameOriginalJsonFilePath, itemNames);
    }
    private void ItemIconsHide(string itemNameOriginalJsonFilePath, string itemNameJsonFilePath) //Used in ConfigureItemIcons()
    {
        if (File.Exists(itemNameOriginalJsonFilePath))
        {
            string namesFile = File.ReadAllText(itemNameOriginalJsonFilePath);
            File.WriteAllText(itemNameJsonFilePath, namesFile);
        }
    }
    private void RuneIconsShow(string itemRuneJsonFilePath) //Used in ConfigureItemIcons()
    {
        string itemRunes = File.ReadAllText(itemRuneJsonFilePath);

        //Replace Runes
        itemRunes = itemRunes.Replace("\"El Rune\"", "\"⅐ Elÿc0\"").Replace("\"Eld Rune\"", "\"⅑ Eldÿc0\"").Replace("\"Tir Rune\"", "\"⅒ Tirÿc0\"");
        itemRunes = itemRunes.Replace("\"Nef Rune\"", "\"⅓ Nefÿc0\"").Replace("\"Eth Rune\"", "\"⅔ Ethÿc0\"").Replace("\"Ith Rune\"", "\"⅕ Ithÿc0\"");
        itemRunes = itemRunes.Replace("\"Tal Rune\"", "\"⅖ Talÿc0\"").Replace("\"Ral Rune\"", "\"⅗ Ralÿc0\"").Replace("\"Ort Rune\"", "\"⅘ Ortÿc0\"");
        itemRunes = itemRunes.Replace("\"Thul Rune\"", "\"⅙ Thulÿc0\"").Replace("\"Amn Rune\"", "\"⅚ Amnÿc0\"").Replace("\"Sol Rune\"", "\"⅛ Solÿc0\"");
        itemRunes = itemRunes.Replace("\"Shael Rune\"", "\"⅜ Shaelÿc0\"").Replace("\"Dol Rune\"", "\"⅝ Dolÿc0\"").Replace("\"Hel Rune\"", "\"⅞ Helÿc0\"");
        itemRunes = itemRunes.Replace("\"Io Rune\"", "\"⅟ Ioÿc0\"").Replace("\"Lum Rune\"", "\"Ⅰ Lumÿc0\"").Replace("\"Ko Rune\"", "\"Ⅱ Koÿc0\"");
        itemRunes = itemRunes.Replace("\"Fal Rune\"", "\"Ⅲ Falÿc0\"").Replace("\"Lem Rune\"", "\"Ⅳ Lemÿc0\"").Replace("\"Pul Rune\"", "\"Ⅴ Pulÿc0\"");
        itemRunes = itemRunes.Replace("\"Um Rune\"", "\"Ⅵ Umÿc0\"").Replace("\"Mal Rune\"", "\"Ⅶ Malÿc0\"").Replace("\"Ist Rune\"", "\"Ⅷ Istÿc0\"");
        itemRunes = itemRunes.Replace("\"Gul Rune\"", "\"Ⅸ Gulÿc0\"").Replace("\"Vex Rune\"", "\"Ⅹ Vexÿc0\"").Replace("\"Ohm Rune\"", "\"Ⅺ Ohmÿc0\"");
        itemRunes = itemRunes.Replace("\"Lo Rune\"", "\"Ⅻ Loÿc0\"").Replace("\"Sur Rune\"", "\"Ⅼ Surÿc0\"").Replace("\"Ber Rune\"", "\"Ⅽ Berÿc0\"");
        itemRunes = itemRunes.Replace("\"Jah Rune\"", "\"Ⅾ Jahÿc0\"").Replace("\"Cham Rune\"", "\"Ⅿ Chamÿc0\"").Replace("\"Zod Rune\"", "\"ⅰ Zodÿc0\"");

        File.WriteAllText(itemRuneJsonFilePath, itemRunes);
    }
    private void RuneIconsHide(string itemRuneJsonFilePath) //Used in ConfigureItemIcons()
    {
        if (File.Exists(itemRuneJsonFilePath))
        {
            string itemRunes = File.ReadAllText(itemRuneJsonFilePath);

            // Reverse the replacements for Runes
            itemRunes = itemRunes.Replace("\"⅐ Elÿc0\"", "\"El Rune\"").Replace("\"⅑ Eldÿc0\"", "\"Eld Rune\"").Replace("\"⅒ Tirÿc0\"", "\"Tir Rune\"");
            itemRunes = itemRunes.Replace("\"⅓ Nefÿc0\"", "\"Nef Rune\"").Replace("\"⅔ Ethÿc0\"", "\"Eth Rune\"").Replace("\"⅕ Ithÿc0\"", "\"Ith Rune\"");
            itemRunes = itemRunes.Replace("\"⅖ Talÿc0\"", "\"Tal Rune\"").Replace("\"⅗ Ralÿc0\"", "\"Ral Rune\"").Replace("\"⅘ Ortÿc0\"", "\"Ort Rune\"");
            itemRunes = itemRunes.Replace("\"⅙ Thulÿc0\"", "\"Thul Rune\"").Replace("\"⅚ Amnÿc0\"", "\"Amn Rune\"").Replace("\"⅛ Solÿc0\"", "\"Sol Rune\"");
            itemRunes = itemRunes.Replace("\"⅜ Shaelÿc0\"", "\"Shael Rune\"").Replace("\"⅝ Dolÿc0\"", "\"Dol Rune\"").Replace("\"⅞ Helÿc0\"", "\"Hel Rune\"");
            itemRunes = itemRunes.Replace("\"⅟ Ioÿc0\"", "\"Io Rune\"").Replace("\"Ⅰ Lumÿc0\"", "\"Lum Rune\"").Replace("\"Ⅱ Koÿc0\"", "\"Ko Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅲ Falÿc0\"", "\"Fal Rune\"").Replace("\"Ⅳ Lemÿc0\"", "\"Lem Rune\"").Replace("\"Ⅴ Pulÿc0\"", "\"Pul Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅵ Umÿc0\"", "\"Um Rune\"").Replace("\"Ⅶ Malÿc0\"", "\"Mal Rune\"").Replace("\"Ⅷ Istÿc0\"", "\"Ist Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅸ Gulÿc0\"", "\"Gul Rune\"").Replace("\"Ⅹ Vexÿc0\"", "\"Vex Rune\"").Replace("\"Ⅺ Ohmÿc0\"", "\"Ohm Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅻ Loÿc0\"", "\"Lo Rune\"").Replace("\"Ⅼ Surÿc0\"", "\"Sur Rune\"").Replace("\"Ⅽ Berÿc0\"", "\"Ber Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅾ Jahÿc0\"", "\"Jah Rune\"").Replace("\"Ⅿ Chamÿc0\"", "\"Cham Rune\"").Replace("\"ⅰ Zodÿc0\"", "\"Zod Rune\"");
            File.WriteAllText(itemRuneJsonFilePath, itemRunes);
        }
    }

    #endregion

    #region ---Color Dyes---
    private async Task ConfigureColorDyes() //Enable or Disable Color Dye System
    {
        eEnabledDisabled ColorDyes = (eEnabledDisabled)UserSettings.ColorDye;

        switch (ColorDyes)
        {
            case eEnabledDisabled.Disabled:
                {
                    string filePath = "";
                    string searchString = "";
                    int rowsToDelete = 0;

                    if (ModInfo.Name == "RMD-MP")
                        return;

                    if (File.Exists(System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt"))))
                    {
                        filePath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt"));
                        searchString = "ColorDye_White";
                        rowsToDelete = 8; 
                        RemoveColorDyes(filePath, searchString, rowsToDelete);
                    }

                    if (File.Exists(System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/properties.txt"))))
                    {
                        filePath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/properties.txt"));
                        searchString = "CD_White";
                        rowsToDelete = 8;
                        RemoveColorDyes(filePath, searchString, rowsToDelete);
                    }

                    if (File.Exists(System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/states.txt"))))
                    {
                        filePath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/states.txt"));
                        searchString = "Weapon_White";
                        rowsToDelete = 28;
                        RemoveColorDyes(filePath, searchString, rowsToDelete);
                    }

                    if (File.Exists(System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/cubemain.txt"))))
                    {
                        filePath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/cubemain.txt"));
                        searchString = "Weapon - Normal -> White";
                        rowsToDelete = 224;
                        RemoveColorDyes(filePath, searchString, rowsToDelete);
                    }


                    try
                    {
                        string stringPath = Path.Combine(SelectedModDataFolder, "local/lng/strings/item-modifiers.json");

                        if (!File.Exists(stringPath))
                        {
                            Console.WriteLine("File does not exist. No entries to remove.");
                            return;
                        }

                        List<Entry> entries;

                        using (StreamReader file = File.OpenText(stringPath))
                        {
                            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                            entries = (List<Entry>)serializer.Deserialize(file, typeof(List<Entry>));
                        }

                        // Remove entries only if "key" contains "ModCD"
                        int[] idsToRemove = { 48990, 48991, 48992, 48993, 48994, 48995, 48996 };
                        entries.RemoveAll(entry => idsToRemove.Contains(entry.id) && entry.Key.Contains("ModCD"));

                        using (StreamWriter file = File.CreateText(stringPath))
                        {
                            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                            serializer.Serialize(file, entries);
                        }

                        Console.WriteLine("Entries removed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }

                    break;
                }
            case eEnabledDisabled.Enabled:
                {
                    await DyesISC();
                    await DyesProp();
                    await DyesState();
                    await DyesCube();
                    break;
                }
        }
    }


    private async Task DyesISC()
    {
        string iscPath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt"));
        string iscPath2 = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost2.txt"));

        if (!File.Exists(iscPath))
            await File.WriteAllBytesAsync(iscPath, await Helper.GetResourceByteArray("CASC.itemstatcost.txt"));

        try
        {
            int statIndex = -1;
            int idIndex = -1;
            int sendBitsIndex = -1;
            int LegacysaveBitsIndex = -1;
            int LegacysaveAddIndex = -1;
            int saveBitsIndex = -1;
            int saveAddIndex = -1;
            int descpriorityIndex = -1;
            int descfuncIndex = -1;
            int descvalIndex = -1;
            int descstr1Index = -1;
            int descstr2Index = -1;
            int eolIndex = -1;

            // Read existing content and determine column indices
            List<string> lines = new List<string>();
            List<string[]> dataRows = new List<string[]>();
            using (StreamReader reader = new StreamReader(iscPath))
            {
                string line;
                bool isFirstRow = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        // Parse the header row to get column indices
                        string[] columns = line.Split('\t');
                        statIndex = Array.IndexOf(columns, "Stat");
                        idIndex = Array.IndexOf(columns, "*ID");
                        sendBitsIndex = Array.IndexOf(columns, "Send Bits");
                        LegacysaveBitsIndex = Array.IndexOf(columns, "1.09-Save Bits");
                        LegacysaveAddIndex = Array.IndexOf(columns, "1.09-Save Add");
                        saveBitsIndex = Array.IndexOf(columns, "Save Bits");
                        saveAddIndex = Array.IndexOf(columns, "Save Add");
                        descpriorityIndex = Array.IndexOf(columns, "descpriority");
                        descfuncIndex = Array.IndexOf(columns, "descfunc");
                        descvalIndex = Array.IndexOf(columns, "descval");
                        descstr1Index = Array.IndexOf(columns, "descstrpos");
                        descstr2Index = Array.IndexOf(columns, "descstrneg");
                        eolIndex = Array.IndexOf(columns, "*eol");

                        // Verify if all indices are found
                        if (statIndex == -1 || idIndex == -1 || sendBitsIndex == -1 || LegacysaveBitsIndex == -1 || LegacysaveAddIndex == -1 || saveBitsIndex == -1 || saveAddIndex == -1 || descpriorityIndex == -1 || descfuncIndex == -1 || descvalIndex == -1 || descstr1Index == -1 || descstr2Index == -1 || eolIndex == -1)
                        {
                            throw new Exception("One or more columns not found in the header row.");
                        }

                        isFirstRow = false;
                        // Store the header row
                        lines.Add(line);
                    }
                    else
                    {
                        // Check if "ColorDye_White" already exists in the "Stat" column
                        string[] columns = line.Split('\t');
                        if (statIndex != -1 && columns.Length > statIndex && columns[statIndex] == "ColorDye_White")
                            return; // Exit the method as no modifications are needed

                        // Store existing rows for later
                        lines.Add(line);
                    }
                }
            }

            // Add 8 new empty rows
            for (int i = 0; i < 8; i++)
            {
                // Create an empty row
                string[] newRow = new string[Math.Max(statIndex, Math.Max(idIndex, Math.Max(sendBitsIndex, Math.Max(saveBitsIndex, Math.Max(saveAddIndex, Math.Max(descpriorityIndex, Math.Max(descfuncIndex, Math.Max(descvalIndex, Math.Max(descstr1Index, Math.Max(descstr2Index, eolIndex)))))))))) + 1];
                // Fill with empty strings
                Array.Fill(newRow, "");
                // Add this empty row to the dataRows list
                dataRows.Add(newRow);
            }

            // Get the total number of rows in the file
            int totalRowCount = lines.Count - 1; // Excluding the header row

            // Fill in specified columns for the new rows
            string[] colorDyes = { "ColorDye_White", "ColorDye_Black", "ColorDye_Red", "ColorDye_Green", "ColorDye_Blue", "ColorDye_Yellow", "ColorDye_Purple" };
            string[] iscStrings = { "ModCDWhite", "ModCDBlack", "ModCDRed", "ModCDGreen", "ModCDBlue", "ModCDYellow", "ModCDPurple" };
            for (int i = 0; i < 7; i++)
            {
                dataRows[i][statIndex] = colorDyes[i];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][sendBitsIndex] = "2";
                dataRows[i][LegacysaveBitsIndex] = "2";
                dataRows[i][LegacysaveAddIndex] = "1";
                dataRows[i][saveBitsIndex] = "2";
                dataRows[i][saveAddIndex] = "1";
                dataRows[i][descpriorityIndex] = "999";
                dataRows[i][descfuncIndex] = "3";
                dataRows[i][descvalIndex] = "0";
                dataRows[i][descstr1Index] = iscStrings[i];
                dataRows[i][descstr2Index] = iscStrings[i];
                dataRows[i][eolIndex] = "0";
            }

            // Fill in specified columns for the 8th row
            dataRows[7][statIndex] = "ColorDye_Tracker";
            dataRows[7][idIndex] = (totalRowCount + 7).ToString(); // Assigning unique row number
            dataRows[7][sendBitsIndex] = "4";
            dataRows[7][LegacysaveBitsIndex] = "4";
            dataRows[7][LegacysaveAddIndex] = "7";
            dataRows[7][saveBitsIndex] = "4";
            dataRows[7][saveAddIndex] = "7";
            dataRows[7][eolIndex] = "0";

            // Write back to the file
            using (StreamWriter writer = new StreamWriter(iscPath, append: false))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
                // Append the new rows to the file
                foreach (var row in dataRows)
                {
                    writer.WriteLine(string.Join("\t", row));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }

        try
        {
            // Read existing JSON file
            string filePath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "local/lng/strings/item-modifiers.json"));

            if (!File.Exists(filePath))
                await File.WriteAllBytesAsync(filePath, await Helper.GetResourceByteArray("CASC.item-modifiers.json"));

            List<Entry> entries;

            using (StreamReader file = File.OpenText(filePath))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                entries = (List<Entry>)serializer.Deserialize(file, typeof(List<Entry>));
            }

            // Add new entries for color dye strings
            entries.Add(new Entry
            {
                id = 48990,
                Key = "ModCDWhite",
                deDE = "ÿc4Farbe gefärbt: ÿc0Weiß",
                enUS = "ÿc4Color Dyed: ÿc0White",
                esES = "ÿc4Color teñido: ÿc0Blanco",
                esMX = "ÿc4Color teñido: ÿc0Blanco",
                frFR = "ÿc4Couleur teint : ÿc0Blanc",
                itIT = "ÿc4Colore tinto: ÿc0Bianco",
                jaJP = "ÿc4カラー染色: ÿc0ホワイト",
                koKR = "ÿc4색상 염색: ÿc0White",
                plPL = "ÿc4Color Barwiony: ÿc0Biały",
                ptBR = "ÿc4Cor tingida: ÿc0Branco",
                ruRU = "ÿc4Окрашенный цвет: ÿc0Белый",
                zhCN = "ÿc4Color 染色：ÿc0White",
                zhTW = "ÿc4Color 染色：ÿc0White"
            });

            entries.Add(new Entry
            {
                id = 48991,
                Key = "ModCDBlack",
                deDE = "ÿc4Farbe gefärbt: ÿc5Schwarz",
                enUS = "ÿc4Color Dyed: ÿc5Black",
                esES = "ÿc4Color teñido: ÿc5Negro",
                esMX = "ÿc4Color teñido: ÿc5Negro",
                frFR = "ÿc4Couleur teint : ÿc5Noir",
                itIT = "ÿc4Colore tinto: ÿc5Nero",
                jaJP = "ÿc4カラー染色: ÿc5ブラック",
                koKR = "ÿc4색상 염색: ÿc5Black",
                plPL = "ÿc4Color Barwiony: ÿc5Black",
                ptBR = "ÿc4Cor tingida: ÿc5Preto",
                ruRU = "ÿc4Окрашенный цвет: ÿc5Черный",
                zhCN = "ÿc4Color 染色：ÿc5Black",
                zhTW = "ÿc4Color 染色：ÿc5Black"
            });

            entries.Add(new Entry
            {
                id = 48992,
                Key = "ModCDRed",
                deDE = "ÿc4Farbe gefärbt: ÿc1Rot",
                enUS = "ÿc4Color Dyed: ÿc1Red",
                esES = "ÿc4Color teñido: ÿc1Rojo",
                esMX = "ÿc4Color teñido: ÿc1Rojo",
                frFR = "ÿc4Color Teint : ÿc1Red",
                itIT = "ÿc4Colore tinto: ÿc1Rosso",
                jaJP = "ÿc4色染め: ÿc1レッド",
                koKR = "ÿc4색상 염색: ÿc1Red",
                plPL = "ÿc4Color Barwiony: ÿc1Red",
                ptBR = "ÿc4Cor tingida: ÿc1Vermelho",
                ruRU = "Окрашенный цвет ÿc4: ÿc1Red",
                zhCN = "ÿc4Color 染色：ÿc1Red",
                zhTW = "ÿc4Color 染色：ÿc1Red"
            });

            entries.Add(new Entry
            {
                id = 48993,
                Key = "ModCDGreen",
                deDE = "ÿc4Farbe gefärbt: ÿc2Grün",
                enUS = "ÿc4Color Dyed: ÿc2Green",
                esES = "ÿc4Color teñido: ÿc2Verde",
                esMX = "ÿc4Color teñido: ÿc2Verde",
                frFR = "ÿc4Color Teint : ÿc2Green",
                itIT = "ÿc4Colore tinto: ÿc2Verde",
                jaJP = "ÿc4色染め: ÿc2グリーン",
                koKR = "ÿc4Color 염색: ÿc2Green",
                plPL = "ÿc4Color Barwiony: ÿc2Green",
                ptBR = "ÿc4Cor tingida: ÿc2Verde",
                ruRU = "ÿc4Окрашенный цвет: ÿc2Зеленый",
                zhCN = "ÿc4Color 染色：ÿc2Green",
                zhTW = "ÿc4Color 染色：ÿc2Green"
            });

            entries.Add(new Entry
            {
                id = 48994,
                Key = "ModCDBlue",
                deDE = "ÿc4Farbe gefärbt: ÿc3Blau",
                enUS = "ÿc4Color Dyed: ÿc3Blue",
                esES = "ÿc4Color teñido: ÿc3Azul",
                esMX = "ÿc4Color teñido: ÿc3Azul",
                frFR = "ÿc4Color Teint : ÿc3Blue",
                itIT = "ÿc4Colore tinto: ÿc3Blu",
                jaJP = "ÿc4カラー染色: ÿc3ブルー",
                koKR = "ÿc4Color 염색: ÿc3Blue",
                plPL = "ÿc4Color Barwiony: ÿc3Blue",
                ptBR = "ÿc4Cor tingida: ÿc3Azul",
                ruRU = "ÿc4Окрашенный цвет: ÿc3Blue",
                zhCN = "ÿc4Color 染色：ÿc3Blue",
                zhTW = "ÿc4Color 染色：ÿc3Blue"
            });

            entries.Add(new Entry
            {
                id = 48995,
                Key = "ModCDYellow",
                deDE = "ÿc4Farbe gefärbt: ÿc9Gelb",
                enUS = "ÿc4Color Dyed: ÿc9Yellow",
                esES = "ÿc4Color teñido: ÿc9Amarillo",
                esMX = "ÿc4Color teñido: ÿc9Amarillo",
                frFR = "ÿc4Couleur teinte : ÿc9Jaune",
                itIT = "ÿc4Colore tinto: ÿc9Giallo",
                jaJP = "ÿc4色染め：ÿc9イエロー",
                koKR = "ÿc4색상 염색: ÿc9Yellow",
                plPL = "ÿc4Color Barwiony: ÿc9Yellow",
                ptBR = "ÿc4Cor tingida: ÿc9Amarelo",
                ruRU = "ÿc4Окрашенный цвет: ÿc9Желтый",
                zhCN = "ÿc4颜色染色：ÿc9黄色",
                zhTW = "ÿc4顏色染色：ÿc9黃色"
            });

            entries.Add(new Entry
            {
                id = 48996,
                Key = "ModCDPurple",
                deDE = "ÿc4Farbe gefärbt: ÿc;Lila",
                enUS = "ÿc4Color Dyed: ÿc;Purple",
                esES = "ÿc4Color teñido: ÿc;Púrpura",
                esMX = "ÿc4Color teñido: ÿc;Púrpura",
                frFR = "ÿc4Color Teint : ÿc ; Violet",
                itIT = "ÿc4Colore tinto: ÿc;Viola",
                jaJP = "ÿc4カラー染色: ÿc;パープル",
                koKR = "ÿc4색상 염색: ÿc; 보라색",
                plPL = "ÿc4Color Barwiony: ÿc;Fioletowy",
                ptBR = "ÿc4Cor tingida: ÿc;Roxo",
                ruRU = "ÿc4Окрашенный цвет: ÿc;Фиолетовый",
                zhCN = "ÿc4颜色染色：ÿc；紫色",
                zhTW = "ÿc4顏色染色：ÿc；紫色"
            });

            // Write the new color dye entries back to the JSON file
            using (StreamWriter file = File.CreateText(filePath))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(file, entries);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private async Task DyesProp()
    {
        string propPath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/properties.txt");
        string propPath2 = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/properties2.txt");

        if (!File.Exists(propPath))
            await File.WriteAllBytesAsync(propPath, await Helper.GetResourceByteArray("CASC.properties.txt"));

        try
        {
            // Initialize column indices
            int codeIndex = -1, enabledIndex = -1, funcIndex = -1, statIndex = -1, eolIndex = -1;

            // Read existing content and determine column indices
            List<string> lines = new List<string>();
            List<string[]> dataRows = new List<string[]>();
            using (StreamReader reader = new StreamReader(propPath))
            {
                string line;
                bool isFirstRow = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        // Parse the header row to get column indices
                        string[] columns = line.Split('\t');
                        codeIndex = Array.IndexOf(columns, "code");
                        enabledIndex = Array.IndexOf(columns, "*Enabled");
                        funcIndex = Array.IndexOf(columns, "func1");
                        statIndex = Array.IndexOf(columns, "stat1");
                        eolIndex = Array.IndexOf(columns, "*eol");

                        // Verify if all indices are found
                        if (codeIndex == -1 || enabledIndex == -1 || funcIndex == -1 || statIndex == -1 || eolIndex == -1)
                        {
                            throw new Exception("One or more columns not found in the header row.");
                        }

                        isFirstRow = false;
                        lines.Add(line); // Store the header row
                    }
                    else
                    {
                        // Check if "CD_White" already exists in the "stat1" column
                        string[] columns = line.Split('\t');
                        if (codeIndex != -1 && columns.Length > codeIndex && columns[codeIndex] == "CD_White")
                            return;

                        lines.Add(line); // Store existing rows for later
                    }
                }
            }

            // Add 8 new empty rows
            for (int i = 0; i < 8; i++)
            {
                string[] newRow = new string[Math.Max(codeIndex, Math.Max(enabledIndex, Math.Max(funcIndex, Math.Max(statIndex, eolIndex)))) + 1];
                Array.Fill(newRow, "");
                dataRows.Add(newRow);
            }

            // Fill in specified columns for the new rows
            string[] colorDyes = { "CD_White", "CD_Black", "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple", "CD_Tracker" };
            string[] colorDyesStats = { "ColorDye_White", "ColorDye_Black", "ColorDye_Red", "ColorDye_Green", "ColorDye_Blue", "ColorDye_Yellow", "ColorDye_Purple", "ColorDye_Tracker" };
            for (int i = 0; i < 8; i++)
            {
                dataRows[i][codeIndex] = colorDyes[i];
                dataRows[i][enabledIndex] = "1";
                dataRows[i][funcIndex] = "1";
                dataRows[i][statIndex] = colorDyesStats[i];
                dataRows[i][eolIndex] = "0";
            }

            // Write back to the file
            using (StreamWriter writer = new StreamWriter(propPath, append: false))
            {
                foreach (var line in lines)
                    writer.WriteLine(line);

                foreach (var row in dataRows)
                    writer.WriteLine(string.Join("\t", row));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async Task DyesState()
    {
        string statePath = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/states.txt"));
        string statePath2 = System.IO.Path.Combine(System.IO.Path.Combine(SelectedModDataFolder, "global/excel/states2.txt"));

        if (!File.Exists(statePath))
            await File.WriteAllBytesAsync(statePath, await Helper.GetResourceByteArray("CASC.states.txt"));

        try
        {
            int stateIndex = -1;
            int idIndex = -1;
            int itemtypeIndex = -1;
            int itemtransIndex = -1;
            int eolIndex = -1;

            // Read existing content and determine column indices
            List<string> lines = new List<string>();
            List<string[]> dataRows = new List<string[]>();
            using (StreamReader reader = new StreamReader(statePath))
            {
                string line;
                bool isFirstRow = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        // Parse the header row to get column indices
                        string[] columns = line.Split('\t');
                        stateIndex = Array.IndexOf(columns, "state");
                        idIndex = Array.IndexOf(columns, "*ID");
                        itemtypeIndex = Array.IndexOf(columns, "itemtype");
                        itemtransIndex = Array.IndexOf(columns, "itemtrans");
                        eolIndex = Array.IndexOf(columns, "*eol");

                        // Verify if all indices are found
                        if (stateIndex == -1 || idIndex == -1 || itemtypeIndex == -1 || itemtransIndex == -1 || eolIndex == -1)
                        {
                            throw new Exception("One or more columns not found in the header row.");
                        }

                        isFirstRow = false;
                        // Store the header row
                        lines.Add(line);
                    }
                    else
                    {
                        // Check if "ColorDye_White" already exists in the "Stat" column
                        string[] columns = line.Split('\t');
                        if (stateIndex != -1 && columns.Length > stateIndex && columns[stateIndex] == "Weapon_White")
                            return; // Exit the method as no modifications are needed

                        // Store existing rows for later
                        lines.Add(line);
                    }
                }
            }

            // Add 8 new empty rows
            for (int i = 0; i < 28; i++)
            {
                // Create an empty row
                string[] newRow = new string[Math.Max(stateIndex, Math.Max(idIndex, Math.Max(itemtypeIndex, Math.Max(itemtransIndex,eolIndex)))) + 1];
                // Fill with empty strings
                Array.Fill(newRow, "");
                // Add this empty row to the dataRows list
                dataRows.Add(newRow);
            }

            // Get the total number of rows in the file
            int totalRowCount = lines.Count - 1; // Excluding the header row

            // Fill in specified columns for the new rows
            string[] colorDyesW = { "Weapon_White", "Weapon_Black", "Weapon_Red", "Weapon_Green", "Weapon_Blue", "Weapon_Yellow", "Weapon_Purple" };
            string[] colorDyesA = { "Torso_White", "Torso_Black", "Torso_Red", "Torso_Green", "Torso_Blue", "Torso_Yellow", "Torso_Purple" };
            string[] colorDyesH = { "Helm_White", "Helm_Black", "Helm_Red", "Helm_Green", "Helm_Blue", "Helm_Yellow", "Helm_Purple" };
            string[] colorDyesS = { "Shield_White", "Shield_Black", "Shield_Red", "Shield_Green", "Shield_Blue", "Shield_Yellow", "Shield_Purple" };
            string[] colorDyesCode = { "bwht", "blac", "cred", "cgrn", "cblu", "lyel", "lpur" };

            // Filling for weapon rows (7 rows)
            for (int i = 0; i < 7; i++)
            {
                dataRows[i][stateIndex] = colorDyesW[i];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][itemtypeIndex] = "weap";
                dataRows[i][itemtransIndex] = colorDyesCode[i];
                dataRows[i][eolIndex] = "0";
            }

            // Filling for tors rows (7 rows starting from index 7)
            for (int i = 7; i < 14; i++)
            {
                dataRows[i][stateIndex] = colorDyesA[i - 7];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][itemtypeIndex] = "tors";
                dataRows[i][itemtransIndex] = colorDyesCode[i - 7];
                dataRows[i][eolIndex] = "0";
            }

            // Filling for helm rows (7 rows starting from index 14)
            for (int i = 14; i < 21; i++)
            {
                dataRows[i][stateIndex] = colorDyesH[i - 14];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][itemtypeIndex] = "helm";
                dataRows[i][itemtransIndex] = colorDyesCode[i - 14];
                dataRows[i][eolIndex] = "0";
            }

            // Filling for shld rows (7 rows starting from index 21)
            for (int i = 21; i < 28; i++)
            {
                dataRows[i][stateIndex] = colorDyesS[i - 21];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][itemtypeIndex] = "shld";
                dataRows[i][itemtransIndex] = colorDyesCode[i - 21];
                dataRows[i][eolIndex] = "0";
            }



            // Write back to the file
            using (StreamWriter writer = new StreamWriter(statePath, append: false))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
                // Append the new rows to the file
                foreach (var row in dataRows)
                {
                    writer.WriteLine(string.Join("\t", row));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async Task DyesCube()
    {

        string cubePath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/cubemain.txt");
        string cubePath2 = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/cubemain2.txt");

        if (!File.Exists(cubePath))
            await File.WriteAllBytesAsync(cubePath, await Helper.GetResourceByteArray("CASC.cubemain.txt"));

        try
        {
            // Define column indices
            int descriptionIndex = -1, enabledIndex = -1, opIndex = -1, paramIndex = -1, valueIndex = -1,
                inputsIndex = -1, input1Index = -1, input2Index = -1, outputIndex = -1, mod1Index = -1,
                mod1minIndex = -1, mod1maxIndex = -1, mod2Index = -1, mod2paramIndex = -1, mod2minIndex = -1, mod2maxIndex = -1,
                mod3Index = -1, mod3minIndex = -1, mod3maxIndex = -1, mod4Index = -1, mod4minIndex = -1,
                mod4maxIndex = -1, mod5Index = -1, mod5paramIndex = -1, mod5minIndex = -1, mod5maxIndex = -1, eolIndex = -1;

            // Read existing content and determine column indices
            List<string> lines = new List<string>();
            List<string[]> dataRows = new List<string[]>();

            using (StreamReader reader = new StreamReader(cubePath))
            {
                string line;
                bool isFirstRow = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        string[] columns = line.Split('\t');
                        descriptionIndex = Array.IndexOf(columns, "description");
                        enabledIndex = Array.IndexOf(columns, "enabled");
                        opIndex = Array.IndexOf(columns, "op");
                        paramIndex = Array.IndexOf(columns, "param");
                        valueIndex = Array.IndexOf(columns, "value");
                        inputsIndex = Array.IndexOf(columns, "numinputs");
                        input1Index = Array.IndexOf(columns, "input 1");
                        input2Index = Array.IndexOf(columns, "input 2");
                        outputIndex = Array.IndexOf(columns, "output");
                        mod1Index = Array.IndexOf(columns, "mod 1");
                        mod1minIndex = Array.IndexOf(columns, "mod 1 min");
                        mod1maxIndex = Array.IndexOf(columns, "mod 1 max");
                        mod2Index = Array.IndexOf(columns, "mod 2");
                        mod2paramIndex = Array.IndexOf(columns, "mod 2 param");
                        mod2minIndex = Array.IndexOf(columns, "mod 2 min");
                        mod2maxIndex = Array.IndexOf(columns, "mod 2 max");
                        mod3Index = Array.IndexOf(columns, "mod 3");
                        mod3minIndex = Array.IndexOf(columns, "mod 3 min");
                        mod3maxIndex = Array.IndexOf(columns, "mod 3 max");
                        mod4Index = Array.IndexOf(columns, "mod 4");
                        mod4minIndex = Array.IndexOf(columns, "mod 4 min");
                        mod4maxIndex = Array.IndexOf(columns, "mod 4 max");
                        mod5Index = Array.IndexOf(columns, "mod 5");
                        mod5paramIndex = Array.IndexOf(columns, "mod 5 param");
                        mod5minIndex = Array.IndexOf(columns, "mod 5 min");
                        mod5maxIndex = Array.IndexOf(columns, "mod 5 max");
                        eolIndex = Array.IndexOf(columns, "*eol");

                        if (descriptionIndex == -1 || enabledIndex == -1 || opIndex == -1 || paramIndex == -1 ||
                            valueIndex == -1 || inputsIndex == -1 || input1Index == -1 || input2Index == -1 ||
                            outputIndex == -1 || mod1Index == -1 || mod1minIndex == -1 || mod1maxIndex == -1 ||
                            mod2Index == -1 || mod2paramIndex == -1 || mod2minIndex == -1 || mod2maxIndex == -1 || mod3Index == -1 ||
                            mod3minIndex == -1 || mod3maxIndex == -1 || mod4Index == -1 || mod4minIndex == -1 ||
                            mod4maxIndex == -1 || mod5Index == -1 || mod2paramIndex == -1 || mod5minIndex == -1 || mod5maxIndex == -1 ||
                            eolIndex == -1)
                        {
                            throw new Exception("One or more columns not found in the header row.");
                        }

                        isFirstRow = false;
                        lines.Add(line); // Store the header row
                    }
                    else
                    {
                        string[] columns = line.Split('\t');
                        if (descriptionIndex != -1 && columns.Length > descriptionIndex && columns[descriptionIndex] == "Weapon - Normal -> White")
                            return; // Exit the method as no modifications are needed

                        lines.Add(line); // Store existing rows for later
                    }
                }
            }

            string filePath = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt");
            string searchTerm = "ColorDye_Tracker";
            string filePath2 = System.IO.Path.Combine(SelectedModDataFolder, "global/excel/states.txt");
            string searchTerm2 = "Weapon_White";

            int result = SearchItemID(filePath, searchTerm);
            int result2 = SearchStateID(filePath2, searchTerm2);

            // Define the colors and their corresponding codes
            string[] colors0 = { "White", "Black", "Red", "Green", "Blue", "Yellow", "Purple" };
            string[] colors1 = { "Black", "Red", "Green", "Blue", "Yellow", "Purple", "Normal" };
            string[] colors2 = { "Red", "Green", "Blue", "Yellow", "Purple", "White", "Normal" };
            string[] colors3 = { "Green", "Blue", "Yellow", "Purple", "White", "Black", "Normal"};
            string[] colors4 = { "Blue", "Yellow", "Purple", "White", "Black", "Red", "Normal" };
            string[] colors5 = { "Yellow", "Purple", "White", "Black", "Red", "Green", "Normal", };
            string[] colors6 = { "Purple", "White", "Black", "Red", "Green", "Blue", "Normal" };
            string[] colors7 = { "White", "Black", "Red", "Green", "Blue", "Yellow", "Normal" };
            string[] gems0 = { "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "gpv,qty=3" };
            string[] gems1 = { "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "gpv,qty=3", "yps,qty=3" };
            string[] gems2 = { "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "gpv,qty=3", "gpw,qty=3", "yps,qty=3" };
            string[] gems3 = { "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "gpv,qty=3", "gpw,qty=3", "skz,qty=3", "yps,qty=3" };
            string[] gems4 = { "gpy,qty=3", "gpv,qty=3", "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "yps,qty=3" };
            string[] gems5 = { "gpv,qty=3", "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "yps,qty=3" };
            string[] gems6 = { "gpv,qty=3", "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "yps,qty=3" };
            string[] gems7 = { "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "yps,qty=3" };
            string[] value = { "1", "2", "3", "4", "5", "6", "7" };
            string[] trackerValue0 = { "1", "2", "3", "4", "5", "6", "7" };
            string[] trackerValue1 = { "1", "2", "3", "4", "5", "6", "-1" };
            string[] trackerValue2 = { "1", "2", "3", "4", "5", "-1", "-2" };
            string[] trackerValue3 = { "1", "2", "3", "4", "-2", "-1", "-3" };
            string[] trackerValue4 = { "1", "2", "3", "-3", "-2", "-1", "-4" };
            string[] trackerValue5 = { "1", "2", "-4", "-3", "-2", "-1", "-5" };
            string[] trackerValue6 = { "1", "-5", "-4", "-3", "-2", "-1", "-6" };
            string[] trackerValue7 = { "-6", "-5", "-4", "-3", "-2", "-1", "-7" };
            int[] stateValue0 = { result2, result2 + 1, result2 + 2, result2 + 3, result2 + 4, result2 + 5, result2 + 6 };
            int[] stateValue1 = { result2 + 1, result2 + 2, result2 + 3, result2 + 4, result2 + 5, result2 + 6, result2 };
            int[] stateValue2 = { result2 + 2, result2 + 3, result2 + 4, result2 + 5, result2 + 6, result2, result2 + 1 };
            int[] stateValue3 = { result2 + 3, result2 + 4, result2 + 5, result2 + 6, result2, result2 + 1, result2 + 2 };
            int[] stateValue4 = { result2 + 4, result2 + 5, result2 + 6, result2, result2 + 1, result2 + 2, result2 + 3 };
            int[] stateValue5 = { result2 + 5, result2 + 6, result2, result2 + 1, result2 + 2, result2 + 3, result2 + 4 };
            int[] stateValue6 = { result2 + 6, result2, result2 + 1, result2 + 2, result2 + 3, result2 + 4, result2 + 5 };
            string[] colorDyeProps0 = { "CD_White", "CD_Black", "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple" };
            string[] colorDyeProps1 = { "CD_Black", "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple", "CD_White" };
            string[] colorDyeProps2 = { "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple", "CD_White", "CD_Black" };
            string[] colorDyeProps3 = { "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple", "CD_White", "CD_Black", "CD_Red" };
            string[] colorDyeProps4 = { "CD_Blue", "CD_Yellow", "CD_Purple", "CD_White", "CD_Black", "CD_Red", "CD_Green" };
            string[] colorDyeProps5 = { "CD_Yellow", "CD_Purple", "CD_White", "CD_Black", "CD_Red", "CD_Green", "CD_Blue" };
            string[] colorDyeProps6 = { "CD_Purple", "CD_White", "CD_Black", "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow" };

            // Define the item types
            string[] itemTypesCode = { "weap", "tors", "helm", "shld" };
            string[] colors = null;
            string[] gems = null;
            string[] colorDyeProps = null;
            string colorDyePropsR = "";
            int iscValue = 0;
            int[] stateValue = null;
            int stateRValue = 0;
            string[] trackerValue = null;


            // Add new rows for each item type
            for (int i = 0; i < 32; i++)
            {
                
                if (i == 0 || i == 8 || i == 16 || i == 24)
                {
                    colors = colors0;
                    gems = gems0;
                    colorDyeProps = colorDyeProps0;
                    colorDyePropsR = "";
                    iscValue = result;
                    stateValue = stateValue0;
                    trackerValue = trackerValue0;
                }
                else if (i == 1 || i == 9 || i == 17 || i == 25)
                {
                    colors = colors1;
                    gems = gems1;
                    colorDyeProps = colorDyeProps1;
                    colorDyePropsR = "CD_White";
                    iscValue = result + 1;
                    stateValue = stateValue1;
                    stateRValue = result2;
                    trackerValue = trackerValue1;
                }
                else if (i == 2 || i == 10 || i == 18 || i == 26)
                {
                    colors = colors2;
                    gems = gems2;
                    colorDyeProps = colorDyeProps2;
                    colorDyePropsR = "CD_Black";
                    iscValue = result + 2;
                    stateValue = stateValue2;
                    stateRValue = result2 + 1;
                    trackerValue = trackerValue2;
                }
                else if (i == 3 || i == 11 || i == 19 || i == 27)
                {
                    colors = colors3;
                    gems = gems3;
                    colorDyeProps = colorDyeProps3;
                    colorDyePropsR = "CD_Red";
                    iscValue = result + 3;
                    stateValue = stateValue3;
                    stateRValue = result2 + 2;
                    trackerValue = trackerValue3;
                }
                else if (i == 4 || i == 12 || i == 20 || i == 28)
                {
                    colors = colors4;
                    gems = gems4;
                    colorDyeProps = colorDyeProps4;
                    colorDyePropsR = "CD_Green";
                    iscValue = result + 4;
                    stateValue = stateValue4;
                    stateRValue = result2 + 3;
                    trackerValue = trackerValue4;
                }
                else if (i == 5 || i == 13 || i == 21 || i == 29)
                {
                    colors = colors5;
                    gems = gems5;
                    colorDyeProps = colorDyeProps5;
                    colorDyePropsR = "CD_Blue";
                    iscValue = result + 5;
                    stateValue = stateValue5;
                    stateRValue = result2 + 4;
                    trackerValue = trackerValue5;
                }
                else if (i == 6 || i == 14 || i == 22 || i == 30)
                {
                    colors = colors6;
                    gems = gems6;
                    colorDyeProps = colorDyeProps6;
                    colorDyePropsR = "CD_Yellow";
                    iscValue = result + 6;
                    stateValue = stateValue6;
                    stateRValue = result2 + 5;
                    trackerValue = trackerValue6;
                }
                else if (i == 7 || i == 15 || i == 23 || i == 31)
                {
                    colors = colors7;
                    gems = gems7;
                    colorDyeProps = colorDyeProps0;
                    colorDyePropsR = "CD_Purple";
                    iscValue = result + 7;
                    stateValue = stateValue0;
                    stateRValue = result2 + 6;
                    trackerValue = trackerValue7;
                }
                else
                {
                    // Handle the case where i is out of range
                }

                for (int j = 0; j < colors.Length; j++)
                {
                    int maxIndex = Math.Max(descriptionIndex, Math.Max(enabledIndex, Math.Max(opIndex, Math.Max(paramIndex, Math.Max(valueIndex,
                        Math.Max(inputsIndex, Math.Max(input1Index, Math.Max(input2Index, Math.Max(outputIndex,
                        Math.Max(mod1Index, Math.Max(mod1minIndex, Math.Max(mod1maxIndex, Math.Max(mod2Index,
                        Math.Max(mod2paramIndex, Math.Max(mod2minIndex, Math.Max(mod2maxIndex, Math.Max(mod3Index, Math.Max(mod3minIndex,
                        Math.Max(mod3maxIndex, Math.Max(mod4Index, Math.Max(mod4minIndex, Math.Max(mod4maxIndex,
                        Math.Max(mod5Index, Math.Max(mod5paramIndex, Math.Max(mod5minIndex, Math.Max(mod5maxIndex,eolIndex))))))))))))))))))))))))));

                    string[] newRow = new string[maxIndex + 1];

                    if (i == 0)
                        newRow[descriptionIndex] = "Weapon - Normal -> " + colors[j];
                    else if (i == 1)
                        newRow[descriptionIndex] = "Weapon - White -> " + colors[j];
                    else if (i == 2)
                        newRow[descriptionIndex] = "Weapon - Black -> " + colors[j];
                    else if (i == 3)
                        newRow[descriptionIndex] = "Weapon - Red -> " + colors[j];
                    else if (i == 4)
                        newRow[descriptionIndex] = "Weapon - Green -> " + colors[j];
                    else if (i == 5)
                        newRow[descriptionIndex] = "Weapon - Blue -> " + colors[j];
                    else if (i == 6)
                        newRow[descriptionIndex] = "Weapon - Yellow -> " + colors[j];
                    else if (i == 7)
                        newRow[descriptionIndex] = "Weapon - Purple -> " + colors[j];
                    else if (i == 8)
                        newRow[descriptionIndex] = "Torso - Normal -> " + colors[j];
                    else if (i == 9)
                        newRow[descriptionIndex] = "Torso - White -> " + colors[j];
                    else if (i == 10)
                        newRow[descriptionIndex] = "Torso - Black -> " + colors[j];
                    else if (i == 11)
                        newRow[descriptionIndex] = "Torso - Red -> " + colors[j];
                    else if (i == 12)
                        newRow[descriptionIndex] = "Torso - Green -> " + colors[j];
                    else if (i == 13)
                        newRow[descriptionIndex] = "Torso - Blue -> " + colors[j];
                    else if (i == 14)
                        newRow[descriptionIndex] = "Torso - Yellow -> " + colors[j];
                    else if (i == 15)
                        newRow[descriptionIndex] = "Torso - Purple -> " + colors[j];
                    else if (i == 16)
                        newRow[descriptionIndex] = "Helm - Normal -> " + colors[j];
                    else if (i == 17)
                        newRow[descriptionIndex] = "Helm - White -> " + colors[j];
                    else if (i == 18)
                        newRow[descriptionIndex] = "Helm - Black -> " + colors[j];
                    else if (i == 19)
                        newRow[descriptionIndex] = "Helm - Red -> " + colors[j];
                    else if (i == 20)
                        newRow[descriptionIndex] = "Helm - Green -> " + colors[j];
                    else if (i == 21)
                        newRow[descriptionIndex] = "Helm - Blue -> " + colors[j];
                    else if (i == 22)
                        newRow[descriptionIndex] = "Helm - Yellow -> " + colors[j];
                    else if (i == 23)
                        newRow[descriptionIndex] = "Helm - Purple -> " + colors[j];
                    else if (i == 24)
                        newRow[descriptionIndex] = "Shield - Normal -> " + colors[j];
                    else if (i == 25)
                        newRow[descriptionIndex] = "Shield - White -> " + colors[j];
                    else if (i == 26)
                        newRow[descriptionIndex] = "Shield - Black -> " + colors[j];
                    else if (i == 27)
                        newRow[descriptionIndex] = "Shield - Red -> " + colors[j];
                    else if (i == 28)
                        newRow[descriptionIndex] = "Shield - Green -> " + colors[j];
                    else if (i == 29)
                        newRow[descriptionIndex] = "Shield - Blue -> " + colors[j];
                    else if (i == 30)
                        newRow[descriptionIndex] = "Shield - Yellow -> " + colors[j];
                    else if (i == 31)
                        newRow[descriptionIndex] = "Shield - Purple -> " + colors[j];

                    newRow[enabledIndex] = "1";
                    newRow[opIndex] = "18";
                    newRow[paramIndex] = result.ToString();
                    newRow[valueIndex] = (i%8).ToString();
                    newRow[inputsIndex] = "4";
                    newRow[input1Index] = (i < 8) ? "weap,any" : (i < 16) ? "tors,any" : (i < 24) ? "helm,any" : "shld,any";
                    newRow[input2Index] = gems[j];
                    newRow[outputIndex] = "useitem";
                    newRow[mod1Index] = (i > 0 && (j % 7) == 6) ? "" : colorDyeProps[j];
                    newRow[mod1minIndex] = (i > 0 && (j % 7) == 6) ? "" : "1";
                    newRow[mod1maxIndex] = (i > 0 && (j % 7) == 6) ? "" : "1";
                    newRow[mod2Index] = (i > 0 && (j % 7) == 6) ? "" : "state";
                    newRow[mod2paramIndex] = (i > 0 && (j % 7) == 6) ? "" : (stateValue[j] + (7*(i/8))).ToString();
                    newRow[mod2minIndex] = (i > 0 && (j % 7) == 6) ? "" : "1";
                    newRow[mod2maxIndex] = (i > 0 && (j % 7) == 6) ? "" : "1";
                    newRow[mod3Index] = "CD_Tracker";
                    newRow[mod3minIndex] = trackerValue[j];
                    newRow[mod3maxIndex] = trackerValue[j];
                    newRow[mod4Index] = colorDyePropsR;
                    newRow[mod4minIndex] = (i == 0) ? "" : "-1";
                    newRow[mod4maxIndex] = (i == 0) ? "" : "-1";
                    newRow[mod5Index] = (i == 0) ? "" : "state";
                    newRow[mod5paramIndex] = (i == 0) ? "" : (i > 0 && (i % 8) == 0) ? "" : ((7 * (i / 8)) + (result2 + (i % 8))-1).ToString();
                    newRow[mod5minIndex] = (i == 0) ? "" : "-1";
                    newRow[mod5maxIndex] = (i == 0) ? "" : "-1";
                    newRow[eolIndex] = "0";

                    dataRows.Add(newRow);
                }
            }


            // Write back to the file
            using (StreamWriter writer = new StreamWriter(cubePath, append: false))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }

                // Append the new rows to the file
                foreach (var row in dataRows)
                {
                    writer.WriteLine(string.Join("\t", row));
                }
            }
            //await DyesCube_Torso();
        }

        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private int SearchItemID(string filePath, string searchTerm)
    {
        int result = -1; // Default result if entry not found
        int statColumnIndex = -1; // Index of the "Stat" column
        int idColumnIndex = -1; // Index of the "*ID" column

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the header row to determine column indexes
                string[] headers = reader.ReadLine().Split('\t');
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] == "Stat")
                        statColumnIndex = i;
                    else if (headers[i] == "*ID")
                        idColumnIndex = i;
                }

                if (statColumnIndex == -1 || idColumnIndex == -1)
                {
                    MessageBox.Show("Column headers not found in the file.");
                    return result;
                }

                // Search for the desired entry
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] columns = line.Split('\t');
                    if (columns.Length > statColumnIndex && columns[statColumnIndex] == searchTerm)
                    {
                        if (int.TryParse(columns[idColumnIndex], out result))
                            return result; // Return the *ID as an integer
                        else
                        {
                            MessageBox.Show("Unable to parse *ID as an integer.");
                            return -1; // Return -1 indicating failure
                        }
                    }
                }

                MessageBox.Show($"No entry with '{searchTerm}' found in the Stat column.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }

        return result;
    }

    private int SearchStateID(string filePath, string searchTerm)
    {
        int result2 = -1; // Default result if entry not found
        int stateColumnIndex = -1; // Index of the "Stat" column
        int idColumnIndex = -1; // Index of the "*ID" column

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the header row to determine column indexes
                string[] headers = reader.ReadLine().Split('\t');
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] == "state")
                        stateColumnIndex = i;
                    else if (headers[i] == "*ID")
                        idColumnIndex = i;
                }

                if (stateColumnIndex == -1 || idColumnIndex == -1)
                {
                    MessageBox.Show("Column headers not found in the file.");
                    return result2;
                }

                // Search for the desired entry
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] columns = line.Split('\t');
                    if (columns.Length > stateColumnIndex && columns[stateColumnIndex] == searchTerm)
                    {
                        if (int.TryParse(columns[idColumnIndex], out result2))
                            return result2; // Return the *ID as an integer
                        else
                        {
                            MessageBox.Show("Unable to parse *ID as an integer.");
                            return -1; // Return -1 indicating failure
                        }
                    }
                }

                MessageBox.Show($"No entry with '{searchTerm}' found in the Stat column.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }

        return result2;
    }

    private void RemoveColorDyes(string filePath, string searchString, int rowsToDelete)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            // Initialize variables to keep track of deletion process
            bool foundSearchString = false;
            int deleteCounter = 0;
            int totalDeleted = 0;

            // Create a new list to store lines to keep
            var linesToKeep = lines.Where(line =>
            {
                if (foundSearchString && deleteCounter < rowsToDelete)
                {
                    deleteCounter++;
                    totalDeleted++;
                    return false; // Exclude this line
                }
                else if (line.StartsWith(searchString))
                {
                    foundSearchString = true;
                    deleteCounter = 0; // Reset delete counter for each match
                    totalDeleted++;
                    return false; // Exclude this line
                }
                return true; // Include this line
            }).ToList();

            // Rewrite the file if any lines were deleted
            if (totalDeleted > 0)
                File.WriteAllLines(filePath, linesToKeep);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }
    #endregion

    #region ---Cinematic Subs---

    private async Task ConfigureCinematicSubs()
    {
        eCinematicSubs cinematicSubs = (eCinematicSubs)UserSettings.CinematicSubs;
        string srtPath = System.IO.Path.Combine(SelectedModDataFolder, "local/lng/subtitles");
        string profilePath = System.IO.Path.Combine(SelectedModDataFolder, "global/ui/layouts");

        var languagePaths = new Dictionary<int, string>
    {
        { 0, "enus" }, { 1, "dede" }, { 2, "eses" }, { 3, "esmx" }, { 4, "frfr" }, { 5, "itit" }, { 6, "jajp" },
        { 7, "kokr" }, { 8, "plpl" }, { 9, "ptbr" }, { 10, "ruru" }, { 11, "zhcn" }, { 12, "zhtw" }
    };

        var filesToExtract = new List<string>
    {
        "act02start.srt", "act03start.srt", "act04end.srt", "act04start.srt", "d2intro.srt", "d2x_intro.srt"
    };

        switch (cinematicSubs)
        {
            case eCinematicSubs.Disabled:
                {
                    if (Directory.Exists(srtPath))
                        Directory.Delete(srtPath, true);

                    if (File.Exists(System.IO.Path.Combine(profilePath, "_profilehd.json")))
                    {
                        string profileContents = File.ReadAllText(System.IO.Path.Combine(profilePath, "_profilehd.json"));
                        string defaultValue = @"""anchor"": { ""x"": 0.5, ""y"": 0.95 }";
                        string bottomValue = @"""anchor"": { ""x"": 0.5, ""y"": 0.8 }";
                        string updatedProfileContents = profileContents.Replace(defaultValue, bottomValue);

                        File.WriteAllText(System.IO.Path.Combine(profilePath, "_profilehd.json"), updatedProfileContents);
                    }

                    break;
                }
            case eCinematicSubs.Enabled:
                {
                    if (!Directory.Exists(srtPath))
                        Directory.CreateDirectory(srtPath);
                    if (!Directory.Exists(profilePath))
                        Directory.CreateDirectory(profilePath);

                    if (!File.Exists(System.IO.Path.Combine(profilePath, "_profilehd.json")))
                        await File.WriteAllBytesAsync(System.IO.Path.Combine(profilePath, "_profilehd.json"), await Helper.GetResourceByteArray("CASC.profilehd.json"));

                    int selectedLanguage = UserSettings.TextLanguage;

                    if (languagePaths.TryGetValue(selectedLanguage, out var languagePath))
                    {
                        string srtFilePath = System.IO.Path.Combine(srtPath, languagePath, "act02start.srt");

                        if (!File.Exists(srtFilePath))
                        {
                            foreach (var file in filesToExtract)
                            {
                                string sourceFileName = $"CASC.Subtitles.{languagePath}.{file}";
                                string destinationFilePath = System.IO.Path.Combine(srtPath, languagePath, file);
                                string languageDirectoryPath = System.IO.Path.Combine(srtPath, languagePath);

                                Directory.CreateDirectory(languageDirectoryPath);
                                byte[] resourceData = await Helper.GetResourceByteArray(sourceFileName);
                                await File.WriteAllBytesAsync(destinationFilePath, resourceData);
                            }
                        }

                        string profileJsonPath = System.IO.Path.Combine(profilePath, "_profilehd.json");
                        if (File.Exists(profileJsonPath))
                        {
                            string profileContents = File.ReadAllText(profileJsonPath);
                            string defaultValue = @"""anchor"": { ""x"": 0.5, ""y"": 0.8 }";
                            string bottomValue = @"""anchor"": { ""x"": 0.5, ""y"": 0.95 }";
                            string updatedProfileContents = profileContents.Replace(defaultValue, bottomValue);

                            File.WriteAllText(profileJsonPath, updatedProfileContents);
                        }

                        // Convert SDH to standard subtitles
                        ConvertSDHToStandard(System.IO.Path.Combine(srtPath, languagePath));
                    }


                    break;
                }
        }
    }
    private void ConvertSDHToStandard(string folderPath)
    {
        var sdhRegex = new Regex(@"\[\s*(?!Marius\s*]|Tyrael\s*]|Mephisto\s*])[^]]*\]");
        var patternRegex = new Regex(@"^\s*\d+\s*$\r?\n^\d{2}:\d{2}:\d{2},\d{3} --> \d{2}:\d{2}:\d{2},\d{3}\r?\n(?:^\s*$\r?)+", RegexOptions.Multiline);

        foreach (var file in Directory.GetFiles(folderPath, "*.srt"))
        {
            string fileContent = File.ReadAllText(file);

            // Replace SDH entries
            string updatedContent = sdhRegex.Replace(fileContent, "");

            // Remove now blank entries
            updatedContent = patternRegex.Replace(updatedContent, "");

            // Renumber IDs sequentially starting from 1
            updatedContent = RenumberIds(updatedContent);

            // Ensure there is never more than one blank line between entries
            updatedContent = NormalizeBlankLines(updatedContent);

            // Write back the updated content to the file
            File.WriteAllText(file, updatedContent);
        }
    }
    private string RenumberIds(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var lineNumber = 1;
        var updatedLines = new List<string>();

        foreach (var line in lines)
        {
            if (Regex.IsMatch(line, @"^\s*\d+\s*$"))
            {
                updatedLines.Add(lineNumber.ToString());
                lineNumber++;
            }
            else
            {
                updatedLines.Add(line);
            }
        }

        return string.Join(Environment.NewLine, updatedLines);
    }
    private string NormalizeBlankLines(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var updatedLines = new List<string>();

        bool previousLineWasEmpty = false;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (!previousLineWasEmpty)
                {
                    updatedLines.Add(line);
                    previousLineWasEmpty = true;
                }
            }
            else
            {
                updatedLines.Add(line);
                previousLineWasEmpty = false;
            }
        }

        return string.Join(Environment.NewLine, updatedLines);
    }
    public class SubtitleExtractor
    {
        private static readonly List<string> Languages = new List<string>
    {
        "dede", "enus", "eses", "esmx", "frfr", "itit", "jajp", "kokr", "plpl", "ptbr", "ruru", "zhcn", "zhtw"
    };

        private static readonly List<string> Files = new List<string>
    {
        "act02start.srt", "act03start.srt", "act04end.srt", "act04start.srt", "d2intro.srt", "d2x_intro.srt"
    };

        public static void ExtractAllSubtitles(string gamePath, string selectedModDataFolder)
        {
            foreach (var language in Languages)
            {
                foreach (var file in Files)
                {
                    string filePath = $@"data:data\local\lng\subtitles\{language}\{file}";
                    File.WriteAllBytes(filePath, Helper.GetResourceByteArray2($@"CASC.{language}.{file}"));
                }
            }
        }
    }

    #endregion

    #region ---Auto Backups---

    public async Task StartAutoBackup() //Determine Auto-Backups status and enable timer
    {
        if (UserSettings == null)
        {
            MessageBox.Show("Auto Backup was not started. Could not find User Settings!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _autoBackupDispatcherTimer?.Stop();

        if ((eBackup)UserSettings.AutoBackups == eBackup.Disabled)
            return;

        if (_autoBackupDispatcherTimer == null)
        {
            _autoBackupDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            _autoBackupDispatcherTimer.Tick += async (sender, args) =>
            {
                _logger.Info("Auto backup timer ticked.");
                await BackupRecentCharacter();
            };
        }

        //Auto-Backup Timer Intervals
        switch ((eBackup)UserSettings.AutoBackups)
        {
            case eBackup.FiveMinutes:
                {
                    _autoBackupDispatcherTimer.Interval = TimeSpan.FromMinutes(5);
                    break;
                }
            case eBackup.FifteenMinutes:
                {
                    _autoBackupDispatcherTimer.Interval = TimeSpan.FromMinutes(15);
                    break;
                }
            case eBackup.ThirtyMinutes:
                {
                    _autoBackupDispatcherTimer.Interval = TimeSpan.FromMinutes(30);
                    break;
                }
            case eBackup.OneHour:
                {
                    _autoBackupDispatcherTimer.Interval = TimeSpan.FromMinutes(60);
                    break;
                }
        }

        _autoBackupDispatcherTimer.Start();
    }
    public async Task<(string characterName, bool passed)> BackupRecentCharacter()
    {
        string mostRecentCharacterName = null;
        string baseSavePath = GetSavePath();
        string actualSaveFilePath;
        string actualBackupFolder;

        try
        {
            // Determine if the mod is using a mod folder or retail folder for backups
            if (!Directory.Exists(Path.Combine(baseSavePath, @$"Diablo II Resurrected\Mods\{Settings.Default.SelectedMod}")))
            {
                // Retail location
                actualSaveFilePath = BaseSaveFilesFilePath;
                actualBackupFolder = Path.Combine(BaseSaveFilesFilePath, "Backups");
            }
            else
            {
                // Mod folder location
                actualSaveFilePath = SaveFilesFilePath;
                actualBackupFolder = BackupFolder;
            }

            _logger.Error($"BackupRecentCharacter using save path: {actualSaveFilePath}, backup folder: {actualBackupFolder}");

            // Create backup folder if it doesn't exist
            if (!Directory.Exists(actualBackupFolder))
                Directory.CreateDirectory(actualBackupFolder);

            await Task.Delay(100);

            var saveFiles = new DirectoryInfo(actualSaveFilePath).GetFiles("*.d2s");
            if (saveFiles.Length >= 1)
            {
                FileInfo mostRecentCharacterFile = saveFiles.OrderByDescending(o => o.LastWriteTimeUtc).First();
                mostRecentCharacterName = Path.GetFileNameWithoutExtension(mostRecentCharacterFile.Name);

                string mostRecentCharacterBackupFolder = Path.Combine(actualBackupFolder, mostRecentCharacterName);
                if (!Directory.Exists(mostRecentCharacterBackupFolder))
                    Directory.CreateDirectory(mostRecentCharacterBackupFolder);

                // Get latest backup file
                var backupFiles = new DirectoryInfo(mostRecentCharacterBackupFolder).GetFiles($"{mostRecentCharacterFile.Name}_*.d2s");
                if (backupFiles.Length > 0)
                {
                    FileInfo latestBackupFile = backupFiles.OrderByDescending(f => f.LastWriteTimeUtc).First();

                    // Compare hashes to check if the file has changed
                    if (ComputeMD5(mostRecentCharacterFile.FullName) == ComputeMD5(latestBackupFile.FullName))
                    {
                        _logger.Error($"Auto Backups: Skipped backup for {mostRecentCharacterFile.Name}, no changes detected.");
                        return (mostRecentCharacterName, true);
                    }
                }

                string backupFilePath = Path.Combine(mostRecentCharacterBackupFolder, mostRecentCharacterFile.Name + DateTime.Now.ToString("_MM_dd--hh_mmtt") + ".d2s");
                File.Copy(mostRecentCharacterFile.FullName, backupFilePath, true);
                _logger.Error($"Auto Backups: Backed up {mostRecentCharacterFile.Name} at {DateTime.Now.ToString("_MM_dd--hh_mmtt")} in {mostRecentCharacterBackupFolder}");

                /* Disabled for now, as default size has been set to 32KB

                // Display Size Limit Warning (55% or more)
                long fileSizeInBytes = mostRecentCharacterFile.Length;
                if (fileSizeInBytes >= 7000)
                {
                    MessageBox.Show(
                        $"WARNING!\nYour current save file size is {fileSizeInBytes} / 8192 bytes.\n\n" +
                        "If you exceed the max size, you will start losing items!\n" +
                        "It is highly recommended that you place smaller items in your shared stash to avoid this.\n\n" +
                        "All items in your personal stash, cube and inventory contribute to this size",
                        "Attention!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                */

                // Backup Stash (Only if changed)
                BackupStashFile(actualSaveFilePath, actualBackupFolder, "SharedStashSoftCoreV2.d2i");
                BackupStashFile(actualSaveFilePath, actualBackupFolder, "SharedStashHardCoreV2.d2i");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return (null, false);
        }

        return (mostRecentCharacterName, true);
    }
    private string ComputeMD5(string filePath)
    {
        using (var md5 = MD5.Create())
        using (var stream = File.OpenRead(filePath))
        {
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }
    }
    private void BackupStashFile(string savePath, string backupFolder, string stashFileName)
    {
        string stashFilePath = Path.Combine(savePath, stashFileName);
        if (!File.Exists(stashFilePath))
            return;

        string stashBackupFolder = Path.Combine(backupFolder, "Stash");
        if (!Directory.Exists(stashBackupFolder))
            Directory.CreateDirectory(stashBackupFolder);

        // Get latest backup file
        var backupFiles = new DirectoryInfo(stashBackupFolder).GetFiles($"{stashFileName}_*.d2i");
        if (backupFiles.Length > 0)
        {
            FileInfo latestBackupFile = backupFiles.OrderByDescending(f => f.LastWriteTimeUtc).First();

            // Compare hashes
            if (ComputeMD5(stashFilePath) == ComputeMD5(latestBackupFile.FullName))
            {
                _logger.Error($"Auto Backups: Skipped stash backup for {stashFileName}, no changes detected.");
                return;
            }
        }

        string backupFilePath = Path.Combine(stashBackupFolder, stashFileName + DateTime.Now.ToString("_MM_dd--hh_mmtt") + ".d2i");
        File.Copy(stashFilePath, backupFilePath, true);
        _logger.Error($"Auto Backups: Backed up {stashFileName} at {DateTime.Now.ToString("_MM_dd--hh_mmtt")} in {stashBackupFolder}");
    }

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

    #endregion

    #endregion

    #region ---Game Path, BNET and User Settings---

    private async Task<string> GetDiabloInstallPath() //Attempt to find D2R Install Path as defined by Blizzard
    {
        //Check Primary Blizzard path location entry
        RegistryKey gameKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Diablo II Resurrected");
        string installLocation = gameKey?.GetValue("InstallLocation")?.ToString();

        if (installLocation != null)
            return installLocation;

        //Perform an exhaustive search of D2R.exe in Secondary Blizzard path location entry
        using RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
        using RegistryKey regKey = baseKey.OpenSubKey(@"System\GameConfigStore\Children");

        if (regKey == null)
            return null;

        string[] subKeyNames = regKey.GetSubKeyNames();
        List<string> results = new();

        foreach (string subKeyName in subKeyNames)
        {
            using RegistryKey subKey = regKey.OpenSubKey(subKeyName);

            if (subKey == null)
                continue;

            string exeFullPath = subKey.GetValue("MatchedExeFullPath")?.ToString();

            if (string.IsNullOrEmpty(exeFullPath))
                continue;

            if (exeFullPath.Contains("D2R.exe"))
                results.Add(exeFullPath);
        }

        //Either use parsed result as GamePath or inform user of multiple installs found; possible game migration issue
        switch (results.Count)
        {
            case 1:
                return results[0].Replace(@"\D2R.exe", "");
            case >= 2:
                MessageBox.Show("If you experience mod loading issues, please contact Bonesy in Discord", "Multiple Install Locations found!");
                break;
        }

        return null;
    }
    public void DisableBNetConnection()
    {
        try
        {
            using (var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                                        .OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\BNA", writable: true))
            {
                key.SetValue("CONNECTION_STRING_CN", "127.0.0.1");
                key.SetValue("CONNECTION_STRING_CXX", "127.0.0.1");
                key.SetValue("CONNECTION_STRING_EU", "127.0.0.1");
                key.SetValue("CONNECTION_STRING_KR", "127.0.0.1");
                key.SetValue("CONNECTION_STRING_US", "127.0.0.1");
                key.SetValue("CONNECTION_STRING_XX", "127.0.0.1");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
    public async Task SaveUserSettings()
    {
        //Protected
        if (Directory.Exists(SelectedModDataFolder))
            await File.WriteAllTextAsync(SelectedUserSettingsFilePath, JsonConvert.SerializeObject(UserSettings));
        //Unprotected
        else
        {
            if (ModInfo != null)
                await File.WriteAllTextAsync(SelectedUserSettingsFilePath, JsonConvert.SerializeObject(UserSettings).Replace($"{Settings.Default.SelectedMod}.mpq/", ""));
        }
            
    }

    #endregion

    #region ---Save File Functions---

    private async Task RenameCharacter() //Function used to change in-game character name
    {
        //TODO: This does not account for character that have been backed up. This should also have its own dedicated dialog
        OpenFileDialog ofd = new OpenFileDialog();
        {
            ofd.InitialDirectory = SaveFilesFilePath;
            ofd.Filter = "D2R Character Files (*.d2s)|*.d2s";
        };
        SaveFileDialog sfd = new SaveFileDialog();
        {
            sfd.InitialDirectory = SaveFilesFilePath;
            sfd.DefaultExt = ".d2s";
        };

        //Extract raw byte data from specified save file
        ofd.ShowDialog();
        string fileSource = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);
        byte[] ba = Encoding.Default.GetBytes(fileSource);
        byte[] bytes = null;
        if (ofd.FileName != "")
        {
            bytes = await File.ReadAllBytesAsync(ofd.FileName);
            MessageBox.Show("Please choose your new save filename");

            sfd.ShowDialog();
            await File.WriteAllBytesAsync(sfd.FileName, bytes);

            //Define and string replace byte code for save file (ugly solution)
            string fileSource2 = System.IO.Path.GetFileNameWithoutExtension(sfd.FileName);
            byte[] ba2 = Encoding.Default.GetBytes(fileSource2);
            string hexString = BitConverter.ToString(ba).Replace("-", string.Empty);
            string hexString2 = BitConverter.ToString(ba2).Replace("-", string.Empty);


            if (fileSource.Length != fileSource2.Length)
            {
                if (fileSource.Length > fileSource2.Length)
                    hexString2 = hexString2 + String.Concat(Enumerable.Repeat("00", fileSource.Length - fileSource2.Length));
                else
                    hexString = hexString + String.Concat(Enumerable.Repeat("00", fileSource2.Length - fileSource.Length));
            }

            //Write and convert byte array to byte data back to save file
            string bitString = BitConverter.ToString(bytes).Replace("-", string.Empty).Replace(hexString, hexString2);
            await File.WriteAllBytesAsync(sfd.FileName, Helper.StringToByteArray(bitString));

            byte[] bytes3 = await File.ReadAllBytesAsync(sfd.FileName);
            FixChecksum(bytes3);
            await File.WriteAllBytesAsync(sfd.FileName, bytes3);

            if (ModInfo.Name == "My Custom Mod")
            {
                File.Move(sfd.FileName, System.IO.Path.Combine(SaveFilesFilePath, "placeholder"));

                foreach (var file in Directory.GetFiles(SaveFilesFilePath, fileSource + "*", SearchOption.TopDirectoryOnly))
                    File.Move(file, file.Replace(fileSource, fileSource2));
                File.Delete(sfd.FileName);
                File.Move(System.IO.Path.Combine(SaveFilesFilePath, "placeholder"), sfd.FileName);
            }
            MessageBox.Show("Character renamed successfully!");
        }
    }
    public static int ParseD2SFile(string filePath)
    {
        // Ensure the file exists before attempting to read it
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file at {filePath} was not found.");

        // Read the file as a byte array
        byte[] fileData = File.ReadAllBytes(filePath);

        // Ensure the file is long enough to contain the data we need
        if (fileData.Length < 175) // 171 + 4 bytes = 175 bytes minimum
            throw new InvalidDataException("The file is too short to contain the required data.");

        // Read 4 bytes starting at byte 171 (0-based index)
        int startIndex = 171;
        byte[] data = new byte[4];
        Array.Copy(fileData, startIndex, data, 0, 4);

        // Convert the 4 bytes to an integer
        int result = BitConverter.ToInt32(data, 0);

        // Output or return the result
        return result;
    }
    private void FixChecksum(byte[] bytes) //Used in RenameCharacter()
    {
        //Update save file checksum data to match edited content
        new byte[4].CopyTo(bytes, 0xc);
        int checksum = 0;

        for (int i = 0; i < bytes.Length; i++)
        {
            checksum = bytes[i] + (checksum * 2) + (checksum < 0 ? 1 : 0);
        }
        MessageBox.Show(BitConverter.ToString(BitConverter.GetBytes(checksum)));
        BitConverter.GetBytes(checksum).CopyTo(bytes, 0xc);
    }

    #endregion

    #region ---Update Functions---

    private async Task CheckForLauncherUpdates() //Performed after loading to check for D2RLAN upgrades
    {
        WebClient webClient = new();

        if (File.Exists(@"..\MyVersions_Temp.txt"))
            File.Delete(@"..\MyVersions_Temp.txt");

        //Download the most recent version info file to compare values
        if (!File.Exists(@"..\MyVersions_Temp.txt"))
        {
            string primaryLink = "https://drive.google.com/uc?export=download&id=1c6KaTa4V782rVX0jEa8I5HLxYdrhvl7q";
            string backupLink = "https://d2filesdrop.s3.us-east-2.amazonaws.com/MyVersions-TCP.txt";

            try
            {
                webClient.DownloadFile(primaryLink, @"..\MyVersions_Temp.txt");
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response && ((int)response.StatusCode == 429 || (int)response.StatusCode == 500))
                {
                    try
                    {
                        webClient.DownloadFile(backupLink, @"..\MyVersions_Temp.txt");
                    }
                    catch (WebException)
                    {
                        _logger.Error("Backup download link for MyVersions_Temp.txt failed.");
                        return;
                    }
                }
                else
                {
                    _logger.Error(ex.Message);
                    _logger.Error("An error occurred during the download: ");
                    return;
                }
            }
        }

        //Read downloaded file and parse entries for comparison
        string[] newVersions = await File.ReadAllLinesAsync(@"..\MyVersions_Temp.txt");

        //If parsed entry does not match appVersion member value, display Update Ready Notification
        if (newVersions[0] != appVersion && (newVersions[0].Length <= 5))
        {
            LauncherUpdateString = $"D2RLAN Update Ready! ({newVersions[0]})";
            LauncherHasUpdate = true;
        }

        File.Delete(@"..\MyVersions_Temp.txt");
    }
    [UsedImplicitly]
    public async void OnUpdateLauncher() //User has decided to update D2RLAN; prep external updater program for update
    {
        WebClient webClient = new();

        if (File.Exists("lnu.txt"))
            File.Delete("lnu.txt");

        //Force download of the latest updater program
        string primaryLink2 = "https://www.dropbox.com/scl/fi/duj7sqvkxw8a04xsj6yt0/D2RT_Updater.zip?rlkey=di34pwocmhxyap8c42m597c25&st=ou60tno7&dl=1";
        string backupLink2 = "https://d2filesdrop.s3.us-east-2.amazonaws.com/D2RT_Updater.zip";
        string baseDir = System.IO.Path.GetFullPath(@"..\");

        string updaterDir = System.IO.Path.Combine(baseDir, "Updater");

        // Check and remove the \\?\ prefix if it exists and is not necessary
        if (updaterDir.StartsWith(@"\\?\"))
        {
            updaterDir = updaterDir.Substring(4);
        }

        try
        {
            webClient.DownloadFile(primaryLink2, System.IO.Path.Combine(baseDir, "UpdateU.zip"));
        }
        catch (WebException ex)
        {
            if (ex.Response is HttpWebResponse response && ((int)response.StatusCode == 429 || (int)response.StatusCode == 500))
            {
                try
                {
                    webClient.DownloadFile(backupLink2, System.IO.Path.Combine(baseDir, "UpdateU.zip"));
                }
                catch (WebException)
                {
                    _logger.Error("Backup download link 2 failed.");
                    MessageBox.Show("Backup download link 2 failed.", "Download error.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }
            }
            else
            {
                _logger.Error(ex.Message);
                _logger.Error("An error occurred during the download: ");
                MessageBox.Show("An error occurred during the download:", "Download error.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
        }

        //Download complete, remove old files and extract new ones
        if (Directory.Exists(updaterDir))
            Directory.Delete(updaterDir, true);

        Directory.CreateDirectory(updaterDir);
        ZipFile.ExtractToDirectory(System.IO.Path.Combine(baseDir, "UpdateU.zip"), updaterDir);

        if (File.Exists(System.IO.Path.Combine(baseDir, "UpdateU.zip")))
            File.Delete(System.IO.Path.Combine(baseDir, "UpdateU.zip"));

        //Updater has finished extraction, create dummy .txt file to inform updater program of launcher update
        File.Create(System.IO.Path.Combine(baseDir, @"Launcher\lnu.txt")).Close(); //lnu = Launcher Needs Update
        Process.Start(System.IO.Path.Combine(baseDir, @"Updater\RMDUpdater-TCP.exe"));
        await TryCloseAsync();



        if (File.Exists(System.IO.Path.Combine(baseDir, "MyVersions_Temp.txt")))
            File.Delete(System.IO.Path.Combine(baseDir, "MyVersions_Temp.txt"));
    }

    #endregion

    #region ---Event System---

    //Not used currently (No TZ in 2.4)
    private async Task CheckForEvents()
    {

        DateTime systemTime = DateTime.UtcNow;
        DateTime ntpTime = GetNetworkTime();
        TimeSpan difference = systemTime - ntpTime;

        //Check if the difference is greater than or equal to 1 hour
        if (Math.Abs(difference.TotalHours) >= 1)
            MessageBox.Show("Manual Time modification detected!\nAborting Event System...");
        else
        {
            dynamic options = new ExpandoObject();
            options.ResizeMode = ResizeMode.NoResize;
            options.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SpecialEventsViewModel vm = new SpecialEventsViewModel(this);

            if (await _windowManager.ShowDialogAsync(vm, null, options))
            {
            }
        }
    }
    static DateTime GetNetworkTime()
    {
        const string ntpServer = "time.windows.com";
        var ntpData = new byte[48];
        ntpData[0] = 0x1B; // Leap indicator, version number, mode

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.Connect(ipEndPoint);
            socket.ReceiveTimeout = 3000;
            socket.Send(ntpData);
            socket.Receive(ntpData);
        }

        const byte serverReplyTime = 40;
        ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
        ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
        var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

        return networkDateTime;
    }
    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                      ((x & 0x0000ff00) << 8) +
                      ((x & 0x00ff0000) >> 8) +
                      ((x & 0xff000000) >> 24));
    }
    public class Entry
    {
        public int id { get; set; }
        public string Key { get; set; }
        public string deDE { get; set; }
        public string enUS { get; set; }
        public string esES { get; set; }
        public string esMX { get; set; }
        public string frFR { get; set; }
        public string itIT { get; set; }
        public string jaJP { get; set; }
        public string koKR { get; set; }
        public string plPL { get; set; }
        public string ptBR { get; set; }
        public string ruRU { get; set; }
        public string zhCN { get; set; }
        public string zhTW { get; set; }
    }

    #endregion

    #region ---TCP/Memory Functions---

    public async Task ApplyTCPPatch() //Load config and Inject D2RHUD.dll, special rules for RMD
    {
        string configPath = "config.json";
        var config = LoadConfig(configPath);

        if (config == null)
        {
            _logger.Error("Failed to load configuration file");
            return;
        }

        string processName = "../D2R/d2r.exe";
        string arguments = UserSettings.CurrentD2RArgs;

        if (HomeDrawerViewModel.ImageRNG == 10)
            arguments = UserSettings.CurrentD2RArgs + " -cheats";

        if (ModInfo.Name == "RMD-MP" && HomeDrawerViewModel.ImageRNG != 10)
            arguments = arguments.Replace(" -cheats", "");

        List<string> MSIPath = null;

        if (UserSettings.MSIFix == true)
        {
            //Close MSI Afterburner and Riva Tuner to allow Monster Stats Display to load correctly; restarted after game launch
            MSIPath = CloseMSIAfterburner("MSIAfterburner"); //Special Function to retrieve MSI path info; don't need others
            CloseRivaTuner("RTSS");
            CloseRivaTuner("RTSSHooksLoader64");
            CloseRivaTuner("EncoderServer");
            await Task.Delay(1000);
        }
        
        Process process = LaunchProcess(processName, arguments); //Start the game

        if (process != null)
        {
            InjectDLL(process.Id, "D2RHUD.dll");
            _logger.Error("D2RHUD.dll has been loaded");
            int skillIndex = await CheckSkillIndexAsync();
            EditMemory(process.Id, config.MemoryConfigs, skillIndex);
            _logger.Error("Memory Editing tasks begun");

            if (UserSettings.MSIFix == true)
            {
                foreach (string path in MSIPath)
                {
                    await Task.Delay(1000);
                    Process.Start(path); //Restart MSI Afterburner (which restarts Riva Tuner)
                }
            }     
        }
        else
            _logger.Error("Failed to launch the process.");

        if (debugLogging)
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        _logger.Error($"\n\n--------------------\nMod Name: {ModInfo.Name}\nGame Path: {GamePath}\nSave Path: {SaveFilesFilePath}\nLaunch Arguments: {UserSettings.CurrentD2RArgs}\n\nAudio Language: {UserSettings.AudioLanguage}\nText Language: {UserSettings.TextLanguage}\nUI Theme: {UserSettings.UiTheme}\nWindow Mode: {UserSettings.WindowMode}\nHDR Fix: {UserSettings.HdrFix}\n\nFont: {UserSettings.Font}\nBackups: {UserSettings.AutoBackups}\nPersonalized Tabs: {UserSettings.PersonalizedStashTabs}\nExpanded Cube: {UserSettings.ExpandedCube}\nExpanded Inventory: {UserSettings.ExpandedInventory}\nExpanded Merc: {UserSettings.ExpandedMerc}\nExpanded Stash: {UserSettings.ExpandedStash}\nBuff Icons: {UserSettings.BuffIcons}\nMonster Display: {UserSettings.MonsterStatsDisplay}\nSkill Icons: {UserSettings.SkillIcons}\nMerc Identifier: {UserSettings.MercIcons}\nItem Levels: {UserSettings.ItemIlvls}\nRune Display: {UserSettings.RuneDisplay}\nHide Helmets: {UserSettings.HideHelmets}\nItem Display: {UserSettings.ItemIcons}\nSuper Telekinesis: {UserSettings.SuperTelekinesis}\nColor Dyes: {UserSettings.ColorDye}\nCinematic Subtitles: {UserSettings.CinematicSubs}\nRuneword Sorting: {UserSettings.RunewordSorting}\nMerged HUD: {UserSettings.HudDesign}\n--------------------");
    }

    public List<string> CloseMSIAfterburner(string processName) //Used to find path info and close MSI Afterburner
    {
        List<string> exePaths = new List<string>();

        try
        {
            // Get all processes by name
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                try
                {
                    exePaths.Add(process.MainModule.FileName);
                    process.Kill();
                }
                catch (AccessViolationException)
                {
                    Console.WriteLine($"Access denied to process {processName}. Unable to retrieve the file path.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing process {processName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return exePaths;
    }
    public void CloseRivaTuner(string processName) //Used to find and close Riva Tuner
    {
        try
        {
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                try
                {
                    _logger.Error($"Closing process: {processName}");
                    _logger.Error($"Executable Path: {process.MainModule.FileName}");
                    process.Kill();
                }
                catch (AccessViolationException)
                {
                    _logger.Error($"Access denied to process {processName}. Unable to retrieve the file path.");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error accessing process {processName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    public async Task<int> CheckSkillIndexAsync() //Hotfix for controller crashing when viewing icons on new skill indexes
    {
        string filePath = SelectedModDataFolder + @"\global\excel\skills.txt";
        int rowCount = 369; // Default row count

        if (File.Exists(filePath))
        {
            string[] lines = await File.ReadAllLinesAsync(filePath);
            rowCount = lines.Length;
        }
        if (!File.Exists(filePath) && ModInfo.Name == "RMD-MP")
            rowCount = 718;

        return rowCount;
    }
    static Process LaunchProcess(string processName, string arguments) //Start the game
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = processName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = !debugLogging
            };

            Process process = Process.Start(startInfo);
            process.WaitForInputIdle();
            return process;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error launching process: {ex.Message}");
            return null;
        }
    }
    static void EditMemory(int processId, List<MemoryConfig> memoryConfigs, int skillIndex) //Apply needed memory adjustments to game
    {
        int desiredAccess = PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION;
        IntPtr hProcess = OpenProcess(desiredAccess, false, processId);

        if (hProcess == IntPtr.Zero)
        {
            Console.WriteLine("Failed to open process for memory editing.");
            return;
        }

        Process process = Process.GetProcessById(processId);
        IntPtr baseAddress = process.MainModule.BaseAddress;

        if (debugLogging)
            Console.WriteLine($"Base Address of {process.ProcessName}: 0x{baseAddress.ToString("X")}");

        foreach (var entry in memoryConfigs)
        {
            try
            {
                if (entry.Addresses != null && entry.Addresses.Count > 0)
                {
                    foreach (var address in entry.Addresses)
                    {
                        ProcessAddress(baseAddress, hProcess, address, entry.Length, entry.Values);
                    }
                }
                else if (!string.IsNullOrEmpty(entry.Address))
                    ProcessAddress(baseAddress, hProcess, entry.Address, entry.Length, entry.Values);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing memory: {ex.Message}");
            }
        }

        //Force-enable cheats for the below "QoL" functions
        ProcessAddress(baseAddress, hProcess, "1803258", 1, "01"); //Identify All
        ProcessAddress(baseAddress, hProcess, "18034C8", 1, "01"); //Reset Skills Only
        ProcessAddress(baseAddress, hProcess, "18034F8", 1, "01"); //Reset Stats Only
        ProcessAddress(baseAddress, hProcess, "1803588", 1, "01"); //Force Save
        ProcessAddress(baseAddress, hProcess, "1803888", 1, "01"); //Clear Ground Items

        //Memory edits needed to hotfix controller crashing when viewing icons above the retail index range. This may not be all possible edits needed; but no known issues currently.
        if (skillIndex != 369)
        {
            ProcessAddress(baseAddress, hProcess, "B4CBB", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex)).Replace("-", ""));
            ProcessAddress(baseAddress, hProcess, "CAEE3", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex)).Replace("-", ""));
            ProcessAddress(baseAddress, hProcess, "CB7DF", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex)).Replace("-", ""));           
            ProcessAddress(baseAddress, hProcess, "CD2CD", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex)).Replace("-", ""));
            ProcessAddress(baseAddress, hProcess, "CD642", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex)).Replace("-", ""));           
            ProcessAddress(baseAddress, hProcess, "111ED5", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex)).Replace("-", ""));           
            ProcessAddress(baseAddress, hProcess, "111F39", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex + 1)).Replace("-", ""));
            ProcessAddress(baseAddress, hProcess, "111F9D", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex + 1)).Replace("-", ""));
            ProcessAddress(baseAddress, hProcess, "112002", 2, BitConverter.ToString(ConvertToLittleEndian(skillIndex + 1)).Replace("-", ""));
        }
        CloseHandle(hProcess);
    }
    static void ProcessAddress(IntPtr baseAddress, IntPtr hProcess, string address, int length, string values) //Process and convert addresses found from user config file
    {
        long offset = Convert.ToInt64(address, 16);
        IntPtr effectiveAddress = IntPtr.Add(baseAddress, (int)offset);

        if (debugLogging)
        {
            Console.WriteLine($"Offset from config: 0x{offset:X}");
            Console.WriteLine($"Calculated Effective Address: 0x{effectiveAddress.ToString("X")}");
        }

        byte[] currentBytes = new byte[length];
        int bytesRead = 0;

        if (ReadProcessMemory(hProcess, effectiveAddress, currentBytes, length, ref bytesRead))
        {
            if (debugLogging)
                Console.WriteLine($"Current bytes at address 0x{effectiveAddress.ToString("X")}: {BitConverter.ToString(currentBytes)}");
        }
        else
            Console.WriteLine($"Failed to read memory at address 0x{effectiveAddress.ToString("X")}");

        byte[] valueBytes;

        if (values.Equals("80", StringComparison.OrdinalIgnoreCase))
            valueBytes = Convert.FromHexString(values);
        else if (int.TryParse(values, out int intValue))
            valueBytes = BitConverter.GetBytes(intValue);
        else
            valueBytes = Convert.FromHexString(values);

        int bytesWritten = 0;

        if (WriteProcessMemory(hProcess, effectiveAddress, valueBytes, valueBytes.Length, ref bytesWritten))
        {
            if (debugLogging)
                Console.WriteLine($"Written values {values} to address 0x{effectiveAddress.ToString("X")}");
        }
        else
            Console.WriteLine($"Failed to write to address 0x{effectiveAddress.ToString("X")}");
    }
    public class MemoryConfig
    {
        public string Description { get; set; }
        public string Address { get; set; }
        public List<string> Addresses { get; set; }
        public int Length { get; set; }
        public string Values { get; set; }
    }
    public class Config
    {
        public bool MonsterStatsDisplay { get; set; }
        public List<MemoryConfig> MemoryConfigs { get; set; }
    }

    #endregion

    #region ---Helper Functions---

    static void InjectDLL(int processId, string dllPath)
    {
        IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | PROCESS_VM_WRITE, false, processId);

        if (hProcess == IntPtr.Zero)
        {
            Console.WriteLine("Failed to open process for DLL injection.");
            return;
        }

        IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((dllPath.Length + 1) * Marshal.SizeOf<char>()), MEM_COMMIT, PAGE_EXECUTE_READWRITE);

        if (allocMemAddress == IntPtr.Zero)
        {
            Console.WriteLine("Failed to allocate memory in the target process.");
            CloseHandle(hProcess);
            return;
        }

        int bytesWritten;
        if (!WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.Default.GetBytes(dllPath), (uint)(dllPath.Length + 1), out bytesWritten))
        {
            Console.WriteLine("Failed to write DLL path to target process.");
            CloseHandle(hProcess);
            return;
        }

        IntPtr hKernel32 = GetModuleHandle("kernel32.dll");
        IntPtr hLoadLibrary = GetProcAddress(hKernel32, "LoadLibraryA");

        if (hLoadLibrary == IntPtr.Zero)
        {
            Console.WriteLine("Failed to get address of LoadLibraryA.");
            CloseHandle(hProcess);
            return;
        }

        IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, hLoadLibrary, allocMemAddress, 0, out _);

        if (hThread == IntPtr.Zero)
            Console.WriteLine("Failed to create remote thread.");
        else
            Console.WriteLine("DLL injected successfully!");

        CloseHandle(hThread);
        CloseHandle(hProcess);
    }
    static Config LoadConfig(string configPath)
    {
        try
        {
            string jsonContent = File.ReadAllText(configPath);
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            options.TypeInfoResolver = ConfigSourceGenerationContext.Default;
            ConfigSourceGenerationContext ctx = new ConfigSourceGenerationContext(options);
            return System.Text.Json.JsonSerializer.Deserialize(jsonContent, ctx.Config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config: {ex.Message}");
            return null;
        }
    }
    static byte[] ConvertToLittleEndian(int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes.Take(2).ToArray();
    }
    public async Task<List<string>> GetCharacterNames()
    {
        string actualSaveFilePath;
        // Determine if the mod is using a mod folder or retail folder for backups by verifying the directories first
        if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @$"Saved Games\Diablo II Resurrected\Mods\{Settings.Default.SelectedMod}")))
        {
            // The save directory doesn't exist; this mod is using retail location - set default pathing info
            actualSaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @$"Saved Games\Diablo II Resurrected\");
        }
        else
        {
            // The save directory exists; this mod is using mod folder locations - proceed normally
            actualSaveFilePath = SaveFilesFilePath;
        }

        List<string> saveFiles = Directory.GetFiles(actualSaveFilePath).ToList();
        List<string> characterNames = new List<string>();
        foreach (string save in saveFiles.Where(s => s.EndsWith(".d2s")))
        {
            characterNames.Add(Path.GetFileNameWithoutExtension(save.Split('\\').Last()));
        }

        return characterNames;
    }

    #endregion 
}