using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using technoleight_THandy.Models;
using technoleight_THandy.Views;
using technoleight_THandy.ViewModels;
using Xamarin.Essentials;

namespace technoleight_THandy.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;
      
        public ItemsPage()
        {
            //最初のメイン画面
            InitializeComponent();

            SetUserDetail();

            // メイン画面をItemsで設定して表示を行う
            //ItemsViewModelでデータベースより抽出
            BindingContext = _viewModel = new ItemsViewModel();
        }

        private async void SetUserDetail()
        {
            var userDetail = new Setei();

            List<Setei> setting = await App.DataBase.GetSeteiAsync();
            if (setting.Count > 0)
            {
                userDetail = setting[0];

                CompanyCode.Text = userDetail.WID;
                CompanyName.Text = "：" + userDetail.CompanyName;
                UserCode.Text = userDetail.user;
                //UserName.Text = userDetail.username;
                WarehouseCode.Text = userDetail.WarehouseCode;
                WarehouseName.Text = "：" + userDetail.WarehouseName;
            }
            else
            {

            }

        }

        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            MainPage RootPage = Application.Current.MainPage as MainPage;
            var page = new HomeMenuItem { Id = MenuItemType.Login, Title = "ログアウト" };
            var id = (int)((page)).Id;
            await RootPage.NavigateFromMenu(id);
            return;
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //最初のメイン画面をさわった時
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            int x = 0;
            if (Int32.TryParse(item.Description, out x) == true)
            {

            }

            //if (x == 3)
            //{
            //    await Browser.OpenAsync("https://www.tozan.co.jp/stock-management-system/d_deliverying");
            //}
            if (x == 202)
            {
                Page page = new ScanBeforePage(new ScanBeforeViewModel(item.Text, item.Description));
                await Navigation.PushAsync(page);
            }
            else if (x == 206)
            {
                Page page = new ScanReceiptPage(new ScanReceiptViewModel(item.Text, item.Description, this.Navigation));
                await Navigation.PushAsync(page);
            }
            else if (x == 1000)
            {
                //デバイス情報ページ
                Page page3 = new ListViewPage2(new ListViewPage2ViewModel());
                await Navigation.PushAsync(page3);
            }

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        //async void AddItem_Clicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel.Items.Count == 0)
                _viewModel.LoadItemsCommand.Execute(null);
        }
    }
}