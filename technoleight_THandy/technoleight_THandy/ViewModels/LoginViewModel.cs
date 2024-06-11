
using Newtonsoft.Json;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using technoleight_THandy.common;
using technoleight_THandy.Common;
using technoleight_THandy.Data;
using technoleight_THandy.Interface;
using technoleight_THandy.Models;
using technoleight_THandy.Models.common;
using technoleight_THandy.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace technoleight_THandy.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public Command SetUpCommand { get; }
        //public Command CameraQrcodeLoginCommand { get; }

        //private bool IsQrScanLogin;

        public static bool ClipboardScanFlag { get; set; }

        public Action ViewsideAction { get; set; }

        public LoginViewModel()
        {
            ActivityRunningLoading();

            LoginIconImageSource = ImageSource.FromResource("technoleight_THandy.img.login_img.png");
            //LoginIconImageSource = ImageSource.FromResource("technoleight_THandy.img.menu_img.png");
            SettingIcon = ImageSource.FromResource("technoleight_THandy.img.setting_icon.png");
            LoginCommand = new Command(OnLoginClicked);
            SetUpCommand = new Command(OnSetUpClicked);
            //CameraQrcodeLoginCommand = new Command(OnCameraQrcodeLoginClicked);

            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Init();

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        DependencyService.Get<IThemeColor>().SetStatusBarColor();
                    }
                });

                List();

            }
            catch (Exception ex)
            {

            }

            ActivityRunningEnd();
        }

        public async Task Init()
        {

            if (Decimal.TryParse(GetAppVersion(), out decimal appVersion))
            {
                AppVersion = appVersion;
            }
            else
            {
                await App.DisplayAlertError("アプリバージョンの取得に失敗しました");
                return;
            }

        }

        private void List()
        {
            IsPass = true;
            NotSetting = false;

            if (App.Setting.CompanyID > 0)
            {
                Txtuser = App.Setting.HandyUserCode;
                if (App.Setting.PasswordMode == 1)
                {
                    IsPass = true;
                }
                else
                {
                    IsPass = false;
                }

            }
            else
            {
                NotSetting = true;
            }

            if (App.Setting.ScanMode == Const.C_SCANNAME_CAMERA)
            {
                CameraQrcodeLoginButtonIsVisible = true;
                CameraQrcodeLoginScanFlag = true;
            }
            else if (App.Setting.ScanMode == Const.C_SCANNAME_CLIPBOARD)
            {
                //読取処理
                try
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        Clipboard.ClipboardContentChanged += OnClipboardContentChanged;

                        CameraQrcodeLoginButtonIsVisible = false;
                        CameraQrcodeLoginScanFlag = false;
                        ClipboardScanFlag = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("#OnScanClicked Err {0}", e.ToString());
                }
            }
            else
            {
                CameraQrcodeLoginButtonIsVisible = true;
                CameraQrcodeLoginScanFlag = true;
            }

        }

        protected async void OnClipboardContentChanged(object sender, EventArgs e)
        {
            if (!ClipboardScanFlag) return;
            ClipboardScanFlag = false;

            try
            {
                IClipBoard clipBoard = DependencyService.Get<IClipBoard>();
                Task<string> clipboardText = clipBoard.GetTextFromClipBoardAsync();
                Debug.WriteLine("#OnClipboardContentChanged {0}", clipboardText.Result);
                await UpdateReadData(clipboardText.Result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("#OnClipboardContentChanged Err {0}", ex.ToString());
            }

            ClipboardScanFlag = true;
        }

        public async Task UpdateReadData(string strScannedCode)
        {
            if (MainThread.IsMainThread)
            {
                await UpdateReadDataOnMainThread(strScannedCode);
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await UpdateReadDataOnMainThread(strScannedCode);
                });
            }
        }

        public async Task UpdateReadDataOnMainThread(string strScannedCode)
        {

            try
            {
                // 読取処理
                string ID = strScannedCode;

                if (ID.StartsWith(Common.Const.SCAN_NAMETAG_STRING))
                {
                    if (Util.NameTagQrcodeCheck(ID))
                    {
                        // OK
                        await Task.Run(() => ActivityRunningLoading());

                        await OnLoginClickedExcute(true);

                        await Task.Run(() => ActivityRunningEnd());
                    }
                }
                else
                {
                    return;
                }

            }
            catch (CustomExtention ex)
            {
                ScanMessage = ex.Message;
                FrameVisible = true;
                await Task.Delay(2000);    // 待機
                FrameVisible = false;
            }
            catch (Exception ex)
            {
                ScanMessage = Common.Const.SCAN_NAMETAG_ERROR;
                FrameVisible = true;
                await Task.Delay(2000);    // 待機
                FrameVisible = false;
            }
            finally
            {

            }

        }

        private async void OnLoginClicked()
        {
            await Task.Run(() => ActivityRunningLoading());

            await OnLoginClickedExcute();

            await Task.Run(() => ActivityRunningEnd());
        }

        //private async void OnCameraQrcodeLoginClicked(object obj)
        //{
        //    //if (!ZxingIsVisible)
        //    //{
        //    //}
        //    //else
        //    //{
        //    //}
        //    //new NavigationPage(new ScanCameraPage());
        //    await new NavigationPage().PushAsync(new ScanCameraPage());
        //}

        private async Task OnLoginClickedExcute(bool isQrLogin = false)
        {
            var loginUserSqlLite = new Login.LoginUserSqlLite();

            if (App.Setting.CompanyID == 0)
            {
                await App.DisplayAlertError("設定ボタンを押して初期登録をしてください");
                return;
            }

            //設定情報取得
            var loginApiRequestBody = new Login.LoginApiRequestBody();
            loginApiRequestBody.CompanyID = App.Setting.CompanyID;
            loginApiRequestBody.HandyUserID = App.Setting.HandyUserID;
            loginApiRequestBody.HandyUserCode = App.Setting.HandyUserCode;
            loginApiRequestBody.PasswordMode = App.Setting.PasswordMode;
            loginApiRequestBody.Device = App.Setting.Device;
            loginApiRequestBody.HandyUserPassword = (Txtpass == null) ? "" : Txtpass.Trim();
            loginApiRequestBody.HandyAppVersion = Convert.ToDecimal(AppVersion);

            try
            {
                //ユーザー情報取得
                var jsonDataSend = JsonConvert.SerializeObject(loginApiRequestBody);
                var loginResponse = await App.API.PostMethod(jsonDataSend, App.Setting.HandyApiUrl, "Login");
                if (loginResponse.status == System.Net.HttpStatusCode.OK)
                {
                    var loginApiResponceBody = JsonConvert.DeserializeObject<Login.LoginApiResponceBody>(loginResponse.content);

                    loginUserSqlLite.CompanyID = App.Setting.CompanyID;
                    loginUserSqlLite.CompanyCode = App.Setting.CompanyCode;
                    loginUserSqlLite.HandyUserCode = App.Setting.HandyUserCode;
                    loginUserSqlLite.CompanyName = loginApiResponceBody.CompanyName;
                    loginUserSqlLite.HandyUserName = loginApiResponceBody.HandyUserName;
                    loginUserSqlLite.AdministratorFlag = loginApiResponceBody.AdministratorFlag;
                    loginUserSqlLite.DepoID = loginApiResponceBody.DepoID;
                    loginUserSqlLite.DepoCode = loginApiResponceBody.DepoCode;
                    loginUserSqlLite.DepoName= loginApiResponceBody.DepoName;
                    loginUserSqlLite.DefaultHandyPageID = loginApiResponceBody.DefaultHandyPageID;
                    loginUserSqlLite.HandyVersion = loginApiRequestBody.HandyAppVersion;
                    //loginUserSqlLite.DefaultHandyPageID = 206;
                }
                else
                {
                    await App.DisplayAlertError(loginResponse.content);
                    return;
                }

            }
            catch (Exception ex)
            {
                await App.DisplayAlertError();
                return;
            }

            var handyPages = new List<MenuX>();
            var handyPageGetUrl = App.Setting.HandyApiUrl + "HandyPage";
            handyPageGetUrl = Util.AddCompanyPath(handyPageGetUrl, App.Setting.CompanyID);
            handyPageGetUrl = Util.AddParameter(handyPageGetUrl, "depoID", loginUserSqlLite.DepoID.ToString());
            handyPageGetUrl = Util.AddParameter(handyPageGetUrl, "administratorFlag", loginUserSqlLite.AdministratorFlag.ToString());
            handyPageGetUrl = Util.AddParameter(handyPageGetUrl, "handyUserID", App.Setting.HandyUserID.ToString());

            var handyPageResponse = await App.API.GetMethod(handyPageGetUrl);
            if (handyPageResponse.status == System.Net.HttpStatusCode.OK)
            {
                var handyPageApiResponceContent = JsonConvert.DeserializeObject<List<MenuX>>(handyPageResponse.content);
                handyPages = handyPageApiResponceContent;
            }
            else
            {
                await App.DisplayAlertError(handyPageResponse.content);
                return;
            }

            // SQLiteデータベース登録
            await App.DataBase.SavLoginAsync(loginUserSqlLite);
            await App.DataBase.ALLDeleteMenuAsync();
            foreach (var page in handyPages)
            {
                await App.DataBase.SavMenuAsync(page);
            }

            try
            {
                await Task.Run(() => DisposeEvent());

                if (isQrLogin)
                {
                    if (loginUserSqlLite.DefaultHandyPageID > 0)
                    {
                        //string pageName = handyPages.Where(x => x.HandyPageID == loginUserSqlLite.DefaultHandyPageID).FirstOrDefault().HandyPageName;
                        //await Util.HandyPagePush(loginUserSqlLite.DefaultHandyPageID, pageName);
                        Application.Current.MainPage = new MainPage(true);
                    }
                    else
                    {
                        Application.Current.MainPage = new MainPage();
                    }
                }
                else
                {
                    Application.Current.MainPage = new MainPage();
                }
            }
            catch (Exception ex)
            {

            }
            // register Loaction
            await this.GetHandyApiUrl();
            await this.OnGeoLocator();
            return;
        }

        private async Task OnGeoLocator()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            // This logic will run on the background automatically
            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, false, new Plugin.Geolocator.Abstractions.ListenerSettings
            {
                ActivityType = Plugin.Geolocator.Abstractions.ActivityType.AutomotiveNavigation,
                AllowBackgroundUpdates = true,
                DeferLocationUpdates = false,
                DeferralDistanceMeters = 10,
                DeferralTime = TimeSpan.FromMinutes(1),
                ListenForSignificantChanges = true,
                PauseLocationUpdatesAutomatically = true
            });

            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
        }

        private void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            //locationLabel.Text += $"{e.Position.Latitude}, {e.Position.Longitude}, {e.Position.Timestamp.TimeOfDay}{Environment.NewLine}";
            var latitude = e.Position.Latitude;
            var longitude = e.Position.Longitude;
            var pos = $"{latitude}, {longitude}, {DateTime.Now}";
            Debug.WriteLine($"Position : {pos}, Date Time {DateTime.Now}");

            var position = new DeviceLocation()
            {
                UUID = Guid.NewGuid().ToString(),
                Latitude = latitude,
                Longitude = longitude,
                GetDateTime = DateTime.Now
            };

            //await App.DataBase.UpdatePositionAsync(position);
            App.AppLocation = position;
#if DEBUG
            //デバイスの地位の取得の仕方
            //var logging = $"{App.SettingAgf.HandyApiAgfUrl}AgfCommons/CurrentPosition";
            //logging = Util.AddCompanyPath(logging, App.Setting.CompanyID);
            //logging = Util.AddParameter(logging, "id", position.UUID);
            //logging = Util.AddParameter(logging, "latitude", position.Latitude.ToString());
            //logging = Util.AddParameter(logging, "longitude", position.Longitude.ToString());
            //var shukaLaneNameTask = await App.API.GetMethod(logging);
#endif

        }

        public async Task GetHandyApiUrl()
        {
            var handyApiUrl = string.Empty;
            // SqlServerからデータをSELECT
            try
            {
                var getAgfUrl = App.Setting.HandyApiUrl + "ReturnAgfUrl";
                getAgfUrl = Util.AddCompanyPath(getAgfUrl, App.Setting.CompanyID);
                getAgfUrl = Util.AddParameter(getAgfUrl, "companyCode", App.Setting.CompanyCode);
                var responseAgfUrlMessage = await App.API.GetMethod(getAgfUrl);
                if (responseAgfUrlMessage.status != System.Net.HttpStatusCode.OK)
                {
                    await App.DisplayAlertError(responseAgfUrlMessage.content);
                    handyApiUrl = null;
                }
                handyApiUrl = responseAgfUrlMessage.content;
            }
            catch (Exception ex)
            {
                await App.DisplayAlertError(ex.Message);
                handyApiUrl = null;
            }
            var settingHandyApiAgfUrl = new Setting.SettingHandyApiAgfUrl()
            {
                HandyApiAgfUrl = handyApiUrl,
                CreateByUserID = App.Setting.HandyUserID,
                CreateDate = DateTime.Now
            };
            await App.DataBase.DeleteALLSettingHandyApiAgfUrlAsync();
            await App.DataBase.SaveSettingHandyApiAgfUrlAsync(settingHandyApiAgfUrl);
            await App.GetSettingAgf();
        }

        private string GetAppVersion()
        {
            var version = "";
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    version = AppInfo.VersionString;
                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    version = DependencyService.Get<IAssemblyService>().GetVersionName();
                }
            }
            catch (Exception  e)
            {

            }
            return version;
        }

        private void OnSetUpClicked()
        {
            Application.Current.MainPage = new SeteiPage();
        }

        public void DisposeEvent()
        {
            if (App.Setting.ScanMode == Const.C_SCANNAME_CLIPBOARD)
            {
                ClipboardScanFlag = false;
                if (Device.RuntimePlatform == Device.Android)
                {
                    Clipboard.ClipboardContentChanged -= OnClipboardContentChanged;
                }
            }
        }

        private ImageSource loginIconImageSource;
        public ImageSource LoginIconImageSource
        {
            get { return loginIconImageSource; }
            set { SetProperty(ref loginIconImageSource, value); }
        }

        private decimal appVersion;
        public decimal AppVersion
        {
            get { return appVersion; }
            set { SetProperty(ref appVersion, value); }
        }

        private string txtuser;
        public string Txtuser
        {
            get { return txtuser; }
            set { SetProperty(ref txtuser, value); }
        }

        private string txtpass;
        public string Txtpass
        {
            get { return txtpass; }
            set { SetProperty(ref txtpass, value); }
        }

        private bool isPass = true;
        public bool IsPass
        {
            get { return isPass; }
            set { SetProperty(ref isPass, value); }
        }

        private bool notSetting = true;
        public bool NotSetting
        {
            get { return notSetting; }
            set { SetProperty(ref notSetting, value); }
        }

        ImageSource settingIcon = "";
        public ImageSource SettingIcon
        {
            get { return settingIcon; }
            set
            { SetProperty(ref settingIcon, value); }
        }

        private bool cameraQrcodeLoginScanFlag;
        public bool CameraQrcodeLoginScanFlag
        {
            get { return cameraQrcodeLoginScanFlag; }
            set { SetProperty(ref cameraQrcodeLoginScanFlag, value); }
        }

        private bool cameraQrcodeLoginButtonIsVisible = false;
        public bool CameraQrcodeLoginButtonIsVisible
        {
            get { return cameraQrcodeLoginButtonIsVisible; }
            set { SetProperty(ref cameraQrcodeLoginButtonIsVisible, value); }
        }

        private bool zxingIsVisible = false;
        public bool ZxingIsVisible
        {
            get { return zxingIsVisible; }
            set { SetProperty(ref zxingIsVisible, value); }
        }

        private string scanMessage;
        public string ScanMessage
        {
            get { return scanMessage; }
            set{ SetProperty(ref scanMessage, value); }
        }

        private bool frameVisible;
        public bool FrameVisible
        {
            get { return frameVisible; }
            set { SetProperty(ref frameVisible, value); }
        }

    }
}
