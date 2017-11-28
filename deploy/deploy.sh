echo 'test'

API_KEY=$1
SOURCE=$2
VERSION=$3
VERSION_PREFIX=$4

dotnet pack --no-build --version-suffix $VERSION_PREFIX --output nupkgs /p:PackageVersion=$VERSION ./src/Autumn.Mvc.Data/Autumn.Mvc.Data.csproj 
