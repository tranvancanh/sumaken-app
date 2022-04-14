using System;
using System.Collections.Generic;
using System.Text;

namespace THandy.Models
{
    public class PageTypeGroup 
    {
        public string Title { get; set; }
        public string ShortName { get; set; } //will be used for jump lists
        //public PageTypeGroup(string title, string shortName)
        //{
        //    Title = title;
        //    ShortName = shortName;
        //}
      
        // public static IList<PageTypeGroup> All { private set; get; }
    }
}
