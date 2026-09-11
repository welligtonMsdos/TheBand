# Diretrizes para CRUDs

Sempre que for solicitado criar, alterar ou completar um CRUD neste projeto, siga estas diretrizes.

## Módulo `TheBand.Collections`

- O novo recurso de coleções deve usar o nome `TheBand.Collections` e ser implementado com .NET 10.
- Crie o controller no projeto de API responsável pela aplicação (preferencialmente `TheBand.CoreApi`), em uma pasta `Controllers` ou `Features/Collections`, conforme a convenção já existente.
- Nomeie o controller como `CollectionsController`, use a rota `api/collections` e mantenha-o fino: ele deve apenas receber a requisição HTTP, encaminhar o caso de uso da Application e retornar o resultado HTTP apropriado.
- Não coloque regras de negócio, acesso a banco de dados ou construção manual de dependências no controller.
- Crie os casos de uso e contratos de `TheBand.Collections` na camada Application, a entidade e regras na camada Domain e a persistência na camada Infrastructure, respeitando as dependências descritas neste documento.
- Configure as dependências do módulo no projeto de API por interfaces e implemente testes correspondentes em `TheBand.Tests`.

## Arquitetura e responsabilidades

- Preserve a separação entre as camadas `CoreApi`, `AuthApi`, `AuthApplication`, `AuthDomain` e `AuthInfrastructure`.
- Aplique a mesma separação também ao módulo Core: `CoreApi`, `CoreApplication`, `CoreDomain` e `CoreInfrastructure`.
- A API deve conter somente preocupações de apresentação: controllers/endpoints, contratos HTTP, mapeamento de requisição/resposta, validação de entrada e configuração.
- A camada Application deve conter os casos de uso do CRUD, regras de orquestração, DTOs/commands/queries e suas interfaces.
- A camada Domain deve concentrar entidades, value objects, regras de negócio e contratos que pertencem ao domínio. Ela não pode depender de Infrastructure nem de API.
- A camada Infrastructure deve implementar persistência, acesso a serviços externos e repositórios. Ela depende dos contratos definidos nas camadas internas, nunca o contrário.

## Escolha do módulo: Auth ou Core

- Determine o módulo pela funcionalidade solicitada, pelo projeto de API de entrada e pelo namespace existente. Não misture os módulos em um mesmo fluxo.
- Use `AuthApi` → `AuthApplication` → `AuthDomain` → `AuthInfrastructure` para recursos de identidade, autenticação, autorização, usuários, papéis, permissões, tokens, login e registro.
- Use `CoreApi` → `CoreApplication` → `CoreDomain` → `CoreInfrastructure` para recursos de negócio que não sejam de autenticação, incluindo `TheBand.Collections`.
- Ao receber uma solicitação ambígua, procure primeiro o controller, entidade, namespace ou projeto relacionado já existente. Se ainda não for possível identificar o módulo, pergunte ao usuário antes de criar código.
- Uma feature iniciada em `CoreApi` deve usar contratos, casos de uso, entidades e repositórios do conjunto `Core*`; uma feature iniciada em `AuthApi` deve usar o conjunto `Auth*`.
- Não faça `Core*` depender de `AuthInfrastructure` ou `Auth*` depender de `CoreInfrastructure`. Quando houver uma integração entre módulos, exponha um contrato na camada Application ou Domain apropriada e injete sua implementação.

## Repositórios e interfaces

- Defina interfaces para abstrações e dependências externas antes de criar suas implementações.
- Coloque a interface do repositório em `AuthDomain` ou `CoreDomain`, conforme o módulo da feature, quando ela fizer parte da linguagem/regra de negócio; caso seja um contrato específico de caso de uso, mantenha-a em `AuthApplication` ou `CoreApplication`.
- Implemente os repositórios concretos em `AuthInfrastructure` ou `CoreInfrastructure`, sempre no mesmo módulo da interface.
- Não acesse `DbContext`, ORM, banco de dados ou serviços externos diretamente em controllers ou casos de uso da Application.
- Mantenha as interfaces pequenas, coesas e orientadas à necessidade do consumidor. Não crie métodos genéricos que não serão utilizados.
- Use operações assíncronas, `CancellationToken` e nomes claros para os métodos públicos quando o padrão do projeto permitir.
- Sempre use Authorize em todas as rotas somente o AuthController não deve ter Authorize

## Injeção de dependências

- Injete todas as dependências por construtor; não instancie repositórios, serviços, `DbContext` ou clientes externos dentro de classes de negócio.
- Registre interfaces e implementações no ponto de composição da aplicação, normalmente no projeto de API.
- Escolha o ciclo de vida correto: repositórios e dependências ligadas ao contexto de requisição normalmente são `Scoped`; serviços sem estado podem ser `Transient` ou `Singleton` apenas quando seguro.
- Garanta que os projetos internos não dependam da API para resolver dependências.

## Clean Code e qualidade

- Use nomes expressivos, classes pequenas e métodos com uma responsabilidade clara.
- Evite duplicação, condicionais extensas, parâmetros em excesso e comentários que apenas repitam o código.
- Valide dados de entrada na borda da aplicação e faça cumprir invariantes importantes no domínio.
- Trate erros de forma consistente, retornando respostas HTTP adequadas sem expor detalhes internos.
- Não altere convenções, contratos públicos ou código não relacionado sem necessidade explícita.
- Sempre pule um linha entre comandos do c# e dos métodos ou atributos
- Sempre use o campo userId nos gets 

## Testes obrigatórios

- Há dois projetos de testes: `TheBand.AuthTests` e `TheBand.CoreTests`.
- Para cada feature do módulo Auth, adicione os testes em `TheBand.AuthTests`. Para cada feature do módulo Core, adicione os testes em `TheBand.CoreTests`.
- Para toda interface nova, crie testes no projeto de testes do respectivo módulo que validem o comportamento da implementação concreta.
- Teste pelo contrato da interface, não por detalhes internos da classe, para que a implementação possa ser substituída sem quebrar os testes.
- Inclua cenários de sucesso, ausência de dados, entradas inválidas e falhas relevantes de infraestrutura quando aplicável.
- Para casos de uso da Application, use mocks/fakes das interfaces e teste suas regras e interações observáveis.
- Para repositórios, prefira testes de integração com uma infraestrutura isolada quando o comportamento depender de persistência real.
- Execute os testes afetados antes de concluir e informe claramente quaisquer testes que não puderam ser executados.

## Checklist de entrega de um CRUD

1. Entidade e regras de domínio criadas ou atualizadas.
2. Contratos/interfaces definidos na camada correta.
3. Caso de uso da Application implementado.
4. Repositório/integração implementado em Infrastructure.
5. Dependências registradas por interface.
6. Endpoint/controller fino na API, com contratos HTTP apropriados.
7. Testes adicionados em `TheBand.AuthTests` ou `TheBand.CoreTests`, conforme o módulo, incluindo os contratos das interfaces novas.
8. Build e testes relevantes executados.
