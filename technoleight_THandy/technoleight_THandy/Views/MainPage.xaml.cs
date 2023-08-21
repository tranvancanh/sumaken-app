using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using technoleight_THandy.Models;
using Xamarin.Essentials;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

namespace technoleight_THandy.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();

        public MainPage(bool isQrcodeLogin = false)
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;
            //1.メニューバー作成[左側]
            // MenuPage.xamlを出力
            MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);

            var itemPage = new ItemsPage();
            itemPage.IsQrcodeLogin = isQrcodeLogin;
            this.Detail = new NavigationPage(itemPage);
        }

        //protected override async void OnAppearing()
        //{
        //    // 位置情報取得の許可状態を確認
        //    try
        //    {
        //        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        //        if (status != PermissionStatus.Granted)
        //        {
        //            //許可されていなかった場合はユーザーに確認する
        //            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //            //ユーザーが拒否した場合
        //            if (status != PermissionStatus.Granted)
        //                return;
        //        }
        //    }
        //    catch(Exception ex)
        //    {

        //    }

        //}

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        //アプリ
                        MenuPages.Add(id, new NavigationPage(new ItemsPage()));                                               
                        break;
                    case (int)MenuItemType.About:
                        //WEB
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                    case (int)MenuItemType.Login:
                        //ログアウト
                        MenuPages.Add(id, new NavigationPage(new LoginPage()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}