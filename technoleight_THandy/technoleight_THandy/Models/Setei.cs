using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace technoleight_THandy.Models
{
    public class Setei
    {
        [PrimaryKey]
        public string WID{ get; set; }
        public string url { get; set; }
        public string k_pass { get; set; }
        public string user { get; set; }
        public string userpass { get; set; }
        public string username { get; set; }

        public string CompanyName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }

        public string Device { get; set; }
        public string ScanMode { get; set; }
        public string PassMode { get; set; }

        public string BarcodeReader { get; set; }

        public string ScanOkeySound { get; set; }
        public string ScanErrorSound { get; set; }

        public Theme ColorTheme { get; set; }

        public string UUID { get; set; }
    }
}
