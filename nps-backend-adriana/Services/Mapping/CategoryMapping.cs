namespace nps_backend_adriana.Services.Mapping
{
    public static class CategoryMapping
    {
        // Mapeamento de inteiros para UUIDs
        public static readonly Dictionary<int, Guid> CategoryIdMap = new Dictionary<int, Guid>
        {
            { 1, Guid.Parse("8656aec6-9f0f-41e1-a94c-49e2d49a5492") }, // OTHER
            { 2, Guid.Parse("e0001d6c-905e-42a0-8f2c-89184a6225da") }, // CRASH
            { 3, Guid.Parse("ab7e4d23-ce17-4049-9856-9f1cea110a7e") }, // SLOWNESS
            { 4, Guid.Parse("438109f9-c8bf-43b1-94a0-a186b758b1e1") }, // INTERFACE
            { 5, Guid.Parse("883fdf80-70a2-4e36-bf0a-a291c1174cba") }, // BUGS
            { 6, Guid.Parse("25301326-f806-42e7-9fd9-4ea1e0ddf396") }  // CONNECTIVITY
        };

        // Pega o UUID correspondente ao inteiro fornecido
        public static Guid GetCategoryId(int categoryNumber)
        {
            if (CategoryIdMap.TryGetValue(categoryNumber, out var categoryId))
            {
                return categoryId;
            }

            throw new ArgumentException("Categoria inválida");
        }

        // Pega o nome da categoria baseado no inteiro
        public static string GetCategoryName(int categoryNumber)
        {
            return categoryNumber switch
            {
                1 => "OTHER",
                2 => "CRASH",
                3 => "SLOWNESS",
                4 => "INTERFACE",
                5 => "BUGS",
                6 => "CONNECTIVITY",
                _ => throw new ArgumentException("Categoria inválida")
            };
        }

    }
}
