namespace nps_backend_adriana.Services.Interfaces
{
    public interface INpsLogService
    {
        Task<string> CheckSurveyAsync();

        Task<bool> ProcessNpsSurvey(int score, string description, Guid categoryId);
    }
}
