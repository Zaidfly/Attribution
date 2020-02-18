using EnvironmentInitializer;
using FluentMigrator;

namespace Attribution.Api.Tests.Migrations
{
    [TestMigration(5)]
    public class Migration005_ChannelAttributes_Add : ForwardOnlyMigration
    {
        public const long UtmParametersHash = 74846L;
        public const long UrlReferrerHash = 876546L;
        public const string UtmSource = "Int-source";
        public const string UtmMedium = "Int-medium";
        public const string UtmCampaign = "Int-campaign";
        
        private const string TableName = "channel_attributes";
        
        public override void Up()
        {
            Insert
                .IntoTable(TableName)
                .Row(new
                {
                    hash = UtmParametersHash, 
                    utm_source = UtmSource,
                    utm_medium = UtmMedium,
                    utm_campaign = UtmCampaign,
                    utm_term = "Int-term",
                    utm_partner_id = "Int-partner_id",
                    utm_campaign_id = "Int-campaign_id",
                    utm_ad_type = "Int-ad_type",
                    channel_id = Migration004_Channels_Add.Id
                });
            
            Insert
                .IntoTable(TableName)
                .Row(new
                {
                    hash = UrlReferrerHash, 
                    url_referrer_id = Migration001_UrlReferrers_Add.Id
                });
        }
    }
}