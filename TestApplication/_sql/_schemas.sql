-- schemas (LF)
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME = N'test')
	exec sp_executesql N'create schema test authorization dbo';
go
