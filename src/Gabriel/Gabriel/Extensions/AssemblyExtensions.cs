using System.Reflection;
using System.Runtime.CompilerServices;
using Disqord;

namespace Gabriel.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, Type extendedType)
    {
        return from type in assembly.GetTypes()
            where type.IsSealed && !type.IsGenericType && !type.IsNested
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where ExtensionMethodHasTypeAsSubject(method, extendedType)
            select method;
    }

    private static bool ExtensionMethodHasTypeAsSubject(MethodInfo method, Type extendedType)
    {
        if (extendedType.IsAssignableFrom(method.GetParameters()[0].ParameterType))
            return true;

        if (!method.IsGenericMethod)
            return false;

        // Method<T> where T : SomeClass
        // Extended type must be assignable to SomeClass
        // SomeClass must not be typeof object or else we will say true for all generic methods
        var firstGenericArgument = method.GetGenericArguments()[0];
        if (extendedType.IsAssignableTo(firstGenericArgument.BaseType) &&
            firstGenericArgument.BaseType != typeof(object))
            return true;

        return false;
    }
}
