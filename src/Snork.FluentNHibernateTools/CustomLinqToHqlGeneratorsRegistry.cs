using NHibernate.Linq.Functions;
using NHibernate.Util;

namespace Snork.FluentNHibernateTools
{
    public sealed class CustomLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public CustomLinqToHqlGeneratorsRegistry()
        {
            CalculatedPropertyGenerator<object, object>.Register(this, a => null, a => null);
            this.Merge(new CastHqlGeneratorForMethod());

            RegisterGenerator(ReflectHelper.GetMethodDefinition(() => ObjectExtensions.Coalesce<object>(null, null)),
                new CoalesceHqlGeneratorForMethod());
            RegisterGenerator(ReflectHelper.GetMethodDefinition(() => ObjectExtensions.Cast<object>(null)),
                new CastHqlGeneratorForMethod());
        }
    }
}