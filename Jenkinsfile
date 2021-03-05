pipeline{
    agent any

    stages{
        stage('Clean'){
            steps{
                sh 'dotnet clean -c Release'
            }
            post{
                success{
                    echo "======== dotnet clean successfully ========"
                }
                failure{
                    echo "======== dotnet clean failed ========"
                }
            }
        }

        stage('Restore'){
            steps{
                sh 'dotnet restore'
            }
            post{
                success{
                    echo "======== dotnet restore successfully ========"
                }
                failure{
                    echo "======== dotnet restore failed ========"
                }
            }            
        }

        stage('Test'){
            steps{
                sh 'dotnet test --no-restore --verbosity normal'
            }
            post{
                success{
                    echo "======== dotnet test successfully ========"
                }
                failure{
                    echo "======== dotnet test failed ========"
                }
            }            
        }

        stage("Docker Build"){
            steps{
                sh 'docker build -f ./WebClientConsul/Dockerfile -t 10.0.18.30:8082/compuletra.webclient:$BUILD_NUMBER .'
                sh 'docker build -f ./WebClientConsul/Dockerfile -t 10.0.18.30:8082/compuletra.webclient:latest .'
            }
            post{
                success{
                    echo "======== Docker build successfully ========"
                }
                failure{
                    echo "======== docker Build failed ========"
                }
            } 
        }

        stage("Docker Push"){
            steps{
                sh 'docker login -u jenkins -p jenkins 10.0.18.30:8082/docker-hosted'
                sh 'docker push 10.0.18.30:8082/compuletra.webclient:$BUILD_NUMBER'
                sh 'docker push 10.0.18.30:8082/compuletra.webclient:latest'
            }
            post{
                success{
                    echo "======== docker Push successfully ========"
                }
                failure{
                    echo "======== docker Push failed ========"
                }
            }             
        }

        stage("Docker Compose"){
            steps{
                    sh 'docker-compose up -d'
            }
            post{
                success{
                    echo "======== docker Compose successfully ========"
                }
                failure{
                    echo "======== docker Compose failed ========"
                }
            }              
        }

        stage("Docker Clean"){
            steps{
                sh 'docker image prune -f'
                sh 'docker container prune -f'
                sh 'docker volume prune -f'
            }
            post{
                success{
                    echo "======== docker Clean successfully ========"
                }
                failure{
                    echo "======== docker Clean failed ========"
                }
            }             
        }        
    }
    post{
        success{
            echo "======== pipeline successfully ========"
        }
        failure{
            echo "======== pipeline failed ========"
        }
    }
}
