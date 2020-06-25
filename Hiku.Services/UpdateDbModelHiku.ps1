$dbContext = "HikuDbContext"

write-host "Regénération des fichiers générés dans DbModels"
dotnet ef dbcontext scaffold 'Server=tcp:MyHikusql.database.windows.net,1433;Initial Catalog=hikudb;Persist Security Info=False;User ID=MyHikuadmin;Password=xxxxxxxxxxxxxxxxxxx;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;' "Microsoft.EntityFrameworkCore.SqlServer" --context-dir "Infrastructure/Repositories/Sql" -o "Models" -f -c $dbContext --framework "netcoreapp3.1" --no-build

#write-host "Annulation des modifications de Hiku.Services/Infrastructure/Repositories/SQL/Models/HikuDbContext.cs"
$contextPath = (Get-ChildItem "Infrastructure\\Repositories\\Sql\\$dbContext.cs").FullName;
$contextContent = [IO.File]::ReadAllText($contextPath , [System.Text.Encoding]::UTF8);
$correctContextContent = $contextContent -replace 'protected override void OnConfiguring(.|\n|\r)*?}(.|\n|\r)*?}', ""
$correctContextContent | Out-File $contextPath  -Encoding UTF8