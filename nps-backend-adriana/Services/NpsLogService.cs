using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Models.Entities;
using nps_backend_adriana.Models.Interfaces;

namespace nps_backend_adriana.Services
{
    public class NpsLogService
    {
        private readonly INpsLogRepository _npsLogRepository;
        private readonly HttpClient _httpClient;
        private const string systemId = "3c477fc7-0d4d-458a-6078-08dc43a0a620";
        private const string user = "adriana4";

        public NpsLogService(INpsLogRepository npsLogRepository, HttpClient httpClient)
        {
            _npsLogRepository = npsLogRepository;
            _httpClient = httpClient;
        }

        public async Task<string> CheckSurveyAsync()
        {
            var url = $"https://nps-stg.ambevdevs.com.br/api/question/check?user={user}&systemId={systemId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", systemId); // Adiciona o systemId no cabeçalho de autenticação

            var response = await _httpClient.SendAsync(request);   // Envia a requisição para a API externa
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> ProcessNpsSurvey(NpsLogDto dto)
        {
            // Constrói o objeto que será enviado no JSON
            var postData = new
            {
                createdDate = DateTime.UtcNow,
                score = dto.Score,
                comments = dto.Description,
                user,
                surveyType = 0,
                systemId,
                categoryId = dto.CategoryId
            };

            // envia nota para API externa
            var externalApiUrl = "https://nps-stg.ambevdevs.com.br/api/survey/create";
            var request = new HttpRequestMessage(HttpMethod.Post, externalApiUrl)
            {
                Content = JsonContent.Create(postData) // Adiciona o objeto no corpo da requisição como JSON
            };
            request.Headers.Add("Authorization", systemId); // Adiciona o systemId no cabeçalho de autenticação
                        
            var response = await _httpClient.SendAsync(request);  // Envia a requisição para a API externa

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            // salva log no banco de dados
            var npsLog = new NpsLog()
            {
                SystemId = new Guid(systemId),
                DateScore = DateTime.Now,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                Score = dto.Score,
                UserId = user
            };
            await _npsLogRepository.AddAsync(npsLog);

            return true;
        }
                
    }
}
