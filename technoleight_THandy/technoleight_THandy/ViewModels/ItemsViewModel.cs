using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using technoleight_THandy.Models;
using technoleight_THandy.Views;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using technoleight_THandy.common;
using static technoleight_THandy.Models.ScanCommon;
using technoleight_THandy.Common;

namespace technoleight_THandy.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        //public ObservableCollection<MenuX> Items { get; set; }
        //public Command LoadItemsCommand { get; set; }
        //public ICommand AppearingCommand { get; }
        public ICommand NotSendViewCommand { get; }
        public ICommand NotSendDataIsSendingCommand { get; }

        public INavigation Navigation;

        public ItemsViewModel()
        {
            ActivityRunningLoading();

            // メイン画面を起動
            Title = "メインメニュー";
            Items = new ObservableCollection<MenuX>();
            Task.Run(async () => { await LoadItemsCommand(); }).Wait();

            NotSendViewCommand = new Command(async () =>
            {
                await NotSendDataViewCommand();
            });
            NotSendDataIsSendingCommand = new Command<int>(
                    execute: async (int id) =>
                    {
                        await NotSendDataIsSendingByPageID(id);
                    });

            ActivityRunningEnd();
        }

        public async void ItemSelected()
        {
            try
            {
                if (Util.StoreInFlag(SelectedItems.HandyPageID) || SelectedItems.HandyPageID == (int) Enums.PageID.ReturnStoreAddress_AddressMatchCheck)
                {
                    Page page = new ScanStoreInPage(SelectedItems.HandyPageID, SelectedItems.HandyPageName);
                    await Navigation.PushAsync(page);
                }
            }
            catch (Exception ex)
            {

            }
        }

        ObservableCollection<MenuX> items = new ObservableCollection<MenuX>();
        public ObservableCollection<MenuX> Items
        {
            get { return items; }
            set
            { SetProperty(ref items, value); }
        }

        MenuX selectedItems = new MenuX();
        public MenuX SelectedItems
        {
            get { return selectedItems; }
            set
            {
                SetProperty(ref selectedItems, value);
                if (value != null && value.HandyPageID > 0)
                {
                    ItemSelected();
                }
            }
        }

        string userName = "";
        public string UserName
        {
            get { return userName; }
            set
            { SetProperty(ref userName, value); }
        }

        string depoName = "";
        public string DepoName
        {
            get { return depoName; }
            set
            { SetProperty(ref depoName, value); }
        }

        bool isNotSendAlert = false;
        public bool IsNotSendAlert
        {
            get { return isNotSendAlert; }
            set
            { SetProperty(ref isNotSendAlert, value); }
        }

        ObservableCollection<NotSendDataGroup> notSendDataGroupList = new ObservableCollection<NotSendDataGroup>();
        public ObservableCollection<NotSendDataGroup> NotSendDataGroupList
        {
            get { return notSendDataGroupList; }
            set
            { SetProperty(ref notSendDataGroupList, value); }
        }

        bool isNotSendDataList = false;
        public bool IsNotSendDataList
        {
            get { return isNotSendDataList; }
            set
            { SetProperty(ref isNotSendDataList, value); }
        }

        bool navigationPageIsVisible = true;
        public bool NavigationPageIsVisible
        {
            get { return navigationPageIsVisible; }
            set
            { SetProperty(ref navigationPageIsVisible, value); }
        }

        public Task NotSendDataViewCommand()
        {
            if (IsNotSendDataList)
            {
                NavigationPageIsVisible = true;
                IsNotSendDataList = false;
                //NotSendAlertIcon = "#xf103;";
            }
            else
            {
                NavigationPageIsVisible = false;
                IsNotSendDataList = true;
                //NotSendAlertIcon = "&#xf102;";
            }

            return Task.CompletedTask;
        }

        private async Task NotSendDataIsSendingByPageID(int pageID)
        {
            List<ScanCommonApiPostRequestBody> receiveApiPostRequestsOkey = await App.DataBase.GetScanReceiveSendOkeyDataAsync(pageID);
            if (receiveApiPostRequestsOkey.Count == 0)
            {
                await App.DisplayAlertError("対象データが存在しません");
                return;
            }

            var menuName = Items.Where(x => x.HandyPageID == pageID).Select(x => x.HandyPageName).ToList().FirstOrDefault();
            string message = "【" + menuName + "メニュー】\nスキャン済データを登録します\nよろしいですか？";

            var result = await Application.Current.MainPage.DisplayAlert("確認", message, "はい", "いいえ");

            if (result)
            {
                await Task.Run(() => ActivityRunningProcessing());

                List<ScanCommonApiPostRequestBody> receiveApiPostRequests = await App.DataBase.GetScanReceiveSendDataAsync(pageID);

                (bool, string) registResult;
                if (Util.StoreInFlag(pageID))
                {
                    registResult = await Common.ServerDataSending.ReceiveDataServerSendingExcute(receiveApiPostRequests);
                }
                else
                {
                    registResult = await Common.ServerDataSending.ReturnStoreAddressDataServerSendingExcute(receiveApiPostRequests);
                }

                await Task.Run(() => ActivityRunningEnd());

                if (registResult.Item1)
                {
                    // データの削除を行う
                    await App.DataBase.DeleteScanReceive(pageID);
                    await App.DataBase.DeleteScanReceiveSendData(pageID);

                    await App.DisplayAlertOkey(registResult.Item2);

                    // Topメニューに戻る
                    Application.Current.MainPage = new MainPage();
                }
                else
                {
                    await App.DisplayAlertError(registResult.Item2);
                }

            }

            return;
        }

        public async Task LoadItemsCommand()
        {
            if (IsBusy)
                return;
            //不具合がでるので　IsBusy停止
            //IsBusy = true;

            try
            {
                Items.Clear();
                List<Login.LoginUserSqlLite> loginUserSqlLites = await App.DataBase.GetLognAsync();
                if (loginUserSqlLites.Count > 0)
                {
                    UserName = loginUserSqlLites[0].HandyUserName;
                    DepoName = loginUserSqlLites[0].DepoName;
                }

                //SQLiteより設定ファイルを抽出
                List<MenuX> menux = await App.DataBase.GetMenuAsync();
                if (menux.Count > 0)
                {
                    Items = new ObservableCollection<MenuX>(menux);
                    int viewPageNumber = 0;
                    foreach (var item in Items)
                    {
                        viewPageNumber++;
                        item.HandyPageNo = viewPageNumber;
                    }

                    await NotSendDataCheckAsync();

                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task NotSendDataCheckAsync()
        {
            NotSendDataGroupList = new ObservableCollection<NotSendDataGroup>();

            foreach (var item in items)
            {
                // Errorのみの未送信データは、削除する
                List<ScanCommonApiPostRequestBody> receiveOkeyDataListAll = await App.DataBase.GetScanReceiveSendDataAsync(item.HandyPageID);
                var receiveOkeyDataListAllReceiveDataGroup = receiveOkeyDataListAll.GroupBy(x => x.ProcessDate).ToList();

                foreach (var receivesAll in receiveOkeyDataListAllReceiveDataGroup)
                {
                    List<ScanCommonApiPostRequestBody> receivesError = await App.DataBase.GetScanReceiveSendErrorDataAsync(item.HandyPageID, receivesAll.Key);
                    if (receivesAll.Count() == receivesError.Count)
                    {
                        await App.DataBase.DeleteScanReceiveSendData(item.HandyPageID, receivesAll.Key);
                    }
                }

                List<ScanCommonApiPostRequestBody> receiveOkeyDataList = await App.DataBase.GetScanReceiveSendOkeyDataAsync(item.HandyPageID);
                if (receiveOkeyDataList.Count > 0)
                {
                    var notSendDataList = new List<NotSendData>();
                    var results = receiveOkeyDataList.GroupBy(x => x.ProcessDate).ToList();

                    foreach (var result in results)
                    {
                        var key = result.Key;

                        //id++;
                        var notSendData = new NotSendData();

                        //notSendData.Id= id;
                        notSendData.ProcessDate = key;
                        notSendData.PageID = item.HandyPageID;
                        notSendData.DataCount = result.Count();

                        notSendDataList.Add(notSendData);
                    }
                    var notSendDataGroup = new NotSendDataGroup(item.HandyPageID, item.HandyPageName, notSendDataList);
                    NotSendDataGroupList.Add(notSendDataGroup);
                }

            }

            if (NotSendDataGroupList.Count > 0)
            {
                IsNotSendAlert = true;
            }
        }


    }
}