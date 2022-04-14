using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using THandy.Models;
using THandy.ViewModels;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using THandy.Interface;
using Android.Bluetooth;
using System.IO;
using Android.PrintServices;
using System.Drawing.Printing;
using Android.Views;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentPage
    {
        public ScanViewModel vm;

        /// <summary>
        /// ボタン処理中に画面遷移させない制御用
        /// </summary>       
        public static bool btnFanction = false;

        public ScanPage(ScanViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
            List1();
        }

        private async void List1()
        {
            //画面表示
            int i;
            NoukiAndBinView.IsVisible = false;
            NoukiAndShabanView.IsVisible = false;
            NoukiAndDkeyView.IsVisible = false;
            MessageView.IsVisible = false;

            //設定ファイルの読取
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            if (Set2.Count > 0)
            {

            }
            else
            {

            }

            if (vm.Readkubun == "204")
            {
                vm.BtnLeft = "読取";
                vm.BtnRight = "印刷";

                EndButton.IsVisible = false;
                NavigationPage.SetHasNavigationBar(this, true);
            }
            else
            {
                vm.BtnLeft = "読取";
                vm.BtnRight = "登録";
            }

            if (vm.Readkubun == "202")
            {
                // テクノエイト かんばん読み取り
                NoukiAndBinView.IsVisible = true;

                var nouki = Convert.ToDateTime(vm.Nouki).ToString("yyyy/MM/dd");

                var binList = vm.BinLIst;
                var binText = "";
                for (int x = 0; x < binList.Count; ++x)
                {
                    if (x != 0)
                    {
                        binText += ".";
                    }
                    binText += binList[x];
                }

                Head_Nouki.Text = nouki;
                Head_Bin.Text = binText;

            }
            else if (vm.Readkubun == "203")
            {
                // テクノエイト 代表キーまたはかんばん読み取り
                NoukiAndShabanView.IsVisible = true;
                //MessageView.IsVisible = true;
                MyListView.IsVisible = false;

                var nouki = Convert.ToDateTime(vm.Nouki).ToString("yyyy/MM/dd") + " 以降";
                var shaban = vm.Shaban.ToString();

                Head_Nouki2.Text = nouki;
                Head_Shaban.Text = shaban;

            }
            else if (vm.Readkubun == "205")
            {
                NoukiAndDkeyView.IsVisible = true;

                var dkey = vm.Dkey.ToString();
                var nouki = vm.Nouki.ToString();
                var bin = vm.BinLIst[0];
                var nyukobi = vm.Nyukobi.ToString();
                var setCount = vm.SetCount.ToString();

                Head_Dkey.Text = dkey;
                Head_Nouki3.Text = nouki;
                Head_Bin2.Text = bin;
                Head_Nyukobi.Text = nyukobi;
                Head_SetCount.Text = setCount;
            }
        }

        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //CollectionViewクリック処理
            var pre = e.PreviousSelection;
            var cur = e.CurrentSelection;


            ObservableCollection<Jiku> previous = new ObservableCollection<Jiku>();
            ObservableCollection<Jiku> current = new ObservableCollection<Jiku>();


            if (pre.Count > 0)
            {

                foreach (Jiku val in pre)
                {
                    Jiku Jikux = new Jiku();
                    Jikux.Id = val.Id;
                    //val.Color = "Yellow";
                    //Jikux.Color = val.Color;
                    previous.Add(Jikux);
                    Save1(val.Id, "0");

                }

            }
            if (cur.Count > 0)
            {

                foreach (Jiku val in cur)
                {

                    Jiku Jikux = new Jiku();
                    Jikux.Id = val.Id;
                    //val.Color = "White";
                    //val.Color = "Blue";
                    //Jikux.Color = val.Color;
                    current.Add(Jikux);
                    Save1(val.Id, "1");


                }

            }

            ////Console.WriteLine(String.Join(" ", lst3.Select(x => x)));
            if (previous.Count == 0)
            {
                return;
            }


        }

        private async void Save1(string Id, string OnOff)
        {
            JikuDB Jikuz = new JikuDB();
            Jikuz.Id = Id;
            Jikuz.OnOff = OnOff;
            int cnt1 = await App.DataBase.SaveJikuDBAsync(Jikuz);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //読み込み中マークを表示
            vm.ActivityRunning = true;

            BindingContext = this.vm;
            var readkbn = vm.Readkubun;

            if (readkbn == "204")
            {
                //登録データの削除を行う
                await App.DataBase.DeleteAllScanReadData();
                await App.DataBase.ALLDeleteNouhinJLAsync();
            }

            //読取画面から戻ってきたときに切断する。
            ScanReadBarcodeViewModel.GetInstance().DisposeEvent();
            ScanReadClipBoardViewModel.GetInstance().DisposeEvent();

            ObservableCollection<PageTypeGroup> roading = new ObservableCollection<PageTypeGroup>();
            PageTypeGroup gr = new PageTypeGroup();


            //gr.Title = "Loading...";
            roading.Add(gr);
            MyListView.ItemsSource = roading;

            int task1 = await Houji();

            ////ナビゲーションバーを表示
            //NavigationPage.SetHasNavigationBar(this, true);

            //リストを選択してもハイライトしないようにする
            MyListView.ItemTapped += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            //読み込みマークを消す
            vm.ActivityRunning = false;
            //画面全体を表示する
            MainView.IsVisible = true;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert("警告", "未登録の読取済みデータは削除されます。戻ってよろしいですか？", "Yes", "No");
                if (result) await this.Navigation.PopAsync(); // or anything else
            });

            return true;
        }

        private void EndButton_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert("警告", "未登録の読取済みデータは削除されます。戻ってよろしいですか？", "Yes", "No");
                if (result) await this.Navigation.PopAsync(); // or anything else
            });
        }

        private async void Scan_Clicked(object sender, EventArgs e)
        {
            //読取ページを起動
            string mode1 = "";
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            if (Set2.Count > 0)
            {
                mode1 = Set2[0].ScanMode;
                if (mode1 == null)
                {
                    mode1 = "";
                }
            }
            else
            {

            }
            if (Common.Const.C_SCANMODE_KEYBOARD == mode1)
            {
                //キーボードモードとしてスキャン画面呼び出し(シングルトン)
                Page page = ScanReadPageKeyBoard.GetInstance(vm.Title, vm.Readkubun);
                await Navigation.PushAsync(page);
            }
            else if (Common.Const.C_SCANMODE_BARCODE == mode1)
            {
                //バーコードモードとしてスキャン画面呼び出し(シングルトン)
                Page page = ScanReadPageBarcode.GetInstance(vm.Title, vm.Readkubun);
                await Navigation.PushAsync(page);
            }
            else if (Common.Const.C_SCANMODE_CLIPBOARD == mode1)
            {
                //クリップボードモードとしてスキャン画面呼び出し(シングルトン)
                Page page = ScanReadPageClipBoard.GetInstance(vm.Title, vm.Readkubun);
                await Navigation.PushAsync(page);
            }
            else
            {
                //カメラモードとしてスキャン画面呼び出し(シングルトン)
                Page page = ScanReadPageCamera.GetInstance(vm.Title, vm.Readkubun);
                await Navigation.PushAsync(page);
            }

        }

        private async Task<int> Houji()
        {
            //読取実績を表示
            ObservableCollection<PageTypeGroup> Items1 = new ObservableCollection<PageTypeGroup>();
            //ViewModelsから画面番号を抽出
            string readkubn = vm.Readkubun;
            //readkubn 画面番号
            switch (readkubn)
            {
                case "202":// 通常入庫
                case "205":// 積増入庫

                    List<Nouhin> nouhin202 = await App.DataBase.GetNouhinAsync();

                    if (nouhin202.Count == 0)
                    {
                        //ユーザー情報抽出
                        string WID = "";
                        string url = "";
                        string k_pass = "";
                        string user = "";
                        string Device = "";

                        List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
                        if (Set2.Count > 0)
                        {
                            WID = Set2[0].WID;
                            url = Set2[0].url;
                            k_pass = Set2[0].k_pass;
                            user = Set2[0].user;
                            Device = Set2[0].Device;
                        }

                        var items1 = new BarModel();

                        items1.Shori = "S1";
                        items1.WID = WID;
                        items1.UserID = user;
                        items1.Device = Device;
                        items1.Shorikubun = "202";
                        items1.BarcodeRead = "";
                        items1.BarcodeRead1 = "";
                        items1.Sryou = "";
                        items1.Cdate1 = "";
                        items1.latitude = "";
                        items1.longitude = "";
                        items1.Nouki = vm.Nouki;

                        var binList = new List<string>();

                        //Scan内容を行数分Listに入れる
                        //items1.Add(new Dictionary<string, string>() { { "Shori", "202View" }, { "WID", WID }, { "UserID", user }, { "Device", Device }, { "Shorikubun", readkubn }, { "BarcodeRead", "" }, { "BarcodeRead1", "" }, { "Sryou", "" }, { "Cdate1", "" }, { "latitude", "" }, { "longitude", "" } });
                        try
                        {
                            for (int x = 0; x < vm.BinLIst.Count; x++)
                            {
                                binList.Add(vm.BinLIst[x]);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        items1.BinList = binList;

                        string message1 = "";
                        //WEBサーバーからSELECT
                        var postLidt = new List<BarModel>();
                        postLidt.Add(items1);
                        List<Dictionary<string, string>> items2 = await App.API.Post_method3(postLidt, "BarTechnolEight");
                        if (items2 == null)
                        {
                            await Application.Current.MainPage.DisplayAlert("サーバーエラー", "データを取得できませんでした。もう一度お願いします", "OK");
                        }
                        else if (items2.Count == 0)
                        {
                            //行数0
                            //システムエラー
                            await Application.Current.MainPage.DisplayAlert("エラー", "戻りデーターエラー", "OK");
                        }
                        else if (items2.Count > 0)
                        {

                            int ic = 0;

                            string dictValue;
                            if (true == items2[0].TryGetValue(key: Common.Const.C_ERR_KEY_NETWORK, value: out dictValue))
                            {
                                //ネットワーク
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                            }
                            else if (items2[0]["Name"].ToString() == "0")
                            {
                                //納品書番号が1件も存在しない
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("エラー", "指示データが存在しません", "OK");
                            }
                            else if (items2[0]["Name"].ToString() == "NG")
                            {
                                //納品書データ
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("エラー", "指示データを取得できません", "OK");
                            }
                            else
                            {
                                for (int i = 0; i <= items2.Count - 1; i++)
                                {
                                    Nouhin VFile = new Nouhin();
                                    //VFile.JJDECD = items2[i]["JJDECD"].ToString();
                                    VFile.VHNOKU = items2[i]["JJNOKU"].ToString();
                                    VFile.VHNOSE = items2[i]["JJNOSE"].ToString();
                                    VFile.VHNONO = items2[i]["JJNONO"].ToString();
                                    VFile.VILINE = items2[i]["JJLINE"].ToString();

                                    VFile.VHTRCD = items2[i]["VHTRCD"].ToString();
                                    //VFile.DEKANJ = items2[i]["DEKANJ"].ToString();

                                    VFile.VHKOKU = items2[i]["VHKOKU"].ToString();
                                    VFile.VHNOBA = items2[i]["VHNOBA"].ToString();
                                    VFile.VHDATE = items2[i]["JJDATE"].ToString();
                                    VFile.VHJIKU = items2[i]["VHJIKU"].ToString();
                                    VFile.VIBUNO = items2[i]["VIBUNO"].ToString();

                                    //VFile.VIBUNM = items2[i]["VIBUNM"].ToString();
                                    //VFile.VIJIKO = items2[i]["VIJIKO"].ToString();
                                    VFile.VISRYO = items2[i]["JJNHSU"].ToString();
                                    VFile.JJNKSU = items2[i]["JJNKSU"].ToString();
                                    VFile.JJNHSU = items2[i]["JJNHSU"].ToString();
                                    VFile.VIYOSU = items2[i]["VIYOSU"].ToString();
                                    VFile.VILOSU = items2[i]["VILOSU"].ToString();

                                    var stratSeq = 0;
                                    if (VFile.JJNKSU != "0")
                                    {
                                        // SEQの計算
                                        int ssryo = 0; //指示数
                                        int nsryo = 0; //入庫数
                                        int ysryo = 0; //容器数
                                        if (int.TryParse(VFile.VISRYO.ToString().Trim(), out ssryo)) { }
                                        if (int.TryParse(VFile.JJNKSU.ToString().Trim(), out nsryo)) { }
                                        if (int.TryParse(VFile.VIYOSU.ToString().Trim(), out ysryo)) { }

                                        stratSeq = nsryo / (ssryo / ysryo);
                                    }
                                    VFile.JLSTARTSEQ = stratSeq;

                                    //SQLiteデータベース登録
                                    ic = await App.DataBase.SaveNouhinAsync(VFile);

                                    PageTypeGroup gr = new PageTypeGroup();
                                    //納品書番号
                                    gr.Title = "納番：" + VFile.VHNOKU + VFile.VHNOSE + VFile.VHNONO + "-" + VFile.VILINE + "　受入：" + VFile.VHNOBA + "　箱数：" + VFile.VIYOSU;
                                    //部品番号　数量
                                    gr.ShortName = VFile.VIBUNO + "　" + VFile.JJNKSU + "／" + VFile.JJNHSU;

                                    Items1.Add(gr);

                                }
                            }

                        }

                    }
                    else
                    {
                        try
                        {

                            for (int x = 0; x <= nouhin202.Count - 1; x++)
                            {
                                string noku = nouhin202[x].VHNOKU.ToString().Trim();
                                string nose = nouhin202[x].VHNOSE.ToString().Trim();
                                string nono = nouhin202[x].VHNONO.ToString().Trim();
                                string line = nouhin202[x].VILINE.ToString().Trim();
                                string noba = nouhin202[x].VHNOBA.ToString().Trim();
                                //string ndat = nouhin202[x].VHDATE.ToString().Trim();
                                //string jiku = nouhin202[x].VHJIKU.ToString().Trim();

                                string buno = nouhin202[x].VIBUNO.ToString().Trim();
                                string sryo = nouhin202[x].VISRYO.ToString().Trim();
                                string nsryo = nouhin202[x].JJNKSU.ToString().Trim();
                                string yosu = nouhin202[x].VIYOSU.ToString().Trim();

                                //string OK = "";

                                //if (sryo == nsryo)
                                //{
                                //    OK = "ＯＫ!";
                                //}

                                if (sryo == nsryo)
                                {
                                    // 数量が満たされれば、一覧に表示しない
                                }
                                else
                                {
                                    PageTypeGroup gr = new PageTypeGroup();
                                    gr.Title = "納番：" + noku + nose + nono + "-" + line + "　受入：" + noba + "　箱数：" + yosu;
                                    //部品番号　数量
                                    gr.ShortName = buno + "　" + nsryo + "／" + sryo;

                                    Items1.Add(gr);
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            await Application.Current.MainPage.DisplayAlert("エラー", "例外エラー", "OK");
                        }

                    }

                    MyListView.ItemsSource = Items1;

                    break;
                case "203":
                    List<NouhinJL> nouhin203 = await App.DataBase.GetNouhinJLAsync();
                    List<Shuko> shuko203 = await App.DataBase.GetShukoAsync();

                    var CanReadItems = new List<Dictionary<string, string>>();

                    bool NoReadData = false;

                    if (nouhin203.Count == 0 && shuko203.Count == 0)
                    {
                        //ユーザー情報抽出
                        string WID = "";
                        string url = "";
                        string k_pass = "";
                        string user = "";
                        string Device = "";

                        List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
                        if (Set2.Count > 0)
                        {
                            WID = Set2[0].WID;
                            url = Set2[0].url;
                            k_pass = Set2[0].k_pass;
                            user = Set2[0].user;
                            Device = Set2[0].Device;
                        }

                        var items1 = new BarModel();

                        items1.Shori = "S1";
                        items1.WID = WID;
                        items1.UserID = user;
                        items1.Device = Device;
                        items1.Shorikubun = "203";
                        items1.BarcodeRead = "";
                        items1.BarcodeRead1 = "";
                        items1.Sryou = "";
                        items1.Cdate1 = "";
                        items1.latitude = "";
                        items1.longitude = "";
                        items1.Nouki = vm.Nouki;

                        string message1 = "";
                        //WEBサーバーからSELECT
                        var postLidt = new List<BarModel>();
                        postLidt.Add(items1);
                        CanReadItems = await App.API.Post_method3(postLidt, "BarTechnolEight");
                        if (CanReadItems == null)
                        {
                            await Application.Current.MainPage.DisplayAlert("サーバーエラー", "データを取得できませんでした。もう一度お願いします", "OK");
                        }
                        else if (CanReadItems.Count == 0)
                        {
                            //行数0
                            //システムエラー
                            await Application.Current.MainPage.DisplayAlert("エラー", "戻りデーターエラー", "OK");
                        }
                        else if (CanReadItems.Count > 0)
                        {

                            int ic = 0;

                            string dictValue;
                            if (true == CanReadItems[0].TryGetValue(key: Common.Const.C_ERR_KEY_NETWORK, value: out dictValue))
                            {
                                //ネットワーク
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                            }
                            else if (CanReadItems[0]["Name"].ToString() == "0")
                            {
                                //納品書番号が1件も存在しない
                                //エラーは後で出す
                                NoReadData = true;
                            }
                            else if (CanReadItems[0]["Name"].ToString() == "NG")
                            {
                                //納品書データ
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("エラー", "入庫済みデータを取得できません", "OK");
                            }
                            else
                            {

                                if (CanReadItems.Count > 0)
                                {
                                    for (int i = 0; i <= CanReadItems.Count - 1; i++)
                                    {
                                        NouhinJL VFile = new NouhinJL();
                                        VFile.JLNOKU = CanReadItems[i]["JLNOKU"].ToString();
                                        VFile.JLNOSE = CanReadItems[i]["JLNOSE"].ToString();
                                        VFile.JLNONO = CanReadItems[i]["JLNONO"].ToString();
                                        VFile.JLLINE = CanReadItems[i]["JLLINE"].ToString();
                                        VFile.JLRNNO = CanReadItems[i]["JLRNNO"].ToString();
                                        VFile.JLBUNO = CanReadItems[i]["JLBUNO"].ToString();
                                        VFile.JLDATE = CanReadItems[i]["JLDATE"].ToString();
                                        VFile.JLNKSU = CanReadItems[i]["JLNKSU"].ToString();
                                        VFile.JLDKEY = CanReadItems[i]["JLDKEY"].ToString();

                                        //SQLiteデータベース登録
                                        // 入庫済みデータを保存しておく（画面表示はしない）
                                        ic = await App.DataBase.SaveNouhinJLAsync(VFile);

                                    }

                                }
                                else
                                {

                                }
                            }

                        }

                    }
                    else
                    {
                        //既にJLデータ（読取から戻った時）が記憶されているので何もしない
                    }

                    // 読取出庫データ一覧表示
                    if (shuko203.Count != 0)
                    {
                        for (int i = 0; i <= shuko203.Count - 1; i++)
                        {
                            //代表キーorかんばん出庫
                            //bool isDkey = true;
                            Shuko shuko = new Shuko();
                            shuko.DKEY = shuko203[i].DKEY;
                            shuko.Nouki = shuko203[i].Nouki;
                            shuko.TotalBoxCount = shuko203[i].TotalBoxCount;
                            //if (!String.IsNullOrEmpty(dkey))
                            //{
                            //    shuko.DKEY = dkey;
                            //}
                            //else
                            //{
                            //    isDkey = false;
                            //    shuko.JLBUNO = shuko203[i].JLBUNO;
                            //    shuko.JLNKSU = shuko203[i].JLNKSU;
                            //}

                            PageTypeGroup gr = new PageTypeGroup();
                            //if (isDkey)
                            //{
                            //    gr.Title = "代表キー";
                            //    gr.ShortName = shuko.DKEY.ToString();
                            //}
                            //else
                            //{
                            //    gr.Title = "かんばん";
                            //    gr.ShortName = "品番：" + shuko.JLBUNO.ToString() + "　数量：" + shuko.JLNKSU.ToString();
                            //}

                            gr.Title = (i + 1) + "." + shuko.DKEY.ToString();
                            gr.ShortName = "納期：" + shuko.Nouki.ToString() + " セット数：" + shuko.TotalBoxCount.ToString();

                            Items1.Add(gr);
                        }

                        
                        MessageView.IsVisible = false;
                        MyListView.IsVisible = true;

                        MyListView.ItemsSource = Items1;

                    }
                    else
                    {
                        MessageView.IsVisible = true;
                        if (NoReadData)
                        {
                            MessageView.Text = "入庫済みデータが存在しません！";
                            MessageView.TextColor = Color.Red;
                        }
                        else
                        {
                            MessageView.Text = "代表キーまたはかんばんを読み取り出庫してください。";
                        }
                        MyListView.IsVisible = false;
                    }

                    break;

                case "204":


                    break;
                default:
                    List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(readkubn);

                    for (int x = 0; x <= sagyoUsers.Count - 1; x++)
                    {
                        PageTypeGroup gr = new PageTypeGroup();
                        gr.Title = sagyoUsers[x].Scanstring; //読取実績
                        gr.ShortName = sagyoUsers[x].cdate.ToString(); //作成日付
                        Items1.Add(gr);
                    }
                    MyListView.ItemsSource = Items1;
                    break;
            }
            return 1;
        }

        public async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                //削除処理
                if (e.Item == null || vm.Readkubun == "202" || vm.Readkubun == "203")
                    return;

                bool answer = await DisplayAlert("削除", "削除しますか", "Yes", "No");
                if (answer == true)
                {
                    string readkubn = vm.Readkubun;
                    PageTypeGroup gr = new PageTypeGroup();
                    gr = (PageTypeGroup)e.Item;
                    int OK1 = 0;
                    string moji = "";
                    switch (readkubn)
                    {
                        case "2":
                            //現品票削除処理

                            string s1 = gr.Title;
                            int x1 = s1.IndexOf(" ");
                            moji = gr.Title.Substring(0, x1);
                            moji = moji.Replace("-", "");
                            //if (x1 > 0)
                            //{
                            //    moji += gr.Title.Substring(8, x1 - 8 ); // - を除いて8桁へ
                            //}

                            List<Nouhin> nouhin2 = await App.DataBase.GetNouhinAsync();

                            for (int x = 0; x <= nouhin2.Count - 1; x++)
                            {
                                string noku = nouhin2[x].VHNOKU.ToString().Trim();
                                string nose = nouhin2[x].VHNOSE.ToString().Trim();
                                string nono = nouhin2[x].VHNONO.ToString().Trim();
                                string line = nouhin2[x].VILINE.ToString().Trim();

                                string OK = "";
                                if (moji == (noku + nose + nono + line))
                                {
                                    //Nouhin nouhin2 = await App.DataBase.GetNouhinAsync(noku, nose, nono, line);
                                    OK1 = await App.DataBase.DeleteNouhinAsync(nouhin2[x]);
                                }

                                //PageTypeGroup gr = new PageTypeGroup();
                                //gr.Title = noku + nose + nono + "-" + line + " " + noba + " " + ndat + " " + jiku; //納品書番号
                                //gr.ShortName = buno + " " + sryo + " " + nsryo + " " + OK; //部品番号　数量
                                //Items1.Add(gr);
                            }

                            //List<ScanReadData> Sdata2 = new List<ScanReadData>();
                            //Sdata2 = await App.DataBase.GetGenpinScanReadDataAsync(readkubn, moji);
                            //int i;
                            //for (i=0; i <= Sdata2.Count-1; i++)
                            //{
                            //    ScanReadData Sdate3 = new ScanReadData();
                            //    Sdate3 = Sdata2[i];
                            //    OK1 = await App.DataBase.DeleteScanReadData(Sdate3);
                            //}
                            Task<int> task3 = Houji();

                            break;
                        case "102":
                            //デンソー現品票削除処理
                            moji = gr.Title.Substring(0, 7) + gr.Title.Substring(8, 1); // - を除いて8桁へ


                            List<Nouhin> nouhin = await App.DataBase.GetNouhinAsync();

                            for (int x = 0; x <= nouhin.Count - 1; x++)
                            {
                                string noku = nouhin[x].VHNOKU.ToString().Trim();
                                string nose = nouhin[x].VHNOSE.ToString().Trim();
                                string nono = nouhin[x].VHNONO.ToString().Trim();
                                string line = nouhin[x].VILINE.ToString().Trim();

                                string OK = "";
                                if (moji == (noku + nose + nono + line))
                                {
                                    //Nouhin nouhin2 = await App.DataBase.GetNouhinAsync(noku, nose, nono, line);
                                    OK1 = await App.DataBase.DeleteNouhinAsync(nouhin[x]);
                                }

                                //PageTypeGroup gr = new PageTypeGroup();
                                //gr.Title = noku + nose + nono + "-" + line + " " + noba + " " + ndat + " " + jiku; //納品書番号
                                //gr.ShortName = buno + " " + sryo + " " + nsryo + " " + OK; //部品番号　数量
                                //Items1.Add(gr);
                            }


                            //List<ScanReadData> Sdata2 = new List<ScanReadData>();
                            //Sdata2 = await App.DataBase.GetGenpinScanReadDataAsync(readkubn, moji);
                            //int i;
                            //for (i=0; i <= Sdata2.Count-1; i++)
                            //{
                            //    ScanReadData Sdate3 = new ScanReadData();
                            //    Sdate3 = Sdata2[i];
                            //    OK1 = await App.DataBase.DeleteScanReadData(Sdate3);
                            //}
                            Task<int> task1 = Houji();

                            break;
                        case "202":
                            //テクノエイト　読取削除
                            int ukeireIndex = gr.Title.IndexOf("受入：");
                            moji = gr.Title.Substring(3, ukeireIndex - 3).Trim();
                            moji = moji.Replace("-", "");

                            List<Nouhin> nouhin_202 = await App.DataBase.GetNouhinAsync();

                            for (int x = 0; x <= nouhin_202.Count - 1; x++)
                            {
                                string noku = nouhin_202[x].VHNOKU.ToString().Trim();
                                string nose = nouhin_202[x].VHNOSE.ToString().Trim();
                                string nono = nouhin_202[x].VHNONO.ToString().Trim();
                                string line = nouhin_202[x].VILINE.ToString().Trim();

                                string OK = "";
                                if (moji == (noku + nose + nono + line))
                                {
                                    //Nouhin nouhin2 = await App.DataBase.GetNouhinAsync(noku, nose, nono, line);

                                    // 読み取り前、読み取り対象から外す　読み取った後は削除しても送信される。
                                    OK1 = await App.DataBase.DeleteNouhinAsync(nouhin_202[x]);
                                }

                                //PageTypeGroup gr = new PageTypeGroup();
                                //gr.Title = noku + nose + nono + "-" + line + " " + noba + " " + ndat + " " + jiku; //納品書番号
                                //gr.ShortName = buno + " " + sryo + " " + nsryo + " " + OK; //部品番号　数量
                                //Items1.Add(gr);
                            }


                            //List<ScanReadData> Sdata2 = new List<ScanReadData>();
                            //Sdata2 = await App.DataBase.GetGenpinScanReadDataAsync(readkubn, moji);
                            //int i;
                            //for (i=0; i <= Sdata2.Count-1; i++)
                            //{
                            //    ScanReadData Sdate3 = new ScanReadData();
                            //    Sdate3 = Sdata2[i];
                            //    OK1 = await App.DataBase.DeleteScanReadData(Sdate3);
                            //}
                            Task<int> task_202 = Houji();

                            break;
                        default:
                            ScanReadData Sdata = new ScanReadData();
                            Sdata = await App.DataBase.GetScanReadDataAsync(readkubn, gr.Title);
                            OK1 = await App.DataBase.DeleteScanReadData(Sdata);
                            Task<int> task2 = Houji();
                            break;
                    }
                }
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void Touroku_Clicked(object sender, EventArgs e)
        {
            // ボタン押下チェック(連打対策)
            if (!btnFanction)
            {
                btnFanction = true; //ボタン押下不可
                MainView.IsVisible = false;
                vm.ActivityRunning = true;

                await Touroku_Clicked_Excute();

                vm.ActivityRunning = false;
                MainView.IsVisible = true;
                btnFanction = false; //ボタン押下可
            }
        }

        private async Task Touroku_Clicked_Excute()
        {
            //登録処理
            //画面の種類区分ごとに処理を行う
            //101 デンソー用納品書読取処理
            int i = 0;
            string readkubn = vm.Readkubun;
            List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(readkubn);
            if (sagyoUsers.Count == 0)
            {
                await DisplayAlert("データ件数", "0件です", "OK");
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

                List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
                if (Set2.Count > 0)
                {
                    WID = Set2[0].WID;
                    url = Set2[0].url;
                    k_pass = Set2[0].k_pass;
                    user = Set2[0].user;
                    Device = Set2[0].Device;
                }

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
                        var request = new GeolocationRequest(GeolocationAccuracy.High);
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


                //List<Dictionary<string, string>> items1 = new List<Dictionary<string, string>>();

                //for (int x = 0; x <= sagyoUsers.Count - 1; x++)
                //{
                //    //Scan内容を行数分Listに入れる
                //    items1.Add(new Dictionary<string, string>() { { "Shori", "I1" }, { "WID", WID }, { "UserID", user }, { "Device", Device },
                //    { "Shorikubun", readkubn }, { "BarcodeRead", sagyoUsers[x].Scanstring }, { "BarcodeRead1", sagyoUsers[x].Scanstring2 },
                //    { "Sryou", sagyoUsers[x].Sryou.ToString() }, { "Cdate1", sagyoUsers[x].cdate.ToString() }, { "latitude", latitude.ToString() },
                //    { "longitude", longitude.ToString() } });

                //}

                var postList = new List<BarModel>();
                for (int x = 0; x <= sagyoUsers.Count - 1; x++)
                {
                    var postItem = new BarModel();
                    postItem.Shori = "I1";
                    postItem.WID = WID;
                    postItem.UserID = user;
                    postItem.Device = Device;
                    postItem.Shorikubun = readkubn;
                    postItem.BarcodeRead = sagyoUsers[x].Scanstring;
                    postItem.BarcodeRead1 = sagyoUsers[x].Scanstring2;
                    postItem.Sryou = sagyoUsers[x].Sryou.ToString();
                    postItem.Cdate1 = sagyoUsers[x].cdate.ToString();
                    postItem.latitude = latitude.ToString();
                    postItem.longitude = longitude.ToString();
                    postItem.BinList = vm.BinLIst;
                    postItem.Nouki = vm.Nouki;
                    postItem.Shaban = vm.Shaban;

                    if (readkubn == "205")
                    {
                        postItem.Dkey = vm.Dkey.ToString();
                    }

                    postList.Add(postItem);
                }

                // APIは会社ごとにControllerが分かれている
                string apiConName = "Bar";
                if (WID == "4")
                {
                    apiConName = "BarTechnolEight";
                }

                string message1 = "";
                //WEBサーバーにUpdate
                //SQLserver登録
                List<Dictionary<string, string>> items2 = await App.API.Post_method3(postList, apiConName);
                if (items2 == null)
                {
                    await Application.Current.MainPage.DisplayAlert("サーバーエラー", "登録できませんでした。もう一度お願いします", "OK");
                    return;
                }
                else if (items2.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("サーバーエラー", "サーバーからの返り値がありませんでした", "OK");
                    return;
                }
                else if (items2.Count > 0)
                {

                    int ic = 0;
                    int OK1a = 0;
                    switch (readkubn)
                    {
                        case "202": // 通常入庫
                        case "205": // 積増入庫
                            //テクノエイト　かんばん入庫処理

                            message1 = "";
                            //エラーチェック
                            for (i = 0; i <= items2.Count - 1; i++)
                            {
                                try
                                {
                                    string dictValue;
                                    if (true == items2[i].TryGetValue(key: Common.Const.C_ERR_KEY_NETWORK, value: out dictValue))
                                    {
                                        //ネットワーク
                                        //エラー
                                        await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                                        return;
                                    }
                                    if (items2[i]["Name"].ToString() == "0")
                                    {
                                        //現品票番号が1件も存在しない
                                        //エラー
                                        await Application.Current.MainPage.DisplayAlert("エラー", "読取指示データが存在しません", "OK");
                                        return;
                                    }
                                    if (items2[i]["Name"].ToString() == "NG")
                                    {
                                        //納品書データ
                                        //エラー
                                        await Application.Current.MainPage.DisplayAlert("エラー", "読取内容に誤りがあります", "OK");
                                        return;
                                    }
                                    if (items2[i]["Name"].ToString() == "NGX")
                                    {
                                        //部品番号エラー
                                        //エラー
                                        await Application.Current.MainPage.DisplayAlert("エラー", "この倉庫で在庫しない部品です", "OK");
                                        return;
                                    }
                                    if (items2[i]["Name"].ToString() == "NG_DKEY")
                                    {
                                        //部品番号エラー
                                        //エラー
                                        await Application.Current.MainPage.DisplayAlert("エラー", "代表キーの取得に失敗しました", "OK");
                                        return;
                                    }
                                }
                                catch (Exception e1)
                                {
                                    //エラー
                                    await Application.Current.MainPage.DisplayAlert("エラー", "システムエラー", "OK");
                                    return;
                                }

                            }

                            // 代表バーコードの印刷

                            var dkeyError = "";

                            var dkey = items2[0]["DKEY"].ToString();
                            var dkeyDate = items2[0]["DKEYDATE"].ToString();
                            var dkeyCount = items2[0]["DKEYCOUNT"].ToString();

                            Head_SetCount.Text = dkeyCount;

                            // 代表バーコードを発行するプリンター名を取得
                            var printerName = Set2[0].BarcodeReader.Trim();

                            try
                            {
                                // プリンターuuidを取得
                                IBluetoothManager btMan = DependencyService.Get<IBluetoothManager>();
                                var uuid = btMan.GetBondedDevices().Where(x => x.strName == printerName).First().strUuid;

                                // プリンターデバイスアドレスを取得
                                var mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                                ICollection<BluetoothDevice> pairedDevices = mBluetoothAdapter.BondedDevices;
                                var deviceAddress = pairedDevices.AsEnumerable().Where(x => x.Name == printerName).First().Address;

                                BluetoothDevice mmDevice = BluetoothAdapter.DefaultAdapter.GetRemoteDevice(deviceAddress);
                                var socket = mmDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString(uuid));

                                // サーバーへの接続部分
                                // 接続タイムアウトは下記のように自作する
                                // これを入れないと接続タイムアウトまで約20秒程度かかってしまう
                                int timeout = 3000;
                                Task task = socket.ConnectAsync();
                                if (!task.Wait(timeout))
                                {
                                    socket.Close();
                                    throw new SocketException(10060);
                                }
                                else
                                {
                                    MemoryStream stream = new MemoryStream();
                                    var datastream = socket.OutputStream;

                                    String ESC = "\x1B";
                                    //data = $@"{ESC}A{ESC}#0{ESC}%3{ESC}KC1{ESC}V075{ESC}H330{ESC}BD101120{"*20220111*"}{ESC}V140{ESC}H200{ESC}P5{ESC}L0101{ESC}K9D{"*20220111*"}{ESC}V110{ESC}H140{ESC}P1{ESC}L0102{ESC}K9D{"ｾｯﾄﾋﾂﾞｹ : 2022/01/11"}{ESC}V110{ESC}H080{ESC}P1{ESC}L0102{ESC}K9D{"ﾂﾐｱﾜｾﾖｳｷ: 0 ｺ"}{ESC}Q1{ESC}Z";
                                    string DKEY = "*" + dkey + "*";
                                    string setDate = dkeyDate;
                                    string yosu = dkeyCount;
                                    string setDateLabel = "835A8362836793FA95748146"; //『セット日付：』
                                    string yosuLabel = "94A081408140814090948146";    //『箱　　　数：』

                                    string data =
                                                ESC + "A" +
                                                ESC + "#0" + ESC + "%3" + ESC + "KC1" +
                                                ESC + "V55" + ESC + "H360" + ESC + "BD101120" + DKEY +
                                                ESC + "V140" + ESC + "H230" + ESC + "P5" + ESC + "L0101" + ESC + "K9D" + DKEY +
                                                ESC + "V80" + ESC + "H170" + ESC + "P1" + ESC + "L0102" + ESC + "K9H" + setDateLabel + ESC + "K9D" + setDate +
                                                ESC + "V80" + ESC + "H110" + ESC + "P1" + ESC + "L0102" + ESC + "K9H" + yosuLabel + ESC + "K9D" + yosu +
                                                ESC + "Q1" +
                                                ESC + "Z"
                                                ;

                                    byte[] byteArray = Encoding.ASCII.GetBytes(data);

                                    datastream.Write(byteArray, 0, byteArray.Length);
                                    datastream.Flush();

                                    socket.Close();
                                }
                            }
                            catch (SocketException ex)
                            {
                                // 接続タイムアウト
                                dkeyError = "サーバーに登録が完了しました。代表バーコードの印刷は失敗しました。（タイムアウト）";
                            }
                            catch (AggregateException ex)
                            {
                                if (ex.InnerException is SocketException)
                                {
                                    // 接続失敗拒否
                                    dkeyError = "サーバーに登録が完了しました。代表バーコードの印刷は失敗しました。（接続失敗）";
                                }
                            }
                            catch (Exception ex)
                            {
                                // その他のエラー
                                dkeyError = "サーバーに登録が完了しました。代表バーコードの印刷は失敗しました。";
                            }


                            // 更新されたデータ
                            for (i = 0; i <= items2.Count - 1; i++)
                            {
                                Nouhin VFile = new Nouhin();
                                VFile.VHNOKU = items2[i]["VHNOKU"].ToString();
                                VFile.VHNOSE = items2[i]["VHNOSE"].ToString();
                                VFile.VHNONO = items2[i]["VHNONO"].ToString();
                                VFile.VILINE = items2[i]["VILINE"].ToString();

                                VFile.VHTRCD = items2[i]["VHTRCD"].ToString();

                                VFile.VHKOKU = items2[i]["VHKOKU"].ToString();
                                VFile.VHNOBA = items2[i]["VHNOBA"].ToString();
                                VFile.VHDATE = items2[i]["VHDATE"].ToString();
                                VFile.VHJIKU = items2[i]["VHJIKU"].ToString();
                                VFile.VIBUNO = items2[i]["VIBUNO"].ToString();

                                VFile.VIBUNM = items2[i]["VIBUNM"].ToString();
                                VFile.VIJIKO = items2[i]["VIJIKO"].ToString();
                                VFile.VISRYO = items2[i]["VISRYO"].ToString();
                                VFile.JJNKSU = items2[i]["JJNKSU"].ToString();
                                VFile.JJNHSU = items2[i]["JJNHSU"].ToString();
                                VFile.VIYOSU = items2[i]["VIYOSU"].ToString();
                                VFile.VILOSU = items2[i]["VILOSU"].ToString();

                                var stratSeq = 0;
                                if (VFile.JJNKSU != "0")
                                {
                                    // SEQの計算
                                    int ssryo = 0; //指示数
                                    int nsryo = 0; //入庫数
                                    int ysryo = 0; //容器数
                                    if (int.TryParse(VFile.VISRYO.ToString().Trim(), out ssryo)) { }
                                    if (int.TryParse(VFile.JJNKSU.ToString().Trim(), out nsryo)) { }
                                    if (int.TryParse(VFile.VIYOSU.ToString().Trim(), out ysryo)) { }

                                    stratSeq = nsryo / (ssryo / ysryo);
                                }
                                VFile.JLSTARTSEQ = stratSeq;

                                //SQLiteデータベース削除登録
                                Nouhin nouhin3 = await App.DataBase.GetNouhinAsync(items2[i]["VHNOKU"].ToString(), items2[i]["VHNOSE"].ToString(), items2[i]["VHNONO"].ToString(), items2[i]["VILINE"].ToString());

                                if (items2[i]["JJNKSU"].ToString() == items2[i]["JJNHSU"].ToString())
                                {
                                    if (nouhin3 != null)
                                    {
                                        int ic0 = await App.DataBase.DeleteNouhinAsync(nouhin3);

                                    }
                                }
                                else
                                {
                                    //SQLiteデータベースUPDATE
                                    int ic1 = await App.DataBase.SaveNouhinAsync(VFile);
                                }

                            }

                            //List<Nouhin> nouhin_202 = await App.DataBase.GetNouhinAsync();
                            //for (int x = 0; x <= nouhin_202.Count - 1; x++)
                            //{
                            //    string nsryo = nouhin_202[x].JJNKSU.ToString().Trim();
                            //    string nhryo = nouhin_202[x].JJNHSU.ToString().Trim();

                            //    string OK = "";
                            //    if (nhryo == nsryo)
                            //    {
                            //        //SQLiteデータベース削除登録
                            //        ic = await App.DataBase.DeleteNouhinAsync(nouhin_202[x]);
                            //    }

                            //}

                            //読取データの削除を行う
                            int OK1_202 = await App.DataBase.DeleteAllkubunScanReadData(readkubn);
                            Task<int> task_202 = Houji();

                            if (dkeyError == "")
                            {
                                await Application.Current.MainPage.DisplayAlert("登録処理", "サーバーに登録が完了しました。", "OK");
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("登録処理", dkeyError, "OK");
                            }

                            return;

                        case "203":
                            //テクノエイト　出庫処理

                            message1 = "";
                            //エラーチェック
                            for (i = 0; i <= items2.Count - 1; i++)
                            {
                                try
                                {
                                    string dictValue;
                                    if (true == items2[i].TryGetValue(key: Common.Const.C_ERR_KEY_NETWORK, value: out dictValue))
                                    {
                                        //ネットワーク
                                        //エラー
                                        await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                                        return;
                                    }
                                    //if (items2[i]["Name"].ToString() == "0")
                                    //{
                                    //    現品票番号が1件も存在しない
                                    //    エラー
                                    //    await Application.Current.MainPage.DisplayAlert("エラー", "入庫指示データが存在しません", "OK");
                                    //    return;
                                    //}
                                    if (items2[i]["Name"].ToString() == "NG")
                                    {
                                        //納品書データ
                                        //エラー
                                        await Application.Current.MainPage.DisplayAlert("エラー", "読取内容に誤りがあります", "OK");
                                        return;
                                    }
                                    if (items2[i]["Name"].ToString() == "NGX")
                                    {
                                        //部品番号エラー
                                        //エラー
                                        await Application.Current.MainPage.DisplayAlert("エラー", "この倉庫で在庫しない部品です", "OK");
                                        return;
                                    }
                                }
                                catch (Exception e1)
                                {
                                    //エラー
                                    await Application.Current.MainPage.DisplayAlert("エラー", "システムエラー", "OK");
                                    return;
                                }

                            }

                            var deleteRes1 = await App.DataBase.ALLDeleteShukoAsync();
                            //var deleteRes2 = await App.DataBase.ALLDeleteNouhinJLAsync();

                            //読取データの削除を行う
                            int OK1_203 = await App.DataBase.DeleteAllkubunScanReadData(readkubn);
                            Task<int> task_203 = Houji();

                            await Application.Current.MainPage.DisplayAlert("登録処理", "サーバーに登録が完了しました。", "OK");

                            return;

                        default:
                            message1 = "";
                            foreach (string Value in items2[0].Values)
                            {
                                message1 = Value;
                            }

                            if (message1 == "OK")
                            {
                                //登録データの削除を行う
                                int OK1 = await App.DataBase.DeleteAllkubunScanReadData(readkubn);
                                Task<int> task1 = Houji();
                                await Application.Current.MainPage.DisplayAlert("登録処理", "サーバーに登録が完了しました。", "OK");
                                return;
                            }
                            else if (message1 == Common.Const.C_ERR_VALUE_NETWORK)
                            {
                                //ネットワーク
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                                return;
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("サーバーエラー", "登録できませんでした。もう一度お願いします。", "OK");
                                return;
                            }

                    }


                }

            }

        }

    }


}