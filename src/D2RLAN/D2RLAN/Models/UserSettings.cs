using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using D2RLAN.ViewModels.Drawers;

namespace D2RLAN.Models;

public class UserSettings : INotifyPropertyChanged
{
    #region members

    private int _audioLanguage;
    private int _autoBackups;
    private int _buffIcons;
    private bool _hdrFix;
    private bool _noSound;
    private bool _skipLogo;
    private bool _forceLowend;
    private int _hideHelmets;
    private int _hudDesign;
    private bool _infiniteRespec;
    private int _itemIcons;
    private int _itemIlvls;
    private int _mercIcons;
    private int _monsterStatsDisplay;
    private int _monsterHP;
    private int _mapLayout;
    private int _windowMode;
    private int _personalizedTabs;
    private bool _resetMaps;
    private int _runewordSorting;
    private int _skillIcons;
    private int _textLanguage;
    private int _uiTheme;
    private bool _directTxt;
    private int _personalizedStashTabs;
    private int _font;
    private int _superTelekinesis;
    private int _runeDisplay;
    private string _buffIconTemplate;
    private int _selectedMonsterItemDrops = 0;
    private int _selectedGroupSize = 0;
    private Dictionary<string, DifficultyCustomizations> _difficultyCustomizations;
    private bool _ExpandedInventory;
    private bool _ExpandedStash;
    private bool _ExpandedCube;
    private bool _ExpandedMerc;
    private int _colorDye;
    private string _currentD2RArgs;
    private string _fastLoad;
    private int _CinematicSubs;
    private string _mapSeed;
    private string _mapSeedName;
    private string _mapSeedLoc;
    private bool _Cheats;
    private bool _CheatsActive;
    private bool _MSIFix;
    private bool _skipCinematics;
    private int _shortenedLevels;
    private int _lootFilter;
    private string _dataHash;
    private bool _dataHashPass;
    private int _BeaconStartup;
    private bool _filterUpdates;
    private int _stringColoring;
    private bool _HUDDebug;
    private bool _LANOffline;
    private bool _closeMinimized;

    #endregion

    #region properties

    public bool InfiniteRespec
    {
        get => _infiniteRespec;
        set
        {
            if (value == _infiniteRespec)
            {
                return;
            }
            _infiniteRespec = value;
            OnPropertyChanged();
        }
    }

    public bool ResetMaps
    {
        get => _resetMaps;
        set
        {
            if (value == _resetMaps)
            {
                return;
            }
            _resetMaps = value;
            OnPropertyChanged();
        }
    }

    public int AudioLanguage
    {
        get => _audioLanguage;
        set
        {
            if (value == _audioLanguage)
            {
                return;
            }
            _audioLanguage = value;
            OnPropertyChanged();
        }
    }

    public int TextLanguage
    {
        get => _textLanguage;
        set
        {
            if (value == _textLanguage)
            {
                return;
            }
            _textLanguage = value;
            OnPropertyChanged();
        }
    }

    public int UiTheme
    {
        get => _uiTheme;
        set
        {
            if (value == _uiTheme)
            {
                return;
            }
            _uiTheme = value;
            OnPropertyChanged();
        }
    }

    public int ItemIcons
    {
        get => _itemIcons;
        set
        {
            if (value == _itemIcons)
            {
                return;
            }
            _itemIcons = value;
            OnPropertyChanged();
        }
    }

    public int MercIcons
    {
        get => _mercIcons;
        set
        {
            if (value == _mercIcons)
            {
                return;
            }
            _mercIcons = value;
            OnPropertyChanged();
        }
    }

    public int RunewordSorting
    {
        get => _runewordSorting;
        set
        {
            if (value == _runewordSorting)
            {
                return;
            }
            _runewordSorting = value;
            OnPropertyChanged();
        }
    }

    public int AutoBackups
    {
        get => _autoBackups;
        set
        {
            if (value == _autoBackups)
            {
                return;
            }
            _autoBackups = value;
            OnPropertyChanged();
        }
    }

    public int HudDesign
    {
        get => _hudDesign;
        set
        {
            if (value == _hudDesign)
            {
                return;
            }
            _hudDesign = value;
            OnPropertyChanged();
        }
    }

    public int ItemIlvls
    {
        get => _itemIlvls;
        set
        {
            if (value == _itemIlvls)
            {
                return;
            }
            _itemIlvls = value;
            OnPropertyChanged();
        }
    }

    public int BuffIcons
    {
        get => _buffIcons;
        set
        {
            if (value == _buffIcons)
            {
                return;
            }
            _buffIcons = value;
            OnPropertyChanged();
        }
    }

    public int MonsterStatsDisplay
    {
        get => _monsterStatsDisplay;
        set
        {
            if (value == _monsterStatsDisplay)
            {
                return;
            }
            _monsterStatsDisplay = value;
            OnPropertyChanged();
        }
    }

    public int MonsterHP
    {
        get => _monsterHP;
        set
        {
            if (value == _monsterHP)
            {
                return;
            }
            _monsterHP = value;
            OnPropertyChanged();
        }
    }

    public int HideHelmets
    {
        get => _hideHelmets;
        set
        {
            if (value == _hideHelmets)
            {
                return;
            }
            _hideHelmets = value;
            OnPropertyChanged();
        }
    }

    public int PersonalizedTabs
    {
        get => _personalizedTabs;
        set
        {
            if (value == _personalizedTabs)
            {
                return;
            }
            _personalizedTabs = value;
            OnPropertyChanged();
        }
    }

    public bool HdrFix
    {
        get => _hdrFix;
        set
        {
            if (value == _hdrFix)
            {
                return;
            }
            _hdrFix = value;
            OnPropertyChanged();
        }
    }

    public bool NoSound
    {
        get => _noSound;
        set
        {
            if (value == _noSound)
            {
                return;
            }
            _noSound = value;
            OnPropertyChanged();
        }
    }

    public bool SkipLogos
    {
        get => _skipLogo;
        set
        {
            if (value == _skipLogo)
            {
                return;
            }
            _skipLogo = value;
            OnPropertyChanged();
        }
    }

    public bool ForceLowend
    {
        get => _forceLowend;
        set
        {
            if (value == _forceLowend)
            {
                return;
            }
            _forceLowend = value;
            OnPropertyChanged();
        }
    }

    public int SkillIcons
    {
        get => _skillIcons;
        set
        {
            if (value == _skillIcons)
            {
                return;
            }
            _skillIcons = value;
            OnPropertyChanged();
        }
    }

    public int MapLayout
    {
        get => _mapLayout;
        set
        {
            if (value == _mapLayout)
            {
                return;
            }
            _mapLayout = value;
            OnPropertyChanged();
        }
    }

    public int WindowMode
    {
        get => _windowMode;
        set
        {
            if (value == _windowMode)
            {
                return;
            }
            _windowMode = value;
            OnPropertyChanged();
        }
    }

    public bool DirectTxt
    {
        get => _directTxt;
        set
        {
            if (value == _directTxt) return;
            _directTxt = value;
            OnPropertyChanged();
        }
    }

    public int PersonalizedStashTabs
    {
        get => _personalizedStashTabs;
        set
        {
            if (value == _personalizedStashTabs) return;
            _personalizedStashTabs = value;
            OnPropertyChanged();
        }
    }

    public int Font
    {
        get => _font;
        set
        {
            if (value == _font) return;
            _font = value;
            OnPropertyChanged();
        }
    }

    public int SuperTelekinesis
    {
        get => _superTelekinesis;
        set
        {
            if (value == _superTelekinesis) return;
            _superTelekinesis = value;
            OnPropertyChanged();
        }
    }

    public int ColorDye
    {
        get => _colorDye;
        set
        {
            if (value == _colorDye) return;
            _colorDye = value;
            OnPropertyChanged();
        }
    }

    public int RuneDisplay
    {
        get => _runeDisplay;
        set
        {
            if (value == _runeDisplay) return;
            _runeDisplay = value;
            OnPropertyChanged();
        }
    }

    public string BuffIconTemplate
    {
        get => _buffIconTemplate;
        set
        {
            if (value == _buffIconTemplate) return;
            _buffIconTemplate = value;
            OnPropertyChanged();
        }
    }

    public int SelectedMonsterItemDrops
    {
        get => _selectedMonsterItemDrops;
        set
        {
            if (value == _selectedMonsterItemDrops)
            {
                return;
            }
            _selectedMonsterItemDrops = value;
            OnPropertyChanged();
        }
    }

    public int SelectedGroupSize
    {
        get => _selectedGroupSize;
        set
        {
            if (value == _selectedGroupSize)
            {
                return;
            }
            _selectedGroupSize = value;
            OnPropertyChanged();
        }
    }

    public Dictionary<string, DifficultyCustomizations> DifficultyCustomizations
    {
        get => _difficultyCustomizations;
        set
        {
            if (Equals(value, _difficultyCustomizations))
            {
                return;
            }
            _difficultyCustomizations = value;
            OnPropertyChanged();
        }
    }

    public bool ExpandedInventory
    {
        get => _ExpandedInventory;
        set
        {
            if (value == _ExpandedInventory)
            {
                return;
            }
            _ExpandedInventory = value;
            OnPropertyChanged();
        }
    }

    public bool ExpandedStash
    {
        get => _ExpandedStash;
        set
        {
            if (value == _ExpandedStash)
            {
                return;
            }
            _ExpandedStash = value;
            OnPropertyChanged();
        }
    }

    public bool ExpandedCube
    {
        get => _ExpandedCube;
        set
        {
            if (value == _ExpandedCube)
            {
                return;
            }
            _ExpandedCube = value;
            OnPropertyChanged();
        }
    }

    public bool ExpandedMerc
    {
        get => _ExpandedMerc;
        set
        {
            if (value == _ExpandedMerc)
            {
                return;
            }
            _ExpandedMerc = value;
            OnPropertyChanged();
        }
    }

    public string CurrentD2RArgs
    {
        get => _currentD2RArgs;
        set
        {
            if (value == _currentD2RArgs) return;
            _currentD2RArgs = value;
            OnPropertyChanged();
        }
    }

    public string FastLoad
    {
        get => _fastLoad;
        set
        {
            if (value == _fastLoad) return;
            _fastLoad = value;
            OnPropertyChanged();
        }
    }

    public int CinematicSubs
    {
        get => _CinematicSubs;
        set
        {
            if (value == _CinematicSubs)
            {
                return;
            }
            _CinematicSubs = value;
            OnPropertyChanged();
        }
    }

    public string MapSeed
    {
        get => _mapSeed;
        set
        {
            if (value == _mapSeed) return;
            _mapSeed = value;
            OnPropertyChanged();
        }
    }

    public string MapSeedName
    {
        get => _mapSeedName;
        set
        {
            if (value == _mapSeedName) return;
            _mapSeedName = value;
            OnPropertyChanged();
        }
    }

    public string MapSeedLoc
    {
        get => _mapSeedLoc;
        set
        {
            if (value == _mapSeedLoc) return;
            _mapSeedLoc = value;
            OnPropertyChanged();
        }
    }

    public bool Cheats
    {
        get => _Cheats;
        set
        {
            if (value == _Cheats)
            {
                return;
            }
            _Cheats = value;
            OnPropertyChanged();
        }
    }

    public bool CheatsActive
    {
        get => _CheatsActive;
        set
        {
            if (value == _CheatsActive)
            {
                return;
            }
            _CheatsActive = value;
            OnPropertyChanged();
        }
    }
    public bool MSIFix
    {
        get => _MSIFix;
        set
        {
            if (value == _MSIFix)
            {
                return;
            }
            _MSIFix = value;
            OnPropertyChanged();
        }
    }
    public bool skipCinematics
    {
        get => _skipCinematics;
        set
        {
            if (value == _skipCinematics)
            {
                return;
            }
            _skipCinematics = value;
            OnPropertyChanged();
        }
    }
    public int ShortenedLevels
    {
        get => _shortenedLevels;
        set
        {
            if (value == _shortenedLevels)
            {
                return;
            }
            _shortenedLevels = value;
            OnPropertyChanged();
        }
    }
    public int LootFilter
    {
        get => _lootFilter;
        set
        {
            if (value == _lootFilter)
            {
                return;
            }
            _lootFilter = value;
            OnPropertyChanged();
        }
    }

    public string DataHash
    {
        get => _dataHash;
        set
        {
            if (value == _dataHash) return;
            _dataHash = value;
            OnPropertyChanged();
        }
    }

    public bool DataHashPass
    {
        get => _dataHashPass;
        set
        {
            if (value == _dataHashPass)
            {
                return;
            }
            _dataHashPass = value;
            OnPropertyChanged();
        }
    }

    public int BeaconStartup
    {
        get => _BeaconStartup;
        set
        {
            if (value == _BeaconStartup)
            {
                return;
            }
            _BeaconStartup = value;
            OnPropertyChanged();
        }
    }

    public bool FilterUpdates
    {
        get => _filterUpdates;
        set
        {
            if (value == _filterUpdates)
            {
                return;
            }
            _filterUpdates = value;
            OnPropertyChanged();
        }
    }

    public int StringColoring
    {
        get => _stringColoring;
        set
        {
            if (value == _stringColoring)
            {
                return;
            }
            _stringColoring = value;
            OnPropertyChanged();
        }
    }

    public bool HUDDebug
    {
        get => _HUDDebug;
        set
        {
            if (value == _HUDDebug)
            {
                return;
            }
            _HUDDebug = value;
            OnPropertyChanged();
        }
    }

    public bool LANOffline
    {
        get => _LANOffline;
        set
        {
            if (value == _LANOffline)
            {
                return;
            }
            _LANOffline = value;
            OnPropertyChanged();
        }
    }

    public bool CloseMinimized
    {
        get => _closeMinimized;
        set
        {
            if (value == _closeMinimized)
            {
                return;
            }
            _closeMinimized = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
}