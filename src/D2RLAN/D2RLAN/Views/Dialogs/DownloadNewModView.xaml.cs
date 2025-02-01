using System;
using System.Windows;

namespace D2RLAN.Views.Dialogs
{
    /// <summary>
    /// Window Owner Centering logic for DownloadNewModView.xaml
    /// </summary>
    public partial class DownloadNewModView : Window
    {
        public DownloadNewModView()
        {
            InitializeComponent();
            Loaded += DownloadNewModView_Loaded;
        }

        private void DownloadNewModView_Loaded(object sender, RoutedEventArgs e)
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
