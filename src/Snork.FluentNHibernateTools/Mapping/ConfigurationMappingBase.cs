using System;
using FluentNHibernate.Cfg.Db;

namespace Snork.FluentNHibernateTools.Mapping
{
    public abstract class ConfigurationMappingBase
    {
        public ConfigurationMappingBase(Type configurerType, ProviderTypeEnum providerType)
        {
            ConfigurerType = configurerType;
            ProviderType = providerType;
        }

        public Type ConfigurerType { get; }
        public ProviderTypeEnum ProviderType { get; }
        public virtual Type DialectType { get; protected set; }
        public virtual Type DriverType { get; protected set; }
        public abstract IPersistenceConfigurer Create(string connectionString, string defaultSchema);

        public bool DialectAndDriverMatches(Type dialectType, Type driverType)
        {
            return (DialectType == dialectType || dialectType.IsSubclassOf(DialectType)) &
                   (DriverType == null || DriverType == driverType);
        }
    }
}