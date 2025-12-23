@echo off
cd ..
dotnet ef database update --project src\CabinetBooking.Domain.Data\CabinetBooking.Domain.Data.csproj --startup-project src\CabinetBooking.Api\CabinetBooking.Api.csproj --context CabinetBookingDbContext
pause