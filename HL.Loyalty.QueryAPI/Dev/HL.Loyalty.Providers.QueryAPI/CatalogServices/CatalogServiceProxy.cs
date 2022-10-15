using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace HL.Loyalty.Providers.QueryAPI.CatalogServices
{
    public class CatalogServiceProxy
    {

        public CatalogServiceResponse SendRequest(HttpCookie cookie, string parameters, string locale)
        {
            CatalogServiceResponse result = new CatalogServiceResponse();
            var url = ConfigurationManager.AppSettings["UrlCatalogService"];
            url = string.Format(url, locale);

            if (!string.IsNullOrEmpty(parameters))
                url += "?" + parameters;

            var client = ClientWithAuthCookie(cookie);
            var response = GetContentAsync(client, url);
            result.Items = JsonConvert.DeserializeObject<List<CatalogItems>>(response.Result);

            return result;
        }
       

        private static HttpClient ClientWithAuthCookie(HttpCookie cookie)
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            var client = new HttpClient(handler)
            {
                //BaseAddress = new Uri("http://www.myherbalife.com")
            };
            cookieContainer.Add(new Cookie { Name = cookie.Name, Value = cookie.Value, HttpOnly = cookie.HttpOnly, Domain = cookie.Domain, Expires = cookie.Expires });
            client.DefaultRequestHeaders.Host = null;
            client.DefaultRequestHeaders.AcceptEncoding.Clear();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            return client;
        }

        private async Task<string> GetContentAsync(HttpClient httpClient, string uri)
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
