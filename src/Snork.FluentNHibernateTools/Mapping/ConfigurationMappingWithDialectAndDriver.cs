using System;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace Snork.FluentNHibernateTools.Mapping
{
    internal class
        ConfigurationMappingWithDialectAndDriver<TPersistenceConfiguration, TConnectionStringBuilder, TDialect,
            TDriver> :
            ConfigurationMappingWithDialect<TPersistenceConfiguration, TConnectionStringBuilder, TDialect>
        where TPersistenceConfiguration :
        PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>
        where TConnectionStringBuilder : ConnectionStringBuilder, new()
        where TDialect : Dialect
        where TDriver : ReflectionBasedDriver
    {
        public ConfigurationMappingWithDialectAndDriver(ProviderTypeEnum providerType,
            Func<PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>> createFunc)
            : base(providerType, createFunc)
        {
            DriverType = typeof(TDriver);
        }
    }
}