using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class Nouhin
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string VHNOKU { get; set; }
        public string VHNOSE { get; set; }
        public string VHNONO { get; set; }
        public string VILINE { get; set; }
        public string VHTRCD { get; set; }
        public string VHKOKU { get; set; }
        public string VHNOBA { get; set; }
        public string VHDATE { get; set; }
        public string VHJIKU { get; set; }
        public string VIBUNO { get; set; }
        public string VIBUNM { get; set; }
        public string VIJIKO { get; set; }
        public string VISRYO { get; set; }
        public string JJNKSU { get; set; }
        public string JJNHSU { get; set; }
        public string VIYOSU { get; set; }
        public string VILOSU { get; set; }
        public int JLSTARTSEQ { get; set; }
        public bool IsRead { get; set; }

        public string NOBAHANDAN { get; set; }

        public Nouhin()
        {
            IsRead = false;
        }


    }
}
