using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using nps_backend_adriana.Exceptions;
using nps_backend_adriana.Models.Dto;
using nps_backend_adriana.Models.Entities;
using nps_backend_adriana.Services.Interfaces;
using nps_backend_adriana.Services.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace nps_backend_adriana.Services
{
    [ExcludeFromCodeCoverage]
    public class NpsLogExporter : INpsLogExporter
    {
        private readonly string _connectionString;
        private readonly ILogger<NpsLogExporter> _logger;

        public NpsLogExporter(string connectionString, ILogger<NpsLogExporter> logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger;
        }

        public async Task ExportToCsvAsync(string outputPath)
        {
            try
            {
                _logger?.LogInformation("Iniciando exportação do CSV");

                // Buscar dados do banco
                var npsLogs = await GetNpsLogsAsync();

                _logger?.LogInformation($"Encontrados {npsLogs.Count} registros para exportação");

                // Configurações de escrita do CSV
                using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";"
                }))
                {
                    // Escrever cabeçalho personalizado
                    csv.WriteHeader<NpsLogCsvModel>();
                    await csv.NextRecordAsync();

                    // Escrever registros
                    foreach (var log in npsLogs)
                    {
                        // Obter descrição da categoria
                        var categoria = log.CategoryId != Guid.Empty
                            ? CategoryMapping.GetCategoryDescription(log.CategoryId)
                            : string.Empty;

                        // Criar o modelo CSV
                        var csvModel = new NpsLogCsvModel
                        {
                            Data = log.DateScore.ToString("dd/MM/yyyy HH:mm:ss"),
                            SystemId = log.SystemId,
                            Nota = log.Score,
                            Descricao = log.Description ?? string.Empty,
                            Categoria = categoria,
                            Usuario = log.UserId ?? string.Empty
                        };

                        csv.WriteRecord(csvModel);
                        await csv.NextRecordAsync();
                    }
                }

                _logger?.LogInformation("Exportação concluída com sucesso");
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "Erro de conexão com o banco de dados");
                throw new NpsException("Erro ao conectar ao banco de dados", 503, ex);
            }
            catch (IOException ex)
            {
                _logger?.LogError(ex, "Erro ao escrever no arquivo CSV");
                throw new NpsException("Erro ao salvar o arquivo CSV", 500, ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro inesperado durante a exportação do CSV");
                throw new NpsException("Erro inesperado durante a exportação", 500, ex);
            }
        }

        private async Task<List<NpsLog>> GetNpsLogsAsync()
        {
            var npsLogs = new List<NpsLog>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand(
                        @"SELECT DateScore, SystemId, Score, Description, CategoryId, UserId 
                          FROM NpsLogs 
                          ORDER BY DateScore DESC", connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            npsLogs.Add(new NpsLog
                            {
                                DateScore = reader.GetDateTime(0),
                                SystemId = reader.GetGuid(1),
                                Score = reader.GetInt32(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CategoryId = reader.IsDBNull(4) ? Guid.Empty : reader.GetGuid(4),
                                UserId = reader.IsDBNull(5) ? null : reader.GetString(5)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger?.LogError(ex, "Erro ao executar a consulta SQL");
                throw new NpsException("Erro ao buscar dados do banco de dados", 500, ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro inesperado ao buscar dados do banco");
                throw new NpsException("Erro inesperado ao buscar dados", 500, ex);
            }

            return npsLogs;
        }

    }
}
