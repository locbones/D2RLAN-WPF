using System;
using System.Windows;

namespace D2RLAN.Views.Dialogs
{
    /// <summary>
    /// Window Owner Centering logic for CASCExtractorView.xaml
    /// </summary>
    public partial class CASCExtractorView : Window
    {
        public CASCExtractorView()
        {
            InitializeComponent();
            Loaded += CASCExtractorView_Loaded;
        }

        private void CASCExtractorView_Loaded(object sender, RoutedEventArgs e)
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
