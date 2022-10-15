using HL.Loyalty.Models;

namespace HL.Loyalty.Providers.QueryAPI.TransactionProvider
{
    public interface ITransactionProvider
    {
        /// <summary>
        /// Checks whether the given receipt has been processed for a given HMS Receipt Id or not
        /// </summary>
        /// <param name="hmsReceiptID">HMS Receipt Id</param>
        /// <returns>Boolean</returns>
        WrapperResult<bool> GetTransactionOrder(string hmsReceiptID);
    }
}
