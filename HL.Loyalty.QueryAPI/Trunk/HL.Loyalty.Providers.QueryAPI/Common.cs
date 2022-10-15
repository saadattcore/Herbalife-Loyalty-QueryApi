using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Providers.QueryAPI
{
    public static class Common
    {

        public static string GetCountryCode(this string locale)
        {
            if (string.IsNullOrWhiteSpace(locale))
                throw new ArgumentNullException("Locale parameter is not provided");
            return locale.Split('-')[1];
        }

    }
}
