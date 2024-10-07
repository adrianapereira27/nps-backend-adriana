using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

    }
}
