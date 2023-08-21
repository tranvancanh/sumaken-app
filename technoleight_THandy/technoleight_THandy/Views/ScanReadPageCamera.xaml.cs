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
using static technoleight_THandy.Common.Enums;

namespace technoleight_THandy.Views
{
    // シングルトンで呼び出すこと
    public partial class ScanReadPageCamera : ContentPage
    {
        public static ScanReadPageCamera scanReadPageCamera;
        public static ScanReadPageCamera GetInstance(string title, int pageID, INavigation navigation)
        {
            ScanReadCameraViewModel.GetInstance().Initilize(title, pageID, navigation);

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
            base.OnAppearing();
            zxing.IsScanning = true;

            //// スケールを大きくして、目的を狭めます
            //zxing.ScaleXTo(2);
            //zxing.ScaleYTo(2);
            BindingContext = ScanReadCameraViewModel.GetInstance();
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            ScanReadCameraViewModel.GetInstance().PageBackEnd();
            return true;
        }

        /// <summary>
        /// サイズが決まった後で呼び出されます。AbsoluteLayout はここで位置を決めるのが良いみたいです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AbsolutePageXamlSizeChanged(object sender, EventArgs e)
        {
            AbsoluteLayout.SetLayoutFlags(Dialog,
                AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(Dialog,
                new Rectangle(0.5d, 0.5d,
                Device.OnPlatform(AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize, this.Width), AbsoluteLayout.AutoSize)); // View の中央に AutoSize で配置

            AbsoluteLayout.SetLayoutFlags(Dialog2,
                AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(Dialog2,
                new Rectangle(0.5d, 0.5d,
                Device.OnPlatform(AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize, this.Width), AbsoluteLayout.AutoSize)); // View の中央に AutoSize で配置

            AbsoluteLayout.SetLayoutFlags(BackgroundLayer,
                AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(BackgroundLayer,
                new Rectangle(0d, 0d,
                this.Width, this.Height)); // View の左上から View のサイズ一杯で配置

            //AbsoluteLayout.SetLayoutFlags(MainContent,
            //    AbsoluteLayoutFlags.PositionProportional);
            //AbsoluteLayout.SetLayoutBounds(MainContent,
            //    new Rectangle(0d, 0d,
            //    this.Width, this.Height)); // View の左上から View のサイズ一杯で配置
        }

        private async void ToggleFlyoutButtonClicked(object sender, EventArgs e)
        {
            var flyout = ScanDataFlyout;
            await SetFlyout(flyout);
        }

        private async Task SetFlyout(Grid flyout)
        {
            if (flyout.IsVisible)
            {
                await flyout.TranslateTo(0, flyout.Height, 300);
                flyout.IsVisible = !flyout.IsVisible;
            }
            else
            {
                zxing.IsAnalyzing = false;

                await flyout.TranslateTo(0, flyout.Height, 0);
                flyout.IsVisible = !flyout.IsVisible;
                await flyout.TranslateTo(0, 0, 300);

                zxing.IsAnalyzing = true;
            }
        }
    }
}