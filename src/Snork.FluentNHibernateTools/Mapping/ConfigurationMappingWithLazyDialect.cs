using System;
using FluentNHibernate.Cfg.Db;

namespace Snork.FluentNHibernateTools.Mapping
{
    internal class
        ConfigurationMappingWithLazyDialect<TPersistenceConfiguration, TConnectionStringBuilder> :
            RootConfigurationMapping<
                TPersistenceConfiguration, TConnectionStringBuilder>
        where TPersistenceConfiguration :
        PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>
        where TConnectionStringBuilder : ConnectionStringBuilder, new()

    {
        private readonly Lazy<Type> _lazyDialectType;

        public ConfigurationMappingWithLazyDialect(ProviderTypeEnum providerType,
            Func<PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>> createFunc,
            string dialectTypeName)
            : base(providerType, createFunc)
        {
            _lazyDialectType = new Lazy<Type>(() => Type.GetType(dialectTypeName));
        }

        public override Type DialectType
        {
            get => _lazyDialectType.Value;
            protected set { }
        }
    }
}