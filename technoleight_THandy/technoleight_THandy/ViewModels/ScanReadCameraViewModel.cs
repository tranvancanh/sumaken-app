using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

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
            Debug.WriteLine("#ScanReadCameraViewModel finish");
        }

        public void Initilize(string name1, int kubun, INavigation navigation)
        {
            ContentIsVisible = false;
            ActivityRunning = true;

            OnScanStart();
            base.Init(name1, kubun, navigation);

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
