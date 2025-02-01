using System;
using System.Text.Json;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using D2RLAN.Models;
using log4net;
using Syncfusion.Licensing;
using System.Linq;

namespace D2RLAN
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ILog _logger = LogManager.GetLogger(typeof(App));
        public App()
        {
            try
            {
                byte[] rawAppSettings = Helper.GetResourceByteArray("appSettings.json").GetAwaiter().GetResult();

                // Skip BOM if present
                int skip = rawAppSettings.Length >= 3 &&
                    rawAppSettings[0] == 0xEF &&
                    rawAppSettings[1] == 0xBB &&
                    rawAppSettings[2] == 0xBF ? 3 : 0;

                string jsonString = Encoding.UTF8.GetString(rawAppSettings.Skip(skip).ToArray());

                using JsonDocument document = JsonDocument.Parse(jsonString);
                string licenseKey = document.RootElement.GetProperty("SyncfusionLicense").GetString();


                SyncfusionLicenseProvider.RegisterLicense(licenseKey);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                MessageBox.Show("Please make sure you have properly setup appSetting.json with the appropriate syncfusion license!");
                Current.Shutdown(0);
            }
            InitializeComponent();
        }

        private void OnUnhandledDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger?.Error(e.Exception);
            _logger?.Error(e.Exception.Message);

            e.Handled = true;
        }
    }
}
