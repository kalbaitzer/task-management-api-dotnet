
Migrations
  dotnet ef migrations add InitialCreate --project TaskManagementAPI.Infrastructure --startup-project TaskManagementAPI.API

Docker Compose
  docker-compose up --build -d
  docker-compose up

Docker Hub  
  docker login
  docker-compose build
  docker tag src-api kalbaitzer/task-management-api:1.0
  docker push kalbaitzer/task-management-api:1.0

Executar o container em outro computador
  Criar uma pasta como, por exemplo, C:\task-management-api
  Copiar o arquivo docker-compose-runtime.yml para esta pasta com o nome docker-compose.yml
  docker-compose up -d
