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
    public partial class ProgressBarView : Window
    {
        private ProgressBarViewModel _viewModel;

        public ProgressBarView()
        {
            InitializeComponent();
            DataContextChanged += ProgressBarView_DataContextChanged;
            Loaded += ProgressBarView_Loaded;
        }

        private void ProgressBarView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as ProgressBarViewModel;
        }

        private void ProgressBarView_Loaded(object sender, RoutedEventArgs e)
        {
            // Center the window relative to its owner
            if (Owner != null)
            {
                Left = Owner.Left + (Owner.Width - Width) / 2;
                Top = Owner.Top + (Owner.Height - Height) / 2;
            }
        }
    }
}

