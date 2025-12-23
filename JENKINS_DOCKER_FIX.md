# Решение проблемы: docker-compose: not found

## Проблема
Jenkins не может найти команду `docker-compose` в контейнере.

## Решение 1: Использовать `docker compose` (РЕКОМЕНДУЕТСЯ)

Я уже обновил `Jenkinsfile` для использования `docker compose` (с пробелом) вместо `docker-compose`. Это новая версия, встроенная в Docker CLI.

**Что нужно сделать:**
1. Закоммитьте и запушьте обновленный Jenkinsfile:
   ```bash
   git add Jenkinsfile
   git commit -m "Fix: Use docker compose instead of docker-compose"
   git push
   ```
2. Запустите сборку заново в Jenkins

## Решение 2: Установить docker-compose в контейнер Jenkins

Если `docker compose` не работает, установите docker-compose вручную:

```bash
# Войти в контейнер Jenkins
docker exec -it jenkins bash

# Установить docker-compose
apt-get update
apt-get install -y docker-compose

# Или установить через pip
apt-get install -y python3-pip
pip3 install docker-compose

# Выйти из контейнера
exit
```

## Решение 3: Перезапустить Jenkins с доступом к Docker

Убедитесь, что Jenkins имеет доступ к Docker socket:

### Для Linux/Mac:
```bash
# Остановить Jenkins
docker stop jenkins
docker rm jenkins

# Запустить с доступом к Docker
docker run -d \
  --name jenkins \
  -p 8080:8080 \
  -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v /usr/bin/docker:/usr/bin/docker \
  jenkins/jenkins:lts
```

### Для Windows:
```powershell
# Остановить Jenkins
docker stop jenkins
docker rm jenkins

# Запустить Jenkins
docker run -d `
  --name jenkins `
  -p 8080:8080 `
  -p 50000:50000 `
  -v jenkins_home:/var/jenkins_home `
  jenkins/jenkins:lts
```

**Примечание для Windows:** Docker socket может работать по-другому. Убедитесь, что Docker Desktop запущен.

## Решение 4: Использовать Docker-in-Docker (DinD)

Если предыдущие решения не работают, можно использовать Docker-in-Docker:

```bash
docker run -d \
  --name jenkins \
  -p 8080:8080 \
  -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  --privileged \
  docker:dind
```

Но это более сложный вариант и обычно не требуется.

## Проверка доступа к Docker

Проверьте, что Jenkins может использовать Docker:

```bash
# Войти в контейнер Jenkins
docker exec -it jenkins bash

# Проверить доступ к Docker
docker ps
docker --version

# Если команды работают, значит доступ есть
# Если нет - нужно перезапустить Jenkins с правильными параметрами
```

## Рекомендуемый порядок действий

1. **Сначала попробуйте Решение 1** (уже сделано - обновлен Jenkinsfile)
   - Закоммитьте и запушьте изменения
   - Запустите сборку заново

2. **Если не работает, попробуйте Решение 3**
   - Перезапустите Jenkins с доступом к Docker socket

3. **Если все еще не работает, используйте Решение 2**
   - Установите docker-compose вручную в контейнер

## После исправления

После того как проблема решена, проверьте:
- Jenkins может выполнять `docker ps`
- Jenkins может выполнять `docker compose version` или `docker-compose version`
- Сборка проходит успешно

