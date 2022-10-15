using System;
using System.Collections.Generic;
using System.Linq;
using HL.Loyalty.Models;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Web;
using HL.Loyalty.Providers.QueryAPI.CatalogServices;
using HL.Loyalty.Providers.QueryAPI.Rewards;
using HL.Loyalty.Repository.QueryAPI.CustomerRepository;
using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;

namespace HL.Loyalty.Providers.QueryAPI.CustomerProvider
{
    public class CustomerProvider : ICustomerProvider
    {
        private ICustomerRepository _repository;

        public CustomerProvider(ICustomerRepository repository)
        {
            this._repository = repository;
        }


        public WrapperResult<IEnumerable<CustomerModel>> GetEnrolled(Guid ProgramId)
        {
            WrapperResult<IEnumerable<CustomerModel>> wrapper = new WrapperResult<IEnumerable<CustomerModel>>();

            if (!ProgramId.ValidateGuid())
                QueryAPIHelper.CreateError<IEnumerable<CustomerModel>>(wrapper, "ProgramId must be provided", TraceTypes.Warning);

            if(wrapper.Status == WrapperResultType.Ok)
                wrapper = _repository.GetEnrolled(ProgramId);

            return wrapper;
        }

        public WrapperResult<CustomerDetailModel> GetDashboard(string Locale, Nullable<Guid> CustomerId = null, Nullable<Guid> GOHLCustomerID = null, string DistributorId = "")
        {
            WrapperResult<CustomerDetailModel> wrapper = new WrapperResult<CustomerDetailModel>();
            wrapper.DataResult = new CustomerDetailModel();

            if (!Locale.ValidateLocale())
                QueryAPIHelper.CreateError<CustomerDetailModel>(wrapper, "Locale must be provided", TraceTypes.Warning);

            if (CustomerId == null && GOHLCustomerID == null)
                QueryAPIHelper.CreateError<CustomerDetailModel>(wrapper, "CustomerId or GOHLCustomerId must be provided", TraceTypes.Warning);

            if (wrapper.Status == WrapperResultType.Ok)
                wrapper = _repository.GetDashboard(Locale, CustomerId, GOHLCustomerID, DistributorId);

            return wrapper;
        }

        public bool? ValidateOLC(string DistributorId , string Email)
        {
            if (string.IsNullOrEmpty(DistributorId) || string.IsNullOrEmpty(Email))
                return false;
            
            return _repository.ValidateOLC(DistributorId, Email);
             
        }
    }
}
