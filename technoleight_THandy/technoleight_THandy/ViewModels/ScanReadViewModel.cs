﻿using technoleight_THandy.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Linq;
using technoleight_THandy.Views;
using System.Text;
using Newtonsoft.Json;
using static technoleight_THandy.Models.Login;
using technoleight_THandy.common;
using static technoleight_THandy.Models.Receive;
using Color = Xamarin.Forms.Color;
using technoleight_THandy.Common;
using static technoleight_THandy.Models.ScanCommon;
using technoleight_THandy.Models.common;

namespace technoleight_THandy.ViewModels
{
    public abstract class ScanReadViewModel : BaseViewModel
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        public INavigation Navigation;

        ISimpleAudioPlayer SEplayer = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

        public int PageID;
        private bool ScanFlag;

        private LoginUserSqlLite LoginUser;

        private string DuplicateCheckStartReceiveDate { get; set; }
         
        private TempSaveScanData TempSaveScanItems { get; set; }

        public ICommand DataSendCommand { get; }
        public ICommand PageBackCommand { get; }
        public ICommand EndButtonCommand { get; }
        public ICommand ScanReceiveViewCommand { get; }
        public ICommand ScanReceiveTotalViewCommand { get; }

        public ICommand PackingCountInputOkeyActionCommand { get; }
        public ICommand PackingCountInputCancelActionCommand { get; }

        public ICommand NumberButtonClickCommand { get; }

        // 入荷済データ
        public List<ReceiveRegisteredData> ReceiveRegisteredDataList = new List<ReceiveRegisteredData>();

        // 在庫入庫の制約
        public List<Qrcode.M_StoreInConstraint> StoreInConstraintList = new List<Qrcode.M_StoreInConstraint>();

        // QRコードの制約
        public List<Qrcode.QrcodeIndex> QrcodeIndexList = new List<Qrcode.QrcodeIndex>();

        // 仮番地一覧
        public List<TemporaryStoreAddressModel> TemporaryStoreAddressList = new List<TemporaryStoreAddressModel>();

        // まとめ入庫対象品番一覧
        public List<ProductBulkStoreInModel> ProductBulkStoreInList = new List<ProductBulkStoreInModel>();

        ~ ScanReadViewModel()
        {
            Console.WriteLine("#ScanReadViewModel finish");
        }

        public ScanReadViewModel()
        {
            DataSendCommand = new Command(Touroku_Clicked);
            EndButtonCommand = new Command(PageBackEnd);
            PageBackCommand = new Command(PageBack);
            ScanReceiveViewCommand = new Command(ScanReceiveView);
            ScanReceiveTotalViewCommand = new Command(ScanReceiveTotalView);
            PackingCountInputOkeyActionCommand = new Command(PackingCountInputOkeyAction);
            PackingCountInputCancelActionCommand = new Command(PackingCountInputCancelAction);
            NumberButtonClickCommand = new Command(
                 (parameter) =>
                 {
                     string numberString = parameter.ToString();
                     if (numberString != "" && int.TryParse(numberString, out int number))
                     {
                         int.TryParse(InputPackingCountLabel.ToString() + numberString, out int setNumber);
                         InputPackingCountLabel = setNumber;
                     }
                     else if(numberString == "delete")
                     {
                         InputPackingCountLabel = 0;
                     }
                     else
                     {
                         return;
                     }
                 });
        }

        public async void Init(string title, int pageID, string receiveDate, INavigation navigation)
        {
            try
            {
                await Task.Run(() => ActivityRunningLoading());

                // 初期化
                ScanFlag = true;
                HeadMessage = "";
                // 初期値セット
                HeadMessage = title;
                Navigation = navigation;
                PageID = pageID;

                // 入庫日セット
                ReceiveDate = receiveDate;

                var loginUsers = await App.DataBase.GetLognAsync();
                if (loginUsers == null || loginUsers.Count != 1)
                {
                    await ErrorPageBack(null, "ログイン情報の取得に失敗しました。", null);
                    return;
                }
                else
                {
                    LoginUser = loginUsers[0];
                }

                await InitializeViewDesign();

                bool initializeViewDataResult = await InitializeViewData();
                if (!initializeViewDataResult)
                {
                    return;
                }

                await Task.Run(() => ActivityRunningEnd());

            }
            catch (Exception ex)
            {
                await ErrorPageBack();
                return;
            }
        }

        private async Task InitializeViewDesign()
        {
            // 箱数集計を初期表示
            await Task.Run(() => ScanReceiveTotalView());

            if (PageID == (int)Enums.PageID.Receive_StoreIn_AddressMatchCheck)
            {
                HeadMessageColor = (Color)App.TargetResource["PrimaryTextColor"];
            }
            else if (PageID == (int)Enums.PageID.Receive_StoreIn_TemporaryAddressMatchCheck)
            {
                HeadMessageColor = (Color)App.TargetResource["AccentTextColor"];
            }
            else if (PageID == (int)Enums.PageID.Receive_StoreIn_AddressMatchCheck_PackingCountInput)
            {
                HeadMessageColor = (Color)App.TargetResource["PrimaryTextColor"];
            }
            else if (PageID == (int)Enums.PageID.ReturnStoreAddress_AddressMatchCheck)
            {
                HeadMessageColor = (Color)App.TargetResource["PrimaryTextColor"];
            }
        }

        private void ScanReceiveView()
        {
            IsScanReceiveView = true;
            IsScanReceiveTotalView = false;
            ScanReceiveViewColor = (Color)App.TargetResource["MainColor"];
            ScanReceiveTotalViewColor = (Color)App.TargetResource["SecondaryButtonColor"];
        }

        private void ScanReceiveTotalView()
        {
            IsScanReceiveView = false;
            IsScanReceiveTotalView = true;
            ScanReceiveViewColor = (Color)App.TargetResource["SecondaryButtonColor"];
            ScanReceiveTotalViewColor = (Color)App.TargetResource["MainColor"];
        }

        private async Task<bool> GetStoreInConstraintList()
        {
            try
            {
                var getUrl = App.Setting.HandyApiUrl + "StoreInConstraint";
                getUrl = Util.AddCompanyPath(getUrl, App.Setting.CompanyID);
                getUrl = Util.AddParameter(getUrl, "depoID", LoginUser.DepoID.ToString());

                var responseMessage = await App.API.GetMethod(getUrl);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    StoreInConstraintList = JsonConvert.DeserializeObject<List<Qrcode.M_StoreInConstraint>>(responseMessage.content);
                    return true;
                }
                else if (responseMessage.status == System.Net.HttpStatusCode.NotFound)
                {
                    return true;
                }
                else
                {
                    await ErrorPageBack(null, responseMessage.content, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ErrorPageBack(null, Const.API_GET_ERROR_DEFAULT, null);
                return false;
            }

        }

        private async Task<bool> GetQrcodeIndexList()
        {
            try
            {
                var getUrl = App.Setting.HandyApiUrl + "Qrcode";
                getUrl = Util.AddCompanyPath(getUrl, App.Setting.CompanyID);
                getUrl = Util.AddParameter(getUrl, "depoID", LoginUser.DepoID.ToString());
                getUrl = Util.AddParameter(getUrl, "handyPageID", PageID.ToString());

                var responseMessage = await App.API.GetMethod(getUrl);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    QrcodeIndexList = JsonConvert.DeserializeObject<List<Qrcode.QrcodeIndex>>(responseMessage.content);
                    return true;
                }
                else
                {
                    await ErrorPageBack(null, responseMessage.content, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ErrorPageBack(null, Const.API_GET_ERROR_DEFAULT, null);
                return false;
            }

        }

        private async Task<bool> GetServerReceiveData()
        {
            ReceiveRegisteredDataList = new List<ReceiveRegisteredData>();

            // 登録済入荷データを取得する、「開始日」をセット
            if (DateTime.TryParse(ReceiveDate, out DateTime date))
            {
                DuplicateCheckStartReceiveDate = date.AddDays(-1).ToString("yyyy/MM/dd");
            }
            else
            {
                await ErrorPageBack(null, Const.ERROR_DEFAULT, null);
                return false;
            }

            // SqlServerから入荷済データをSELECT
            try
            {
                var getUrl = App.Setting.HandyApiUrl + "Receive";
                getUrl = Util.AddCompanyPath(getUrl, App.Setting.CompanyID);
                getUrl = Util.AddParameter(getUrl, "ReceiveDateStart", DuplicateCheckStartReceiveDate); // 開始日
                getUrl = Util.AddParameter(getUrl, "ReceiveDateEnd", ReceiveDate); // 終了日

                var responseMessage = await App.API.GetMethod(getUrl);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    ReceiveRegisteredDataList = JsonConvert.DeserializeObject<List<ReceiveRegisteredData>>(responseMessage.content);
                    return true;
                }
                else if (responseMessage.status == System.Net.HttpStatusCode.NotFound)
                {
                    return true;
                }
                else
                {
                    await ErrorPageBack(null, responseMessage.content, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ErrorPageBack(null, Const.API_GET_ERROR_DEFAULT, null);
                return false;
            }

        }

        private async Task<bool> GetTemporaryStoreAddressList()
        {
            try
            {
                var getUrl = App.Setting.HandyApiUrl + "TemporaryStoreAddress";
                getUrl = Util.AddCompanyPath(getUrl, App.Setting.CompanyID);
                getUrl = Util.AddParameter(getUrl, "depoID", LoginUser.DepoID.ToString());

                var responseMessage = await App.API.GetMethod(getUrl);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    TemporaryStoreAddressList = JsonConvert.DeserializeObject<List<TemporaryStoreAddressModel>>(responseMessage.content);
                    return true;
                }
                else
                {
                    await ErrorPageBack(null, responseMessage.content, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ErrorPageBack(null, Const.API_GET_ERROR_DEFAULT, null);
                return false;
            }

        }

        private async Task<bool> GetProductBulkStoreInList()
        {
            try
            {
                var getUrl = App.Setting.HandyApiUrl + "ProductBulkStoreIn";
                getUrl = Util.AddCompanyPath(getUrl, App.Setting.CompanyID);
                getUrl = Util.AddParameter(getUrl, "depoID", LoginUser.DepoID.ToString());

                var responseMessage = await App.API.GetMethod(getUrl);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    ProductBulkStoreInList = JsonConvert.DeserializeObject<List<ProductBulkStoreInModel>>(responseMessage.content);
                    return true;
                }
                else
                {
                    await ErrorPageBack(null, responseMessage.content, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ErrorPageBack(null, Const.API_GET_ERROR_DEFAULT, null);
                return false;
            }

        }

        private async Task ErrorPageBack(string title = null , string message = null, string buttonName = null) 
        {
            title = title ?? "エラー";
            message = message ?? Common.Const.SCAN_ERROR_DEFAULT;
            buttonName = buttonName ?? "OK";

            await Application.Current.MainPage.DisplayAlert(title, message, buttonName);
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
            return;
        }

        private async void Touroku_Clicked()
        {
            ScanFlag = false;

            List<ScanCommonApiPostRequestBody> receiveApiPostRequestsOkey = await App.DataBase.GetScanReceiveSendOkeyDataAsync(PageID, ReceiveDate);
            if (receiveApiPostRequestsOkey.Count == 0)
            {
                // 登録対象のデータが存在しない
                ScanFlag = true;
                return;
            }

            if (Util.StoreInFlag(PageID))
            {
                await Task.Run(() => ActivityRunningProcessing());

                List<ScanCommonApiPostRequestBody> receiveApiPostRequests = await App.DataBase.GetScanReceiveSendDataAsync(PageID, ReceiveDate);
                var registResult = await Common.ServerDataSending.ReceiveDataServerSendingExcute(receiveApiPostRequests);

                await Task.Run(() => ActivityRunningEnd());

                if (registResult.result)
                {
                    // データの削除を行う
                    await App.DataBase.DeleteScanReceive(PageID, ReceiveDate);
                    await App.DataBase.DeleteScanReceiveSendData(PageID, ReceiveDate);

                    bool result = await InitializeViewData();
                    if (result) { await RegistedOkeyAction(); }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    await App.DisplayAlertError(registResult.message);
                }

            }
            else if (PageID == (int)Enums.PageID.ReturnStoreAddress_AddressMatchCheck)
            {
                await Task.Run(() => ActivityRunningProcessing());

                List<ScanCommonApiPostRequestBody> receiveApiPostRequests = await App.DataBase.GetScanReceiveSendDataAsync(PageID, ReceiveDate);
                var registResult = await Common.ServerDataSending.ReturnStoreAddressDataServerSendingExcute(receiveApiPostRequests);

                await Task.Run(() => ActivityRunningEnd());

                if (registResult.result == Enums.ProcessResultPattern.Okey)
                {
                    // データの削除を行う
                    await App.DataBase.DeleteScanReceive(PageID, ReceiveDate);
                    await App.DataBase.DeleteScanReceiveSendData(PageID, ReceiveDate);

                    bool result = await InitializeViewData();
                    if (result) { await RegistedOkeyAction(); }
                    else
                    {
                        return;
                    }
                }
                else if (registResult.result == Enums.ProcessResultPattern.Alert)
                {
                    await App.DisplayAlertOkey(registResult.message, Const.ALERT_DEFAULT_TITLE, Const.ENTER_BUTTON);
                }
            }
            else
            {
                await App.DisplayAlertError();
            }

            ScanFlag = true;

        }

        public async void PageBack()
        {
            await Navigation.PopAsync();
        }

        public async void PageBackEnd()
        {
            var scanReceiveSendOkeyData = await App.DataBase.GetScanReceiveSendOkeyDataAsync(PageID, ReceiveDate);
            if (scanReceiveSendOkeyData.Count > 0)
            {
                var result = await Application.Current.MainPage.DisplayAlert(Const.ALERT_DEFAULT_TITLE, "未登録データが存在します\n戻ってよろしいですか？", "はい", "いいえ");
                if (result)
                {
                    PageBack();
                }
            }
            else
            {
                PageBack();
            }
            return;
        }

        public async Task ScanCountUp()
        {
            var scanReceiveSendOkeyData = await App.DataBase.GetScanReceiveSendOkeyDataAsync(PageID, ReceiveDate);
            ScanCount = scanReceiveSendOkeyData.Count;
        }

        private async Task<bool> InitializeViewData()
        {
            // Viewのデータ関係を初期化
            ScannedCode = "";
            Address1 = "";
            Address2 = "";
            InputQuantityCountLabel = 0;
            InputPackingCountLabel = 0;
            ScanReceiveViews = new ObservableCollection<ReceiveViewModel>();
            ScanReceiveTotalViews = new ObservableCollection<ReceiveTotalViewModel>();

            try
            {

                // QRコードマスタを取得
                var getQrcodeIndexList = await GetQrcodeIndexList();
                if (!getQrcodeIndexList)
                {
                    throw new CustomExtention();
                }

                // 入荷済データを取得
                if (PageID != (int)Enums.PageID.ReturnStoreAddress_AddressMatchCheck) // 番地戻し処理以外
                {
                    // 入庫済データを取得
                    var getServerReceiveData = await GetServerReceiveData();
                    if (!getServerReceiveData)
                    {
                        throw new CustomExtention();
                    }
                }

                // 在庫入庫制約データを取得
                var getStoreInConstraintList = await GetStoreInConstraintList();
                if (!getStoreInConstraintList)
                {
                    throw new CustomExtention();
                }

                // 仮番地マスタを取得
                if (PageID == (int)Enums.PageID.Receive_StoreIn_TemporaryAddressMatchCheck)
                {
                    var getTemporaryStoreAddressList = await GetTemporaryStoreAddressList();
                    if (!getTemporaryStoreAddressList)
                    {
                        throw new CustomExtention();
                    }
                }

                // まとめ入庫品番リストを取得
                if (PageID == (int)Enums.PageID.Receive_StoreIn_AddressMatchCheck_PackingCountInput)
                {
                    var getProductBulkStoreInList = await GetProductBulkStoreInList();
                    if (!getProductBulkStoreInList)
                    {
                        throw new CustomExtention();
                    }
                }

                await ScanCountUp();
                await ListView();

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private async Task<int> ListView()
        {

            List<Qrcode.QrcodeItem> scanReceiveList = await App.DataBase.GetScanReceiveAsync(PageID, ReceiveDate);

            if (scanReceiveList.Count > 0)
            {
                // 読取順に並び替える
                scanReceiveList = new ObservableCollection<Qrcode.QrcodeItem>(scanReceiveList
                    .OrderBy(o => o.ScanTime))
                    .ToList();

                try
                {
                    for (int x = 0; x <= scanReceiveList.Count - 1; x++)
                    {
                        var ReceiveView = new ReceiveViewModel();
                        ReceiveView.ProductCode = scanReceiveList[x].ProductCode;
                        ReceiveView.ProductLabelBranchNumber = scanReceiveList[x].ProductLabelBranchNumber;
                        ReceiveView.LotQuantity = scanReceiveList[x].Quantity;
                        ReceiveView.PackingCount = scanReceiveList[x].InputPackingCount;
                        ReceiveView.NextProcess1 = scanReceiveList[x].NextProcess1;
                        ReceiveView.NextProcess2 = scanReceiveList[x].NextProcess2;
                        ReceiveView.StoreInAddress1 = scanReceiveList[x].ScanStoreAddress1;
                        ReceiveView.StoreInAddress2 = scanReceiveList[x].ScanStoreAddress2;
                        ScanReceiveViews.Add(ReceiveView);
                    }

                    // 集計側
                    if (ScanReceiveViews.Count > 0)
                    {
                        var scanReceiveViewsGroupSelect = ScanReceiveViews
                            .GroupBy(x => new { x.ProductCode, x.LotQuantity, x.StoreInAddress1, x.StoreInAddress2 })
                            .Select(x => new { x.Key.ProductCode, x.Key.LotQuantity, x.Key.StoreInAddress1, x.Key.StoreInAddress2, PackingTotalCount = x.Sum(c => c.PackingCount) });
                        foreach (var item in scanReceiveViewsGroupSelect)
                        {
                            var scanReceiveTotalView = new ReceiveTotalViewModel();
                            scanReceiveTotalView.ProductCode = item.ProductCode;
                            scanReceiveTotalView.LotQuantity = item.LotQuantity;
                            scanReceiveTotalView.PackingTotalCount = item.PackingTotalCount;
                            scanReceiveTotalView.StoreInAddress1 = item.StoreInAddress1;
                            scanReceiveTotalView.StoreInAddress2 = item.StoreInAddress2;
                            ScanReceiveTotalViews.Add(scanReceiveTotalView);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await App.DisplayAlertError("スキャンデータ表示エラー");
                }

            }

            return 1;
        }

        public async Task UpdateReadData(string strScannedCode, string strScanMode)
        {
            if (MainThread.IsMainThread)
            {
                await UpdateReadDataOnMainThread(strScannedCode, strScanMode);
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await UpdateReadDataOnMainThread(strScannedCode, strScanMode);
                });
            }
        }

        public async Task UpdateReadDataOnMainThread(string strScannedCode, string strScanMode)
        {
            // 読取処理

            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("#UpdateReadDataOnMainThread Start {0} {1} {2}", strScannedCode, ScanFlag.ToString(), id.ToString());

            // データ登録キーの場合
            if (strScannedCode == Const.SCAN_EXECUTION_KEY_STRING_1)
            {
                Touroku_Clicked();
                return;
            }

            if (ScanFlag)
            {

                ScanFlag = false;
                //this.IsAnalyzing = false;  //読み取り停止
                //FrameVisible = true;       //Frameを表示

                string ID = strScannedCode;

                // ----------------------------------------------------------------------------
                double latitude, longitude; // 緯度、経度
                latitude = 0.0;
                longitude = 0.0;

                //// 位置情報をセット
                //var location = await Util.GetLocationInformation();
                //latitude = location.latitude;
                //longitude = location.longitude;
                // ----------------------------------------------------------------------------

                try
                {

                    #region 番地QR処理
                    if (ID.StartsWith(Common.Const.SCAN_ADDRESS_START_STRING_1) || ID.StartsWith(Common.Const.SCAN_ADDRESS_START_STRING_2))
                    {
                        var scanStringArray = ID.Split(':');

                        if (scanStringArray.Length == 2 && !String.IsNullOrEmpty(scanStringArray[1]))
                        {
                            string address1 = "";
                            string address2 = scanStringArray[1].Trim();

                            // 仮番地入庫の場合は、仮番地マスタに登録済の番地かをチェック
                            if (PageID == (int)Enums.PageID.Receive_StoreIn_TemporaryAddressMatchCheck)
                            {
                                var temporaryStoreAddress = TemporaryStoreAddressList.Where(x => x.TemporaryStoreAddress2 == address2).ToList().FirstOrDefault();
                                if (temporaryStoreAddress == null)
                                {
                                    await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.AddressError, Common.Const.SCAN_ERROR_INCORRECT_ADDRESS_QR);
                                    return;
                                }
                                else
                                {
                                    // 番地セット
                                    Address1 = temporaryStoreAddress.TemporaryStoreAddress1;
                                    Address2 = temporaryStoreAddress.TemporaryStoreAddress2;
                                }
                            }
                            else
                            {
                                // 通常の番地セット
                                Address1 = address1;
                                Address2 = address2;
                            }

                            // 番地セット完了アクション
                            await SetAddressAction();
                        }
                        else
                        {
                            await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.AddressError, Common.Const.SCAN_ERROR_INCORRECT_ADDRESS_QR);
                        }

                        return;

                    }
                    else if (Address2 == "")
                    {
                        // 番地スキャンではないが、番地未セットの場合
                        await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.AddressError, Common.Const.SCAN_ERROR_NOT_SET_ADDRESS);
                        return;
                    }
                    #endregion

                    #region 製品かんばんQR処理
                    var scanData = new Qrcode.QrcodeItem();

                    var getQrcodeItem = await Qrcode.GetQrcodeItem(ID, QrcodeIndexList);
                    if (!getQrcodeItem.result)
                    {
                        await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.ConversionFailedError, getQrcodeItem.message);
                        return;
                    }

                    // スキャン情報セット
                    scanData = getQrcodeItem.item;

                    // スキャン情報と番地をチェック
                    if (PageID == (int)Enums.PageID.Receive_StoreIn_TemporaryAddressMatchCheck) // 仮番地入庫処理
                    {
                        // 番地の【不一致】をチェック
                        if (scanData.Location1 == Address2)
                        {
                            //  仮番地しか登録できないので、【一致】だったらエラー　もう一度番地をスキャンする
                            await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.AddressError, Common.Const.SCAN_ERROR_MATCH_ADDRESS);
                            Address2 = "";
                            return;
                        }
                    }
                    else
                    {
                        // 通常の番地照合
                        // 番地の【一致】をチェック
                        if (scanData.Location1 != Address2)
                        {
                            // 【不一致】だったらエラー、もう一度番地をスキャンする
                            await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.AddressError, Common.Const.SCAN_ERROR_NOT_MATCH_ADDRESS);
                            Address2 = "";
                            return;
                        }
                    }

                    // 箱数入力型の場合は、まとめ入庫対象品番かをチェック
                    if (PageID == (int)Enums.PageID.Receive_StoreIn_AddressMatchCheck_PackingCountInput)
                    {
                        var productBulkStoreInList = ProductBulkStoreInList.Where(x => x.ProductCode == scanData.ProductCode).ToList().FirstOrDefault();
                        if (productBulkStoreInList == null)
                        {
                            await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.NotBulkStoreInError, Common.Const.SCAN_ERROR_NOT_BULK_STOREIN_PRODUCT);
                            return;
                        }
                    }

                    // スキャン付属情報セット
                    scanData.PageID = PageID;
                    scanData.ProcessceDate = ReceiveDate;
                    scanData.ScanStoreAddress1 = Address1;
                    scanData.ScanStoreAddress2 = Address2;
                    scanData.ScanTime = DateTime.Now;

                    // 未送信のスキャン済みデータに同じデータが存在しないかチェック
                    List<ScanCommonApiPostRequestBody> scanDataListToday = await App.DataBase.GetReceiveSendOkeyDataAsync(ID);
                    if (scanDataListToday.Count > 0)
                    {
                        await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.DuplicationError, Common.Const.SCAN_ERROR_DUPLICATION);
                        return;
                    }

                    // 在庫対象かチェック
                    var checkStoreInConstraint = await Qrcode.CheckStoreInConstraint(scanData, StoreInConstraintList);
                    if (!checkStoreInConstraint.result)
                    {
                        await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.ExcludedScanError, Common.Const.SCAN_ERROR_NOT_STOCK);
                        return;
                    }

                    if (PageID  != (int)Enums.PageID.ReturnStoreAddress_AddressMatchCheck) // 番地戻し処理以外
                    {

                        // 登録済み入荷データと重複が無いかチェック
                        if (ReceiveRegisteredDataList.Count > 0)
                        {
                            var scanReceiveExists = ReceiveRegisteredDataList.Exists(x =>
                                x.SupplierCode == scanData.SupplierCode
                                && x.SupplierClass == scanData.SupplierClass
                                && x.ProductCode == scanData.ProductCode
                                && x.ProductLabelBranchNumber == scanData.ProductLabelBranchNumber
                                && x.Quantity == scanData.Quantity // ロット数
                                && x.Packing == scanData.Packing
                                && x.NextProcess1 == scanData.NextProcess1
                                && x.NextProcess2 == scanData.NextProcess2);
                            if (scanReceiveExists)
                            {
                                await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.DuplicationError, Common.Const.SCAN_ERROR_REGIST_DUPLICATION);
                                return;
                            }
                        }

                    }

                    var tempScanSaveData = new TempSaveScanData();
                    tempScanSaveData.Latitude = latitude;
                    tempScanSaveData.Longitude = longitude;
                    tempScanSaveData.ScanString = ID;
                    tempScanSaveData.ScanData = scanData;

                    if (PageID == (int)Enums.PageID.Receive_StoreIn_AddressMatchCheck_PackingCountInput)
                    {
                        // スキャンデータを一時保存
                        TempSaveScanItems = tempScanSaveData;

                        // 箱数入力ダイアログを開く
                        await Task.Run(() => { PackingCountInputOpenAction(); });
                        return;
                    }
                    else
                    {
                        // 箱数を入力しない場合は、自動的に１をセット
                        tempScanSaveData.ScanData.InputPackingCount = 1;

                        // スキャンOK
                        await ScanDataViewAndSave(tempScanSaveData);
                        await OkeyAction();
                        return;
                    }

                }
                catch (Exception ex)
                {
                    await ScanErrorAction(ID, latitude, longitude, Enums.HandyOperationClass.OtherError);
                    return;
                }

            }

            #endregion
        
        }

        private async Task ScanDataViewAndSave(TempSaveScanData tempSaveScanData)
        {
            var temp = tempSaveScanData;

            // オリジナルQR形の作成
            string createScanString = "";

            // SqlServer登録用データ作成
            StringBuilder createScanStringSb = new StringBuilder("");
            createScanStringSb.Append(temp.ScanData.DeleveryDate); // 0
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.DeliveryTimeClass); // 1
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.DataClass); // 2
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.OrderClass); // 3
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.DeliverySlipNumber); // 4
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.SupplierCode); // 5
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.SupplierClass); // 6
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.ProductCode); // 7
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.ProductAbbreviation); // 8
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.ProductLabelBranchNumber); // 9
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.Quantity); // 10
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.NextProcess1); // 11
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.Location1); // 12
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.NextProcess2); // 13
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.Packing); // 14
            createScanStringSb.Append(":");
            createScanStringSb.Append(temp.ScanData.Location2); // 15
            createScanString = createScanStringSb.ToString();

            // 画面表示スキャン済データ作成

            // 履歴側ーーー
            var ReceiveView = new ReceiveViewModel();
            ReceiveView.ProductCode = temp.ScanData.ProductCode;
            ReceiveView.ProductLabelBranchNumber = temp.ScanData.ProductLabelBranchNumber;
            ReceiveView.LotQuantity = temp.ScanData.Quantity;
            ReceiveView.PackingCount = temp.ScanData.InputPackingCount;
            ReceiveView.NextProcess1 = temp.ScanData.NextProcess1;
            ReceiveView.NextProcess2 = temp.ScanData.NextProcess2;
            ReceiveView.StoreInAddress1 = temp.ScanData.ScanStoreAddress1;
            ReceiveView.StoreInAddress2 = temp.ScanData.ScanStoreAddress2;
            // 画面リストに挿入
            ScanReceiveViews.Add(ReceiveView);

            // 集計側ーーー
            // 削除
            var removeScanReceiveTotalView = new ReceiveTotalViewModel();
            removeScanReceiveTotalView = ScanReceiveTotalViews.FirstOrDefault(
                x => x.ProductCode == temp.ScanData.ProductCode
                && x.LotQuantity == temp.ScanData.Quantity
                && x.StoreInAddress1 == temp.ScanData.ScanStoreAddress1
                && x.StoreInAddress2 == temp.ScanData.ScanStoreAddress2);
            if (removeScanReceiveTotalView != null)
            {
                ScanReceiveTotalViews.Remove(removeScanReceiveTotalView);
            }
            // 画面リストに挿入
            var insertScanReceiveTotalView = new ReceiveTotalViewModel();
            var scanReceiveViewsSelect = ScanReceiveViews.GroupBy(x => new { x.ProductCode, x.LotQuantity, x.StoreInAddress1, x.StoreInAddress2 })
            .Where(x => x.Key.ProductCode == temp.ScanData.ProductCode && x.Key.LotQuantity == temp.ScanData.Quantity && x.Key.StoreInAddress1 == temp.ScanData.ScanStoreAddress1 && x.Key.StoreInAddress2 == temp.ScanData.ScanStoreAddress2)
            .Select(x => new { x.Key.ProductCode, x.Key.LotQuantity, x.Key.StoreInAddress1, x.Key.StoreInAddress2, PackingTotalCount = x.Sum(c => c.PackingCount) }).First();
            insertScanReceiveTotalView.ProductCode = scanReceiveViewsSelect.ProductCode;
            insertScanReceiveTotalView.LotQuantity = scanReceiveViewsSelect.LotQuantity;
            insertScanReceiveTotalView.PackingTotalCount = scanReceiveViewsSelect.PackingTotalCount;
            insertScanReceiveTotalView.StoreInAddress1 = scanReceiveViewsSelect.StoreInAddress1;
            insertScanReceiveTotalView.StoreInAddress2 = scanReceiveViewsSelect.StoreInAddress2;
            ScanReceiveTotalViews.Add(insertScanReceiveTotalView);

            // スキャンデータを保存
            await App.DataBase.SaveScanReceiveAsync(temp.ScanData);

            // サーバー送信用スキャンデータを保存
            var sendReceiveData = new ScanCommonApiPostRequestBody();
            sendReceiveData.ProcessDate = ReceiveDate;
            sendReceiveData.DuplicateCheckStartProcessDate = DuplicateCheckStartReceiveDate;
            sendReceiveData.DepoID = LoginUser.DepoID;
            sendReceiveData.HandyPageID = PageID;
            sendReceiveData.HandyOperationClass = (int)Enums.HandyOperationClass.Okey;
            sendReceiveData.HandyOperationMessage = "";
            sendReceiveData.ScanString1 = temp.ScanString;
            sendReceiveData.ScanString2 = "";
            sendReceiveData.ScanChangeData = createScanString;
            sendReceiveData.HandyUserID = App.Setting.HandyUserID;
            sendReceiveData.ScanTime = temp.ScanData.ScanTime;
            sendReceiveData.Device = App.Setting.Device;
            sendReceiveData.Latitude = temp.Latitude;
            sendReceiveData.Longitude = temp.Longitude;
            sendReceiveData.StoreInFlag = Util.StoreInFlag(PageID);
            // Input関係
            sendReceiveData.InputQuantity = InputQuantityCountLabel; // 数量入力
            sendReceiveData.InputPackingCount = InputPackingCountLabel; // 箱数入力
            sendReceiveData.ScanStoreAddress1 = Address1; // 入庫番地１
            sendReceiveData.ScanStoreAddress2 = Address2; // 入庫番地2

            await App.DataBase.SaveScanReceiveSendDataAsync(sendReceiveData);

            await ScanCountUp();

            return;
        }

        private async Task OkeyAction(string okeyMessage = Common.Const.SCAN_OKEY)
        {
            // OKアクション
            SEplayer.Load(Util.GetStreamFromFile(App.Setting.ScanOkeySound));
            SEplayer.Play();
            ColorState = Color.DarkGray;
            ScannedCode = okeyMessage;

            await Task.Delay(300);    // 待機

            //this.IsAnalyzing = true;   // スキャン再開
            ScanFlag = true;
        }

        private async Task SetAddressAction(string okeyMessage = Common.Const.SCAN_OKEY_SET_ADDRESS)
        {
            // ダイアログ表示
            DialogTitleText = Const.SCAN_ADDRESS_TITLE_TEXT;
            DialogMainText = Address2;
            DialogSubText = Const.SCAN_ADDRESS_SUB_TEXT;
            DialogSubTextIsVisible = true;
            BackgroundLayerIsVisible = true;
            DialogIsVisible = true;

            SEplayer.Load(Util.GetStreamFromFile(App.Setting.ScanOkeySound));
            SEplayer.Play();
            ColorState = Color.DarkGray;
            ScannedCode = okeyMessage;

            // 待機
            await Task.Delay(1000);

            //this.IsAnalyzing = true;   // スキャン再開

            // スキャン再開
            ScanFlag = true;

            // 待機
            await Task.Delay(500);

            // ポップアップ閉じる
            BackgroundLayerIsVisible = false;
            DialogIsVisible = false;
        }

        private async Task RegistedOkeyAction()
        {
            // ダイアログ表示
            DialogTitleText = Const.OKEY_DEFAULT_TITLE;
            DialogMainText = Const.OKEY_DEFAULT_REGISTED_MESSAGE;
            DialogSubText = "";
            DialogSubTextIsVisible = false;
            BackgroundLayerIsVisible = true;
            DialogIsVisible = true;

            SEplayer.Load(Util.GetStreamFromFile(App.Setting.ScanOkeySound));
            SEplayer.Play();

            // 待機
            await Task.Delay(1300);

            //this.IsAnalyzing = true;   // スキャン再開

            // スキャン再開
            ScanFlag = true;

            // 待機
            await Task.Delay(500);

            // ポップアップ閉じる
            BackgroundLayerIsVisible = false;
            DialogIsVisible = false;
        }

        private void PackingCountInputOpenAction()
        {
            // ダイアログ表示
            BackgroundLayerIsVisible = true;
            Dialog2IsVisible = true;

            SEplayer.Load(Util.GetStreamFromFile(App.Setting.ScanOkeySound));
            SEplayer.Play();
        }

        private async void PackingCountInputOkeyAction()
        {
            // 箱数が未入力または0の場合はエラーを返す
            if (InputPackingCountLabel == 0)
            {
                await InputErrorMessageAction("", TempSaveScanItems.Longitude, TempSaveScanItems.Longitude, Enums.HandyOperationClass.InputError, Common.Const.INPUT_ERROR_REQUIRED_PACKING_COUNT);
                return;
            }

            var inputNumberOkey = await Application.Current.MainPage.DisplayAlert(
                "確認", 
                "箱数を【" + InputPackingCountLabel + "】で登録します。\nよろしいですか？", 
                "はい", 
                "いいえ");

            if (!inputNumberOkey)
            {
                return;
            }

            // 箱数分のデータをセット
            TempSaveScanItems.ScanData.InputPackingCount = InputPackingCountLabel;
            await ScanDataViewAndSave(TempSaveScanItems);

            // OK
            await OkeyAction(Common.Const.INPUT_OKEY_SET_PACKING_COUNT);

            // 箱数を０に戻す
            InputPackingCountLabel = 0;

            // ポップアップ閉じる
            BackgroundLayerIsVisible = false;
            Dialog2IsVisible = false;
        }

        private void PackingCountInputCancelAction()
        {
            ScannedCode = "";
            InputPackingCountLabel = 0;

            // ポップアップ閉じる
            BackgroundLayerIsVisible = false;
            Dialog2IsVisible = false;

            // スキャン再開
            ScanFlag = true;
        }

        private async Task InputErrorMessageAction(string scanString, double latitude, double longitude, Enums.HandyOperationClass handyScanClass, string errorMessage = Common.Const.INPUT_ERROR_DEFAULT)
        {
            // ダイアログ表示
            PackingCountInputErrorMessage = errorMessage;

            await ScanErrorDataSave(errorMessage, scanString, latitude, longitude, handyScanClass);
            await InputErrorAction();

            // 待機
            await Task.Delay(2000);

            // ポップアップ閉じる
            PackingCountInputErrorMessage = "";
        }

        private async Task InputErrorAction()
        {
            // バイブレーションとサウンドを設定
            SEplayer.Load(Util.GetStreamFromFile(App.Setting.ScanErrorSound));

            Vibration.Vibrate();
            SEplayer.Play();
            await Task.Delay(300);    // 待機
            SEplayer.Play();
            Vibration.Cancel();
        }

        private async Task ScanErrorAction(string scanString, double latitude, double longitude, Enums.HandyOperationClass handyScanClass, string errorMessage = Common.Const.SCAN_ERROR_DEFAULT)
        {
            ColorState = (Color)App.TargetResource["AccentTextColor"];
            ScannedCode = errorMessage;

            await ScanErrorDataSave(errorMessage, scanString, latitude, longitude, handyScanClass);

            // バイブレーションとサウンドを設定
            SEplayer.Load(Util.GetStreamFromFile(App.Setting.ScanErrorSound));

            Vibration.Vibrate();
            SEplayer.Play();
            await Task.Delay(300);    // 待機
            SEplayer.Play();
            Vibration.Cancel();

            await Task.Delay(500);    // 待機

            //IsAnalyzing = true;   // スキャン再開
            ScanFlag = true;
        }

        private async Task ScanErrorDataSave(string errorMessage, string scanString, double latitude, double longitude, Enums.HandyOperationClass handyScanClass)
        {
            // サーバー送信用スキャンデータを保存
            var sendReceiveData = new ScanCommonApiPostRequestBody();
            sendReceiveData.ProcessDate = ReceiveDate;
            sendReceiveData.DuplicateCheckStartProcessDate = DuplicateCheckStartReceiveDate;
            sendReceiveData.DepoID = LoginUser.DepoID;
            sendReceiveData.HandyPageID = PageID;
            sendReceiveData.HandyOperationClass = (int)handyScanClass;
            sendReceiveData.HandyOperationMessage = errorMessage;
            sendReceiveData.ScanString1 = scanString;
            sendReceiveData.ScanString2 = "";
            sendReceiveData.ScanChangeData = "";
            sendReceiveData.HandyUserID = App.Setting.HandyUserID;
            sendReceiveData.ScanTime = DateTime.Now;
            sendReceiveData.Device = App.Setting.Device;
            sendReceiveData.Latitude = latitude;
            sendReceiveData.Longitude = longitude;
            sendReceiveData.StoreInFlag = Util.StoreInFlag(PageID);
            // Input関係
            sendReceiveData.InputQuantity = InputQuantityCountLabel; // 数量入力
            sendReceiveData.InputPackingCount = InputPackingCountLabel; // 箱数入力
            sendReceiveData.ScanStoreAddress1 = Address1; // 入庫番地１
            sendReceiveData.ScanStoreAddress2 = Address2; // 入庫番地2

            await App.DataBase.SaveScanReceiveSendDataAsync(sendReceiveData);

            return;
        }

        private string scannedCode = "";
        public string ScannedCode
        {
            get { return scannedCode; }
            set { SetProperty(ref scannedCode, value); }
        }

        private int scanCount;
        public int ScanCount
        {
            get { return scanCount; }
            set { SetProperty(ref scanCount, value); }
        }

        private Color colorState;
        public Color ColorState
        {
            get { return colorState; }
            set { SetProperty(ref colorState, value); }
        }

        private ObservableCollection<ReceiveViewModel> scanReceiveViews;
        public ObservableCollection<ReceiveViewModel> ScanReceiveViews
        {
            get { return scanReceiveViews; }
            set { SetProperty(ref scanReceiveViews, value); }
        }

        private ObservableCollection<ReceiveTotalViewModel> scanReceiveTotalViews;
        public ObservableCollection<ReceiveTotalViewModel> ScanReceiveTotalViews
        {
            get { return scanReceiveTotalViews; }
            set { SetProperty(ref scanReceiveTotalViews, value); }
        }

        private string address1 = "";
        public string Address1
        {
            get { return address1; }
            set { SetProperty(ref address1, value); }
        }

        private string address2 = "";
        public string Address2
        {
            get { return address2; }
            set { SetProperty(ref address2, value); }
        }

        private string receiveDate;
        public string ReceiveDate
        {
            get { return receiveDate; }
            set { SetProperty(ref receiveDate, value); }
        }

        private string headMessage;
        public string HeadMessage
        {
            get { return headMessage; }
            set { SetProperty(ref headMessage, value); }
        }

        private Color headMessageColor;
        public Color HeadMessageColor
        {
            get { return headMessageColor; }
            set { SetProperty(ref headMessageColor, value); }
        }

        private bool dialog2IsVisible;
        public bool Dialog2IsVisible
        {
            get { return dialog2IsVisible; }
            set { SetProperty(ref dialog2IsVisible, value); }
        }

        private int inputQuantityCountLabel;
        public int InputQuantityCountLabel
        {
            get { return inputQuantityCountLabel; }
            set { SetProperty(ref inputQuantityCountLabel, value); }
        }

        private int inputPackingCountLabel;
        public int InputPackingCountLabel
        {
            get { return inputPackingCountLabel; }
            set { SetProperty(ref inputPackingCountLabel, value); }
        }

        private string packingCountInputErrorMessage;
        public string PackingCountInputErrorMessage
        {
            get { return packingCountInputErrorMessage; }
            set { SetProperty(ref packingCountInputErrorMessage, value); }
        }

        private bool isScanReceiveView;
        public bool IsScanReceiveView
        {
            get { return isScanReceiveView; }
            set { SetProperty(ref isScanReceiveView, value); }
        }

        private bool isScanReceiveTotalView;
        public bool IsScanReceiveTotalView
        {
            get { return isScanReceiveTotalView; }
            set { SetProperty(ref isScanReceiveTotalView, value); }
        }

        private Color scanReceiveViewColor;
        public Color ScanReceiveViewColor
        {
            get { return scanReceiveViewColor; }
            set { SetProperty(ref scanReceiveViewColor, value); }
        }

        private Color scanReceiveTotalViewColor;
        public Color ScanReceiveTotalViewColor
        {
            get { return scanReceiveTotalViewColor; }
            set { SetProperty(ref scanReceiveTotalViewColor, value); }
        }

        private bool canReadClipBoard;
        public bool CanReadClipBoard
        {
            get { return canReadClipBoard; }
            set { SetProperty(ref canReadClipBoard, value); }
        }

        private bool canReceiveBarcode;
        public bool CanReceiveBarcode
        {
            get { return canReceiveBarcode; }
            set { SetProperty(ref canReceiveBarcode, value); }
        }

        private bool gridVisible;
        public bool GridVisible
        {
            get { return gridVisible; }
            set { SetProperty(ref gridVisible, value); }
        }

        private bool isAnalyzing;
        public bool IsAnalyzing
        {
            get { return isAnalyzing; }
            set { SetProperty(ref isAnalyzing, value); }
        }

        private bool frameVisible;
        public bool FrameVisible
        {
            get { return frameVisible; }
            set { SetProperty(ref frameVisible, value); }
        }

        private string strName;
        public string StrName
        {
            get { return strName; }
            set { SetProperty(ref strName, value); }
        }

        private string strUuid;
        public string StrUuid
        {
            get { return strUuid; }
            set { SetProperty(ref strUuid, value); }
        }

        private string strState;
        public string StrState
        {
            get { return strState; }
            set { SetProperty(ref strState, value); }
        }

    }
}
