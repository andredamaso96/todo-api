dotnet ef migrations add UpdateEntities -s Todo.Api -p Todo.Infra -c TodoContext
dotnet ef database update -s Todo.Api -p Todo.Infra -c TodoContext