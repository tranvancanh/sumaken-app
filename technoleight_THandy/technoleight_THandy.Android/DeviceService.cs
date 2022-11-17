using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using technoleight_THandy.Data;
using technoleight_THandy.Droid;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceService))]

namespace technoleight_THandy.Droid
{
    public class DeviceService : IDeviceService
    {
        // OSバージョンが指定した数値より大きいかどうかを判定する
        public bool IsUpperVersion(int major, int minor)
        {
            if ((int)(Build.VERSION.SdkInt) >= major)
            {
                return true;
            }
            return false;
        }

        // OSバージョンを取得する
        public string GetDeviceVersion()
        {
            return Build.VERSION.Release;
        }
        // メーカー名を取得する
        public string GetManufacturerName()
        {
            return Build.Manufacturer;
        }
        // 型番を取得する
        public string GetModelName()
        {
            return Build.Model;
        }
        //CPUを取得する
        public string GetCpuType()
        {
            //return Build.CpuAbi.ToString()
            return Java.Lang.JavaSystem.GetProperty("os.arch");
        }

        [Obsolete]
        public string GetID()
        {
            string id = "";
            id = Build.Serial;
            if (string.IsNullOrWhiteSpace(id) || id == Build.Unknown || id == "0")
            {
                try
                {
                    var context = Android.App.Application.Context;
                    id = Secure.GetString(context.ContentResolver, Secure.AndroidId);
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
                }
            }
            //return Build.CpuAbi.ToString()
            return id;
        }

    }
}
