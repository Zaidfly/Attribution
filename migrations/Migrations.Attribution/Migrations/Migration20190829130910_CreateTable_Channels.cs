using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    /// <summary>
    /// Каналы привлечения клиентов
    /// id - идентификатор
    /// owner - создатель канала
    /// description - описание
    /// contract - номер договора
    /// some_credentials - дополнительные данные
    /// </summary>
    [Migration(20190829130910)]
    [Tags("Pre")]
    public class Migration20190829130910_CreateTable_Channels : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@"-- noinspection SqlNoDataSourceInspectionForFile

				CREATE TABLE IF NOT EXISTS channels (
					id serial primary key,
					owner varchar(100) not null,
					contract varchar(80) null,
					price_terms varchar(1000) null,					
					partner_name varchar(100) null,
					contacts_email varchar(100) null,
					contacts_phone varchar(20) null,
					contacts_website varchar(100) null,
					description varchar(1000) null,
                    comments varchar(1000) null);"
            );
            
            Execute.Sql(@"-- noinspection SqlNoDataSourceInspectionForFile
				-- добавление дефолтного канала Direct
				insert into channels (owner, description) values('direct','не атрибуцированный канал, type in или отсутствует referrer или или referrer не в white list');"
            );
        }
    }
}