using System;
using FluentNHibernate.Cfg.Db;

namespace Snork.FluentNHibernateTools
{
    public static class ProviderTypeHelper
    {
        public static ProviderTypeEnum InferProviderType(IPersistenceConfigurer config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            var info = PersistenceConfigurationHelper.GetDerivedConnectionInfo(config);
            var providerType = ProviderMappingHelper.DeriveProviderType(info.Dialect, info.Driver);
            if (providerType != ProviderTypeEnum.None) return providerType;

            throw new ArgumentOutOfRangeException($"Unknown configurer {config.GetType().FullName}");
        }
    }
}