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
        
        stage('Build') {
            steps {
                script {
                    echo 'Building Docker images...'
                    // Используем docker-compose (установлен в контейнер)
                    sh 'docker-compose --version || echo "docker-compose not found"'
                    sh 'docker-compose build --no-cache'
                }
            }
        }
        
        stage('Deploy') {
            steps {
                script {
                    echo 'Stopping existing containers...'
                    sh 'docker-compose down || true'
                    
                    echo 'Starting services...'
                    sh 'docker-compose up -d'
                    
                    echo 'Waiting for services to be ready...'
                    sh 'sleep 15'
                }
            }
        }
        
        stage('Health Check') {
            steps {
                echo 'Checking service health...'
                script {
                    sh 'docker-compose ps || echo "Cannot check services"'
                }
            }
        }
    }
    
    post {
        always {
            echo 'Pipeline execution completed'
            script {
                sh 'docker-compose ps || echo "Cannot check status"'
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
                sh 'docker-compose logs --tail=50 || echo "Cannot get logs"'
            }
        }
    }
}

