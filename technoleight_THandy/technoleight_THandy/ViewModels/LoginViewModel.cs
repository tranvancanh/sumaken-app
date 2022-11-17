
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

namespace technoleight_THandy.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        //View側に変更を教えるため
        public event PropertyChangedEventHandler PropertyChanged;

        private INavigation navigation;
        public Command LoginCommand { get; }
        public Command SetUpCommand { get; }

        // ★ビュー側のメソッドを登録するためのAction1

        public Action ViewsideAction { get; set; }

        //public Users Log1 = new Users(); 
        //public Logins Log1 { get; set; }

        /// <summary>
        /// ボタン処理中に画面遷移させない制御用
        /// </summary>       
        public bool btnFanction = false;

        public LoginViewModel(INavigation navigation)
        {
            ContentIsVisible = false;
            ActivityRunning = true;

            this.navigation = navigation;
            LoginIconImageSource = ImageSource.FromResource("technoleight_THandy.img.login_img.png");
            LoginCommand = new Command(OnLoginClicked);
            SetUpCommand = new Command(OnSetUpClicked);

            Init();
        }

        public async void Init()
        {
            await List1();

            ActivityRunning = false;
            ContentIsVisible = true;
        }

        private async Task List1()
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            mergedDictionaries.Clear();

            //SQLiteより設定ファイルを抽出
            IsPass = true;
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            if (Set2.Count > 0)
            {
                Txtuser = Set2[0].user;
                if (Set2[0].PassMode == "1")
                {
                    IsPass = false;
                }

                var theme = Set2[0].ColorTheme;
                switch (theme)
                {
                    case Theme.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    case Theme.Light:
                    default:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }

            }
            else
            {
                mergedDictionaries.Add(new LightTheme());
            }

        }

        private async void OnLoginClicked(object obj)
        {
            // ボタン押下チェック(連打対策)
            if (!btnFanction)
            {
                btnFanction = true;
                ContentIsVisible = false;
                ActivityRunning = true;

                await OnLoginClickedExcute();

                ActivityRunning = false;
                ContentIsVisible = true;
                btnFanction = false; //ボタン押下可
            }
        }
        private async Task OnLoginClickedExcute()
        {
            //(double latitude, double longitude) = Gps.Method().Result;

            string WID1="0";
            string Device = "";
            string PassMode = "0";
            string username = "";
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            if (Set2.Count > 0)
            {
                WID1 = Set2[0].WID;
                Device = Set2[0].Device;
                PassMode = Set2[0].PassMode;
                username = Set2[0].username;
                
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("設定登録チェック", "設定ボタンを押して登録をしてください。", "OK");
                return;

            }
            List<MenuX> menux = await App.DataBase.GetMenuAsync(WID1, "0");
            if (menux.Count > 0)
            {
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("メニューチェック", "設定ボタンを押してメニューの登録をしてください。", "OK");
                return;
            }

            
            //userチェック
            string User1 = Txtuser;
            string Pass1 = Txtpass;

            if (User1 == null) { User1 = ""; }
            if (Pass1 == null) { Pass1 = ""; }

            //(int properCount, int partnerCount) = Company.GetEmployeeCountAsyncTuple().Result;

            //ソフトのバージョンチェックを行う
            string ManuFacturer = DependencyService.Get<IDeviceService>().GetManufacturerName();
            //アプリバージョン文字列を取得する場合
            string VerName = DependencyService.Get<IAssemblyService>().GetVersionName();
            
            //アプリバージョンコードを取得する場合
            string VerCode = "";
            try
            {
                VerCode = DependencyService.Get<IAssemblyService>().GetVersionCode();

            }
            catch (Exception)
            {

                //throw;
            }
            
            //Web Serverと通信
            List<Dictionary<string, string>> items1 = new List<Dictionary<string, string>>();
            //S1 ログイン WUMasterControllerを処理
            items1.Add(new Dictionary<string, string>() { { "Shori", "S1" }, { "WID", WID1 }, { "UserID", User1 }, { "Password", Pass1 }, { "Device", Device }, { "PassMode", PassMode }, { "ManuFacturer", ManuFacturer }, { "VerName", VerName }, { "VerCode", VerCode } });
            List<Dictionary<string, string>> items2 = await App.API.Post_method(items1, "WUMaster");

            if (items2 == null)
            {
                await Application.Current.MainPage.DisplayAlert("ユーザー名・パスワードチェック", "どちらかが違っています。", "OK");
                return;

            }
            else if (items2.Count > 0)
            {
                string message1 = "";
                foreach (string Value in items2[0].Values)
                {
                    message1 = Value;
                }

                switch (message1)
                {
                    case "OK":
                    case "VUP":
                        //ユーザー名、パスワードOK
                        Setei Set1 = new Setei();
                        Set1.WID = WID1;
                        Set1.url = Set2[0].url;
                        Set1.k_pass = Set2[0].k_pass;
                        Set1.user = User1;
                        Set1.userpass = Pass1;
                        Set1.Device = Set2[0].Device;
                        Set1.ScanMode = Set2[0].ScanMode;

                        // 現状、クリップボードオンリーなので、強制的にクリップボード
                        Set1.ScanMode = Const.C_SCANNAME_CLIPBOARD;

                        Set1.PassMode = Set2[0].PassMode;
                        Set1.username = Set2[0].username;
                        Set1.BarcodeReader = Set2[0].BarcodeReader;
                        Set1.UUID = Set2[0].UUID;
                        Set1.ColorTheme = Set2[0].ColorTheme;
                        Set1.ScanOkeySound = Set2[0].ScanOkeySound;
                        Set1.ScanErrorSound = Set2[0].ScanErrorSound;


                        List<Dictionary<string, string>> userData = new List<Dictionary<string, string>>();
                        userData.Add(new Dictionary<string, string>() { { "Shori", "S2" }, { "WID", Set1.WID }, { "UserID", Set1.user } });
                        var userDetailData = await App.API.Post_method(userData, "WUMaster");

                        if (userDetailData != null && userDetailData.Count > 0)
                        {
                            var userDetailResult = userDetailData[0]["Name"];

                            if (userDetailResult == "OK")
                            {
                                foreach (Dictionary<string, string> item in userDetailData)
                                {
                                    // ループ変数にKeyValuePairを使う
                                    foreach (KeyValuePair<string, string> kv in item)
                                    {
                                        if (kv.Key == Common.Const.C_ERR_KEY_NETWORK)
                                        {
                                            message1 = kv.Value;
                                        }
                                        if (kv.Key == "CompanyName")
                                        {
                                            Set1.CompanyName = kv.Value;
                                        }
                                        if (kv.Key == "WarehouseCode")
                                        {
                                            Set1.WarehouseCode = kv.Value;
                                        }
                                        if (kv.Key == "WarehouseName")
                                        {
                                            Set1.WarehouseName = kv.Value;
                                        }
                                        if (kv.Key == "User_name")
                                        {
                                            Set1.username = kv.Value;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("エラー", "ユーザー情報の取得に失敗しました。", "OK");
                                return;
                            }

                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("エラー", "ユーザー情報の取得に失敗しました。", "OK");
                            return;
                        }

                        int ia = await App.DataBase.SavSeteiAsync(Set1);
                        Application.Current.MainPage = new MainPage();
                        //MainPageを立ち上げる
                        break;
                   
                        //バージョンアップ
                        // ★VIEW登録したメソッドの呼び出し
                        
                        //ViewsideAction?.Invoke();
                        //return;


                    case "NG":
                        await Application.Current.MainPage.DisplayAlert("ユーザー名・パスワードチェック", "どちらかが違っています。", "OK");
                        return;
                        //break;
                    case "NG1":
                        await Application.Current.MainPage.DisplayAlert("デバイス登録エラー", "ユーザーマスターに登録してあるデバイスIDと違います。", "OK");
                        return;
                    case Common.Const.C_ERR_VALUE_NETWORK:
                        await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                        return;
                    //break;
                    default:
                        break;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("ユーザー名・パスワードチェック", "どちらかが違っています。", "OK");
                return;

            }
        }

       
        private void OnSetUpClicked()
        {
            Application.Current.MainPage = new SeteiPage();
        }

        private ImageSource loginIconImageSource;
        public ImageSource LoginIconImageSource
        {
            get { return loginIconImageSource; }
            set
            {
                    loginIconImageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoginIconImageSource)));
            }
        }

        private string txtuser;
        public string Txtuser
        {
            get { return txtuser; }
            set
            {
                if (txtuser != value)
                {
                    txtuser = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Txtuser)));
                }
            }
        }

        private string txtpass;
        public string Txtpass
        {
            get { return txtpass; }
            set
            {
                if (txtpass != value)
                {
                    txtpass = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Txtpass)));
                }
            }
        }

        private bool isPass = true;
        public bool IsPass
        {
            get { return isPass; }
            set
            {
                isPass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPass)));
            }
        }

        private static bool activityRunning = false;
        public bool ActivityRunning
        {
            get { return activityRunning; }
            set
            {
                if (activityRunning != value)
                {
                    activityRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActivityRunning)));
                }
            }
        }
        private static bool contentIsVisible = false;
        public bool ContentIsVisible
        {
            get { return contentIsVisible; }
            set
            {
                if (contentIsVisible != value)
                {
                    contentIsVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContentIsVisible)));
                }
            }
        }
    }
}
