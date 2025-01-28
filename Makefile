add-migration:
	dotnet ef migrations add $(m) -p BankDemo.Infrastructure/BankDemo.Infrastructure.csproj -s BankDemo.Api/BankDemo.Api.csproj

drop-db:
	dotnet ef database drop -p BankDemo.Infrastructure/BankDemo.Infrastructure.csproj -s BankDemo.Api/BankDemo.Api.csproj

update-db:
	dotnet ef database update -p BankDemo.Infrastructure/BankDemo.Infrastructure.csproj -s BankDemo.Api/BankDemo.Api.csproj