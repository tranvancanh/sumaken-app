using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace technoleight_THandy.Models
{
    public class ReturnAgfLuggageStation
    {
        public class ReturnAgfLuggageStationPostBackBody
        {
            /// <summary>
            /// 移動登録成功データカウント
            /// </summary>
            public int SuccessDataCount { get; set; }
            /// <summary>
            /// 移動元データが存在しなかったデータカウント
            /// </summary>
            public int AgfLuggageStationNotFoundDataCount { get; set; }
            /// <summary>
            /// 移動元データが存在しなかったデータ
            /// </summary>
            public List<Qrcode.QrcodeItem> AgfLuggageStationNotFoundDatas { get; set; } = new List<Qrcode.QrcodeItem>();
        }

    }
}
