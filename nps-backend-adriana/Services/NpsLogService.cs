using Microsoft.Extensions.Options;
using nps_backend_adriana.Models.Dto.Settings;
using nps_backend_adriana.Models.Entities;
using nps_backend_adriana.Models.Interfaces;
using nps_backend_adriana.Services.Interfaces;
using System.Net;

namespace nps_backend_adriana.Services
{
    public class NpsLogService : INpsLogService
    {
        private readonly INpsLogRepository _npsLogRepository;
        private readonly HttpClient _httpClient;
        private readonly string _checkSurveyUrl;
        private readonly string _surveyCreateUrl;
        private readonly string _systemId;

        public NpsLogService(INpsLogRepository npsLogRepository, HttpClient httpClient, IOptions<NpsApiSettings> npsApiSettings)
        {
            _npsLogRepository = npsLogRepository;
            _httpClient = httpClient;
            _checkSurveyUrl = npsApiSettings.Value.CheckSurveyUrl;
            _surveyCreateUrl = npsApiSettings.Value.SurveyCreateUrl;
            _systemId = npsApiSettings.Value.SystemId;
        }

        public async Task<string> CheckSurveyAsync(string login)
        {
            var url = $"{_checkSurveyUrl}?user={login}&systemId={_systemId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", _systemId); // Adiciona o systemId no cabeçalho de autenticação

            try
            {
                var response = await _httpClient.SendAsync(request);   // Envia a requisição para a API externa

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new Exception("Usuário não tem pesquisa para responder!");
                }
                else
                {
                    response.EnsureSuccessStatusCode();   // Verifica se o status da resposta é sucesso (2xx)

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Erro ao gerar pesquisa NPS!");
            }
        }

        public async Task<bool> ProcessNpsSurvey(int score, string description, string userId, Guid categoryId)
        {
            // Constrói o objeto que será enviado no JSON
            var postData = new
            {
                createdDate = DateTime.Now,
                score,
                comments = description,
                user = userId,
                surveyType = 0,
                systemId = _systemId,
                categoryId
            };

            // Configura a requisição
            var request = new HttpRequestMessage(HttpMethod.Post, _surveyCreateUrl)
            {
                Content = JsonContent.Create(postData) // Adiciona o objeto no corpo da requisição como JSON
            };
            request.Headers.Add("Authorization", _systemId); // Adiciona o systemId no cabeçalho de autenticação

            var response = await _httpClient.SendAsync(request);  // Envia a requisição para a API externa

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            // salva log no banco de dados
            var npsLog = new NpsLog()
            {
                SystemId = new Guid(_systemId),
                DateScore = DateTime.Now,
                CategoryId = categoryId,
                Description = description,
                Score = score,
                UserId = userId
            };

            await _npsLogRepository.AddAsync(npsLog);

            return true;
        }

    }
}
