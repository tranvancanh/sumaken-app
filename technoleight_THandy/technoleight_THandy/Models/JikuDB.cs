using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace technoleight_THandy.Models
{
    public class JikuDB
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string OnOff { get; set; }


    }
}
