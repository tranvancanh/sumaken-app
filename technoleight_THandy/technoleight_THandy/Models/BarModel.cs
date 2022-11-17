using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class BarModel
    {
        public string Shori { get; set; }
        public string WID { get; set; }
        public string UserID { get; set; }
        public string Device { get; set; }
        public string Shorikubun { get; set; }
        public string BarcodeRead { get; set; }
        public string BarcodeRead1 { get; set; }
        public string Sryou { get; set; }
        public string Cdate1 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string Nouki { get; set; }
        public List<string> BinList { get; set; }
        public string Shaban { get; set; }

        public string Dkey { get; set; }//積み増しで使用

        public string ShouhinCode { get; set; }//代表印刷で使用
        public string NyukoSuryo { get; set; }//代表印刷で使用

        public string ReceiptDate { get; set; }// 在庫入庫で使用
    }
}
