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
using technoleight_THandy.Models.common;

namespace technoleight_THandy.ViewModels
{
    // シングルトンで呼び出すこと
    public class ScanCameraViewModel: LoginViewModel
    {
        public Command OnScan { get; set; }

        public ScanCameraViewModel()
        {
            Title = "QRでログイン";
            OnScanStart();
        }

        public void OnScanStart()
        {
            OnScan = new Command<ZXing.Result>((OnScanClicked) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //ScanFlag = false;  //読み取り停止
                    //FrameVisible = false;

                    if (!CameraQrcodeLoginScanFlag) return;
                    CameraQrcodeLoginScanFlag = false;

                    await UpdateReadData(OnScanClicked.Text.Trim());

                    await Task.Delay(1500);    //1秒待機

                    CameraQrcodeLoginScanFlag = true;

                    //FrameVisible = true;
                    //this.ScanFlag = true;   //読み取り再開
                });
            });
        }

        //private string scannedCodeString;
        //public string ScannedCodeString
        //{
        //    get { return scannedCodeString; }
        //    set { SetProperty(ref scannedCodeString, value); }
        //}

        //private bool isScanningFlag;
        //public bool IsScanningFlag
        //{
        //    get { return isScanningFlag; }
        //    set { SetProperty(ref isScanningFlag, value); }
        //}

        //private bool zxingIsVisible = false;
        //public bool ZxingIsVisible
        //{
        //    get { return zxingIsVisible; }
        //    set { SetProperty(ref zxingIsVisible, value); }
        //}

    }
}
