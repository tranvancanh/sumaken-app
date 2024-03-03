using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static technoleight_THandy.Models.Receive;
using System.Threading.Tasks;
using technoleight_THandy.common;
using technoleight_THandy.Views;
using Xamarin.Forms;
using static technoleight_THandy.Models.ReturnStoreAddress;
using static technoleight_THandy.Models.ScanCommon;
using static technoleight_THandy.Common.Enums;
using static technoleight_THandy.Models.ReceiveDeleteModel;
using static technoleight_THandy.Models.ReturnAgfLuggageStation;

namespace technoleight_THandy.Common
{
    public class ServerDataSending
    {

        public static async Task<(ProcessResultPattern result, string message)> ReceiveDataServerSendingExcute(List<ScanCommonApiPostRequestBody> receiveApiPostRequests)
        {
            // 位置情報をセット
            var location = await Util.GetLocationInformation();
            foreach (var item in receiveApiPostRequests)
            {
                item.Latitude = location.latitude;
                item.Longitude = location.longitude;
            }

            try
            {
                var jsonSendData = JsonConvert.SerializeObject(receiveApiPostRequests);
                // SQLserver登録
                var responseMessage = await App.API.PostMethod(jsonSendData,
                    App.Setting.HandyApiUrl, "Receive", App.Setting.CompanyID);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    ReceivePostBackBody receivePostBackBody = JsonConvert.DeserializeObject<ReceivePostBackBody>(responseMessage.content);

                    if (receivePostBackBody.AlreadyRegisteredDataCount > 0)
                    {
                        string registeredDatasString = "";
                        StringBuilder stringBuilder = new StringBuilder("");
                        foreach (var item in receivePostBackBody.AlreadyRegisteredDatas)
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("\n\n");
                            }
                            stringBuilder.Append("[品　番]");
                            stringBuilder.Append(item.ProductCode);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[数　量]");
                            stringBuilder.Append(item.Quantity);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[仕入先]");
                            stringBuilder.Append(item.SupplierCode);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[枝　番]");
                            stringBuilder.Append(item.ProductLabelBranchNumber);
                        }
                        registeredDatasString = stringBuilder.ToString();

                        var alertMessage = "※登録済のためスキップしたデータがあります\n\n登録成功：" + receivePostBackBody.SuccessDataCount + "件" +
                        "\n登録済：" + receivePostBackBody.AlreadyRegisteredDataCount + "件" +
                        "\n\n登録済 一覧：" +
                        "\n\n" +
                        registeredDatasString;

                        return (ProcessResultPattern.Alert, alertMessage);
                    }
                    else
                    {
                        string succsessMessage = "すべての登録が完了しました";
                        return (ProcessResultPattern.Okey, succsessMessage);
                    }

                }
                else
                {
                    //await App.DisplayAlertError(responseMessage.content);
                    return (ProcessResultPattern.Error, responseMessage.content);
                }

            }
            catch (Exception ex)
            {
                //await App.DisplayAlertError();
                return (ProcessResultPattern.Error, null);
            }

        }

        public static async Task<(ProcessResultPattern result, string message)> ReturnStoreAddressDataServerSendingExcute(List<ScanCommonApiPostRequestBody> scanCommonApiPostRequestBodies)
        {
            // 位置情報をセット
            var location = await Util.GetLocationInformation();
            foreach (var item in scanCommonApiPostRequestBodies)
            {
                item.Latitude = location.latitude;
                item.Longitude = location.longitude;
            }

            try
            {
                var jsonSendData = JsonConvert.SerializeObject(scanCommonApiPostRequestBodies);
                // SQLserver登録
                var responseMessage = await App.API.PostMethod(jsonSendData,
                    App.Setting.HandyApiUrl, "ReturnStoreAddress", App.Setting.CompanyID);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    ReturnStoreAddressPostBackBody returnStoreAddressPostBackBody = JsonConvert.DeserializeObject<ReturnStoreAddressPostBackBody>(responseMessage.content);

                    if (returnStoreAddressPostBackBody.StoreInNotFoundDataCount > 0)
                    {
                        string registeredDatasString = "";
                        StringBuilder stringBuilder = new StringBuilder("");
                        foreach (var item in returnStoreAddressPostBackBody.StoreInNotFoundDatas)
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("\n\n");
                            }
                            stringBuilder.Append("[品　番]");
                            stringBuilder.Append(item.ProductCode);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[数　量]");
                            stringBuilder.Append(item.Quantity);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[仕入先]");
                            stringBuilder.Append(item.SupplierCode);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[枝　番]");
                            stringBuilder.Append(item.ProductLabelBranchNumber);
                        }
                        registeredDatasString = stringBuilder.ToString();

                        string alertMessage = "※移動元のデータが存在しないためスキップしたデータがあります\n\n登録成功：" + returnStoreAddressPostBackBody.SuccessDataCount + "件" +
                        "\n移動元データ無：" + returnStoreAddressPostBackBody.StoreInNotFoundDataCount + "件" +
                        "\n\n移動元データ無 一覧：" +
                        "\n\n" +
                        registeredDatasString;

                        return (ProcessResultPattern.Alert, alertMessage);
                    }
                    else
                    {
                        string succsessMessage = "すべての登録が完了しました";
                        return (ProcessResultPattern.Okey, succsessMessage);
                    }

                }
                else
                {
                    return (ProcessResultPattern.Error, responseMessage.content);
                }

            }
            catch (Exception ex)
            {
                return (ProcessResultPattern.Error, null);
            }

        }

        public static async Task<(bool result, string message)> ReceiveServerDataDelete(ReceiveDeleteRequestBody receiveDeleteRequestBody)
        {
            try
            {
                var jsonSendData = JsonConvert.SerializeObject(receiveDeleteRequestBody);
                // SQLserver登録
                var responseMessage = await App.API.PostMethod(jsonSendData,
                    App.Setting.HandyApiUrl, "ReceiveDelete", App.Setting.CompanyID);
                if (responseMessage.status == System.Net.HttpStatusCode.Created)
                {
                    ReceiveDeletePostBackBody receivePostBackBody = JsonConvert.DeserializeObject<ReceiveDeletePostBackBody>(responseMessage.content);
                    string succsessMessage = "入荷・入庫データの削除が完了しました";
                    return (true, succsessMessage);
                }
                else
                {
                    return (false, responseMessage.content);
                }

            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }

        public static async Task<(ProcessResultPattern result, string message)> ShipmentStoreOutDataServerSendingExcute(List<ScanCommonApiPostRequestBody> scanCommonApiPostRequestBodies)
        {
            // 位置情報をセット
            var location = await Util.GetLocationInformation();
            foreach (var item in scanCommonApiPostRequestBodies)
            {
                item.Latitude = location.latitude;
                item.Longitude = location.longitude;
            }

            try
            {
                var jsonSendData = JsonConvert.SerializeObject(scanCommonApiPostRequestBodies);
                // SQLserver登録
                var responseMessage = await App.API.PostMethod(jsonSendData,
                    App.Setting.HandyApiUrl, "Shipment", App.Setting.CompanyID);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    ReceivePostBackBody returnStoreAddressPostBackBody = JsonConvert.DeserializeObject<ReceivePostBackBody>(responseMessage.content);

                    if (returnStoreAddressPostBackBody.AlreadyRegisteredDataCount > 0)
                    {
                        string registeredDatasString = "";
                        StringBuilder stringBuilder = new StringBuilder("");
                        foreach (var item in returnStoreAddressPostBackBody.AlreadyRegisteredDatas)
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("\n\n");
                            }
                            stringBuilder.Append("[品　　番]");
                            stringBuilder.Append(item.ProductCode);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[入　　数]");
                            stringBuilder.Append(item.Quantity);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[番　　地]");
                            stringBuilder.Append(item.ScanStoreAddress2);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[出荷枝番]");
                            stringBuilder.Append(item.ProductLabelBranchNumber);
                        }
                        registeredDatasString = stringBuilder.ToString();
                        var alertMessage = "※登録済のためスキップしたデータがあります\n\n登録成功：" + returnStoreAddressPostBackBody.SuccessDataCount + "件" +
                        "\n登録済：" + returnStoreAddressPostBackBody.AlreadyRegisteredDataCount + "件" +
                        "\n\n登録済 一覧：" +
                        "\n\n" +
                        registeredDatasString;

                        return (ProcessResultPattern.Alert, alertMessage);
                    }
                    else
                    {
                        string succsessMessage = "すべての登録が完了しました";
                        return (ProcessResultPattern.Okey, succsessMessage);
                    }

                }
                else
                {
                    return (ProcessResultPattern.Error, responseMessage.content);
                }

            }
            catch (Exception ex)
            {
                return (ProcessResultPattern.Error, null);
            }

        }

        public static async Task<(ProcessResultPattern result, string message)> ReturnAgfLuggageStationDataServerSendingExcute(List<ScanCommonApiPostRequestBody> scanCommonApiPostRequestBodies)
        {
            // 位置情報をセット
            var location = await Util.GetLocationInformation();
            foreach (var item in scanCommonApiPostRequestBodies)
            {
                item.Latitude = location.latitude;
                item.Longitude = location.longitude;
            }

            try
            {
                var jsonSendData = JsonConvert.SerializeObject(scanCommonApiPostRequestBodies);
                // SQLserverチェックを行う
                var responseMessage = await App.API.PostMethod(jsonSendData,
                    App.Setting.HandyApiUrl, "AgfLuggageStation", App.Setting.CompanyID);
                if (responseMessage.status == System.Net.HttpStatusCode.OK)
                {
                    ReturnAgfLuggageStationPostBackBody returnAgfLuggageStationPostBackBody = JsonConvert.DeserializeObject<ReturnAgfLuggageStationPostBackBody>(responseMessage.content);

                    if (returnAgfLuggageStationPostBackBody.AgfLuggageStationNotFoundDataCount > 0)
                    {
                        string registeredDatasString = "";
                        StringBuilder stringBuilder = new StringBuilder("");
                        foreach (var item in returnAgfLuggageStationPostBackBody.AgfLuggageStationNotFoundDatas)
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("\n\n");
                            }
                            stringBuilder.Append("[品　番]");
                            stringBuilder.Append(item.ProductCode);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[数　量]");
                            stringBuilder.Append(item.Quantity);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[仕入先]");
                            stringBuilder.Append(item.SupplierCode);
                            stringBuilder.Append("\n");
                            stringBuilder.Append("[枝　番]");
                            stringBuilder.Append(item.ProductLabelBranchNumber);
                        }
                        registeredDatasString = stringBuilder.ToString();

                        string alertMessage = "※移動元のデータが存在しないためスキップしたデータがあります\n\n登録成功：" + returnAgfLuggageStationPostBackBody.SuccessDataCount + "件" +
                        "\n移動元データ無：" + returnAgfLuggageStationPostBackBody.AgfLuggageStationNotFoundDataCount + "件" +
                        "\n\n移動元データ無 一覧：" +
                        "\n\n" +
                        registeredDatasString;

                        return (ProcessResultPattern.Alert, alertMessage);
                    }
                    else
                    {
                        string succsessMessage = "すべての登録が完了しました";
                        return (ProcessResultPattern.Okey, succsessMessage);
                    }

                }
                else
                {
                    return (ProcessResultPattern.Error, responseMessage.content);
                }

            }
            catch (Exception ex)
            {
                return (ProcessResultPattern.Error, null);
            }

        }
    }
}
