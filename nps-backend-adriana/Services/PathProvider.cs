using nps_backend_adriana.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace nps_backend_adriana.Services
{
    public class PathProvider : IPathProvider
    {
        [ExcludeFromCodeCoverage]
        public string GetTempPath()
        {
            return Path.GetTempPath();
        }
    }
}
