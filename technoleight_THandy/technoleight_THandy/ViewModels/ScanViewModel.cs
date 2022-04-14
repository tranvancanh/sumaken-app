using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.Threading.Tasks;
using THandy.Models;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using THandy;
using System.ComponentModel;
using System.Threading;
using THandy.Views;

namespace THandy.ViewModels
{
    public class ScanViewModel : INotifyPropertyChanged
    {
        private INavigation navigation;

        public event PropertyChangedEventHandler PropertyChanged;

        public Command ReadCommand { get; }

        public string Readkubun;

        public string Nouki;
        public List<string> BinLIst;
        public string Shaban;
        public Int64 Dkey;
        public string Nyukobi;
        public int SetCount;

        public ScanViewModel(string name1, string kubun, string nouki, List<string> bin, string shaban, Int64 dkey)
        {
            title = name1;
            Readkubun = kubun;
            BinLIst = bin;
            Nouki = nouki;
            Shaban = shaban;
            Dkey = dkey;
        }
        public ScanViewModel(string name1, string kubun, string nouki, List<string> bin, Int64 dkey, string nyukobi, int setCount)
        {
            title = name1;
            Readkubun = kubun;
            BinLIst = bin;
            Nouki = nouki;
            Dkey = dkey;
            Nyukobi = nyukobi;
            SetCount = setCount;
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

        private string btnLeft;
        public string BtnLeft
        {
            get { return btnLeft; }
            set
            {
                if (btnLeft != value)
                {
                    btnLeft = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(btnLeft)));
                }
            }
        }

        private string btnRight;
        public string BtnRight
        {
            get { return btnRight; }
            set
            {
                if (btnRight != value)
                {
                    btnRight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(btnRight)));
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
    }

}
