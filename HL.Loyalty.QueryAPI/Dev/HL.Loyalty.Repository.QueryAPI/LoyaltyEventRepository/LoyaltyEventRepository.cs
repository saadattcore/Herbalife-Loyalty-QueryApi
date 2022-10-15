using HL.Loyalty.Common.QueryAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL.Loyalty.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using HL.Loyalty.Common.QueryAPI.Enums;
using System.Data;

namespace HL.Loyalty.Repository.QueryAPI.LoyaltyEventRepository
{
    public class LoyaltyEventRepository : ILoyaltyEventRepository
    {
        private IConfigurationSettings _config { get; set; }

        public LoyaltyEventRepository(IConfigurationSettings config)
        {
            this._config = config;
        }

        public WrapperResult<LoyaltyCustomerEventsFeed> GetCustomerLoyaltyEventHistory(Guid CustomerID)
        {
            WrapperResult<LoyaltyCustomerEventsFeed> result = new WrapperResult<LoyaltyCustomerEventsFeed>();

            using (var context = new Data.LoyaltyEntitiesDB(this._config.ConnectionString))
            {
                // Create a SQL command to execute the sproc

                var command = context.Database.Connection.CreateCommand() as SqlCommand;
                command.CommandText = "usp_GetCustomerLoyaltyEventHistory";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@CustomerID", CustomerID));                
                try
                {
                    LoyaltyCustomerEventsFeed customerFeed = new LoyaltyCustomerEventsFeed();

                    DataSet dataSet = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0) // Populate program related events 
                    {
                        customerFeed.ProgramFeeds = new List<CustomerProgramEventsFeed>();

                        var dataTable = dataSet.Tables[0];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var programFeed = new CustomerProgramEventsFeed();

                            programFeed.CustomerStatus = row["CustomerStatus"] as string;
                            programFeed.StartDate = Convert.ToDateTime(row["StartDate"]);
                            programFeed.EndDate = Convert.ToDateTime(row["EndDate"]);
                            programFeed.EventDate = Convert.ToDateTime(row["EventDate"]);

                            customerFeed.ProgramFeeds.Add(programFeed);
                        }
                    }

                    if (dataSet.Tables.Count > 1)
                    {
                        customerFeed.PointsFeeds = new List<CustomerPointsEventsFeed>();

                        var dataTable = dataSet.Tables[1];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var pointsFeed = new CustomerPointsEventsFeed();

                            pointsFeed.EventDate = Convert.ToDateTime(row["EventDate"]);
                            pointsFeed.Points = row["Points"] != DBNull.Value ? (decimal?)row["Points"] : null;
                            pointsFeed.ActityPoints = row["ActivityPoints"] != DBNull.Value ? (decimal)row["ActivityPoints"] : 0;
                            pointsFeed.Tier = row["Tier"] != DBNull.Value ? (int?)row["Tier"] : null;
                            pointsFeed.Category = row["CategoryCode"] as string;
                            pointsFeed.TransactionType = row["TrnType"] as string;                           
                            pointsFeed.ActivityName = row["ActivityTitle"] != DBNull.Value ? row["ActivityTitle"] as string : null;
                            pointsFeed.ActivityDescription = row["Description"] != DBNull.Value ? row["Description"] as string : null;
                            pointsFeed.ReceiptNumber = row["ReceiptID"] != DBNull.Value ? row["ReceiptID"] as string : null;
                            pointsFeed.OrderNumber = row["OrderNumber"] != DBNull.Value ? row["OrderNumber"] as string : null;
                            pointsFeed.VolumePoints = row["VolumePoints"] != DBNull.Value ? (decimal)row["VolumePoints"]  : 0;
                            pointsFeed.TrnHeaderID = row["TrnHeaderID"] != DBNull.Value ? (Guid)row["TrnHeaderID"] : Guid.Empty;

                            customerFeed.PointsFeeds.Add(pointsFeed);
                        }
                    }

                    if (dataSet.Tables.Count > 2)
                    {
                        customerFeed.RewardFeeds = new List<CustomerRewardEventsFeed>();

                        var dataTable = dataSet.Tables[2];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var rewardFeed = new CustomerRewardEventsFeed();

                            rewardFeed.EventDate = Convert.ToDateTime(row["EventDate"]);
                            rewardFeed.RewardName = row["RewardName"] as string;
                            rewardFeed.Tier = row["Tier"] != DBNull.Value ? (int)row["Tier"] : 0;
                            rewardFeed.Category = row["CategoryCode"] as string;
                            rewardFeed.RedemptionStatus = row["RedemptionStatus"] as string;

                            customerFeed.RewardFeeds.Add(rewardFeed);
                        }
                    }

                    if (dataSet.Tables.Count > 3)
                    {
                        customerFeed.WishListFeeds = new List<CustomerWishListEventsFeed>();

                        var dataTable = dataSet.Tables[3];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var wishListFeed = new CustomerWishListEventsFeed();

                            wishListFeed.EventDate = Convert.ToDateTime(row["EventDate"]);                       
                            wishListFeed.Category = row["CategoryCode"] as string;
                            wishListFeed.Sku = row["Sku"] as string;
                            wishListFeed.Tier = row["Tier"] != DBNull.Value ? (int)row["Tier"] : 0;
                            wishListFeed.CountryCode = row["CountryCode"] as string;

                            customerFeed.WishListFeeds.Add(wishListFeed);
                        }
                    }

                    result.DataResult = customerFeed;
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
