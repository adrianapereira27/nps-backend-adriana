using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using nps_backend_adriana.Models.Entities;
using nps_backend_adriana.Models.Repositories;
using nps_backend_adriana.Services;

namespace nps_backend_adriana.UnitTests.Repositories
{
    public class NpsLogRepositoryTests
    {
        // teste que garante que a entidade NpsLog é salva corretamente no banco de dados
        [Fact]
        public async Task AddAsync_ShouldAddNpsLogToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<NpsDbContext>()
                .UseInMemoryDatabase(databaseName: "NpsLogTestDb")  // Usando banco de dados InMemory para testes
                .Options;

            // Cria o contexto com as opções acima
            using var context = new NpsDbContext(options);
            var repository = new NpsLogRepository(context);

            // Cria uma entidade NpsLog para ser adicionada
            var npsLog = new NpsLog
            {
                Id = 1,
                SystemId = Guid.NewGuid(),
                DateScore = DateTime.Now,
                CategoryId = Guid.NewGuid(),
                Description = "Teste de adição",
                Score = 8,
                UserId = "user_test"
            };

            // Act
            await repository.AddAsync(npsLog);

            // Assert
            var logFromDb = await context.NpsLog.FirstOrDefaultAsync(l => l.Id == npsLog.Id);
            logFromDb.Should().NotBeNull(); // Verifica se o registro foi adicionado ao banco
            logFromDb.Description.Should().Be("Teste de adição");  // Verifica se o campo Description foi salvo corretamente
            logFromDb.Score.Should().Be(8);  // Verifica se o campo Score foi salvo corretamente
        }

        // teste que retorna ArgumentNullException se for passado um objeto nulo
        [Fact]
        public async Task AddAsync_ShouldThrowArgumentNullException_WhenNpsLogIsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<NpsDbContext>()
                .UseInMemoryDatabase(databaseName: "NpsLogTestDb_NullCheck")
                .Options;

            using var context = new NpsDbContext(options);
            var repository = new NpsLogRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddAsync(null));
        }

    }
}
