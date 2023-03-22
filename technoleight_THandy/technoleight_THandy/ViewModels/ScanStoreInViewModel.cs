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
    public class ScanStoreInViewModel : BaseViewModel
    {
        public INavigation Navigation;

        public int PageID;

        public Command ReadCommand { get; }
        public Command ScanStartCommand { get; }

        public ScanStoreInViewModel(int pageID, string title, INavigation navigation)
        {
            Task.Run(() => ActivityRunningLoading());

            PageID = pageID;
            Title = title;
            StoreInDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            Navigation = navigation;
            ScanStartCommand = new Command(ScanStart);
        }

        public async void ScanStart()
        {
            try 
            {
                // クリップボードモードとしてスキャン画面呼び出し(シングルトン)
                Page page = ScanReadPageClipBoard.GetInstance(Title, PageID, StoreInDate.ToString("yyyy/MM/dd"), Navigation);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {

            }

        }

        private DateTime storeInDate;
        public DateTime StoreInDate
        {
            get { return storeInDate; }
            set { SetProperty(ref storeInDate, value); }
        }

        private bool isDatePickerEnabled = true;
        public bool IsDatePickerEnabled
        {
            get { return isDatePickerEnabled; }
            set { SetProperty(ref isDatePickerEnabled, value); }
        }

    }
}
