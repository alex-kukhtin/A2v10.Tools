-- catalog.agent (CRLF)
create or alter procedure test.[Agent.Index]
@UserId bigint
as
begin
	set nocount on;
	select [Agents!TAgent!Array] = null, [Id!!Id] = 1, [Name] = N'Agent';
end
go
