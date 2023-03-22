using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace technoleight_THandy.Models
{
    public class ScanCommon
    {    
        public class ScanCommonApiPostRequestBody
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }

            [Required]
            public int DepoID { get; set; }
            [Required]
            public int HandyPageID { get; set; }
            [Required]
            public int HandyOperationClass { get; set; }

            public string HandyOperationMessage { get; set; } = String.Empty;

            public string ScanString1 { get; set; } = String.Empty;
            public string ScanString2 { get; set; } = String.Empty;
            public string ScanChangeData { get; set; } = String.Empty;

            [Required]
            public string ProcessDate { get; set; } = String.Empty;
            public string DuplicateCheckStartProcessDate { get; set; } = String.Empty;
            [Required]
            public int HandyUserID { get; set; }
            [Required]
            public string Device { get; set; } = String.Empty;
            [Required]
            public bool StoreInFlag { get; set; }

            public string ScanStoreAddress1 { get; set; } = String.Empty;
            public string ScanStoreAddress2 { get; set; } = String.Empty;
            public int InputQuantity { get; set; }
            public int InputPackingCount { get; set; }

            [Required]
            public DateTime ScanTime { get; set; }
            [Required]
            public double Latitude { get; set; }
            [Required]
            public double Longitude { get; set; }
        }

    }
}
