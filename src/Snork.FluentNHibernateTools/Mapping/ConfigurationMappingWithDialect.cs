using System;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;

namespace Snork.FluentNHibernateTools.Mapping
{
    internal class
        ConfigurationMappingWithDialect<TPersistenceConfiguration, TConnectionStringBuilder, TDialect> :
            RootConfigurationMapping<TPersistenceConfiguration, TConnectionStringBuilder>
        where TPersistenceConfiguration :
        PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>
        where TConnectionStringBuilder : ConnectionStringBuilder, new()
        where TDialect : Dialect
    {
        public ConfigurationMappingWithDialect(ProviderTypeEnum providerType,
            Func<PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>> createFunc)
            : base(providerType, createFunc)
        {
            DialectType = typeof(TDialect);
        }
    }
}