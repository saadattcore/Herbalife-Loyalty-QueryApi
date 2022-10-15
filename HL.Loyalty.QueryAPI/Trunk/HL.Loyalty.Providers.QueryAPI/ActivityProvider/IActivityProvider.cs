using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Providers.QueryAPI.Activities
{

    public interface IActivityProvider
    {
        WrapperResult<List<ActivityModel>> GetActivities(string locale);

        WrapperResult<IEnumerable<ProgramActivity>> GetActivitiesByProgram(Guid ProgramId);
    }
}
