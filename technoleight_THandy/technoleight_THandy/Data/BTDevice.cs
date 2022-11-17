using System;
using System.Collections.Generic;
using System.Text;
using technoleight_THandy.Event;

namespace technoleight_THandy.Data
{
    public class BTDevice
    {
        public string strName { get; set; }

        // デバイス名のみではユニークではないのでUUIDも覚えておく
        // 同じデバイスで(HID,SPP)とモードが変わるとUUIDが同じの可能性はある。
        public string strUuid { get; set; }
    }
}
