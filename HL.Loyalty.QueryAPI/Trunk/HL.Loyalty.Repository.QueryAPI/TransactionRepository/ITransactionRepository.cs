using HL.Loyalty.Models;

namespace HL.Loyalty.Repository.QueryAPI.TransactionRepository
{
    /// <summary>
    /// Transaction Repository for transaction order
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Checks whether the given receipt has been processed for a given HMS Receipt Id or not
        /// </summary>
        /// <param name="hmsReceiptId">HMS Receipt Id</param>
        /// <returns>Boolean</returns>
        WrapperResult<bool> GetTransactionOrder(string hmsReceiptId);
    }
}
