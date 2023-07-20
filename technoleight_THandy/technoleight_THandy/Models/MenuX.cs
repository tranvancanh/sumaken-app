using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace technoleight_THandy.Models
{
    public class MenuX
    {
        [PrimaryKey]
        public int HandyPageID { get; set; }
        public string HandyPageName { get; set; }
        public int HandyPageNumber { get; set; }
    }
}
