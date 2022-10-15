using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Common.QueryAPI
{
    public class ConfigurationSettings : IConfigurationSettings
    {

        public ConfigurationSettings(string connectionString, string urlCatalogServices)
        {
            this.ConnectionString = connectionString;
            this.UrlCatalogServices = urlCatalogServices;
        }

        public string UrlCatalogServices { get; set; }
        public string ConnectionString { get; set; }

    }
}
