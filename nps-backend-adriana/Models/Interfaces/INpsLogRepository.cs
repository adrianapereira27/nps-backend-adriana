using nps_backend_adriana.Models.Entities;

namespace nps_backend_adriana.Models.Interfaces
{
    public interface INpsLogRepository
    {
        Task AddAsync(NpsLog npslog);
    }
}
