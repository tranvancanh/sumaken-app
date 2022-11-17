using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Foundation;
using Xamarin.Forms;
using technoleight_THandy.iOS;
using technoleight_THandy.Data;

[assembly: Dependency(typeof(AssemblyService))]

namespace technoleight_THandy.iOS
{
    class AssemblyService : IAssemblyService
    {
        //アプリ名称を取得する
        public string GetPackageName()
        {
            string name = NSBundle.MainBundle.InfoDictionary["CFBundleDisplayName"].ToString();
            return name.ToString();
        }
        //アプリバージョン文字列を取得する
        public string GetVersionName()
        {
            string name = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
            return name.ToString();
        }
        //アプリバージョンコードを取得する
        public string GetVersionCode()
        {
            string code = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
            return code.ToString();
        }
    }
}