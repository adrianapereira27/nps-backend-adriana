# nps-backend-adriana

Este projeto é uma API para o gerenciamento de pesquisas de Net Promoter Score (NPS). Ele permite consultar e salvar feedbacks dos usuários, além de integrar com serviços externos para coletar dados de pesquisa.

## Sumário

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Configuração do Projeto](#configuração-do-projeto)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Endpoints da API](#endpoints-da-api)
- [Testes](#testes)
- [Contribuição](#contribuição)

## Sobre o Projeto

Este projeto foi desenvolvido para facilitar o controle de pesquisas de NPS, onde é possível registrar feedbacks e notas de usuários. A API está configurada para:
- Obter pesquisas existentes para o usuário.
- Registrar novas respostas de pesquisa no banco de dados.
- Integrar com uma API externa para verificar e criar pesquisas.

## Tecnologias Utilizadas

- **ASP.NET Core** 8.0
- **Entity Framework Core** para o acesso ao banco de dados
- **FluentValidation** para validação de dados de entrada
- **Swagger** para documentação da API
- **Moq** e **xUnit** para testes unitários
- **HttpClient** para chamadas de API externas

## Configuração do Projeto

### Pré-requisitos

- .NET 8.0 SDK
- SQL Server ou outro banco de dados suportado pelo Entity Framework
- Visual Studio ou VS Code

### Configurando o `appsettings.json`

Para configurar a aplicação, você precisará de um arquivo `appsettings.json` com os dados necessários para conexão com o banco de dados e URLs das APIs externas:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "sua-string-de-conexão"
  },
  "NpsApiSettings": {
    "CheckSurveyUrl": "https://sua-api-de-check.com/api/question/check",
    "SurveyCreateUrl": "https://sua-api-de-create.com/api/question/create",
    "SystemId": "seu-system-id"
  }
}
````

## Estrutura do Projeto

- **Controllers**: Contém os controladores para gerenciar as requisições de NPS e Feedback.
  - `FeedbackController.cs`
  - `NpsController.cs`
- **Data**: Inclui a configuração do contexto de banco de dados (`ApplicationDbContext.cs`) utilizando Entity Framework.
- **Dtos**: Contém os Data Transfer Objects (DTOs) que facilitam a transferência de dados entre as camadas da aplicação.
  - `FeedbackDto.cs`
  - `NpsDto.cs`
  - `NpsLogDto.cs`
- **Entities**: Contém as classes de entidades, que representam os modelos de dados do sistema.
  - `Feedback.cs`
  - `Nps.cs`
  - `NpsLog.cs`
- **Interfaces**: Define as interfaces para os repositórios, garantindo que a implementação dos repositórios siga as mesmas regras de interface.
- **Repositories**: Implementa as interfaces de repositório para comunicação com o banco de dados.
  - `FeedbackRepository.cs`
  - `NpsRepository.cs`
  - `NpsLogRepository.cs`
- **Services**: Implementa a lógica de negócios de NPS e Feedback.
  - `FeedbackService.cs`
  - `NpsService.cs`
  - `NpsLogService.cs`
- **Validations**: Implementações de validação usando FluentValidation para validar os dados de entrada da API.
