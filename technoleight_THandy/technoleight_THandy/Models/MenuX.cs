using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace technoleight_THandy.Models
{
    public class MenuX
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string WID { get; set; }
        public string gamen_id { get; set; }
        public string gamen_edaban { get; set; }
        public string gamen_name { get; set; }
    }
}
