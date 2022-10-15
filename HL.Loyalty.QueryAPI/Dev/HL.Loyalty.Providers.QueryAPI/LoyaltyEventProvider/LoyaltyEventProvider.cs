using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL.Loyalty.Models;
using HL.Loyalty.Repository.QueryAPI.LoyaltyEventRepository;
using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Providers.QueryAPI.Rewards;
using System.Web;
using HL.Loyalty.Repository.QueryAPI.CatalogServices;

namespace HL.Loyalty.Providers.QueryAPI.LoyaltyEventProvider
{
    public class LoyaltyEventProvider : ILoyaltyEventProvider
    {
        private ILoyaltyEventRepository _repository;
        private IRewardsProvider _rewardProvider;

        public LoyaltyEventProvider(LoyaltyEventRepository repository, IRewardsProvider _rewardProvider)
        {
            this._repository = repository;
            this._rewardProvider = _rewardProvider;
        }

        public WrapperResult<LoyaltyCustomerEventsFeed> GetCustomerLoyaltyEventHistory(Guid CustomerID, string Locale)
        {
            WrapperResult<LoyaltyCustomerEventsFeed> wrapper = new WrapperResult<LoyaltyCustomerEventsFeed>();

            if (!CustomerID.ValidateGuid())
                QueryAPIHelper.CreateError<LoyaltyCustomerEventsFeed>(wrapper, "CustomerID must be provided", TraceTypes.Warning);

            if (wrapper.Status == WrapperResultType.Ok)
                wrapper = _repository.GetCustomerLoyaltyEventHistory(CustomerID);

            var customerWishList = wrapper.DataResult.WishListFeeds;
            if (customerWishList == null || customerWishList.Count == 0) return wrapper;

            // Assign each wish list reward name
            HttpCookie cookie = CookieFactory.Create();
            var rewardsWrapper = _rewardProvider.GetRewards(cookie, RewardType.All, Locale);

            if (rewardsWrapper != null && rewardsWrapper.DataResult != null && rewardsWrapper.DataResult.Count() > 0)
            {
                var rewards = rewardsWrapper.DataResult.ToList();

                foreach (var wishItem in customerWishList)
                {
                    var reward = rewards.Where(r => r.SKU == wishItem.Sku && r.Tier == wishItem.Tier && r.CountryCodeIso == wishItem.CountryCode).FirstOrDefault();
                    wishItem.RewardName = reward != null ? reward.Name : null;
                }
            }
            return wrapper;
        }
    }
}
