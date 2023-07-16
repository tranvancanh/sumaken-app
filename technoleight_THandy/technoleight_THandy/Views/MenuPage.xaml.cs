using technoleight_THandy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static technoleight_THandy.Models.Login;
using System.Threading.Tasks;
using Xamarin.Essentials;
using technoleight_THandy.ViewModels;
using Newtonsoft.Json;
using static technoleight_THandy.Models.Setting;
using static technoleight_THandy.Models.HandyAdminModel;
using technoleight_THandy.Models.common;

namespace technoleight_THandy.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        //List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            TopImage.Source = ImageSource.FromResource("technoleight_THandy.img.menu_img.png");

            //menuItems = new List<HomeMenuItem>
            //{
            //    //メニュー画面
            //    new HomeMenuItem {Id = MenuItemType.Browse, Title="アプリ" },
            //    new HomeMenuItem {Id = MenuItemType.About, Title="Web" },
            //    new HomeMenuItem {Id = MenuItemType.Login, Title="ログアウト" }
            //};

            //ListViewMenu.ItemsSource = menuItems;

            //ListViewMenu.SelectedItem = menuItems[0];
            //ListViewMenu.ItemSelected += async (sender, e) =>
            //{
            //    if (e.SelectedItem == null)
            //        return;

            //    var id = (int)((HomeMenuItem)e.SelectedItem).Id;
            //    await RootPage.NavigateFromMenu(id);
            //};

            SetUserDetail();

        }
        private async void SetUserDetail()
        {
            var loginUserSqlLite = new Login.LoginUserSqlLite();

            List<Login.LoginUserSqlLite> loginUserSqlLites = await App.DataBase.GetLognAsync();
            if (loginUserSqlLites.Count > 0)
            {
                loginUserSqlLite = loginUserSqlLites[0];

                CompanyCode.Text = loginUserSqlLite.CompanyCode;
                CompanyName.Text = loginUserSqlLite.CompanyName;
                WarehouseCode.Text = loginUserSqlLite.DepoCode;
                WarehouseName.Text = loginUserSqlLite.DepoName;
                UserCode.Text = loginUserSqlLite.HandyUserCode;
                UserName.Text = loginUserSqlLite.HandyUserName;
            }
            else
            {

            }

        }

        /// <summary>
        /// サイズが決まった後で呼び出されます。AbsoluteLayout はここで位置を決めるのが良いみたいです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AbsolutePageXamlSizeChanged(object sender, EventArgs e)
        {
            AbsoluteLayout.SetLayoutFlags(AdminPasswordInputDialog,
                AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(AdminPasswordInputDialog,
                new Rectangle(0.5d, 0.5d,
                Device.OnPlatform(AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize, this.Width), AbsoluteLayout.AutoSize)); // View の中央に AutoSize で配置

            AbsoluteLayout.SetLayoutFlags(BackgroundLayer,
                AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(BackgroundLayer,
                new Rectangle(0d, 0d,
                this.Width, this.Height)); // View の左上から View のサイズ一杯で配置
        }

        private async Task<int> DeleteScanData()
        {
            await App.DataBase.DeleteAllScanReceiveSendData();
            await App.DataBase.DeleteAllScanReceive();
            return 1;
        }

        private void OnDeleteScanDataClicked(object sender, EventArgs e)
        {
            AdminPasswordDialogToggle(true);
        }

        private void OnDeleteScanDataCancelClicked(object sender, EventArgs e)
        {
            AdminPasswordDialogToggle(false);
        }

        private void AdminPasswordDialogToggle(bool viewFlag)
        {
            if (viewFlag)
            {
                if (!AdminPasswordInputDialog.IsVisible)
                {
                    BackgroundLayer.IsVisible = true;
                    AdminPasswordInputDialog.IsVisible = true;
                    MainContent.IsEnabled = false;
                    return;
                }
            }
            else
            {
                if (AdminPasswordInputDialog.IsVisible)
                {
                    AdminPasswordInputDialog.IsVisible = false;
                    BackgroundLayer.IsVisible = false;
                    MainContent.IsEnabled = true;
                    return;
                }
            }
            return;
        }

        private void OnLoginDataViewClicked(object sender, EventArgs e)
        {
            LoginDataViewToggle();
        }

        private void LoginDataViewToggle()
        {
            if (LoginDataView.IsVisible)
            {
                LoginDataViewIcon.Text = "\uf078";
                LoginDataView.IsVisible = false;
            }
            else
            {
                LoginDataViewIcon.Text = "\uf106";
                LoginDataView.IsVisible = true;
            }
            return;
        }

        protected override void OnDisappearing()
        {
            AdminPasswordDialogToggle(false);

            if (LoginDataView.IsVisible)
            {
                LoginDataView.IsVisible = false;
            }
        }

        private async void OnDeleteScanDataDoneClicked(object sender, EventArgs e)
        {
            var handyAdminRequest = new HandyAdminModel.HandyAdminRequestBody();

            try
            {
                handyAdminRequest.CompanyID = App.Setting.CompanyID;
                handyAdminRequest.HandyAdminPassword = AdminPasswordInputPassword.Text;

                var jsonDataSend = JsonConvert.SerializeObject(handyAdminRequest);
                var responseMessage = await App.API.PostMethod(jsonDataSend, App.Setting.HandyApiUrl, "HandyAdminCheck");
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    // OK
                    var result = await Application.Current.MainPage.DisplayAlert("警告", "未登録データを削除します\nよろしいですか？", "Yes", "No");
                    if (result)
                    {
                        await DeleteScanData();
                        await Application.Current.MainPage.DisplayAlert("完了", "未登録データの削除が完了しました", "OK");

                        // TOPメニューを再表示する
                        Application.Current.MainPage = new MainPage();
                    }
                }
                else
                {
                    await App.DisplayAlertError(responseMessage.content);
                    return;
                }

            }
            catch (CustomExtention ex)
            {
                await App.DisplayAlertError(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                await App.DisplayAlertError();
                return;
            }

            return;
        }

        private async void LogoutButtonClicked(object sender, EventArgs e)
        {
            MainPage RootPage = Application.Current.MainPage as MainPage;
            var page = new HomeMenuItem { Id = MenuItemType.Login, Title = "ログアウト" };
            var id = (int)((page)).Id;
            await RootPage.NavigateFromMenu(id);
            return;
        }

    }
}