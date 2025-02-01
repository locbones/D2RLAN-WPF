using System;
using System.Windows;

namespace D2RLAN.Views.Dialogs
{
    /// <summary>
    /// Window Owner Centering logic for BuffIconSettingsView.xaml
    /// </summary>
    public partial class BuffIconSettingsView : Window
    {
        public BuffIconSettingsView()
        {
            InitializeComponent();
            Loaded += BuffIconSettingsView_Loaded;
        }

        private void BuffIconSettingsView_Loaded(object sender, RoutedEventArgs e)
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
