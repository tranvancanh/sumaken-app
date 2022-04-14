using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THandy.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using THandy.Data;
using THandy.Interface;
using THandy.Models;
using Xamarin.Essentials;

namespace THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {

        private INotificationManager notificationManager;
        int notificationNumber = 0;
        public LoginPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            this.BindingContext = new LoginViewModel(this.Navigation);
        
            //lblPass.IsVisible = true;
            //txtPass.IsVisible = true;
            
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };

            // ★データコンテキスト登録(変化)時、VM側のActionに自分(View側)のメソッドを登録する
            var vm = (LoginViewModel)BindingContext;
            vm.ViewsideAction += ViewsideAction;

            LoginImg.Source = ImageSource.FromResource("technoleight_THandy.img.login_img.jpg");

        }

        public async void ViewsideAction()
        {
            notificationNumber++;
            string title = $"スマホアプリ";
            string message = $"ソフトが古いです更新をお願いします!";
            //notificationManager.SendNotification(title, message);
            //ソフトのバージョンチェックを行う
            string ManuFacturer = DependencyService.Get<IDeviceService>().GetManufacturerName();
            Uri url;
            if (ManuFacturer == "Apple")
            {
                url = new Uri("https://apps.apple.com/jp/app/pok%C3%A9mon-go/id1094591345");
            }
            else
            {
                url = new Uri("https://play.google.com/store/apps/details?id=com.nianticlabs.pokemongo");
            }

            await DisplayAlert(title, message, "次へ");
            await OpenBrowser(url);
        }

        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var msg = new Label()
                {
                    Text = $"修正依頼:\nTitle: {title}\nMessage: {message}"
                };

                //stackLayout.Children.Add(msg);
            });
        }

        public async Task OpenBrowser(Uri uri)
        {
            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // An unexpected error occured. No browser may be installed on the device.
            }
        }
    }
}