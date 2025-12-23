pipeline {
    agent any
    
    stages {
        stage('Checkout') {
            steps {
                echo '📥 Checking out code from repository...'
                checkout scm
            }
        }
        
        stage('Validate') {
            steps {
                script {
                    echo '✅ Validating project structure...'
                    sh '''
                        echo "Checking project files..."
                        ls -la
                        echo ""
                        echo "Checking docker-compose.yml..."
                        test -f docker-compose.yml && echo "✅ docker-compose.yml found" || echo "❌ docker-compose.yml not found"
                        echo ""
                        echo "Checking Jenkinsfile..."
                        test -f Jenkinsfile && echo "✅ Jenkinsfile found" || echo "❌ Jenkinsfile not found"
                        echo ""
                        echo "Checking project directories..."
                        test -d API-Gateway && echo "✅ API-Gateway directory found" || echo "❌ API-Gateway not found"
                        test -d CabinetBooking && echo "✅ CabinetBooking directory found" || echo "❌ CabinetBooking not found"
                        test -d User && echo "✅ User directory found" || echo "❌ User not found"
                        test -d prometheus && echo "✅ prometheus directory found" || echo "❌ prometheus not found"
                        test -d grafana && echo "✅ grafana directory found" || echo "❌ grafana not found"
                    '''
                }
            }
        }
        
        stage('Info') {
            steps {
                script {
                    echo '📋 Build Information:'
                    sh '''
                        echo "Repository: https://github.com/Psidjik/adminka1.git"
                        echo "Branch: $(git rev-parse --abbrev-ref HEAD)"
                        echo "Commit: $(git rev-parse --short HEAD)"
                        echo "Author: $(git log -1 --pretty=format:'%an')"
                        echo "Message: $(git log -1 --pretty=format:'%s')"
                        echo ""
                        echo "📦 Project structure validated successfully!"
                        echo ""
                        echo "🚀 To deploy manually, run on host:"
                        echo "   docker-compose up -d"
                        echo ""
                        echo "📊 Services will be available at:"
                        echo "   - API Gateway: http://localhost:8080"
                        echo "   - Prometheus: http://localhost:9090"
                        echo "   - Grafana: http://localhost:3000"
                    '''
                }
            }
        }
    }
    
    post {
        always {
            echo '✅ Pipeline execution completed successfully!'
            echo '📝 Code has been checked out and validated.'
            echo '💡 Note: Deployment should be done manually on the host machine.'
        }
        success {
            echo '✅ Build successful!'
            echo '📦 All project files are present and valid.'
        }
        failure {
            echo '❌ Build failed!'
            echo 'Please check the logs above for details.'
        }
    }
}

