using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace D2RLAN.Views.Dialogs
{
    /// <summary>
    /// Window Owner Centering logic for DownloadNewModView.xaml
    /// </summary>
    public partial class SpecialEventsView : Window
    {
        public SpecialEventsView()
        {
            InitializeComponent();
            Loaded += SpecialEventsView_Loaded;
        }

        private void SpecialEventsView_Loaded(object sender, RoutedEventArgs e)
        {
            // Verify owner window and then center this window relative to it
            if (Owner != null)
            {
                Left = Owner.Left + (Owner.Width - Width) / 2;
                Top = Owner.Top + (Owner.Height - Height) / 2;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
