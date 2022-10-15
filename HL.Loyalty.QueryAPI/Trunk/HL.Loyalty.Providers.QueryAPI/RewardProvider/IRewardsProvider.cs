using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace HL.Loyalty.Providers.QueryAPI.Rewards
{
    public interface IRewardsProvider
    {
        WrapperResult<IEnumerable<LoyaltyRewardModel>> GetRewards(HttpCookie cookie, RewardType rewardType, string locale);

        WrapperResult<IEnumerable<RewardsTierGroup>> GetRewardsGroupByTier(HttpCookie cookie, RewardType rewardType, string locale);

        WrapperResult<IEnumerable<RewardHistory>> GetRedeemedRewards(Guid ProgramId, string locale);

        WrapperResult<IEnumerable<CustomerReward>> GetPendingRewards(Guid ProgramId, string locale);

        WrapperResult<List<SkusRewards>> ValidateRewards(SkusRewardsValidation SkusValidation);


    }
}
