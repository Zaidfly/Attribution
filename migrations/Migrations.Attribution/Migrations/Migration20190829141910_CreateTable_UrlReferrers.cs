using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    /// <summary>
    /// Белый список разрешенных UrlReferrer
    /// id - идентификатор
    /// url_referrer - url адрес хоста, с которого был перенаправлен запрос
    /// </summary>
    [Migration(20190829141910)]
    [Tags("Pre")]
    public class Migration20190829141910_CreateTable_UrlReferrers : ForwardOnlyMigration
    {
	    private const string TableName = "url_referrers";
	    
        public override void Up()
        {
            Execute.Sql($@"-- noinspection SqlNoDataSourceInspectionForFile

				CREATE TABLE IF NOT EXISTS {TableName} (
					id serial primary key,
					host varchar(200) not null,
					channel_id int4 null REFERENCES channels(id),
					is_deleted bool not null DEFAULT false);"
            );

            Execute.Sql($@"-- noinspection SqlNoDataSourceInspectionForFile
				
				CREATE UNIQUE INDEX IF NOT EXISTS 
					uix_{TableName}_url_referrer 
					ON {TableName} (host);");
        }
    }
}