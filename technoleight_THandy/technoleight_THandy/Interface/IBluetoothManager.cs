using System;
using System.Collections.Generic;
using System.Text;
using THandy.Event;
using THandy.Data;

namespace THandy.Interface
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