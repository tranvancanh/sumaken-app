using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Models
{
    public class TemporaryStoreAddressModel
    {
        [PrimaryKey]
        public string TemporaryStoreAddressID { get; set; }
        public string TemporaryStoreAddress1 { get; set; }
        public string TemporaryStoreAddress2 { get; set; }

        public TemporaryStoreAddressModel()
        {

        }
    }
}
