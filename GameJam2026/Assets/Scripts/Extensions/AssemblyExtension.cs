using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extension
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetClassesOfType<T>(this Assembly assembly)
        {
            // Get all types in the assembly, handling potential loading errors
            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }

            // Filter the types based on the target type T
            return types
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass)
                .ToList();
        }
    }
}