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
    public class HandyAdminModel
    {
        public class HandyAdminRequestBody
        {
            [Required]
            public int CompanyID { get; set; }

            string handyAdminPassword = string.Empty;
            [Required(ErrorMessage = "管理者パスワードを入力してください")]
            public string HandyAdminPassword
            {
                get
                {
                    return handyAdminPassword;
                }
                set 
                {
                    var password = (value ?? "").Trim();

                    if (String.IsNullOrEmpty(password))
                    {
                        throw new CustomExtention("管理者パスワードが未入力です");
                    }
                    else
                    {
                        handyAdminPassword = password;
                    }

                }
            }
        }
    }
}
