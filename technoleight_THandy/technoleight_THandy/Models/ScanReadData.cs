using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace THandy.Models
{
    public class ScanReadData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Kubn { get; set; }
        public string Scanstring { get; set; }
        public string Scanstring2 { get; set; }
        public int Sryou { get; set; }
        public string cuser { get; set; }
        public DateTime cdate { get; set; }
        public Double latitude { get; set; }
        public Double longitude { get; set; }
    }
}
