using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loyalty.QueryAPI.Controllers;
using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI.ProgramProvider;
using HL.Loyalty.Repository.QueryAPI.ProgramRepository;
using NSubstitute;
using System.Configuration;

namespace HL.Loyalty.QueryAPI.Test.Controllers
{
    [TestClass]
    public class ProgramControllerTest
    {
        ProgramController _controller;

        public ProgramControllerTest()
        {

            IProgramRepository repository = Substitute.For<IProgramRepository>();
            repository.GetProgram(Arg.Any<string>(), Arg.Any<string>()).Returns(this.GetProgramMockup());
            IProgramProvider provider = new ProgramProvider(repository);
            _controller = new ProgramController(provider);
        }


        [TestMethod]
        public void GetProgram()
        {
            
            var response = _controller.Get("CGB102","en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void GetProgram_EMPTY_DISTRIBUTORID()
        {
            
            var response = _controller.Get("", "en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status,  WrapperResultType.Error);
        }


        [TestMethod]
        public void GetProgram_NULL_DISTRIBUTORID()
        {
            var response = _controller.Get(null, "en-US");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status,  WrapperResultType.Error);
        }


        [TestMethod]
        public void GetProgram_EMPTY_LOCALE()
        {
            var response = _controller.Get("CGB102", "");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }



        [TestMethod]
        public void GetProgram_NULL_LOCALE()
        {
            var response = _controller.Get("CGB102", null);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }



        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetProgram_Integration()
        {
            var operation = "Program?locale=en-US&DistributorId=staff";
            var response = Common.SendRequest<WrapperResult<ProgramModel>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }


        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetProgram_EMPTY_DISTRIBUTORID_Integration()
        {
            var operation = "Program?locale=en-US&DistributorId=";
            var response = Common.SendRequest<WrapperResult<ProgramModel>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetProgram_NULL_LOCALE_INTEGRATION()
        {
            var operation = "Program?locale=&DistributorId=staff";
            var response = Common.SendRequest<WrapperResult<ProgramModel>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }


        private WrapperResult<ProgramModel> GetProgramMockup()
        {
            WrapperResult<ProgramModel> result = new WrapperResult<ProgramModel>();
             result.DataResult = new ProgramModel();

            result.DataResult.ProgramId = Guid.NewGuid();
            result.DataResult.ProgramName = "Test";

            return result;
        }

    }
}
