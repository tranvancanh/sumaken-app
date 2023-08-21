using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using technoleight_THandy.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using technoleight_THandy.Data;
using technoleight_THandy.Interface;
using technoleight_THandy.Models;
using System.Net.NetworkInformation;

namespace technoleight_THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanCameraPage : ContentPage
    {

        private INotificationManager notificationManager;
        int notificationNumber = 0;

        public ScanCameraPage()
        {
            InitializeComponent();
            var cameraViewModel = new ScanCameraViewModel();
            this.BindingContext = cameraViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;
            base.OnDisappearing();
        }

    }
}