using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace D2RLAN.Models;

public class ModInfo : INotifyPropertyChanged
{
    #region members

    private bool _customizations;
    private bool _hudDisplay;
    private bool _itemIcons;
    private bool _mapLayouts;
    private string _modConfigDownloadLink;
    private string _modDescription;
    private string _modDownloadLink;
    private string _modTitle;
    private string _modVersion;
    private bool _monsterStatsDisplay;
    private string _name;
    private string _newsDescription;
    private string _newsTitle;
    private bool _runewordSorting;
    private string _savePath;
    private bool _uiThemes;
    private bool _vaultAccess;
    private string _discord;
    private string _wiki;
    private string _patreon;

    #endregion

    #region properties

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name)
            {
                return;
            }
            _name = value;
            OnPropertyChanged();
        }
    }

    public string SavePath
    {
        get => _savePath;
        set
        {
            if (value == _savePath)
            {
                return;
            }
            _savePath = value;
            OnPropertyChanged();
        }
    }

    public string ModDownloadLink
    {
        get => _modDownloadLink;
        set
        {
            if (value == _modDownloadLink)
            {
                return;
            }
            _modDownloadLink = value;
            OnPropertyChanged();
        }
    }

    public string ModConfigDownloadLink
    {
        get => _modConfigDownloadLink;
        set
        {
            if (value == _modConfigDownloadLink)
            {
                return;
            }
            _modConfigDownloadLink = value;
            OnPropertyChanged();
        }
    }

    public string ModVersion
    {
        get => _modVersion;
        set
        {
            if (value == _modVersion)
            {
                return;
            }
            _modVersion = value;
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

    public string NewsTitle
    {
        get => _newsTitle;
        set
        {
            if (value == _newsTitle)
            {
                return;
            }
            _newsTitle = value;
            OnPropertyChanged();
        }
    }

    public string NewsDescription
    {
        get => _newsDescription;
        set
        {
            if (value == _newsDescription)
            {
                return;
            }
            _newsDescription = value;
            OnPropertyChanged();
        }
    }

    public bool MapLayouts
    {
        get => _mapLayouts;
        set
        {
            if (value == _mapLayouts)
            {
                return;
            }
            _mapLayouts = value;
            OnPropertyChanged();
        }
    }

    public bool UIThemes
    {
        get => _uiThemes;
        set
        {
            if (value == _uiThemes)
            {
                return;
            }
            _uiThemes = value;
            OnPropertyChanged();
        }
    }

    public bool Customizations
    {
        get => _customizations;
        set
        {
            if (value == _customizations)
            {
                return;
            }
            _customizations = value;
            OnPropertyChanged();
        }
    }

    public bool VaultAccess
    {
        get => _vaultAccess;
        set
        {
            if (value == _vaultAccess)
            {
                return;
            }
            _vaultAccess = value;
            OnPropertyChanged();
        }
    }

    public bool ItemIcons
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

    public bool RunewordSorting
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

    public bool HudDisplay
    {
        get => _hudDisplay;
        set
        {
            if (value == _hudDisplay)
            {
                return;
            }
            _hudDisplay = value;
            OnPropertyChanged();
        }
    }

    public bool MonsterStatsDisplay
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

    public string Discord
    {
        get => _discord;
        set
        {
            if (value == _discord) return;
            _discord = value;
            OnPropertyChanged();
        }
    }

    public string Wiki
    {
        get => _wiki;
        set
        {
            if (value == _wiki) return;
            _wiki = value;
            OnPropertyChanged();
        }
    }

    public string Patreon
    {
        get => _patreon;
        set
        {
            if (value == _patreon) return;
            _patreon = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
}