using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using D2RLAN.Models;
using D2RLAN.ViewModels;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLAN;

public class Bootstrapper : BootstrapperBase
{
    #region members

    private SimpleContainer _container;

    #endregion

    public Bootstrapper() { Initialize(); }

    protected override object GetInstance(Type service, string key) { return _container.GetInstance(service, key); }

    protected override IEnumerable<object> GetAllInstances(Type service) { return _container.GetAllInstances(service); }

    protected override void BuildUp(object instance) { _container.BuildUp(instance); }

    protected override async void OnStartup(object sender, StartupEventArgs e) { await DisplayRootViewForAsync<ShellViewModel>(); }

    protected override void Configure()
    {
        try
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "D2RLaunch.log4net.config")));

            _container = new SimpleContainer();

            _container.PerRequest<ShellViewModel>();

            _container.Singleton<IWindowManager, ChromelessWindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            var config = Helper.GetResourceByteArray("appSettings.json").GetAwaiter().GetResult();

            using MemoryStream ms = new MemoryStream(config);

            IConfigurationBuilder configBuilder = new ConfigurationBuilder().AddJsonStream(ms);

            IConfigurationRoot configuration = configBuilder.Build();

            _container.RegisterInstance(typeof(IConfigurationRoot), "appSettings", configuration);
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}:{e.StackTrace}");
            if (null != e.InnerException)
            {
                MessageBox.Show($"{e.InnerException.Message}:{e.InnerException.StackTrace}");
            }
        }
    }
}