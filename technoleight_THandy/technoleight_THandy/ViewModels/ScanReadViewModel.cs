using THandy.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.SimpleAudioPlayer;
using System.IO;
using System;
using System.Text.RegularExpressions;
using THandy;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Linq;
using THandy.Views;
using System.Text;

namespace THandy.ViewModels
{
    public abstract class ScanReadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation m_navigation;

        ISimpleAudioPlayer SEplayer = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

        protected abstract int getDensoStartBit(string iD);

        public string Readkubun;
        public string nameA;
        private static bool flag = true;    // 非同期でも共有されるようにstaticにする。
        public bool bCompletedDsp = false;

        ~ ScanReadViewModel()
        {
            Console.WriteLine("#ScanReadViewModel finish");
        }

        public async void init(string name1, string kubun)
        {
            Readkubun = kubun;
            nameA = name1;
            ScannedCode = "";
            StrTitle = nameA.Replace("画面", "") + "　読取";
            HName = nameA;
            FrameVisible = true;       //Frameを表示
            GridVisible = true;

            if (Readkubun == "204")
            {
                //登録データの削除を行う
                await App.DataBase.DeleteAllScanReadData();
                await App.DataBase.ALLDeleteNouhinJLAsync();
            }

            await setPicker();
            await houji();
        }

        public async Task houji()
        {
            int cou = await App.DataBase.GetScanReadDataAsync4(Readkubun);
            ScannedCode2 = cou.ToString();

            int HoujiResult = await Houji();
            IsAnalyzing = true;   //読み取り開始
        }

        private async Task<int> Houji()
        {
            ////読取実績を表示
            //ObservableCollection<PageTypeGroup> Items1 = new ObservableCollection<PageTypeGroup>();

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
                                    //string noku = nouhin202[x].VHNOKU.ToString().Trim();
                                    //string nose = nouhin202[x].VHNOSE.ToString().Trim();
                                    //string nono = nouhin202[x].VHNONO.ToString().Trim();
                                    //string line = nouhin202[x].VILINE.ToString().Trim();
                                    //string noba = nouhin202[x].VHNOBA.ToString().Trim();

                                    //string ndat = nouhin202[x].VHDATE.ToString().Trim();
                                    //string jiku = nouhin202[x].VHJIKU.ToString().Trim();

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
            return 1;
        }

        private async Task setPicker()
        {

            //PICKを設定
            ScanModeItems = new ObservableCollection<string>();
            if (Readkubun != "204")
            {
                ScanModeItems.Add(Common.Const.C_SCANNAME_KEYBOARD);
            }
            //ScanModeItems.Add(Common.Const.C_SCANNAME_CAMERA);
            if (Device.RuntimePlatform == Device.Android)
            {
                //ScanModeItems.Add(Common.Const.C_SCANNAME_BARCODE);
                ScanModeItems.Add(Common.Const.C_SCANNAME_CLIPBOARD);
            }
            string mode1 = "";
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            if (Set2.Count > 0)
            {
                mode1 = Set2[0].ScanMode;
                if (mode1 == null || mode1 == "")
                {
                    //mode1 = Common.Const.C_SCANMODE_CAMERA;
                    mode1 = Common.Const.C_SCANMODE_CLIPBOARD;
                }
            }
            else
            {
                //mode1 = Common.Const.C_SCANMODE_CAMERA;
                mode1 = Common.Const.C_SCANMODE_CLIPBOARD;
            }
            ScanModeSelectedIndex = int.Parse(mode1) - 1;
            bCompletedDsp = true;

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

                ScannedCode = strScannedCode;

                //読取実績の整合性検証                                            
                string ID = ScannedCode;
                string ID2 = ScannedCode;
                //デンソー現品票は70桁以上をOKとする
                int D234L = 70;

                //設定ファイルの読取
                List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
                string WID = "";
                string user = "";
                if (Set2.Count > 0)
                {
                    WID = Set2[0].WID;
                    user = Set2[0].user;
                    //if (Set2[0].WID == "1")
                    //{
                    //    //デンソー処理　時区表示を行う
                    //    MyCollectionView.IsVisible = true;
                    //}
                }
                else
                {

                }

                //SQLiteより設定ファイルを抽出
                List<BarCodeM> barCodem = await App.DataBase.GetBarCodeMAsync(WID, Readkubun);
                if (barCodem.Count > 0)
                {
                    int x;
                    //バーコードの長さ判断を抽出
                    bool NotZero = false; //長さ判断が必要か 0の場合は長さ判断をしない
                    bool LengthOK = false; //長さOKかどうか
                    for (x = 0; x <= barCodem.Count - 1; x++)
                    {
                        int y = 0;
                        if (int.TryParse(barCodem[x].Ketasu, out y)) { }
                        if (y != 0)
                        {
                            NotZero = true;
                            if (y == ID.Length)
                            {
                                LengthOK = true;
                            }

                            if (Readkubun == "102")
                            {
                                //デンソー現品票は70桁以上をOKとする
                                if (ID.Length > D234L)
                                {
                                    LengthOK = true;
                                }
                            }
                        }
                    }

                    if (NotZero == true)
                    {
                        //長さ判断OK
                        if (LengthOK == false)
                        {
                            //桁数エラー
                            System.Diagnostics.Debug.WriteLine("Warning WorkResults: double Read");
                            ScannedCode = "桁数エラー";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                            SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                            SEplayer.Play();
                            await Task.Delay(1000);   //1秒待機
                            this.IsAnalyzing = true;   //読み取り再開
                            flag = true;
                            return;
                        }
                    }
                }
                else
                {

                }

                // ----------------------------------------------------------------------------
                double latitude, longitude;//緯度、経度
                latitude = 0.0;
                longitude = 0.0;

                // ----------------------------------------------------------------------------
                
                // テクノエイトで使用、現品票QR形の作成
                string createBarcode = "";

                //テクノエイト　入庫かんばん読みの場合
                if (Readkubun == "202" || Readkubun == "205")
                {
                    List<Nouhin> nouhin = await App.DataBase.GetNouhinAsync();

                    string letters0_3= ID.Substring(0, 3);

                    //正しいかんばんデータか　最初にK01がついてるか
                    if (letters0_3 != "K01")
                    {
                        ScannedCode = "QRコードが不正です";
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                        SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                        SEplayer.Play();
                        await Task.Delay(1000);    //1秒待機
                        this.IsAnalyzing = true;   //読み取り再開
                        flag = true;
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
                            System.Diagnostics.Debug.WriteLine("Warning WorkResults: double Read");
                            ScannedCode = "重複です";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                            SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                            SEplayer.Play();
                            await Task.Delay(1000);    //1秒待機
                            this.IsAnalyzing = true;   //読み取り再開
                            flag = true;
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
                            ScannedCode = "該当データがありません";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                            SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                            SEplayer.Play();
                            await Task.Delay(1000);    //1秒待機
                            this.IsAnalyzing = true;   //読み取り再開
                            flag = true;
                            return;
                        }

                    }
                    else
                    {
                        ScannedCode = "数量の読取不可";
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                        SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                        SEplayer.Play();
                        await Task.Delay(1000);    //1秒待機
                        this.IsAnalyzing = true;   //読み取り再開
                        flag = true;
                        return;
                    }

                }
                //テクノエイト　出庫代表バーコードorかんばん読みの場合
                else if (Readkubun == "203")
                {
                    List<NouhinJL> nouhinJL = await App.DataBase.GetNouhinJLAsync();

                    var shuko = new Shuko();

                    // 文字数をチェック
                    int baeLengh = ID.ToString().Length;

                    // 11桁なら代表バーコード出庫
                    if (baeLengh == 11)
                    {
                        var firstText = ID.Substring(0, 1);
                        // 代表バーコードは最初が”3”
                        if (firstText == "3")
                        {
                            // 入庫済みデータに該当するものがあるかチェック

                            for (int x = 0; x <= nouhinJL.Count - 1; x++)
                            {
                                string dkey = nouhinJL[x].JLDKEY.ToString().Trim();

                                // 代表キーが一致すればOK
                                if (dkey == ID)
                                {
                                    // 同じ代表キーを持つリスト
                                    var sameDkeyList = nouhinJL.Where(e => e.JLDKEY == dkey).ToList();

                                    string nouki = nouhinJL[x].JLDATE.ToString().Trim();

                                    int totalBoxCount = sameDkeyList.Count();

                                    shuko.DKEY = dkey;
                                    shuko.Nouki = nouki;
                                    shuko.TotalBoxCount = totalBoxCount;

                                    createBarcode = dkey;
                                    int OKa = await App.DataBase.SaveShukoAsync(shuko);

                                    // 同じ代表キーの入庫済みデータをすべて照合対象から削除
                                    for (int n = 0; n <= sameDkeyList.Count - 1; n++)
                                    {
                                        int OKb = await App.DataBase.DeleteNouhinJLAsync(sameDkeyList[n]);
                                    }

                                    break;
                                }
                            }

                            if (String.IsNullOrEmpty(shuko.DKEY))
                            {
                                ScannedCode = "入庫データがありません";
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                                SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                                SEplayer.Play();
                                await Task.Delay(1000);    //1秒待機
                                this.IsAnalyzing = true;   //読み取り再開
                                flag = true;
                                return;
                            }

                        }
                        else
                        {
                            // 11桁なのに、最初が3ではない場合エラー
                            ScannedCode = "QRコードが不正です";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                            SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                            SEplayer.Play();
                            await Task.Delay(1000);    //1秒待機
                            this.IsAnalyzing = true;   //読み取り再開
                            flag = true;
                            return;
                        }
                    }
                    // elseかんばん読みの場合
                    else
                    {
                        string letters0_3 = ID.Substring(0, 3);

                        //正しいかんばんデータか　最初にK01がついてるか
                        if (letters0_3 != "K01")
                        {
                            ScannedCode = "QRコードが不正です";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                            SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                            SEplayer.Play();
                            await Task.Delay(1000);    //1秒待機
                            this.IsAnalyzing = true;   //読み取り再開
                            flag = true;
                            return;
                        }

                        //トヨテツコードの位置で、通常品か支給品かを判断
                        //トヨテツコードがなければ、臨時かんばん
                        string toytetsuCode = "3999";
                        string letters11_4 = ID.Substring(11 - 1, 4);
                        string letters4_4 = ID.Substring(4 - 1, 4);

                        string hinban = "";
                        string suryo = "";
                        string edaban = "";
                        string ukeire = "";

                        // 通常品
                        if (letters11_4 == toytetsuCode)
                        {
                            hinban = ID.Substring(45 - 1, 20).Trim();
                            suryo = ID.Substring(29 - 1, 5).Trim();

                            // 現在は未使用項目
                            edaban = ID.Substring(25 - 1, 4).Trim();
                            ukeire = ID.Substring(34 - 1, 2).Trim();
                        }
                        // 支給品
                        else if (letters4_4 == toytetsuCode)
                        {
                            hinban = ID.Substring(44 - 1, 20).Trim();
                            suryo = ID.Substring(66 - 1, 5).Trim();
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
                                System.Diagnostics.Debug.WriteLine("Warning WorkResults: double Read");
                                ScannedCode = "重複です";
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                                SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                                SEplayer.Play();
                                await Task.Delay(1000);    //1秒待機
                                this.IsAnalyzing = true;   //読み取り再開
                                flag = true;
                                return;
                            }

                        }

                        // 読取数量の数値化
                        int read_suryo;
                        if (int.TryParse(suryo, out read_suryo))
                        {

                            //入庫済みデータに該当するものがあるか、あれば保存
                            for (int x = 0; x <= nouhinJL.Count - 1; x++)
                            {
                                string nyuko_hinban = nouhinJL[x].JLBUNO.ToString().Trim();

                                string nksu = nouhinJL[x].JLNKSU.ToString().Trim();
                                int nyuko_suryo;
                                if (int.TryParse(nksu, out nyuko_suryo)) { }

                                // 品番と数量が一致するものがあれば、その納番に割り当てていく
                                if (nyuko_hinban == hinban && nyuko_suryo == read_suryo)
                                {

                                    //// 既に代表キーを読んだかんばんを読んだらエラー
                                    //List<Shuko> shukoList = await App.DataBase.GetShukoAsync();

                                    //string dkey = nouhinJL[x].JLDKEY.ToString().Trim();
                                    //bool IsDKEYRead = shukoList.Where(e => e.DKEY == dkey).Count() > 0 ? true : false;

                                    //if (IsDKEYRead)
                                    //{
                                    //    System.Diagnostics.Debug.WriteLine("Warning WorkResults: double Read");
                                    //    ScannedCode = "代表キー既読";
                                    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                                    //    SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                                    //    SEplayer.Play();
                                    //    await Task.Delay(1000);    //1秒待機
                                    //    this.IsAnalyzing = true;   //読み取り再開
                                    //    flag = true;
                                    //    return;

                                    //}


                                    //shuko.JLNOKU = nouhinJL[x].JLNOKU.ToString();
                                    //shuko.JLNOSE = nouhinJL[x].JLNOSE.ToString();
                                    //shuko.JLNONO = nouhinJL[x].JLNONO.ToString();
                                    //shuko.JLLINE = nouhinJL[x].JLLINE.ToString();
                                    //shuko.JLRNNO = nouhinJL[x].JLRNNO.ToString();
                                    //shuko.JLBUNO = nouhinJL[x].JLBUNO.ToString();
                                    //shuko.JLDATE = nouhinJL[x].JLDATE.ToString();
                                    //shuko.JLDKEY = nouhinJL[x].JLDKEY.ToString();
                                    //shuko.JLNKSU = nyuko_suryo.ToString();

                                    //createBarcode = shuko.JLNOKU + ":" + shuko.JLNOSE + ":" + shuko.JLNONO + ":" + String.Format("{0:D3}", shuko.JLLINE) + ":" + String.Format("{0:D3}", shuko.JLRNNO) + ":" + String.Format("{0:D5}", shuko.JLNKSU);

                                    //int OKa = await App.DataBase.SaveShukoAsync(shuko);
                                    //int OKb = await App.DataBase.DeleteNouhinJLAsync(nouhinJL[x]);

                                    string dkey = nouhinJL[x].JLDKEY.ToString().Trim();
                                    string nouki = nouhinJL[x].JLDATE.ToString().Trim();

                                    // 同じ代表キーを持つリスト
                                    var sameDkeyList = nouhinJL.Where(e => e.JLDKEY == dkey).ToList();

                                    int totalBoxCount = sameDkeyList.Count();

                                    shuko.DKEY = dkey;
                                    shuko.Nouki = nouki;
                                    shuko.TotalBoxCount = totalBoxCount;

                                    createBarcode = dkey;

                                    int OKa = await App.DataBase.SaveShukoAsync(shuko);

                                    // 同じ代表キーの入庫済みデータをすべて照合対象から削除
                                    for (int n = 0; n <= sameDkeyList.Count - 1; n++)
                                    {
                                        int OKb = await App.DataBase.DeleteNouhinJLAsync(sameDkeyList[n]);
                                    }

                                    break;

                                }
                            }
                        }
                        else
                        {
                            ScannedCode = "数量の読取不可";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                            SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                            SEplayer.Play();
                            await Task.Delay(1000);    //1秒待機
                            this.IsAnalyzing = true;   //読み取り再開
                            flag = true;
                            return;
                        }

                        if (String.IsNullOrEmpty(shuko.DKEY))
                        {
                            ScannedCode = "入庫データがありません";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                            SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                            SEplayer.Play();
                            await Task.Delay(1000);    //1秒待機
                            this.IsAnalyzing = true;   //読み取り再開
                            flag = true;
                            return;
                        }

                    }

                }
                else
                {
                    //2度読みチェック
                    //二回目以降 ２度読み検証
                    //DBに読取実績が存在するかチェック
                    List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(Readkubun);

                    for (int x = 0; x <= sagyoUsers.Count - 1; x++)
                    {
                        if (sagyoUsers[x].Scanstring == ID)
                        {
                            System.Diagnostics.Debug.WriteLine("Warning WorkResults: double Read");
                            ScannedCode = "重複です";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                            SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                            SEplayer.Play();
                            await Task.Delay(1000);    //1秒待機
                            this.IsAnalyzing = true;   //読み取り再開
                            flag = true;
                            return;
                        }
                    }
                }

                //テクノエイト　代表再印刷
                if (Readkubun == "204")
                {
                    string letters0_3 = ID.Substring(0, 3);

                    //正しいかんばんデータか　最初にK01がついてるか
                    if (letters0_3 != "K01")
                    {
                        ScannedCode = "QRコードが不正です";
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                        SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                        SEplayer.Play();
                        await Task.Delay(1000);    //1秒待機
                        this.IsAnalyzing = true;   //読み取り再開
                        flag = true;
                        return;
                    }

                    //トヨテツコードの位置で、通常品か支給品かを判断
                    //トヨテツコードがなければ、臨時かんばん
                    string toytetsuCode = "3999";
                    string letters11_4 = ID.Substring(11 - 1, 4);
                    string letters4_4 = ID.Substring(4 - 1, 4);

                    string hinban = "";
                    string suryo = "";
                    string edaban = "";
                    string ukeire = "";

                    // 通常品
                    if (letters11_4 == toytetsuCode)
                    {
                        createBarcode = ID;
                        hinban = ID.Substring(45 - 1, 20).Trim();
                        suryo = ID.Substring(29 - 1, 5).Trim();

                        // 現在は未使用項目
                        edaban = ID.Substring(25 - 1, 4).Trim();
                        ukeire = ID.Substring(34 - 1, 2).Trim();
                    }
                    // 支給品
                    else if (letters4_4 == toytetsuCode)
                    {
                        createBarcode = ID;
                        hinban = ID.Substring(44 - 1, 20).Trim();
                        suryo = ID.Substring(66 - 1, 5).Trim();
                    }
                    // 臨時
                    else
                    {
                        //ScannedCode = "QRコードが不正です";
                        //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScannedCode)));
                        //SEplayer.Load(GetStreamFromFile("alert2.mp3"));
                        //SEplayer.Play();
                        //await Task.Delay(1000);    //1秒待機
                        //this.IsAnalyzing = true;   //読み取り再開
                        //flag = true;
                        //return;

                        //　品番と数量の位置は通常品と同じ
                        createBarcode = ID;
                        hinban = ID.Substring(45 - 1, 20).Trim();
                        suryo = ID.Substring(29 - 1, 5).Trim();
                    }

                    var nouhinJL = new NouhinJL();

                    // 読取数量の数値化
                    int read_suryo = 0;
                    if (int.TryParse(suryo, out read_suryo))

                    nouhinJL.JLBUNO = hinban;
                    nouhinJL.JLNKSU = read_suryo.ToString();

                    int OKa = await App.DataBase.ALLDeleteNouhinJLAsync();
                    int OKb = await App.DataBase.DeleteAllScanReadData();

                    int OKc = await App.DataBase.SaveNouhinJLAsync(nouhinJL);

                }

                //音鳴らす
                SEplayer.Load(GetStreamFromFile("decision1.mp3"));
                SEplayer.Play();

                //DBに書き込み
                ScanReadData Sdata = new ScanReadData();
                Sdata.Kubn = Readkubun;
                Sdata.Scanstring = ID;
                //Sdata.Scanstring2 = ID2;
                Sdata.Scanstring2 = createBarcode;
                Sdata.Sryou = 0;
                Sdata.cuser = user;
                Sdata.cdate = DateTime.Now;
                Sdata.latitude = latitude;
                Sdata.longitude = longitude;
                int OK1 = await App.DataBase.SaveScanReadDataAsync(Readkubun, Sdata);
                await houji();
                //var HoujiReturn = await Houji();

                ////音鳴らす
                //SEplayer.Load(GetStreamFromFile("decision1.mp3"));
                //SEplayer.Play();

                // 高速化対応で高速化が必要なものは除外
                if (!strScanMode.Equals(Common.Const.C_SCANMODE_BARCODE) && !strScanMode.Equals(Common.Const.C_SCANMODE_CLIPBOARD))
                { 
                    await Task.Delay(1000);    //1秒待機
                }
                this.IsAnalyzing = true;   //読み取り再開
                flag = true;
            }
        }

        private Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            //ビルドアクションで埋め込みリソースにしたファイルを取ってくる
            var stream = assembly.GetManifestResourceStream("technoleight_THandy." + filename);
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

    }
}
