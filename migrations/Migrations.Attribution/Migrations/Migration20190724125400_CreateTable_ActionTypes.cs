using System;
using System.Linq;
using Attribution.Domain;
using Attribution.Domain.Models;
using FluentMigrator;

namespace Migration.Attribution.Migrations
{
	/// <summary>
	/// Справочник типов действий пользователя
	/// id - уникальный идентификатор
	/// name - имя действия
	/// description - произвольное описание, комментарий
	/// </summary>
    [Migration(20190724125400)]
    [Tags("Pre")]
    public class Migration20190724125400_CreateTable_ActionTypes : ForwardOnlyMigration
    {
        private const string TableName = "action_types";
        public override void Up()
        {
            Execute.Sql($@"-- noinspection SqlNoDataSourceInspectionForFile

				CREATE TABLE IF NOT EXISTS {TableName} (
					id int4 primary key,
					name varchar(100) not null,
					description varchar(500) null);");
	        
            Execute.Sql($@"-- noinspection SqlNoDataSourceInspectionForFile
				
				CREATE UNIQUE INDEX IF NOT EXISTS 
					uix_{TableName}_name 
					ON {TableName} (name);");
            
            Execute.Sql($@"
					
					INSERT INTO {TableName} (id, name, description) 
                        VALUES {GetActionTypes()}
                        ON CONFLICT (id) DO NOTHING;");
        }

        private static string GetActionTypes()
        {
	        var actionTypes = (UserActionType[])Enum.GetValues(typeof(UserActionType));
	        var insertValues = actionTypes.Select(at => $"({(int)at}, '{at.ToString()}', null)");

	        return string.Join(",", insertValues);
        }
    }
}