using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using THandy.ViewModels;
using THandy.Interface;
using THandy.Models;

namespace THandy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SeteiPage : ContentPage
    {
        INotificationManager notificationManager;
        int notificationNumber = 0;

        public SeteiPage()
        {
            InitializeComponent();
            this.BindingContext = new SeteiViewModel(this.Navigation);

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
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
    }
}