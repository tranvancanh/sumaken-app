using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace technoleight_THandy.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "tozan";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.tozan.co.jp"));

            OpenWeb1Command = new Command(async () => await Browser.OpenAsync("https://www.asahi-sun-clean.co.jp"));
        }

        public ICommand OpenWebCommand { get; }
        public ICommand OpenWeb1Command { get; }
    }
}