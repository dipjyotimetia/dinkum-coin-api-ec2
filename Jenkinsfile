def buildVersion = ''

pipeline {
	agent{ dockerfile true}
	//agent any
	environment {
		DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
		DOTNET_CLI_TELEMETRY_OPTOUT = "1"
	}

	options { 
		//skipDefaultCheckout() 
		buildDiscarder(logRotator(numToKeepStr: '10', artifactNumToKeepStr: '10'))
		}

	stages {
		stage("Build") {
			steps {
				//deleteDir()
                checkout scm

				buildTarget "Export_Build_Version", "-BuildVersionFilePath \"${env.WORKSPACE}/version.txt\""
				script {
					currentBuild.displayName = readFile "${env.WORKSPACE}/version.txt"
					echo "reading build version"
                    buildVersion = readFile "${env.WORKSPACE}/version.txt"
				}
				buildTarget "Compile"
    			stash name: "solution", useDefaultExcludes: false
			}
		}				
		stage("Unit Test")  {
		steps {
		//		deleteDir()
		//		unstash "solution"
				buildTarget "Unit_Test", "-NoDeps"
		//		stash name: "solution", useDefaultExcludes: false
 			}
		}
		stage("Verify Pacts")  {
		steps {
		//		deleteDir()
		//		unstash "solution"
				buildTarget "Verify_Pacts", "-NoDeps"
		//		stash name: "solution", useDefaultExcludes: false
 			}
		}
		stage("Package / Upload")  {
		steps {
		//		deleteDir()
		//		unstash "solution"
				buildTarget "Package", "-NoDeps"
				buildTarget "Upload", "-NoDeps"
		//		stash name: "solution", useDefaultExcludes: false
 			}
		}	
		stage("Deploy DEV")  {
		steps {
		//		deleteDir()
		//		unstash "solution"
				buildTarget "Deploy", "-Account \"wgtpoc\" -Environment \"DEV\" -VersionToDeploy \"${buildVersion}\""
		//		stash name: "solution", useDefaultExcludes: false
 			}
		}		
	}
	post {
		always {
		//	deleteDir()
		//	unstash "solution" 
			step([$class: 'XUnitBuilder',
				thresholds: [[$class: 'FailedThreshold', unstableThreshold: '1']],
				tools: [[ $class: 'XUnitDotNetTestType', pattern: '**/TestResults.xml']]]
			)
		}	
	}
}
void buildTarget(String targetName, String parameters = "") {
	sh "dotnet run -p build -Target ${targetName} ${parameters}"

}
