using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.MaterialDark.WPF;
using Syncfusion.Windows.Shared;
using Color = System.Drawing.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace D2RLAN.Models;

public class ChromelessWindowManager : WindowManager
{
    protected override Window EnsureWindow(object model, object view, bool isDialog)
    {
        if (view is Window window)
        {
            Window owner = InferOwnerOf(window);
            if (owner != null && isDialog)
            {
                window.Owner = owner;
                window.ResizeMode = ResizeMode.CanResizeWithGrip;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            SfSkinManager.SetVisualStyle(window, VisualStyles.MaterialDark);
            SfSkinManager.ApplyStylesOnApplication = true;

            MaterialDarkThemeSettings settings = new MaterialDarkThemeSettings();
            settings.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Fonts/#Formal436 BT");
            settings.PrimaryBackground = new SolidColorBrush(Colors.Firebrick);
            SfSkinManager.RegisterThemeSettings("MaterialDark", settings);

        }
        else
        {
            window = new ChromelessWindow { Content = view, SizeToContent = SizeToContent.WidthAndHeight, UseNativeChrome = true };

            SfSkinManager.SetTheme(window, new Theme("MaterialDark"));

            MaterialDarkThemeSettings settings = new MaterialDarkThemeSettings();
            settings.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Fonts/#Formal436 BT");
            settings.PrimaryBackground = new SolidColorBrush(Colors.CornflowerBlue);
            SfSkinManager.RegisterThemeSettings("MaterialDark", settings);

            window.SetValue(View.IsGeneratedProperty, true);

            Window owner = InferOwnerOf(window);

            if (owner != null)
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner = owner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            SfSkinManager.SetVisualStyle(window, VisualStyles.MaterialDark);
            SfSkinManager.ApplyStylesOnApplication = true;
        }

        return window;
    }
}