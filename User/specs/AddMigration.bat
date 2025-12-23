@echo off
set /p migration=Enter Migration Name:
if "%migration%"=="" (
	echo You didn't enter migration name!
)


cd ..\src\User.Domain.Data

if "%migration%"=="-" (
	echo Removing last migration!
	dotnet ef migrations remove -s ..\User.Api\User.Api.csproj -c UserDbContext
) else (
	dotnet ef migrations add %migration% -s ..\User.Api\User.Api.csproj -c UserDbContext -o Migrations
)

if %errorlevel% == 0 (
	timeout 5
) else (
	pause
)