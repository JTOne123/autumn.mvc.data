node
{
	try
	{
		currentBuild.result = "SUCCESS"
		
		stage('Checkout'){
			checkout scm
		}
		
		stage('Compile'){
			sh 'dotnet restore src/Autumn.Data.Rest.sln'
		}
	}
	catch (err) 
	{
		currentBuild.result = "FAILURE"
		throw err
	}
}