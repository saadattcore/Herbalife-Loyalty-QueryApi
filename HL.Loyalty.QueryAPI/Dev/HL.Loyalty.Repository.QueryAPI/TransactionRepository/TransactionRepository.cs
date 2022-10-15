using System;
using HL.Loyalty.Models;
using HL.Loyalty.Common.QueryAPI;
using System.Data.SqlClient;
using HL.Loyalty.Common.QueryAPI.Enums;

namespace HL.Loyalty.Repository.QueryAPI.TransactionRepository
{
    /// <summary>
    /// Transaction Repository for transaction order
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private IConfigurationSettings _configurationSettings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
        /// </summary>
        /// <param name="configurationSettings">Configuration settings instance</param>
        public TransactionRepository(IConfigurationSettings configurationSettings)
        {
            if (configurationSettings == null)
                throw new ArgumentNullException(nameof(configurationSettings));

            _configurationSettings = configurationSettings;
        }

        /// <inheritdoc />
        public WrapperResult<bool> GetTransactionOrder(string hmsReceiptId)
        {
            var result = new WrapperResult<bool>()
            {
                DataResult = new bool()
            };
            using (var context = new Data.LoyaltyEntitiesDB(_configurationSettings.ConnectionString))
            {
                // Create a SQL command to execute the sproc
                var command = context.Database.Connection.CreateCommand();
                command.CommandText = "usp_GetTrnOrder";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@HMSReceiptID", hmsReceiptId));

                try
                {
                    context.Database.Connection.Open();
                    // Run the sproc 
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();

                        if (reader.IsDBNull(0))
                            return result;

                        result.DataResult = reader.HasRows;
                    }
                }
                catch (Exception ex)
                {
                    QueryAPIHelper.CreateError(result, ex.Message, TraceTypes.Error);
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return result;
        }
    }
}
