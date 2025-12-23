# Скрипт для перезапуска Jenkins с доступом к Docker (Windows)

Write-Host "Останавливаем Jenkins..."
docker stop jenkins 2>$null
docker rm jenkins 2>$null

Write-Host "Запускаем Jenkins с доступом к Docker..."
# На Windows Docker Desktop автоматически предоставляет доступ к Docker
# Но нужно убедиться, что Jenkins может использовать Docker
docker run -d `
  --name jenkins `
  -p 8080:8080 `
  -p 50000:50000 `
  -v jenkins_home:/var/jenkins_home `
  -v //var/run/docker.sock:/var/run/docker.sock `
  --group-add $(docker info | Select-String -Pattern "Group:" | ForEach-Object { $_.Line.Split(':')[1].Trim() }) `
  jenkins/jenkins:lts

Write-Host "Ждем запуска Jenkins..."
Start-Sleep -Seconds 5

Write-Host "Проверяем доступ к Docker..."
docker exec jenkins docker --version
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Docker доступен в Jenkins!"
} else {
    Write-Host "⚠️ Docker недоступен. Попробуйте альтернативный метод."
    Write-Host "Альтернативный метод: установите Docker CLI в контейнер Jenkins"
}

Write-Host "`nJenkins запущен на http://localhost:8080"
Write-Host "Пароль администратора:"
docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword

