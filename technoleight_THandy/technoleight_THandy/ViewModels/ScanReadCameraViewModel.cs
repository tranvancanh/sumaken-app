using THandy.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.SimpleAudioPlayer;
using System.IO;
using System;
using System.Text.RegularExpressions;
using THandy;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;

namespace THandy.ViewModels
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

        public void Initilize(string name1, string kubun)
        {
            base.init(name1, kubun);
            OnScan = new Command<ZXing.Result>(OnScanClicked);
        }

        private async void OnScanClicked(ZXing.Result result)
        {
            //読取処理
            try
            {
                await UpdateReadData(result.Text.Trim(), Common.Const.C_SCANMODE_CAMERA);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("#OnScanClicked Err {0}", e.ToString());
            }

        }

        protected override int getDensoStartBit(string strCode)
        {
            return strCode.IndexOf("D234") + 6;
        }

    }
}
