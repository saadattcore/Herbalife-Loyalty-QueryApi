using System.Collections.Generic;
using System.Web.Http;
using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI.Rewards;
using System;
using System.Web;
using HL.Loyalty.Providers.QueryAPI.CatalogServices;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Repository.QueryAPI.CatalogServices;
using HL.Loyalty.Common.QueryAPI;

namespace Loyalty.QueryAPI.Controllers
{
    [ServiceRequestActionFilter]
    public class RewardsController : ApiController
    {

        private IRewardsProvider _provider;

        public RewardsController(IRewardsProvider provider)
        {
            _provider = provider;
            
        }


        public WrapperResult<IEnumerable<LoyaltyRewardModel>> GetActivityRewards(string locale)
        {
            WrapperResult<IEnumerable<LoyaltyRewardModel>> result = this.GetValueRewards(RewardType.Activity,locale);
            return result; 
        }

        public WrapperResult<IEnumerable<RewardsTierGroup>> GetActivityRewardsGroupedByTier(string locale)
        {
            HttpCookie cookie = CookieFactory.Create();
            WrapperResult<IEnumerable<RewardsTierGroup>> result = _provider.GetRewardsGroupByTier(cookie, RewardType.Activity, locale);
            return result;
        }

        public WrapperResult<IEnumerable<LoyaltyRewardModel>> GetShoppingRewards(string locale)
        {
            WrapperResult<IEnumerable<LoyaltyRewardModel>> result = this.GetValueRewards(RewardType.Product,locale);
            return result;
        }

        public WrapperResult<IEnumerable<LoyaltyRewardModel>> GetHighValueRewards(string locale, string rewardType=null)
        {
            //Reward filter
           RewardType _rewardType = RewardType.All; 
           if (!string.IsNullOrWhiteSpace(rewardType))
                Enum.TryParse<RewardType>(rewardType, out _rewardType);
            
            var result = this.GetValueRewards(_rewardType, locale);
            return result;
        }

        public WrapperResult<IEnumerable<RewardHistory>> GetRedeemedRewards(Guid ProgramId, string locale=null)
        {
            return _provider.GetRedeemedRewards(ProgramId,locale);
        }


        public WrapperResult<IEnumerable<CustomerReward>> GetPendingRewards(Guid ProgramId, string locale)
        {
            return _provider.GetPendingRewards(ProgramId, locale);
        }


        [HttpPost]
        public WrapperResult<List<SkusRewards>> ValidateSkusRewards([FromBody] SkusRewardsValidation RewardsInformation)
        {
            WrapperResult<List<SkusRewards>> result = _provider.ValidateRewards(RewardsInformation);
            return result;
        }


        private WrapperResult<IEnumerable<LoyaltyRewardModel>> GetValueRewards(HL.Loyalty.Common.QueryAPI.Enums.RewardType rewardType, string locale)
        {
            HttpCookie cookie = CookieFactory.Create();
            WrapperResult<IEnumerable<LoyaltyRewardModel>> result = _provider.GetRewards(cookie, rewardType, locale);
            return result;
        }


    }
}