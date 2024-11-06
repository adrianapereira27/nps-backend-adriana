using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using nps_backend_adriana.Exceptions;
using nps_backend_adriana.Models.Dto.Settings;
using nps_backend_adriana.Models.Entities;
using nps_backend_adriana.Models.Interfaces;
using nps_backend_adriana.Services;
using System.Net;

namespace nps_backend_adriana.UnitTests.Services
{
    public class NpsLogServiceTests
    {
        private readonly Mock<INpsLogRepository> _mockRepository;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _mockHttpClient;
        private readonly NpsLogService _npsLogService;

        public NpsLogServiceTests()
        {
            _mockRepository = new Mock<INpsLogRepository>();

            // Cria um HttpClient falso que pode ser controlado pelo mock
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object);

            // Mockando as opções de configuração
            var mockOptions = Options.Create(new NpsApiSettings
            {
                CheckSurveyUrl = "https://mock-nps-url.com/api/question/check",
                SurveyCreateUrl = "https://mock-nps-url.com/api/survey/create",
                SystemId = "3c477fc7-0d4d-458a-6078-08dc43a0a620"
            });

            // Injetando as dependências
            _npsLogService = new NpsLogService(_mockRepository.Object, _mockHttpClient, mockOptions);
        }

        // Teste quando a API retorna uma resposta válida (200 OK)
        [Fact]
        public async Task CheckSurveyAsync_ReturnsContent_WhenSurveyAvailable()
        {
            // Arrange
            var expectedContent = "{\"survey\": \"some data\"}";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent) // Simula um JSON de resposta
            };
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _npsLogService.CheckSurveyAsync(It.IsAny<string>());

            // Assert
            result.Should().Be(expectedContent);
        }

        // Teste quando a API retorna 204 (NoContent)
        [Fact]
        public async Task CheckSurveyAsync_ReturnsMessage_WhenNoSurveyAvailable()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NoContent); // Simula resposta 204
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act e assert
            var exception = await Assert.ThrowsAsync<NpsException>(() => _npsLogService.CheckSurveyAsync(It.IsAny<string>()));
            exception.Message.Should().Be("Usuário não tem pesquisa para responder!");  // Verifica se a mensagem da exceção está correta

        }

        // Testa retorno de erro para cair no catch do método CheckSurveyAsync
        [Fact]
        public async Task CheckSurveyAsync_WhenHttpRequestExceptionIsThrown_ShouldReturnErrorMessage()
        {
            // Arrange           
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException()); // Simula uma exceção HttpRequestException

            // Act e assert
            var exception = await Assert.ThrowsAsync<NpsException>(() => _npsLogService.CheckSurveyAsync(It.IsAny<string>()));
            exception.Message.Should().Be("Erro ao gerar pesquisa NPS!");  // Verifica se a mensagem da exceção está correta
        }

        // Teste quando o envio da nota para a API externa é bem-sucedido
        [Fact]
        public async Task ProcessNpsSurvey_ReturnsTrue_WhenSurveyProcessedSuccessfully()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK); // Simula sucesso na chamada à API
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<NpsLog>()))
                           .Returns(Task.CompletedTask); // Simula o salvamento no repositório

            // Act
            var result = await _npsLogService.ProcessNpsSurvey(8, "Good service", "login", Guid.NewGuid());

            // Assert
            result.Should().BeTrue();
        }

        // Teste quando a API externa retorna erro
        [Fact]
        public async Task ProcessNpsSurvey_ReturnsFalse_WhenApiCallFails()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError); // Simula erro da API
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _npsLogService.ProcessNpsSurvey(8, "Good service", "login", Guid.NewGuid());

            // Assert
            result.Should().BeFalse();
        }

        // Teste quando o salvamento do log no banco de dados é bem-sucedido
        [Fact]
        public async Task ProcessNpsSurvey_CallsRepository_WhenSurveyProcessedSuccessfully()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK); // Simula sucesso na chamada à API
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _npsLogService.ProcessNpsSurvey(8, "Good service", "login", Guid.NewGuid());

            // Assert
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<NpsLog>()), Times.Once); // Verifica se o método foi chamado
        }

    }
}
