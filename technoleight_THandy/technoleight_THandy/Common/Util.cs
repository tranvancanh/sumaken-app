using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using technoleight_THandy.Common;
using technoleight_THandy.Models;
using Xamarin.Essentials;
using static System.Net.Mime.MediaTypeNames;

namespace technoleight_THandy.common
{
    public static class Util
    {
        // 共通関数
        // 参考：https://gist.github.com/Buravo46/49c34e77ff1a75177340

        public static Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            // ビルドアクションで埋め込みリソースにしたファイルを取ってくる
            var stream = assembly.GetManifestResourceStream(filename);
            return stream;
        }

        public static string AddCompanyPath(string url, int companyID)
        {
            string createUrl = url;

            var lastString = url.Substring(url.Length - 1);
            if (lastString != "/")
            {
                createUrl += "/";
            }

            createUrl += companyID.ToString();

            return createUrl;
        }

        // https://stackoverflow.com/questions/14517798/append-values-to-query-string
        public static string AddParameter(string url, string paramName, string paramValue)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[paramName] = paramValue;
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri.ToString();
        }

        public static async Task<(double latitude, double longitude)> GetLocationInformation()
        {
            // 緯度、経度
            double latitude = 0.0;
            double longitude = 0.0;

            try
            {
                // ネットワークが切れた状態で緯度経度を取得すると返ってこなくなり、
                // エラーをキャッチするが、長い時間反応がないので、事前に接続チェックする。
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    // GPSの精度指定、レスポンス取得の時間指定
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
                    // 経緯度の取得
                    var location = await Geolocation.GetLocationAsync(request);

                    latitude = location.Latitude;
                    longitude = location.Longitude;

                    return (latitude, longitude);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                latitude = 0.0;
                longitude = 0.0;

                return (latitude, longitude);
            }

        }

        /// <summary>
        /// 在庫入庫対象か
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public static bool StoreInFlag(int pageID)
        {
            if (pageID >= 200 && pageID <= 299)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static async Task<int> DeleteScanData()
        {
            await App.DataBase.DeleteAllScanReceiveSendData();
            await App.DataBase.DeleteAllScanReceive();
            return 1;
        }
    }
}
