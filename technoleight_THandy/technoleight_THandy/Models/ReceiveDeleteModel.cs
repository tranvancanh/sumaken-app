using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SQLite;
using technoleight_THandy.Data;
using technoleight_THandy.Models.common;
using Xamarin.Forms;

namespace technoleight_THandy.Models
{
    public class ReceiveDeleteModel
    {
        public class ReceiveDeleteRequestBody
        {
            [Required]
            public string DeleteReceiveStartDate { get; set; }

            [Required]
            public int UserID
            { 
                get{ return App.Setting.HandyUserID; }
                set { }
            }

        }

        public class ReceiveDeletePostBackBody
        {
            /// <summary>
            /// 削除データカウント
            /// </summary>
            public int DeleteReceiveDataCount { get; set; }
        }
    }
}
