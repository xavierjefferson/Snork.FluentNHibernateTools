using NHibernate.Linq;
using NHibernate.Linq.Functions;

namespace Snork.FluentNHibernateTools
{
    public sealed class CustomLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public CustomLinqToHqlGeneratorsRegistry()
        {
            CalculatedPropertyGenerator<object, object>.Register(this, a => null, a => null);
            this.Merge(new CastHqlGeneratorForMethod());

            RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => ObjectExtensions.Coalesce<object>(null, null)),
                new CoalesceHqlGeneratorForMethod());
            RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => ObjectExtensions.Cast<object>(null)),
                new CastHqlGeneratorForMethod());
        }
    }
}