using System.Collections.Generic;
using static technoleight_THandy.Models.Qrcode;

namespace technoleight_THandy.Models
{
    public class StoreOutModel
    {
        public QrcodeItem StoreOutQrcodeItem { get; set; }
        public List<QrcodeItem> ListProductQrcodeItems { get; set; }

        public StoreOutModel()
        {
            StoreOutQrcodeItem = null;
            ListProductQrcodeItems = new List<QrcodeItem>();
        }
    }

    
}
