using Microsoft.AspNetCore.Mvc;
using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Services.Interfaces;
using nps_backend_adriana.Services.Mapping;

namespace nps_backend_adriana.Controllers
{
    [Route("api/nps")]
    [ApiController]
    public class NpsLogController : ControllerBase
    {
        private readonly INpsLogService _logService;

        public NpsLogController(INpsLogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> CheckSurvey()
        {
            var result = await _logService.CheckSurveyAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostSurvey([FromBody] NpsLogDto npsDto)
        {    
            try
            {                
                // Faz o mapeamento do int para o UUID correspondente
                var categoryId = CategoryMapping.GetCategoryId(npsDto.CategoryNumber);
                
                // Chama o serviço para salvar o Log e enviar o JSON para a API externa
                var result = await _logService.ProcessNpsSurvey(
                    npsDto.Score,
                    npsDto.Description,
                    categoryId); // Passa o UUID correspondente

                if (result)
                {
                    return Ok("Nota e log salvos com sucesso.");
                }

                return StatusCode(500, "Erro ao salvar a nota.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
