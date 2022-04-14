using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using THandy.Models;
using THandy.Views;
using THandy.ViewModels;
using Xamarin.Essentials;

namespace THandy.Views
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
            // メイン画面をItemsで設定して表示を行う
            //ItemsViewModelでデータベースより抽出
            BindingContext = _viewModel = new ItemsViewModel();
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

            if ((4 <= x) && (x <= 100))
            {
                Page page = new ScanPage(new ScanViewModel(item.Text, item.Description, null,null, null,0));
                await Navigation.PushAsync(page);
            }
            else if (x == 1)
            {
                //納品書画面
                Page page = new ScanPage(new ScanViewModel(item.Text, item.Description, null, null, null,0));
                await Navigation.PushAsync(page);
            }
            else if (x == 2)
            {
                //現品票画面
                List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync("101");
                if (sagyoUsers.Count > 0)
                {
                    await DisplayAlert("注意", "登録処理をしていない、ピッキングが存在します", "OK");
                }
                else
                {
                    List<Nouhin> nouhin = await App.DataBase.GetNouhinAsync();
                    if (nouhin.Count == 0)
                    {
                        await DisplayAlert("注意", "ピッキング実績が存在しません。まずピッキングの読取をしてください", "OK");
                    }

                }

                Page page = new ScanPage(new ScanViewModel(item.Text, item.Description, null, null,null,0));
                await Navigation.PushAsync(page);
            }
            else if (x == 3)
            {
                await Browser.OpenAsync("https://www.tozan.co.jp/stock-management-system/d_deliverying");
            }
            else if (x == 101)
            {
                //納品書画面
                Page page = new ScanPage(new ScanViewModel(item.Text, item.Description, null, null, null,0));
                await Navigation.PushAsync(page);
            }
            else if (x == 102)
            {
                //現品票画面
                List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync("101");
                if (sagyoUsers.Count > 0)
                {
                    await DisplayAlert("注意", "登録処理をしていない、納品書が存在します", "OK");
                }
                else
                {
                    List<Nouhin> nouhin = await App.DataBase.GetNouhinAsync();
                    if (nouhin.Count == 0)
                    {
                        await DisplayAlert("注意", "納品書実績が存在しません。まず納品書の読取をしてください", "OK");
                    }

                }

                Page page = new ScanPage(new ScanViewModel(item.Text, item.Description, null, null, null, 0));
                await Navigation.PushAsync(page);
            }
            else if (x == 103)
            {
                await Browser.OpenAsync("https://www.tozan.co.jp/tzn-denso-nyushuko/d_deliverying?page=1111111111");
            }
            else if (x == 202)
            {
                Page page = new ScanBeforePage(new ScanBeforeViewModel(item.Text, item.Description));
                await Navigation.PushAsync(page);
            }
            else if (x == 203)
            {
                Page page = new ScanBeforePage(new ScanBeforeViewModel(item.Text, item.Description));
                await Navigation.PushAsync(page);
            }
            else if (x == 204)
            {
                //Page page = new ScanPage(new ScanViewModel(item.Text, item.Description, null, null, null));
                //await Navigation.PushAsync(page);

                //クリップボードモードとしてスキャン画面呼び出し(シングルトン)
                Page page = ScanReadPageClipBoard.GetInstance(item.Text, item.Description);
                await Navigation.PushAsync(page);
            }
            else if (x == 205)
            {
                Page page = new ScanBeforePage(new ScanBeforeViewModel(item.Text, item.Description));
                await Navigation.PushAsync(page);
            }
            else if ((104 <= x) && (x < 1000))
            {
                Page page = new ScanPage(new ScanViewModel(item.Text, item.Description, null, null, null, 0));
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