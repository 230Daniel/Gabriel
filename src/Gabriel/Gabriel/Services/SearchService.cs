namespace Gabriel.Services;

public class SearchService
{
    private readonly AssemblyService _assemblyService;

    public SearchService(AssemblyService assemblyService)
    {
        _assemblyService = assemblyService;
    }

    public IEnumerable<Type> SearchTypes(string query)
    {
        query = query.ToLower().Trim();
        var types = _assemblyService.GetAssemblies().SelectMany(x => x.GetExportedTypes());

        return types.Where(x => x.Name.ToLower() == query);
    }
}
