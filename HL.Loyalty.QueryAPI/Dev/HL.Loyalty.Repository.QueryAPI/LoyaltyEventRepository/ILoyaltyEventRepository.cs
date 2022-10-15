using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Repository.QueryAPI.LoyaltyEventRepository
{
    public interface ILoyaltyEventRepository
    {
        WrapperResult<LoyaltyCustomerEventsFeed> GetCustomerLoyaltyEventHistory(Guid CustomerID);
    }
}
