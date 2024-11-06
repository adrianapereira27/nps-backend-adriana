using System.Diagnostics.CodeAnalysis;

namespace nps_backend_adriana.Models.Dto.Settings
{
    [ExcludeFromCodeCoverage]
    public class NpsApiSettings
    {
        public string CheckSurveyUrl { get; set; }
        public string SurveyCreateUrl { get; set; }
        public string SystemId { get; set; }
    }
}
