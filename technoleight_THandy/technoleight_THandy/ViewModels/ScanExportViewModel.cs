using System;
using System.Collections.Generic;
using System.Text;
using static technoleight_THandy.Common.Enums;
using System.Threading.Tasks;
using Xamarin.Forms;
using technoleight_THandy.Common;
using System.Windows.Input;
using technoleight_THandy.Models;
using System.Collections.ObjectModel;

namespace technoleight_THandy.ViewModels
{
    public class ScanExportViewModel : BaseViewModel
    {
        public INavigation Navigation;
        public int PageID;

        public ICommand DataSendCommand { get; }
        public ICommand PageBackCommand { get; }
        public ICommand EndButtonCommand { get; }
        public ICommand ScanReceiveViewCommand { get; }
        public ICommand ScanReceiveTotalViewCommand { get; }

        private string headMessage;
        public string HeadMessage
        {
            get { return headMessage; }
            set { SetProperty(ref headMessage, value); }
        }

        private Color headMessageColor;
        public Color HeadMessageColor
        {
            get { return headMessageColor; }
            set { SetProperty(ref headMessageColor, value); }
        }

        private Color scanReceiveViewColor;
        public Color ScanReceiveViewColor
        {
            get { return scanReceiveViewColor; }
            set { SetProperty(ref scanReceiveViewColor, value); }
        }

        private Color scanReceiveTotalViewColor;
        public Color ScanReceiveTotalViewColor
        {
            get { return scanReceiveTotalViewColor; }
            set { SetProperty(ref scanReceiveTotalViewColor, value); }
        }

        private bool isScanReceiveView;
        public bool IsScanReceiveView
        {
            get { return isScanReceiveView; }
            set { SetProperty(ref isScanReceiveView, value); }
        }

        private bool isScanReceiveTotalView;
        public bool IsScanReceiveTotalView
        {
            get { return isScanReceiveTotalView; }
            set { SetProperty(ref isScanReceiveTotalView, value); }
        }

        private string _exportDate;
        public string ExportDate
        {
            get { return _exportDate; }
            set { SetProperty(ref _exportDate, value); }
        }

        private Color colorState;
        public Color ColorState
        {
            get { return colorState; }
            set { SetProperty(ref colorState, value); }
        }

        private string scannedCode = "";
        public string ScannedCode
        {
            get { return scannedCode; }
            set { SetProperty(ref scannedCode, value); }
        }

        private int scanCount;
        public int ScanCount
        {
            get { return scanCount; }
            set { SetProperty(ref scanCount, value); }
        }

        private ObservableCollection<ReceiveViewModel> scanReceiveViews;
        public ObservableCollection<ReceiveViewModel> ScanReceiveViews
        {
            get { return scanReceiveViews; }
            set { SetProperty(ref scanReceiveViews, value); }
        }

        private ObservableCollection<ReceiveTotalViewModel> scanReceiveTotalViews;
        public ObservableCollection<ReceiveTotalViewModel> ScanReceiveTotalViews
        {
            get { return scanReceiveTotalViews; }
            set { SetProperty(ref scanReceiveTotalViews, value); }
        }

        private string address2 = "";
        public string Address2
        {
            get { return address2; }
            set { SetProperty(ref address2, value); }
        }

        private bool canReadClipBoard;
        public bool CanReadClipBoard
        {
            get { return canReadClipBoard; }
            set { SetProperty(ref canReadClipBoard, value); }
        }

        private bool canReceiveBarcode;
        public bool CanReceiveBarcode
        {
            get { return canReceiveBarcode; }
            set { SetProperty(ref canReceiveBarcode, value); }
        }

        private bool gridVisible;
        public bool GridVisible
        {
            get { return gridVisible; }
            set { SetProperty(ref gridVisible, value); }
        }

        private bool isAnalyzing;
        public bool IsAnalyzing
        {
            get { return isAnalyzing; }
            set { SetProperty(ref isAnalyzing, value); }
        }

        private bool frameVisible;
        public bool FrameVisible
        {
            get { return frameVisible; }
            set { SetProperty(ref frameVisible, value); }
        }

        private bool scanFlag;
        public bool ScanFlag
        {
            get { return scanFlag; }
            set { SetProperty(ref scanFlag, value); }
        }

        private string strName;
        public string StrName
        {
            get { return strName; }
            set { SetProperty(ref strName, value); }
        }

        private string strUuid;
        public string StrUuid
        {
            get { return strUuid; }
            set { SetProperty(ref strUuid, value); }
        }

        private string strState;
        public string StrState
        {
            get { return strState; }
            set { SetProperty(ref strState, value); }
        }

        private string scannedCodeString;
        public string ScannedCodeString
        {
            get { return scannedCodeString; }
            set { SetProperty(ref scannedCodeString, value); }
        }

        public ScanExportViewModel()
        {
            DataSendCommand = new Command(SubmitData);
            EndButtonCommand = new Command(async() => await PrePageBack());
            PageBackCommand = new Command(PageBack);
            ScanReceiveViewCommand = new Command(ScanReceiveView);
            ScanReceiveTotalViewCommand = new Command(ScanReceiveTotalView);

        }

        public async Task InitPage(string title, int pageID, INavigation navigation)
        {
            try
            {
                await Task.Run(() => ActivityRunningLoading());

                // 初期値セット
                HeadMessage = title;
                Navigation = navigation;
                PageID = pageID;

                // 出庫処理日セット
                _exportDate = DateTime.Now.ToString("yyyy/MM/dd");

                ScanReceiveViews = new ObservableCollection<ReceiveViewModel>();
                ScanReceiveTotalViews = new ObservableCollection<ReceiveTotalViewModel>();

                this.InitializeViewDesign();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await Task.Run(() => ActivityRunningEnd());
            }
        }


        private void InitializeViewDesign()
        {
            // 箱数集計を初期表示
            this.ScanReceiveTotalView();

            if (PageID == (int)Enums.PageID.Receive_StoreIn_AddressMatchCheck)
            {
                HeadMessageColor = (Color)App.TargetResource["PrimaryTextColor"];
            }
            else if (PageID == (int)Enums.PageID.Receive_StoreIn_TemporaryAddressMatchCheck)
            {
                HeadMessageColor = (Color)App.TargetResource["AccentTextColor"];
            }
            else if (PageID == (int)Enums.PageID.Receive_StoreIn_AddressMatchCheck_PackingCountInput)
            {
                HeadMessageColor = (Color)App.TargetResource["PrimaryTextColor"];
            }
            else if (PageID == (int)Enums.PageID.ReturnStoreAddress_AddressMatchCheck)
            {
                HeadMessageColor = (Color)App.TargetResource["PrimaryTextColor"];
            }

        }

        private void ScanReceiveView()
        {
            IsScanReceiveView = true;
            IsScanReceiveTotalView = false;
            ScanReceiveViewColor = (Color)App.TargetResource["MainColor"];
            ScanReceiveTotalViewColor = (Color)App.TargetResource["SecondaryButtonColor"];
        }

        private void ScanReceiveTotalView()
        {
            IsScanReceiveView = false;
            IsScanReceiveTotalView = true;
            ScanReceiveViewColor = (Color)App.TargetResource["SecondaryButtonColor"];
            ScanReceiveTotalViewColor = (Color)App.TargetResource["MainColor"];
        }


        private async void SubmitData()
        {
            
        }

        public async Task PrePageBack()
        {
            await Navigation.PopAsync();
        }

        public async void PageBack()
        {
            await Navigation.PopAsync();
        }
    }
}
