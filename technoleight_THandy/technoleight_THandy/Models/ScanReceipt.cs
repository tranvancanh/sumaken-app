using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class ScanReceipt
    {
        public string Result { get; set; }

        public string SupplierCode { get; set; }
        public string ProductCode { get; set; }
        public int ProductLabelBranchNumber { get; set; }
        public int ReceiptQuantity { get; set; }
        public string NextProcess1 { get; set; }
        public string NextProcess2 { get; set; }
        public string Packing { get; set; }

        public bool NotRegistFlag { get; set; }
        public DateTime ScanTime { get; set; }

         public ScanReceipt()
        {
            NotRegistFlag = false;
        }

    }

    public class ScanReceiptTotal
    {
        public string ProductCode { get; set; }
        public int ReceiptQuantity { get; set; }
        public int PackingCount { get; set; }

        public ScanReceiptTotal()
        {

        }

    }

}
