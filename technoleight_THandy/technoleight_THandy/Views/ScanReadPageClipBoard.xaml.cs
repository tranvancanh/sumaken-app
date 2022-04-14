﻿using Plugin.SimpleAudioPlayer;
using THandy.Models;
using THandy.ViewModels;
using THandy;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;
using System.Net.Sockets;
using System.Text;
using THandy.Interface;
using System.Linq;
using Android.Bluetooth;

namespace THandy.Views
{
    // シングルトンで呼び出すこと
    public partial class ScanReadPageClipBoard : ContentPage
    {
        /// <summary>
        /// ボタン処理中に画面遷移させない制御用
        /// </summary>       
        public static bool btnFanction = false;

        public string readkubun = "";

        // 画面再表示後に表示更新しない不具合があった。通信処理を作ったためメモリ開放しないのが原因と判断した。なのでシングルトンとする。
        private static ScanReadPageClipBoard scanReadPageClipBoard;
        public static ScanReadPageClipBoard GetInstance(string name1, string kubun)
        {
            ScanReadClipBoardViewModel.GetInstance().Initilize(name1, kubun);
            if (scanReadPageClipBoard == null)
            {
                scanReadPageClipBoard = new ScanReadPageClipBoard();
            }

            scanReadPageClipBoard.readkubun = kubun;
            return scanReadPageClipBoard;
        }

        public ScanReadPageClipBoard()
        {
            InitializeComponent();
        }

        ~ScanReadPageClipBoard()
        {
            Console.WriteLine("#ScanReadPageClipBoard finish");
        }

        protected override void OnAppearing()
        {

            if (readkubun == "204")
            {
                QRstringAfter.IsVisible = false;
                QRstring.IsVisible = false;
                QRstringAfter2.IsVisible = false;
                QRstring2.IsVisible = false;

                PrintView.IsVisible = false;
                DkeyPrintView.IsVisible = true;
            }
            else
            {
                QRstringAfter.IsVisible = true;
                QRstring.IsVisible = true;
                QRstringAfter2.IsVisible = true;
                QRstring2.IsVisible = true;

                PrintView.IsVisible = true;
                DkeyPrintView.IsVisible = false;
            }

            scankidou();
        }

        private void scankidou()
        {
            base.OnAppearing();
            BindingContext = ScanReadClipBoardViewModel.GetInstance();
        }

        private async void PickScanModeSelectedIndexChanged(object sender, EventArgs e)
        {
            ScanReadClipBoardViewModel vm = ScanReadClipBoardViewModel.GetInstance();
            // ボタン押下チェック(連打対策)
            if (!btnFanction)
            {
                btnFanction = true; //ボタン押下不可
                vm.ActivityRunning = true;
                await PickScanModeSelectedIndexChanged();
                vm.ActivityRunning = false;
                btnFanction = false; //ボタン押下可
            }
        }

        private async Task PickScanModeSelectedIndexChanged()
        {
            try
            {
                int index = PickScanMode.SelectedIndex;
                ScanReadClipBoardViewModel vm = ScanReadClipBoardViewModel.GetInstance();
                // 表示完了後のみイベント拾う
                if (true == vm.bCompletedDsp && index >= 0)
                {
                    string strSelectItem = PickScanMode.Items[index];

                    if (strSelectItem.Equals(Common.Const.C_SCANNAME_KEYBOARD))
                    {
                        //クリップボードからキーボード切替
                        List<Setei> Set1 = await App.DataBase.GetSeteiAsync();
                        Setei Set2 = Set1[0];
                        Set2.ScanMode = Common.Const.C_SCANMODE_KEYBOARD;
                        await App.DataBase.SavSeteiAsync(Set2);

                        Page page = ScanReadPageKeyBoard.GetInstance(vm.nameA, vm.Readkubun);
                        Navigation.InsertPageBefore(page, this);
                        await Navigation.PopAsync();
                        vm.DisposeEvent();
                    }
                    else if (strSelectItem.Equals(Common.Const.C_SCANNAME_CAMERA))
                    {
                        //クリップボードからカメラ切替
                        List<Setei> Set1 = await App.DataBase.GetSeteiAsync();
                        Setei Set2 = Set1[0];
                        Set2.ScanMode = Common.Const.C_SCANMODE_CAMERA;
                        await App.DataBase.SavSeteiAsync(Set2);

                        Page page = ScanReadPageCamera.GetInstance(vm.nameA, vm.Readkubun);
                        Navigation.InsertPageBefore(page, this);
                        await Navigation.PopAsync();
                        vm.DisposeEvent();
                    }
                    else if (strSelectItem.Equals(Common.Const.C_SCANNAME_BARCODE))
                    {
                        //クリップボードからバーコードリーダ切替
                        List<Setei> Set1 = await App.DataBase.GetSeteiAsync();
                        Setei Set2 = Set1[0];
                        Set2.ScanMode = Common.Const.C_SCANMODE_BARCODE;
                        await App.DataBase.SavSeteiAsync(Set2);

                        Page page = ScanReadPageBarcode.GetInstance(vm.nameA, vm.Readkubun);
                        Navigation.InsertPageBefore(page, this);
                        await Navigation.PopAsync();
                        vm.DisposeEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("#PickScanModeSelectedIndexChanged Err {0}", ex.ToString());
            }
        }

        private async void PrintButton_ClickedAsync(object sender, EventArgs e)
        {
            //List<ScanReadData> sagyoUsers = await App.DataBase.GetScanReadDataAsync(readkubun);
            List<NouhinJL> nouhinJL204 = await App.DataBase.GetNouhinJLAsync();
            if (nouhinJL204.Count == 0)
            {
                await DisplayAlert("データ無し", "スキャンしてください", "OK");
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

                double latitude, longitude;//緯度、経度
                latitude = 0.0;
                longitude = 0.0;

                var postList = new List<BarModel>();
                for (int x = 0; x <= nouhinJL204.Count - 1; x++)
                {
                    var postItem = new BarModel();
                    postItem.Shori = "S1";
                    postItem.WID = WID;
                    postItem.UserID = user;
                    postItem.Device = Device;
                    postItem.Shorikubun = readkubun;
                    //postItem.BarcodeRead = sagyoUsers[x].Scanstring;
                    //postItem.BarcodeRead1 = sagyoUsers[x].Scanstring2;
                    postItem.ShouhinCode = nouhinJL204[x].JLBUNO;
                    postItem.NyukoSuryo = nouhinJL204[x].JLNKSU;

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
                    await Application.Current.MainPage.DisplayAlert("サーバーエラー", "代表キーを取得できませんでした。", "OK");
                    return;
                }
                else if (items2.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("サーバーエラー", "代表キーを取得できませんでした。", "OK");
                    return;
                }
                else if (items2.Count > 0)
                {
                    message1 = "";
                    //エラーチェック
                    for (int i = 0; i <= items2.Count - 1; i++)
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
                            else if (items2[i]["Name"].ToString() == "0")
                            {
                                //現品票番号が1件も存在しない
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("エラー", "読取データが存在しません", "OK");
                                return;
                            }
                            else if (items2[i]["Name"].ToString() == "NG")
                            {
                                //納品書データ
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("エラー", "代表キー検索に失敗しました", "OK");
                                return;
                            }
                            else
                            {
                                bool PrintReady = false;

                                string dkey = "";
                                string dkeySetDate = "";
                                string dkeyTotalCount = "";

                                // 複数の代表バーコードが該当した場合は、選択式にする。
                                if (items2.Count > 1)
                                {
                                    string dkeyDateLabel = "最終入庫：";
                                    string dkeyCountLabel = "箱　　数：";

                                    var list = new List<string>();
                                    for (int x = 0; x < items2.Count; x++)
                                    {
                                        var _dkey = items2[x]["DKEY"].ToString();
                                        var _dkeyDate = items2[x]["DKEYDATE"].ToString();
                                        var _dkeyCount = items2[x]["DKEYCOUNT"].ToString();
                                        list.Add(
                                            //"\r\n" +
                                            "#" + (x + 1) + " " + _dkey
                                            + "\r\n" + dkeyDateLabel + _dkeyDate
                                            + "\r\n" + dkeyCountLabel + _dkeyCount
                                            + "\r\n"
                                            );
                                    }

                                    var action = await DisplayActionSheet("選択してください", "キャンセル", null, list.ToArray());

                                    if (String.IsNullOrEmpty(action) || action == "キャンセル")
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        int numberIndexStart = action.IndexOf("#");
                                        int numberIndexEnd = action.IndexOf(" ");
                                        string numberString = action.Substring(numberIndexStart + 1, numberIndexEnd - (numberIndexStart + 1));

                                        int number;
                                        if (int.TryParse(numberString, out number))
                                        {
                                            int rowNumber = number - 1;
                                            dkey = items2[rowNumber]["DKEY"].ToString();
                                            dkeySetDate = items2[rowNumber]["DKEYDATE"].ToString();
                                            dkeyTotalCount = items2[rowNumber]["DKEYCOUNT"].ToString();

                                            PrintReady = true;
                                        }
                                        else
                                        {
                                            await Application.Current.MainPage.DisplayAlert("エラー", "選択内容を判別できませんでした。", "OK");
                                            return;
                                        }

                                    }

                                }
                                else if (items2.Count == 1)
                                {
                                    dkey = items2[i]["DKEY"].ToString();
                                    dkeySetDate = items2[i]["DKEYDATE"].ToString();
                                    dkeyTotalCount = items2[i]["DKEYCOUNT"].ToString();

                                    PrintReady = true;
                                }

                                // 代表バーコードの印刷
                                if (PrintReady)
                                {
                                    string dialogText = 
                                        " 代表キー：" + dkey + "\r\n" +
                                        " セット日：" + dkeySetDate + "\r\n" +
                                        " セット数：" + dkeyTotalCount;

                                    //DisplayAlertの表示
                                    var result = await DisplayAlert("印刷を開始します", dialogText, "OK", "キャンセル");

                                    if (result)
                                    {
                                        var dkeyError = "";

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
                                                string setDate = dkeySetDate;
                                                string yosu = dkeyTotalCount;
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
                                            dkeyError = "代表バーコードの印刷は失敗しました。（タイムアウト）";
                                            await Application.Current.MainPage.DisplayAlert("エラー", dkeyError, "OK");
                                            return;
                                        }
                                        catch (AggregateException ex)
                                        {
                                            if (ex.InnerException is SocketException)
                                            {
                                                // 接続失敗拒否
                                                dkeyError = "代表バーコードの印刷は失敗しました。（接続失敗）";
                                                return;
                                            }
                                            else
                                            {
                                                // その他のエラー
                                                dkeyError = "代表バーコードの印刷は失敗しました。";
                                            }
                                            await Application.Current.MainPage.DisplayAlert("エラー", dkeyError, "OK");
                                            return;
                                        }
                                        catch (Exception ex)
                                        {
                                            // その他のエラー
                                            dkeyError = "代表バーコードの印刷は失敗しました。";
                                            await Application.Current.MainPage.DisplayAlert("エラー", dkeyError, "OK");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }

                                }
                            }
                        }
                        catch (Exception e1)
                        {
                            //エラー
                            await Application.Current.MainPage.DisplayAlert("エラー", "システムエラー", "OK");
                            return;
                        }
                    }
                }

                OnAppearing();
            }

        }
    }
}