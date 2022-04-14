using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace THandy.Models
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
