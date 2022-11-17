using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using technoleight_THandy.Models;
using System.Threading.Tasks;
using technoleight_THandy.Views;

namespace technoleight_THandy.ViewModels
{
    public class ScanReceiptViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation Navigation;

        public string ReadKubun;

        public PageItem PageItem;
        public Command ReadCommand { get; }
        public Command ScanStartCommand { get; }

        public ScanReceiptViewModel(string title, string readKubun, INavigation navigation)
        {
            Title = title;
            ReadKubun = readKubun;
            ReceiptDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            Navigation = navigation;
            ScanStartCommand = new Command(ScanStart);
            //SetPageItem();
        }

        //private async void SetPageItem()
        //{
        //    var pageItem = await App.DataBase.GetPageItemAsync();
        //    if (pageItem != null)
        //    {
        //        PageItem = pageItem;
        //        Title = PageItem.Title;
        //    }
        //}

        public async void ScanStart()
        {
            await App.DataBase.DeleteAllScanReadData();
            await App.DataBase.DeleteAllScanReceipt();

            // クリップボードモードとしてスキャン画面呼び出し(シングルトン)
            Page page = ScanReadPageClipBoard.GetInstance(Title, ReadKubun, ReceiptDate.ToString("yyyy/MM/dd"), Navigation);
            //Navigation = PageItem.Navigation;
            await Navigation.PushAsync(page);
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
                }
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

        private DateTime receiptDate;
        public DateTime ReceiptDate
        {
            get { return receiptDate; }
            set
            {
                if (receiptDate != value)
                {
                    receiptDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReceiptDate)));
                }
            }
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

    }
}
