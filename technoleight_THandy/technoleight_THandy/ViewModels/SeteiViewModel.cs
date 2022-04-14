using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using THandy.Views;
using THandy.Models;
using THandy.Data;
using THandy.Driver;
using System.Collections.ObjectModel;
using System.ComponentModel;
using THandy.Interface;

namespace THandy.ViewModels
{
    public class SeteiViewModel : INotifyPropertyChanged
    {

        private INavigation navigation;

        //View側に変更を教えるため
        public event PropertyChangedEventHandler PropertyChanged;

        public Command TourokuCommand { get; }
        public Command CancelCommand { get; }
        //public Users Log1 = new Users(); 

        public Command DelShipCommand { get; }

        private bool btnFanction = false;

        // バーコードリーダ情報
        private List<BTDevice> lstBTDevice = new List<BTDevice>();


        public SeteiViewModel(INavigation navigation)
        {
            this.navigation = navigation;
            TourokuCommand = new Command(OnTourokuClicked);
            CancelCommand = new Command(OnCancelClicked);
            DelShipCommand = new Command(OnDelShipClicked);
            List1();
        }

        private async void List1()
        {
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            if (Set2.Count>0) {
                TxtWID = Set2[0].WID;
                Txturl = Set2[0].url;
                Txtpass = Set2[0].k_pass;
                Txtuser = Set2[0].user;
                TxtDevice = Set2[0].Device;
                if (Device.RuntimePlatform == Device.Android)
                {
                    IsVisiblePickBarcode = true;
                    setBarcodeReaderInfo(Set2[0].BarcodeReader, Set2[0].UUID);
                }
                else
                {
                    IsVisiblePickBarcode = false;
                }
            } else
            {
                APIClass Ap = new APIClass();
                Txturl = Ap.API_URL;
                if (Device.RuntimePlatform == Device.Android)
                {
                    IsVisiblePickBarcode = true;
                    setBarcodeReaderInfo("", "");
                }
                else
                {
                    IsVisiblePickBarcode = false;
                }
            }

        }

        private void setBarcodeReaderInfo(string BarcodeReader, string UUID)
        {
            IBluetoothManager btMan = DependencyService.Get<IBluetoothManager>();
            List<BTDevice> listBTDevices = btMan.GetBondedDevices();
            BarcodeItems = new ObservableCollection<string>();
            int index = -1;
            int iLp = -1;
            BarcodeItems.Add(Common.Const.C_MSG_NONE_PAIRING_INFO);
            if (listBTDevices != null && listBTDevices.Count > 0)
            {
                foreach (var device in listBTDevices)
                {
                    iLp += 1;
                    BarcodeItems.Add(device.strName);
                    lstBTDevice.Add(device);
                    if (device.strName.Equals(BarcodeReader) && device.strUuid.Equals(UUID))
                    {
                        index = iLp;
                    }        
                }
                PickBarcodeSelectedIndex = index + 1;
            }
        }

        private async void OnTourokuClicked(object obj)
        {
            if (btnFanction) return;

            btnFanction = true;

            //設定ファイル登録
            string WID = TxtWID;
            string url = Txturl;
            string k_pass = Txtpass;
            string user = Txtuser;
            string Device = DependencyService.Get<IDeviceService>().GetID();
            string manufacturer = DependencyService.Get<IDeviceService>().GetManufacturerName();
            string Model = DependencyService.Get<IDeviceService>().GetModelName();
            string osVersion = DependencyService.Get<IDeviceService>().GetDeviceVersion();
            string DeviceName = manufacturer + "[" + Model + "]-" +  osVersion;

            //会社コードチェック
            List<Dictionary<string, string>> items1 = new List<Dictionary<string, string>>();
            items1.Add(new Dictionary<string, string>() { { "Shori", "S3" }, { "WID", WID }, { "k_Password", k_pass } });
            List<Dictionary<string, string>> items2 = await App.API.Post_method2(items1, "WUMaster", url);
            if (items2 == null)
            {
                btnFanction = false;
                await Application.Current.MainPage.DisplayAlert("会社コードエラー", "登録がありません。", "OK");
                return;
            }
            else if (items2.Count > 0)
            {
                string message1 = "";
                
                foreach (string Value in items2[0].Values)
                {
                    message1 = Value;
                }
                
                if (message1 == "NG")
                {
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("会社コードエラー", "登録がありません。", "OK");
                    return;
                }
                else if (message1 == Common.Const.C_ERR_VALUE_NETWORK)
                {
                    //ネットワーク
                    //エラー
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                    return;
                }
            }

            //ユーザーチェック
            items1 = new List<Dictionary<string, string>>();
            items1.Add(new Dictionary<string, string>() { { "Shori", "S2" }, { "WID", WID }, { "UserID", user } });
            items2 = await App.API.Post_method2(items1, "WUMaster", url);
            string PassMode = "0";
            string User_name = "";

            if (items2 == null)
            {
                btnFanction = false;
                await Application.Current.MainPage.DisplayAlert("ユーザーコードエラー", "登録がありません。", "OK");
                return;
            }
            else if (items2.Count > 0)
            {
                string message1 = "";
                
                //foreach (string Value in items2[0].Values)
                //{
                //    message1 = Value;
                //}
                foreach (Dictionary<string, string> items3 in items2)
                {
                    // ループ変数にKeyValuePairを使う
                    foreach (KeyValuePair<string, string> kv in items3)
                    {
                        if (kv.Key == "Name")
                        {
                            message1 = kv.Value;
                        }
                        if (kv.Key == "PassMode")
                        {
                            PassMode = kv.Value;
                        }
                        if (kv.Key == "User_name")
                        {
                            User_name = kv.Value;
                        }
                        if (kv.Key == Common.Const.C_ERR_KEY_NETWORK)
                        {
                            message1 = kv.Value;
                        }
                    }
                }

                if (message1 == "NG")
                {
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("ユーザーコードエラー", "登録がありません。", "OK");
                    return;
                }
                else if (message1 == Common.Const.C_ERR_VALUE_NETWORK)
                {
                    //ネットワーク
                    //エラー
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                    return;
                }
            }

            //メニュー確認 2021/04/06 K.Hoshino
            //SQLiteデータベース登録
            int ib = await App.DataBase.ALLDeleteMenuAsync();

            items1 = new List<Dictionary<string, string>>();
            items1.Add(new Dictionary<string, string>() { { "Shori", "S4" }, { "WID", WID } });
            items2 = await App.API.Post_method2(items1, "WUMaster", url);

            MenuX menux = new MenuX();
            
            if (items2 == null)
            {
                btnFanction = false;
                await Application.Current.MainPage.DisplayAlert("メニュー", "登録がありません。", "OK");
                return;
            }
            else if (items2.Count > 0)
            {
                string message1 = "";
                menux.WID = WID;

                foreach (Dictionary<string, string> items3 in items2)
                {
                    // ループ変数にKeyValuePairを使う
                    foreach (KeyValuePair<string, string> kv in items3)
                    {
                        switch (kv.Key)
                        {
                            case "gamen_id":
                                menux.gamen_id = kv.Value;
                                break;
                            case "gamen_edaban":
                                menux.gamen_edaban = kv.Value;
                                break;
                            case "gamen_name":
                                menux.gamen_name = kv.Value;
                                break;
                            case "Name":
                                message1 = kv.Value;
                                break;
                            case Common.Const.C_ERR_KEY_NETWORK:
                                message1 = kv.Value;
                                break;
                        }
                    }

                    //SQLiteデータベース登録
                    int ic = await App.DataBase.SavMenuAsync(menux);

                }

                if (message1 == "NG")
                {
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("メニュー", "登録がありません。", "OK");
                    return;
                }
                else if (message1 == Common.Const.C_ERR_VALUE_NETWORK)
                {
                    //ネットワーク
                    //エラー
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                    return;
                }
            }

            //メニュー確認 2021/04/13 K.Hoshino
            //SQLiteデータベース登録
            ib = await App.DataBase.ALLDeleteBarCodeMAsync();

            items1 = new List<Dictionary<string, string>>();
            items1.Add(new Dictionary<string, string>() { { "Shori", "S5" }, { "WID", WID } });
            items2 = await App.API.Post_method2(items1, "WUMaster", url);

            BarCodeM barCodem = new BarCodeM();

            if (items2 == null)
            {
                btnFanction = false;
                await Application.Current.MainPage.DisplayAlert("メニュー", "登録がありません。", "OK");
                return;
            }
            else if (items2.Count > 0)
            {
                string message1 = "";
                barCodem.WID = WID;

                foreach (Dictionary<string, string> items3 in items2)
                {
                    // ループ変数にKeyValuePairを使う
                    foreach (KeyValuePair<string, string> kv in items3)
                    {
                        switch (kv.Key)
                        {
                            case "gamen_id":
                                barCodem.gamen_id = kv.Value;
                                break;
                            case "gamen_edaban":
                                barCodem.gamen_edaban = kv.Value;
                                break;
                            case "edaban":
                                barCodem.edaban = kv.Value;
                                break;
                            case "IndexString":
                                barCodem.IndexString = kv.Value;
                                break;
                            case "BuhinStart":
                                barCodem.BuhinStart = kv.Value;
                                break;
                            case "BuhinEnd":
                                barCodem.BuhinEnd = kv.Value;
                                break;
                            case "SryouStart":
                                barCodem.SryouStart = kv.Value;
                                break;
                            case "SryouEnd":
                                barCodem.SryouEnd = kv.Value;
                                break;
                            case "TyoufukuOKFlag":
                                barCodem.TyoufukuOKFlag = kv.Value;
                                break;
                            case "SouRyouInputFlg":
                                barCodem.SouRyouInputFlg = kv.Value;
                                break;
                            case "Ketasu":
                                barCodem.Ketasu = kv.Value;
                                break;
                            case "Name":
                                message1 = kv.Value;
                                break;
                            case Common.Const.C_ERR_KEY_NETWORK:
                                message1 = kv.Value;
                                break;
                        }
                    }
                    //SQLiteデータベース登録
                    int ic = await App.DataBase.SaveBarCodeMAsync(barCodem);
                }

                // バーコードマスターの登録は必須ではないのでコメントアウト
                //if (message1 == "NG")
                //{
                //    btnFanction = false;
                //    await Application.Current.MainPage.DisplayAlert("バーコードマスター", "登録がありません。", "OK");
                //    return;
                //}
                if (message1 == Common.Const.C_ERR_VALUE_NETWORK)
                {
                    //ネットワーク
                    //エラー
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                    return;
                }
            }

            //バーコードリーダ情報取得
            string strBarcode_Reader = "";
            string strUuid = "";
            int index = PickBarcodeSelectedIndex;
            if (index >= 1)
            {
                strBarcode_Reader = lstBTDevice[index-1].strName;
                strUuid = lstBTDevice[index-1].strUuid;
            }

            //ユーザー登録

            Setei Set1 = new Setei();
            Set1.WID = WID;
            Set1.url = url;
            Set1.k_pass = k_pass;
            Set1.user = user;
            Set1.userpass = "";
            Set1.Device = Device;
            Set1.ScanMode = "";
            Set1.PassMode = PassMode;
            Set1.username = User_name;
            Set1.BarcodeReader = strBarcode_Reader;
            Set1.UUID = strUuid;

            //SQLiteデータベース登録
            int ia = await App.DataBase.SavSeteiAsync(Set1);
       
            if (ia == 0)
            {
                btnFanction = false;
                await Application.Current.MainPage.DisplayAlert("登録エラー", "値がおかしいです。", "OK");
                return;
            }
            else
            {
                //WEBサーバーにUpdate
                //SQLserver-ユーザーマスターにデバイス情報を登録
                items1 = new List<Dictionary<string, string>>();
                items1.Add(new Dictionary<string, string>() { { "Shori", "U1" }, { "WID", WID }, { "UserID", user }, { "Device", Device }, { "DeviceName", DeviceName } });
                items2 = await App.API.Post_method2(items1, "WUMaster", url);

                string message1 = "";
                foreach (Dictionary<string, string> items3 in items2)
                {
                    // ループ変数にKeyValuePairを使う
                    foreach (KeyValuePair<string, string> kv in items3)
                    {
                        if (kv.Key == "Name")
                        {
                            message1 = kv.Value;
                        }
                        if (kv.Key == Common.Const.C_ERR_KEY_NETWORK)
                        {
                            message1 = kv.Value;
                        }
                    }
                }

                if (message1 == "NG")
                {
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("デバイス情報登録エラー", "登録がありません。", "OK");
                    return;
                }
                else if (message1 == Common.Const.C_ERR_VALUE_NETWORK)
                {
                    //ネットワーク
                    //エラー
                    btnFanction = false;
                    await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                    return;
                }

                btnFanction = false;
                await Application.Current.MainPage.DisplayAlert("登録処理", "登録が完了しました。", "OK");
                Application.Current.MainPage = new LoginPage();
                return;
            }
           
        }

        private void OnCancelClicked()
        {
            Application.Current.MainPage = new LoginPage();
        }

        private async void OnDelShipClicked()
        {
            await App.DataBase.DeleteAllScanReadData();
            await App.DataBase.ALLDeleteJikuDBAsync();
            await App.DataBase.ALLDeleteNouhinAsync();
            await App.DataBase.ALLDeleteCarAsync();
            await App.DataBase.ALLDeleteCarDBAsync();
            await Application.Current.MainPage.DisplayAlert("削除処理", "削除が完了しました。", "OK");
        }

        private string txturl;
        public string Txturl
        {
            get { return txturl; }
            set
            {
                if (txturl != value)
                {
                    txturl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Txturl)));
                }
            }
        }

        private string txtWID;
        public string TxtWID
        {
            get { return txtWID; }
            set
            {
                if (txtWID != value)
                {
                    txtWID = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TxtWID)));
                }
            }
        }

        private string txtpass;
        public string Txtpass
        {
            get { return txtpass; }
            set
            {
                if (txtpass != value)
                {
                    txtpass = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Txtpass)));
                }
            }
        }

        private string txtuser;
        public string Txtuser
        {
            get { return txtuser; }
            set
            {
                if (txtuser != value)
                {
                    txtuser = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Txtuser)));
                }
            }
        }

        private string txtDevice;
        public string TxtDevice
        {
            get { return txtDevice; }
            set
            {
                if (txtDevice != value)
                {
                    txtDevice = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TxtDevice)));
                }
            }
        }

        private ObservableCollection<string> barcodeItems;
        public ObservableCollection<string> BarcodeItems
        {
            get { return barcodeItems; }
            set
            {
                if (barcodeItems != value)
                {
                    barcodeItems = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BarcodeItems)));
                }
            }
        }

        private int pickBarcodeSelectedIndex;
        public int PickBarcodeSelectedIndex
        {
            get { return pickBarcodeSelectedIndex; }
            set
            {
                if (pickBarcodeSelectedIndex != value)
                {
                    pickBarcodeSelectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PickBarcodeSelectedIndex)));
                }
            }
        }

        private bool isVisiblePickBarcode;
        public bool IsVisiblePickBarcode
        {
            get { return isVisiblePickBarcode; }
            set
            {
                if (isVisiblePickBarcode != value)
                {
                    isVisiblePickBarcode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisiblePickBarcode)));
                }
            }
        }
    }
}
