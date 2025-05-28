using D2RLAN.Models;
using D2RLAN.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Controls;

namespace D2RLAN.Views.Dialogs
{
    /// <summary>
    /// Window Owner Centering logic for StashTabSettingsView.xaml
    /// </summary>
    ///


    public partial class LootFilterView : Window
    {

        public LootFilterView()
        {
            InitializeComponent();
            Loaded += LootFilterView_Loaded;
        }

        private void LootFilterView_Loaded(object sender, RoutedEventArgs e)
        {
            // Verify owner window and then center this window relative to it
            if (Owner != null)
            {
                Left = Owner.Left + (Owner.Width - Width) / 2;
                Top = Owner.Top + (Owner.Height - Height) / 2;
            }    
        }

        
    }
}
