using nps_backend_adriana.Models.Entities;
using nps_backend_adriana.Models.Interfaces;

namespace nps_backend_adriana.Services
{
    public class NpsLogService
    {
        private readonly INpsLogRepository _npsLogRepository;
        private readonly HttpClient _httpClient;        
        private const string systemId = "3c477fc7-0d4d-458a-6078-08dc43a0a620";
        private const string userName = "adriana4";

        public NpsLogService(INpsLogRepository npsLogRepository, HttpClient httpClient)
        {
            _npsLogRepository = npsLogRepository;
            _httpClient = httpClient;
        }

        public NpsLog IncluirNpsLog(NpsLog entity)
        {
            var npsLog = new NpsLog()
            {
                SystemId = new Guid(systemId),
                DateScore = DateTime.Now,
                CategoryId = entity.CategoryId,
                Description = entity.Description,
                Score = entity.Score,
                UserId = userName
            };

            _npsLogRepository.AddAsync(npsLog);

            return npsLog;
        }

        public async Task<string> CheckSurveyAsync()
        {
            var url = $"https://nps-stg.ambevdevs.com.br/api/question/check?user={userName}&systemId={systemId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", systemId); // Adiciona o systemId no cabeçalho de autenticação

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }


    }
}
