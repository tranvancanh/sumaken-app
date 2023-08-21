
using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using technoleight_THandy.Views;
using technoleight_THandy.Models;
using System.ComponentModel;
using technoleight_THandy.Data;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net;
using System.Linq.Expressions;
using Xamarin.Essentials;
using technoleight_THandy.Common;
using Newtonsoft.Json;
using System.Data.Common;
using technoleight_THandy.common;
using static technoleight_THandy.Models.Setting;
using static technoleight_THandy.Models.Login;
using technoleight_THandy.Interface;
using System.Linq;
using System.Net.NetworkInformation;
using static technoleight_THandy.Common.Enums;
using static technoleight_THandy.Models.ScanCommon;
using System.ComponentModel.Design;
using technoleight_THandy.Models.common;
using ZXing.Common.Detector;

namespace technoleight_THandy.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public Command SetUpCommand { get; }
        //public Command CameraQrcodeLoginCommand { get; }

        //private bool IsQrScanLogin;

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
                QrcodeLoginScanFlag = true;
            }
            else if (App.Setting.ScanMode == Const.C_SCANNAME_CLIPBOARD)
            {
                //読取処理
                try
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        Clipboard.ClipboardContentChanged += OnClipboardContentChanged;
                        QrcodeLoginScanFlag = true;
                        CameraQrcodeLoginButtonIsVisible = false;
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("#OnScanClicked Err {0}", e.ToString());
                }
            }
            else
            {
                CameraQrcodeLoginButtonIsVisible = true;
            }

        }

        protected async void OnClipboardContentChanged(object sender, EventArgs e)
        {
            try
            {
                IClipBoard clipBoard = DependencyService.Get<IClipBoard>();
                Task<string> clipboardText = clipBoard.GetTextFromClipBoardAsync();
                System.Console.WriteLine("#OnClipboardContentChanged {0}", clipboardText.Result);
                await UpdateReadData(clipboardText.Result);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("#OnClipboardContentChanged Err {0}", ex.ToString());
            }
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

            if (QrcodeLoginScanFlag)
            {
                QrcodeLoginScanFlag = false;
            }
            else
            {
                return;
            }

            try
            {
                // 読取処理
                string ID = strScannedCode;

                if (ID.StartsWith(Common.Const.SCAN_NAMETAG_STRING))
                {
                    if (await Util.NameTagQrcodeCheck(ID))
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

                return;
            }
            catch (Exception ex)
            {
                ScanMessage = Common.Const.SCAN_NAMETAG_ERROR;

                FrameVisible = true;
                await Task.Delay(2000);    // 待機
                FrameVisible = false;

                return;
            }
            finally
            {
                QrcodeLoginScanFlag = true;
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
                var jsonDataSend = JsonConvert.SerializeObject(loginApiRequestBody);
                var loginResponse = await App.API.PostMethod(jsonDataSend, App.Setting.HandyApiUrl, "Login");
                if (loginResponse.status == System.Net.HttpStatusCode.OK)
                {
                    var loginApiResponceBody = JsonConvert.DeserializeObject<Login.LoginApiResponceBody>(loginResponse.content);

                    loginUserSqlLite.CompanyCode = App.Setting.CompanyCode;
                    loginUserSqlLite.HandyUserCode = App.Setting.HandyUserCode;
                    loginUserSqlLite.CompanyName = loginApiResponceBody.CompanyName;
                    loginUserSqlLite.HandyUserName = loginApiResponceBody.HandyUserName;
                    loginUserSqlLite.AdministratorFlag = loginApiResponceBody.AdministratorFlag;
                    loginUserSqlLite.DepoID = loginApiResponceBody.DepoID;
                    loginUserSqlLite.DepoCode = loginApiResponceBody.DepoCode;
                    loginUserSqlLite.DepoName= loginApiResponceBody.DepoName;
                    loginUserSqlLite.DefaultHandyPageID = loginApiResponceBody.DefaultHandyPageID;
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

            return;
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
                QrcodeLoginScanFlag = false;
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

        private bool qrcodeLoginScanFlag;
        public bool QrcodeLoginScanFlag
        {
            get { return qrcodeLoginScanFlag; }
            set { SetProperty(ref qrcodeLoginScanFlag, value); }
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
