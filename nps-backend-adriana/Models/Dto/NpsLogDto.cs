using System.Diagnostics.CodeAnalysis;

namespace nps_backend_adriana.Models.Dto
{
    public class NpsLogDto
    {
        public string UserId {  get; set; }
        public int Score { get; set; }
        public string Description { get; set; }
        public int CategoryNumber { get; set; }
    }
}
