using System.Reflection;
using System.Text;

namespace Gabriel.Extensions;

public static class TypeExtensions
{
    public static string GetPrettyName(this Type type)
    {
        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
            return nullableType.Name + "?";

        if (!(type.IsGenericType && type.Name.Contains('`')))
            return type.Name switch
            {
                "String" => "string",
                "Int32" => "int",
                "Decimal" => "decimal",
                "Boolean" => "bool",
                "Object" => "object",
                "Void" => "void",
                _ => type.Name
            };

        var sb = new StringBuilder(type.Name[..type.Name.IndexOf('`')]);
        sb.Append('<');
        var first = true;
        foreach (var t in type.GetGenericArguments())
        {
            if (!first) sb.Append(',');
            sb.Append(t.GetPrettyName());
            first = false;
        }
        sb.Append('>');
        return sb.ToString();
    }

    public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type, List<Type> visited = null)
    {
        var properties = type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).ToList();
        if (!type.IsInterface) return properties;

        visited ??= new List<Type>();
        var subInterfaces = type.GetInterfaces();

        foreach (var subInterface in subInterfaces.Where(x => !visited.Contains(x)))
            properties.AddRange(GetPublicProperties(subInterface, visited));
        visited.AddRange(subInterfaces);

        return properties;
    }
}
