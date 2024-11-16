namespace nps_backend_adriana.Services.Mapping
{
    public static class CategoryMapping
    {
        // Mapeamento de inteiros para UUIDs e suas descrições
        private static readonly Dictionary<int, (Guid Id, string Description)> CategoryIdMap = new Dictionary<int, (Guid, string)>
        {
            { 1, (Guid.Parse("e0001d6c-905e-42a0-8f2c-89184a6225da"), "PRODUCT ACCESS") },
            { 2, (Guid.Parse("25301326-f806-42e7-9fd9-4ea1e0ddf396"), "CONNECTION/INTERNET") },
            { 3, (Guid.Parse("ab7e4d23-ce17-4049-9856-9f1cea110a7e"), "SLOWNESS/CRASH") },
            { 4, (Guid.Parse("438109f9-c8bf-43b1-94a0-a186b758b1e1"), "INTERFACE/APPEARANCE") },
            { 5, (Guid.Parse("883fdf80-70a2-4e36-bf0a-a291c1174cba"), "BUGS") },
            { 6, (Guid.Parse("8656aec6-9f0f-41e1-a94c-49e2d49a5492"), "OTHER") }
        };

        // Pega o UUID correspondente ao inteiro fornecido
        public static Guid GetCategoryId(int categoryNumber)
        {
            if (CategoryIdMap.TryGetValue(categoryNumber, out var category))
            {
                return category.Id;
            }

            return Guid.Empty;            
        }

        // Pega a descrição correspondente ao UUID fornecido
        public static string GetCategoryDescription(Guid categoryId)
        {
            foreach (var category in CategoryIdMap.Values)
            {
                if (category.Id == categoryId)
                {
                    return category.Description;
                }
            }
            return "Categoria inválida";
        }

    }
}
