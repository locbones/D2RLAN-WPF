using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace D2RLAN.Views
{
    public partial class ShellView : INotifyPropertyChanged
    {
        public ShellView()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
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
        }
    }
}
