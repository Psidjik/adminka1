# PowerShell скрипт для запуска Jenkins с доступом к Docker (Windows)

# Остановить и удалить существующий контейнер (если есть)
docker stop jenkins 2>$null
docker rm jenkins 2>$null

# Запустить Jenkins с доступом к Docker socket
# Примечание: На Windows Docker socket может работать по-другому
docker run -d `
  --name jenkins `
  -p 8080:8080 `
  -p 50000:50000 `
  -v jenkins_home:/var/jenkins_home `
  jenkins/jenkins:lts

Write-Host "Jenkins запущен!"
Write-Host "Откройте http://localhost:8080"
Write-Host "Пароль администратора:"
docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword

