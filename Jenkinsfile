node
{
	try
	{
	
		currentBuild.result = "SUCCESS"
		
		stage('Checkout'){
			checkout scm
		}
		
		stage('Build'){
			sh 'dotnet restore src/Autumn.Data.Mvc.sln'
			sh 'dotnet build --configuration Release src/Autumn.Data.Mvc.sln'
		}
		
		stage('Unit Tests') {
			sh 'dotnet test --configuration Release --no-build src/Autumn.Data.Mvc.Tests/Autumn.Data.Mvc.Tests.csproj'
		}
		
		if (env.BRANCH_NAME=='master' || env.BRANCH_NAME=='staging') {
			
			stage('Package') 
			{
				sh 'dotnet pack --no-build --output nupkgs src/Autumn.Data.Mvc/Autumn.Data.Mvc.csproj'
				sh 'dotnet pack --no-build --output nupkgs src/Autumn.Data.Mvc.MongoDB/Autumn.Data.Mvc.MongoDB.csproj'
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
