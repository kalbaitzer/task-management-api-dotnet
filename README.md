# API de Gerenciamento de Tarefas

![.NET](https://img.shields.io/badge/.NET-9-blueviolet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)
![Docker](https://img.shields.io/badge/Docker-Ready-blue)
![Arquitetura](https://img.shields.io/badge/Arquitetura-Clean-orange)
![Visual Studio Code](https://custom-icon-badges.demolab.com/badge/Visual%20Studio%20Code-0078d7.svg?logo=vsc&logoColor=white)

## Sobre o Projeto

Este repositório contém o código-fonte de uma API RESTful completa para um sistema de gerenciamento de tarefas. A API permite que os usuários organizem seus projetos e tarefas diárias, colaborem com colegas e monitorem o progresso.

O projeto foi desenvolvido com **.NET 9** e **PostgreSQL**, seguindo os princípios da **Clean Architecture** para garantir um código limpo, testável, escalável e de fácil manutenção. Toda a aplicação, incluindo o banco de dados, está pronta para ser executada em contêineres **Docker**.

---

## Funcionalidades e Regras de Negócio

A API implementa as seguintes regras de negócio:

- **Organização por Projetos e Tarefas**: A estrutura principal se baseia em `Projetos` que contêm múltiplas `Tarefas`.
- **Prioridades de Tarefas**: Cada tarefa possui uma prioridade (`Baixa`, `Média`, `Alta`) que é imutável após a sua criação.
- **Restrições de Remoção**: Um projeto não pode ser removido se ainda possuir tarefas pendentes ou em andamento.
- **Histórico de Alterações**: Todas as atualizações em uma tarefa (detalhes, status, etc.) são registradas em um histórico de alterações, incluindo quem fez a alteração, o que foi alterado e quando.
- **Limite de Tarefas**: Cada projeto pode conter no máximo 20 tarefas.
- **Relatórios de Desempenho**: Um endpoint especial, acessível apenas por usuários com a função de "Gerente", fornece a média de tarefas concluídas nos últimos 30 dias.
- **Comentários**: Usuários podem adicionar comentários às tarefas, que são salvos no histórico de alterações.

---

## Tecnologias Utilizadas

- **Backend**: .NET 9, ASP.NET Core
- **Persistência de Dados**: Entity Framework Core 9
- **Banco de Dados**: PostgreSQL 16
- **Arquitetura**: Clean Architecture
- **Conteinerização**: Docker & Docker Compose
- **Ambiente de Desenvolvimento**: Visual Studio Code
- **Versionamento de Código**: Git e GitHub
- **Teste da API**: Postman

---

## Arquitetura

O projeto está estruturado em quatro camadas principais, seguindo os princípios da Clean Architecture, o que garante uma clara separação de preocupações.

```
[ API (Apresentação) ]  -->  [ Application ]  -->  [ Core (Domínio) ]
           |                                              ^
           '------------->  [ Infrastructure ] -----------'
```

- **`Core`**: Camada central e mais interna. Contém as entidades de domínio (`User`, `Project`, `Task`, `TaskHistory`) e não depende de nenhuma outra camada.
- **`Application`**: Contém a lógica de negócio e os casos de uso (Services). Define as interfaces para os repositórios (`IRepository`) e outras abstrações.
- **`Infrastructure`**: Contém as implementações concretas das interfaces da camada de Aplicação. É aqui que reside o `DbContext`, os repositórios que usam EF Core e outras implementações de serviços externos.
- **`API`**: Camada mais externa. Expõe os endpoints RESTful (`Controllers`), recebe as requisições HTTP e as delega para a camada de Aplicação.

---

## Como Executar o Projeto no Docker

Esta é a maneira mais simples e recomendada de executar toda a aplicação.

### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.

### Passos
1. **Criação do arquivo `Dockerfile`**: Na raiz do projeto, crie um arquivo com nome `Dockerfile` com o seguinte conteúdo:
   ```yaml
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
   ```
   Este arquivo está disponível na raiz do projeto.

2. **Criação do arquivo `docker-compose.yml`**: Na raiz do projeto, crie um arquivo `docker-compose.yml` com o seguinte conteúdo:
   ```yaml
      # Configuração do container para o ambiente (computador) de desenvolvimento

      services:
        # Serviço do Banco de Dados PostgreSQL
        db:
          image: postgres:16 # Usa a imagem oficial do PostgreSQL
          container_name: task-management-db
          restart: always
          environment:
            - POSTGRES_USER=postgres
            - POSTGRES_PASSWORD=sql_pass # Use a mesma senha do seu appsettings
            - POSTGRES_DB=task_management_db
          ports:
            - "5433:5432" # Mapeia a porta 5432 do contêiner para a porta 5433 da sua máquina
          volumes:
            - postgres_data:/var/lib/postgresql/data # Garante que os dados do banco persistam

        # Serviço da sua API .NET
        api:
          container_name: task-management-api
          build:
            context: . # Constrói a imagem usando o Dockerfile na pasta atual
            dockerfile: Dockerfile
          ports:
            - "5000:8080" # Mapeia a porta 8080 do contêiner para a porta 5000 da sua máquina
          environment:
            - ASPNETCORE_URLS=http://+:8080 # Diz à API para rodar na porta 8080 dentro do contêiner
            - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=task_management_db;Username=postgres;Password=sql_pass
          depends_on:
            - db # Diz ao Docker para iniciar o contêiner 'db' antes do contêiner 'api'

      # Define o volume nomeado para persistir os dados do PostgreSQL
      volumes:
        postgres_data:
   ```
   Este arquivo está disponível na raiz do projeto.

3. **Criação dos contêineres**: No terminal, na raiz do projeto, execute:
   ```bash
      docker-compose up --build -d
   ```
   Serão criados dois contêineres: um para a API e outro para o PostgreSQL.
   A aplicação irá iniciar, e as migrações do banco de dados serão aplicadas automaticamente na primeira inicialização, criando todas as tabelas.

4. **Acesso à API**: A api disponível em:
   **[http://localhost:5000/](http://localhost:5000/)**

5. **Publicação no Docker Hub**: Para publicar no Docker Hub é necessário apenas o contêiner da API, pois o Docker baixa automaticmente a imagem do PostgreSQL quando for executado em outros computadores. Para a publicação é necessário a execução dos seguintes comandos através do terminal, na raiz do projeto:
   ```bash
      docker login
      docker-compose build
      docker tag src-api kalbaitzer/task-management-api:1.0
      docker push kalbaitzer/task-management-api:1.0
   ```

6. **Execução do contêiner em outros computadores**: Para executar o contêiner em outros computadores é necessário executar os seguintes passos:
   
   1. Criar uma pasta no computador onde o contêiner será executado com o nome `task-management-api`
   
   2. Criar um arquivo na pasta `task-management-api` com o nome `docker-compose.yml` com o seguinte conteúdo:
      ```yaml
         # Configuração do container para execução em outros computadores

         services:
         # O serviço do banco de dados não muda nada.
         db:
            image: postgres:16
            container_name: task-management-db
            restart: always
            environment:
               - POSTGRES_USER=postgres
               - POSTGRES_PASSWORD=your_password
               - POSTGRES_DB=task_management_db
            ports:
               - "5433:5432"
            volumes:
               - postgres_data:/var/lib/postgresql/data

         # O serviço da API é modificado
         api:
            container_name: task-management-api
            image: kalbaitzer/task-management-api:1.0
            ports:
               - "5000:8080"
            environment:
               - ASPNETCORE_URLS=http://+:8080
               - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=task_management_db;Username=postgres;Password=your_password
            depends_on:
               - db

         volumes:
         postgres_data:
      ```
      O conteúdo dste arquivo é diferente do usado no computador de desenvolvimento.
      Este arquivo está disponível na raiz do projeto com o nome `docker-compose-runtime.yml`.

   3. No terminal, na pasta `task-management-api`, execute:
      ```bash
         docker-compose up -d
      ```
---

## Documentação da API (Endpoints)

A API assume um modelo de autenticação externa, onde a identidade do usuário é passada através de cabeçalhos HTTP.

**Cabeçalho Obrigatório para as requisições:**
- `X-User-Id`: O GUID do usuário que está fazendo a requisição. Este Id é previamente cadastrado na tabela `Users` para que a API possa verificar se é um usuário válido. Na inicialização do banco de dados são cadastrados dois usuários para teste.

<details>
<summary><strong>Endpoints de Usuários: /api/users</strong></summary>

| Verbo | Rota | Descrição |
| :--- | :--- | :--- |
| `POST` | `/api/users` | Cria um novo usuário para teste da API. |
| `GET` | `/api/users` | Lista todos os usuários cadastrados. |
| `GET` | `/api/users/{userId}` | Busca os detalhes de um usuário específico. |
| `DELETE` | `/api/users/{userId}` | Remove um usuário. |

</details>

<details>
<summary><strong>Endpoints de Projetos: /api/projects</strong></summary>

| Verbo | Rota | Descrição |
| :--- | :--- | :--- |
| `POST` | `/api/projects` | Cria um novo projeto para o usuário identificado no header `X-User-Id`. |
| `GET` | `/api/projects` | Lista todos os projetos do usuário identificado no header `X-User-Id`. |
| `GET` | `/api/projects/{projectId}` | Busca os detalhes de um projeto específico. |
| `DELETE` | `/api/projects/{projectId}` | Remove um projeto (se não houver tarefas ativas). |

</details>

<details>
<summary><strong>Endpoints de Tarefas: /api/tasks</strong></summary>

| Verbo | Rota | Descrição |
| :--- | :--- | :--- |
| `POST` | `/api/tasks/projects/{projectId}` | Cria uma nova tarefa dentro de um projeto específico. |
| `GET` | `/api/tasks/projects/{projectId}` | Lista todas as tarefas de um projeto específico. |
| `GET` | `/api/tasks/{taskId}` | Busca os detalhes de uma tarefa específica. |
| `PUT` | `/api/tasks/{taskId}` | Atualiza os detalhes (título, descrição, data e status) de uma tarefa. |
| `PATCH` | `/api/tasks/{taskId}/status` | Atualiza apenas o status de uma tarefa. |
| `DELETE` | `/api/tasks/{taskId}` | Remove uma tarefa. |
| `POST` | `/api/tasks/{taskId}/comments` | Adiciona um comentário a uma tarefa. |
| `GET` | `/api/tasks/{taskId}/history` | Lista todo o histórico de alterações e comentários de uma tarefa. |

</details>

<details>
<summary><strong>Endpoints de Relatórios: /api/reports</strong></summary>

| Verbo | Rota | Descrição |
| :--- | :--- | :--- |
| `GET` | `/api/reports/performance`| Gera um relatório de desempenho. Somente usuários com Role `Manager`. |

</details>