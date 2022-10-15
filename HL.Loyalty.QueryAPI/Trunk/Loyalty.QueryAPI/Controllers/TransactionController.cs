using HL.Loyalty.Models;
using HL.Loyalty.Providers.QueryAPI.TransactionProvider;
using System.Web.Http;

namespace Loyalty.QueryAPI.Controllers
{
    /// <summary>
    /// This API is used to get the transaction order details
    /// </summary>
    [ServiceRequestActionFilter]
    public class TransactionController :  ApiController
    {
        private readonly ITransactionProvider _transactionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionController"/> class.
        /// </summary>
        /// <param name="transactionProvider">Transaction provider instance</param>
        public TransactionController(ITransactionProvider transactionProvider)
        {
            _transactionProvider = transactionProvider;
        }

        /// <inheritdoc />
        public WrapperResult<bool> GetTransactionOrder(string hmsReceiptID)
        {
            WrapperResult<bool> wrapperResult = _transactionProvider.GetTransactionOrder(hmsReceiptID);
            return wrapperResult;
        }
    }
}
