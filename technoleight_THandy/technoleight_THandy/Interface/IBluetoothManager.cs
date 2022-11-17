using System;
using System.Collections.Generic;
using System.Text;
using technoleight_THandy.Event;
using technoleight_THandy.Data;

namespace technoleight_THandy.Interface
{
    public interface IBluetoothManager
    {
        // データ受信イベント
        event EventHandler<DataEventArgs> DataReceived;
        // 接続状態変更通知イベント
        event EventHandler<DataEventArgs> NotifyConnet;
        void BTConnet(BTDevice bTDevice);
        void BTDisConnet();
        // ペアリング情報取得
        List<BTDevice> GetBondedDevices();
    }
}
;