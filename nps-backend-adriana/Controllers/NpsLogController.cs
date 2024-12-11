using Microsoft.AspNetCore.Mvc;
using nps_backend_adriana.Exceptions;
using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Services.Interfaces;
using nps_backend_adriana.Services.Mapping;

namespace nps_backend_adriana.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de logs do NPS
    /// </summary>
    [Route("api/nps")]
    [ApiController]
    public class NpsLogController : ControllerBase
    {
        private readonly INpsLogService _logService;
        private readonly INpsLogExporter _npsExporter;
        private readonly ILogger<NpsLogController> _logger;
        private readonly IPathProvider _pathProvider;

        public NpsLogController(INpsLogService logService, INpsLogExporter npsExporter, ILogger<NpsLogController> logger, IPathProvider pathProvider)
        {
            _logService = logService;
            _npsExporter = npsExporter;
            _logger = logger;
            _pathProvider = pathProvider;
        }

        /// <summary>
        /// Verifica a existência de uma pesquisa NPS para o usuário especificado.
        /// </summary>
        /// <param name="login">Login do usuário</param>
        /// <returns>Pergunta</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">NotFound</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // codigo de retorno
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // codigo de retorno
        [ProducesResponseType(StatusCodes.Status404NotFound)] // codigo de retorno
        public async Task<IActionResult> CheckSurvey([FromQuery] PerguntaDto login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _logService.CheckSurveyAsync(login.UserId);

                return Ok(result);
            }
            catch (NpsException ex) when (ex.ErrorCode == 404)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (NpsException ex)
            {
                return StatusCode(ex.ErrorCode, ex.Message);
            }

        }

        /// <summary>
        /// Processa e registra a resposta de uma pesquisa NPS.
        /// </summary>
        /// <param name ="score"> Pontuação dada pelo usuário.</param>
        /// <param name="description">Descrição ou feedback fornecido pelo usuário.</param>
        /// <param name="userId">ID do usuário que respondeu à pesquisa.</param>
        /// <param name="categoryId">ID da categoria relacionada à pesquisa.</param>
        /// <returns>Retorna um status de sucesso ou falha.</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)] // codigo de retorno
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // codigo de retorno
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // codigo de retorno
        public async Task<IActionResult> PostSurvey([FromBody] NpsLogDto npsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var categoryId = CategoryMapping.GetCategoryId(npsDto.CategoryNumber);

                // Chama o serviço para salvar o Log e enviar o JSON para a API externa
                var result = await _logService.ProcessNpsSurvey(
                    npsDto.Score,
                    npsDto.Description,
                    npsDto.UserId,
                    categoryId); // Passa o UUID correspondente

                if (result)
                {
                    return Ok("Nota e log salvos com sucesso.");
                }

                throw new NpsException("Erro ao salvar a nota.", 500);
            }
            catch (NpsException ex)
            {
                // Se a exceção NpsException for capturada, podemos retornar um StatusCode adequado
                return StatusCode(ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                // Caso ocorra qualquer outra exceção, retornamos um StatusCode genérico para erro
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }

        }

        /// <summary>
        /// Exporta os registros de NPS para um arquivo CSV.
        /// O arquivo é gerado temporariamente e, em seguida, retornado ao usuário para download.        
        /// </summary>
        /// <returns>Um arquivo CSV com os dados de NPS ou um erro caso ocorra uma falha durante o processo.</returns>
        /// <response code="200">Arquivo CSV gerado com sucesso.</response>
        /// <response code="500">Erro inesperado durante a geração do arquivo CSV.</response>        
        [HttpGet("export")]
        public async Task<IActionResult> ExportCsv()
        {
            try
            {
                // Criar nome do arquivo com timestamp
                var fileName = $"nps_export_{DateTime.Now:yyyyMMddHHmmss}.csv";
                var tempPath = Path.Combine(_pathProvider.GetTempPath(), fileName);

                // Gerar o CSV
                await _npsExporter.ExportToCsvAsync(tempPath);

                // Ler o arquivo e retornar como download
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(tempPath);

                // Limpar o arquivo temporário
                System.IO.File.Delete(tempPath);

                // Configurar os headers manualmente
                Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                return File(
                    fileContents: fileBytes,
                    contentType: "application/octet-stream",
                    fileDownloadName: fileName
                );
            }
            catch (NpsException ex)
            {
                // Retorna o código de erro personalizado e a mensagem da NpsException
                _logger.LogError(ex, "Erro específico ao exportar CSV");
                return StatusCode(ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                // Retorna 500 para qualquer erro inesperado
                _logger.LogError(ex, "Erro inesperado ao exportar CSV");
                return StatusCode(500, new { error = $"Erro inesperado: {ex.Message}" });
            }
        }

    }
}
