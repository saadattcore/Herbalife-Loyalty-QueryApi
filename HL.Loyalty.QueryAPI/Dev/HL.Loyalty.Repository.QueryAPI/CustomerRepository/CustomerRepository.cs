using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using HL.Loyalty.Repository.QueryAPI.CatalogServices;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HL.Loyalty.Repository.QueryAPI.CustomerRepository
{
    public class CustomerRepository: ICustomerRepository
    {
        
        public IConfigurationSettings _config { get; set; }

        public CustomerRepository(IConfigurationSettings config)
        {
            this._config = config;
        }

        public WrapperResult<IEnumerable<CustomerModel>> GetEnrolled(Guid ProgramId)
        {
            WrapperResult<IEnumerable<CustomerModel>> result = new WrapperResult<IEnumerable<CustomerModel>>();
            using (var context = new Repository.QueryAPI.Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                try
                {
                    SqlParameter programIdParameter = new SqlParameter() { ParameterName = "ProgramId", Value = ProgramId };
                    result.DataResult = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<CustomerModel>("usp_GetCustomerEnrolled @ProgramId", programIdParameter).ToList();

                }
                catch (Exception ex)
                {
                    QueryAPIHelper.CreateError<IEnumerable<CustomerModel>>(result, ex.Message, TraceTypes.Error);
                }
            }
            return result;

        }

        public WrapperResult<CustomerDetailModel> GetDashboard(string Locale, Nullable<Guid> CustomerId = null, Nullable<Guid> GOHLCustomerID = null, string DistributorId = "")
        {
            WrapperResult<CustomerDetailModel> result = new WrapperResult<CustomerDetailModel>();
            result.DataResult = new CustomerDetailModel();

            using (var context = new Repository.QueryAPI.Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {

                // Create a SQL command to execute the sproc
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "usp_GetCustomerDashboard";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                object CustomerIdValue = CustomerId ?? (object)DBNull.Value;
                object GohlCustomerIdValue = GOHLCustomerID ?? (object)DBNull.Value;
                object DistributorIdValue = string.IsNullOrEmpty(DistributorId) ? (object)DBNull.Value : DistributorId;
                string CountryCodeValue = Locale.GetCountryCode();

                cmd.Parameters.Add(new SqlParameter("@CustomerId", CustomerIdValue));
                cmd.Parameters.Add(new SqlParameter("@GOHLCustomerID", GohlCustomerIdValue));
                cmd.Parameters.Add(new SqlParameter("@DistributorId", DistributorIdValue));
                cmd.Parameters.Add(new SqlParameter("@CountryCodeISO", CountryCodeValue));

                try
                {
                    context.Database.Connection.Open();
                    // Run the sproc 
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        if (reader.IsDBNull(0))
                            return result;

                        result.DataResult.CustomerId = reader.GetGuid(reader.GetOrdinal("CustomerId"));
                        result.DataResult.GoHLCustomerId = reader.GetGuid(reader.GetOrdinal("GoHLCustomerID"));
                        result.DataResult.ProgramId = reader.GetGuid(reader.GetOrdinal("ProgramId"));
                        result.DataResult.EnableShoppingRewards = reader.GetBoolean(reader.GetOrdinal("EnableShoppingRewards"));
                        result.DataResult.EnableActivityRewards = reader.GetBoolean(reader.GetOrdinal("EnableActivityRewards"));
                        result.DataResult.CustomerStartDate = reader.GetDateTime(reader.GetOrdinal("CustomerStartDate"));
                        result.DataResult.CustomerEndDate = reader.GetDateTime(reader.GetOrdinal("CustomerEndDate"));

                        reader.NextResult();

                        var activities = ((IObjectContextAdapter)context)
                                        .ObjectContext
                                        .Translate<CustomerActivityPoints>(reader).ToList();
                        result.DataResult.ActivityPoints = activities.FirstOrDefault();
                        reader.NextResult();

                        var shopping = ((IObjectContextAdapter)context)
                                        .ObjectContext
                                        .Translate<CustomerShoppingPoints>(reader).FirstOrDefault();
                        result.DataResult.ShoppingPoints = shopping == null ? new CustomerShoppingPoints() : shopping;
                        reader.NextResult();

                        var rewards = ((IObjectContextAdapter)context)
                                        .ObjectContext
                                        .Translate<LoyaltyRewardModel>(reader).ToList();
                        //result.DataResult.Rewards = rewards;
                        reader.NextResult();

                        var activeTier = ((IObjectContextAdapter)context)
                                        .ObjectContext
                                        .Translate<ActiveTierInfo>(reader).ToList();

                        result.DataResult.TierInfo = activeTier;

                        HttpCookie cookie = CookieFactory.Create();
                        RewardRepository.RewardRepository rewardsRepository = new RewardRepository.RewardRepository(_config);


                        //REMOVE THIS WHEN CATALOG SERVICE IS WORKING
                        //var rewardsCompleted = rewards;
                        List<LoyaltyRewardModel> catalogResult = new List<LoyaltyRewardModel>();
                        catalogResult.AddRange(rewardsRepository.GetRewardsCatalogServices(cookie, Locale, "ShoppingRewards"));
                        catalogResult.AddRange(rewardsRepository.GetRewardsCatalogServices(cookie, Locale, "ActivityRewards"));

                        var rewardsCompleted = rewards.Join(catalogResult, reward => reward.SKU, catalog => catalog.SKU, (reward, catalog) =>
                        {
                            reward.Name = catalog.Name;
                            reward.CountryCodeIso = Locale;
                            reward.Image = catalog.Image;
                            return reward;
                        }).ToList();

                        if (result.DataResult.ActivityPoints != null)
                            result.DataResult.ActivityPoints.RewardsGroups = this.GroupRewardsGroup(rewardsCompleted, PointsCategoryType.Activity);

                        if (result.DataResult.ShoppingPoints != null)
                        {
                            result.DataResult.ShoppingPoints.RewardsGroups = this.GroupRewardsGroup(rewardsCompleted, PointsCategoryType.Product);

                            var productInfo = result.DataResult.ShoppingPoints;
                            result.DataResult.ShoppingPoints.CanMoveToNextLevel = (productInfo.ConsecutiveMonthsAchieved >= 1
                               && productInfo.ProductPointsNeededNextLevel != null
                               && productInfo.ProductPointsNeededNextLevel > 0);
                        }
                    }
                }
                catch (Exception ex)
                {
                        QueryAPIHelper.CreateError<CustomerDetailModel>(result, ex.Message, TraceTypes.Error);
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

            return result;
        }

        private List<RewardsTierGroup> GroupRewardsGroup(List<LoyaltyRewardModel> rewards, PointsCategoryType categoryType)
        {
            List<RewardsTierGroup> result = new List<RewardsTierGroup>();
            var _rewards = rewards.Where(x => x.CategoryCode == categoryType.ToString()).ToList();
            result = _rewards.GroupBy(x => x.Tier, (key, rewardGroup) => new RewardsTierGroup
            {
                Redeemed = rewardGroup.Where(y => y.Redeemed).Count() > 0 ? true : false,
                Tier = key??0,
                Rewards = rewardGroup.ToList()
            }).ToList();
            return result;
        }


        public bool? ValidateOLC(string DistributorId, string Email)
        {
            bool? result;
            using (var context = new Repository.QueryAPI.Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                SqlParameter distributorIdParameter = new SqlParameter() { ParameterName = "DistributorId", Value = DistributorId };
                SqlParameter emailParameter = new SqlParameter() { ParameterName = "Email", Value = Email };

                try
                {
                    result = context.usp_ValidateCustomerOLC(DistributorId, Email, null).First();
                }
                catch (Exception ex)
                {
                    HL.Loyalty.ServiceFabric.Tracing.Logger.Current.Error(ex.Message, ex);
                    return null;
                }
            }
            return result;
        }

    }
}
