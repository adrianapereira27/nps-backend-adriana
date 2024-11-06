using Newtonsoft.Json;
using nps_backend_adriana.Models.Dto;

namespace nps_backend_adriana.UnitTests.Entities
{
    public class NpsLogDtoTests
    {
        [Fact]
        public void Test_NpsLogDto_DefaultInitialization()
        {
            // Act
            var dto = new NpsLogDto();

            // Assert
            Assert.Equal(0, dto.Score);
            Assert.Null(dto.Description);
            Assert.Equal(0, dto.CategoryNumber);
        }

        [Fact]
        public void Test_NpsLogDto_ValidAssignment()
        {
            // Act
            var dto = new NpsLogDto
            {
                Score = 9,
                Description = "Excelente serviço",
                CategoryNumber = 1
            };

            // Assert
            Assert.Equal(9, dto.Score);
            Assert.Equal("Excelente serviço", dto.Description);
            Assert.Equal(1, dto.CategoryNumber);
        }

        [Fact]
        public void Test_NpsLogDto_Score_OutOfBounds()
        {
            // Act
            var dto = new NpsLogDto
            {
                Score = 11, // Score fora da faixa permitida
                Description = "Erro no serviço",
                CategoryNumber = 2
            };

            // Assert
            Assert.True(dto.Score < 0 || dto.Score > 10, "Score deveria estar entre 0 e 10.");
        }

        [Fact]
        public void Test_NpsLogDto_Description_EmptyString()
        {
            // Act
            var dto = new NpsLogDto
            {
                Score = 8,
                Description = "", // Description vazia
                CategoryNumber = 3
            };

            // Assert
            Assert.Equal("", dto.Description);
        }

        [Fact]
        public void Test_NpsLogDto_CategoryNumber_OutOfBounds()
        {
            // Act
            var dto = new NpsLogDto
            {
                Score = 7,
                Description = "Bom atendimento",
                CategoryNumber = 6 // Categoria fora do intervalo permitido
            };

            // Assert
            Assert.True(dto.CategoryNumber < 1 || dto.CategoryNumber > 5, "CategoryNumber deveria estar entre 1 e 5.");
        }

        [Fact]
        public void Test_NpsLogDto_ValidInstance()
        {
            // Act
            var dto = new NpsLogDto
            {
                Score = 10,
                Description = "Excelente atendimento",
                CategoryNumber = 3
            };

            // Assert
            Assert.Equal(10, dto.Score);
            Assert.Equal("Excelente atendimento", dto.Description);
            Assert.Equal(3, dto.CategoryNumber);
        }

        [Fact]
        public void Test_NpsLogDto_ModifyValues()
        {
            // Act
            var dto = new NpsLogDto
            {
                Score = 7,
                Description = "Bom atendimento",
                CategoryNumber = 2
            };

            // Modify values
            dto.Score = 8;
            dto.Description = "Excelente atendimento";

            // Assert
            Assert.Equal(8, dto.Score);
            Assert.Equal("Excelente atendimento", dto.Description);
            Assert.Equal(2, dto.CategoryNumber);
        }

        [Fact]
        public void Test_NpsLogDto_Serialization()
        {
            // Act
            var dto = new NpsLogDto
            {
                Score = 5,
                Description = "Atendimento ok",
                CategoryNumber = 4
            };
            var json = JsonConvert.SerializeObject(dto);
            var deserializedDto = JsonConvert.DeserializeObject<NpsLogDto>(json);

            // Assert
            Assert.Equal(dto.Score, deserializedDto.Score);
            Assert.Equal(dto.Description, deserializedDto.Description);
            Assert.Equal(dto.CategoryNumber, deserializedDto.CategoryNumber);
        }

    }
}
