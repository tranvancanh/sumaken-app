using Plugin.SimpleAudioPlayer;
using technoleight_THandy.Models;
using technoleight_THandy.ViewModels;
using technoleight_THandy;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;

namespace technoleight_THandy.Views
{
    // シングルトンで呼び出すこと
    public partial class ScanReadPageBarcode : ContentPage
    {
        // 画面再表示後に表示更新しない不具合があった。通信処理を作ったためメモリ開放しないのが原因と判断した。なのでシングルトンとする。
        private static ScanReadPageBarcode scanReadPageBarcode;
        public static ScanReadPageBarcode GetInstance(string name1, int kubun, INavigation navigation)
        {
            ScanReadBarcodeViewModel.GetInstance().Initilize(name1, kubun, navigation);

            if (scanReadPageBarcode == null)
            {
                scanReadPageBarcode = new ScanReadPageBarcode();
                return scanReadPageBarcode;
            }
            return scanReadPageBarcode;
        }

        public ScanReadPageBarcode()
        {
            InitializeComponent();
        }

        ~ScanReadPageBarcode()
        {
            Console.WriteLine("#ScanReadPageBarcode finish");
        }

        protected override void OnAppearing()
        {
            scankidou();
        }

        private void scankidou()
        {
            base.OnAppearing();
            BindingContext = ScanReadBarcodeViewModel.GetInstance();
            // xamlで設定しているのに、背景色が変わらない不具合が発生したため、
            // 暫定対応としてここで色を設定する。
            btnConnet.BackgroundColor = Color.FromRgb(233, 84, 32);
        }

        public void Hyouji()
        {
            //HName.Text = "ああああ";
        }

        //private async void PickScanModeSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int index = PickScanMode.SelectedIndex;
        //        ScanReadBarcodeViewModel vm = ScanReadBarcodeViewModel.GetInstance();
        //        // 表示完了後のみイベント拾う
        //        if (true == vm.bCompletedDsp && index >= 0)
        //        {
        //            string strSelectItem = PickScanMode.Items[index];

        //            if (strSelectItem.Equals(Common.Const.C_SCANNAME_KEYBOARD))
        //            {
        //                //バーコードリーダからキーボード切替
        //                List<Setting.SettingSqlLite> Set1 = await App.DataBase.GetSettingAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_KEYBOARD;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageKeyBoard.GetInstance(vm.nameA, vm.Readkubun);
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //                vm.DisposeEvent();
        //            }
        //            else if (strSelectItem.Equals(Common.Const.C_SCANNAME_CAMERA))
        //            {
        //                //バーコードリーダからカメラ切替
        //                List<Setting.SettingSqlLite> Set1 = await App.DataBase.GetSettingAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_CAMERA;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageCamera.GetInstance(vm.nameA, vm.Readkubun);
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //                vm.DisposeEvent();
        //            }
        //            else if (strSelectItem.Equals(Common.Const.C_SCANNAME_CLIPBOARD))
        //            {
        //                //バーコードリーダからクリップボード切替
        //                List<Setting.SettingSqlLite> Set1 = await App.DataBase.GetSettingAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_CLIPBOARD;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageClipBoard.GetInstance(vm.nameA, vm.Readkubun);
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //                vm.DisposeEvent();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.WriteLine("#PickScanModeSelectedIndexChanged Err {0}", ex.ToString());
        //    }
        //}

        private void ConnetClicked(Object Sender, EventArgs args)
        {
            //接続
            ScanReadBarcodeViewModel.GetInstance().BTConnet();
        }

        private void DisConnetClicked(Object Sender, EventArgs args)
        {
            //切断
            ScanReadBarcodeViewModel.GetInstance().BTDisConnet();
        }
    }
}