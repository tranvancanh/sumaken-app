using System;
using System.Collections.Generic;

namespace technoleight_THandy.Models
{
    public class NotSendData
    {
        public int PageID { get; set; }
        public string ProcessDate { get; set; }
        public int DataCount { get; set; }
    }

    public class NotSendDataGroup : List<NotSendData>
    {
        // グループのヘッダ
        public int PageID { get; private set; }
        public string PageName { get; private set; }

        public NotSendDataGroup(int pageID, string pageName, List<NotSendData> notSendDatas) : base(notSendDatas)
        {
            this.PageID = pageID;
            this.PageName = pageName;
        }
    }
}
