﻿using Microsoft.AspNetCore.Mvc;
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

        public NpsLogController(INpsLogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// Verifica a existência de uma pesquisa NPS para o usuário especificado.
        /// </summary>
        /// <param name="login">Login do usuário</param>
        /// <returns>Pergunta</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Bad Request</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // codigo de retorno
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // codigo de retorno
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

            return StatusCode(500, "Erro ao salvar a nota.");

        }

    }
}