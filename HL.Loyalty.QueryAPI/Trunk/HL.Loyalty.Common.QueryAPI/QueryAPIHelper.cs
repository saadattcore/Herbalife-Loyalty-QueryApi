using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using System;
using System.Fabric;

namespace HL.Loyalty.Common.QueryAPI
{
    public static class QueryAPIHelper
    {

        public static string GetCountryCode(this string locale)
        {
            if (string.IsNullOrWhiteSpace(locale) || locale.Split('-').Length<2)
                throw new ArgumentNullException("Locale parameter is not provided");
            return locale.Split('-')[1];
        }

        public static bool ValidateGuid(this Guid guid)
        {
            string strGuid = guid.ToString();
            Guid tempGuid;

            if (guid == Guid.Empty)
                return false;

            return Guid.TryParse(strGuid, out tempGuid);
        }


        public static bool ValidateLocale(this string locale)
        {
            return (!string.IsNullOrWhiteSpace(locale) && !(locale.Split('-').Length < 2));
        }

        public static bool ValidateHMSReceiptID(this string HMSReceiptID)
        {
            return (!string.IsNullOrWhiteSpace(HMSReceiptID));
        }


        public static void CreateError<T>(WrapperResult<T> wrapper, string message, TraceTypes traceType)
        {
            wrapper.ErrorResult = new Exception(message);
            wrapper.Status = WrapperResultType.Error;

            //Trace
            switch (traceType)
            {
                case TraceTypes.Error:
                    HL.Loyalty.Common.QueryAPI.ServiceEventSource.Current.MessageError(message);
                    break;
                case TraceTypes.Warning:
                    HL.Loyalty.Common.QueryAPI.ServiceEventSource.Current.MessageWarning(message);
                    break; 
                default:
                    HL.Loyalty.Common.QueryAPI.ServiceEventSource.Current.MessageInfo(message);
                    break;
            }

        }

        

       
    }
}
