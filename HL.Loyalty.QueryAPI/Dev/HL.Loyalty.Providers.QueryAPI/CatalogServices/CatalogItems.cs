using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Providers.QueryAPI.CatalogServices
{
    public class CatalogItems
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Image { get; set; }

        public CatalogContent Content { get; set; }

    }

    public class CatalogContent
    {
        public string ThumbnailImagePath { get; set; }


    }

}
