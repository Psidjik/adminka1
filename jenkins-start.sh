#!/bin/bash
# Скрипт для запуска Jenkins с доступом к Docker

# Остановить и удалить существующий контейнер (если есть)
docker stop jenkins 2>/dev/null
docker rm jenkins 2>/dev/null

# Запустить Jenkins с доступом к Docker socket
docker run -d \
  --name jenkins \
  -p 8080:8080 \
  -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v /usr/bin/docker:/usr/bin/docker \
  jenkins/jenkins:lts

echo "Jenkins запущен!"
echo "Откройте http://localhost:8080"
echo "Пароль администратора:"
docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword

