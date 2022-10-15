using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL.Loyalty.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using HL.Loyalty.Common.QueryAPI;
using System.Data.Common;
using HL.Loyalty.Common.QueryAPI.Enums;

namespace HL.Loyalty.Repository.QueryAPI.ActivityRepository
{
    public class ActivityRepository : IActivityRepository
    {
        private IConfigurationSettings _config { get; set; }


        public ActivityRepository(IConfigurationSettings config)
        {
            this._config = config;
        }

        public  WrapperResult<List<ActivityModel>> GetActivities(string countryCode)
        {
            WrapperResult<List<ActivityModel>> result = new WrapperResult<List<ActivityModel>>();
            result.DataResult = new List<ActivityModel>();

            using (var context = new Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                try
                {
                    var activities = context.usp_GetActivities(countryCode, DateTime.Now);
                    if (activities != null)
                    {
                        foreach (var item in activities)
                        {
                            decimal points = item.Points.HasValue ? (decimal)item.Points : 0;
                            result.DataResult.Add(new ActivityModel() { ActivityId = item.ActivityID, ActivityCode = item.ActivityCode, Description = item.Description, Title = item.Title, Points = points });
                        }
                    }
                }
                catch (Exception ex)
                {
                    QueryAPIHelper.CreateError<List<ActivityModel>>(result, ex.Message , TraceTypes.Error);
                }
            }
            return result;
        }

        public WrapperResult<IEnumerable<ProgramActivity>> GetActivitiesByProgram(Guid ProgramId)
        {
            WrapperResult<IEnumerable<ProgramActivity>> result = new WrapperResult<IEnumerable<ProgramActivity>>();
            using (var context = new Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                try
                {
                    SqlParameter programIdParameter = new SqlParameter() { ParameterName = "ProgramId", Value = ProgramId };
                    result.DataResult = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<ProgramActivity>("usp_GetActivitiesByProgram @ProgramId", programIdParameter).ToList();
                }
                catch (Exception ex)
                {
                    QueryAPIHelper.CreateError<IEnumerable<ProgramActivity>>(result, ex.Message, TraceTypes.Error);
                }
            }

            return result;
        }
    }
}
