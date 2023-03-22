using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using technoleight_THandy.common;
using System.Net;
using System.ComponentModel;

namespace technoleight_THandy.Driver
{
    public class APIClass
    {

        public async Task<(HttpStatusCode status, string content)> GetMethod(string getApiUrl)
        {

#if DEBUG
            HttpClientHandler insecureHandler = GetInsecureHandler();
            HttpClient httpClient = new HttpClient(insecureHandler);
#else
    HttpClient httpClient = new HttpClient();
#endif
            // POST送信
            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync(getApiUrl);
                string responseContent = response.Content.ReadAsStringAsync().Result;
                return (response.StatusCode, responseContent);
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

#if DEBUG
            HttpClientHandler insecureHandler = GetInsecureHandler();
            HttpClient httpClient = new HttpClient(insecureHandler);
#else
    HttpClient httpClient = new HttpClient();
#endif
            var content = new StringContent(jsonPostData, Encoding.UTF8, @"application/json");

            var errdatas = new List<Dictionary<string, string>>();

            // POST送信
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(postUrl, content);
                string responseContent = response.Content.ReadAsStringAsync().Result;
                return (response.StatusCode, responseContent);
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

        }

        public HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }


    }

}


