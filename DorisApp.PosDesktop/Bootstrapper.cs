using Caliburn.Micro;
using DorisApp.Data.Library.API;
using DorisApp.Data.Library.Model;
using DorisApp.PosDesktop.Helpers;
using DorisApp.PosDesktop.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DorisApp.PosDesktop
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new(); 

        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
                        PasswordBoxHelper.BoundPasswordProperty,
                        "Password",
                        "PasswordChanged");


        }

        protected override void Configure()
        {
            _container.Instance(_container)
                .PerRequest<SalesEndpoint>()
                .PerRequest<CategoryEndpoint>()
                .PerRequest<SubCategoryEndpoint>();

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<AuthenticatedUserModel>()
                .Singleton<TransactionHelper>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IAPIHelper, APIHelper>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(@"C:\Users\PC\Desktop\DorisRetailApp\DorisApp.PosDesktop")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _container.RegisterInstance(typeof(IConfiguration), null, configuration);

            GetType().Assembly.GetTypes()
                .Where(t => t.IsClass)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));

            CultureInfo cultureInfo= new("en-ph");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), 
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));
        }

        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

    }


}
