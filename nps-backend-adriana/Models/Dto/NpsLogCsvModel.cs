using CsvHelper.Configuration.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace nps_backend_adriana.Models.Dto
{
    [ExcludeFromCodeCoverage]
    public class NpsLogCsvModel
    {
        [Name("Data/Hora")]
        [Index(0)]
        public string Data { get; set; }

        [Name("ID do Sistema")]
        [Index(1)]
        public Guid SystemId { get; set; }

        [Name("Nota")]
        [Index(2)]
        public int Nota { get; set; }

        [Name("Descrição")]
        [Index(3)]
        public string Descricao { get; set; }

        [Name("Categoria")]
        [Index(4)]
        public string Categoria { get; set; }

        [Name("Usuário")]
        [Index(5)]
        public string Usuario { get; set; }
    }
}
