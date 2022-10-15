using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI.TransactionProvider;
using HL.Loyalty.Repository.QueryAPI.TransactionRepository;
using Loyalty.QueryAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HL.Loyalty.QueryAPI.Test.Controller
{
    [TestClass]
    public class TransactionControllerTest
    {
        private ITransactionProvider provider;
        private TransactionController _controller;

        public TransactionControllerTest()
        {
            var result = this.TransactionOrderResult();

            var repository = Substitute.For<ITransactionRepository>();
            repository.GetTransactionOrder("435be906-c6ba-d739-c93b-afc76d4d56a0").Returns((WrapperResult<bool>)result);

            provider = new TransactionProvider(repository);
            _controller = new TransactionController(provider);

        }

        [TestMethod]
        public void GetTransactionOrder()
        {
            //Arrange
 
            //Act
            var response = _controller.GetTransactionOrder("435be906-c6ba-d739-c93b-afc76d4d56a0");

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, WrapperResultType.Ok);

        }

        public WrapperResult<bool> TransactionOrderResult()
        {
            WrapperResult<bool> result = new WrapperResult<bool>();
            result.DataResult = new bool();

            result.DataResult = true;
            return result;
        }

    }

}
