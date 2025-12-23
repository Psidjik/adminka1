@echo off
cd ..
dotnet ef database update --project src\User.Domain.Data\User.Domain.Data.csproj --startup-project src\User.Api\User.Api.csproj --context UserDbContext
pause