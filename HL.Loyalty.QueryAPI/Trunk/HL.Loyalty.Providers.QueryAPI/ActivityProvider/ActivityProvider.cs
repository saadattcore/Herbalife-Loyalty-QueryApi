using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using HL.Loyalty.Repository.QueryAPI;
using HL.Loyalty.Repository.QueryAPI.ActivityRepository;
using System;
using System.Collections.Generic;

namespace HL.Loyalty.Providers.QueryAPI.Activities
{
    public class ActivityProvider:IActivityProvider
    {
        private IActivityRepository _repository;

        public ActivityProvider(IActivityRepository repository)
        {
            this._repository = repository;
        }


        public WrapperResult<List<ActivityModel>> GetActivities(string locale)
        {
            WrapperResult<List<ActivityModel>> wrapper = new WrapperResult<List<ActivityModel>>();
            string countryCode = "";

            if (!locale.ValidateLocale())
                QueryAPIHelper.CreateError<List<ActivityModel>>(wrapper, "Must Provide ProgramId", TraceTypes.Warning);


            if (wrapper.Status == WrapperResultType.Ok)
            {
                countryCode = locale.GetCountryCode();
                wrapper = _repository.GetActivities(countryCode);
            }

            return wrapper;
            
        }

        public WrapperResult<IEnumerable<ProgramActivity>> GetActivitiesByProgram(Guid ProgramId)
        {
            WrapperResult<IEnumerable<ProgramActivity>> wrapper = new WrapperResult<IEnumerable<ProgramActivity>>();

            if(!ProgramId.ValidateGuid())
                QueryAPIHelper.CreateError<IEnumerable<ProgramActivity>>(wrapper, "ProgramId must be provided", TraceTypes.Warning);

            if(wrapper.Status== WrapperResultType.Ok)
                wrapper = _repository.GetActivitiesByProgram(ProgramId);

            return wrapper;

        }
    }
}
