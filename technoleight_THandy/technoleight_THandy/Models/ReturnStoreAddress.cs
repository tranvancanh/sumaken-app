using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace technoleight_THandy.Models
{
    public class ReturnStoreAddress
    {
        public class ReturnStoreAddressPostBackBody
        {
            /// <summary>
            /// 移動登録成功データカウント
            /// </summary>
            public int SuccessDataCount { get; set; }
            /// <summary>
            /// 移動元データが存在しなかったデータカウント
            /// </summary>
            public int StoreInNotFoundDataCount { get; set; }
            /// <summary>
            /// 移動元データが存在しなかったデータ
            /// </summary>
            public List<Qrcode.QrcodeItem> StoreInNotFoundDatas { get; set; } = new List<Qrcode.QrcodeItem>();
        }

    }
}
