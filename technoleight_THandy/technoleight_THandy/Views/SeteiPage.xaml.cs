using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using technoleight_THandy.ViewModels;
using technoleight_THandy.Interface;
using technoleight_THandy.Models;
using technoleight_THandy.Data;
using System.IO;
using System.Reflection;
using Plugin.SimpleAudioPlayer;
using System.Collections.ObjectModel;
using Android.Graphics.Fonts;
using Font = Xamarin.Forms.Font;
using System.Drawing.Text;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using Color = Xamarin.Forms.Color;
using technoleight_THandy.Controls;

namespace technoleight_THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SeteiPage : ContentPage
    {
        INotificationManager notificationManager;
        int notificationNumber = 0;

        Sound sound = new Sound();

        public SeteiViewModel vm;

        public SeteiPage()
        {
            InitializeComponent();
            vm = new SeteiViewModel(this.Navigation);
            this.BindingContext = vm;

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };

            // サウンドPickerセット
            PickSoundOkey.IsVisible = true;

            var pickerOkey = new List<string>();
            foreach (var item in sound.SoundOkeyList)
            {
                pickerOkey.Add(item.DisplayName);
            }
            PickSoundOkey.ItemsSource = pickerOkey;

            var pickerError = new List<string>();
            foreach (var item in sound.SoundErrorList)
            {
                pickerError.Add(item.DisplayName);
            }
            PickSoundError.ItemsSource = pickerError;

            SetSoundPicker();

            // テーマカラーセット
            SetThemeColor();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }



        private async void SetThemeColor()
        {

            List<Setting.SettingSqlLite> Set2 = await App.DataBase.GetSettingAsync();
            if (Set2.Count > 0)
            {
                ThemeColorPicker.SelectedItem = Set2[0].ColorTheme;
            }
            else
            {
                ThemeColorPicker.SelectedIndex = 0;
            }

        }

        protected async void SetSoundPicker()
        {
            List<Setting.SettingSqlLite> Set2 = await App.DataBase.GetSettingAsync();
            if (Set2.Count > 0)
            {
                var soundOkey = Set2[0].ScanOkeySound;
                var soundError = Set2[0].ScanErrorSound;

                if (String.IsNullOrEmpty(soundOkey))
                {
                    PickSoundOkey.SelectedIndex = 0;
                }
                else
                {
                    var soundName = sound.SoundOkeyList.Where(x => x.Item == soundOkey);
                    if (soundName.Count() > 0)
                    {
                        PickSoundOkey.SelectedItem = soundName.FirstOrDefault().DisplayName;
                    }
                    else
                    {
                        PickSoundOkey.SelectedIndex = 0;
                    }
                }

                if (String.IsNullOrEmpty(soundError))
                {
                    PickSoundError.SelectedIndex = 0;
                }
                else
                {
                    var soundName = sound.SoundErrorList.Where(x => x.Item == soundError);
                    if (soundName.Count() > 0) 
                    {
                        PickSoundError.SelectedItem = soundName.FirstOrDefault().DisplayName;
                    }
                    else
                    {
                        PickSoundError.SelectedIndex = 0;
                    }
                }

            }
            else
            {
                PickSoundOkey.SelectedIndex = 0;
                PickSoundError.SelectedIndex = 0;
            }

            var soundSelectedEvent = new EventHandler(PickSoundSelectedIndexChanged);
            PickSoundOkey.SelectedIndexChanged += soundSelectedEvent;
            PickSoundError.SelectedIndexChanged += soundSelectedEvent;
        }

        void OnSendClick(object sender, EventArgs e)
        {
            notificationNumber++;
            string title = $"Local Notification #{notificationNumber}";
            string message = $"You have now received {notificationNumber} notifications!";
            notificationManager.SendNotification(title, message);
        }

        void OnScheduleClick(object sender, EventArgs e)
        {
            notificationNumber++;
            string title = $"THandy #{notificationNumber}新しいバージョンが公開されました。";
            string message = $"クリックして更新をしてください!";
            notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));
        }

        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
                //stackLayout.Children.Add(msg);
            });
        }

        private void PickSoundSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                var sound = new Sound();

                string soundItem = "";

                if (picker == PickSoundOkey)
                {
                    soundItem = sound.SoundOkeyList[selectedIndex].Item;
                }
                else if (picker == PickSoundError)
                {
                    soundItem = sound.SoundErrorList[selectedIndex].Item;
                }

                if (soundItem != "")
                {
                    ISimpleAudioPlayer SEplayer = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

                    //音鳴らす
                    SEplayer.Load(GetStreamFromFile(soundItem));
                    SEplayer.Play();
                }

            }

        }

        protected override bool OnBackButtonPressed()
        {
            vm.OnCancelClicked();
            return true;
        }

        //void OnPickerSelectionChanged(object sender, EventArgs e)
        //{
        //    Picker picker = sender as Picker;
        //    Theme theme = (Theme)picker.SelectedItem;

        //    ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        //    if (mergedDictionaries != null)
        //    {
        //        mergedDictionaries.Clear();

        //        switch (theme)
        //        {
        //            case Theme.Dark:
        //                mergedDictionaries.Add(new DarkTheme());
        //                break;
        //            case Theme.Light:
        //            default:
        //                mergedDictionaries.Add(new LightTheme());
        //                break;
        //        }
        //    }
        //}

        private Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            //ビルドアクションで埋め込みリソースにしたファイルを取ってくる
            var stream = assembly.GetManifestResourceStream(filename);
            return stream;
        }

    }
}