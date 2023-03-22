using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using technoleight_THandy.Driver;
//using technoleight_THandy.Services;
using technoleight_THandy.Views;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using technoleight_THandy.Models;
using Java.Util;
using technoleight_THandy.Common;
using System.Linq;
using technoleight_THandy.Data;

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
        public static Setting.SettingSqlLite Setting;
        public static ResourceDictionary TargetResource;

        public static async Task DisplayAlertError(string message = "エラーが発生しました", string title = "エラー", string buttonName = "OK")
        {
            await Application.Current.MainPage.DisplayAlert(title, message, buttonName);
            return;
        }

        public static async Task DisplayAlertOkey(string message = "登録が完了しました", string title = "完了", string buttonName = "OK")
        {
            await Application.Current.MainPage.DisplayAlert(title, message, buttonName);
            return;
        }

        public static technoleight_THandydatabase DataBase
        {
            get
            {
                if (m_database == null)
                {
                    m_database = new technoleight_THandydatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tozanhandy.db3"));
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

            Task.Run(async () => { await GetSetting(); }).Wait();

            Task.Run(async () => { await GetTargetResource(); }).Wait();

            MainPage = new LoginPage();
        }

        public static async Task GetSetting()
        {
            Setting = new Setting.SettingSqlLite();
            List<Setting.SettingSqlLite> settingSqlLites = await App.DataBase.GetSettingAsync();

            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            mergedDictionaries.Clear();

            if (settingSqlLites.Count> 0)
            {
                Setting = settingSqlLites[0];

                var theme = Setting.ColorTheme;
                switch (theme)
                {
                    case Theme.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    case Theme.Light:
                    default:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }
            }
            else
            {
                mergedDictionaries.Add(new LightTheme());
            }
        }

        public static Task GetTargetResource()
        {
            TargetResource = Xamarin.Forms.Application.Current.Resources.MergedDictionaries.ElementAt(0);
            return Task.CompletedTask;
        }

        protected override void OnStart()
        {
            GetLocationInformation();
        }


        private async void GetLocationInformation()
        {
            // 位置情報取得の許可状態を確認
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    // 許可されていなかった場合はユーザーに確認する
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    // ユーザーが拒否した場合
                    if (status != PermissionStatus.Granted)
                        return;
                }
            }
            catch (Exception ex)
            {
                await App.DisplayAlertError("位置情報の許可設定に失敗しました");
                return;
            }
        }

        protected override void OnSleep()
        {
            // 最終スリープ時間を保存
            var now = DateTime.Now;
            Application.Current.Properties["LastSleepTime"] = now;
        }

        protected override void OnResume()
        {
            // 最終スリープ時間を復元
            if (Application.Current.Properties.ContainsKey(Const.C_APPLICATION_SESSION_LASTSLEEPTIME))
            {
                var now = DateTime.Now;
                var lastLoginDateString = Application.Current.Properties[Const.C_APPLICATION_SESSION_LASTSLEEPTIME].ToString();

                if (DateTime.TryParse(lastLoginDateString, out DateTime lastLogindate))
                {
                    // 最終スリープ時間が現在時刻より15分以上前だったらログイン画面に戻す
                    if (now > lastLogindate.AddSeconds(15))
                    {
                        MainPage = new LoginPage();
                    }
                }
            }
        }

    }
}
