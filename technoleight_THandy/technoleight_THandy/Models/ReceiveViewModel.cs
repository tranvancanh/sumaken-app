using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using technoleight_THandy.Data;
using Xamarin.Forms;

namespace technoleight_THandy.Models
{
    public class ReceiveViewModel
    {
        public string ProductCode { get; set; }
        public int ProductLabelBranchNumber { get; set; }
        public int ProductLabelBranchNumber2 { get; set; } // 出荷枝番(出庫用)
        public int LotQuantity { get; set; }
        public int PackingCount { get; set; }
        public string NextProcess1 { get; set; }
        public string NextProcess2 { get; set; }
        public string StoreInAddress1 { get; set; }
        public string StoreInAddress2 { get; set; }
        public string StoreOutAddress1 { get; set; }
        public string StoreOutAddress2 { get; set; }

        public ReceiveViewModel()
        {
            PackingCount = 1;
        }
    }

    public class ReceiveTotalViewModel
    {
        public string ProductCode { get; set; }
        public int LotQuantity { get; set; }
        public string StoreInAddress1 { get; set; }
        public string StoreInAddress2 { get; set; }
        public string StoreOutAddress1 { get; set; } // 出庫用
        public string StoreOutAddress2 { get; set; } // 出庫用

        public int PackingTotalCount { get; set; }
        public int TotalQuantity
        {
            get
            {
                int quantity = 0;
                if (LotQuantity != 0 && PackingTotalCount != 0)
                {
                    quantity = LotQuantity * PackingTotalCount;
                }
                return quantity;
            }
        }
    }

}
