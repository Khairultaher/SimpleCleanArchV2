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
