using System;
using System.Collections.Generic;
using System.Text;
using THandy.Event;

namespace THandy.Data
{
    public class BTDevice
    {
        public string strName { get; set; }

        // デバイス名のみではユニークではないのでUUIDも覚えておく
        // 同じデバイスで(HID,SPP)とモードが変わるとUUIDが同じの可能性はある。
        public string strUuid { get; set; }
    }
}
