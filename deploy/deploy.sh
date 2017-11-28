
API_KEY=$1
SOURCE=$2
VERSION=$3

echo 'nuget packaging'
dotnet pack --configuration Release --output ../../nupkgs /p:PackageVersion=$VERSION ./src/Autumn.Mvc.Data/Autumn.Mvc.Data.csproj
dotnet pack --configuration Release --output ../../nupkgs /p:PackageVersion=$VERSION ./src/Autumn.Mvc.Data.MongoDB/Autumn.Mvc.Data.MongoDB.csproj
dotnet pack --configuration Release --output ../../nupkgs /p:PackageVersion=$VERSION ./src/Autumn.Mvc.Data.EF.SqlServer/Autumn.Mvc.Data.EF.SqlServer.csproj
dotnet pack --configuration Release --output ../../nupkgs /p:PackageVersion=$VERSION ./src/Autumn.Mvc.Data.EF.Mysql/Autumn.Mvc.Data.EF.Mysql.csproj
dotnet pack --configuration Release --output ../../nupkgs /p:PackageVersion=$VERSION ./src/Autumn.Mvc.Data.EF.Sqlite/Autumn.Mvc.Data.EF.Sqlite.csproj
dotnet pack --configuration Release --output ../../nupkgs /p:PackageVersion=$VERSION ./src/Autumn.Mvc.Data.EF.Npgsql/Autumn.Mvc.Data.EF.Npgsql.csproj

echo 'nuget push'
dotnet nuget push ./nupkgs/Autumn.Mvc.Data.$VERSION.nupkg -k $API_KEY -s $SOURCE
dotnet nuget push ./nupkgs/Autumn.Mvc.Data.MongoDB.$VERSION.nupkg -k $API_KEY -s $SOURCE
dotnet nuget push ./nupkgs/Autumn.Mvc.Data.EF.SqlServer.$VERSION.nupkg -k $API_KEY -s $SOURCE
dotnet nuget push ./nupkgs/Autumn.Mvc.Data.EF.Mysql.$VERSION.nupkg -k $API_KEY -s $SOURCE
dotnet nuget push ./nupkgs/Autumn.Mvc.Data.EF.Sqlite.$VERSION.nupkg -k $API_KEY -s $SOURCE
dotnet nuget push ./nupkgs/Autumn.Mvc.Data.EF.Npgsql.$VERSION.nupkg -k $API_KEY -s $SOURCE
