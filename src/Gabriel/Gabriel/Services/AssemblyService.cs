using System.Reflection;
using LoxSmoke.DocXml;

namespace Gabriel.Services;

public class AssemblyService
{
    private static readonly string[] AssemblyNames =
    {
        "Disqord",
        "Disqord.Core",
        "Disqord.Gateway",
        "Disqord.Gateway.Api",
        "Disqord.Rest",
        "Disqord.Rest.Api",
        "Disqord.Webhook",
        "Disqord.Bot",
        "Disqord.OAuth2"
    };

    private readonly Dictionary<Assembly, DocXmlReader> _assemblyDocumentation;
    private readonly List<Assembly> _assemblies;

    public AssemblyService()
    {
        _assemblies = new();
        _assemblyDocumentation = new();
        foreach (var assemblyName in AssemblyNames)
        {
            var assembly = Assembly.Load(assemblyName);
            _assemblies.Add(assembly);
            var documentationReader = new DocXmlReader($"{assemblyName}.xml");
            _assemblyDocumentation.Add(assembly, documentationReader);
        }
    }

    public List<Assembly> GetAssemblies() => _assemblies;

    public DocXmlReader GetDocumentation(Assembly assembly) => _assemblyDocumentation[assembly];
}
