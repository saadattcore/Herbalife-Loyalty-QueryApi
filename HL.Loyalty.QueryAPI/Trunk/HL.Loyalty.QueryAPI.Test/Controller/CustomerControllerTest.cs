using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loyalty.QueryAPI.Controllers;
using HL.Loyalty.Providers.QueryAPI.CustomerProvider;
using HL.Loyalty.Models;
using NSubstitute;
using HL.Loyalty.Repository.QueryAPI.CustomerRepository;
using System.Collections.Generic;
using System.Configuration;

namespace HL.Loyalty.QueryAPI.Test.Controllers
{
    [TestClass]
    public class CustomerControllerTest
    {
        private CustomerController _controller;


        public CustomerControllerTest()
        {
            var repository = Substitute.For<ICustomerRepository>();

            repository.GetDashboard(Arg.Any<string>(),Arg.Any<Nullable<Guid>>(), Arg.Any<Nullable<Guid>>(),Arg.Any<string>()).Returns(this.GetCustomerDashboarMockup());
            repository.GetEnrolled(Arg.Any<Guid>()).Returns(this.GetCustomerModelsMockup());

            CustomerProvider provider = new CustomerProvider(repository);
            _controller = new CustomerController(provider);
        }


        [TestMethod]
        public void Get_Enrolled()
        {
            var response = _controller.GetEnrolled(Guid.Parse("7C5C7A07-F903-499E-B97D-5344F981D54F"));

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void GetEnrolled_EMPTY_PROGRAMID()
        {
            var response = _controller.GetEnrolled(Guid.Empty);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status,  WrapperResultType.Error);
        }

        
        [TestMethod]
        public void Get_GetDashboard()
        {

            var response = _controller.Get("en-US", Guid.Parse("7D729953-AFAA-E611-80C3-00155DE1E513"));

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        public void Get_GetDashboard_EMPTY_LOCALE()
        {

            var response = _controller.Get("", Guid.Parse("7D729953-AFAA-E611-80C3-00155DE1E513"));

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }



        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Get_Enrolled_Integration()
        {
            var operation = "Customer/GetEnrolled?ProgramId=" + Guid.NewGuid();
            var response = Common.SendRequest<WrapperResult<IEnumerable<CustomerModel>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void GetEnrolled_EMPTY_PROGRAMID_Integration()
        {
            var operation = "Customer/GetEnrolled?ProgramId=" + Guid.Empty;
            var response = Common.SendRequest<WrapperResult<IEnumerable<CustomerModel>>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Get_GetDashboard_Integration()
        {
            var operation = "customer?locale=en-US&CustomerId=" + Guid.NewGuid();
            var response = Common.SendRequest<WrapperResult<CustomerDetailModel>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Get_GetDashboard_EMPTY_LOCALE_Integration()
        {
            var operation = "customer?locale=&CustomerId=" + Guid.NewGuid();
            var response = Common.SendRequest<WrapperResult<CustomerDetailModel>>(operation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Error);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void ValidateCustomerForOLC()
        {
            var operation = "customer/ValidateCustomerForOLC?DistributorId=09513828&email=carloasdfsqatestdo@herbalife.com";
            var response = Common.SendRequest<bool>(operation);

            Assert.IsNotNull(response);
        }


        private WrapperResult<CustomerDetailModel> GetCustomerDashboarMockup()
        {
            WrapperResult<CustomerDetailModel> result = new WrapperResult<CustomerDetailModel>();

            CustomerDetailModel cd = new CustomerDetailModel();
            cd.ProgramId = new Guid();
            result.DataResult = cd;
            return result;

        }

        private WrapperResult<IEnumerable<CustomerModel>> GetCustomerModelsMockup()
        {
            WrapperResult<IEnumerable<CustomerModel>> result = new WrapperResult<IEnumerable<CustomerModel>>();
            List<CustomerModel> list = new List<CustomerModel>();
            list.Add(new CustomerModel() { Id = new Guid(), FirstName = "Test 1" });
            list.Add(new CustomerModel() { Id = new Guid(), FirstName = "Test 2" });
            result.DataResult = list;
            return result;

        }

    }
}
