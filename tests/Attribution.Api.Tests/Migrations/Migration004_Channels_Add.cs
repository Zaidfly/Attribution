using EnvironmentInitializer;
using FluentMigrator;
using FluentMigrator.SqlServer;

namespace Attribution.Api.Tests.Migrations
{
    [TestMigration(4)]
    public class Migration004_Channels_Add : ForwardOnlyMigration
    {
        public const int Id = 120;
        public const string Name = "Int-name";
        public const string Owner = "Int-owner";
        public const string Contract = "Int-contract";
        public const string PriceTerms = "Int-price_terms";
        public const string PartnerName = "Int-partner_name";
        public const string ContactsEmail = "Int-contacts_email";
        public const string ContactsPhone = "Int-contacts_phone";
        public const string ContactsWebsite = "Int-contacts_website";
        public const string Description = "Int-description";
        public const string Comments = "Int-comments";
            
        private const string TableName = "channels";

        public override void Up()
        {
            Insert
                .IntoTable(TableName)
                .Row(new
                {
                    id = Id,
                    name = Name,
                    owner = Owner,
                    contract = Contract,
                    price_terms = PriceTerms,
                    partner_name = PartnerName,
                    contacts_email = ContactsEmail,
                    contacts_phone = ContactsPhone,
                    contacts_website = ContactsWebsite,
                    description = Description,
                    comments = Comments
                }).WithIdentityInsert();
        }
    }
}