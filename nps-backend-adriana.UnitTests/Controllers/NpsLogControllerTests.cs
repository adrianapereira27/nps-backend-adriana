using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using nps_backend_adriana.Controllers;
using nps_backend_adriana.Exceptions;
using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Services.Interfaces;
using System.Text;

namespace nps_backend_adriana.UnitTests.Controllers
{
    public class NpsLogControllerTests
    {
        private readonly Mock<INpsLogService> _mockService;  // Mock do service
        private readonly Mock<INpsLogExporter> _mockExporter;  // Mock do exporter
        private readonly Mock<IPathProvider> _mockPathProvider;  // Mock do PathProvider       
        private readonly Mock<ILogger<NpsLogController>> _mockLogger;  // Mock do logger
        private readonly NpsLogController _controller;
        private string _tempDirectory;

        public NpsLogControllerTests()
        {
            _mockService = new Mock<INpsLogService>();
            _mockExporter = new Mock<INpsLogExporter>();
            _mockLogger = new Mock<ILogger<NpsLogController>>();
            _mockPathProvider = new Mock<IPathProvider>();            
            _tempDirectory = Path.Combine(@"C:\Temp\", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
            _mockPathProvider.Setup(p => p.GetTempPath()).Returns(_tempDirectory);

            _controller = new NpsLogController(_mockService.Object, _mockExporter.Object, _mockLogger.Object, _mockPathProvider.Object);  // Injeta os mocks
        }

        // Testa o método CheckSurvey da controller com sucesso
        [Fact]
        public async Task CheckSurvey_ReturnsOk_WhenSurveyExists()
        {
            // Arrange
            var expectedResult = "Survey data";
            var perguntaDto = new PerguntaDto { UserId = "12345" };
            _mockService.Setup(service => service.CheckSurveyAsync(It.IsAny<string>()))
                        .ReturnsAsync(expectedResult); // Simula o retorno do método da service

            // Act
            var result = await _controller.CheckSurvey(perguntaDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(expectedResult);
        }

        // Testa o método CheckSurvey da controller com falha retornando 404
        [Fact]
        public async Task CheckSurvey_ReturnsNotFound_WhenNpsException404()
        {
            // Arrange
            var login = new PerguntaDto { UserId = "1234" };
            _mockService.Setup(x => x.CheckSurveyAsync(login.UserId))
                .ThrowsAsync(new NpsException("Survey not found", 404));

            // Act
            var result = await _controller.CheckSurvey(login);

            // Assert
            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = Assert.IsAssignableFrom<object>(objectResult.Value);
            var message = value.GetType().GetProperty("message").GetValue(value);
            Assert.Equal("Survey not found", message);
        }        

        // Testa o método CheckSurvey da controller com falha retornando 500
        [Fact]
        public async Task CheckSurvey_ReturnsStatusCode_WhenNpsException()
        {
            // Arrange
            var login = new PerguntaDto { UserId = "1234" };
            _mockService.Setup(x => x.CheckSurveyAsync(login.UserId))
                .ThrowsAsync(new NpsException("Internal Server Error", 500));

            // Act
            var result = await _controller.CheckSurvey(login);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal Server Error", objectResult.Value);
        }

        // Testa o método PostSurvey da controller com sucesso quando nota >= 7
        [Fact]
        public async Task PostSurvey_ReturnsOk_WhenSurveyIsProcessedSuccessfully()
        {
            // Arrange
            var npsDto = new NpsLogDto { Score = 8, Description = "Good service", CategoryNumber = 0 };
            _mockService.Setup(service => service.ProcessNpsSurvey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
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
            _mockService.Setup(service => service.ProcessNpsSurvey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
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
            _mockService.Setup(service => service.ProcessNpsSurvey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
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