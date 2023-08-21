using technoleight_THandy.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.SimpleAudioPlayer;
using System.IO;
using System;
using System.Text.RegularExpressions;
using technoleight_THandy;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using technoleight_THandy.Interface;
using technoleight_THandy.Data;
using technoleight_THandy.Event;

namespace technoleight_THandy.ViewModels
{
    // シングルトンで呼び出すこと
    public class ScanReadBarcodeViewModel : ScanReadViewModel
    {
        // 画面再表示後に表示更新しない。おそらく通信タスクを作ったためメモリ開放しないのが原因と判断した。なのでシングルトンとする。
        private static ScanReadBarcodeViewModel scanReadBarcodeViewModel;

        public static ScanReadBarcodeViewModel GetInstance()
        {
            if (scanReadBarcodeViewModel == null)
            {
                scanReadBarcodeViewModel = new ScanReadBarcodeViewModel();
                return scanReadBarcodeViewModel;
            }
            return scanReadBarcodeViewModel;
        }

        // BlueToothデバイス情報設定
        private async Task setGridBTInfo()
        {
            List<Setting.SettingSqlLite> Set2 = await App.DataBase.GetSettingAsync();
            if (Set2.Count > 0 && "" != Set2[0].ScanReader && "" != Set2[0].UUID && Device.RuntimePlatform == Device.Android)
            {
                StrName = Set2[0].ScanReader;
                StrUuid = Set2[0].UUID;
                StrState = Common.Const.C_CONNET_NG;
                ColorState = Color.Black;
                CanReceiveBarcode = true;
            }
            else
            {
                CanReceiveBarcode = false;
                await Application.Current.MainPage.DisplayAlert("バーコードリーダ未登録", "ログイン画面に戻り設定ボタンを押してバーコードリーダを登録してください。", "OK");
                return;
            }
        }

        public async void Initilize(string name1, int kubun, INavigation navigation)
        {
            //画面初期化
            base.Init(name1, kubun, navigation);

            // BlueToothデバイス情報設定
            await setGridBTInfo();

            // バーコードスキャナが登録済みならイベント拾う
            if (CanReceiveBarcode)
            {
                IBluetoothManager btMan = DependencyService.Get<IBluetoothManager>();
                btMan.DataReceived += (sender, e) =>
                {
                    Console.WriteLine("#socket Read Data {0}", e.Data);
                    NofityScanRead(e.Data);
                };
                btMan.NotifyConnet += (sender, e) =>
                {
                    Console.WriteLine("#socket connet {0}", e.Data);
                    NofityConneted(e.Data);
                };

                this.BTConnet();
            }
        }

        ~ScanReadBarcodeViewModel()
        {
            Console.WriteLine("#ScanReadBarcodeViewModel finish");
        }

        public void DisposeEvent()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                IBluetoothManager btMan = DependencyService.Get<IBluetoothManager>();
                btMan.DataReceived -= (sender, e) =>
                {
                    NofityScanRead(e.Data);
                };
                btMan.NotifyConnet -= (sender, e) =>
                {
                    NofityConneted(e.Data);
                };
                CanReceiveBarcode = false;
                this.BTDisConnet();
            }
        }

        private async void NofityScanRead(string strData)
        {
            //読取処理
            try
            {
                if (CanReceiveBarcode)
                {
                    await UpdateReadData(strData, Common.Const.C_SCANMODE_BARCODE);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("#NofityScanRead Err {0}", e.ToString());
            }
        }

        private void NofityConneted(string strData)
        {
            try
            {
                if (CanReceiveBarcode)
                {
                    StrState = strData;
                    if (strData == Common.Const.C_CONNET_OK)
                    {
                        ColorState = Color.Red;
                    }
                    else
                    {
                        ColorState = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("#OnConneted Err {0}", ex.ToString());
            }
        }

        public void BTConnet()
        {
            //接続
            IBluetoothManager btMan = DependencyService.Get<IBluetoothManager>();
            BTDevice bTDevice = new BTDevice();
            bTDevice.strName = StrName;
            bTDevice.strUuid = StrUuid;
            btMan.BTConnet(bTDevice);
        }

        public void BTDisConnet()
        {
            //切断
            IBluetoothManager btMan = DependencyService.Get<IBluetoothManager>();
            btMan.BTDisConnet();
        }
    }
}
