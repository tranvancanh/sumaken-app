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

namespace technoleight_THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanReceiptPage : ContentPage
    {
        public ScanReceiptViewModel vm;

        /// <summary>
        /// ボタン処理中に画面遷移させない制御用
        /// </summary>       
        public static bool btnFanction = false;

        public ScanReceiptPage(ScanReceiptViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
            BindingContext = this.vm;
        }

    }


}