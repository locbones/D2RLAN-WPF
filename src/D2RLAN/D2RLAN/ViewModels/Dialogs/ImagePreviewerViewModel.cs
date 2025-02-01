using Caliburn.Micro;

namespace D2RLAN.ViewModels.Dialogs;

public class ImagePreviewerViewModel : Screen
{
    #region ---Static Members---

    private string _imagePath;

    #endregion

    #region ---Window/Loaded Handlers---

    public ImagePreviewerViewModel(string imagePath, string title)
    {
        DisplayName = title;
        _imagePath = imagePath;
    }

    #endregion

    #region ---Properties---

    public string ImagePath
    {
        get => _imagePath;
        set
        {
            if (value == _imagePath)
            {
                return;
            }
            _imagePath = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion
}