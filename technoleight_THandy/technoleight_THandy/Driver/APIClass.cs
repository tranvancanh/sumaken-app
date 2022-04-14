using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using THandy.Models;
using System.Text;
using Web_PRS.API;
using RestSharp;
using Xamarin.Essentials;
using System.Linq;
using Xamarin.Forms.Internals;

namespace THandy.Driver
{
    public class APIClass
    {

        //public List<Users> articleList;

        // AWS APIのURl
        public string API_URL = "https://www.tozan.co.jp/thandyweb/api/";
        //public string API_URL = "https://www.tozan.co.jp/thandyweb_1/api/";

        //
        // データを取得するメソッド
        //public async Task<List<Users>> AsyncGetAPIData()
        //{
        //    // Listの作成
        //    articleList = new List<Users>();
        //    // HttpClientの作成 
        //    HttpClient httpClient = new HttpClient();
        //    // 非同期でAPIからデータを取得
        //    Task<string> stringAsync = httpClient.GetStringAsync(API_URL);
        //    string result = await stringAsync;
        //    // JSON形式のデータをデシリアライズ
        //    articleList = JsonConvert.DeserializeObject<List<Users>>(result);
        //    // List でデータを返す
        //    return articleList;
        //}

        //Jsonデータ送信・受信
        public async Task<List<Dictionary<string, string>>> Post_method(List<Dictionary<string, string>> post_data, string shorimei)
        {
            //機能：Jsonデータ送信、受信
            //引数：送信データ（配列）、送信先URL
            //使い方：JsonConvert.DeserializeObject<Person>(task.Result)

            //APIでRDSからデータ取得
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            string urlx = "";

            if (Set2.Count > 0)
            {
                urlx = Set2[0].url;
            } else
            {
                urlx = API_URL;
            }

            string url = urlx + shorimei;
            
            //DictionaryをJsonData（String）に変換
            var JsonDataSend = JsonConvert.SerializeObject(post_data);

            //リクエストの送信データ作成Body
            var httpClient = new HttpClient();
            var content = new StringContent(JsonDataSend, Encoding.UTF8, @"application/json");

            //ヘッダーにAPIキー設定
            //content.Headers.Add("x-api-key", "Osy5yUPMxg1oCDSJhhG9XaidBHQTYEwh7eJzTmbd");

            //ネットワークが有効かどうか
            //var current = Connectivity.NetworkAccess;

            //System.Diagnostics.Debug.WriteLine("Connectivity.NetworkAccess: {0}", current);
            //if (current == NetworkAccess.Unknown)
            //{
            //    // Connection to internet is available
            //}

            //POST送信
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(url, content);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("network err", e.Message);
                var errdatas = new List<Dictionary<string, string>>();
                var errdata = new Dictionary<string, string>();
                errdata.Add(Common.Const.C_ERR_KEY_NETWORK, Common.Const.C_ERR_VALUE_NETWORK);
                errdatas.Add(errdata);
                return errdatas;
            }


            //ボディーを受け取る
            var result = response.Content.ReadAsStringAsync();
            await result;
            List<Dictionary<string, string>> ReString = new List<Dictionary<string, string>>();
            var JsonData = JsonConvert.SerializeObject(result.Result);
            try
            {
                ReString = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result.Result);
            }
            catch (Exception e)
            {
                var errdata = new Dictionary<string, string>();
                errdata.Add("err", "server err: " + e.Message);
                ReString.Add(errdata);
            }


            //return result.Result;

            //Json形式でreturn
            //// シリアライズ時に指定
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

            //// デシリアライズ時に指定
            //UserModel model = JsonConvert.DeserializeObject<UserModel>(jsonstring, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });

            //if (post_data["code"] != "UPDATE")
            //{

            //}
            //if (post_data["code"] != "UPDATE")
            //{
            //    var JsonData = JsonConvert.SerializeObject(result.Result);
            //    var ReString = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result.Result);
            //    return ReString;

            //}
            return ReString;
        }

        public async Task<List<Dictionary<string, string>>> Post_method2(List<Dictionary<string, string>> post_data, string shorimei, string url1)
        {
            //機能：Jsonデータ送信、受信
            //引数：送信データ（配列）、送信先URL
            //使い方：JsonConvert.DeserializeObject<Person>(task.Result)

            //APIでRDSからデータ取得
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            string urlx = url1;

            string url = urlx + shorimei;

            //DictionaryをJsonData（String）に変換
            var JsonDataSend = JsonConvert.SerializeObject(post_data);

            //リクエストの送信データ作成Body
            var httpClient = new HttpClient();
            var content = new StringContent(JsonDataSend, Encoding.UTF8, @"application/json");

            //ヘッダーにAPIキー設定
            //content.Headers.Add("x-api-key", "Osy5yUPMxg1oCDSJhhG9XaidBHQTYEwh7eJzTmbd");

            //ネットワークが有効かどうか
            //var current = Connectivity.NetworkAccess;

            //System.Diagnostics.Debug.WriteLine("Connectivity.NetworkAccess: {0}", current);
            //if (current == NetworkAccess.Unknown)
            //{
            //    // Connection to internet is available
            //}

            //POST送信
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(url, content);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("network err", e.Message);
                var errdatas = new List<Dictionary<string, string>>();
                var errdata = new Dictionary<string, string>();
                errdata.Add(Common.Const.C_ERR_KEY_NETWORK, Common.Const.C_ERR_VALUE_NETWORK);
                errdatas.Add(errdata);
                return errdatas;
            }

            //ボディーを受け取る
            var result = response.Content.ReadAsStringAsync();
            await result;
            List<Dictionary<string, string>> ReString = new List<Dictionary<string, string>>();
            var JsonData = JsonConvert.SerializeObject(result.Result);
            try
            {
                ReString = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result.Result);
            }
            catch (Exception e)
            {
                var errdata = new Dictionary<string, string>();
                errdata.Add("err", "server err: " + e.Message);
                ReString.Add(errdata);
            }


            //return result.Result;

            //Json形式でreturn
            //// シリアライズ時に指定
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

            //// デシリアライズ時に指定
            //UserModel model = JsonConvert.DeserializeObject<UserModel>(jsonstring, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });

            //if (post_data["code"] != "UPDATE")
            //{

            //}
            //if (post_data["code"] != "UPDATE")
            //{
            //    var JsonData = JsonConvert.SerializeObject(result.Result);
            //    var ReString = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result.Result);
            //    return ReString;

            //}
            return ReString;
        }

        //Jsonデータ送信・受信
        // パラメータpostListをModelにしたVer.
        public async Task<List<Dictionary<string, string>>> Post_method3(List<BarModel> postList, string shorimei)
        {
            //機能：Jsonデータ送信、受信
            //引数：送信データ（配列）、送信先URL
            //使い方：JsonConvert.DeserializeObject<Person>(task.Result)

            //APIでRDSからデータ取得
            List<Setei> Set2 = await App.DataBase.GetSeteiAsync();
            string urlx = "";

            if (Set2.Count > 0)
            {
                urlx = Set2[0].url;
            }
            else
            {
                urlx = API_URL;
            }

            string url = urlx + shorimei;

            //DictionaryをJsonData（String）に変換
            var JsonDataSend = JsonConvert.SerializeObject(postList);

            //リクエストの送信データ作成Body
            var httpClient = new HttpClient();
            var content = new StringContent(JsonDataSend, Encoding.UTF8, @"application/json");

            //POST送信
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(url, content);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("network err", e.Message);
                var errdatas = new List<Dictionary<string, string>>();
                var errdata = new Dictionary<string, string>();
                errdata.Add(Common.Const.C_ERR_KEY_NETWORK, Common.Const.C_ERR_VALUE_NETWORK);
                errdatas.Add(errdata);
                return errdatas;
            }


            //ボディーを受け取る
            var result = response.Content.ReadAsStringAsync();
            await result;
            List<Dictionary<string, string>> ReString = new List<Dictionary<string, string>>();
            var JsonData = JsonConvert.SerializeObject(result.Result);
            try
            {
                ReString = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result.Result);
            }
            catch (Exception e)
            {
                var errdata = new Dictionary<string, string>();
                errdata.Add("err", "server err: " + e.Message);
                ReString.Add(errdata);
            }


            //return result.Result;

            //Json形式でreturn
            //// シリアライズ時に指定
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

            //// デシリアライズ時に指定
            //UserModel model = JsonConvert.DeserializeObject<UserModel>(jsonstring, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });

            //if (post_data["code"] != "UPDATE")
            //{

            //}
            //if (post_data["code"] != "UPDATE")
            //{
            //    var JsonData = JsonConvert.SerializeObject(result.Result);
            //    var ReString = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result.Result);
            //    return ReString;

            //}
            return ReString;
        }


    }

}


