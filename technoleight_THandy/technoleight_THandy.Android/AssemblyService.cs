using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using technoleight_THandy.Data;
using technoleight_THandy.Droid;
using Xamarin.Forms;
[assembly: Dependency(typeof(AssemblyService))]

namespace technoleight_THandy.Droid
{
    class AssemblyService : IAssemblyService
    {
        //アプリ名称を取得する
        public string GetPackageName()
        {
            Context context = Android.App.Application.Context;    //Android.App.Application.Context;
            var name = context.PackageManager.GetPackageInfo(context.PackageName, 0).PackageName;
            return name;
        }

        //アプリバージョン文字列を取得する
        public string GetVersionName()
        {
            Context context = Android.App.Application.Context;    //Android.App.Application.Context;
            var name = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            return name;
        }

        //アプリバージョンコードを取得する
        public string GetVersionCode()
        {
            Context context = Android.App.Application.Context;    //Android.App.Application.Context;
            long code = context.PackageManager.GetPackageInfo(context.PackageName, 0).LongVersionCode;
            string code1 = Convert.ToString(code);
            return code1;
        }
    }
}