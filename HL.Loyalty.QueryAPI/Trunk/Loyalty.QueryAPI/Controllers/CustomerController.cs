using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI;
using HL.Loyalty.Providers.QueryAPI.CustomerProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Loyalty.QueryAPI.Controllers
{
    [ServiceRequestActionFilter]
    public class CustomerController : ApiController 
    {

        private ICustomerProvider _provider;

        public CustomerController(ICustomerProvider provider)
        {
            _provider = provider;
        }

        // GET api/<controller>
        public WrapperResult<IEnumerable<CustomerModel>> GetEnrolled(Guid ProgramId)
        {
            WrapperResult<IEnumerable<CustomerModel>> result = _provider.GetEnrolled(ProgramId);
            return result;
        }

        //GETCUSTOMERDASHBOARD
        public WrapperResult<CustomerDetailModel> Get(string Locale, Nullable<Guid> CustomerId = null, Nullable<Guid> GOHLCustomerID = null, string DistributorId = "")
        {
            WrapperResult<CustomerDetailModel> result = _provider.GetDashboard(Locale, CustomerId, GOHLCustomerID,DistributorId);
            return result;
        }


        [JsonCallback]
        public WrapperResult<CustomerDetailModel> GetDashboardJsonP(string Locale,string callback, Nullable<Guid> CustomerId = null, Nullable<Guid> GOHLCustomerID = null, string DistributorId = "")
        {
            WrapperResult<CustomerDetailModel> result = _provider.GetDashboard(Locale, CustomerId, GOHLCustomerID,DistributorId);
            return result;
        }

        [HttpGet]
        public bool? ValidateCustomerForOLC(string DistributorId, string Email)
        {
            bool? result = _provider.ValidateOLC(DistributorId, Email);
            return result;

        }



    }
}
