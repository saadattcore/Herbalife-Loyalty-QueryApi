using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using HL.Loyalty.Repository.QueryAPI.RewardRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HL.Loyalty.Providers.QueryAPI.Rewards
{
    public class RewardsProvider: IRewardsProvider
    {


        private IRewardRepository _repository;

        public RewardsProvider(IRewardRepository repository)
        {
            this._repository = repository;
        }


        public WrapperResult<IEnumerable<LoyaltyRewardModel>> GetRewards(HttpCookie cookie, RewardType rewardType, string locale)
        {
            WrapperResult<IEnumerable<LoyaltyRewardModel>> wrapper = new WrapperResult<IEnumerable<LoyaltyRewardModel>>();

            if (!locale.ValidateLocale())
                QueryAPIHelper.CreateError<IEnumerable<LoyaltyRewardModel>>(wrapper, "Locale must be provided", TraceTypes.Warning);

            if (wrapper.Status == WrapperResultType.Ok)
                wrapper = _repository.GetRewards(cookie, rewardType, locale);

            return wrapper;
        }


        public WrapperResult<IEnumerable<RewardHistory>> GetRedeemedRewards(Guid programId,string locale=null)
        {
            WrapperResult<IEnumerable<RewardHistory>> wrapper = new WrapperResult<IEnumerable<RewardHistory>>();

            if (!programId.ValidateGuid())
                QueryAPIHelper.CreateError<IEnumerable<RewardHistory>>(wrapper, "ProgramId must be provided", TraceTypes.Warning);

            if(wrapper.Status== WrapperResultType.Ok)
                wrapper = _repository.GetRewardsByStatus(programId, "FULFILLED", locale);

            if (wrapper.Status != WrapperResultType.Ok) return wrapper;

            // Adding PendingInOrder rewards as these should be treated as redeemed rewards
            var pendingRewards = _repository.GetRewardsByStatus(programId, "PendingInOrder", locale);
            if(pendingRewards.Status == WrapperResultType.Ok)
                wrapper.DataResult.ToList().AddRange(pendingRewards.DataResult.ToList());

            return wrapper;

        }

        public WrapperResult<IEnumerable<RewardsTierGroup>> GetRewardsGroupByTier(HttpCookie cookie, RewardType rewardType, string locale)
        {
            WrapperResult<IEnumerable<RewardsTierGroup>> wrapper = new WrapperResult<IEnumerable<RewardsTierGroup>>();

            if (!locale.ValidateLocale())
                QueryAPIHelper.CreateError<IEnumerable<RewardsTierGroup>>(wrapper, "Locale must be provided", TraceTypes.Warning);

            if(wrapper.Status== WrapperResultType.Ok)
                wrapper = _repository.GetRewardsGroupByTier(cookie, rewardType, locale);

            return wrapper;
        }

        //This is only for activity Rewards
        public WrapperResult<IEnumerable<CustomerReward>> GetPendingRewards(Guid ProgramId,string locale)
        {
            WrapperResult<IEnumerable<CustomerReward>> wrapper = new WrapperResult<IEnumerable<CustomerReward>>();

            if(!ProgramId.ValidateGuid())
                QueryAPIHelper.CreateError<IEnumerable<CustomerReward>>(wrapper, "ProgramId must be provided", TraceTypes.Warning);

            if (!locale.ValidateLocale())
                QueryAPIHelper.CreateError<IEnumerable<CustomerReward>>(wrapper, "Locale must be provided", TraceTypes.Warning);

            if (wrapper.Status == WrapperResultType.Ok)
                wrapper = _repository.GetPendingRewards(ProgramId, locale);

            return wrapper;

        }

        public WrapperResult<List<SkusRewards>> ValidateRewards(SkusRewardsValidation SkusValidation)
        {
            WrapperResult<List<SkusRewards>> wrapper = new WrapperResult<List<SkusRewards>>();

            wrapper = _repository.ValidateRewards(SkusValidation);
            return wrapper;
        }

    }
}
