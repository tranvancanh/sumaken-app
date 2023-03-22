using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace technoleight_THandy.Models
{
    public class HandyUserViewModel
    {
        public int CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string HandyUserCode { get; set; }
        public string HandyUserName { get; set; }

        public string DepoCode { get; set; }
        public string DepoName { get; set; }

    }
}
