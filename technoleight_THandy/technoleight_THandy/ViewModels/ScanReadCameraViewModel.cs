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

namespace technoleight_THandy.ViewModels
{
    // シングルトンで呼び出すこと
    public class ScanReadCameraViewModel : ScanReadViewModel
    {
        public Command OnScan { get; set; }
        private static ScanReadCameraViewModel scanReadCameraViewModel;
        public static ScanReadCameraViewModel GetInstance()
        {
            if (scanReadCameraViewModel == null)
            {
                scanReadCameraViewModel = new ScanReadCameraViewModel();
                return scanReadCameraViewModel;
            }
            return scanReadCameraViewModel;
        }

        ~ScanReadCameraViewModel()
        {
            Console.WriteLine("#ScanReadCameraViewModel finish");
        }

        public void Initilize(string name1, int kubun, string receiptData, INavigation navigation)
        {
            ContentIsVisible = false;
            ActivityRunning = true;

            OnScanStart();
            base.Init(name1, kubun, receiptData, navigation);

            ActivityRunning = false;
            ContentIsVisible = true;
        }

        public void OnScanStart()
        {
            OnScan = new Command<ZXing.Result>((OnScanClicked) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    this.IsAnalyzing = false;  //読み取り停止
                    FrameVisible = true;       //Frameを表示
                    ScannedCodeString = OnScanClicked.Text;

                    await UpdateReadData(OnScanClicked.Text.Trim(), Common.Const.C_SCANMODE_CAMERA);

                    await Task.Delay(1500);    //1秒待機
                    FrameVisible = false;      //Frameを非表示
                    this.IsAnalyzing = true;   //読み取り再開
                });
            });
        }

    }
}
