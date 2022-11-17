using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class NouhinJL
    {
        public string Name { get; set; }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string JLNOKU { get; set; }
        public string JLNOSE { get; set; }
        public string JLNONO { get; set; }
        public string JLLINE { get; set; }
        public string JLRNNO { get; set; }
        public string JLBUNO { get; set; }
        public string JLDATE { get; set; }
        public string JLNKSU { get; set; }
        public string JLDKEY { get; set; }
    }

    public class NouhinJL205
    {
        public string Name { get; set; }
        public string JLDKEY { get; set; }
        public string JLDATE { get; set; }

    }
}

