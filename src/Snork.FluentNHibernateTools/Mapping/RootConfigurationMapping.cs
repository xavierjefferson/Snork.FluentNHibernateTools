using System;
using FluentNHibernate.Cfg.Db;

namespace Snork.FluentNHibernateTools.Mapping
{
    internal class
        RootConfigurationMapping<TPersistenceConfiguration, TConnectionStringBuilder> : ConfigurationMappingBase
        where TPersistenceConfiguration :
        PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>
        where TConnectionStringBuilder : ConnectionStringBuilder, new()
    {
        private readonly Func<PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>>
            _config;

        public RootConfigurationMapping(ProviderTypeEnum providerType,
            Func<PersistenceConfiguration<TPersistenceConfiguration, TConnectionStringBuilder>> createFunc) : base(
            typeof(TPersistenceConfiguration), providerType)
        {
            _config = createFunc;
        }

        public override IPersistenceConfigurer Create(string connectionString, string defaultSchema)
        {
            var persistenceConfiguration = _config().ConnectionString(connectionString);
            return !string.IsNullOrWhiteSpace(defaultSchema)
                ? persistenceConfiguration.DefaultSchema(defaultSchema)
                : persistenceConfiguration;
        }
    }
}