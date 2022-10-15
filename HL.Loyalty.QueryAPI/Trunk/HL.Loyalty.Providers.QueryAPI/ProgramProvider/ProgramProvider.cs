using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using HL.Loyalty.Repository.QueryAPI.ProgramRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Providers.QueryAPI.ProgramProvider
{
    public class ProgramProvider: IProgramProvider
    {

        private IProgramRepository _repository;

        public ProgramProvider(IProgramRepository repository)
        {
            this._repository = repository;
        }

        public WrapperResult<ProgramModel> GetProgram(string distributorId, string locale)
        {
            WrapperResult<ProgramModel> wrapper = new WrapperResult<ProgramModel>();

            if (!locale.ValidateLocale())
                QueryAPIHelper.CreateError<ProgramModel>(wrapper, "Locale must be provided", TraceTypes.Warning);


            if (string.IsNullOrWhiteSpace(distributorId))
                QueryAPIHelper.CreateError<ProgramModel>(wrapper, "DistributorId must be provided", TraceTypes.Warning);

            if (wrapper.Status == WrapperResultType.Ok)
                wrapper = _repository.GetProgram(distributorId, locale);

            return wrapper;
        }

    }
}
