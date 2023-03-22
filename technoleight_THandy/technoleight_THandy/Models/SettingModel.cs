using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SQLite;
using technoleight_THandy.Data;
using Xamarin.Forms;

namespace technoleight_THandy.Models
{
    public class Setting
    {
        public class SettingApiRequestBody
        {
            [Required]
            public string CompanyCode { get; set; } = string.Empty;
            [Required]
            public string CompanyPassword { get; set; } = string.Empty;
            [Required]
            public string HandyUserCode { get; set; } = string.Empty;
            [Required]
            public string Device
            {
                get
                {
                    string device = DependencyService.Get<IDeviceService>().GetID();
                    return device;
                }
            }
            [Required]
            public string DeviceName
            {
                get
                {
                    string manufacturerName = DependencyService.Get<IDeviceService>().GetManufacturerName();
                    string modelName = DependencyService.Get<IDeviceService>().GetModelName();
                    string deviceVersion = DependencyService.Get<IDeviceService>().GetDeviceVersion();
                    string deviceName = manufacturerName + "[" + modelName + "]-" + deviceVersion;

                    return deviceName;
                }
            }
        }

        public class SettingApiResponceBody
        {
            public int CompanyID { get; set; }
            public int HandyUserID { get; set; }
            public int PasswordMode { get; set; }
        }

        public class SettingSqlLite
        {
            [PrimaryKey]
            public int CompanyID { get; set; }
            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public string CompanyPassword { get; set; }

            public string HandyApiUrl { get; set; }
            public int HandyUserID { get; set; }
            public string HandyUserCode { get; set; }
            public string HandyUserPassword { get; set; }
            public string HandyUserName { get; set; }

            public string Device { get; set; }
            public string DeviceName { get; set; }

            public string UUID { get; set; }
            public string ScanMode { get; set; }
            public int PasswordMode { get; set; }

            public string ScanReader { get; set; }

            public string ScanOkeySound { get; set; }
            public string ScanErrorSound { get; set; }

            public Theme ColorTheme { get; set; }
        }
    }
}
