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
using static technoleight_THandy.Common.Enums;

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
            this.GetHandyApiUrl();
            Initilize(title, pageID, navigation);
        }

        
        public void Initilize(string title, int pageID, INavigation navigation)
        {
            CanReadClipBoard = false;
            title = "荷取り";
            //画面初期化
            base.Init(title, pageID, navigation);
            Address1 = string.Empty;
            Address2 = string.Empty;
            Address3 = string.Empty;
            AGFState = Enums.AGFShijiState.Nitori; // 荷取りST
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

        private async void OnClipboardContentChanged(object sender, EventArgs e)
        {
            var hasText = Clipboard.HasText;
            try
            {
                if (CanReadClipBoard && MainThread.IsMainThread)
                {
                    IClipBoard clipBoard = DependencyService.Get<IClipBoard>();
                    var clipboardText = await clipBoard.GetTextFromClipBoardAsync();
                    System.Console.WriteLine("#OnClipboardContentChanged {0}", clipboardText);
                    await UpdateReadCheckData(clipboardText, Common.Const.C_SCANMODE_CLIPBOARD, PageID);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("#OnClipboardContentChanged Err {0}", ex.ToString());
            }
        }
        //public void DisposeEvent()
        //{
        //    CanReadClipBoard = false;
        //    if (Device.RuntimePlatform == Device.Android)
        //    {
        //        Clipboard.ClipboardContentChanged -= OnClipboardContentChanged;
        //    }
        //}

        public void DisposeEvent2()
        {
            CanReadClipBoard = false;
            if (Device.RuntimePlatform == Device.Android)
            {
                Clipboard.ClipboardContentChanged -= OnClipboardContentChanged;
            }
        }

    }
}
