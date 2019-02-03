namespace Snork.FluentNHibernateTools
{
    internal class DefaultHqlGeneratorForMethod : CompositeHqlGeneratorForMethod
    {
        public DefaultHqlGeneratorForMethod() : base(new CastHqlGeneratorForMethod(),
            new CoalesceHqlGeneratorForMethod())
        {
            
        }
    }
}