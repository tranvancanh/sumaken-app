using System;
using technoleight_THandy.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace technoleight_THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanStoreOutPageClipBoard : ContentPage
    {
        public int PageID;

        public ScanStoreOutPageClipBoard()
        {
            InitializeComponent();
        }

        public static ScanStoreOutPageClipBoard scanReadPageClipBoard;
        public static ScanStoreOutPageClipBoard GetInstance(string title, int pageID, INavigation navigation)
        {
            scanReadPageClipBoard = new ScanStoreOutPageClipBoard();
            ScanReadClipBoardViewModel.GetInstance().Initilize(title, pageID, navigation);
            scanReadPageClipBoard.PageID = pageID;
            return scanReadPageClipBoard;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = ScanReadClipBoardViewModel.GetInstance();
        }

        protected override void OnDisappearing()
        {
            ScanReadClipBoardViewModel.GetInstance().DisposeEvent();
            base.OnDisappearing();
        }


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
    }
}