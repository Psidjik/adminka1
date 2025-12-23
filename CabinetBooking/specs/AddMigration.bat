@echo off
set /p migration=Enter Migration Name:
if "%migration%"=="" (
	echo You didn't enter migration name!
)


cd ..\src\CabinetBooking.Domain.Data

if "%migration%"=="-" (
	echo Removing last migration!
	dotnet ef migrations remove -s ..\CabinetBooking.Api\CabinetBooking.Api.csproj -c CabinetBookingDbContext
) else (
	dotnet ef migrations add %migration% -s ..\CabinetBooking.Api\CabinetBooking.Api.csproj -c CabinetBookingDbContext -o Migrations
)

if %errorlevel% == 0 (
	timeout 5
) else (
	pause
)