using System;
using System.Diagnostics;
using System.Threading.Tasks;
using technoleight_THandy.Interface;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace technoleight_THandy.ViewModels
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
            Debug.WriteLine("#ScanReadClipBoardViewModel finish");
        }

        ScanReadClipBoardViewModel() { }

        public ScanReadClipBoardViewModel(string title, int pageID, INavigation navigation) 
        {
            Initilize(title, pageID, navigation);
        }

        public void Initilize(string title, int pageID, INavigation navigation)
        {
            CanReadClipBoard = false;
            //画面初期化
            base.Init(title, pageID, navigation);
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
                Debug.WriteLine("#OnScanClicked Err {0}", e.ToString());
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
                    Debug.WriteLine("#OnClipboardContentChanged {0}", clipboardText.Result);
                    await UpdateReadData(clipboardText.Result, Common.Const.C_SCANMODE_CLIPBOARD);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("#OnClipboardContentChanged Err {0}", ex.ToString());
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
    }
}
