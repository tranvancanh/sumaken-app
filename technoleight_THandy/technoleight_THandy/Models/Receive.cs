using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace technoleight_THandy.Models
{
    public class Receive
    {    
        public class ReceiveRegisteredData
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }

            public string SupplierCode { get; set; } = String.Empty;
            public string SupplierClass { get; set; } = String.Empty;
            public string ProductCode { get; set; } = String.Empty;
            public int ProductLabelBranchNumber { get; set; }
            public int Quantity { get; set; }
            public string Packing { get; set; } = String.Empty;
            public string NextProcess1 { get; set; } = String.Empty;
            public string NextProcess2 { get; set; } = String.Empty;
        }

        public class ReceivePostBackBody
        {
            /// <summary>
            /// 登録成功データカウント
            /// </summary>
            public int SuccessDataCount { get; set; }
            /// <summary>
            /// 既に登録済のデータカウント
            /// </summary>
            public int AlreadyRegisteredDataCount { get; set; }
            /// <summary>
            /// 既に登録済のデータ
            /// </summary>
            public List<Qrcode.QrcodeItem> AlreadyRegisteredDatas { get; set; } = new List<Qrcode.QrcodeItem>();
        }

        //public class RegisteredData
        //{
        //    public string RegisteredProductCode { get; set; } = String.Empty;
        //    public string RegisteredSupplierCode { get; set; } = String.Empty;
        //    public int RegisteredQuantity { get; set; }
        //    public int RegisteredProductLabelBranchNumber { get; set; }
        //}

    }
}
