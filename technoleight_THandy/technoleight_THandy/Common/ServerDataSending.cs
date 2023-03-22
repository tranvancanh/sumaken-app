using Android.Content.Res;
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

namespace technoleight_THandy.Common
{
    public class ServerDataSending
    {
        public static async Task<(bool,string)> ReceiveDataServerSendingExcute(List<ScanCommonApiPostRequestBody> receiveApiPostRequests)
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
                    string succsessMessage = "すべての登録が完了しました";

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

                        succsessMessage = "※登録済のためスキップしたデータがあります\n\n登録成功：" + receivePostBackBody.SuccessDataCount + "件" +
                        "\n登録済：" + receivePostBackBody.AlreadyRegisteredDataCount + "件" +
                        "\n\n登録済 一覧：" +
                        "\n\n" +
                        registeredDatasString;

                    }

                    return (true, succsessMessage);

                }
                else
                {
                    //await App.DisplayAlertError(responseMessage.content);
                    return (false, responseMessage.content);
                }

            }
            catch (Exception ex)
            {
                //await App.DisplayAlertError();
                return (false, null);
            }

        }

        public static async Task<(bool, string)> ReturnStoreAddressDataServerSendingExcute(List<ScanCommonApiPostRequestBody> scanCommonApiPostRequestBodies)
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
                    string succsessMessage = "すべての登録が完了しました";

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

                        succsessMessage = "※移動元のデータが存在しないためスキップしたデータがあります\n\n登録成功：" + returnStoreAddressPostBackBody.SuccessDataCount + "件" +
                        "\n移動元データ無：" + returnStoreAddressPostBackBody.StoreInNotFoundDataCount + "件" +
                        "\n\n移動元データ無 一覧：" +
                        "\n\n" +
                        registeredDatasString;

                    }

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



    }
}
