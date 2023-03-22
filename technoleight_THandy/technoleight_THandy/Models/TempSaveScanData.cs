using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    class TempSaveScanData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ScanString { get; set; }
        public Qrcode.QrcodeItem ScanData { get; set; }
    }
}
