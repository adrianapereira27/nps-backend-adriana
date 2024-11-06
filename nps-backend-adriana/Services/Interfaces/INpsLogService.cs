using nps_backend_adriana.Models.Dto;

namespace nps_backend_adriana.Services.Interfaces
{
    public interface INpsLogService
    {
        Task<string> CheckSurveyAsync(string login);

        Task<bool> ProcessNpsSurvey(int score, string description, string userId, Guid categoryId);
    }
}
