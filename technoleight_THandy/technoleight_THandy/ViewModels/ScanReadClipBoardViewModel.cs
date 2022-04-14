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
using THandy.Interface;

namespace THandy.ViewModels
{
    // シングルトンで呼び出すこと
    public class ScanReadClipBoardViewModel : ScanReadViewModel
    {
        private static ScanReadClipBoardViewModel scanReadClipBoardViewModel;
        

        public static ScanReadClipBoardViewModel GetInstance()
        {
            if (scanReadClipBoardViewModel == null)
            {
                scanReadClipBoardViewModel = new ScanReadClipBoardViewModel();
                return scanReadClipBoardViewModel;
            }
            //await scanReadClipBoardViewModel.houji();
            return scanReadClipBoardViewModel;
        }

        ~ScanReadClipBoardViewModel()
        {
            Console.WriteLine("#ScanReadClipBoardViewModel finish");
        }

        ScanReadClipBoardViewModel() { }

        public void Initilize(string name1, string kubun)
        {
            CanReadClipBoard = false;
            //画面初期化
            base.init(name1, kubun);
            //読取処理
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    Clipboard.ClipboardContentChanged += OnClipboardContentChanged;
                    CanReadClipBoard = true;
                }
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

        protected async void OnClipboardContentChanged(object sender, EventArgs e)
        {

            try
            {
                if (CanReadClipBoard)
                {
                    IClipBoard clipBoard = DependencyService.Get<IClipBoard>();
                    Task<string> clipboardText = clipBoard.GetTextFromClipBoardAsync();
                    System.Console.WriteLine("#OnClipboardContentChanged {0}", clipboardText.Result);
                    await UpdateReadData(clipboardText.Result, Common.Const.C_SCANMODE_CLIPBOARD);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("#OnClipboardContentChanged Err {0}", ex.ToString());
            }
        }
        public void DisposeEvent()
        {
            CanReadClipBoard = false;
            if (Device.RuntimePlatform == Device.Android)
            {
                Clipboard.ClipboardContentChanged -= OnClipboardContentChanged;
            }
        }
    }
}
