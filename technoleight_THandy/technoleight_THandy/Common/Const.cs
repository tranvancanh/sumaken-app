using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Common
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
        //public const string C_SCANNAME_BARCODE = "Bluetoothリーダー";
        public const string C_SCANNAME_CLIPBOARD = "ハンディリーダー";

        // 接続状態
        public const string C_CONNET_OK = "接続";
        public const string C_CONNET_NG = "切断";
        public const string C_CONNET_TRY = "接続処理中";

        // Msg
        public const string C_MSG_NONE_PAIRING_INFO = "設定しない";

        // Error
        public const string C_ERR_KEY_NETWORK = "err";
        public const string C_ERR_VALUE_NETWORK = "NetWorkErr";

        // Application Session
        public const string C_APPLICATION_SESSION_LASTSLEEPTIME = "LastSleepTime";

        #region ActivityRunning
        public const string ACTIVITYRUNNING_TEXT_LOADING = "ロード中...";
        public const string ACTIVITYRUNNING_TEXT_PROCESSING = "処理中...";
        #endregion

        // Default Enter
        public const string ENTER_BUTTON = "OK";

        // Default Okey
        public const string OKEY_DEFAULT_TITLE = "完了";
        public const string OKEY_DEFAULT_MESSAGE = "完了しました";
        public const string OKEY_DEFAULT_REGISTED_MESSAGE = "データ登録が完了しました";

        // Default Alert
        public const string ALERT_DEFAULT_TITLE = "警告";

        // Default Error
        public const string ERROR_DEFAULT = "エラーが発生しました";

        // Server Error
        public const string API_GET_ERROR_DEFAULT = "サーバーデータの取得エラーが発生しました";

        #region Scan

        // Okey
        public const string SCAN_OKEY = "スキャンOK";
        public const string SCAN_OKEY_SET_ADDRESS = "番地OK　次は製品かんばん";
        public const string SCAN_OKEY_STORE_OUT = "次は製品かんばんをスキャン";

        // Error
        public const string SCAN_ERROR_DEFAULT = "エラー";
        public const string SCAN_ERROR_OTHER = "予期せぬエラー";

        public const string SCAN_ERROR_INCORRECT_QR = "不正なQR";

        public const string SCAN_ERROR_DUPLICATION = "スキャン済のかんばん";
        public const string SCAN_ERROR_REGIST_DUPLICATION = "登録済のかんばん";

        public const string SCAN_ERROR_STORE_OUT = "出荷かんばんがスキャン";
        public const string SCAN_ERROR_STORE_OUT_DUPLICATION = "出荷かんばんがスキャン済です";
        public const string SCAN_ERROR_PRODUCT_DUPLICATION = "製品かんばんがスキャン済です";
        public const string SCAN_ERROR_REGIST_STORE_OUT_DUPLICATION = "出荷かんばんが登録済です";
        public const string SCAN_ERROR_REGIST_PRODUCT_DUPLICATION = "製品かんばんが登録済です";

        public const string SCAN_ERROR_NOT_STOCK = "在庫対象外";

        public const string SCAN_ERROR_NOT_SET_ADDRESS = "番地をスキャン";
        public const string SCAN_ERROR_NOT_ADDRESS = "番地の未セット";
        public const string SCAN_ERROR_NOT_MATCH_ADDRESS = "番地の不一致";
        public const string SCAN_ERROR_MATCH_ADDRESS = "番地の一致";
        public const string SCAN_ERROR_INCORRECT_ADDRESS_QR = "不正な番地QR";
        public const string SCAN_ERROR_NOT_BULK_STOREIN_PRODUCT = "まとめ入庫対象外";

        // Login Scan
        public const string SCAN_NAMETAG_STRING = "***NAMETAG***";
        public const string SCAN_NAMETAG_ERROR_INCORRECT_STRING = "不正な名札QR";
        public const string SCAN_NAMETAG_ERROR = "名札QRスキャンエラー";

        // AddressStartString
        public const string SCAN_ADDRESS_START_STRING_1 = "STORE";
        public const string SCAN_ADDRESS_START_STRING_2 = "***ADDRESS***";

        public const string SCAN_ADDRESS_TITLE_TEXT = "番地セット完了";
        public const string SCAN_ADDRESS_SUB_TEXT = "次は製品かんばんをスキャン";

        // 実行スキャン文字列
        public const string SCAN_EXECUTION_KEY_STRING_1 = "***EXECUTION***";

        #endregion Scan

        #region Input

        public const string INPUT_OKEY_SET_PACKING_COUNT = "箱数セットOK";

        public const string INPUT_ERROR_DEFAULT = "エラー";
        public const string INPUT_ERROR_REQUIRED_PACKING_COUNT = "0以上で入力してください";

        #endregion Input
    }
}
