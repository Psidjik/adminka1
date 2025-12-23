pipeline {
    agent any
    
    environment {
        DOCKER_COMPOSE = 'docker-compose'
    }
    
    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out code from repository...'
                checkout scm
            }
        }
        
        stage('Build') {
            steps {
                echo 'Building Docker images...'
                sh 'docker-compose build --no-cache'
            }
        }
        
        stage('Deploy') {
            steps {
                echo 'Stopping existing containers...'
                sh 'docker-compose down || true'
                
                echo 'Starting services...'
                sh 'docker-compose up -d'
                
                echo 'Waiting for services to be ready...'
                sh 'sleep 10'
            }
        }
        
        stage('Health Check') {
            steps {
                echo 'Checking service health...'
                script {
                    def services = ['api-gateway', 'cabinet-booking', 'user', 'prometheus', 'grafana']
                    services.each { service ->
                        sh """
                            if docker ps | grep -q ${service}; then
                                echo "${service} is running"
                            else
                                echo "WARNING: ${service} is not running"
                            fi
                        """
                    }
                }
            }
        }
    }
    
    post {
        always {
            echo 'Pipeline execution completed'
            script {
                sh 'docker-compose ps'
            }
        }
        success {
            echo '✅ Deployment successful!'
            echo 'Services are available at:'
            echo '  - API Gateway: http://localhost:8080'
            echo '  - Prometheus: http://localhost:9090'
            echo '  - Grafana: http://localhost:3000'
        }
        failure {
            echo '❌ Deployment failed!'
            sh 'docker-compose logs --tail=50'
        }
    }
}

