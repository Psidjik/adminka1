# Инструкция по развертыванию и настройке мониторинга

## Обзор проекта

Проект состоит из трех микросервисов:
- **API-Gateway** - GraphQL Gateway на порту 8080
- **CabinetBooking** - gRPC сервис для бронирования кабинетов на порту 28710
- **User** - gRPC сервис для аутентификации на порту 28711

## Что было сделано

### 1. Добавлены метрики Prometheus
Во все три проекта добавлена поддержка метрик Prometheus:
- Установлен пакет `prometheus-net.AspNetCore` версии 8.2.1
- Добавлены middleware для сбора метрик HTTP запросов
- Настроены endpoints `/metrics` для экспорта метрик

### 2. Обновлен docker-compose.yml
- Исправлены пути к проектам (используются относительные пути)
- Добавлены переменные окружения для подключения к БД
- Добавлены сервисы Prometheus и Grafana
- Настроены зависимости между сервисами

### 3. Создана конфигурация Prometheus
- Файл `prometheus/prometheus.yml` настроен на сбор метрик со всех трех сервисов
- Интервал сбора метрик: 15 секунд

### 4. Создана конфигурация Grafana
- Настроен автоматический провайдинг Prometheus как источника данных
- Создан базовый дашборд для мониторинга метрик приложений

## Порты сервисов

- **PostgreSQL**: 5443 (внешний) -> 5432 (внутренний)
- **API-Gateway**: 8080 (GraphQL), 28720 (резервный)
- **CabinetBooking**: 28710
- **User**: 28711
- **Prometheus**: 9090
- **Grafana**: 3000

## Задание: Настройка CI/CD с Jenkins

### Шаги выполнения:

#### 1. Подготовка Git репозитория
```bash
# Инициализация репозитория (если еще не сделано)
git init
git add .
git commit -m "Initial commit with Prometheus metrics and monitoring"

# Добавление remote репозитория
git remote add origin <URL_ВАШЕГО_РЕПОЗИТОРИЯ>
git push -u origin main
```

#### 2. Настройка Jenkins

1. **Установка Jenkins** (если еще не установлен):
   - Скачать Jenkins с https://www.jenkins.io/download/
   - Или использовать Docker образ: `docker run -p 8080:8080 jenkins/jenkins:lts`

2. **Установка необходимых плагинов в Jenkins**:
   - Docker Pipeline
   - Git
   - Docker Compose Build Step (опционально)

3. **Создание Jenkins Pipeline**:
   - Создать новый Pipeline job
   - Настроить подключение к Git репозиторию
   - Добавить Jenkinsfile (см. ниже)

#### 3. Создание Jenkinsfile

Создайте файл `Jenkinsfile` в корне проекта:

```groovy
pipeline {
    agent any
    
    environment {
        DOCKER_COMPOSE = 'docker-compose'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Build') {
            steps {
                sh 'docker-compose build'
            }
        }
        
        stage('Test') {
            steps {
                // Здесь можно добавить тесты
                echo 'Running tests...'
            }
        }
        
        stage('Deploy') {
            steps {
                sh 'docker-compose down'
                sh 'docker-compose up -d'
            }
        }
    }
    
    post {
        always {
            echo 'Pipeline completed'
        }
        success {
            echo 'Deployment successful!'
        }
        failure {
            echo 'Deployment failed!'
        }
    }
}
```

#### 4. Настройка автоматической сборки
- В Jenkins настройте webhook или polling для автоматической сборки при push в репозиторий
- Настройте GitHub/GitLab webhook (если используется): `http://<JENKINS_URL>/github-webhook/`

#### 5. Проверка работы
1. Сделайте изменения в коде
2. Закоммитьте и запушьте изменения:
   ```bash
   git add .
   git commit -m "Test changes"
   git push
   ```
3. Jenkins автоматически запустит сборку
4. После успешной сборки все сервисы будут пересобраны и перезапущены

## Запуск приложения

### Локальный запуск через docker-compose

```bash
# Запуск всех сервисов
docker-compose up -d

# Просмотр логов
docker-compose logs -f

# Остановка всех сервисов
docker-compose down
```

### Проверка работы сервисов

1. **API-Gateway**: http://localhost:8080/graphql/ui
2. **Prometheus**: http://localhost:9090
3. **Grafana**: http://localhost:3000 (admin/admin)

### Проверка метрик

1. Откройте Prometheus: http://localhost:9090
2. В разделе Status -> Targets проверьте, что все targets в состоянии UP
3. В разделе Graph можно выполнить запросы, например:
   - `rate(http_requests_received_total[5m])`
   - `http_requests_active`

### Настройка Grafana

1. Войдите в Grafana (admin/admin)
2. Перейдите в Configuration -> Data Sources
3. Убедитесь, что Prometheus добавлен автоматически
4. Перейдите в Dashboards -> Browse
5. Импортируйте или создайте дашборды для мониторинга

## Демонстрация на зачете

### Порядок действий:

1. **Показать docker-compose.yml**:
   ```bash
   cat docker-compose.yml
   ```

2. **Поднять все сервисы**:
   ```bash
   docker-compose up -d
   ```

3. **Проверить статус**:
   ```bash
   docker-compose ps
   ```

4. **Показать метрики в Prometheus**:
   - Открыть http://localhost:9090
   - Показать targets и метрики

5. **Показать дашборды в Grafana**:
   - Открыть http://localhost:3000
   - Показать дашборды с метриками

6. **Продемонстрировать CI/CD**:
   - Сделать изменения в коде
   - Закоммитить и запушить:
     ```bash
     git add .
     git commit -m "Test changes for demo"
     git push
     ```
   - Показать, что Jenkins запустил сборку
   - Показать логи сборки
   - Показать, что сервисы обновились

## Структура проекта

```
CabinetBooking/
├── API-Gateway/          # GraphQL Gateway
├── CabinetBooking/        # Сервис бронирования
├── User/                  # Сервис аутентификации
├── docker-compose.yml     # Конфигурация Docker Compose
├── prometheus/            # Конфигурация Prometheus
│   └── prometheus.yml
├── grafana/               # Конфигурация Grafana
│   └── provisioning/
│       ├── datasources/
│       └── dashboards/
└── Jenkinsfile            # Jenkins Pipeline (создать)
```

## Важные замечания

1. **База данных**: При первом запуске создайте базы данных в PostgreSQL:
   ```sql
   CREATE DATABASE cabinet_booking;
   CREATE DATABASE users;
   ```

2. **Переменные окружения**: Все настройки подключения к БД передаются через переменные окружения в docker-compose.yml

3. **Сеть**: Все сервисы находятся в одной Docker сети `external-network` и могут обращаться друг к другу по именам контейнеров

4. **Метрики**: Метрики доступны на endpoint `/metrics` каждого сервиса

## Troubleshooting

### Проблема: Сервисы не могут подключиться к БД
- Проверьте, что PostgreSQL запущен: `docker-compose ps`
- Проверьте логи: `docker-compose logs postgres`
- Убедитесь, что базы данных созданы

### Проблема: Метрики не собираются
- Проверьте, что endpoint `/metrics` доступен: `curl http://localhost:8080/metrics`
- Проверьте конфигурацию Prometheus: `docker-compose logs prometheus`
- Убедитесь, что все сервисы в одной сети

### Проблема: Jenkins не запускает сборку
- Проверьте настройки webhook в Git репозитории
- Проверьте логи Jenkins: `docker logs <jenkins-container>`
- Убедитесь, что Jenkins имеет доступ к Docker

