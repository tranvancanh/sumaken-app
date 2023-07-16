using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using technoleight_THandy.Views;
using technoleight_THandy.Models;
using technoleight_THandy.Data;
using technoleight_THandy.Driver;
using System.Collections.ObjectModel;
using System.ComponentModel;
using technoleight_THandy.Interface;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace technoleight_THandy.ViewModels
{
    public class SeteiViewModel : BaseViewModel
    {

        private INavigation Navigation;

        public Command TourokuCommand { get; }
        public Command CancelCommand { get; }

        public Command DelShipCommand { get; }
        public Command DeleteDemoDataCommand { get; }
        public Command DeleteAllSettingCommand { get; }

        // バーコードリーダ情報
        private List<BTDevice> lstBTDevice = new List<BTDevice>();

        private Sound sound = new Sound();

        public SeteiViewModel(INavigation navigation)
        {
            ActivityRunningLoading();

            Navigation = navigation;
            TourokuCommand = new Command(OnTourokuClicked);
            CancelCommand = new Command(OnCancelClicked);
            DelShipCommand = new Command(OnDelShipClicked);
            DeleteAllSettingCommand = new Command(OnDeleteAllSettingClicked);

            Task.Run(async () => { await Init(); }).Wait();

            ActivityRunningEnd();
        }

        public async Task Init()
        {
            await List1();
            await SetScanModePicker();
        }

        private async Task List1()
        {
            List<Setting.SettingSqlLite> settings = await App.DataBase.GetSettingAsync();
            if (settings.Count > 0)
            {
                TxtWID = settings[0].CompanyCode;
                Txturl = settings[0].HandyApiUrl;
                Txtpass = settings[0].CompanyPassword;
                Txtuser = settings[0].HandyUserCode;
                TxtDevice = settings[0].Device;

                if (Device.RuntimePlatform == Device.Android)
                {
                    IsVisibleScanMode = true;
                    IsVisiblePickBarcode = true;
                    setBarcodeReaderInfo(settings[0].ScanReader, settings[0].UUID);
                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    IsVisibleScanMode = false;
                }
                else
                {
                    IsVisiblePickBarcode = false;
                }

            }
            else
            {
                //APIClass Ap = new APIClass();
                Txturl = "https://www.tozan.co.jp/WarehouseWebApi/api/v1.0/";
                if (Device.RuntimePlatform == Device.Android)
                {
                    IsVisibleScanMode = true;
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


        private async Task SetScanModePicker()
        {
            var pickScamModeItems = new ObservableCollection<string>
            {
                Common.Const.C_SCANNAME_CAMERA,
                //Common.Const.C_SCANNAME_BARCODE,
                Common.Const.C_SCANNAME_CLIPBOARD
            };

            if (Device.RuntimePlatform == Device.Android)
            {

            }
            PickScamModeItems = pickScamModeItems;

            List<Setting.SettingSqlLite> Set2 = await App.DataBase.GetSettingAsync();
            if (Set2.Count > 0)
            {
                var scanMode = Set2[0].ScanMode;
                PickScamModeSelectItem = scanMode;
            }
            else
            {
                string manufacturerName = DependencyService.Get<IDeviceService>().GetManufacturerName();
                if (manufacturerName.StartsWith("DENSO"))
                {
                    PickScamModeSelectItem = Common.Const.C_SCANNAME_CLIPBOARD;
                }
                else
                {
                    PickScamModeSelectItem = Common.Const.C_SCANNAME_CAMERA;
                }
            }

        }

        private async void OnTourokuClicked(object obj)
        {
            await Task.Run(() => ActivityRunningProcessing());

            var result = await Touroku(obj);

            await Task.Run(() => ActivityRunningEnd());

            if (result.result)
            {
                await App.DisplayAlertOkey();
                Application.Current.MainPage = new LoginPage();
            }
            else
            {
                await App.DisplayAlertError(result.message);
                return;
            }

        }

        private async Task<(bool result, string message)> Touroku(object obj)
        {
            var settingSqlLite = new Setting.SettingSqlLite();

            var settingApiRequestBody = new Setting.SettingApiRequestBody();

            var requestUrl = Txturl == null ? "" : Txturl.Trim();
            settingApiRequestBody.CompanyCode = TxtWID == null ? "" : TxtWID.Trim();
            settingApiRequestBody.CompanyPassword = Txtpass == null ? "" : Txtpass.Trim();
            settingApiRequestBody.HandyUserCode =  Txtuser == null ? "" : Txtuser.Trim();

            if (requestUrl == "" || settingApiRequestBody.CompanyCode == "" || settingApiRequestBody.CompanyPassword == "" || settingApiRequestBody.HandyUserCode == "")
            {
                return (false, "未入力項目があります");
            }

            try
            {
                var jsonDataSend = JsonConvert.SerializeObject(settingApiRequestBody);
                var responseMessage = await App.API.PostMethod(jsonDataSend, requestUrl, "Setting");
                if (responseMessage.status == System.Net.HttpStatusCode.Created)
                {
                    var settingApiResponceBody = JsonConvert.DeserializeObject<Setting.SettingApiResponceBody>(responseMessage.content);

                    settingSqlLite.HandyApiUrl = requestUrl;

                    settingSqlLite.CompanyCode = settingApiRequestBody.CompanyCode;
                    settingSqlLite.CompanyPassword = settingApiRequestBody.CompanyPassword;
                    settingSqlLite.HandyUserCode = settingApiRequestBody.HandyUserCode;
                    settingSqlLite.Device = settingApiRequestBody.Device;
                    settingSqlLite.DeviceName = settingApiRequestBody.DeviceName;

                    settingSqlLite.CompanyID = settingApiResponceBody.CompanyID;
                    settingSqlLite.HandyUserID = settingApiResponceBody.HandyUserID;
                    settingSqlLite.PasswordMode = settingApiResponceBody.PasswordMode;
                }
                else
                {
                    return (false, responseMessage.content);
                }

            }
            catch (Exception ex)
            {
                return (false, null);
            }

            // スキャンリーダー情報取得
            string strBarcode_Reader = "";
            string strUuid = "";
            int index = PickBarcodeSelectedIndex;
            if (index >= 1)
            {
                strBarcode_Reader = lstBTDevice[index - 1].strName;
                strUuid = lstBTDevice[index - 1].strUuid;
            }

            // スキャンサウンド情報取得
            string soundOkeyDispName = sound.SoundOkeyList[PickSoundOkeySelectedIndex].Item;
            string soundErrorDispName = sound.SoundErrorList[PickSoundErrorSelectedIndex].Item;

            settingSqlLite.ScanMode = PickScamModeSelectItem;
            settingSqlLite.ScanReader = strBarcode_Reader;
            settingSqlLite.UUID = strUuid;
            settingSqlLite.ScanOkeySound = soundOkeyDispName;
            settingSqlLite.ScanErrorSound = soundErrorDispName;
            settingSqlLite.ColorTheme = themeColorPickerSelectItem;

            //SQLiteデータベース登録
            int saveSetting = await App.DataBase.SavSettingAsync(settingSqlLite);

            return (true, null);
        }

        public async void OnCancelClicked()
        {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "入力内容は破棄されます\n戻ってよろしいですか？", "Yes", "No");
            if (result)
            {
                Application.Current.MainPage = new LoginPage();
            }
        }

        private async Task<int> DeleteScanData()
        {
            await App.DataBase.DeleteAllScanReceiveSendData();
            await App.DataBase.DeleteAllScanReceive();
            return 1;
        }

        private async void OnDelShipClicked()
        {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "未登録のスキャンデータを削除します。よろしいですか？", "Yes", "No");
            if (result)
            {
                await DeleteScanData();
                await Application.Current.MainPage.DisplayAlert("完了", "未登録のスキャンデータ削除が完了しました。", "OK");
            }
            return;

        }

        private async void OnDeleteAllSettingClicked()
        {
            var result = await Application.Current.MainPage.DisplayAlert("警告", "アプリの設定を初期化します。よろしいですか？", "Yes", "No");
            if (result)
            {
                await DeleteScanData();
                await App.DataBase.ALLDeleteSettingAsync();

                await Application.Current.MainPage.DisplayAlert("完了", "アプリの設定を初期化しました。", "OK");
                Application.Current.MainPage = new SeteiPage();

            }

            return;
        }

        private string txturl;
        public string Txturl
        {
            get { return txturl; }
            set { SetProperty(ref txturl, value); }
        }
        private string txtWID;
        public string TxtWID
        {
            get { return txtWID; }
            set { SetProperty(ref txtWID, value); }
        }
        private string txtpass;
        public string Txtpass
        {
            get { return txtpass; }
            set { SetProperty(ref txtpass, value); }
        }
        private string txtuser;
        public string Txtuser
        {
            get { return txtuser; }
            set { SetProperty(ref txtuser, value); }
        }
        private string txtDevice;
        public string TxtDevice
        {
            get { return txtDevice; }
            set { SetProperty(ref txtDevice, value); }
        }
        private int pickSoundOkeySelectedIndex;
        public int PickSoundOkeySelectedIndex
        {
            get { return pickSoundOkeySelectedIndex; }
            set { SetProperty(ref pickSoundOkeySelectedIndex, value); }
        }
        private int pickSoundErrorSelectedIndex;
        public int PickSoundErrorSelectedIndex
        {
            get { return pickSoundErrorSelectedIndex; }
            set { SetProperty(ref pickSoundErrorSelectedIndex, value); }
        }
        private ObservableCollection<string> barcodeItems;
        public ObservableCollection<string> BarcodeItems
        {
            get { return barcodeItems; }
            set { SetProperty(ref barcodeItems, value); }
        }
        private ObservableCollection<string> pickScamModeItems;
        public ObservableCollection<string> PickScamModeItems
        {
            get { return pickScamModeItems; }
            set { SetProperty(ref pickScamModeItems, value); }
        }
        private string pickScamModeSelectItem;
        public string PickScamModeSelectItem
        {
            get { return pickScamModeSelectItem; }
            set { SetProperty(ref pickScamModeSelectItem, value); }
        }
        private int pickBarcodeSelectedIndex;
        public int PickBarcodeSelectedIndex
        {
            get { return pickBarcodeSelectedIndex; }
            set { SetProperty(ref pickBarcodeSelectedIndex, value); }
        }
        private bool isVisibleScanMode;
        public bool IsVisibleScanMode
        {
            get { return isVisibleScanMode; }
            set { SetProperty(ref isVisibleScanMode, value); }
        }
        private bool isVisiblePickBarcode;
        public bool IsVisiblePickBarcode
        {
            get { return isVisiblePickBarcode; }
            set { SetProperty(ref isVisiblePickBarcode, value); }
        }
        private Theme themeColorPickerSelectItem;
        public Theme ThemeColorPickerSelectItem
        {
            get { return themeColorPickerSelectItem; }
            set { SetProperty(ref themeColorPickerSelectItem, value); }
        }

    }
}
