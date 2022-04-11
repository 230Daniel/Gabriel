using System.Reflection;
using Gabriel.Extensions;

namespace Gabriel.Services;

public class DocumentationService
{
    private readonly AssemblyService _assemblyService;

    public DocumentationService(AssemblyService assemblyService)
    {
        _assemblyService = assemblyService;
    }

    public TypeDocumentation GetDocumentation(Type type)
    {
        var documentationReader = _assemblyService.GetDocumentation(type.Assembly);
        var comments = documentationReader.GetTypeComments(type);

        var documentation = new TypeDocumentation()
        {
            Name = type.FullName,
            Summary = comments.Summary,
            Properties = new List<string>(),
            Methods = new List<string>(),
            ExtensionMethods = new List<string>()
        };

        if (string.IsNullOrWhiteSpace(comments.Summary))
            comments.Summary = "This type has no summary.";

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Instance);
        foreach (var method in methods.Where(x =>
                     !x.IsConstructor &&
                     char.IsUpper(x.Name[0]) &&
                     x.DeclaringType != typeof(object) &&
                     (x.IsPublic || x.IsVirtual && !x.IsFinal)))
            documentation.Methods.Add(method.GetSignature());

        var extensionMethods = _assemblyService.GetAssemblies().SelectMany(x => x.GetExtensionMethods(type));
        foreach (var extensionMethod in extensionMethods)
            documentation.ExtensionMethods.Add(extensionMethod.GetSignature());

        var properties = type.GetPublicProperties();
        foreach (var property in properties)
            documentation.Properties.Add(property.GetSignature());

        return documentation;
    }
}

public class TypeDocumentation
{
    public string Name { get; init; }
    public string Summary { get; init; }
    public List<string> Properties { get; init; }
    public List<string> Methods { get; init; }
    public List<string> ExtensionMethods { get; init; }
}
