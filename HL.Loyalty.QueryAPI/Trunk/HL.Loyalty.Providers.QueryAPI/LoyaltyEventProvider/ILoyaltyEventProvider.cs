using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Providers.QueryAPI.LoyaltyEventProvider
{
    public interface ILoyaltyEventProvider
    {
        WrapperResult<LoyaltyCustomerEventsFeed> GetCustomerLoyaltyEventHistory(Guid CustomerID , string locale);
    }
}
