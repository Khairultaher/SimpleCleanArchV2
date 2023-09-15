# SimpleCleanArchV2
Simple CleanArch V2

# EF Migrations
```
dotnet ef migrations add "initial" --project Source\SimpleCleanArch.Infrastructure --startup-project Source\SimpleCleanArch.API --output-dir Persistence\Migrations
dotnet ef database update --project Source\SimpleCleanArch.Infrastructure --startup-project Source\SimpleCleanArch.API
```

## React
```
Use $Env:BROWSER="none". Now running npm start will not open a new browser window/tab.
```

## Migrations
```
Scaffold-DbContext "Server=.;Database=dbname;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities
Scaffold-DbContext "Server=.;Database=dbname;user=sa;Password=SA@12345;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities


Update-Database -Context PersistedGrantDbContext
Update-Database -Context ConfigurationDbContext
OR
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb

dotnet ef database update




Add-Migration initial -c ApplicationDbContext -o Data/Migrations/AspNetIdentity
Update-Database -Context ApplicationDbContext



remove-migration -Context ApplicationWriteDbContext
add-migration v1_0 -Context ApplicationWriteDbContext -o Migrations
Update-Database -Context ApplicationWriteDbContext
script-migration v1_3 v1_4 -c ApplicationDbContext -o source/Services/WeatherForecast/WeatherForecast.Infrastucture/Migrations/Scripts/v1.4.0.sql -i

```
