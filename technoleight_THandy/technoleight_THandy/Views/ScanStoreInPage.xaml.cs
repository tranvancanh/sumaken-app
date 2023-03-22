using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using technoleight_THandy.Models;
using technoleight_THandy.ViewModels;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using technoleight_THandy.Interface;
using Android.Bluetooth;
using System.IO;
using Android.PrintServices;
using System.Drawing.Printing;
using Android.Views;
using System.Net.Sockets;
using Newtonsoft.Json;
using Android.Content.Res;

namespace technoleight_THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanStoreInPage : ContentPage
    {
        public ScanStoreInViewModel vm;

        /// <summary>
        /// ボタン処理中に画面遷移させない制御用
        /// </summary>       
        public static bool btnFanction = false;

        public int PageID;
        public string PageName;

        public ScanStoreInPage(int pageID , string pageName)
        {
            InitializeComponent();

            PageID = pageID;
            PageName = pageName;
        }

        protected override void OnAppearing()
        {
            vm = new ScanStoreInViewModel(PageID, PageName, Navigation);

            BindingContext = this.vm;

            this.vm.ActivityRunningEnd();
        }

    }


}