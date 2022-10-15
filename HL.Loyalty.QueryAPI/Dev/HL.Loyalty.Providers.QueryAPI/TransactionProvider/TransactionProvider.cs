using HL.Loyalty.Common.QueryAPI;
using HL.Loyalty.Common.QueryAPI.Enums;
using HL.Loyalty.Models;
using HL.Loyalty.Repository.QueryAPI.TransactionRepository;
using System;

namespace HL.Loyalty.Providers.QueryAPI.TransactionProvider
{
    /// <summary>
    /// Provider for transaction order entity
    /// </summary>
    public class TransactionProvider : ITransactionProvider
    {
        private ITransactionRepository _transactionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionProvider"/> class.
        /// </summary>
        /// <param name="transactionRepository">Transaction repository instance</param>
        public TransactionProvider(ITransactionRepository transactionRepository)
        {
            if(transactionRepository == null)
                throw new ArgumentNullException(nameof(transactionRepository));

            _transactionRepository = transactionRepository;
        }

        /// <inheritcdoc />
        public WrapperResult<bool> GetTransactionOrder(string hmsReceiptID)
        {
            var wrapperResult = new WrapperResult<bool>();

            if (!hmsReceiptID.ValidateHMSReceiptID())
            {
                QueryAPIHelper.CreateError(wrapperResult, "HMSReceiptID must be provided", TraceTypes.Warning);
            }

            if (wrapperResult.Status == WrapperResultType.Ok)
            {
                wrapperResult = _transactionRepository.GetTransactionOrder(hmsReceiptID);
            }

            return wrapperResult;
        }

    }
}
