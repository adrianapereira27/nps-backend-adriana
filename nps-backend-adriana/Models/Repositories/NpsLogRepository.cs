using nps_backend_adriana.Models.Entities;
using nps_backend_adriana.Models.Interfaces;
using nps_backend_adriana.Services;

namespace nps_backend_adriana.Models.Repositories
{
    public class NpsLogRepository : INpsLogRepository
    {
        private readonly NpsDbContext _context;

        public NpsLogRepository(NpsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NpsLog npslog)
        {
            await _context.NpsLog.AddAsync(npslog);
            await _context.SaveChangesAsync();
        }
    }
}
