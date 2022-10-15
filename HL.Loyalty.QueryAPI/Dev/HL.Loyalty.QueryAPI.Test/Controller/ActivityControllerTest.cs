using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loyalty.QueryAPI.Controllers;
using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI.Activities;
using NSubstitute;
using System.Collections.Generic;
using HL.Loyalty.Repository.QueryAPI.ActivityRepository;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace HL.Loyalty.QueryAPI.Test.Controllers
{
    [TestClass]
    public class ActivityControllerTest
    {
        private IActivityProvider provider;
        private ActivityController _controller;


        public ActivityControllerTest()
        {
            var repository = Substitute.For<IActivityRepository>();
            var activities = this.ActivityModels();
            var programActivities = this.ProgramActivities();

            repository.GetActivities(Arg.Any<string>()).Returns(activities);
            repository.GetActivitiesByProgram(Arg.Any<Guid>()).Returns(programActivities);
            provider = new ActivityProvider(repository);

            _controller = new ActivityController(provider);

        }

        [TestMethod]
        public void Get_AllActivities()
        {
            var response = _controller.Get("en-us");
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void Get_AllActivities_NULL_LOCALE()
        {
            var response = _controller.Get(null);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status== WrapperResultType.Error);
        }

        [TestMethod]
        public void Get_AllActivities_EMPTY_LOCALE()
        {
            var response = _controller.Get("");
            Assert.IsTrue(response.Status == WrapperResultType.Error);
        }


        [TestMethod]
        public void GetActivitiesByProgram()
        {
            var response = _controller.GetActivitiesByProgram(Guid.Parse("5196E9A6-AEAA-E611-80C3-00155DE1E513"));

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void GetGroupByTier_EMPTY_PROGRAMID()
        {
            var response = _controller.GetActivitiesByProgram(Guid.Empty);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Get_AllActivities_Integration()
        {
            var response = Common.SendRequest<WrapperResult<List<ActivityModel>>>("Activity?locale=en-US");

            Assert.IsNotNull(response);
            Assert.IsTrue(response.DataResult.Count > 0);
        }


        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Get_AllActivities_NULL_Locale_Integration()
       {
            var response = Common.SendRequest<WrapperResult<List<ActivityModel>>>("Activity?locale=");

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == WrapperResultType.Error);

        }


        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetActivitiesByProgram_Integration()
        {
            var operation = "Activity/GetActivitiesByProgram?ProgramId=" + Guid.NewGuid();
            var response = Common.SendRequest<WrapperResult<IEnumerable<ProgramActivity>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetActivitiesByProgram_EMPTY_PROGRAMID_Integration()
        {
            var operation = "Activity/GetActivitiesByProgram?ProgramId=" + Guid.Empty;
            var response = Common.SendRequest<WrapperResult<IEnumerable<ProgramActivity>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        

        private WrapperResult<List<ActivityModel>> ActivityModels()
        {
            var data = new WrapperResult<List<ActivityModel>>();
            data.DataResult = new List<ActivityModel>();
            data.DataResult.Add(new ActivityModel() { ActivityId = Guid.NewGuid() });
            data.DataResult.Add(new ActivityModel() { ActivityId = Guid.NewGuid() });
            return data;

        }

        private WrapperResult<IEnumerable<ProgramActivity>> ProgramActivities()
        {

            var dataProgramActivity = new WrapperResult<IEnumerable<ProgramActivity>>();
            var listProgramActivities = new List<ProgramActivity>();
            listProgramActivities.Add(new ProgramActivity() { ActivityId = Guid.NewGuid(), Title = "Test 1" });
            listProgramActivities.Add(new ProgramActivity() { ActivityId = Guid.NewGuid(), Title = "Test 2" });

            dataProgramActivity.DataResult = listProgramActivities;
            return dataProgramActivity;
        }


       


    }
}
