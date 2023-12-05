using System;
using technoleight_THandy.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static technoleight_THandy.Common.Enums;

namespace technoleight_THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanExportClipPageBoard : ContentPage
    {
        ScanExportClipBoardViewModel _viewModel;

        public ScanExportClipPageBoard()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ScanExportClipBoardViewModel();
        }

        public ScanExportClipPageBoard(string title, int pageId, INavigation navigation)
        {
            InitializeComponent();
            BindingContext = _viewModel = new ScanExportClipBoardViewModel(title, pageId, navigation);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearing();
        }


        private void AbsolutePageXamlSizeChanged(object sender, EventArgs e)
        {
            //AbsoluteLayout.SetLayoutFlags(Dialog,
            //    AbsoluteLayoutFlags.PositionProportional);
            //AbsoluteLayout.SetLayoutBounds(Dialog,
            //    new Rectangle(0.5d, 0.5d,
            //    Device.OnPlatform(AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize, this.Width), AbsoluteLayout.AutoSize)); // View の中央に AutoSize で配置

            //AbsoluteLayout.SetLayoutFlags(Dialog2,
            //    AbsoluteLayoutFlags.PositionProportional);
            //AbsoluteLayout.SetLayoutBounds(Dialog2,
            //    new Rectangle(0.5d, 0.5d,
            //    Device.OnPlatform(AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize, this.Width), AbsoluteLayout.AutoSize)); // View の中央に AutoSize で配置

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
    }
}