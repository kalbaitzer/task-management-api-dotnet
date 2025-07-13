# Estágio 1: Build (Compilação)
# Usamos a imagem do SDK completo do .NET 9 para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /

# Copia os arquivos de projeto (.csproj) e o arquivo de solução (.sln) primeiro
COPY ["TaskManagementAPI.API/TaskManagementAPI.API.csproj", "TaskManagementAPI.API/"]
COPY ["TaskManagementAPI.Application/TaskManagementAPI.Application.csproj", "TaskManagementAPI.Application/"]
COPY ["TaskManagementAPI.Core/TaskManagementAPI.Core.csproj", "TaskManagementAPI.Core/"]
COPY ["TaskManagementAPI.Infrastructure/TaskManagementAPI.Infrastructure.csproj", "TaskManagementAPI.Infrastructure/"]
COPY ["TaskManagementAPI.sln", "."]

# Restaura as dependências NuGet (isso é feito antes para aproveitar o cache do Docker)
RUN dotnet restore TaskManagementAPI.sln

# Copia todo o resto do código fonte
COPY . .

# Publica a aplicação em modo de Release, otimizada para produção
WORKDIR "TaskManagementAPI.API"
RUN dotnet publish "TaskManagementAPI.API.csproj" -c Release -o /app/publish

# Estágio 2: Final (Execução)
# Usamos a imagem do ASP.NET, que é menor
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia apenas os arquivos publicados do estágio de build
COPY --from=build /app/publish .

# Define o ponto de entrada, o comando que será executado quando o contêiner iniciar
ENTRYPOINT ["dotnet", "TaskManagementAPI.API.dll"]