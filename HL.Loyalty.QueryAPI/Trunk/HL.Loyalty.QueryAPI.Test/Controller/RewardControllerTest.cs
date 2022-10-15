using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loyalty.QueryAPI.Controllers;
using HL.Loyalty.Providers.QueryAPI.Rewards;
using HL.Loyalty.Models;
using HL.Loyalty.Repository.QueryAPI.RewardRepository;
using NSubstitute;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using HL.Loyalty.Common.QueryAPI.Enums;
using System.Configuration;

namespace HL.Loyalty.QueryAPI.Test.Controllers
{
    [TestClass]
    public class RewardControllerTest
    {
        RewardsController _controller;

        public RewardControllerTest()
        {
            IRewardRepository repository = NSubstitute.Substitute.For<IRewardRepository>();
            repository.GetPendingRewards(Arg.Any<Guid>(), Arg.Any<string>()).Returns(this.GetCustomerRewardMockup());
            repository.GetRewardsByStatus(Arg.Any<Guid>(),Arg.Any<string>(), Arg.Any<string>()).Returns(this.GetRedeemedRewardsMockup());
            repository.GetRewards(Arg.Any<HttpCookie>(), Arg.Any<RewardType>(), Arg.Any<string>()).Returns(this.GetRewardsMockup());
            repository.GetRewardsGroupByTier(Arg.Any<HttpCookie>(), Arg.Any<RewardType>(), Arg.Any<string>()).Returns(this.GetRewardsGroupByTierMockup());

            IRewardsProvider provider = new RewardsProvider(repository);
            _controller = new RewardsController(provider);
        }


        [TestMethod]
        public void GetActivitiesRewards()
        {
            var response = _controller.GetActivityRewards("en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void GetActivitiesRewards_NULL_LOCALE()
        {
            var response = _controller.GetActivityRewards(null);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        public void GetActivitiesRewards_EMPTY_LOCALE()
        {
            var response = _controller.GetActivityRewards("");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }



        [TestMethod]
        public void GetActivitiesRewardsGroupByTier()
        {
            var response = _controller.GetActivityRewardsGroupedByTier("en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }


        [TestMethod]
        public void GetActivitiesRewardsGroupByTier_NULL_LOCALE()
        {
            var response = _controller.GetActivityRewardsGroupedByTier(null);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }



        [TestMethod]
        public void GetShoppingRewards()
        {
            var response = _controller.GetShoppingRewards("en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void GetShoppingRewards_NULL_LOCALE()
        {
            var response = _controller.GetShoppingRewards(null);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        public void GetHighValueRewards()
        {
            var response = _controller.GetHighValueRewards("en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void GetHighValueRewards_NULL_LOCALE()
        {
            var response = _controller.GetHighValueRewards(null);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        public void GetRedeemedRewards()
        {
            var response = _controller.GetRedeemedRewards(Guid.Parse("5196E9A6-AEAA-E611-80C3-00155DE1E513"));

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void GetRedeemedRewards_EMPTY_PROGRAMID()
        {
            var response = _controller.GetRedeemedRewards(Guid.Empty);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }


        [TestMethod]
        public void GetPendingRewards()
        {
            var response = _controller.GetPendingRewards(Guid.Parse("5196E9A6-AEAA-E611-80C3-00155DE1E513"), "en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }


        [TestMethod]
        public void GetPendingRewards_EMPTY_PROGRAMID()
        {
            var response = _controller.GetPendingRewards(Guid.Empty, "en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        public void GetPendingRewards_NULL_LOCALE()
        {
            var response = _controller.GetPendingRewards(Guid.NewGuid(), null);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetActivitiesRewards_INTEGRATION()
        {
            var operation = "rewards/GetActivityRewards?locale=en-US";
            var response = Common.SendRequest<WrapperResult<IEnumerable<LoyaltyRewardModel>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }


        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetActivitiesRewards_NULL_LOCALE_INTEGRATION()
        {
            var operation = "rewards/GetActivityRewards?locale=";
            var response = Common.SendRequest<WrapperResult<IEnumerable<LoyaltyRewardModel>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetActivitiesRewardsGroupByTier_INTEGRATION()
        {
            var operation = "rewards/GetActivityRewardsGroupedByTier?locale=en-US";
            var response = Common.SendRequest<WrapperResult<IEnumerable<RewardsTierGroup>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }


        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetActivitiesRewardsGroupByTier_NULL_LOCALE_INTEGRATION()
        {
            var operation = "rewards/GetActivityRewardsGroupedByTier?locale=";
            var response = Common.SendRequest<WrapperResult<IEnumerable<RewardsTierGroup>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetShoppingRewards_INTEGRATION()
        {
            var operation = "rewards/GetShoppingRewards?locale=en-US";
            var response = Common.SendRequest<WrapperResult<IEnumerable<LoyaltyRewardModel>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetShoppingRewards_NULL_LOCALE_INTEGRATION()
        {
            var operation = "rewards/GetShoppingRewards?locale=";
            var response = Common.SendRequest<WrapperResult<IEnumerable<LoyaltyRewardModel>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetHighValueRewards_INTEGRATION()
        {
            var operation = "rewards/GetHighValueRewards?locale=en-US";
            var response = Common.SendRequest<WrapperResult<IEnumerable<LoyaltyRewardModel>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetHighValueRewards_NULL_LOCALE_INTEGRATION()
        {
            var operation = "rewards/GetHighValueRewards?locale=";
            var response = Common.SendRequest<WrapperResult<IEnumerable<LoyaltyRewardModel>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetRedeemedRewards_INTEGRATION()
        {
            var operation = "rewards/GetRedeemedRewards?ProgramId=" + Guid.NewGuid();
            var response = Common.SendRequest<WrapperResult<IEnumerable<RewardHistory>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetRedeemedRewards_EMPTY_PROGRAMID_INTEGRATION()
        {
            var operation = "rewards/GetRedeemedRewards?ProgramId=" + Guid.Empty;
            var response = Common.SendRequest<WrapperResult<IEnumerable<RewardHistory>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetPendingRewards_INTEGRATION()
        {
            var operation = "rewards/GetPendingRewards?locale=en-US&ProgramId=" + Guid.NewGuid();
            var response = Common.SendRequest<WrapperResult<IEnumerable<CustomerReward>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetPendingRewards_EMPTY_PROGRAMID_INTEGRATION()
        {
            var operation = "rewards/GetPendingRewards?locale=en-US&ProgramId=" + Guid.Empty;
            var response = Common.SendRequest<WrapperResult<IEnumerable<CustomerReward>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetPendingRewards_NULL_LOCALE_INTEGRATION()
        {
            var operation = "rewards/GetPendingRewards?locale=&ProgramId=" + Guid.NewGuid();
            var response = Common.SendRequest<WrapperResult<IEnumerable<CustomerReward>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        private WrapperResult<IEnumerable<CustomerReward>> GetCustomerRewardMockup()
        {
            WrapperResult<IEnumerable<CustomerReward>> result = new WrapperResult<IEnumerable<CustomerReward>>();
            List<CustomerReward> list = new List<CustomerReward>();
            list.Add(new CustomerReward() { CustomerId = Guid.NewGuid(), FirstName = "Test 1" });
            list.Add(new CustomerReward() { CustomerId = Guid.NewGuid(), FirstName = "Test 2" });
            result.DataResult = list;

            return result;
        }

        private WrapperResult<IEnumerable<RewardHistory>> GetRedeemedRewardsMockup()
        {
            WrapperResult<IEnumerable<RewardHistory>> result = new WrapperResult<IEnumerable<RewardHistory>>();
            List<RewardHistory> list = new List<RewardHistory>();
            list.Add(new RewardHistory(){ CustomerId = Guid.NewGuid(), FirstName="Test 1" });
            list.Add(new RewardHistory() { CustomerId = Guid.NewGuid(), FirstName = "Test 2" });
            result.DataResult = list;

            return result;
        }

        private WrapperResult<IEnumerable<LoyaltyRewardModel>> GetRewardsMockup()
        {
            WrapperResult<IEnumerable<LoyaltyRewardModel>> result = new WrapperResult<IEnumerable<LoyaltyRewardModel>>();
            List<LoyaltyRewardModel> list = new List<LoyaltyRewardModel>();
            list.Add(new LoyaltyRewardModel {  RewardId = Guid.NewGuid(), Name="Test 1" });
            list.Add(new LoyaltyRewardModel { RewardId = Guid.NewGuid(), Name = "Test 2" });
            result.DataResult = list;

            return result;
        }


        private WrapperResult<IEnumerable<RewardsTierGroup>> GetRewardsGroupByTierMockup()
        {
            WrapperResult<IEnumerable<RewardsTierGroup>> result = new WrapperResult<IEnumerable<RewardsTierGroup>>();
            List<RewardsTierGroup> list = new List<RewardsTierGroup>();
            list.Add(new RewardsTierGroup {  Tier = 1 });
            list.Add(new RewardsTierGroup {  Tier = 2 });
            result.DataResult = list;

            return result;
        }

    }
}
