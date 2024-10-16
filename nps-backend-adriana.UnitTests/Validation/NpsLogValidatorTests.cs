using FluentValidation.TestHelper;
using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Validators;

namespace nps_backend_adriana.UnitTests.Validation
{
    public class NpsLogValidatorTests
    {
        private readonly NpsLogValidator _validator;

        public NpsLogValidatorTests()
        {
            _validator = new NpsLogValidator();
        }

        [Fact]
        public void Test_Score_NotEqualZero_ShouldHaveError()
        {
            // Arrange
            var dto = new NpsLogDto { Score = 0 };

            // Act & Assert
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Score)
                .WithErrorMessage("Obrigatório informar uma nota");
        }

        [Fact]
        public void Test_Score_NotEqualZero_ShouldNotHaveError()
        {
            // Arrange
            var dto = new NpsLogDto { Score = 8 };

            // Act & Assert
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Score);
        }

        [Fact]
        public void Test_Description_MaxLength_ShouldHaveError()
        {
            // Arrange
            var dto = new NpsLogDto
            {
                Description = new string('a', 151) // Excede o limite de 150 caracteres
            };

            // Act & Assert
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Tamanho máximo da Descrição é de 150 caracteres");
        }

        [Fact]
        public void Test_Description_MaxLength_ShouldNotHaveError()
        {
            // Arrange
            var dto = new NpsLogDto
            {
                Description = new string('a', 150) // Dentro do limite de 150 caracteres
            };

            // Act & Assert
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

    }
}
