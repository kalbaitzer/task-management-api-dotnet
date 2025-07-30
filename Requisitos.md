# .NET (C#)

### Visão geral:

O time de desenvolvimento de uma empresa precisa de sua ajuda para criar um sistema de gerenciamento de tarefas. O objetivo é desenvolver uma **API** que permita aos usuários organizar e monitorar suas tarefas diárias, bem como colaborar com colegas de equipe.

---

### Detalhes do App:

**Usuário**

Pessoa que utiliza o aplicativo detentor de uma conta.

**Projeto**

Um projeto é uma entidade que contém várias tarefas. Um usuário pode criar, visualizar e gerenciar vários projetos.

**Tarefa**

Uma tarefa é uma unidade de trabalho dentro de um projeto. Cada tarefa possui um título, uma descrição, uma data de vencimento e um status (pendente, em andamento, concluída).

---

### Fase 1: API Coding

Para a primeira Sprint, foi estipulado o desenvolvimento de funcionalidades básicas para o gerenciamento de tarefas. Desenvolva uma RESTful API capaz de responder a requisições feitas pelo aplicativo para os seguintes itens:

1. **Listagem de Projetos** - listar todos os projetos do usuário
2. **Visualização de Tarefas** - visualizar todas as tarefas de um projeto específico
3. **Criação de Projetos** - criar um novo projeto
4. **Criação de Tarefas** - adicionar uma nova tarefa a um projeto
5. **Atualização de Tarefas** - atualizar o status ou detalhes de uma tarefa
6. **Remoção de Tarefas** - remover uma tarefa de um projeto

**Regras de negócio:**

1. **Prioridades de Tarefas:**
    - Cada tarefa deve ter uma prioridade atribuída (baixa, média, alta).
    - Não é permitido alterar a prioridade de uma tarefa depois que ela foi criada.
2. **Restrições de Remoção de Projetos:**
    - Um projeto não pode ser removido se ainda houver tarefas pendentes associadas a ele.
    - Caso o usuário tente remover um projeto com tarefas pendentes, a API deve retornar um erro e sugerir a conclusão ou remoção das tarefas primeiro.
3. **Histórico de Atualizações:**
    - Cada vez que uma tarefa for atualizada (status, detalhes, etc.), a API deve registrar um histórico de alterações para a tarefa.
    - O histórico de alterações deve incluir informações sobre o que foi modificado, a data da modificação e o usuário que fez a modificação.
4. **Limite de Tarefas por Projeto:**
    - Cada projeto tem um limite máximo de 20 tarefas. Tentar adicionar mais tarefas do que o limite deve resultar em um erro.
5. **Relatórios de Desempenho:**
    - A API deve fornecer endpoints para gerar relatórios de desempenho, como o número médio de tarefas concluídas por usuário nos últimos 30 dias.
    - Os relatórios devem ser acessíveis apenas por usuários com uma função específica de "gerente".
6. **Comentários nas Tarefas:**
    - Os usuários podem adicionar comentários a uma tarefa para fornecer informações adicionais.
    - Os comentários devem ser registrados no histórico de alterações da tarefa.

**Regras da API e avaliação:**

1. **Não é** **necessário** nenhum tipo de CRUD para usuários.
2. **Não é necessário** nenhum tipo de autenticação; este será um serviço externo.
3. Tenha pelo menos **80%** de cobertura de testes de unidade para validar suas regras de negócio.
4. **Utilize o git** como ferramenta de versionamento de código.
5. **Utilize um banco de dados** (o que preferir) para salvar os dados.
6. **Utilize o framework e libs** que julgue necessário para uma boa implementação.
7. **O projeto deve executar no docker e as informações de execução via terminal devem estar disponíveis no [README.md](http://README.md) do projeto**

---

### Fase 2: Refinamento

Para a segunda fase, escreva no arquivo **README.md** em uma sessão dedicada, o que você perguntaria para o *PO* visando o refinamento para futuras implementações ou melhorias.

---

### Fase 3: Final

Na terceira fase, escreva no arquivo **README.md** em uma sessão dedicada o que você melhoraria no projeto, identificando possíveis pontos de melhoria, implementação de padrões, visão do projeto sobre arquitetura/cloud, etc.

