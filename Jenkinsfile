pipeline {
    agent any
    
    environment {
        // Используем 'docker compose' (новая версия, встроенная в Docker CLI)
        // вместо 'docker-compose' (старая версия, требует отдельной установки)
        DOCKER_COMPOSE = 'docker compose'
    }
    
    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out code from repository...'
                checkout scm
            }
        }
        
        stage('Check Docker') {
            steps {
                script {
                    echo 'Checking Docker availability...'
                    sh '''
                        if ! command -v docker &> /dev/null; then
                            echo "ERROR: Docker is not installed or not in PATH"
                            exit 1
                        fi
                        docker --version
                        docker info || echo "WARNING: Cannot connect to Docker daemon"
                    '''
                }
            }
        }
        
        stage('Build') {
            steps {
                script {
                    echo 'Building Docker images...'
                    // Используем 'docker compose' вместо 'docker-compose'
                    sh 'docker compose version || docker-compose version || echo "Trying docker compose..."'
                    sh 'docker compose build --no-cache || docker-compose build --no-cache'
                }
            }
        }
        
        stage('Deploy') {
            steps {
                script {
                    echo 'Stopping existing containers...'
                    sh 'docker compose down || docker-compose down || true'
                    
                    echo 'Starting services...'
                    sh 'docker compose up -d || docker-compose up -d'
                    
                    echo 'Waiting for services to be ready...'
                    sh 'sleep 15'
                }
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
                                echo "✅ ${service} is running"
                            else
                                echo "⚠️ WARNING: ${service} is not running"
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
                sh 'docker compose ps || docker-compose ps || docker ps'
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
            script {
                sh 'docker compose logs --tail=50 || docker-compose logs --tail=50 || echo "Cannot get logs"'
            }
        }
    }
}

