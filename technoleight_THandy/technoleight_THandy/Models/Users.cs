using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class Users
    {
        [PrimaryKey]
        public string ShainNo { get; set; }
        public string Name { get; set; }
        public Users()
        {
            //Condition = "";
        }
    }
}
