using technoleight_THandy.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.SimpleAudioPlayer;
using System.IO;
using System;
using System.Text.RegularExpressions;
using technoleight_THandy;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Linq;
using technoleight_THandy.Views;
using System.Text;
using Newtonsoft.Json;

namespace technoleight_THandy.ViewModels
{
    public abstract class ScanReadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation m_navigation;

        ISimpleAudioPlayer SEplayer = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

        /// <summary>
        /// ボタン処理中に画面遷移させない制御用
        /// </summary>       
        public bool btnFanction;

        public string Readkubun;
        public string nameA;
        private static bool flag = true;    // 非同期でも共有されるようにstaticにする。
        public bool bCompletedDsp = false;

        private string soundOkey { get; set; }
        private string soundError { get; set; }

        private static Setei UserSetting;
        public PageItem PageItem;

        public ICommand DataSendCommand { get; }
        public ICommand PageBackCommand { get; }
        public ICommand EndButtonCommand { get; }
        public ICommand ScanReceiptViewCommand { get; }
        public ICommand ScanReceiptTotalViewCommand { get; }

        ~ ScanReadViewModel()
        {
            Console.WriteLine("#ScanReadViewModel finish");
        }

        public ScanReadViewModel()
        {
            DataSendCommand = new Command(Touroku_Clicked);
            EndButtonCommand = new Command(PageBackEnd);
            PageBackCommand = new Command(PageBack);
            ScanReceiptViewCommand = new Command(ScanReceiptView);
            ScanReceiptTotalViewCommand = new Command(ScanReceiptTotalView);
        }

        public async void init(string name1, string kubun, string receiptDate, INavigation navigation)
        {
            m_navigation = navigation;

            btnFanction = false;

            ContentIsVisible = false;
            ActivityRunning = true;

            IsScanReceiptView = false;
            IsScanReceiptTotalView = false;

            Readkubun = kubun;
            nameA = name1;
            ScannedCode = "";
            //Title = nameA.Replace("画面", "") + "　読取";
            HName = nameA;
            FrameVisible = true;       //Frameを表示
            GridVisible = true;

            var settingList = await App.DataBase.GetSeteiAsync();
            if (settingList == null || settingList.Count > 1)
            {
                await Application.Current.MainPage.DisplayAlert("エラー", "ユーザー設定の取得に失敗しました。", "OK");
                return;
            }
            else
            {
                UserSetting = settingList[0];
            }

            var okSound = UserSetting.ScanOkeySound;
            var errSound = UserSetting.ScanErrorSound;

            if (!string.IsNullOrEmpty(okSound) && !string.IsNullOrEmpty(errSound))
            {
                soundOkey = UserSetting.ScanOkeySound;
                soundError = UserSetting.ScanErrorSound;
            }
            else
            {
                Sound sound = new Sound();

                soundOkey = sound.SoundOkeyList.FirstOrDefault().Item;
                soundError = sound.SoundErrorList.FirstOrDefault().Item;
            }

            if (Readkubun == "206")
            {
                //await SetPageItem();
                ScanReceiptTotalView();

                ReceiptDate = receiptDate;
                await GetServerReceiptData();
            }

            await houji();

            ActivityRunning = false;
            ContentIsVisible = true;
        }

        private void ScanReceiptView()
        {
            IsScanReceiptView = true;
            IsScanReceiptTotalView = false;
            ScanReceiptViewColor = "Teal";
            ScanReceiptTotalViewColor = "LightGray";
        }

        private void ScanReceiptTotalView()
        {
            IsScanReceiptView = false;
            IsScanReceiptTotalView = true;
            ScanReceiptViewColor = "LightGray";
            ScanReceiptTotalViewColor = "Teal";
        }

        private async Task GetServerReceiptData()
        {

            var barModel = new BarModel();
            barModel.Shori = "SELECT";
            barModel.WID = UserSetting.WID;
            barModel.UserID = UserSetting.user;
            barModel.Device = UserSetting.Device;
            barModel.Shorikubun = "206";
            barModel.ReceiptDate = ReceiptDate;

            // SqlServerから入庫日（本日）の入庫済データをSELECT
            try
            {
                var postList = new List<BarModel>();
                postList.Add(barModel);
                var postBack = await App.API.Post_method4(postList, UserSetting.url, "TechnolEight");
                var result = postBack[0]["Result"];
                if (result == "OK")
                {
                    var jsonData = JsonConvert.SerializeObject(postBack);
                    var postBackData = JsonConvert.DeserializeObject<List<ScanReceipt>>(jsonData);
                    await App.DataBase.SaveScanReceiptListAsync(postBackData);
                }
                else if (result == "0")
                {

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "入庫済データの取得に失敗しました。", "OK");
                }
            }
            catch (Exception ex)
            {

            }

        }

        private async void Touroku_Clicked()
        {
            List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(Readkubun);
            if (sagyoUsers.Count == 0)
            {
                return;
            }

            // ボタン押下チェック(連打対策)
            if (!btnFanction)
            {

                btnFanction = true; //ボタン押下不可
                //ContentIsVisible = false;
                //ActivityRunning = true;

                string message = "スキャンしたデータ登録します。よろしいですか？";
                if (Readkubun == "206")
                {
                    var boxCount = sagyoUsers.Count;
                    message = "\n【！】スキャン件数　" + boxCount + "　箱\n\n登録します。よろしいですか？\n";
                }

                var result = await Application.Current.MainPage.DisplayAlert("確認", message, "はい", "いいえ");
                if (result)
                {
                    await Touroku_Clicked_Excute();
                }

                //ActivityRunning = false;
                //ContentIsVisible = true;
                btnFanction = false; //ボタン押下可

            }
        }

        private async Task Touroku_Clicked_Excute()
        {
            //登録処理
            //画面の種類区分ごとに処理を行う
            int i = 0;
            List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(Readkubun);
            if (sagyoUsers.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("エラー", "スキャン済データがありません", "OK");
                return;
            }
            else
            {
                //ユーザー情報抽出
                string WID = "";
                string url = "";
                string k_pass = "";
                string user = "";
                string Device = "";

                WID = UserSetting.WID;
                url = UserSetting.url;
                k_pass = UserSetting.k_pass;
                user = UserSetting.user;
                Device = UserSetting.Device;

                // ----------------------------------------------------------------------------
                double latitude, longitude;//緯度、経度
                latitude = 0.0;
                longitude = 0.0;
                try
                {
                    // ネットワークが切れた状態で緯度経度を取得すると返ってこなくなり、
                    // エラーをキャッチするが、長い時間反応がないので、
                    // 事前に接続チェックする。
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        //GPSの精度指定
                        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
                        //経緯度の取得
                        var location = await Geolocation.GetLocationAsync(request);

                        latitude = location.Latitude;
                        longitude = location.Longitude;
                    }
                }
                catch
                {
                    latitude = 0.0;
                    longitude = 0.0;
                }
                // ----------------------------------------------------------------------------

                var postList = new List<BarModel>();
                for (int x = 0; x <= sagyoUsers.Count - 1; x++)
                {
                    var postItem = new BarModel();
                    postItem.Shori = "REGIST";
                    postItem.WID = WID;
                    postItem.UserID = user;
                    postItem.Device = Device;
                    postItem.Shorikubun = Readkubun;
                    postItem.BarcodeRead = sagyoUsers[x].Scanstring;
                    postItem.BarcodeRead1 = sagyoUsers[x].Scanstring2;
                    postItem.Sryou = sagyoUsers[x].Sryou.ToString();
                    postItem.Cdate1 = sagyoUsers[x].cdate.ToString();
                    postItem.latitude = latitude.ToString();
                    postItem.longitude = longitude.ToString();
                    postItem.ReceiptDate = ReceiptDate;

                    postList.Add(postItem);
                }

                //WEBサーバーにUpdate
                //SQLserver登録
                List<Dictionary<string, string>> items2 = await App.API.Post_method4(postList, UserSetting.url, "TechnolEight");
                if (items2.Count > 0)
                {
                    try
                    {
                        string dictValue;
                        if (true == items2[0].TryGetValue(key: Common.Const.C_ERR_KEY_NETWORK, value: out dictValue))
                        {
                            // ネットワークエラー
                            await Application.Current.MainPage.DisplayAlert("エラー", "ネットワーク接続に失敗しました。", "OK");
                            return;
                        }
                        else if (items2[0]["Result"].ToString() == "NG")
                        {
                            // 登録エラー
                            var message = items2[0]["Message"];
                            await Application.Current.MainPage.DisplayAlert("エラー", message, "OK");
                            return;
                        }
                        else if (items2[0]["Result"].ToString() == "OK")
                        {
                            if (Readkubun == "206")
                            {
                                // 読取データの削除を行う
                                await App.DataBase.DeleteAllScanReadData();
                                await App.DataBase.DeleteAllScanReceipt();

                                // 最新の入庫済データを取得
                                await GetServerReceiptData();

                                ScannedCode = "";

                                //await Application.Current.MainPage.DisplayAlert("完了", "登録が完了しました。", "OK");
                                var result = await Application.Current.MainPage.DisplayAlert("完了", "登録が完了しました", "メニューに戻る", "続けてスキャン");
                                if (result)
                                {
                                    for (var counter = 1; counter < 3; counter++)
                                    {
                                        m_navigation.RemovePage(m_navigation.NavigationStack[m_navigation.NavigationStack.Count - 1]);
                                    }
                                }
                            }

                            await houji();

                            return;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("エラー", "予期せぬエラーが発生しました。", "OK");
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "予期せぬエラーが発生しました。", "OK");
                        await houji();

                        return;

                    }

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "予期せぬエラーが発生しました。", "OK");
                    return;
                }

            }

        }

        public async void PageBack()
        {
            //// 切断処理
            //if (UserSetting.ScanMode == Common.Const.C_SCANNAME_BARCODE && StrState == Common.Const.C_CONNET_OK)
            //{
            //    ScanReadBarcodeViewModel.GetInstance().BTDisConnet();
            //}
            //else if (UserSetting.ScanMode == Common.Const.C_SCANNAME_CLIPBOARD)
            //{
            //    ScanReadClipBoardViewModel.GetInstance().DisposeEvent();
            //}
            //else if (UserSetting.ScanMode == Common.Const.C_SCANNAME_CAMERA)
            //{
            //    IsAnalyzing = false;
            //    ScanReadCameraViewModel.GetInstance().OnScan = null;
            //}

            await m_navigation.PopAsync();
        }

        public async void PageBackEnd()
        {
            if (UserSetting.ScanMode == Common.Const.C_SCANNAME_CAMERA)
            {
                IsAnalyzing = false;
            }

            List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(Readkubun);
            if (sagyoUsers.Count > 0)
            {
                var result = await Application.Current.MainPage.DisplayAlert("警告", "未登録のスキャン済データは削除されます。戻ってよろしいですか？", "はい", "いいえ");
                if (result)
                {
                    PageBack();
                }
                else
                {
                    if (UserSetting.ScanMode == Common.Const.C_SCANNAME_CAMERA)
                    {
                        await Task.Delay(1000);    //1秒待機
                        IsAnalyzing = true;
                    }
                }

            }
            else
            {
                PageBack();
            }


            return;
        }

        public async Task houji()
        {
            await ReadCountUp();
            int HoujiResult = await Houji();
            IsAnalyzing = true;   //読み取り開始
        }

        public async Task ReadCountUp()
        {
            int cou = await App.DataBase.GetScanReadDataAsync4(Readkubun);
            ScannedCode2 = cou.ToString();
        }

        private async Task<int> Houji()
        {

            if (Readkubun == "206") // 在庫入庫
            {
                ScanReceiptViews.Clear();
                ScanReceiptTotalViews.Clear();

                List<ScanReceipt> scanReceipts = await App.DataBase.GetScanReceiptAsync();

                if (scanReceipts.Count > 0)
                {
                    // 履歴側
                    ObservableCollection<ScanReceipt> scanReceiptViews = new ObservableCollection<ScanReceipt>();
                    try
                    {
                        for (int x = 0; x <= scanReceipts.Count - 1; x++)
                        {
                            if (scanReceipts[x].NotRegistFlag)
                            {
                                var viewData = scanReceipts[x];
                                scanReceiptViews.Add(viewData);
                            }
                        }

                        if (scanReceiptViews.Count > 0)
                        {
                            // 読取順に並び替える
                            scanReceiptViews = new ObservableCollection<ScanReceipt>(scanReceiptViews.OrderBy(o => o.ScanTime));

                            ScanReceiptViews = scanReceiptViews;
                        }


                        // 集計側
                        if (ScanReceiptViews.Count > 0)
                        {
                            var scanReceiptViewsGroupSelect = ScanReceiptViews.GroupBy(x => new { x.ProductCode, x.ReceiptQuantity })
                                .Select(x => new { x.Key.ProductCode, x.Key.ReceiptQuantity, PackingCount = x.Count() });
                            foreach (var item in scanReceiptViewsGroupSelect)
                            {
                                var scanReceiptTotalView = new ScanReceiptTotal();
                                scanReceiptTotalView.ProductCode = item.ProductCode;
                                scanReceiptTotalView.ReceiptQuantity = item.ReceiptQuantity;
                                scanReceiptTotalView.PackingCount = item.PackingCount;
                                ScanReceiptTotalViews.Add(scanReceiptTotalView);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await Application.Current.MainPage.DisplayAlert("エラー", "結果表示エラー", "OK");
                    }

                }
            }
            else
            {

                ReadData = "";
                StringBuilder sb = new StringBuilder("");
                int readCount = 0;

                //readkubn 画面番号
                switch (Readkubun)
                {
                    case "202":
                    case "205":
                        List<Nouhin> nouhin202 = await App.DataBase.GetNouhinAsync();
                        if (nouhin202.Count > 0)
                        {
                            try
                            {
                                for (int x = 0; x <= nouhin202.Count - 1; x++)
                                {
                                    if (nouhin202[x].IsRead)
                                    {
                                        string buno = nouhin202[x].VIBUNO.ToString().Trim();
                                        string sryo = nouhin202[x].VISRYO.ToString().Trim();
                                        string nsryo = nouhin202[x].JJNKSU.ToString().Trim();
                                        string yosu = nouhin202[x].VIYOSU.ToString().Trim();
                                        string lotsu = nouhin202[x].VILOSU.ToString().Trim();

                                        string OK = "";
                                        if (sryo == nsryo)
                                        {
                                            OK = "ＯＫ!";
                                        }

                                        readCount += 1;
                                        var RowNo = readCount.ToString() + ".";

                                        //
                                        Title = RowNo + buno + "　" + nsryo + "／" + sryo + "　" + lotsu + "×" + yosu + "　　" + OK;

                                        if (sb.ToString() != "")
                                        {
                                            sb.Append("\r\n");
                                        }
                                        sb.Append(Title);

                                    }
                                }

                                ReadData = sb.ToString();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                await Application.Current.MainPage.DisplayAlert("エラー", "読取結果表示エラー", "OK");
                            }

                        }

                        if (ReadData == "")
                        {
                            //ReadData = "読取結果を表示します。";

                            //var list = new ObservableCollection<PageTypeGroup>();
                            //list.Add(new PageTypeGroup { Title = "aaa", ShortName = "iii"});
                            //MyListViewItems = list;
                        }

                        break;
                    case "204":

                        List<NouhinJL> nouhin204 = await App.DataBase.GetNouhinJLAsync();
                        List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(Readkubun);

                        if (nouhin204.Count > 0)
                        {
                            try
                            {
                                string buno = nouhin204[0].JLBUNO.ToString().Trim();
                                string sryo = nouhin204[0].JLNKSU.ToString().Trim();
                                string bar = sagyoUsers[0].Scanstring.ToString().Trim();
                                DkeyPrintData1 = "品番：" + buno + " 数量：" + sryo;
                                DkeyPrintData2 = bar;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                await Application.Current.MainPage.DisplayAlert("エラー", "読取結果表示エラー", "OK");
                            }

                        }
                        else
                        {
                            DkeyPrintData1 = "";
                            DkeyPrintData2 = "";
                        }

                        break;
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
            //読取処理

            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("#UpdateReadDataOnMainThread Start {0} {1} {2}", strScannedCode, flag.ToString(), id.ToString());
            if (flag)
            {

                flag = false;
                this.IsAnalyzing = false;  //読み取り停止
                FrameVisible = true;       //Frameを表示

                //処理の開始

                //ScannedCode = strScannedCode;

                //読取実績の整合性検証                                            
                string ID = strScannedCode;
                string ID2 = strScannedCode;

                //設定ファイルの読取
                List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
                string WID = "";
                string user = "";
                if (Set2.Count > 0)
                {
                    WID = Set2[0].WID;
                    user = Set2[0].user;
                }

                // ----------------------------------------------------------------------------
                double latitude, longitude;//緯度、経度
                latitude = 0.0;
                longitude = 0.0;

                // ----------------------------------------------------------------------------
                
                // 現品票QR形の作成
                string createBarcode = "";

                //テクノエイト　入庫かんばん読みの場合
                if (Readkubun == "202")
                {
                    List<Nouhin> nouhin = await App.DataBase.GetNouhinAsync();

                    string letters0_3= ID.Substring(0, 3);

                    //正しいかんばんデータか　最初にK01がついてるか
                    if (letters0_3 != "K01")
                    {
                        await ErrorAction("QRコードが不正です");
                        return;
                    }

                    //トヨテツコードの位置で、通常品か支給品かを判断
                    //トヨテツコードがなければ、臨時かんばん
                    string toytetsuCode = "3999";
                    string letters11_4 = ID.Substring(11-1, 4);
                    string letters4_4 = ID.Substring(4-1, 4);

                    string hinban = "";
                    string suryo = "";
                    string edaban = "";
                    string ukeire = "";

                    // 通常品
                    if (letters11_4 == toytetsuCode)
                    {
                        hinban = ID.Substring(45-1, 20).Trim();
                        suryo = ID.Substring(29-1, 5).Trim();

                        // 現在は未使用項目
                        edaban = ID.Substring(25 - 1, 4).Trim();
                        ukeire = ID.Substring(34 - 1, 2).Trim();
                    }
                    // 支給品
                    else if(letters4_4 == toytetsuCode)
                    {
                        hinban = ID.Substring(44-1, 20).Trim();
                        suryo = ID.Substring(66-1, 5).Trim();
                    }
                    // 臨時
                    else
                    {
                        //　品番と数量の位置は通常品と同じ
                        hinban = ID.Substring(45 - 1, 20).Trim();
                        suryo = ID.Substring(29 - 1, 5).Trim();
                    }

                    //二回目以降 ２度読み検証
                    //DBに読取実績が存在するかチェック
                    List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(Readkubun);

                    for (int x = 0; x <= sagyoUsers.Count - 1; x++)
                    {
                        if (sagyoUsers[x].Scanstring == ID)
                        {
                            await ErrorAction("重複です");
                            return;
                        }
                    }

                    // 読取数量の数値化
                    int read_suryo = 0;
                    if (int.TryParse(suryo, out read_suryo))
                    {
                        bool Matched = false;

                        //数量更新
                        for (int x = 0; x <= nouhin.Count - 1; x++)
                        {
                            string hinban_shizi = nouhin[x].VIBUNO.ToString().Trim();
                            string suryo_shizi = nouhin[x].VISRYO.ToString().Trim();
                            string lot_shizi = nouhin[x].VILOSU.ToString().Trim();

                            // 品番とロット数が一致するものがあれば、その納番に割り当てていく
                            if (hinban == hinban_shizi && read_suryo.ToString() == lot_shizi)
                            {
                                int total_nyuko_suryo = 0;

                                // 入庫数
                                int nyuko_suryo = 0;
                                if (int.TryParse(nouhin[x].JJNKSU, out nyuko_suryo)) { }
                                // 指示数
                                int shizi_suryo = 0;
                                if (int.TryParse(suryo_shizi, out shizi_suryo)) { }

                                total_nyuko_suryo = read_suryo + nyuko_suryo;

                                //入庫数が指示数を超えてなかったらデータを更新
                                if (shizi_suryo >= total_nyuko_suryo)
                                {
                                    var noku = nouhin[x].VHNOKU;
                                    var nose = nouhin[x].VHNOSE;
                                    var nono = nouhin[x].VHNONO;
                                    int line = 0;
                                    if (int.TryParse(nouhin[x].VILINE.ToString().Trim(), out line)) { }
                                    int seq = 0;
                                    if (int.TryParse(nouhin[x].JLSTARTSEQ.ToString().Trim(), out seq)) { }

                                    // パッケージ現品票QR型の値を作成
                                    seq = seq + 1;
                                    createBarcode = noku + ":" + nose + ":" + nono + ":" + String.Format("{0:D3}", line) + ":" + String.Format("{0:D3}", seq) + ":" + String.Format("{0:D5}", read_suryo);

                                    nouhin[x].JLSTARTSEQ = seq;
                                    nouhin[x].JJNKSU = total_nyuko_suryo.ToString();
                                    nouhin[x].IsRead = true;

                                    int OKa = await App.DataBase.SaveNouhinAsync(nouhin[x]);
                                    Matched = true;
                                    break;
                                }
                            }
                        }

                        if (Matched == false)
                        {
                            await ErrorAction("該当データがありません");
                            return;
                        }

                    }
                    else
                    {
                        await ErrorAction("数量の読取不可");
                        return;
                    }

                }
                else if (Readkubun == "206") // 在庫入庫処理
                {
                    string letters0_3 = ID.Substring(0, 3);

                    // 正しいかんばんデータか　最初にK01がついてるか
                    if (letters0_3 != "K01")
                    {
                        await ErrorAction("QRコードが不正です");
                        return;
                    }

                    string supplierCode = "";
                    string productCode = "";
                    string receiptQuantity = "";
                    string productLabelBranchNumber = "";
                    string nextProcess1 = "";
                    string nextProcess2 = "";
                    string packing = "";
                    string location1 = "";
                    string productAbbreviation = "";

                    supplierCode = ID.Substring(4 - 1, 7).Trim();
                    productCode = ID.Substring(45 - 1, 20).Trim();
                    receiptQuantity = ID.Substring(29 - 1, 5).Trim();
                    productLabelBranchNumber = ID.Substring(25 - 1, 4).Trim();
                    nextProcess1 = ID.Substring(85 - 1, 4).Trim();
                    nextProcess2 = ID.Substring(34 - 1, 2).Trim();
                    packing = ID.Substring(67 - 1, 17).Trim();
                    location1 = ID.Substring(89 - 1, 4).Trim();
                    productAbbreviation = ID.Substring(40 - 1, 4).Trim();

                    // 正しいかんばんデータか　在庫対象であるか
                    if (nextProcess2 != "T1" && nextProcess2 != "T3")
                    {
                        await ErrorAction("在庫対象ではありません");
                        return;
                    }

                    // スキャン内容
                    string scanSupplierCode = supplierCode;
                    string scanProductCode = productCode;
                    string scanNextProcess1 = nextProcess1;
                    string scanNextProcess2 = nextProcess2;
                    string scanPacking = packing;
                    int scanReceiptQuantity = 0;
                    int scanProductLabelBranchNumber = 0;

                    // 数値が必要なものは変換
                    if ((!int.TryParse(receiptQuantity, out scanReceiptQuantity)) || (!int.TryParse(productLabelBranchNumber, out scanProductLabelBranchNumber)))
                    {
                        await ErrorAction("数値変換エラー");
                        return;
                    }

                    // 同じ入庫日で既にスキャンしたデータが無いかチェック
                    List<ScanReceipt> scanReceipts = await App.DataBase.GetScanReceiptAsync();
                    var scanReceiptExists = scanReceipts.Exists(x =>
                    x.SupplierCode == scanSupplierCode
                    && x.ProductCode == scanProductCode 
                    && x.ProductLabelBranchNumber == scanProductLabelBranchNumber 
                    && x.ReceiptQuantity == scanReceiptQuantity 
                    && x.NextProcess1 == scanNextProcess1
                    && x.NextProcess2 == scanNextProcess2);

                    if (scanReceiptExists)
                    {
                        await ErrorAction("スキャン済のかんばんです");
                        return;
                    }

                    // SqlServer登録用データ作成
                    StringBuilder createBarcodeSb = new StringBuilder("");
                    createBarcodeSb.Append(scanSupplierCode);
                    createBarcodeSb.Append(":");
                    createBarcodeSb.Append(scanProductCode);
                    createBarcodeSb.Append(":");
                    createBarcodeSb.Append(scanProductLabelBranchNumber);
                    createBarcodeSb.Append(":");
                    createBarcodeSb.Append(scanReceiptQuantity);
                    createBarcodeSb.Append(":");
                    createBarcodeSb.Append(scanNextProcess1);
                    createBarcodeSb.Append(":");
                    createBarcodeSb.Append(scanNextProcess2);
                    createBarcodeSb.Append(":");
                    createBarcodeSb.Append(scanPacking);
                    createBarcodeSb.Append(":");
                    createBarcodeSb.Append(location1);
                    createBarcodeSb.Append(":");
                    createBarcodeSb.Append(productAbbreviation);
                    createBarcode = createBarcodeSb.ToString();

                    // 画面表示スキャン済データ作成

                    // 履歴側ーーー
                    var scanReceiptView = new ScanReceipt();
                    scanReceiptView.SupplierCode = scanSupplierCode;
                    scanReceiptView.ProductCode = scanProductCode;
                    scanReceiptView.ProductLabelBranchNumber = scanProductLabelBranchNumber;
                    scanReceiptView.ReceiptQuantity = scanReceiptQuantity;
                    scanReceiptView.NextProcess1 = scanNextProcess1;
                    scanReceiptView.NextProcess2 = scanNextProcess2;
                    scanReceiptView.Packing = scanPacking;
                    scanReceiptView.NotRegistFlag = true;
                    scanReceiptView.ScanTime = DateTime.Now;
                    // 画面リストに挿入
                    ScanReceiptViews.Add(scanReceiptView);

                    // 集計側ーーー
                    // 削除
                    var removeScanReceiptTotal = new ScanReceiptTotal();
                    removeScanReceiptTotal = ScanReceiptTotalViews.FirstOrDefault(x => x.ProductCode == scanProductCode && x.ReceiptQuantity == scanReceiptQuantity);
                    if (removeScanReceiptTotal != null)
                    {
                        ScanReceiptTotalViews.Remove(removeScanReceiptTotal);
                    }
                    // 画面リストに挿入
                    var insertScanReceiptTotal = new ScanReceiptTotal();
                    var scanReceiptViewsSelect = ScanReceiptViews.GroupBy(x => new { x.ProductCode, x.ReceiptQuantity })
                                .Where(x => x.Key.ProductCode == scanProductCode && x.Key.ReceiptQuantity == scanReceiptQuantity)
                                .Select(x => new { x.Key.ProductCode, x.Key.ReceiptQuantity, PackingCount = x.Count() }).First();
                    insertScanReceiptTotal.ProductCode = scanReceiptViewsSelect.ProductCode;
                    insertScanReceiptTotal.ReceiptQuantity = scanReceiptViewsSelect.ReceiptQuantity;
                    insertScanReceiptTotal.PackingCount = scanReceiptViewsSelect.PackingCount;
                    ScanReceiptTotalViews.Add(insertScanReceiptTotal);

                    // 端末内にスキャンデータを保存
                    int saveScanReceipt = await App.DataBase.SaveScanReceiptAsync(scanReceiptView);

                }
                else
                {
                    await ErrorAction("予期せぬエラー");
                    return;
                }

                // OKアクション
                SEplayer.Load(GetStreamFromFile(soundOkey));
                SEplayer.Play();
                ColorState = Color.DarkGray;
                ScannedCode = "スキャンOK";

                // DBに書き込み
                ScanReadData Sdata = new ScanReadData();
                Sdata.Kubn = Readkubun;
                Sdata.Scanstring = ID;
                Sdata.Scanstring2 = createBarcode;
                Sdata.Sryou = 0;
                Sdata.cuser = user;
                Sdata.cdate = DateTime.Now;
                Sdata.latitude = latitude;
                Sdata.longitude = longitude;

                int OK1 = await App.DataBase.SaveScanReadDataAsync(Readkubun, Sdata);
                //await houji();
                await ReadCountUp();

                // 高速化対応で高速化が必要なものは除外
                if (!strScanMode.Equals(Common.Const.C_SCANMODE_BARCODE) && !strScanMode.Equals(Common.Const.C_SCANMODE_CLIPBOARD))
                { 
                    await Task.Delay(1000);    //1秒待機
                }
                this.IsAnalyzing = true;   //読み取り再開
                flag = true;
            }
        }

        private async Task ErrorAction(string errorMessage)
        {
            ColorState = ColorConverters.FromHex("#F4504D");
            ScannedCode = errorMessage;

            // バイブレーションとサウンドを設定
            var duration = TimeSpan.FromSeconds(1);
            SEplayer.Load(GetStreamFromFile(soundError));

            Vibration.Vibrate();
            SEplayer.Play();
            await Task.Delay(300);    //1秒待機
            SEplayer.Play();
            Vibration.Cancel();

            await Task.Delay(500);    //1秒待機

            IsAnalyzing = true;   //読み取り再開
            flag = true;
        }

        private Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            //ビルドアクションで埋め込みリソースにしたファイルを取ってくる
            var stream = assembly.GetManifestResourceStream(filename);
            return stream;
        }

        private bool isAnalyzing;
        public bool IsAnalyzing
        {
            get { return isAnalyzing; }
            set
            {
                if (isAnalyzing != value)
                {
                    isAnalyzing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAnalyzing)));
                }
            }
        }

        private string scannedCode;
        public string ScannedCode
        {
            get { return scannedCode; }
            set
            {
                if (scannedCode != value)
                {
                    scannedCode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                }
            }
        }

        private string scannedCode2;
        public string ScannedCode2
        {
            get { return scannedCode2; }
            set
            {
                if (scannedCode2 != value)
                {
                    scannedCode2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode2)));
                }
            }
        }

        private string hname;
        public string HName
        {
            get { return hname; }
            set
            {
                if (hname != value)
                {
                    hname = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HName)));
                }
            }
        }

        private bool frameVisible;
        public bool FrameVisible
        {
            get { return frameVisible; }
            set
            {
                if (frameVisible != value)
                {
                    frameVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FrameVisible)));
                }
            }
        }

        private bool gridVisible;
        public bool GridVisible
        {
            get { return gridVisible; }
            set
            {
                if (gridVisible != value)
                {
                    gridVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GridVisible)));
                }
            }
        }

        private string txtCode;
        public string TxtCode
        {
            get { return txtCode; }
            set
            {
                if (txtCode != value)
                {
                    txtCode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TxtCode)));
                }
            }
        }

        private ObservableCollection<string> scanModeItems;
        public ObservableCollection<string> ScanModeItems
        {
            get { return scanModeItems; }
            set
            {
                if (scanModeItems != value)
                {
                    scanModeItems = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScanModeItems)));
                }
            }
        }

        private int scanModeSelectedIndex;
        public int ScanModeSelectedIndex
        {
            get { return scanModeSelectedIndex; }
            set
            {
                if (scanModeSelectedIndex != value)
                {
                    scanModeSelectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScanModeSelectedIndex)));
                }
            }
        }

        private string strTitle;
        public string StrTitle
        {
            get { return this.strTitle; }
            set
            {
                if (this.strTitle != value)
                {
                    this.strTitle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StrTitle)));
                }
            }
        }

        private bool txtCodeFocuse;
        public bool TxtCodeFocuse
        {
            get { return this.txtCodeFocuse; }
            set
            {
                if (this.txtCodeFocuse != value)
                {
                    this.txtCodeFocuse = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TxtCodeFocuse)));
                }
            }
        }

        private string strName;
        public string StrName
        {
            get { return this.strName; }
            set
            {
                if (this.strName != value)
                {
                    this.strName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StrName)));
                }
            }
        }

        public string strUuid;
        public string StrUuid
        {
            get { return this.strUuid; }
            set
            {
                if (this.strUuid != value)
                {
                    this.strUuid = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StrUuid)));
                }
            }
        }

        private string strState;
        public string StrState
        {
            get { return this.strState; }
            set
            {
                if (this.strState != value)
                {
                    this.strState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StrState)));
                }
            }
        }

        private Color colorState;
        public Color ColorState
        {
            get { return this.colorState; }
            set
            {
                if (this.colorState != value)
                {
                    this.colorState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColorState)));
                }
            }
        }

        private bool canReceiveBarcode;
        public bool CanReceiveBarcode
        {
            get { return this.canReceiveBarcode; }
            set
            {
                if (this.canReceiveBarcode != value)
                {
                    this.canReceiveBarcode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanReceiveBarcode)));
                }
            }
        }

        private bool canReadClipBoard;
        public bool CanReadClipBoard
        {
            get { return this.canReadClipBoard; }
            set
            {
                if (this.canReadClipBoard != value)
                {
                    this.canReadClipBoard = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanReadClipBoard)));
                }
            }
        }
        private string readData;
        public string ReadData
        {
            get { return readData; }
            set
            {
                if (readData != value)
                {
                    readData = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReadData)));
                }
            }
        }
        //private ListView myListView;
        //public ListView MyListView
        //{
        //    get { return myListView; }
        //    set
        //    {
        //        if (myListView != value)
        //        {
        //            myListView = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyListView)));
        //        }
        //    }
        //}
        private ObservableCollection<PageTypeGroup> myListViewItems;
        public ObservableCollection<PageTypeGroup> MyListViewItems
        {
            get { return myListViewItems; }
            set
            {
                if (myListViewItems != value)
                {
                    myListViewItems = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyListViewItems)));
                }
            }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
                }
            }
        }
        //private string printBtnFlg;
        //public string PrintBtnFlg
        //{
        //    get { return printBtnFlg; }
        //    set
        //    {
        //        if (printBtnFlg != value)
        //        {
        //            printBtnFlg = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrintBtnFlg)));
        //        }
        //    }
        //}
        private string dkeyPrintData1;
        public string DkeyPrintData1
        {
            get { return dkeyPrintData1; }
            set
            {
                if (dkeyPrintData1 != value)
                {
                    dkeyPrintData1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DkeyPrintData1)));
                }
            }
        }
        private string dkeyPrintData2;
        public string DkeyPrintData2
        {
            get { return dkeyPrintData2; }
            set
            {
                if (dkeyPrintData2 != value)
                {
                    dkeyPrintData2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DkeyPrintData2)));
                }
            }
        }

        private ObservableCollection<ScanReceipt> scanReceiptViews = new ObservableCollection<ScanReceipt>();
        public ObservableCollection<ScanReceipt> ScanReceiptViews
        {
            get { return scanReceiptViews; }
            set
            {
                scanReceiptViews = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScanReceiptViews)));
            }
        }

        private ObservableCollection<ScanReceiptTotal> scanReceiptTotalViews = new ObservableCollection<ScanReceiptTotal>();
        public ObservableCollection<ScanReceiptTotal> ScanReceiptTotalViews
        {
            get { return scanReceiptTotalViews; }
            set
            {
                scanReceiptTotalViews = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScanReceiptTotalViews)));
            }
        }

        //private string shortName;
        //public string ShortName
        //{
        //    get { return shortName; }
        //    set
        //    {
        //        if (shortName != value)
        //        {
        //            shortName = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShortName)));
        //        }
        //    }
        //}
        //private PageTypeGroup gr;
        //public PageTypeGroup Gr
        //{
        //    get { return gr; }
        //    set
        //    {
        //        if (gr != value)
        //        {
        //            gr = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gr)));
        //        }
        //    }
        //}
        //private ObservableCollection<PageTypeGroup> myListViewItems;
        //public ObservableCollection<PageTypeGroup> MyListViewItems
        //{
        //    get { return myListViewItems; }
        //    set
        //    {
        //        if (myListViewItems != value)
        //        {
        //            myListViewItems = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyListViewItems)));
        //        }
        //    }
        //}

        private static bool activityRunning = false;
        public bool ActivityRunning
        {
            get { return activityRunning; }
            set
            {
                if (activityRunning != value)
                {
                    activityRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActivityRunning)));
                }
            }
        }

        private static string receiptDate;
        public string ReceiptDate
        {
            get { return receiptDate; }
            set
            {
                if (receiptDate != value)
                {
                    receiptDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReceiptDate)));
                }
            }
        }

        private static bool contentIsVisible = false;
        public bool ContentIsVisible
        {
            get { return contentIsVisible; }
            set
            {
                if (contentIsVisible != value)
                {
                    contentIsVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContentIsVisible)));
                }
            }
        }

        private static bool isScanReceiptView;
        public bool IsScanReceiptView
        {
            get { return isScanReceiptView; }
            set
            {
                if (isScanReceiptView != value)
                {
                    isScanReceiptView = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsScanReceiptView)));
                }
            }
        }

        private static bool isScanReceiptTotalView;
        public bool IsScanReceiptTotalView
        {
            get { return isScanReceiptTotalView; }
            set
            {
                if (isScanReceiptTotalView != value)
                {
                    isScanReceiptTotalView = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsScanReceiptTotalView)));
                }
            }
        }

        private static string scanReceiptViewColor;
        public string ScanReceiptViewColor
        {
            get { return scanReceiptViewColor; }
            set
            {
                if (scanReceiptViewColor != value)
                {
                    scanReceiptViewColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScanReceiptViewColor)));
                }
            }
        }

        private static string scanReceiptTotalViewColor;
        public string ScanReceiptTotalViewColor
        {
            get { return scanReceiptTotalViewColor; }
            set
            {
                if (scanReceiptTotalViewColor != value)
                {
                    scanReceiptTotalViewColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScanReceiptTotalViewColor)));
                }
            }
        }


    }
}
