using System.Collections.Generic;
using System.Web.Http;
using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI.Activities;
using HL.Loyalty.Providers.QueryAPI;
using System;

namespace Loyalty.QueryAPI.Controllers
{
    [ServiceRequestActionFilter]
    public class ActivityController : ApiController
    {
        private IActivityProvider _provider;

        public ActivityController(IActivityProvider provider)
        {
            _provider = provider;

        }

        // GET api/<controller>
        public WrapperResult<List<ActivityModel>> Get(string locale)
        {
            WrapperResult<List<ActivityModel>> wrapperResult = _provider.GetActivities(locale);
            return wrapperResult;
        }

        public WrapperResult<List<ActivityModel>> GetGroupByTier(string locale)
        {
            WrapperResult<List<ActivityModel>> wrapperResult = _provider.GetActivities(locale);
            return wrapperResult;
        }

        public WrapperResult<IEnumerable<ProgramActivity>> GetActivitiesByProgram(Guid ProgramId)
        {
            WrapperResult<IEnumerable<ProgramActivity>> wrapperResult = _provider.GetActivitiesByProgram(ProgramId);
            return wrapperResult;
        }



    }
}
