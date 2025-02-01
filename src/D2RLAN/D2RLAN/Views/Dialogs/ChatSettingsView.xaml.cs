using D2RLAN.Models;
using D2RLAN.ViewModels;
using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Controls;
using D2RLAN.ViewModels.Dialogs;
using System.ComponentModel;

namespace D2RLAN.Views.Dialogs
{
    public partial class ChatSettingsView : Window
    {
        // Reference to the ViewModel
        private ChatSettingsViewModel _viewModel;

        public ChatSettingsView()
        {
            InitializeComponent();
            _viewModel = (ChatSettingsViewModel)DataContext; // Assuming your ViewModel is set as the DataContext
            Loaded += ChatSettingsView_Loaded;
        }

        private void ChatSettingsView_Loaded(object sender, RoutedEventArgs e)
        {
            // Center window based on the owner window
            if (Owner != null)
            {
                Left = Owner.Left + (Owner.Width - Width) / 2;
                Top = Owner.Top + (Owner.Height - Height) / 2;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Check if the ViewModel is set correctly and if SaveConfig should be called
            if (DataContext is ChatSettingsViewModel viewModel)
            {
                // Call the SaveConfig method in your ViewModel
                viewModel.SaveConfig();
            }
        }
    }
}
