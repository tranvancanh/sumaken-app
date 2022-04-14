using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using THandy.Models;
using THandy.ViewModels;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanBeforePage : ContentPage
    {
        public ScanBeforeViewModel vm;

        /// <summary>
        /// ボタン処理中に画面遷移させない制御用
        /// </summary>       
        public static bool btnFanction = false;

        public ScanBeforePage(ScanBeforeViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
        }

        void OnButton1Toggled(object sender, ToggledEventArgs args)
        {
            if (args.Value)
            {
                Save1("1", "1");

                Save1("2", "0");
                Save1("3", "0");
                Save1("4", "0");

                Toggle2.IsToggled = false;
                Toggle3.IsToggled = false;
                Toggle4.IsToggled = false;
            }
            else
            {
                Save1("1", "0");
            }
        }
        void OnButton2Toggled(object sender, ToggledEventArgs args)
        {
            if (args.Value)
            {
                Save1("2", "1");

                Save1("1", "0");
                Save1("3", "0");
                Save1("4", "0");

                Toggle1.IsToggled = false;
                Toggle3.IsToggled = false;
                Toggle4.IsToggled = false;
            }
        }
        void OnButton3Toggled(object sender, ToggledEventArgs args)
        {
            if (args.Value)
            {
                Save1("3", "1");

                Save1("1", "0");
                Save1("2", "0");
                Save1("4", "0");

                Toggle1.IsToggled = false;
                Toggle2.IsToggled = false;
                Toggle4.IsToggled = false;
            }
        }
        void OnButton4Toggled(object sender, ToggledEventArgs args)
        {
            if (args.Value)
            {
                Save1("4", "1");

                Save1("1", "0");
                Save1("2", "0");
                Save1("3", "0");

                Toggle1.IsToggled = false;
                Toggle2.IsToggled = false;
                Toggle3.IsToggled = false;

            }
        }
 
        void OnDateSelected(object sender, DateChangedEventArgs args)
        {
            Recalculate();
        }

        void OnSwitchToggled(object sender, ToggledEventArgs args)
        {
            Recalculate();
        }

        void Recalculate()
        {
            //TimeSpan timeSpan = endDatePicker.Date - startDatePicker.Date +
            //    (includeSwitch.IsToggled ? TimeSpan.FromDays(1) : TimeSpan.Zero);

            //resultLabel.Text = String.Format("{0} day{1} between dates",
            //                                    timeSpan.Days, timeSpan.Days == 1 ? "" : "s");
        }

        private async Task List1()
        {
            //画面表示
            int i;
            NoukiView.IsVisible = false;
            ShabanView.IsVisible = false;
            DkeyView.IsVisible = false;
            ToggleButtonView.IsVisible = false;
            NextBottonView.IsVisible = false;

            //設定ファイルの読取
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            if (Set2.Count > 0)
            {
                Txtuser.Text = Set2[0].username;
            }
            else
            {

            }

            // 画面番号 テクノエイト
            // 入庫or出庫画面
            if (vm.Readkubun == "202")
            {

                // 納期
                NoukiView.IsVisible = true;
                // 便
                ToggleButtonView.IsVisible = true;
                // 次へ
                NextBottonView.IsVisible = true;

                //便　作成
                int cnt = await App.DataBase.GetJikuDBAsync2();
                if (cnt > 0)
                {

                }
                else
                {
                    for (i = 1; i <= 4; i++)
                    {
                        JikuDB Jikux = new JikuDB();
                        Jikux.Id = i.ToString();
                        Jikux.OnOff = "0";
                        int cnt1 = await App.DataBase.SaveJikuDBAsync(Jikux);
                    }
                    Toggle1.IsToggled = true;
                }

                List<JikuDB> JikuDBList = await App.DataBase.GetJikuDBAsync();

                if (JikuDBList[0].OnOff == "1")
                {
                    Toggle1.IsToggled = true;

                    Toggle2.IsToggled = false;
                    Toggle3.IsToggled = false;
                    Toggle4.IsToggled = false;
                }
                else if (JikuDBList[1].OnOff == "1")
                {
                    Toggle2.IsToggled = true;

                    Toggle1.IsToggled = false;
                    Toggle3.IsToggled = false;
                    Toggle4.IsToggled = false;
                }
                else if (JikuDBList[2].OnOff == "1")
                {
                    Toggle3.IsToggled = true;

                    Toggle1.IsToggled = false;
                    Toggle2.IsToggled = false;
                    Toggle4.IsToggled = false;
                }
                else if (JikuDBList[3].OnOff == "1")
                {
                    Toggle4.IsToggled = true;

                    Toggle1.IsToggled = false;
                    Toggle2.IsToggled = false;
                    Toggle3.IsToggled = false;
                }
            }
            else if (vm.Readkubun == "203")
            {
                // 納期
                NoukiView.IsVisible = true;
                // 車番
                ShabanView.IsVisible = true;
                // 次へ
                NextBottonView.IsVisible = true;

                bool shabanSet_result = await SetShabanPicker();
            }
            else if (vm.Readkubun == "205")
            {
                // 代表
                DkeyView.IsVisible = true;
                // 次へ
                NextBottonView.IsVisible = true;
            }

        }

        private async Task<bool> SetShabanPicker()
        {
            var shabanItems = new ObservableCollection<string>();

            // 車番一覧を取得
            var carNoList = await App.DataBase.GetCarAsync();

            if (carNoList.Count > 0)
            {
                //以前選択されてたCarNoを取得
                var carNoDB = await App.DataBase.GetCarDBAsync();

                var selectedCar = carNoList.Where(x => x.CarNo == carNoDB[0].CarNo).ToList().FirstOrDefault();
                var selectedIndex = carNoList.IndexOf(selectedCar);

                // セレクトボックスの中身とインデックスをセット
                for (int i = 0; i < carNoList.Count; i++)
                {
                    var carNo = carNoList[i].CarNo;
                    shabanItems.Add(carNo);
                }

                vm.ShabanItems = shabanItems;
                vm.ShabanSelectedIndex = selectedIndex;

                return true;
            }
            else
            {
                //ユーザー情報抽出
                string WID = "";
                string user = "";
                string Device = "";

                List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
                if (Set2.Count > 0)
                {
                    WID = Set2[0].WID;
                    user = Set2[0].user;
                    Device = Set2[0].Device;
                }

                var items1 = new BarModel();

                items1.Shori = "SelectMastarData";
                items1.WID = WID;
                items1.UserID = user;
                items1.Device = Device;
                items1.Shorikubun = "Get_m_car";

                //WEBサーバーからSELECT
                var postLidt = new List<BarModel>();
                postLidt.Add(items1);
                List<Dictionary<string, string>> carMastarList = await App.API.Post_method3(postLidt, "BarTechnolEight");
                if (carMastarList == null)
                {
                    await Application.Current.MainPage.DisplayAlert("サーバーエラー", "車両データを取得できませんでした。もう一度お願いします", "OK");
                }
                else if (carMastarList.Count == 0)
                {
                    //行数0
                    await Application.Current.MainPage.DisplayAlert("エラー", "戻りデーターエラー", "OK");
                }
                else if (carMastarList.Count > 0)
                {
                    string dictValue;
                    if (true == carMastarList[0].TryGetValue(key: Common.Const.C_ERR_KEY_NETWORK, value: out dictValue))
                    {
                        //ネットワーク
                        //エラー
                        await Application.Current.MainPage.DisplayAlert("ネットワーク接続エラー", "ネットワーク接続後に再度実行して下さい。", "OK");
                    }
                    else if (carMastarList[0]["Name"].ToString() == "0")
                    {
                        //納品書番号が1件も存在しない
                        //エラー
                        await Application.Current.MainPage.DisplayAlert("エラー", "車両データが存在しません", "OK");
                    }
                    else if (carMastarList[0]["Name"].ToString() == "NG")
                    {
                        //納品書データ
                        //エラー
                        await Application.Current.MainPage.DisplayAlert("エラー", "車両データを取得できません", "OK");
                    }
                    else
                    {

                        for (int i = 0; i <= carMastarList.Count - 1; i++)
                        {
                            var cerModel = new Car();
                            cerModel.CarNo = carMastarList[i]["CarNo"].ToString();
                            shabanItems.Add(cerModel.CarNo);

                            //SQLiteデータベース登録
                            var saveCarResult = await App.DataBase.SaveCarAsync(cerModel);

                        }

                        if (shabanItems.Count > 0)
                        {
                            vm.ShabanItems = shabanItems;
                            vm.ShabanSelectedIndex = 0;

                            return true;
                        }
                        else
                        {

                        }

                    }

                }

                return false;
            }

        }


        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //CollectionViewクリック処理
            var pre = e.PreviousSelection;
            var cur = e.CurrentSelection;


            ObservableCollection<Jiku> previous = new ObservableCollection<Jiku>();
            ObservableCollection<Jiku> current = new ObservableCollection<Jiku>();


            if (pre.Count > 0)
            {

                foreach (Jiku val in pre)
                {
                    Jiku Jikux = new Jiku();
                    Jikux.Id = val.Id;
                    //val.Color = "Yellow";
                    //Jikux.Color = val.Color;
                    previous.Add(Jikux);
                    Save1(val.Id, "0");

                }

            }
            if (cur.Count > 0)
            {

                foreach (Jiku val in cur)
                {

                    Jiku Jikux = new Jiku();
                    Jikux.Id = val.Id;
                    //val.Color = "White";
                    //val.Color = "Blue";
                    //Jikux.Color = val.Color;
                    current.Add(Jikux);
                    Save1(val.Id, "1");


                }

            }

            //ObservableCollection<Jiku> Jikub1 = new ObservableCollection<Jiku>();

            //Jikub1= (ObservableCollection<Jiku>)MyCollectionView.ItemsSource;
            //if (Jikub1 != null)
            //{
            //    foreach (Jiku val in Jikub1)
            //    {

            //        //Jiku Jikux = new Jiku();
            //        if (pre.Count == 0 && cur.Count == 0)
            //        {
            //            val.Color = "Red";
            //        } 
            //        else if (pre.Count == 0 && cur.Count > 0)
            //        {
            //            val.Color = "Blue";
            //        }
            //        else if (pre.Count > 0 && cur.Count == 0)
            //        {
            //            val.Color = "White";
            //        }



            //    }
            //}


            ////Console.WriteLine(String.Join(" ", lst3.Select(x => x)));
            if (previous.Count == 0)
            {
                return;
            }


        }

        private async void Save1(string Id, string OnOff)
        {
            if (vm.Readkubun == "202" || vm.Readkubun == "203")
            {
                JikuDB Jikuz = new JikuDB();
                Jikuz.Id = Id;
                Jikuz.OnOff = OnOff;
                int cnt1 = await App.DataBase.SaveJikuDBAsync(Jikuz);
            }
        }   

        protected override async void OnAppearing()
        {
            //MainView.IsVisible = false;
            vm.ActivityRunning = true;

            await List1();

            //登録データの削除を行う
            await App.DataBase.DeleteAllScanReadData();
            await App.DataBase.ALLDeleteNouhinAsync();
            await App.DataBase.ALLDeleteNouhinAsync();
            await App.DataBase.ALLDeleteNouhinJLAsync();
            await App.DataBase.ALLDeleteShukoAsync();

            base.OnAppearing();

            BindingContext = this.vm;

            NavigationPage.SetHasNavigationBar(this, true);
            vm.ActivityRunning = false;
            MainView.IsVisible = true;
        }

       //private async Task aaa(object sender, EventArgs e)
       //{
       //    Page page = new ScanPage(new ScanViewModel(vm.Title, "202"));
       //    await Navigation.PushAsync(page);
       //}

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            var binList = new List<string>();

            if (Toggle1.IsToggled)
            {
                binList.Add(Toggle1.Text.Trim());
            }
            if (Toggle2.IsToggled)
            {
                binList.Add(Toggle2.Text.Trim());
            }
            if (Toggle3.IsToggled)
            {
                binList.Add(Toggle3.Text.Trim());
            }
            if (Toggle4.IsToggled)
            {
                binList.Add(Toggle4.Text.Trim());
            }

            Page page = null;

            if (vm.Readkubun == "202")
            {
                page = new ScanPage(new ScanViewModel(vm.Title, "202", startDatePicker.Date.ToString(), binList, null, 0));
            }
            else if (vm.Readkubun == "203")
            {
                if (String.IsNullOrEmpty(vm.Shaban))
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "車番を入力してください。", "OK");
                    return;
                }
                else
                {
                    var cardbModel = new CarDB();
                    cardbModel.CarNo = vm.Shaban;

                    //SQLiteデータベース登録
                    var deleteCarDBResult = await App.DataBase.ALLDeleteCarDBAsync();
                    var saveCarDBResult = await App.DataBase.SaveCarDBAsync(cardbModel);
                }

                page = new ScanPage(new ScanViewModel(vm.Title, "203", startDatePicker.Date.ToString(), null, vm.Shaban, 0));
            }
            else if (vm.Readkubun == "205")
            {
                var dkeyString = "";

                if (String.IsNullOrEmpty(Dkey.Text))
                {
                    await Application.Current.MainPage.DisplayAlert("エラー", "代表キーを入力してください", "OK");
                    return;
                }
                else
                {
                    dkeyString = Dkey.Text.Trim();

                    // 11文字か
                    if (dkeyString.Length != 11)
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "代表キーの文字数が不正です", "OK");
                        return;
                    }
                    // 数値に変換できるか
                    else if(!Int64.TryParse(dkeyString, out Int64 dkey))
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "代表キーの形式が不正です", "OK");
                        return;
                    }
                    else
                    {
                        // 代表バーコードの納期・便を取得
                        var getNouki = "";
                        var getBin = "";
                        var getFirstNyukobi = "";
                        var getNowSetBoxCount = 0;

                        List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
                        List<Setei> Setting = await App.DataBase.GetSeteiAsync();
                        if (Set2.Count > 0)
                        {
                            var items1 = new BarModel();
                            items1.Shori = "S1";
                            items1.WID = Set2[0].WID;
                            items1.UserID = Set2[0].user;
                            items1.Device = Set2[0].Device;
                            items1.Shorikubun = "205";
                            items1.BarcodeRead = dkeyString;
                            items1.BarcodeRead1 = dkeyString;

                            //WEBサーバーからSELECT
                            var postLidt = new List<BarModel>();
                            postLidt.Add(items1);
                            var selectData205 = await App.API.Post_method3(postLidt, "BarTechnolEight");

                            if (selectData205 == null)
                            {
                                await Application.Current.MainPage.DisplayAlert("サーバーエラー", "データを取得できませんでした。もう一度お願いします", "OK");
                                return;
                            }
                            else if (selectData205[0]["Name"].ToString() == "NG")
                            {
                                //納品書データ
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("エラー", "代表キーを取得できません", "OK");
                                return;
                            }
                            else if (selectData205[0]["Name"].ToString() == "0")
                            {
                                //納品書データ
                                //エラー
                                await Application.Current.MainPage.DisplayAlert("エラー", "該当する代表キーはありません", "OK");
                                return;
                            }
                            else
                            {
                                getBin = selectData205[0]["VHJIKU"].ToString();
                                getNouki = Convert.ToDateTime(selectData205[0]["JLDATE"]).ToString("yyyy/MM/dd");
                                getFirstNyukobi = Convert.ToDateTime(selectData205[0]["JLNKDT"]).ToString("yyyy/MM/dd");
                                getNowSetBoxCount = Convert.ToInt32(selectData205[0]["JLDKEYCOUNT"]);

                                var binList205 = new List<string>() { getBin };

                                page = new ScanPage(new ScanViewModel(vm.Title, "205", getNouki, binList205, dkey, getFirstNyukobi, getNowSetBoxCount));
                            }

                        }
                    }
                }
            }

            await Navigation.PushAsync(page);

        }
    }




}