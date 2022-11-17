using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace technoleight_THandy.Models
{
    public class PageItem
    {
        [PrimaryKey]
        public string ReadKubun { get; set; }
        public string Title { get; set; }
        //public INavigation Navigation { get; set; }
        public DateTime ReceiptDate { get; set; }
    }

}
