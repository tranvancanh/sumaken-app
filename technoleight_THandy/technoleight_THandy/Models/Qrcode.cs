using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace technoleight_THandy.Models
{
    public class Qrcode
    {
        public class QrcodeItem
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }

            /// <summary>
            /// ページID
            /// </summary>
            public int PageID { get; set; }
            /// <summary>
            /// 処理日
            /// </summary>
            public string ProcessceDate { get; set; }
            /// <summary>
            /// 入力数量
            /// </summary>
            public int InputPackingQuantity { get; set; }
            /// <summary>
            /// 入力箱数
            /// </summary>
            public int InputPackingCount { get; set; }
            /// <summary>
            /// 在庫番地1
            /// </summary>
            public string ScanStoreAddress1 { get; set; }
            /// <summary>
            /// 在庫番地2
            /// </summary>
            public string ScanStoreAddress2 { get; set; }
            /// <summary>
            /// スキャンした時間
            /// </summary>
            public DateTime ScanTime { get; set; }

            /// <summary>
            /// 納期
            /// </summary>
            public string DeleveryDate { get; set; }
            /// <summary>
            /// 便
            /// </summary>
            public string DeliveryTimeClass { get; set; }
            /// <summary>
            /// データ区分
            /// </summary>
            public string DataClass { get; set; }
            /// <summary>
            /// 発注区分
            /// </summary>
            public string OrderClass { get; set; }
            /// <summary>
            /// 伝票番号
            /// </summary>
            public string DeliverySlipNumber { get; set; }
            /// <summary>
            /// 仕入先コード
            /// </summary>
            public string SupplierCode { get; set; }
            /// <summary>
            /// 仕入先区分
            /// </summary>
            public string SupplierClass { get; set; }
            /// <summary>
            /// 製品コード
            /// </summary>
            public string ProductCode { get; set; }
            /// <summary>
            /// 製品略称（略番、背番）
            /// </summary>
            public string ProductAbbreviation { get; set; }
            /// <summary>
            /// 発行枝番（シリアル）
            /// </summary>
            public int ProductLabelBranchNumber { get; set; }
            /// <summary>
            /// 数量
            /// </summary>
            public int Quantity { get; set; }
            /// <summary>
            /// 次工程1（納入先1）
            /// </summary>
            public string NextProcess1 { get; set; }
            /// <summary>
            /// 置き場1（受入1）
            /// </summary>
            public string Location1 { get; set; }
            /// <summary>
            /// 次工程2（納入先2）
            /// </summary>
            public string NextProcess2 { get; set; }
            /// <summary>
            /// 箱種（荷姿）
            /// </summary>
            public string Packing { get; set; }
            /// <summary>
            /// 置き場2（受入2）
            /// </summary>
            public string Location2 { get; set; }

            ///// <summary>
            ///// API受渡し結果取得用
            ///// </summary>
            //public string Result { get; set; }

            ///// <summary>
            ///// 画面表示用：未登録フラグ
            ///// </summary>
            //public bool NotRegistFlag { get; set; }

            ///// <summary>
            ///// 画面表示用：箱数の集計値
            ///// </summary>
            //public int PackingCount { get; set; }

            public QrcodeItem()
            {
                //NotRegistFlag = false;
                ScanTime = DateTime.Now;
            }

        }

        public class QrcodeIndex
        {
            /// <summary>
            /// API受渡し結果取得用
            /// </summary>
            public string Result { get; set; }

            public string IdentifyString { get; set; }
            public int IdentifyIndex { get; set; }

            public int MaxStringLength { get; set; }

            public int DeleveryDateIndex { get; set; }
            public int DeliveryTimeClassIndex { get; set; }
            public int DataClassIndex { get; set; }
            public int OrderClassIndex { get; set; }
            public int DeliverySlipNumberIndex { get; set; }
            public int SupplierCodeIndex { get; set; }
            public int SupplierClassIndex { get; set; }
            public int ProductCodeIndex { get; set; }
            public int ProductAbbreviationIndex { get; set; }
            public int ProductLabelBranchNumberIndex { get; set; }
            public int QuantityIndex { get; set; }
            public int NextProcess1Index { get; set; }
            public int Location1Index { get; set; }
            public int NextProcess2Index { get; set; }
            public int Location2Index { get; set; }
            public int PackingIndex { get; set; }

            public int DeleveryDateLength { get; set; }
            public int DeliveryTimeClassLength { get; set; }
            public int DataClassLength { get; set; }
            public int OrderClassLength { get; set; }
            public int DeliverySlipNumberLength { get; set; }
            public int SupplierCodeLength { get; set; }
            public int SupplierClassLength { get; set; }
            public int ProductCodeLength { get; set; }
            public int ProductAbbreviationLength { get; set; }
            public int ProductLabelBranchNumberLength { get; set; }
            public int QuantityLength { get; set; }
            public int NextProcess1Length { get; set; }
            public int Location1Length { get; set; }
            public int NextProcess2Length { get; set; }
            public int Location2Length { get; set; }
            public int PackingLength { get; set; }

        }

        public static string QrcodeValueSubstring(int index, int stringLength, string qr)
        {
            var value = qr.Substring(index - 1, stringLength).Trim();
            return value;
        }

        public static Task<(bool result, string message, QrcodeItem item)> GetQrcodeItem(string qr, List<QrcodeIndex> indexList)
        {
            bool result = false;
            string message = "";
            QrcodeItem item = new QrcodeItem();

            //// 未登録データ
            //item.NotRegistFlag = true;

            var identifyOK = false;
            foreach (var index in indexList)
            {
                if (indexList.Count == 1 && index.IdentifyIndex == 0)
                {
                    // QRは1種類で識別文字が無い場合は、識別文字チェックを行わない
                    identifyOK = true;
                }
                else
                {
                    // 識別文字の長さ以上の文字数であるか
                    //var identifyStringLength = index.IdentifyString.Length;
                    //if (identifyStringLength >= qr.Length)
                    //{
                    //    message = Common.Const.SCAN_ERROR_INCORRECT_QR;
                    //    return Task.FromResult((result, message, item));
                    //}

                    // 識別文字が一致しているか
                    var identifyStringLength = index.IdentifyString.Length;
                    var qrIdentifyString = QrcodeValueSubstring(index.IdentifyIndex, identifyStringLength, qr);
                    if (qrIdentifyString == index.IdentifyString)
                    {
                        identifyOK = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                // 識別チェックに成功したか
                if (!identifyOK)
                {
                    message = "識別文字エラー";
                    return Task.FromResult((result, message, item));
                }

                // QRから各項目をセットしていく
                item.DeliveryTimeClass = index.DeliveryTimeClassIndex == 0 ? "" : QrcodeValueSubstring(index.DeliveryTimeClassIndex, index.DeliveryTimeClassLength, qr);
                item.DeliverySlipNumber = index.DeliverySlipNumberIndex == 0 ? "" : QrcodeValueSubstring(index.DeliverySlipNumberIndex, index.DeliverySlipNumberLength, qr);
                item.DataClass = index.DataClassIndex == 0 ? "" : QrcodeValueSubstring(index.DataClassIndex, index.DataClassLength, qr);
                item.OrderClass = index.OrderClassIndex == 0 ? "" : QrcodeValueSubstring(index.OrderClassIndex, index.OrderClassLength, qr);
                item.SupplierCode = index.SupplierCodeIndex == 0 ? "" : QrcodeValueSubstring(index.SupplierCodeIndex, index.SupplierCodeLength, qr);
                item.SupplierClass = index.SupplierClassIndex == 0 ? "" : QrcodeValueSubstring(index.SupplierClassIndex, index.SupplierClassLength, qr);
                item.ProductCode = index.ProductCodeIndex == 0 ? "" : QrcodeValueSubstring(index.ProductCodeIndex, index.ProductCodeLength, qr);
                item.NextProcess1 = index.NextProcess1Index == 0 ? "" : QrcodeValueSubstring(index.NextProcess1Index, index.NextProcess1Length, qr);
                item.NextProcess2 = index.NextProcess2Index == 0 ? "" : QrcodeValueSubstring(index.NextProcess2Index, index.NextProcess2Length, qr);
                item.Location1 = index.Location1Index == 0 ? "" : QrcodeValueSubstring(index.Location1Index, index.Location1Length, qr);
                item.Location2 = index.Location2Index == 0 ? "" : QrcodeValueSubstring(index.Location2Index, index.Location2Length, qr);
                item.Packing = index.PackingIndex == 0 ? "" : QrcodeValueSubstring(index.PackingIndex, index.PackingLength, qr);
                item.ProductAbbreviation = index.ProductAbbreviationIndex == 0 ? "" : QrcodeValueSubstring(index.ProductAbbreviationIndex, index.ProductAbbreviationLength, qr);

                // 数値変換
                string quantityString = index.QuantityIndex == 0 ? "0" : QrcodeValueSubstring(index.QuantityIndex, index.QuantityLength, qr);
                string productLabelBranchNumberString = index.ProductLabelBranchNumberIndex == 0 ? "0" : QrcodeValueSubstring(index.ProductLabelBranchNumberIndex, index.ProductLabelBranchNumberLength, qr);
                if ((!int.TryParse(quantityString, out int quantity)) || (!int.TryParse(productLabelBranchNumberString, out int productLabelBranchNumber)))
                {
                    message = "数量：数値変換エラー";
                    return Task.FromResult((result, message, item));
                }
                else
                {
                    item.Quantity = quantity;
                    item.ProductLabelBranchNumber = productLabelBranchNumber;
                }

                // 日付変換
                if (index.DeleveryDateIndex == 0)
                {
                    item.DeleveryDate = "";
                }
                else
                {
                    string deleveryDateString = QrcodeValueSubstring(index.DeleveryDateIndex, index.DeleveryDateLength, qr);
                    if (!DateTime.TryParse(deleveryDateString.Substring(0, 4) + "/" + deleveryDateString.Substring(4, 2) + "/" + deleveryDateString.Substring(6, 2), out DateTime deleveryDate))
                    {
                        message = "納期：日付変換エラー";
                        return Task.FromResult((result, message, item));
                    }
                    else
                    {
                        item.DeleveryDate = deleveryDate.ToString("yyyy/MM/dd");
                    }
                }

            }

            // 識別文字チェックがOKでないあ、または品番か数量が認識できなければエラー
            if (!identifyOK || String.IsNullOrEmpty(item.ProductCode) || item.Quantity == 0)
            {
                message = Common.Const.SCAN_ERROR_INCORRECT_QR;
                return Task.FromResult((result, message, item));
            }

            result = true;
            return Task.FromResult((result, message, item));

        }

        /// <summary>
        /// 在庫入庫の制約項目
        /// </summary>
        public class M_StoreInConstraint
        {
            public string NextProcess1 { get; set; }
            public string NextProcess2 { get; set; }
        }

        /// <summary>
        /// 在庫入庫の制約チェック
        /// </summary>
        public static Task<(bool result, string message)> CheckStoreInConstraint(QrcodeItem qr, List<M_StoreInConstraint> warehouseInConstraintList)
        {
            bool result = false;
            string message = "";

            // 納入先と受入、どちらか空白ではないものを抽出
            var constraitList = warehouseInConstraintList.Where(x => (!string.IsNullOrEmpty(x.NextProcess1) || !string.IsNullOrEmpty(x.NextProcess2))).ToList();

            if (constraitList.Count > 0)
            {

                foreach (var c in constraitList)
                {
                    bool check = true;

                    var nextProcess1 = c.NextProcess1;
                    var nextProcess2 = c.NextProcess2;

                    // 在庫対象であるか
                    if (!string.IsNullOrEmpty(nextProcess1))
                    {
                        if (nextProcess1 != qr.NextProcess1)
                        {
                            check = false;
                        }
                    }

                    if (!string.IsNullOrEmpty(nextProcess2))
                    {
                        if (nextProcess2 != qr.NextProcess2)
                        {
                            check = false;
                        }
                    }

                    if (check)
                    {
                        result = true;
                        return Task.FromResult((result, message));
                    }

                }

                result = false;
                return Task.FromResult((result, message));

            }
            else
            {
                // 制約が存在しない
                result = true;
                return Task.FromResult((result, message));
            }

        }


    }
}
