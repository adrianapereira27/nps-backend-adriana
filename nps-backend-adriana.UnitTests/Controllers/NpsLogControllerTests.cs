using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using nps_backend_adriana.Controllers;
using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Services;

namespace nps_backend_adriana.UnitTests.Controllers
{
    public class NpsLogControllerTests
    {
        private readonly Mock<NpsLogService> _mockService;  // Mock do service
        private readonly NpsLogController _controller;

        public NpsLogControllerTests()
        {
            _mockService = new Mock<NpsLogService>(null, null);
            _controller = new NpsLogController(_mockService.Object);  // Injeta o mock
        }

        // Testa o método CheckSurvey da controller
        [Fact]
        public async Task CheckSurvey_ReturnsOk_WhenSurveyExists()
        {
            // Arrange
            var expectedResult = "Survey data";
            _mockService.Setup(service => service.CheckSurveyAsync())
                        .ReturnsAsync(expectedResult); // Simula o retorno do método da service

            // Act
            var result = await _controller.CheckSurvey();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(expectedResult);
        }

        // Testa o método PostSurvey da controller com sucesso quando nota >= 7
        [Fact]
        public async Task PostSurvey_ReturnsOk_WhenSurveyIsProcessedSuccessfully()
        {
            // Arrange
            var npsDto = new NpsLogDto { Score = 8, Description = "Good service", CategoryNumber = 0 };
            _mockService.Setup(service => service.ProcessNpsSurvey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()))
                           .ReturnsAsync(true); // Simula sucesso no processamento da service

            // Act
            var result = await _controller.PostSurvey(npsDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be("Nota e log salvos com sucesso.");
        }

        // Testa o método PostSurvey da controller com sucesso quando nota <= 6
        [Fact]
        public async Task PostSurvey_ReturnsOk_WhenSurveyProcessedSuccessfully()
        {
            // Arrange
            var npsDto = new NpsLogDto { Score = 6, Description = "slowness", CategoryNumber = 3 };
            _mockService.Setup(service => service.ProcessNpsSurvey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()))
                           .ReturnsAsync(true); // Simula sucesso no processamento da service

            // Act
            var result = await _controller.PostSurvey(npsDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be("Nota e log salvos com sucesso.");
        }

        [Fact]
        public async Task PostSurvey_ReturnsBadRequest_WhenScoreIsLessThanSevenAndCategoryIsZero()
        {
            // Arrange
            var npsDto = new NpsLogDto
            {
                Score = 5,  // Nota inferior a 7
                Description = "Não gostei do serviço",
                CategoryNumber = 0 // Categoria zerada
            };

            // Não é necessário simular o serviço porque o erro ocorrerá antes da chamada ao service
            _mockService.Setup(service => service.ProcessNpsSurvey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()))
                           .ReturnsAsync(true); // Apenas para garantir que o serviço não será chamado

            // Act
            var result = await _controller.PostSurvey(npsDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);  // Verifica se o status é 400 (Bad Request)
            badRequestResult.Value.Should().Be("Categoria inválida.");  // Verifica se a mensagem de erro é a esperada
        }

        // Testa o método PostSurvey quando o DTO é nulo
        [Fact]
        public async Task PostSurvey_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.PostSurvey(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Dados inválidos.");
        }

        // Testa o método PostSurvey quando a service falha
        [Fact]
        public async Task PostSurvey_ReturnsStatus500_WhenServiceFails()
        {
            // Arrange
            var npsDto = new NpsLogDto { Score = 8, Description = "Good service", CategoryNumber = 1 };
            _mockService.Setup(service => service.ProcessNpsSurvey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()))
                           .ReturnsAsync(false); // Simula falha no processamento da service

            // Act
            var result = await _controller.PostSurvey(npsDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(500);
            statusCodeResult.Value.Should().Be("Erro ao salvar a nota.");
        }

                
    }
}