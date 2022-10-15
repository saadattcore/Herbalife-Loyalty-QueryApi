using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.QueryAPI.Test
{
    public class Common
    {
        public static string ConnectionString {

            get {
                return ConfigurationManager.AppSettings["LoyaltyDBConnectionString"];

            }
        }


        public static T SendRequest<T>(string operation)
        {

            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Host = null;
            client.DefaultRequestHeaders.AcceptEncoding.Clear();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

            var url = ConfigurationManager.AppSettings["UrlTestEnvironment"];
            url += operation;

            var response = GetContentAsync(client, url);
            T result = JsonConvert.DeserializeObject<T>(response.Result);
            return result;
        }


        private static async Task<string> GetContentAsync(HttpClient httpClient, string uri)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            HttpResponseMessage response = httpClient.GetAsync(uri).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                return result; // HttpUtility.HtmlEncode(result);
            }
            else
                throw new Exception(response.ReasonPhrase);
        }



    }
}
