using Plugin.SimpleAudioPlayer;
using technoleight_THandy.Models;
using technoleight_THandy.ViewModels;
using technoleight_THandy;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace technoleight_THandy.Views
{
    // シングルトンで呼び出すこと
    public partial class ScanReadPageKeyBoard : ContentPage
    {
        public static string ReadKbn = "";

        private static ScanReadPageKeyBoard scanReadPageKeyBoard;
        public static ScanReadPageKeyBoard GetInstance(string name1, string kubun, INavigation navigation)
        {
            ReadKbn = kubun;

            ScanReadKeyBoardViewModel.GetInstance().Initilize(name1, kubun, navigation);

            //キーボード入力画面
            if (scanReadPageKeyBoard == null)
            {
                scanReadPageKeyBoard = new ScanReadPageKeyBoard();
                return scanReadPageKeyBoard;
            }
            return scanReadPageKeyBoard;
        }

        public ScanReadPageKeyBoard()
        {
            InitializeComponent();
        }

        ~ScanReadPageKeyBoard()
        {
            Console.WriteLine("#ScanReadPageKeyBoard finish");
        }
        protected override void OnAppearing()
        {

            if (ReadKbn == "203")
            {
                DKeyLabel.IsVisible = true;
                DKey.IsVisible = true;
            }
            else
            {
                DKeyLabel.IsVisible = false;
                DKey.IsVisible = false;
            }

            scankidou();
        }

        public void Hyouji()
        {
            //HName.Text = "いいいい";
        }

        private async void scankidou()
        {
            base.OnAppearing();

            await ResetAsync();

            BindingContext = ScanReadKeyBoardViewModel.GetInstance();
            //if (ReadKbn == "203")
            //{
            //    DKey.Unfocus();   // 再表示時にキーボードがでない時があったので、一度フォーカスを逃がす
            //    DKey.Focus();
            //}
            //else
            //{
            //    BuhinCode1.Unfocus();   // 再表示時にキーボードがでない時があったので、一度フォーカスを逃がす
            //    BuhinCode1.Focus();
            //}

        }

        private async void OnCompleted(object sender, EventArgs eventArgs)
        {
            //Entryに入力されている文字を表示
            //DisplayAlert("", this.txtCode.Text, "OK");
            await ScanReadKeyBoardViewModel.GetInstance().OnCompleted();

            await ResetAsync();

        }

        private async Task ResetAsync()
        {
            int cou = await App.DataBase.GetScanReadDataAsync4(ReadKbn);
            QRstring2.Text = cou.ToString();

            if (ReadKbn == "203")
            {
                DKey.Text = "";
                BuhinCode1.Text = "";
                BuhinCode2.Text = "";
                BuhinCode3.Text = "";
                BuhinSuryo.Text = "";
                DKey.Focus();
            }
            else
            {
                BuhinCode1.Text = "";
                BuhinCode2.Text = "";
                BuhinCode3.Text = "";
                BuhinSuryo.Text = "";
                BuhinCode1.Focus();
            }
        }

        //private async void PickScanModeSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int index = PickScanMode.SelectedIndex;
        //        ScanReadKeyBoardViewModel vm = ScanReadKeyBoardViewModel.GetInstance();
        //        // 表示完了後のみイベント拾う
        //        if (true == vm.bCompletedDsp && index >= 0)
        //        {
        //            string strSelectItem = PickScanMode.Items[index];

        //            if (strSelectItem.Equals(Common.Const.C_SCANNAME_CAMERA))
        //            {
        //                //キーボードからカメラ切替
        //                List<Setei> Set1 = await App.DataBase.GetSeteiAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_CAMERA;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageCamera.GetInstance(vm.nameA, vm.Readkubun);
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //            }
        //            else if (strSelectItem.Equals(Common.Const.C_SCANNAME_BARCODE))
        //            {
        //                //キーボードからバーコードリーダ切替
        //                List<Setei> Set1 = await App.DataBase.GetSeteiAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_BARCODE;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageBarcode.GetInstance(vm.nameA, vm.Readkubun);
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //            }
        //            else if (strSelectItem.Equals(Common.Const.C_SCANNAME_CLIPBOARD))
        //            {
        //                //キーボードからクリップボード切替
        //                List<Setei> Set1 = await App.DataBase.GetSeteiAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_CLIPBOARD;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageClipBoard.GetInstance(vm.nameA, vm.Readkubun, "");
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.WriteLine("#PickScanModeSelectedIndexChanged Err {0}", ex.ToString());
        //    }
        //}

        private async void Text_Scan_Clicked(object sender, EventArgs e)
        {

            //設定ファイルの読取
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            string WID = "";
            string user = "";
            if (Set2.Count > 0)
            {
                WID = Set2[0].WID;
                user = Set2[0].user;
            }
            else
            {

            }

            // ----------------------------------------------------------------------------
            double latitude, longitude;//緯度、経度
            latitude = 0.0;
            longitude = 0.0;

            // 現品票QR形の作成
            string createBarcode = "";

            //テクノエイト　入庫かんばん読みの場合
            if (ReadKbn == "202" || ReadKbn == "205")
            {
                var buhinCode1 = BuhinCode1.Text == null ? "" : BuhinCode1.Text.Trim();
                var buhinCode2 = BuhinCode2.Text == null ? "" : BuhinCode2.Text.Trim();
                var buhinCode3 = BuhinCode3.Text == null ? "" : BuhinCode3.Text.Trim();
                var buhinSuryo = BuhinSuryo.Text == null ? "" : BuhinSuryo.Text.Trim();

                int read_suryo = 0;
                string buhinCode = "";

                // 入力不正チェック
                if (String.IsNullOrEmpty(buhinCode1))
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "品番を入力してください。", "OK");
                    return;
                }
                else if (String.IsNullOrEmpty(buhinCode2) && !(String.IsNullOrEmpty(buhinCode3)))
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "品番が不正です。", "OK");
                    return;
                }
                else if (String.IsNullOrEmpty(buhinSuryo))
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "数量を入力してください。", "OK");
                    return;
                }

                // 数量の数値変換チェック
                if (int.TryParse(buhinSuryo, out read_suryo))
                {

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "数量を数値に変換できません。", "OK");
                    return;
                }

                // 品番結合
                if (String.IsNullOrEmpty(buhinCode2) && String.IsNullOrEmpty(buhinCode3))
                {
                    // 品番１つ目のみ入力
                    buhinCode = buhinCode1;
                }
                else if (!(String.IsNullOrEmpty(buhinCode2)) && String.IsNullOrEmpty(buhinCode3))
                {
                    // 品番１つ目と２つ目が入力
                    buhinCode = buhinCode1 + "-" + buhinCode2 + "-";
                }
                else if (!(String.IsNullOrEmpty(buhinCode2)) && !(String.IsNullOrEmpty(buhinCode3)))
                {
                    // 品番１つ目と２つ目が入力
                    buhinCode = buhinCode1 + "-" + buhinCode2 + "-" + buhinCode3;
                }

                if (buhinCode == "")
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "品番を正しく認識できません。", "OK");
                    return;
                }

                List<Nouhin> nouhin = await App.DataBase.GetNouhinAsync();
                bool Matched = false;

                //数量更新
                for (int x = 0; x <= nouhin.Count - 1; x++)
                {
                    string hinban_shizi = nouhin[x].VIBUNO.ToString().Trim();
                    string suryo_shizi = nouhin[x].VISRYO.ToString().Trim();
                    string lot_shizi = nouhin[x].VILOSU.ToString().Trim();

                    // 品番とロット数が一致するものがあれば、その納番に割り当てていく
                    if (buhinCode == hinban_shizi && read_suryo.ToString() == lot_shizi)
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
                    await Application.Current.MainPage.DisplayAlert("エラー", "該当データがありません。", "OK");
                    return;
                }

            }
            else if (ReadKbn == "203")
            {
                bool isDKey = true;
                if (DKey.Text == "")
                {
                    isDKey = false;
                }

                if (isDKey)
                {
                    long dkey;
                    var dkeyString = DKey.Text.Trim();

                    // 代表キーの数値変換チェック
                    if (!long.TryParse(dkeyString, out dkey))
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "代表キーは数値で入力してください。", "OK");
                        return;
                    }

                    // 文字数をチェック
                    int dkeyLengh = dkeyString.Length;

                    // 11桁なら代表バーコード出庫
                    if (dkeyLengh != 11)
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "代表キーの文字数が正しくありません。", "OK");
                        return;
                    }

                    // 最初の文字をチェック
                    var firstText = dkeyString.Substring(0, 1);

                    // 代表バーコードは最初が”3”
                    if (firstText != "3")
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "代表キーが正しくありません。", "OK");
                        return;
                    }

                    //二回目以降 代表キー
                    //DBに代表キー読取実績が存在するかチェック
                    List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(ReadKbn);

                    for (int x = 0; x <= sagyoUsers.Count - 1; x++)
                    {
                        if (sagyoUsers[x].Scanstring2 == dkeyString)
                        {
                            await Application.Current.MainPage.DisplayAlert("エラー", "代表キーが重複しています。", "OK");
                            return;
                        }

                    }

                    List<NouhinJL> nouhinJL = await App.DataBase.GetNouhinJLAsync();
                    var shuko = new Shuko();

                    // 入庫済みデータに該当するものがあるかチェック
                    for (int x = 0; x <= nouhinJL.Count - 1; x++)
                    {
                        string JLdkey = nouhinJL[x].JLDKEY.ToString().Trim();

                        // 代表キーが一致すればOK
                        if (JLdkey == dkeyString)
                        {
                            // 同じ代表キーを持つリスト
                            var sameDkeyList = nouhinJL.Where(k => k.JLDKEY == JLdkey).ToList();

                            string nouki = nouhinJL[x].JLDATE.ToString().Trim();

                            int totalBoxCount = sameDkeyList.Count();

                            shuko.DKEY = dkeyString;
                            shuko.Nouki = nouki;
                            shuko.TotalBoxCount = totalBoxCount;

                            createBarcode = dkeyString;
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
                        await Application.Current.MainPage.DisplayAlert("エラー", "入庫済みデータが存在しません。", "OK");
                        return;
                    }

                }
                else
                {
                    var buhinCode1 = BuhinCode1.Text == null ? "" : BuhinCode1.Text.Trim();
                    var buhinCode2 = BuhinCode2.Text == null ? "" : BuhinCode2.Text.Trim();
                    var buhinCode3 = BuhinCode3.Text == null ? "" : BuhinCode3.Text.Trim();
                    var buhinSuryo = BuhinSuryo.Text == null ? "" : BuhinSuryo.Text.Trim();

                    int suryo = 0;
                    string buhinCode = "";

                    // 入力不正チェック
                    if (String.IsNullOrEmpty(buhinCode1))
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "代表キーまたは品番を入力してください。", "OK");
                        return;
                    }
                    else if (String.IsNullOrEmpty(buhinCode2) && !(String.IsNullOrEmpty(buhinCode3)))
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "品番が不正です。", "OK");
                        return;
                    }
                    else if (String.IsNullOrEmpty(buhinSuryo))
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "数量を入力してください。", "OK");
                        return;
                    }

                    // 数量の数値変換チェック
                    if (int.TryParse(buhinSuryo, out suryo))
                    {

                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "数量を数値に変換できません。", "OK");
                        return;
                    }

                    // 品番結合
                    if (String.IsNullOrEmpty(buhinCode2) && String.IsNullOrEmpty(buhinCode3))
                    {
                        // 品番１つ目のみ入力
                        buhinCode = buhinCode1;
                    }
                    else if (!(String.IsNullOrEmpty(buhinCode2)) && String.IsNullOrEmpty(buhinCode3))
                    {
                        // 品番１つ目と２つ目が入力
                        buhinCode = buhinCode1 + "-" + buhinCode2 + "-";
                    }
                    else if (!(String.IsNullOrEmpty(buhinCode2)) && !(String.IsNullOrEmpty(buhinCode3)))
                    {
                        // 品番１つ目と２つ目が入力
                        buhinCode = buhinCode1 + "-" + buhinCode2 + "-" + buhinCode3;
                    }

                    if (buhinCode == "")
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "品番を正しく認識できません。", "OK");
                        return;
                    }

                    List<NouhinJL> nouhinJL = await App.DataBase.GetNouhinJLAsync();
                    var shuko = new Shuko();

                    //入庫済みデータに該当するものがあるか、あれば保存
                    for (int x = 0; x <= nouhinJL.Count - 1; x++)
                    {
                        int read_suryo = suryo;

                        string nyuko_hinban = nouhinJL[x].JLBUNO.ToString().Trim();
                        string nksu = nouhinJL[x].JLNKSU.ToString().Trim();
                        int nyuko_suryo;
                        if (int.TryParse(nksu, out nyuko_suryo)) { }

                        // 品番と数量が一致するものがあれば、その納番に割り当てていく
                        if (nyuko_hinban == buhinCode && nyuko_suryo == read_suryo)
                        {

                            //// 既に代表キーを読んだかんばんを読んだらエラー
                            //List<Shuko> shukoList = await App.DataBase.GetShukoAsync();
                            //bool IsDKEYRead = shukoList.Where(i => i.DKEY == nouhinJL[x].JLDKEY.ToString()).Count() > 0 ? true : false;

                            //if (IsDKEYRead)
                            //{
                            //    await Application.Current.MainPage.DisplayAlert("エラー", "既に代表キーを読んでいます。", "OK");
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
                            var sameDkeyList = nouhinJL.Where(i => i.JLDKEY == dkey).ToList();

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
                        await Application.Current.MainPage.DisplayAlert("エラー", "入庫済みデータが存在しません。", "OK");
                        return;
                    }

                }
            }

            //DBに書き込み
            ScanReadData Sdata = new ScanReadData();
            Sdata.Kubn = ReadKbn;
            Sdata.Scanstring = createBarcode;
            Sdata.Scanstring2 = createBarcode;
            Sdata.Sryou = 0;
            Sdata.cuser = user;
            Sdata.cdate = DateTime.Now;
            Sdata.latitude = latitude;
            Sdata.longitude = longitude;
            int OK1 = await App.DataBase.SaveScanReadDataAsync(ReadKbn, Sdata);
            await ResetAsync();
            await Application.Current.MainPage.DisplayAlert("登録処理", "読取登録しました。", "OK");

        }
    }
}
