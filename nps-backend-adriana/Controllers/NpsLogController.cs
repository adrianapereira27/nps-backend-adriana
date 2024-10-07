using Microsoft.AspNetCore.Mvc;
using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Services;

namespace nps_backend_adriana.Controllers
{
    [Route("api/nps")]
    [ApiController]
    public class NpsLogController : ControllerBase
    {
        private readonly NpsLogService _logService;

        public NpsLogController(NpsLogService logService)
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
            if (npsDto == null)
            {
                return BadRequest("Dados inválidos.");
            }

            var result = await _logService.ProcessNpsSurvey(npsDto);

            if (result)
            {
                return Ok("Nota salva com sucesso.");
            }

            return StatusCode(500, "Erro ao salvar a nota.");
        }

    }
}
