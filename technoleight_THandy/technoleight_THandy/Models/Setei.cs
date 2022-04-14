using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace THandy.Models
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

        public string Device { get; set; }
        public string ScanMode { get; set; }
        public string PassMode { get; set; }

        public string BarcodeReader { get; set; }

        public string UUID { get; set; }
    }
}
