using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using technoleight_THandy.Models.common;
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
            /// 発注元コード
            /// </summary>
            public string CustomerCode { get; set; }
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
            /// 出荷枝番（出荷画面用）
            /// </summary>
            public int ProductLabelBranchNumber2 { get; set; }
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

            /// <summary>
            /// IdentifyString
            /// </summary>
            public string IdentifyString { get; set; }

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
            public string IdentifyString { get; set; }
            public int IdentifyIndex { get; set; }

            public string IdentifySecondString { get; set; }
            public int IdentifySecondIndex { get; set; }

            public int MaxStringLength { get; set; }

            public bool ToyotaEdiWgFlag { get; set; }

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
            public int ProductCodeLength { get; set; }
            public int ProductAbbreviationLength { get; set; }
            public int ProductLabelBranchNumberLength { get; set; }
            public int QuantityLength { get; set; }
            public int NextProcess1Length { get; set; }
            public int Location1Length { get; set; }
            public int NextProcess2Length { get; set; }
            public int Location2Length { get; set; }
            public int PackingLength { get; set; }
            public bool ForShipmentFlag { get; set; }
        }

        public static string GetValueFromQRCode(int index, int stringLength, string qrStrng)
        {
            var value = qrStrng.Substring(index, stringLength).Replace(" ", "");
            return value;
        }

        public static string QrcodeValueSubstring(int index, int stringLength, string qr)
        {
            var value = qr.Substring(index - 1, stringLength).Replace(" ", "");
            return value;
        }
        public static int ChangeNumeric(string numericString, string errorItemName)
        {
            string errorMessage = errorItemName + "：数値変換エラー";
            bool isNumber = int.TryParse(numericString, out int numeric);
            if (!isNumber)
            {
                throw new CustomExtention(errorMessage);
            }
            return numeric;
        }
        public static string ChangeDateTimeString(string dateString, string errorItemName)
        {
            string errorMessage = errorItemName + "：日付変換エラー";
            bool isNumber = int.TryParse(dateString, out int orderDateValue);
            if (!isNumber)
            {
                throw new CustomExtention(errorMessage);
            }

            var year = "";
            var month = "";
            var day = "";
            if (dateString.Length == 6) // 数字[6桁]の日付の場合
            {
                year = "20" + dateString.Substring(0, 2);
                month = dateString.Substring(2, 2);
                day = dateString.Substring(4, 2);
            }
            else if (dateString.Length == 8) // 数字[8桁]の日付の場合
            {
                year = dateString.Substring(0, 4);
                month = dateString.Substring(4, 2);
                day = dateString.Substring(6, 2);
            }
            else
            {
                throw new CustomExtention(errorMessage);
            }

            if (DateTime.TryParse(year + "/" + month + "/" + day, out DateTime orderDate))
            {
                return orderDate.ToString("yyyy/MM/dd");
            }
            else
            {
                throw new CustomExtention(errorMessage);
            }
        }

        public static QrcodeItem GetQrcodeItem(string qr, List<QrcodeIndex> indexList)
        {
            QrcodeItem item = new QrcodeItem();
            QrcodeIndex index = new QrcodeIndex();

            try
            {
                // 識別文字が一致しているものが存在するか
                var qrIdentifyMatchStringList = indexList.Where(x => x.IdentifyString == QrcodeValueSubstring(x.IdentifyIndex, x.IdentifyString.Length, qr)).ToList();
                if (qrIdentifyMatchStringList == null || qrIdentifyMatchStringList.Count() == 0)
                {
                    throw new CustomExtention("QR内容を認識できません");
                }
                else if (qrIdentifyMatchStringList.Count() == 1)
                {
                    index = qrIdentifyMatchStringList.FirstOrDefault();
                }
                else
                {
                    // 識別文字が一致しているものが2つ以上ある場合は、識別文字2を参照する
                    try
                    {
                        var qrIdentifySecondMatchStringList = qrIdentifyMatchStringList.Where(x => x.IdentifySecondString == QrcodeValueSubstring(x.IdentifySecondIndex, x.IdentifySecondString.Length, qr)).ToList();
                        if (qrIdentifySecondMatchStringList == null || qrIdentifySecondMatchStringList.Count() == 0)
                        {
                            index = qrIdentifyMatchStringList.FirstOrDefault();
                        }
                        else
                        {
                            index = qrIdentifySecondMatchStringList.FirstOrDefault();
                        }
                    }
                    catch(Exception)
                    {

                    }
                   
                }

                var identifyStringLength = index.IdentifyString.Length;

                if (index.ToyotaEdiWgFlag)
                {
                    var toyotaEdiWgCommonItems = ToyotaWgEdiCommonFormat.GetControlQrcodeItems(qr, index);
                    item = toyotaEdiWgCommonItems;
                }
                else
                {
                    // QRから各項目をセットしていく
                    item.DeliveryTimeClass = index.DeliveryTimeClassIndex == 0 ? "" : QrcodeValueSubstring(index.DeliveryTimeClassIndex, index.DeliveryTimeClassLength, qr);
                    item.DeliverySlipNumber = index.DeliverySlipNumberIndex == 0 ? "" : QrcodeValueSubstring(index.DeliverySlipNumberIndex, index.DeliverySlipNumberLength, qr);
                    item.DataClass = index.DataClassIndex == 0 ? "" : QrcodeValueSubstring(index.DataClassIndex, index.DataClassLength, qr);
                    item.OrderClass = index.OrderClassIndex == 0 ? "" : QrcodeValueSubstring(index.OrderClassIndex, index.OrderClassLength, qr);
                    item.SupplierCode = index.SupplierCodeIndex == 0 ? "" : QrcodeValueSubstring(index.SupplierCodeIndex, index.SupplierCodeLength, qr);
                    item.ProductCode = index.ProductCodeIndex == 0 ? "" : QrcodeValueSubstring(index.ProductCodeIndex, index.ProductCodeLength, qr);
                    item.NextProcess1 = index.NextProcess1Index == 0 ? "" : QrcodeValueSubstring(index.NextProcess1Index, index.NextProcess1Length, qr);
                    item.NextProcess2 = index.NextProcess2Index == 0 ? "" : QrcodeValueSubstring(index.NextProcess2Index, index.NextProcess2Length, qr);
                    item.Location1 = index.Location1Index == 0 ? "" : QrcodeValueSubstring(index.Location1Index, index.Location1Length, qr);
                    item.Location2 = index.Location2Index == 0 ? "" : QrcodeValueSubstring(index.Location2Index, index.Location2Length, qr);
                    item.Packing = index.PackingIndex == 0 ? "" : QrcodeValueSubstring(index.PackingIndex, index.PackingLength, qr);
                    item.ProductAbbreviation = index.ProductAbbreviationIndex == 0 ? "" : QrcodeValueSubstring(index.ProductAbbreviationIndex, index.ProductAbbreviationLength, qr);
                    //item.ProductLabelBranchNumber = index.ProductLabelBranchNumberIndex == 0 ? 0 : QrcodeValueSubstring(index.ProductLabelBranchNumberIndex, index.ProductLabelBranchNumberLength, qr);

                    // 数値変換
                    item.Quantity = index.QuantityIndex == 0 ? 0 : ChangeNumeric(QrcodeValueSubstring(index.QuantityIndex, index.QuantityLength, qr), "数量");
                    item.ProductLabelBranchNumber = index.ProductLabelBranchNumberIndex == 0 ? 0 : ChangeNumeric(QrcodeValueSubstring(index.ProductLabelBranchNumberIndex, index.ProductLabelBranchNumberLength, qr), "枝番");

                    // 日付変換
                    item.DeleveryDate = index.DeleveryDateIndex == 0 ? "" : ChangeDateTimeString(QrcodeValueSubstring(index.DeleveryDateIndex, index.DeleveryDateLength, qr), "納期");
                }

                // 品番が認識できなければエラー
                if (String.IsNullOrEmpty(item.ProductCode))
                {
                    throw new CustomExtention("品番の取得に失敗しました");
                }

                // 数量が認識できなければエラー
                if (item.Quantity == 0)
                {
                    throw new CustomExtention("数量の取得に失敗しました");
                }
                item.IdentifyString = index.IdentifyString;
                return item;
            }
            catch (Exception ex)
            {
                throw;
            }

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
        public static string GetTargetItem(string[] qrArray, string indexString)
        {
            var targetItem = "";
            targetItem = qrArray.Where(x => x.StartsWith(indexString)).FirstOrDefault();

            // 判断文字と空白をカットする
            targetItem = targetItem.Replace(indexString, "").Replace(" ", "");

            return targetItem;
        }


    }
}
