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
    public partial class ScanReadPageCamera : ContentPage
    {
        private static ScanReadPageCamera scanReadPageCamera;
        public static ScanReadPageCamera GetInstance(string name1, int kubun, INavigation navigation)
        {
            ScanReadCameraViewModel.GetInstance().Initilize(name1, kubun, navigation);

            if (scanReadPageCamera == null)
            {
                scanReadPageCamera = new ScanReadPageCamera();
                return scanReadPageCamera;
            }
            return scanReadPageCamera;
        }

        public ScanReadPageCamera()
        {
            //カメラ画面
            InitializeComponent();
        }

        ~ScanReadPageCamera()
        {
            Console.WriteLine("#ScanReadPageCamera finish");
        }

        protected override void OnAppearing()
        {
            scankidou();
        }

        private void scankidou()
        {
            base.OnAppearing();
            zxing.IsScanning = true;
            BindingContext = ScanReadCameraViewModel.GetInstance();
            //SEplayer.Load(GetStreamFromFile("decision1.mp3"));
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
        //        ScanReadCameraViewModel vm = ScanReadCameraViewModel.GetInstance();
        //        // 表示完了後のみイベント拾う
        //        if (true == vm.bCompletedDsp && index >= 0)
        //        {
        //            string strSelectItem = PickScanMode.Items[index];
        //            if (strSelectItem.Equals(Common.Const.C_SCANNAME_KEYBOARD))
        //            {
        //                //カメラからキーボード切替
        //                List<Setting.SettingSqlLite> Set1 = await App.DataBase.GetSettingAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_KEYBOARD;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageKeyBoard.GetInstance(vm.nameA, vm.Readkubun);
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //            }
        //            else if (strSelectItem.Equals(Common.Const.C_SCANNAME_BARCODE))
        //            {
        //                //カメラからバーコードリーダ切替
        //                List<Setting.SettingSqlLite> Set1 = await App.DataBase.GetSettingAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_BARCODE;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageBarcode.GetInstance(vm.nameA, vm.Readkubun);
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //            }
        //            else if (strSelectItem.Equals(Common.Const.C_SCANNAME_CLIPBOARD))
        //            {
        //                //キーボードからクリップボード切替
        //                List<Setting.SettingSqlLite> Set1 = await App.DataBase.GetSettingAsync();
        //                Setei Set2 = Set1[0];
        //                Set2.ScanMode = Common.Const.C_SCANMODE_CLIPBOARD;
        //                await App.DataBase.SavSeteiAsync(Set2);

        //                Page page = ScanReadPageClipBoard.GetInstance(vm.nameA, vm.Readkubun);
        //                Navigation.InsertPageBefore(page, this);
        //                await Navigation.PopAsync();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.WriteLine("#PickScanModeSelectedIndexChanged Err {0}", ex.ToString());
        //    }
        //}
    }
}