using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Shared
{
    public static class Extensions
    {
        public static IEnumerable<Type> GetInheritingTypes<T>(this Assembly assembly)
        {
            var types = assembly.DefinedTypes.Where(
                p => p.DeclaredConstructors.Any(
                         q => q.IsPublic) && typeof(T).GetTypeInfo().IsAssignableFrom(p) &&
                     !p.IsAbstract);
            return types.ToList();
        }
    }
}
