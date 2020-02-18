using FluentMigrator;

namespace Migration.Attribution.Migrations
{
	/// <summary>
    /// Атрибуты канала привлечения
    /// id - идентификатор
    /// hash - хэш набора utm-меток
    /// utm_* - метки аттрибуции
    /// channel_id - идентификатор канала привлечения
    /// url_referrer - содержимое http-заголовка Referrer (только host, без протокола, адреса и параметров запроса)
    /// unparsed_data - json c набором utm-меток, которые не распарсились по колонкам
	/// </summary>
    [Migration(20190829150910)]
    [Tags("Pre")]
    public class Migration20190829150910_CreateTable_ChannelAttributes : ForwardOnlyMigration
	{
		private const string TableName = "channel_attributes";
	    public override void Up()
	    {
		    Execute.Sql($@"-- noinspection SqlNoDataSourceInspectionForFile

				CREATE TABLE IF NOT EXISTS {TableName} (
					id bigserial primary key,
					hash int8 not null,
					utm_source varchar(500) null,
					utm_medium varchar(500) null,
					utm_campaign varchar(500) null,
					utm_term varchar(500) null,
					utm_content varchar(500) null,
					utm_agency varchar(500) null,
					utm_partner_id varchar(500) null,
					utm_campaign_id varchar(500) null,
					utm_ad_type varchar(500) null,
					channel_id int4 null REFERENCES channels(id),
					url_referrer_id int4 null REFERENCES url_referrers(id),
					unparsed_data text null);"
		    );
		    
		    Execute.Sql($@"-- noinspection SqlNoDataSourceInspectionForFile
				
				CREATE UNIQUE INDEX IF NOT EXISTS 
					uix_{TableName}_hash 
					ON {TableName} (hash);");
		    
		    Execute.Sql(@"-- noinspection SqlNoDataSourceInspectionForFile
				-- добавление дефолтных аттрибутов канала Direct
				INSERT INTO channel_attributes (hash, channel_id) VALUES(0, 1);"
		    );
	    }
    }
}