node
{
	try
	{
	
		currentBuild.result = "SUCCESS"
		
		stage('Checkout'){
			checkout scm
		}
		
		stage('Build'){
			sh 'dotnet restore src/Autumn.Mvc.Data.sln'
			sh 'dotnet build --configuration Release src/Autumn.Mvc.Data.sln'
		}
		
		stage('Unit Tests') {
			sh 'dotnet test --configuration Release --no-build src/Autumn.Mvc.Data.Tests/Autumn.Mvc.Data.Tests.csproj'
		}
		
		if (env.BRANCH_NAME=='master' || env.BRANCH_NAME=='staging') {
			
			stage('Package') 
			{
				sh 'dotnet pack --no-build --output nupkgs src/Autumn.Mvc.Data/Autumn.Mvc.Data.csproj'
				sh 'dotnet pack --no-build --output nupkgs src/Autumn.Mvc.Data.MongoDB/Autumn.Mvc.Data.MongoDB.csproj'
			}
			
			stage('Publish')
			{
				
			}
		}		
	}
	catch (err) 
	{
		currentBuild.result = "FAILURE"
		throw err
	}
}
