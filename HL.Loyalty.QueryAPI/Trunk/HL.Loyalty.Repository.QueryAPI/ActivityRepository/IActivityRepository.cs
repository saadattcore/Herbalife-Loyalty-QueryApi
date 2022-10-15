using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Repository.QueryAPI.ActivityRepository
{
    public interface IActivityRepository
    {
        WrapperResult<List<ActivityModel>> GetActivities(string locale);

        WrapperResult<IEnumerable<ProgramActivity>> GetActivitiesByProgram(Guid ProgramId);
    }
}
