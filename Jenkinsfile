node
{
	try
	{
		currentBuild.result = "SUCCESS"
		
		stage('Checkout'){
			checkout scm
		}
		
		stage('Build'){
			sh 'dotnet restore src/Autumn.Data.Rest.sln'
			sh 'dotnet build --configuration Release src/Autumn.Data.Rest.sln'
		}
		
		stage('Unit Tests') {
			sh 'dotnet test --configuration Release --no-build src/Autumn.Data.Rest.Tests/Autumn.Data.Rest.Tests.csproj'
		}
	}
	catch (err) 
	{
		currentBuild.result = "FAILURE"
		throw err
	}
}