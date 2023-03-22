﻿using System;
using System.ComponentModel;
using Xamarin.Forms;
using technoleight_THandy.ViewModels;

namespace technoleight_THandy.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;
      
        public ItemsPage()
        {
            //最初のメイン画面
            InitializeComponent();

            // メイン画面をItemsで設定して表示を行う
            //ItemsViewModelでデータベースより抽出
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

            AbsoluteLayout.SetLayoutFlags(BackgroundLayer,
                AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(BackgroundLayer,
                new Rectangle(0d, 0d,
                this.Width, this.Height)); // View の左上から View のサイズ一杯で配置
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel = new ItemsViewModel();
            _viewModel.Navigation = Navigation;
            BindingContext = _viewModel;
        }

    }
}