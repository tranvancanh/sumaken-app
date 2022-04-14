using System;
using System.Collections.Generic;
using System.Text;

namespace THandy.Common
{
    public static class Const
    {
        // スキャンモード設定
        public const string C_SCANMODE_KEYBOARD = "1";
        public const string C_SCANMODE_CAMERA = "2";
        public const string C_SCANMODE_BARCODE = "3";
        public const string C_SCANMODE_CLIPBOARD = "4";

        // スキャンモード名
        public const string C_SCANNAME_KEYBOARD = "キーボード";
        public const string C_SCANNAME_CAMERA = "カメラ";
        public const string C_SCANNAME_BARCODE = "バーコードリーダ(SPP)";
        public const string C_SCANNAME_CLIPBOARD = "クリップボード";

        // 接続状態
        public const string C_CONNET_OK = "接続";
        public const string C_CONNET_NG = "切断";
        public const string C_CONNET_TRY = "接続処理中";

        // Msg
        public const string C_MSG_NONE_PAIRING_INFO = "設定しない";

        // Error
        public const string C_ERR_KEY_NETWORK = "err";
        public const string C_ERR_VALUE_NETWORK = "NetWorkErr";
    }
}
