setlocal
set Timeout=12000

dotnet run -p "%~dp0Migrations.Attribution" migrate --tags Pre --singleTransaction -t %Timeout% --checktags
dotnet run -p "%~dp0Migrations.Attribution" migrate --tags Post --singleTransaction -t %Timeout% --checktags


pause