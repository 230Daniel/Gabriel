using System.Reflection;
using System.Text;

namespace Gabriel.Extensions;

public static class PropertyInfoExtensions
{
    public static string GetSignature(this PropertyInfo property)
    {
        var sigBuilder = new StringBuilder("public ");

        sigBuilder.Append(property.PropertyType.GetPrettyName());
        sigBuilder.Append(' ');
        sigBuilder.Append(property.Name);

        var gettable = property.CanRead;
        var settable = property.GetSetMethod()?.IsPublic == true;

        if (gettable || settable)
        {
            sigBuilder.Append(" {");
            if (gettable)
                sigBuilder.Append(" get;");
            if(settable)
                sigBuilder.Append(" set;");
            sigBuilder.Append(" }");
        }

        return sigBuilder.ToString();
    }
}
