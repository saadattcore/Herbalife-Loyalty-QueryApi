using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using HL.Loyalty.Repository.QueryAPI.RewardRepository;
using System.Text;
using System.Threading.Tasks;

namespace HL.Loyalty.Repository.QueryAPI.ProgramRepository
{
    public class ProgramRepository:IProgramRepository
    {
        private IConfigurationSettings _config { get; set; }

        public ProgramRepository(IConfigurationSettings config)
        {
            this._config = config;
        }
        public WrapperResult<ProgramModel> GetProgram(string distributorId, string locale)
        {
            WrapperResult<ProgramModel> result = new WrapperResult<ProgramModel>();
            var countryCode = locale.GetCountryCode();

            using (var context = new Repository.QueryAPI.Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                // Create a SQL command to execute the sproc
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "Exec usp_GetProgramDetailsByDistributor @DistributorId,@CountryCodeISO";
                cmd.Parameters.Add(new SqlParameter("DistributorId", distributorId));
                cmd.Parameters.Add(new SqlParameter("CountryCodeISO", countryCode));

                try
                {
                    context.Database.Connection.Open();
                    // Run the sproc 
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        // Read Blogs from the first result set
                        var programs = ((IObjectContextAdapter)context)
                            .ObjectContext
                            .Translate<ProgramModel>(reader).ToList();

                        result.DataResult = programs.FirstOrDefault();

                        reader.NextResult();

                        var activities = ((IObjectContextAdapter)context)
                                        .ObjectContext
                                        .Translate<ActivityModel>(reader).ToList();
                        result.DataResult.Activities = activities;

                        reader.NextResult();
                        var activityrewards = ((IObjectContextAdapter)context)
                                        .ObjectContext
                                        .Translate<LoyaltyRewardModel>(reader).ToList();
                        result.DataResult.ActivityRewardsGifts = activityrewards;

                        reader.NextResult();
                        var purchaseRewards = ((IObjectContextAdapter)context)
                                        .ObjectContext
                                        .Translate<LoyaltyRewardModel>(reader).ToList();
                        result.DataResult.PurchaseRewardsGifts = purchaseRewards;


                        List<LoyaltyRewardModel> catalogResult = new List<LoyaltyRewardModel>();
                        var cookie = CatalogServices.CookieFactory.Create();
                        RewardRepository.RewardRepository rewardRepository = new RewardRepository.RewardRepository(_config);

                        catalogResult.AddRange(rewardRepository.GetRewardsCatalogServices(cookie, locale, PromotionalTypes.ActivityRewards.ToString()));
                        catalogResult.AddRange(rewardRepository.GetRewardsCatalogServices(cookie, locale, PromotionalTypes.ShoppingRewards.ToString()));

                        this.RewardGroup(result.DataResult.ActivityRewardsGifts, catalogResult);
                        this.RewardGroup(result.DataResult.PurchaseRewardsGifts, catalogResult);

                    }

                }
                catch (Exception ex)
                {
                    QueryAPIHelper.CreateError<ProgramModel>(result, ex.Message, TraceTypes.Error);
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

            return result;
        }


        private void RewardGroup(List<LoyaltyRewardModel> rewards, List<LoyaltyRewardModel> catalogResult)
        {

            rewards = rewards.Join(catalogResult, reward => reward.SKU, catalog => catalog.SKU, (reward, catalog) =>
            {
                reward.Image = catalog.Image;
                reward.Name = catalog.Name;
                reward.Description = catalog.Description;
                reward.Price = catalog.Price;
                return reward;
            }).ToList();

        }
    }
}
