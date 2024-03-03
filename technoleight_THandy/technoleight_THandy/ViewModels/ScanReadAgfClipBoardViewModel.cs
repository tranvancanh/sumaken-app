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
using technoleight_THandy.Common;

namespace technoleight_THandy.ViewModels
{
    // シングルトンで呼び出すこと
    public class ScanReadAgfClipBoardViewModel : ScanReadViewModel
    {
        private static ScanReadAgfClipBoardViewModel scanReadAgfClipBoardViewModel;
        

        public static ScanReadAgfClipBoardViewModel GetInstance()
        {
            if (scanReadAgfClipBoardViewModel == null)
            {
                scanReadAgfClipBoardViewModel = new ScanReadAgfClipBoardViewModel();
                return scanReadAgfClipBoardViewModel;
            }
            //await scanReadClipBoardViewModel.houji();
            return scanReadAgfClipBoardViewModel;
        }

        ~ScanReadAgfClipBoardViewModel()
        {
            Console.WriteLine("#ScanReadClipBoardViewModel finish");
        }

        ScanReadAgfClipBoardViewModel() { }

        public ScanReadAgfClipBoardViewModel(string title, int pageID, INavigation navigation) 
        {
            Initilize(title, pageID, navigation);
        }

        public void Initilize(string title, int pageID, INavigation navigation)
        {
            CanReadClipBoard = false;
            
            //画面初期化
            base.Init(title, pageID, navigation);
            Address3 = "";
            Address4 = "";
            Address5 = "";

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

        protected async void OnClipboardContentChanged(object sender, EventArgs e)
        {
            var hastext = Clipboard.HasText;
            try
            {
                if (CanReadClipBoard)
                {
                    IClipBoard clipBoard = DependencyService.Get<IClipBoard>();
                    Task<string> clipboardText = clipBoard.GetTextFromClipBoardAsync();
                   
                    System.Console.WriteLine("#OnClipboardContentChanged {0}", clipboardText.Result);

                    await UpdateReadCheckData(clipboardText.Result, Common.Const.C_SCANMODE_CLIPBOARD, PageID);
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

        public void DisposeEvent2()
        {
            CanReadClipBoard = false;
            if (Device.RuntimePlatform == Device.Android)
            {
                Clipboard.ClipboardContentChanged -= OnClipboardContentChanged;
            }
        }

        private string address3;
        public string Address3
        {
            get { return address3; }
            set { SetProperty(ref address3, value); }
        }

        private string address4;
        public string Address4
        {
            get { return address4; }
            set { SetProperty(ref address4, value); }
        }

        private string address5;
        public string Address5
        {
            get { return address5; }
            set { SetProperty(ref address5, value); }
        }

    }
}
