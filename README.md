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

Add-Migration InitialPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
Add-Migration InitialConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb

Update-Database -Context PersistedGrantDbContext
Update-Database -Context ConfigurationDbContext
OR
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb

dotnet ef database update




Add-Migration initial -c ApplicationDbContext -o Data/Migrations/AspNetIdentity
Update-Database -Context ApplicationDbContext
```
