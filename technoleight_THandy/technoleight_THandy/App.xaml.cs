using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using technoleight_THandy.Driver;
//using technoleight_THandy.Services;
using technoleight_THandy.Views;
using System.Data;
using System.IO;

namespace technoleight_THandy
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        //To debug on Android emulators run the web backend against .NET Core not IIS
        //If using other emulators besides stock Google images you may need to adjust the IP address
        public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
        public static bool UseMockDataStore = true;

        static technoleight_THandydatabase m_database;

        public static technoleight_THandydatabase DataBase
        {
            get
            {
                if (m_database == null)
                {
                    m_database = new technoleight_THandydatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Thandy1.db3"));
                }
                return m_database;
            }
        }

        static Driver.APIClass m_api;
        public static Driver.APIClass API
        {
            get
            {
                if (m_api == null)
                {
                    m_api = new Driver.APIClass();
                }
                return m_api;
            }
        }

        public App()
        {
            InitializeComponent();

            //if (UseMockDataStore)
            //    DependencyService.Register<MockDataStore>();
            //else
            //    DependencyService.Register<AzureDataStore>();
            //初期立ち上げページを設定
            //MainPage = new MainPage();
            MainPage = new LoginPage();

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
