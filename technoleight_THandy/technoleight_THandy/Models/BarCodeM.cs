using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class BarCodeM
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string WID { get; set; }
        public string gamen_id { get; set; }
        public string gamen_edaban { get; set; }
        public string edaban { get; set; }

        public string IndexString { get; set; }
        public string BuhinStart { get; set; }
        public string BuhinEnd { get; set; }
        public string SryouStart { get; set; }
        public string SryouEnd { get; set; }
        public string TyoufukuOKFlag { get; set; }
        public string SouRyouInputFlg { get; set; }
        public string Ketasu { get; set; }
    }
}
