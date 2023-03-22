using System;
using System.Collections.Generic;
using System.Text;

namespace technoleight_THandy.Common
{
    class Enums
    {
        public enum PageID
        {
            Receive_StoreIn_AddressMatchCheck = 206,
            Receive_StoreIn_TemporaryAddressMatchCheck = 207,
            Receive_StoreIn_AddressMatchCheck_PackingCountInput = 208,
            ReturnStoreAddress_AddressMatchCheck = 401,
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
            InputError = 9100, // 入力エラー
            OtherError = 9999, // その他エラー
        }
    }
}
