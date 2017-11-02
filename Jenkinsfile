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
			sh 'dotnet build src/Autumn.Data.Rest.sln'
		}
		
		stage('Test') {
			sh 'dotnet test src/Autumn.Data.Rest.Tests/Autumn.Data.Rest.Tests.csproj'
		}
	}
	catch (err) 
	{
		currentBuild.result = "FAILURE"
		throw err
	}
}