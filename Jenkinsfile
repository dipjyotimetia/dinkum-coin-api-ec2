pipeline {
	//agent  { label 'win' } 	
	agent any
	environment {
		DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
		DOTNET_CLI_TELEMETRY_OPTOUT = "1"
	}
	stages {
		stage("Build") {
			steps {
				buildTarget "Export_Build_Version", "-BuildVersionFilePath \"${env.WORKSPACE}/version.txt\""
				script {
					currentBuild.displayName = readFile "${env.WORKSPACE}/version.txt"
				}
				buildTarget "Compile", "-NoDeps"
    			stash name: "solution", useDefaultExcludes: false
			}
		}				
		stage("Unit test")  {
		steps {
				deleteDir()
				unstash "solution"
				buildTarget "UnitTest", "-NoDeps"
				stash name: "solution", useDefaultExcludes: false
 			}
		}
	}
	post {
		always {
			deleteDir()
			unstash "solution" 
			step([$class: 'XUnitBuilder',
				thresholds: [[$class: 'FailedThreshold', unstableThreshold: '1']],
				tools: [[ $class: 'XUnitDotNetTestType', pattern: '**/TestResults.xml']]]
			)
		}	
	}
}
void buildTarget(String targetName, String parameters = "") {
	bat "dotnet run -p Build/Build.csproj -Target ${targetName} ${parameters}"

}
