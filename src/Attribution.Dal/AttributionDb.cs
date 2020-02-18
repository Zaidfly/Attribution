using Attribution.Configuration;
using LinqToDB.DataProvider.PostgreSQL;

namespace Attribution.Dal
{
    public partial class AttributionDb
    {
        public AttributionDb(DbConnectionString config)
            : base(new PostgreSQLDataProvider(PostgreSQLVersion.v95), config.Value)
        {
            InitDataContext();
        }
    }
}