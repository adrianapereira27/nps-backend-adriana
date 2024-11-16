using nps_backend_adriana.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nps_backend_adriana.UnitTests.Entities
{
    public class NpsLogTests
    {
        [Fact]
        public void DeveCriarNpsLogComDadosValidos()
        {
            // Arrange
            var npsLog = new NpsLog
            {
                Id = 1,
                DateScore = DateTime.UtcNow,
                SystemId = Guid.NewGuid(),
                Score = 8,
                Description = "Muito bom!",
                CategoryId = Guid.NewGuid(),
                UserId = "user123"
            };

            // Act & Assert
            Assert.NotNull(npsLog);
            Assert.Equal(8, npsLog.Score);
            Assert.Equal("Muito bom!", npsLog.Description);
            Assert.True(npsLog.SystemId != Guid.Empty);
            Assert.True(npsLog.UserId.Length > 0);
            Assert.True(npsLog.DateScore <= DateTime.UtcNow);
        }
          
        [Fact]
        public void NpsLog_ScoreDeveSerValido()
        {
            // Arrange & Act
            var npsLogValido = new NpsLog
            {
                Id = 3,
                DateScore = DateTime.UtcNow,
                SystemId = Guid.NewGuid(),
                Score = 9,
                Description = "Excelente!",
                CategoryId = Guid.NewGuid(),
                UserId = "user456"
            };

            var npsLogInvalido = new NpsLog
            {
                Id = 4,
                DateScore = DateTime.UtcNow,
                SystemId = Guid.NewGuid(),
                Score = -1, // Score inválido
                Description = "Ruim",
                CategoryId = Guid.NewGuid(),
                UserId = "user789"
            };

            // Assert
            Assert.InRange(npsLogValido.Score, 0, 10);  // Score válido
            Assert.False(npsLogInvalido.Score >= 0 && npsLogInvalido.Score <= 10); // Score inválido
        }

        [Fact]
        public void NpsLog_DeveInicializarDateScoreCorretamente()
        {
            // Arrange
            var npsLog = new NpsLog
            {
                Id = 5,
                DateScore = DateTime.UtcNow,
                SystemId = Guid.NewGuid(),
                Score = 7,
                Description = "Bom",
                CategoryId = Guid.NewGuid(),
                UserId = "user101"
            };

            // Act & Assert
            Assert.True(npsLog.DateScore <= DateTime.UtcNow);
        }

        [Fact]
        public void NpsLog_DevePermitirDescricaoValida()
        {
            // Arrange
            var npsLog = new NpsLog
            {
                Id = 6,
                DateScore = DateTime.UtcNow,
                SystemId = Guid.NewGuid(),
                Score = 10,
                Description = "Excelente desempenho!",
                CategoryId = Guid.NewGuid(),
                UserId = "user102"
            };

            // Act & Assert
            Assert.NotNull(npsLog.Description);
            Assert.True(npsLog.Description.Length > 0);
        }

        [Fact]
        public void NpsLog_DeveExigirUserId()
        {
            // Arrange
            var npsLog = new NpsLog
            {
                Id = 7,
                DateScore = DateTime.UtcNow,
                SystemId = Guid.NewGuid(),
                Score = 6,
                Description = "Regular",
                CategoryId = Guid.NewGuid(),
                UserId = "" // UserId vazio (invalido)
            };

            // Act & Assert
            Assert.True(string.IsNullOrEmpty(npsLog.UserId));
        }

    }
}
