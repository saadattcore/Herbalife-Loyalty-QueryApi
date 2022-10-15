using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Common.QueryAPI
{
    public interface IConfigurationSettings
    {
        string UrlCatalogServices { get; set; }
        string ConnectionString { get; set; }



    }
}
