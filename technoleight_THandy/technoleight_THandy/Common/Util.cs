using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using technoleight_THandy.Common;
using technoleight_THandy.Models;
using technoleight_THandy.Models.common;
using technoleight_THandy.ViewModels;
using technoleight_THandy.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
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

        public static async Task<bool> HandyPagePush(int pageId, INavigation navigation)
        {
            string pageName = "";

            // SQLiteよりページ情報を抽出
            List<MenuX> menux = await App.DataBase.GetMenuAsync();
            if (menux.Count > 0)
            {
                pageName = menux.Where(x => x.HandyPageID == pageId).FirstOrDefault().HandyPageName;
            }
            else
            {
                return false;
            }

            // 入庫メニュー
            if((pageId > 200 && pageId < 300) || (pageId > 400 && pageId < 500))
            {
                ScanReadViewModel.StoreInFlg = true;
                if (App.Setting.ScanMode == Const.C_SCANNAME_CAMERA)
                {
                    Page page = ScanReadPageCamera.GetInstance(pageName, pageId, navigation);
                    await navigation.PushAsync(page);
                }
                else if (App.Setting.ScanMode == Const.C_SCANNAME_CLIPBOARD)
                {
                    Page page = ScanReadPageClipBoard.GetInstance(pageName, pageId, navigation);
                    await navigation.PushAsync(page);
                }
                else
                {
                    Page page = ScanReadPageCamera.GetInstance(pageName, pageId, navigation);
                    await navigation.PushAsync(page);
                }
            }

            // 出庫メニュー
            else if (pageId > 300 && pageId < 400)
            {
                ScanReadViewModel.StoreInFlg = false;
                Page page = null;
                if (App.Setting.ScanMode == Const.C_SCANNAME_CAMERA)
                {

                }
                else if(App.Setting.ScanMode == Const.C_SCANNAME_CLIPBOARD)
                {
                    page = new ScanStoreOutPageClipBoard(pageName, pageId, navigation);
                 }
                else
                {

                }
                await navigation.PushAsync(page);
            }

            // AGFメニュー
            if ((pageId > 600 && pageId < 700))
            {
                ScanReadViewModel.StoreInFlg = false;
                if (App.Setting.ScanMode == Const.C_SCANNAME_CAMERA)
                {
                    Page page = ScanReadPageCamera.GetInstance(pageName, pageId, navigation);
                    //Page page = null;
                    //page = new ScanStoreOutPageClipBoard(pageName, pageId, navigation);
                    await navigation.PushAsync(page);
                }
                else if (App.Setting.ScanMode == Const.C_SCANNAME_CLIPBOARD)
                {
                    Page page = ScanReadAgfClipBoard.GetInstance(pageName, pageId, navigation);
                    await navigation.PushAsync(page);
                }
                else
                {
                    Page page = ScanReadPageCamera.GetInstance(pageName, pageId, navigation);
                    await navigation.PushAsync(page);
                }
            }

            return true;
        }

        public static bool NameTagQrcodeCheck(string scanString)
        {
                var scanStringArray = scanString.Split(':');

                if (scanStringArray.Length == 2 && !String.IsNullOrEmpty(scanStringArray[1]))
                {
                    if (App.Setting.HandyUserCode == scanStringArray[1])
                    {
                        return true;
                    }
                    else
                    {
                        throw new CustomExtention(Common.Const.SCAN_NAMETAG_ERROR_INCORRECT_STRING);
                    }
                }
                else
                {
                    throw new CustomExtention(Common.Const.SCAN_NAMETAG_ERROR);
                }
        }
    }
}
