# Решение проблемы: Docker Permission denied в Jenkins

## Проблема
Jenkins не может использовать Docker из-за отсутствия прав доступа.

## Решение для Windows

### Вариант 1: Перезапустить Jenkins с доступом к Docker socket

**Остановите текущий Jenkins:**
```powershell
docker stop jenkins
docker rm jenkins
```

**Запустите Jenkins с доступом к Docker:**
```powershell
docker run -d `
  --name jenkins `
  -p 8080:8080 `
  -p 50000:50000 `
  -v jenkins_home:/var/jenkins_home `
  -v //var/run/docker.sock:/var/run/docker.sock `
  jenkins/jenkins:lts
```

**Проверьте доступ:**
```powershell
docker exec jenkins docker --version
docker exec jenkins docker ps
```

### Вариант 2: Добавить пользователя jenkins в группу docker

Если вариант 1 не работает, нужно добавить пользователя jenkins в группу docker внутри контейнера:

```powershell
# Войти в контейнер Jenkins
docker exec -it jenkins bash

# В контейнере выполнить:
# Найти группу docker на хосте
# Затем добавить пользователя jenkins в эту группу
```

### Вариант 3: Использовать Docker-in-Docker (DinD)

Это более сложный вариант, но он гарантированно работает:

```powershell
# Остановить Jenkins
docker stop jenkins
docker rm jenkins

# Запустить Jenkins с Docker-in-Docker
docker run -d `
  --name jenkins `
  -p 8080:8080 `
  -p 50000:50000 `
  -v jenkins_home:/var/jenkins_home `
  --privileged `
  docker:dind
```

### Вариант 4: Установить Docker CLI в контейнер Jenkins

Если Docker socket недоступен, можно установить Docker CLI в контейнер:

```powershell
# Войти в контейнер
docker exec -it jenkins bash

# Установить Docker CLI
apt-get update
apt-get install -y docker.io

# Или скачать Docker CLI
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh
```

## Рекомендуемое решение для Windows

На Windows с Docker Desktop лучше всего использовать **Вариант 1**, но с дополнительной настройкой:

```powershell
# 1. Остановить Jenkins
docker stop jenkins
docker rm jenkins

# 2. Запустить Jenkins
docker run -d `
  --name jenkins `
  -p 8080:8080 `
  -p 50000:50000 `
  -v jenkins_home:/var/jenkins_home `
  jenkins/jenkins:lts

# 3. Установить Docker CLI в контейнер
docker exec -u root jenkins bash -c "apt-get update && apt-get install -y docker.io"

# 4. Проверить доступ
docker exec jenkins docker --version
```

## Альтернативное решение: Использовать Docker Compose через хост

Если доступ к Docker из контейнера невозможен, можно выполнять команды на хосте:

Обновите Jenkinsfile, чтобы использовать Docker на хосте через SSH или другой метод.

## Проверка после исправления

После перезапуска Jenkins проверьте:

```powershell
# Проверить, что Jenkins запущен
docker ps | findstr jenkins

# Проверить доступ к Docker
docker exec jenkins docker --version
docker exec jenkins docker ps

# Если команды работают, значит доступ есть
```

## Если ничего не помогает

Можно использовать Jenkins на хосте (не в Docker) или использовать Jenkins в WSL2, где доступ к Docker работает лучше.

