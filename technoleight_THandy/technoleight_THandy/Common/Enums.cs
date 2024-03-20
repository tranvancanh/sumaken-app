using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Common
{
    public class Enums
    {
        public enum PageID
        {
            /// <summary>
            /// 入荷・入庫処理・・・番地照合チェック
            /// </summary>
            Receive_StoreIn_AddressMatchCheck = 206,
            /// <summary>
            /// 入荷・入庫処理・・・仮番地照合チェック
            /// </summary>
            Receive_StoreIn_TemporaryAddressMatchCheck = 207,
            /// <summary>
            /// 入荷・入庫処理・・・番地照合チェック、箱数入力
            /// </summary>
            Receive_StoreIn_AddressMatchCheck_PackingCountInput = 208,
            /// <summary>
            /// 入庫番地戻し処理・・・番地照合チェック
            /// </summary>
            ReturnStoreAddress_AddressMatchCheck = 401,
            /// 出荷処理・・・出荷かんばん、製品かんばんをチェック
            /// </summary>
            Return_Agf_LuggageStationCheck = 601,
            /// AGF処理・・・荷取場所をチェック
            /// </summary>
            Return_Agf_SKanbanMatchCheck = 602,
            /// AGF処理・・・出荷かんばんをチェック
            /// </summary>
            Return_Agf_LaneNoCheck = 603,
            /// AGF処理・・・レーン番号をチェック
            /// </summary>
            Receive_StoreOut_ShippedData = 301,
            /// <summary>
            /// 登録済データ削除（テスト・デモ環境用）
            /// </summary>
            Receive_ServerData_Delete = 999,
        }

        public enum HandyOperationClass
        {
            Okey = 0,
            DuplicationError = 9001, // 重複エラー
            ExcludedScanError = 9010, // スキャン対象外エラー
            IncorrectQrcodeError = 9020, // 不正なQRエラー
            AddressError = 9030, // 番地エラーAddresMoveOnly
            ConversionFailedError = 9040, // QRの項目変換エラー
            NotBulkStoreInError = 9050, // まとめ入庫対象外エラー
            ProcedureIsDifferentError = 9060, // かんばんスキャンの順番が違うエラー
            DoubleCheckError = 9070, // 2重照合エラー
            InputError = 9100, // 入力エラー
            NameTagError = 9500, // 名札QRスキャンエラー
            OtherError = 9999, // その他エラー
        }

        public enum ProcessResultPattern
        {
            Okey = 0,
            Alert = 1,
            Error = 2,
        }

        public enum StoreOutState
        {
            Unknown = 0,  // invalid
            Process1 = 1, // 出荷かんばん
            Process2 = 2  // 製品かんばん
        }

        public enum ProductState
        {
            Unknown = 0,  // invalid
            Returnable = 1, // リターナブルかんばん
            EDI = 2  // EDIかんばん
        }

        public enum AGFShijiState
        {
            Unknown = 0,  // invalid
            Nitori = 1,  // AGF荷取り
            ShukaKanban = 2, // 出荷かんばん
            ShukaLane = 3  // 出荷レーン
        }
    }
}
