pipeline {
	agent  { label 'win' } 
	
	environment {
		DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
		DOTNET_CLI_TELEMETRY_OPTOUT = "1"
	}

	stages {
	
	
		stage("Static Analysis") {
			steps {
				def sqScannerMsBuildHome = tool 'SonarQube Scanner for MSBuild'
				withSonarQubeEnv('My SonarQube Server') {
				  // Due to SONARMSBRU-307 value of sonar.host.url and credentials should be passed on command line
				  bat "${sqScannerMsBuildHome}\\SonarQube.Scanner.MSBuild.exe begin /k:myKey /n:myName /v:1.0 /d:sonar.host.url=%SONAR_HOST_URL% /d:sonar.login=%SONAR_AUTH_TOKEN%"
				  bat 'dotnet clean'
				  bat 'dotnet build'
				  bat "${sqScannerMsBuildHome}\\SonarQube.Scanner.MSBuild.exe end"

				}
			}
		}
		
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

		stage("Code Coverage") {
			steps {
				deleteDir()
				unstash "solution"

				buildTarget "Code_Coverage", "-NoDeps"
				
				stash name: "solution", useDefaultExcludes: false
			}
		}

		
		stage("Publish NuGet package") {
			when { branch "master" }
			steps {
				deleteDir()
				unstash "solution"

				buildTarget "Package", "-NoDeps"
				buildTarget "Publish", "-NoDeps"

			}
		}
	}
	post {
		always {
			deleteDir()
			unstash "solution" 
			
			step([$class: 'XUnitBuilder',
			thresholds: [[$class: 'FailedThreshold', unstableThreshold: '1']],
			tools: [[ $class: 'XUnitDotNetTestType', pattern: '**/TestResults.xml']]])
			
	
	}
	}
}
void buildTarget(String targetName, String parameters = "") {
	bat "dotnet run -p Build/Build.csproj -Target ${targetName} ${parameters}"

}
