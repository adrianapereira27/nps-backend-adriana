namespace nps_backend_adriana.Services.Interfaces
{
    public interface INpsLogExporter
    {
        Task ExportToCsvAsync(string filePath);
    }
}
