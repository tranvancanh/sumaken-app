using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Foundation;
using technoleight_THandy.Data;
using technoleight_THandy.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceService))]

namespace technoleight_THandy.iOS
{
   
    class DeviceService : IDeviceService
    {
        // OSバージョンが指定した数値より大きいかどうかを判定する
        public bool IsUpperVersion(int major, int minor)
        {
            return UIDevice.CurrentDevice.CheckSystemVersion(major, minor);
        }
        // OSバージョンを取得する
        public string GetDeviceVersion()
        {
            return UIDevice.CurrentDevice.SystemVersion;
        }
        // メーカー名を取得する
        public string GetManufacturerName()
        {
            //iOSはAppleしかなく、DependencySerivceの為に記述
            return "Apple";
        }
        // 型番を取得する
        public string GetModelName()
        {
            return UIDevice.CurrentDevice.LocalizedModel;
        }
        // CPUを取得する
        public string GetCpuType()
        {
            //returned is one of X86, X64, ARM, ARM64
            return System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
        }
        public string GetID()
        {
            //returned is one of X86, X64, ARM, ARM64
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }
    }
}