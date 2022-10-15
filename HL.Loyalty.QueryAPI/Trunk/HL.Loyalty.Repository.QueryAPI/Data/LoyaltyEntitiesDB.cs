using System.Data.Entity;

namespace HL.Loyalty.Repository.QueryAPI.Data
{
    public partial class LoyaltyEntitiesDB : LoyaltyEntities
    {
        public LoyaltyEntitiesDB(string connectionString)
        {
            if(!string.IsNullOrWhiteSpace(connectionString))
                this.Database.Connection.ConnectionString = connectionString;
        }
    }
}
