using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Repository.QueryAPI.CustomerRepository
{
    public interface ICustomerRepository
    {
        WrapperResult<IEnumerable<CustomerModel>> GetEnrolled(Guid ProgramId);
        WrapperResult<CustomerDetailModel> GetDashboard(string Locale, Nullable<Guid> CustomerId = null, Nullable<Guid> GOHLCustomerID = null, string DistributorId = null);


        bool? ValidateOLC(string DistributorId, string Email);

    }
}
