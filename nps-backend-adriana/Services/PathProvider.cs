using nps_backend_adriana.Services.Interfaces;

namespace nps_backend_adriana.Services
{
    public class PathProvider : IPathProvider
    {
        public string GetTempPath()
        {
            return Path.GetTempPath();
        }
    }
}
