using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using HL.Loyalty.Repository.QueryAPI.CatalogServices;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web;

namespace HL.Loyalty.Repository.QueryAPI.RewardRepository
{
    public class RewardRepository: IRewardRepository
    {
        
        public IConfigurationSettings _config { get; set; }

        public RewardRepository(IConfigurationSettings config)
        {
            this._config = config;
        }

        public WrapperResult<IEnumerable<LoyaltyRewardModel>> GetRewards(HttpCookie cookie, RewardType rewardType, string locale)
        {
            WrapperResult<IEnumerable<LoyaltyRewardModel>> result = new WrapperResult<IEnumerable<LoyaltyRewardModel>>();

            string countryCode = locale.GetCountryCode();

            using (var context = new Repository.QueryAPI.Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                SqlParameter categoryParameter = new SqlParameter() { ParameterName = "categoryCode" };
                SqlParameter countryCodeParameter = new SqlParameter() { ParameterName = "countryCode", Value = countryCode };
                SqlParameter[] parameters = new SqlParameter[] { categoryParameter, countryCodeParameter };
                //Getting rewards from Catalog Service
                List<LoyaltyRewardModel> catalogResult = new List<LoyaltyRewardModel>();


                try
                {
                    categoryParameter.Value = rewardType == RewardType.All ? (object)DBNull.Value : rewardType.ToString();
                    result.DataResult = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<LoyaltyRewardModel>("usp_GetRewards @CategoryCode = @categoryCode,@CountryCode=@countryCode", parameters).ToList();

                    if (rewardType == RewardType.Activity || rewardType == RewardType.All)
                        catalogResult.AddRange(this.GetRewardsCatalogServices(cookie, locale, PromotionalTypes.ActivityRewards.ToString()));

                    if (rewardType == RewardType.Product || rewardType == RewardType.All)
                        catalogResult.AddRange(this.GetRewardsCatalogServices(cookie, locale, PromotionalTypes.ShoppingRewards.ToString()));

                }
                catch (Exception ex)
                {
                    QueryAPIHelper.CreateError<IEnumerable<LoyaltyRewardModel>>(result, ex.Message, TraceTypes.Error);
                }

                result.DataResult = result.DataResult.Join(catalogResult, reward => reward.SKU, catalog => catalog.SKU, (reward, catalog) =>
                {
                    reward.Image = catalog.Image;
                    reward.Name = catalog.Name;
                    reward.Description = catalog.Description;
                    reward.Price = catalog.Price;
                    return reward;
                }).ToList();
            }


            return result;
        }

        public List<LoyaltyRewardModel> GetRewardsCatalogServices(HttpCookie cookie, string locale, string promotionalType)
        {
            CatalogServiceProxy proxy = new CatalogServiceProxy(_config);
            List<LoyaltyRewardModel> result = new List<LoyaltyRewardModel>();

            string parameters = string.Format("PromotionalType={0}&DetailLevel=ListingContent", promotionalType);
            CatalogServiceResponse serviceResponse = proxy.SendRequest(cookie, parameters, locale);
            if (serviceResponse != null)
            {
                foreach (var item in serviceResponse.Items)
                {
                    result.Add(new LoyaltyRewardModel() { Name = item.Name, SKU = item.Sku, Image = item.Content.ThumbnailImagePath, Price = item.Pricing.ListPrice });
                }
            }
            return result;
        }


        public WrapperResult<IEnumerable<RewardHistory>> GetRewardsByStatus(Guid ProgramId, string rewardStatus, string locale=null )
        {
            WrapperResult<IEnumerable<RewardHistory>> result = new WrapperResult<IEnumerable<RewardHistory>>();
            using (var context = new Repository.QueryAPI.Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                try
                {
                    SqlParameter programIdParam = new SqlParameter() { ParameterName = "programId", Value = ProgramId };
                    SqlParameter redemptionStatusCodeParam = new SqlParameter() { ParameterName = "redemptionStatusCode", Value = rewardStatus };
                    var parameters = new SqlParameter[] { programIdParam, redemptionStatusCodeParam };
                    

                    result.DataResult = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<RewardHistory>("usp_GetCustomerRewardStatus @ProgramId=@programid, @RedemptionStatusCode=@redemptionStatusCode", parameters).ToList();
                    if (!string.IsNullOrWhiteSpace(locale))
                    {
                        List<LoyaltyRewardModel> catalogResult = new List<LoyaltyRewardModel>();
                        HttpCookie cookie = CookieFactory.Create();
                        catalogResult.AddRange(this.GetRewardsCatalogServices(cookie, locale, PromotionalTypes.ActivityRewards.ToString()));
                        catalogResult.AddRange(this.GetRewardsCatalogServices(cookie, locale, PromotionalTypes.ShoppingRewards.ToString()));

                        result.DataResult = result.DataResult.Join(catalogResult, reward => reward.Sku, catalog => catalog.SKU, (reward, catalog) =>
                        {
                            reward.RewardName = catalog.Name;
                            return reward;
                        }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    QueryAPIHelper.CreateError<IEnumerable<RewardHistory>>(result, ex.Message, TraceTypes.Error);
                }
            }
            return result;

        }

        public WrapperResult<IEnumerable<RewardsTierGroup>> GetRewardsGroupByTier(HttpCookie cookie, RewardType rewardType, string locale)
        {
            WrapperResult<IEnumerable<RewardsTierGroup>> result = new WrapperResult<IEnumerable<RewardsTierGroup>>();
            WrapperResult<IEnumerable<LoyaltyRewardModel>> wrapperRewards = this.GetRewards(cookie, rewardType, locale);

            if (wrapperRewards.Status == WrapperResultType.Ok)
            {
                result.DataResult = wrapperRewards.DataResult.GroupBy(x => x.Tier, (key, rewards) => new RewardsTierGroup
                {
                    Tier = key??0,
                    Rewards = rewards.ToList()
                });
            }

            return result;
        }

        //This is only for activity Rewards
        public WrapperResult<IEnumerable<CustomerReward>> GetPendingRewards(Guid ProgramId, string locale)
        {
            WrapperResult<IEnumerable<CustomerReward>> result = new WrapperResult<IEnumerable<CustomerReward>>();
            string countryCode = locale.GetCountryCode();

            using (var context = new Repository.QueryAPI.Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                SqlParameter programIdParameter = new SqlParameter() { ParameterName = "programId", Value = ProgramId };
                SqlParameter redemptionStatusCodeParam = new SqlParameter() { ParameterName = "redemptionCode", Value = "Pending" };
                SqlParameter categoryCodeParam = new SqlParameter() { ParameterName = "categoryCode", Value = "Activity" };
                SqlParameter[] parameters = new SqlParameter[] { programIdParameter, redemptionStatusCodeParam, categoryCodeParam };
                HttpCookie cookie = CookieFactory.Create();
                

                try
                {
                    result.DataResult = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<CustomerReward>("usp_GetCustomerRewardStatus @ProgramId=@programId,@RedemptionStatusCode=@redemptionCode,@CategoryCode=@categoryCode", parameters).ToList();
                    List<LoyaltyRewardModel> catalogResult = new List<LoyaltyRewardModel>();
                    catalogResult.AddRange(this.GetRewardsCatalogServices(cookie, locale, PromotionalTypes.ActivityRewards.ToString()));
                    catalogResult.AddRange(this.GetRewardsCatalogServices(cookie, locale, PromotionalTypes.ShoppingRewards.ToString()));

                    result.DataResult = result.DataResult.Join(catalogResult, reward => reward.Sku, catalog => catalog.SKU, (reward, catalog) =>
                    {
                        reward.RewardName = catalog.Name;
                        return reward;
                    }).ToList();
                }
                catch (Exception ex)
                {
                    QueryAPIHelper.CreateError<IEnumerable<CustomerReward>>(result, ex.Message, TraceTypes.Error);
                }
            }


            return result;

        }

        public WrapperResult<List<SkusRewards>> ValidateRewards(SkusRewardsValidation SkusValidation)
        {
            WrapperResult<List<SkusRewards>> result = new WrapperResult<List<SkusRewards>>();
            result.DataResult = new List<SkusRewards>();
            SkusValidation.Skus.ForEach(x => result.DataResult.Add(new SkusRewards() { Sku = x }));

            var productInfo = this.RequestCustomerDashBoard(SkusValidation.CustomerIdType, SkusValidation.Locale, SkusValidation.Id,SkusValidation.DistributorId).ShoppingPoints;

            if (productInfo != null)
            {
                var ProductMaxTier = productInfo.ProductMaxTier ?? 0;

                bool achievedNextLevel = productInfo.ProductPoints + SkusValidation.TotalCartVolumenPoints >= (decimal)productInfo.ProductPointsNeededNextLevel;
                int? NextProductLevel = achievedNextLevel && productInfo.CanMoveToNextLevel ? productInfo.ProductNextTier : productInfo.ProductCurrentTier;

                if (productInfo.RewardsGroups != null)
                {
                    List<LoyaltyRewardModel> dashboardRewards = productInfo.RewardsGroups.Where(x => x.Tier <= NextProductLevel || x.Tier <= ProductMaxTier).SelectMany(x => x.Rewards).ToList();
                    if (dashboardRewards != null && dashboardRewards.Count > 0)
                    {
                        //if the customer redeemed a sku then the all rewards for that tier will be discard
                        var TierRedeemed = dashboardRewards.Where(x => x.Redeemed).Select(x => x.Tier);
                        dashboardRewards = dashboardRewards.Where(x => !TierRedeemed.Contains(x.Tier)).ToList();
                        dashboardRewards = dashboardRewards.OrderByDescending(x => x.Selected).ToList();

                        //set valid just one reward per tier
                        List<int> tiers = new List<int>();
                        foreach (var item in result.DataResult)
                        {
                            var skuTemp = dashboardRewards.Where(x => x.SKU == item.Sku && !tiers.Contains(x.Tier ?? 0)).FirstOrDefault();
                            if (skuTemp != null)
                            {
                                item.IsValid = true;
                                tiers.Add(skuTemp.Tier ?? 0);
                            }
                        }
                    }
                }
            }
            return result;
        }


        private CustomerDetailModel RequestCustomerDashBoard(CustomerTypes customerType, string locale, Guid Id, string DistributorId = "")
        {
            CustomerRepository.CustomerRepository customerRepository = new CustomerRepository.CustomerRepository(this._config);
            Nullable<Guid> GOHLCustomerId = null;
            Nullable<Guid> CustomerId = null;

            if (customerType == CustomerTypes.GOHLCustomer)
                GOHLCustomerId = Id;
            else
                CustomerId = Id;

            return customerRepository.GetDashboard(locale, CustomerId, GOHLCustomerId,DistributorId).DataResult;

        }
    }
}
