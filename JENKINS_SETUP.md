# Пошаговая настройка Jenkins для CI/CD

## Шаг 1: Первый запуск Jenkins

### 1.1. Откройте Jenkins в браузере
```
http://localhost:8080
```

### 1.2. Получите начальный пароль администратора

Выполните команду в терминале:
```bash
docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword
```

Или если контейнер называется по-другому:
```bash
docker ps  # найти имя контейнера Jenkins
docker exec <имя_контейнера> cat /var/jenkins_home/secrets/initialAdminPassword
```

### 1.3. Введите пароль в Jenkins
- Скопируйте пароль из терминала
- Вставьте в поле "Administrator password" в Jenkins
- Нажмите "Continue"

### 1.4. Установите рекомендуемые плагины
- Выберите "Install suggested plugins"
- Дождитесь установки всех плагинов

### 1.5. Создайте администратора
- Введите имя пользователя (например: `admin`)
- Введите пароль
- Введите email (опционально)
- Нажмите "Save and Continue"

### 1.6. Настройте URL Jenkins
- Оставьте URL по умолчанию: `http://localhost:8080/`
- Нажмите "Save and Finish"
- Нажмите "Start using Jenkins"

## Шаг 2: Настройка доступа Jenkins к Docker

⚠️ **ВАЖНО!** Jenkins должен иметь доступ к Docker для сборки образов.

### Вариант A: Запуск Jenkins с доступом к Docker socket (РЕКОМЕНДУЕТСЯ)

Остановите текущий контейнер Jenkins:
```bash
docker stop jenkins
docker rm jenkins
```

Запустите Jenkins с доступом к Docker:
```bash
docker run -d \
  --name jenkins \
  -p 8080:8080 \
  -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v /usr/bin/docker:/usr/bin/docker \
  jenkins/jenkins:lts
```

**Для Windows (PowerShell):**
```powershell
docker run -d `
  --name jenkins `
  -p 8080:8080 `
  -p 50000:50000 `
  -v jenkins_home:/var/jenkins_home `
  -v //var/run/docker.sock:/var/run/docker.sock `
  jenkins/jenkins:lts
```

**Примечание для Windows:** Docker socket может не работать напрямую. В этом случае используйте Docker-in-Docker или установите Jenkins на Linux/WSL2.

### Вариант B: Установка Docker внутри Jenkins контейнера

Если вариант A не работает, можно установить Docker CLI в контейнер Jenkins, но это сложнее.

## Шаг 3: Установка необходимых плагинов

### 3.1. Откройте Manage Jenkins
- В Jenkins нажмите "Manage Jenkins" (в левом меню)

### 3.2. Откройте Manage Plugins
- Нажмите "Manage Plugins"
- Перейдите на вкладку "Available"

### 3.3. Установите плагины:
Найдите и установите следующие плагины:
- ✅ **Git** (обычно уже установлен)
- ✅ **Docker Pipeline** (для работы с Docker)
- ✅ **Docker** (базовый плагин для Docker)
- ✅ **Pipeline** (для работы с Jenkinsfile)

### 3.4. Установка плагинов
- Отметьте нужные плагины
- Нажмите "Install without restart"
- Дождитесь установки
- При необходимости перезапустите Jenkins

## Шаг 4: Настройка Git в Jenkins

### 4.1. Настройка Git (если нужно)
- Manage Jenkins → Configure System
- Найдите раздел "Git"
- Убедитесь, что путь к Git указан правильно (обычно `/usr/bin/git`)

## Шаг 5: Создание Pipeline Job

### 5.1. Создайте новый Pipeline
- На главной странице Jenkins нажмите "New Item"
- Введите имя: `CabinetBooking-CI-CD`
- Выберите "Pipeline"
- Нажмите "OK"

### 5.2. Настройте Pipeline

#### Вкладка "General"
- ✅ Отметьте "GitHub project" (опционально)
- URL проекта: `https://github.com/Psidjik/adminka1`

#### Вкладка "Pipeline"
- **Definition:** выберите "Pipeline script from SCM"
- **SCM:** выберите "Git"
- **Repository URL:** `https://github.com/Psidjik/adminka1.git`
- **Credentials:** оставьте пустым (если репозиторий публичный)
- **Branches to build:** `*/master` или `*/main` (в зависимости от вашей ветки)
- **Script Path:** `Jenkinsfile` (путь к файлу в репозитории)
- Нажмите "Save"

## Шаг 6: Первый запуск сборки

### 6.1. Запустите сборку вручную
- На странице Pipeline нажмите "Build Now"
- Сборка начнется автоматически

### 6.2. Просмотр логов
- Нажмите на номер сборки (например, "#1")
- Нажмите "Console Output" для просмотра логов

### 6.3. Проверка результата
- Если сборка успешна, вы увидите зеленый индикатор ✅
- Если есть ошибки, проверьте логи

## Шаг 7: Настройка автоматической сборки

### 7.1. Настройка Poll SCM (периодическая проверка)

В настройках Pipeline:
- Откройте "Configure"
- В разделе "Build Triggers" отметьте "Poll SCM"
- Введите расписание: `H/5 * * * *` (каждые 5 минут)
- Нажмите "Save"

### 7.2. Настройка Webhook (рекомендуется)

Для автоматической сборки при push в Git:

#### На GitHub:
1. Перейдите в репозиторий: https://github.com/Psidjik/adminka1
2. Settings → Webhooks → Add webhook
3. Payload URL: `http://<ваш_IP>:8080/github-webhook/`
   - Для localhost: `http://localhost:8080/github-webhook/`
   - Для внешнего доступа: `http://<ваш_IP>:8080/github-webhook/`
4. Content type: `application/json`
5. Events: выберите "Just the push event"
6. Active: ✅
7. Add webhook

#### В Jenkins:
1. Откройте настройки Pipeline
2. В "Build Triggers" отметьте "GitHub hook trigger for GITScm polling"
3. Save

## Шаг 8: Проверка работы

### 8.1. Тестовая сборка
1. Сделайте небольшое изменение в коде (например, добавьте комментарий)
2. Закоммитьте и запушьте:
   ```bash
   git add .
   git commit -m "Test CI/CD"
   git push
   ```
3. Проверьте, что Jenkins автоматически запустил сборку

### 8.2. Проверка сервисов
После успешной сборки проверьте:
```bash
docker ps  # должны быть запущены все контейнеры
docker-compose ps  # если docker-compose доступен
```

## Возможные проблемы и решения

### Проблема 1: Jenkins не может выполнить docker-compose

**Решение:**
- Убедитесь, что Jenkins имеет доступ к Docker socket
- Установите docker-compose в контейнер Jenkins:
  ```bash
  docker exec -it jenkins bash
  apt-get update
  apt-get install -y docker-compose
  ```

### Проблема 2: Ошибка доступа к Git

**Решение:**
- Проверьте, что репозиторий публичный или добавьте credentials
- Убедитесь, что URL репозитория правильный

### Проблема 3: Ошибка сборки Docker образов

**Решение:**
- Проверьте, что все Dockerfile находятся в правильных местах
- Проверьте логи сборки в Jenkins Console Output
- Убедитесь, что контекст сборки правильный (пути в docker-compose.yml)

### Проблема 4: Порт 8080 занят

**Решение:**
- Измените порт Jenkins:
  ```bash
  docker run -d -p 8081:8080 --name jenkins jenkins/jenkins:lts
  ```
- Или остановите сервис, занимающий порт 8080

## Команды для управления Jenkins

```bash
# Остановить Jenkins
docker stop jenkins

# Запустить Jenkins
docker start jenkins

# Просмотр логов Jenkins
docker logs jenkins

# Войти в контейнер Jenkins
docker exec -it jenkins bash

# Перезапустить Jenkins
docker restart jenkins
```

## Итоговая проверка

После настройки у вас должно быть:
- ✅ Jenkins запущен и доступен на http://localhost:8080
- ✅ Установлены необходимые плагины
- ✅ Создан Pipeline Job
- ✅ Pipeline подключен к Git репозиторию
- ✅ Первая сборка выполнена успешно
- ✅ Автоматическая сборка настроена (Poll SCM или Webhook)

## Следующие шаги

1. **Протестируйте CI/CD:**
   - Сделайте изменения в коде
   - Закоммитьте и запушьте
   - Убедитесь, что Jenkins автоматически запустил сборку

2. **Проверьте работу сервисов:**
   - API Gateway: http://localhost:8080/graphql/ui
   - Prometheus: http://localhost:9090
   - Grafana: http://localhost:3000

3. **На зачете:**
   - Покажите docker-compose.yml
   - Поднимите все сервисы через Jenkins
   - Покажите метрики в Prometheus и Grafana
   - Продемонстрируйте автоматическую сборку при push в Git

