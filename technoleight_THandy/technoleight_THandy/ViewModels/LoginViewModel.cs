
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

namespace technoleight_THandy.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        private INavigation Navigation;
        public Command LoginCommand { get; }
        public Command SetUpCommand { get; }

        public Action ViewsideAction { get; set; }

        public LoginViewModel(INavigation navigation)
        {
            ActivityRunningLoading();

            Navigation = navigation;
            LoginIconImageSource = ImageSource.FromResource("technoleight_THandy.img.login_img.png");
            LoginCommand = new Command(OnLoginClicked);
            SetUpCommand = new Command(OnSetUpClicked);

            Init();
            
            ActivityRunningEnd();
        }

        public async void Init()
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

            await List();
        }

        private async Task List()
        {
            // 設定を取得し直す
            await App.GetSetting();

            // カラーテーマをセットし直す
            await App.GetTargetResource();

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

        }

        private async void OnLoginClicked(object obj)
        {
            await Task.Run(() => ActivityRunningLoading());

            await OnLoginClickedExcute();

            await Task.Run(() => ActivityRunningEnd());
        }

        private async Task OnLoginClickedExcute()
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
                Application.Current.MainPage = new MainPage();
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

    }
}
