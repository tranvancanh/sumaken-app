using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using technoleight_THandy.Common;
using technoleight_THandy.Models;
using technoleight_THandy.Models.common;
using technoleight_THandy.ViewModels;
using technoleight_THandy.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

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

        public static async Task<Location> GetCurrentLocation()
        {
            var location = new Location();
            var getCurrentLacationFlg = false;
            // Check For Permission
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            try
            {
                if (status == PermissionStatus.Granted)
                {
                    //status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    // We have permission, let's get current Location
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                    getCurrentLacationFlg = true;
                }
                else
                {
                    throw new PermissionException("No Permission");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Debug.WriteLine("Not Supported:" + fnsEx.Message);
                getCurrentLacationFlg = false;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Debug.WriteLine("Not Enabled:" + fneEx.Message);
                getCurrentLacationFlg = false;
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Debug.WriteLine("No Permission:" + pEx.Message);
                getCurrentLacationFlg = false;
            }
            catch (Exception ex)
            {
                // Unable to get location
                Debug.WriteLine("Grr Error:" + ex.Message);
                getCurrentLacationFlg = false;
            }
            if(!getCurrentLacationFlg)
            {
                location = await Geolocation.GetLastKnownLocationAsync();
            }
            return location;
        }

        public static async Task<Location> GetCurrentLocationWithTimes(int millisecondsDelay = 3000)
        {
            var lastLocationFlg = false;
            var location = new Location();
            // Check For Permission
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            try
            {
                if (status == PermissionStatus.Granted)
                {
                    //status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    // We have permission, let's get current Location
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    // Create CancellationTokenSource to manage cancellations
                    using (var cancellationTokenSourceCurrentLocation = new CancellationTokenSource())
                    {
                        // Initialize a CancellationToken from CancellationTokenSource
                        var cancellationTokenCurrentLocation = cancellationTokenSourceCurrentLocation.Token;

                        // Start a task to process work
                        var getCurrentLocationTask = Geolocation.GetLocationAsync(request, cancellationTokenCurrentLocation);

                        using (var cancellationTokenSourceDelayTask = new CancellationTokenSource())
                        {
                            // Initialize a CancellationToken from CancellationTokenSource
                            var cancellationTokenDelayTask = cancellationTokenSourceDelayTask.Token;
                            // Wait for milliseconds Delay
                            var delayTask = Task.Delay(millisecondsDelay, cancellationTokenDelayTask);

                            var completedTask = await Task.WhenAny(getCurrentLocationTask, delayTask);
                            // Wait until the task is completed or milliseconds, whichever comes first
                            if (completedTask == delayTask)
                            {
                                // If it runs for more than milliseconds, cancel task get current loaction
                                cancellationTokenSourceCurrentLocation.Cancel();
                                lastLocationFlg = true;
                            }
                            else if (completedTask == getCurrentLocationTask)
                            {
                                cancellationTokenSourceDelayTask.Cancel();
                                lastLocationFlg = false;
                                location = await getCurrentLocationTask;
                            }
                            else
                            {
                                cancellationTokenSourceCurrentLocation.Cancel();
                                lastLocationFlg = true;
                            }
                        }
                    }
                }
                else
                {
                    throw new PermissionException("No Permission");
                }
            }
            catch (Exception)
            {
                lastLocationFlg = true;
            }

            if (lastLocationFlg)
                location = await Geolocation.GetLastKnownLocationAsync();

            return location;
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
            await App.DataBase.DeleteALLAGFShukaKanbanDataAsync(); //AGF出荷かんばん
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
                Page page = null;
                if (App.Setting.ScanMode == Const.C_SCANNAME_CAMERA)
                {
                    //page = new ScanReadAgfClipBoard(pageName, pageId, navigation);
                }
                else if (App.Setting.ScanMode == Const.C_SCANNAME_CLIPBOARD)
                {
                    page = new ScanReadAgfClipBoard(pageName, pageId, navigation);
                }
                else
                {
                    //page = new ScanReadAgfClipBoard(pageName, pageId, navigation);
                }
                await navigation.PushAsync(page);
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
