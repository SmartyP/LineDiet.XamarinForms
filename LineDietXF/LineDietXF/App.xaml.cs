using LineDietXF.Interfaces;
using LineDietXF.MockServices;
using LineDietXF.Services;
using LineDietXF.Views;
using Unity;
using Prism.Unity;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Prism;
using Prism.Ioc;

namespace LineDietXF
{
    public partial class App : PrismApplication
    {
        string DBPath { get; set; }
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null, "") { }

        public App(IPlatformInitializer platformInitializer, string dbPath) : base(platformInitializer)
        {
            DBPath = dbPath;
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
           
            Container.Resolve<IAnalyticsService>().Initialize(Constants.Analytics.GA_TrackingID, Constants.Analytics.GA_AppName, Constants.Analytics.GA_DispatchPeriod);
            Container.Resolve<ISettingsService>().Initialize();

            try
            {
                await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(MainPage)}");
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();

                var analyticsService = Container.Resolve<IAnalyticsService>();
                if (analyticsService != null)
                    analyticsService.TrackFatalError($"{nameof(OnInitialized)} - an exception occurred trying to navigate to the MainPage.", ex);
                throw ex;
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            bool useMockServices = false;
#if DEBUG
            useMockServices = Constants.App.DEBUG_UseMocks;
#endif

            // Register Services
            // NOTE:: we register with ContainerControlledLifetimeManager to ensure only one instance exists
            if (useMockServices)
            {
                containerRegistry.RegisterSingleton<IAnalyticsService, MockAnalyticsService>();
                containerRegistry.RegisterSingleton<ISettingsService, MockSettingsService>();
                containerRegistry.RegisterSingleton<IDataService, MockDataService>();
                containerRegistry.RegisterSingleton<IReviewService, MockReviewService>();
                containerRegistry.RegisterSingleton<IWindowColorService, MockWindowColorService>();
            }
            else
            {
                // NOTE:: IAnalyticsService must be registered by platform via IPlatformInitializer.RegisterTypes()
                containerRegistry.RegisterSingleton<ISettingsService, LocalSettingsService>();
                containerRegistry.RegisterSingleton<IDataService, SQLiteDataService>();
                // NOTE:: IReviewService must be registered by platform via IPlatformInitializer.RegisterTypes()
                containerRegistry.RegisterSingleton<IWindowColorService, WindowColorService>();
            }

            // Register Pages
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<DailyInfoPage>();
            containerRegistry.RegisterForNavigation<GraphPage>();
            containerRegistry.RegisterForNavigation<MenuPage>();
            containerRegistry.RegisterForNavigation<SetGoalPage>();
            containerRegistry.RegisterForNavigation<WeightEntryPage>();
            containerRegistry.RegisterForNavigation<GettingStartedPage>();
            containerRegistry.RegisterForNavigation<AboutPage>();
            containerRegistry.RegisterForNavigation<SettingsPage>();

#if DEBUG
            containerRegistry.RegisterForNavigation<DebugPage>();
#endif
        }

        async Task InitializeAsyncServices()
        {
            await Container.Resolve<IDataService>().Initialize(DBPath);
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            await InitializeAsyncServices();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}