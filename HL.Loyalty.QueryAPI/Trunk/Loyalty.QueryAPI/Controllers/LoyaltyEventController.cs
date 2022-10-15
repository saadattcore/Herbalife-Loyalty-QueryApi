using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI.LoyaltyEventProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Loyalty.QueryAPI.Controllers
{
    [ServiceRequestActionFilter]
    public class LoyaltyEventController : ApiController
    {
        private ILoyaltyEventProvider _provider;

        public LoyaltyEventController(ILoyaltyEventProvider provider)
        {
            _provider = provider;
        }


        // GET api/<controller>
        public WrapperResult<LoyaltyCustomerEventsFeed> Get(Guid CustomerID , string Locale)
        {
            WrapperResult<LoyaltyCustomerEventsFeed> wrapperResult = _provider.GetCustomerLoyaltyEventHistory(CustomerID , Locale);
            return wrapperResult;
        }

    }
}
