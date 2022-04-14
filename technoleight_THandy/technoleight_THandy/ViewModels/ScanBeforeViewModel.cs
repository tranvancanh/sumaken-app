using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using THandy.Models;
using System.Threading.Tasks;

namespace THandy.ViewModels
{
    public class ScanBeforeViewModel : INotifyPropertyChanged
    {
        private INavigation navigation;

        public event PropertyChangedEventHandler PropertyChanged;

        public Command ReadCommand { get; }

        public string Readkubun;

        public ScanBeforeViewModel(string name1, string kubun)
        {
            title = name1;
            Readkubun = kubun;
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(title)));
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

        private string txtuser;

        public string Txtuser { get => txtuser; set => SetProperty(ref txtuser, value); }

        public string Shaban { get; set; }

        private ObservableCollection<string> shabanItems;
        public ObservableCollection<string> ShabanItems
        {
            get { return shabanItems; }
            set
            {
                if (shabanItems != value)
                {
                    shabanItems = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShabanItems)));
                }
            }
        }

        private int shabanSelectedIndex;
        public int ShabanSelectedIndex
        {
            get { return shabanSelectedIndex; }
            set
            {
                if (shabanSelectedIndex != value)
                {
                    shabanSelectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShabanSelectedIndex)));
                }
            }
        }

        public string Dkey { get; set; }

    }
}
