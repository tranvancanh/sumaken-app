using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
//using technoleight_THandy.Services;

namespace technoleight_THandy.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        //public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        //AboutPageの元ページのため削除不可　

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private bool backgroundLayerIsVisible = false;
        public bool BackgroundLayerIsVisible
        {
            get { return backgroundLayerIsVisible; }
            set { SetProperty(ref backgroundLayerIsVisible, value); }
        }

        private bool dialogIsVisible = false;
        public bool DialogIsVisible
        {
            get { return dialogIsVisible; }
            set { SetProperty(ref dialogIsVisible, value); }
        }

        string dialogTitleText = string.Empty;
        public string DialogTitleText
        {
            get { return dialogTitleText; }
            set { SetProperty(ref dialogTitleText, value); }
        }

        string dialogMainText = string.Empty;
        public string DialogMainText
        {
            get { return dialogMainText; }
            set { SetProperty(ref dialogMainText, value); }
        }

        string dialogSubText = string.Empty;
        public string DialogSubText
        {
            get { return dialogSubText; }
            set { SetProperty(ref dialogSubText, value); }
        }

        private bool dialogSubTextIsVisible = false;
        public bool DialogSubTextIsVisible
        {
            get { return dialogSubTextIsVisible; }
            set { SetProperty(ref dialogSubTextIsVisible, value); }
        }

        private bool contentIsVisible = false;
        public bool ContentIsVisible
        {
            get { return contentIsVisible; }
            set { SetProperty(ref contentIsVisible, value); }
        }

        private bool activityRunning = false;
        public bool ActivityRunning
        {
            get { return activityRunning; }
            set { SetProperty(ref activityRunning, value); }
        }

        private string activityRunningText;
        public string ActivityRunningText
        {
            get { return activityRunningText; }
            set { SetProperty(ref activityRunningText, value); }
        }

        private Color activityRunningColor;
        public Color ActivityRunningColor
        {
            get { return activityRunningColor; }
            set { SetProperty(ref activityRunningColor, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public bool ActivityRunningLoading()
        {
            ContentIsVisible = false;
            ActivityRunningText = Common.Const.ACTIVITYRUNNING_TEXT_LOADING;
            ResourceDictionary targetResource = Xamarin.Forms.Application.Current.Resources.MergedDictionaries.ElementAt(0);
            ActivityRunningColor = (Color)targetResource["MainColor"];
            ActivityRunning = true;
            return true;
        }

        public bool ActivityRunningProcessing()
        {
            ContentIsVisible = false;
            ActivityRunningText = Common.Const.ACTIVITYRUNNING_TEXT_PROCESSING;
            ResourceDictionary targetResource = Xamarin.Forms.Application.Current.Resources.MergedDictionaries.ElementAt(0);
            ActivityRunningColor = (Color)targetResource["AccentTextColor"];
            ActivityRunning = true;
            return true;
        }

        public bool ActivityRunningEnd()
        {
            ActivityRunning = false;
            ContentIsVisible = true;
            return true;
        }

    }
}
