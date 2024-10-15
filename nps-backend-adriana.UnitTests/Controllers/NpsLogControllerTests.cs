using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using nps_backend_adriana.Controllers;
using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Services.Interfaces;

namespace nps_backend_adriana.UnitTests.Controllers
{
    public class NpsLogControllerTests
    {
        private readonly Mock<INpsLogService> _mockService;  // Mock do service
        private readonly NpsLogController _controller;

        public NpsLogControllerTests()
        {
            _mockService = new Mock<INpsLogService>();
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