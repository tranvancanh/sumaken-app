using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace technoleight_THandy.Models
{
    public class ToyotaWgEdiCommonFormat
    {
        public static class StartIndexString
        {
            public const string SupplierCodeIndex = "6V";
            public const string SupplierCodeClassIndex = "11V";
            //public const string ShippingLocationIndex = "20L";
            public const string NextProcess1Index = "2L";
            public const string Location1Index = "1L";
            public const string CustomerCodeIndex = "7V";
            public const string DeliveryDateIndex = "16D";
            public const string DeliveryTimeClassIndex = "9D";
            public const string SlipNumber = "10K";
            public const string ProductAbbreviationIndex = "20P";
            public const string ProductCodeIndex = "P";
            public const string QuantityIndex = "Q";
            public const string ProductLabelBranchNumberIndex = "17K";
            public const string FreeAreaIndex = "Z";
        }

        public static string[] GetQrcode(string qrString)
        {
            // QR中身を制御コードで区切れるよう変換する http://nanoappli.com/blog/archives/4841

            string changeCode = "";
            string[] ctrlStr = {
                "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL",
                "BS",  "HT",  "LF",  "VT",  "NP",  "CR",  "SO",  "SI",
                "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB",
                "CAN", "EM",  "SUB", "ESC", "FS",  "GS",  "RS",  "US" };
            changeCode = Regex.Replace(qrString, @"\p{Cc}", str =>
            {
                int offset = str.Value[0];
                if (ctrlStr.Length > offset)
                {
                    return "[" + ctrlStr[offset] + "]";
                }
                else
                {
                    return string.Format("<{0:X2}>", (byte)str.Value[0]);
                }
            });

            // レコード区切り[RS]～[RS]の内容取得
            var rs = "[RS]";
            var rsIndex = changeCode.IndexOf("[RS]");
            string strValue = changeCode.Remove(0, rsIndex + 1 + rs.Length);
            //try
            //{
            //    strValue = strValue.Remove(strValue.LastIndexOf("E" + rs));
            //}
            //catch (Exception ex)
            //{

            //}
            

            // グループ区切り[GS]ごとで区切る
            var words = strValue.Split(new string[] { "[GS]" }, StringSplitOptions.RemoveEmptyEntries);
            words = words.Select(x => x = x.Trim()).ToArray();

            return words;
        }

        public static Qrcode.QrcodeItem GetControlQrcodeItems(string scanQrcode, Qrcode.QrcodeIndex index)
        {
            var items = new Qrcode.QrcodeItem();
            var qrArray = GetQrcode(scanQrcode);

            items.DeleveryDate = Qrcode.ChangeDateTimeString(GetTargetItem(qrArray, StartIndexString.DeliveryDateIndex), "納期");
            items.ProductCode = GetTargetItem(qrArray, StartIndexString.ProductCodeIndex);
            items.DeliveryTimeClass = GetTargetItem(qrArray, StartIndexString.DeliveryTimeClassIndex);
            items.ProductAbbreviation = GetTargetItem(qrArray, StartIndexString.ProductAbbreviationIndex);
            //items.ProductLabelBranchNumber = Qrcode.ChangeNumeric(GetTargetItem(qrArray, StartIndexString.ProductLabelBranchNumberIndex), "枝番");
            items.ProductLabelBranchNumber = Qrcode.ChangeNumeric(GetTargetItem(qrArray, StartIndexString.ProductLabelBranchNumberIndex), "枝番");
            items.SupplierCode = GetTargetItem(qrArray, StartIndexString.SupplierCodeIndex) + GetTargetItem(qrArray, StartIndexString.SupplierCodeClassIndex);
            items.Quantity = Qrcode.ChangeNumeric(GetTargetItem(qrArray, StartIndexString.QuantityIndex), "数量");
            items.Location1 = GetTargetItem(qrArray, StartIndexString.Location1Index);
            items.DeliverySlipNumber = GetTargetItem(qrArray, StartIndexString.SlipNumber);

            //var nextProcess1CompanyCodeItem = GetTargetItem(qrArray, StartIndexString.NextProcess1Index).Substring(0, 10).Trim();
            //var nextProcess1ClassCodeItem = GetTargetItem(qrArray, StartIndexString.NextProcess1Index).Substring(9).Trim();
            //items.NextProcess1 = nextProcess1CompanyCodeItem + nextProcess1ClassCodeItem;
            items.NextProcess1 = GetTargetItem(qrArray, StartIndexString.NextProcess1Index);

            //var customerCompanyCodeItem = GetTargetItem(qrArray, StartIndexString.NextProcess1Index).Substring(0, 10).Trim();
            //var customerClassCodeItem = GetTargetItem(qrArray, StartIndexString.NextProcess1Index).Substring(9).Trim();
            //items.CustomerCode = customerCompanyCodeItem + customerClassCodeItem;
            items.CustomerCode = GetTargetItem(qrArray, StartIndexString.CustomerCodeIndex);

            var freeItemString = GetTargetItem(qrArray, StartIndexString.FreeAreaIndex);

            int indexOf = scanQrcode.IndexOf(StartIndexString.FreeAreaIndex) + 1;
            string freeString = scanQrcode.Substring(indexOf);

            // フリーエリアでインデックス指定しているものは上書きする
            items.DeliverySlipNumber = index.DeliverySlipNumberIndex == 0 ? items.DeliverySlipNumber : Qrcode.QrcodeValueSubstring(index.DeliverySlipNumberIndex, index.DeliverySlipNumberLength, freeItemString);
            items.SupplierCode = index.SupplierCodeIndex == 0 ? items.SupplierCode : Qrcode.QrcodeValueSubstring(index.SupplierCodeIndex, index.SupplierCodeLength, freeString);
            items.ProductCode = index.ProductCodeIndex == 0 ? items.ProductCode : Qrcode.QrcodeValueSubstring(index.ProductCodeIndex, index.ProductCodeLength, freeString);
            items.Packing = index.PackingIndex == 0 ? items.Packing : Qrcode.QrcodeValueSubstring(index.PackingIndex, index.PackingLength, freeString);
            items.NextProcess1 = index.NextProcess1Index == 0 ? items.NextProcess1 : Qrcode.QrcodeValueSubstring(index.NextProcess1Index, index.NextProcess1Length, freeString);
            items.Location1 = index.Location1Index == 0 ? items.Location1 : Qrcode.QrcodeValueSubstring(index.Location1Index, index.Location1Length, freeString);

            return items;
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
