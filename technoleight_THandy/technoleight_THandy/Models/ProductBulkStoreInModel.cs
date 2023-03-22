using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class ProductBulkStoreInModel
    {
        [PrimaryKey]
        public string ProductID { get; set; }
        public string ProductCode { get; set; }

        public ProductBulkStoreInModel()
        {

        }
    }
}
