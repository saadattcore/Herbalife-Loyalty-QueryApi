using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HL.Loyalty.Repository.QueryAPI.RewardRepository
{
    public interface IRewardRepository
    {
        WrapperResult<IEnumerable<LoyaltyRewardModel>> GetRewards(HttpCookie cookie, RewardType rewardType, string locale);

        WrapperResult<IEnumerable<RewardsTierGroup>> GetRewardsGroupByTier(HttpCookie cookie, RewardType rewardType, string locale);

        WrapperResult<IEnumerable<RewardHistory>> GetRewardsByStatus(Guid ProgramId, string locale, string rewardStatus);

        WrapperResult<IEnumerable<CustomerReward>> GetPendingRewards(Guid ProgramId, string locale);

        WrapperResult<List<SkusRewards>> ValidateRewards(SkusRewardsValidation SkusValidation);
    }
}
