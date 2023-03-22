using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class Login
    {
        public class LoginApiRequestBody
        {
            public int CompanyID { get; set; }
            public int HandyUserID { get; set; }
            public string HandyUserCode { get; set; }
            public int PasswordMode { get; set; }
            public string HandyUserPassword { get; set; }
            public string Device { get; set; }
            public decimal HandyAppVersion { get; set; }
        }

        public class LoginApiResponceBody
        {
            public string CompanyName { get; set; } = string.Empty;
            public int DepoID { get; set; }
            public string DepoCode { get; set; } = string.Empty;
            public string DepoName { get; set; } = string.Empty;
            public int AdministratorFlag { get; set; }
            public string HandyUserName { get; set; } = string.Empty;
        }

        public class LoginUserSqlLite
        {
            [PrimaryKey, AutoIncrement, Column("_id")]
            public int Id { get; set; }

            public string CompanyCode { get; set; }
            public string HandyUserCode { get; set; }

            public string CompanyName { get; set; }
            public string HandyUserName { get; set; }
            public int AdministratorFlag { get; set; }

            public int DepoID { get; set; }
            public string DepoCode { get; set; }
            public string DepoName { get; set; }

        }

    }
}
