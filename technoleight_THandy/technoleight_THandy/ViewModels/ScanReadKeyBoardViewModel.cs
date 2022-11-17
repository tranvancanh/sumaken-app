using technoleight_THandy.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.SimpleAudioPlayer;
using System.IO;
using System;
using System.Text.RegularExpressions;
using technoleight_THandy;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;

namespace technoleight_THandy.ViewModels
{
    // シングルトンで呼び出すこと
    public class ScanReadKeyBoardViewModel : ScanReadViewModel
    {
        private static ScanReadKeyBoardViewModel scanReadKeyBoardViewModel;
        public static ScanReadKeyBoardViewModel GetInstance()
        {
            if (scanReadKeyBoardViewModel == null)
            {
                scanReadKeyBoardViewModel = new ScanReadKeyBoardViewModel();
                return scanReadKeyBoardViewModel;
            }
            return scanReadKeyBoardViewModel;
        }

        ~ScanReadKeyBoardViewModel()
        {
            Console.WriteLine("#ScanReadKeyBoardViewModel finish");
        }

        public void Initilize(string name1, string kubun, INavigation navigation)
        {
            base.init(name1, kubun, "", navigation);
            TxtCode = "";
        }

        public async Task OnCompleted()
        {
            try
            {
                await UpdateReadData(TxtCode, Common.Const.C_SCANMODE_KEYBOARD);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("#OnCompleted Err {0}", e.ToString());
            }
        }

    }
}
