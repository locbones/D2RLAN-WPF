using D2RLAN.Models;
using D2RLAN.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Application = System.Windows.Application;


namespace D2RLAN.Views
{
    public partial class ShellView : INotifyPropertyChanged
    {
        private NotifyIcon _trayIcon;
        private bool _trayInitialized;
        private bool _isExitRequested;

        private ShellViewModel ViewModel =>
    DataContext as ShellViewModel;


        public ShellView()
        {
            InitializeComponent();
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            if (_trayInitialized)
                return;

            var stream = Application.GetResourceStream(new Uri("pack://application:,,,/D2RLAN.ico")).Stream;
            _trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(stream),
                Visible = true,
                Text = "D2RLAN"
            };

            _trayIcon.DoubleClick += (s, e) => RestoreFromTray();

            var menu = new ContextMenuStrip();
            menu.Items.Add("Open", null, (s, e) => RestoreFromTray());
            menu.Items.Add("Exit", null, (s, e) => ExitApplication());

            _trayIcon.ContextMenuStrip = menu;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (ViewModel?.UserSettings?.CloseMinimized == true && !_isExitRequested)
            {
                e.Cancel = true;
                Hide();

                if (!_trayInitialized)
                _trayIcon.ShowBalloonTip(1000, "Minimized Only", "D2RLAN is still running!\nIt has been minimized to your system tray", ToolTipIcon.Info);

                return;
            }

            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                    .OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\BNA", writable: true))
                {
                    key.SetValue("CONNECTION_STRING_CN", "cn.actual.battlenet.com.cn");
                    key.SetValue("CONNECTION_STRING_CXX", "cn-ptr.actual.battle.net");
                    key.SetValue("CONNECTION_STRING_EU", "eu.actual.battle.net");
                    key.SetValue("CONNECTION_STRING_KR", "kr.actual.battle.net");
                    key.SetValue("CONNECTION_STRING_US", "us.actual.battle.net");
                    key.SetValue("CONNECTION_STRING_XX", "test.actual.battle.net");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            _trayIcon.Visible = false;
            _trayIcon.Dispose();
        }

        private void RestoreFromTray()
        {
            Show();
            WindowState = System.Windows.WindowState.Normal;
            Activate();
        }

        private void ExitApplication()
        {
            _isExitRequested = true;
            Close();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

    }
}
