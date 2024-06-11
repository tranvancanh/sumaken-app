using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using technoleight_THandy.common;
using static technoleight_THandy.Models.Login;

namespace technoleight_THandy.Driver
{
    public class APIClass
    {
        // タイムアウト時間の設定(単位：秒)
        private readonly int HttpClientTimeOut = 86400;

        public async Task<(HttpStatusCode status, string content)> GetMethod(string getApiUrl)
        {
            HttpClientHandler insecureHandler = null;
            HttpClient httpClient = null;
#if DEBUG
            insecureHandler = GetInsecureHandler();
            httpClient = new HttpClient(insecureHandler);
#else
    httpClient = new HttpClient();
#endif
            // POST送信
            //HttpResponseMessage response;

            try
            {
                // Set default request headers
                await httpClient.AddRequestHeaders();

                httpClient.Timeout = TimeSpan.FromSeconds(HttpClientTimeOut);
                using (var response = await httpClient.GetAsync(getApiUrl))
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return (response.StatusCode, responseContent);
                }
            }
            catch (Exception ex)
            {
                var w32ex = ex as Win32Exception;
                if (w32ex == null)
                {
                    w32ex = ex.InnerException as Win32Exception;
                }
                if (w32ex != null)
                {
                    int code = w32ex.ErrorCode;
                    if (code == 10060)
                    {
                        return (0, "接続先URLにアクセスできません");
                    }
                    else
                    {
                        throw;
                    }
                }

                throw;
            }
            finally
            {
                // Need to call dispose on the HttpClient and HttpClientHandler objects
                // when done using them, so the app doesn't leak resources
                insecureHandler?.Dispose();
                httpClient?.Dispose();
            }

        }

        public async Task<(HttpStatusCode status, string content)> PostMethod(string jsonPostData, string apiUrl, string controllerName, int companyID = 0)
        {
            string postUrl = "";
            if (companyID == 0)
            {
                postUrl = apiUrl + controllerName;
            }
            else
            {
                postUrl = Util.AddCompanyPath(apiUrl + controllerName, companyID);
            }
            HttpClientHandler insecureHandler = null;
            HttpClient httpClient = null;
#if DEBUG
            insecureHandler = GetInsecureHandler();
            httpClient = new HttpClient(insecureHandler);
#else
    httpClient = new HttpClient();
#endif

            var content = new StringContent(jsonPostData, Encoding.UTF8, @"application/json");

            var errdatas = new List<Dictionary<string, string>>();

            // POST送信
            //HttpResponseMessage response;
            try
            {
                // Set default request headers
                await httpClient.AddRequestHeaders();

                httpClient.Timeout = TimeSpan.FromSeconds(HttpClientTimeOut);
                using (var response = await httpClient.PostAsync(postUrl, content))
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return (response.StatusCode, responseContent);
                }
            }
            catch (Exception ex)
            {
                var w32ex = ex as Win32Exception;
                if (w32ex == null)
                {
                    w32ex = ex.InnerException as Win32Exception;
                }
                if (w32ex != null)
                {
                    int code = w32ex.ErrorCode;
                    if (code == 10060 || code == 10065)
                    {
                        return (0, "接続先URLにアクセスできません");
                    }
                    else
                    {
                        throw;
                    }
                }

                throw;
            }
            finally
            {
                // Need to call dispose on the HttpClient and HttpClientHandler objects
                // when done using them, so the app doesn't leak resources
                insecureHandler?.Dispose();
                httpClient?.Dispose();
            }
        }

        public static HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            //{
            //    if (cert.Issuer.Equals("CN=localhost"))
            //        return true;
            //    return errors == System.Net.Security.SslPolicyErrors.None;
            //};
            handler.ServerCertificateCustomValidationCallback += (o, c, ch, er) => true;
            return handler;
        }


    }


    public static class RequestHeaderExtensions
    {
        public static async Task AddRequestHeaders(this HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Clear();
            var userInfor = await GetuserInfor();
            if (userInfor != null)
            {
                httpClient.DefaultRequestHeaders.Accept.Remove(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var output = JsonConvert.SerializeObject(userInfor);
                // Convert string to Base64
                byte[] bytes = Encoding.UTF8.GetBytes(output);
                var base64String = System.Convert.ToBase64String(bytes);

                httpClient.DefaultRequestHeaders.Add("UserInfor", base64String);

            }
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        private static async Task<LoginUserSqlLite> GetuserInfor()
        {
            var loginUsers = await App.DataBase.GetLognAsync();
            return loginUsers.FirstOrDefault();
        }
    }

}


