-- PASSO 1: Criar o banco de dados (execute isso conectado ao seu servidor PostgreSQL, não a um banco específico)
-- Você pode precisar de permissões de superusuário.
CREATE DATABASE task_management_db
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1;

-- PASSO 2: Conecte-se ao banco de dados 'task_management_db' recém-criado e execute o resto do script.

-- Habilita a extensão para gerar UUIDs.
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Tabela de Usuários
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(150) NOT NULL,
    "Email" VARCHAR(150) NOT NULL UNIQUE,
    "Role" VARCHAR(50) NOT NULL
);

-- Tabela de Projetos
CREATE TABLE "Projects" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(500),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "OwnerUserId" UUID NOT NULL,
    CONSTRAINT "FK_Projects_Users" FOREIGN KEY ("OwnerUserId") 
        REFERENCES "Users"("Id") ON DELETE RESTRICT
);

-- Tabela de Tarefas
-- Enums (Status, Priority) são armazenados como inteiros por padrão.
-- 0=Pendente, 1=EmAndamento, 2=Concluida
-- 0=Baixa, 1=Media, 2=Alta
CREATE TABLE "Tasks" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Title" VARCHAR(200) NOT NULL,
    "Description" VARCHAR(1000),
    "DueDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Status" INTEGER NOT NULL,
    "Priority" INTEGER NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL, -- Campo recomendado que adicionamos
    "ProjectId" UUID NOT NULL,
    CONSTRAINT "FK_Tasks_Projects" FOREIGN KEY ("ProjectId") 
        REFERENCES "Projects"("Id") ON DELETE CASCADE
);

-- Tabela de Histórico de Tarefas
CREATE TABLE "TaskHistories" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "ChangeType" VARCHAR(50) NOT NULL,
    "FieldName" VARCHAR(100),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "Comment" TEXT,
    "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL,
    "TaskId" UUID NOT NULL,
    "UserId" UUID NOT NULL,
    CONSTRAINT "FK_TaskHistories_Tasks" FOREIGN KEY ("TaskId") 
        REFERENCES "Tasks"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TaskHistories_Users" FOREIGN KEY ("UserId") 
        REFERENCES "Users"("Id") ON DELETE RESTRICT
);

-- Criação de Índices para otimização de consultas
CREATE INDEX "IX_Projects_OwnerUserId" ON "Projects" ("OwnerUserId");
CREATE INDEX "IX_Tasks_ProjectId" ON "Tasks" ("ProjectId");
CREATE INDEX "IX_TaskHistories_TaskId" ON "TaskHistories" ("TaskId");
CREATE INDEX "IX_TaskHistories_UserId" ON "TaskHistories" ("UserId");

-- O índice UNIQUE para Users.Email já foi criado pela restrição UNIQUE na definição da tabela.

-- Inserção dos Usuários para Teste, uma vez que o cadastro de usuários e a autenticação são externos
INSERT INTO "Users" ("Name", "Email", "Role") VALUES ('Ednaldo Kalbaitzer', 'kalbaitzer@yahoo.com.br', 'Manager');
INSERT INTO "Users" ("Name", "Email", "Role") VALUES ('José Silva', 'jose.silva@gmail.com', 'User');