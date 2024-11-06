using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;

namespace nps_backend_adriana.UnitTests.Validation
{
    public class ValidationFilterTests
    {
        [Fact]
        public void OnActionExecuting_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            );

            // Simula um erro no ModelState (sem atribuir diretamente)
            actionContext.ModelState.AddModelError("CategoryNumber", "A categoria é obrigatória.");

            // Cria o contexto da ação com um ModelState inválido
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );

            var filter = new Validators.ValidationFilter();

            // Act - Executa o filtro com o ModelState inválido
            filter.OnActionExecuting(context);

            // Assert - Verifica se o resultado foi um BadRequestObjectResult
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);
            var errorMessages = Assert.IsType<List<string>>(result.Value);

            // Verifica se a mensagem de erro esperada está presente
            Assert.Contains("A categoria é obrigatória.", errorMessages);
        }

        [Fact]
        public void OnActionExecuting_ValidModelState_DoesNotReturnResult()
        {
            // Arrange
            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            );

            // Cria o contexto da ação sem erros no ModelState
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );

            var filter = new Validators.ValidationFilter();

            // Act - Executa o filtro com o ModelState válido
            filter.OnActionExecuting(context);

            // Assert - Verifica que o filtro não alterou o resultado (resultado é null)
            Assert.Null(context.Result);
        }
    }
}
