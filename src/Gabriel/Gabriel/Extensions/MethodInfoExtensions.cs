using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gabriel.Extensions;

public static class MethodInfoExtensions
{
    public static string GetSignature(this MethodInfo method)
    {
        var sigBuilder = new StringBuilder();

        if (method.IsPublic)
            sigBuilder.Append("public ");
        if (method.IsPrivate)
            sigBuilder.Append("private ");
        if (method.IsAssembly)
            sigBuilder.Append("internal ");
        if (method.IsFamily)
            sigBuilder.Append("protected ");
        if (method.IsStatic)
            sigBuilder.Append("static ");
        if (method.IsAbstract)
            sigBuilder.Append("abstract ");
        if (method.IsVirtual)
            sigBuilder.Append("virtual ");
        if (method.IsFinal)
            sigBuilder.Append("sealed ");
        if (method.CustomAttributes.Any(x => x.AttributeType == typeof(AsyncStateMachineAttribute)))
            sigBuilder.Append("async ");

        sigBuilder.Append(method.ReturnType.GetPrettyName());
        sigBuilder.Append(' ');
        sigBuilder.Append(method.Name);

        if (method.IsGenericMethod)
        {
            sigBuilder.Append('<');
            sigBuilder.Append(string.Join(", ", method.GetGenericArguments().Select(x => x.GetPrettyName())));
            sigBuilder.Append('>');
        }

        sigBuilder.Append('(');

        if (method.IsDefined(typeof(ExtensionAttribute), false))
            sigBuilder.Append("this ");

        sigBuilder.Append(string.Join(", ", method.GetParameters().Select(param =>
        {
            var paramBuilder = new StringBuilder();

            if (param.IsIn)
                paramBuilder.Append("in ");
            else if (param.IsOut)
                paramBuilder.Append("out ");
            else if (param.ParameterType.IsByRef)
                paramBuilder.Append("ref ");

            paramBuilder.Append(param.ParameterType.GetPrettyName());
            paramBuilder.Append(' ');
            paramBuilder.Append(param.Name);

            if (param.HasDefaultValue)
            {
                paramBuilder.Append(" = ");
                paramBuilder.Append(param.DefaultValue ?? "null");
            }

            return paramBuilder.ToString();
        })));

        sigBuilder.Append(')');
        return sigBuilder.ToString();
    }
}
