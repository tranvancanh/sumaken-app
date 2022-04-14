using System;
using System.Collections.Generic;
using System.Text;

namespace THandy.Interface
{
    public interface INotificationManager
    {
        //通知メッセージの作成
        event EventHandler NotificationReceived;
        void Initialize();
        void SendNotification(string title, string message, DateTime? notifyTime = null);
        void ReceiveNotification(string title, string message);
    }
}
