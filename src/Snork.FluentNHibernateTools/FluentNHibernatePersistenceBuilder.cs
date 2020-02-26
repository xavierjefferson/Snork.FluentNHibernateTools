using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FluentNHibernate.Cfg.Db;
using Snork.FluentNHibernateTools.Mapping;

namespace Snork.FluentNHibernateTools
{
    public sealed class FluentNHibernatePersistenceBuilder
    {
        private static readonly Dictionary<ProviderTypeEnum, ConfigurationMappingBase> MappingsDictionary;

        static FluentNHibernatePersistenceBuilder()
        {
            MappingsDictionary = Constants.ProviderMappings.ToDictionary(i => i.ProviderType, i => i);
        }

        /// <summary>
        ///     Factory method.  Return simple configuration info based on the given provider type, connection string, and default
        ///     schema
        /// </summary>
        /// <param name="nameOrConnectionString">Connection string or its name</param>
        /// <param name="providerType">Provider type from enumeration</param>
        /// <param name="options"></param>
        public static ConfigurationInfo Build(ProviderTypeEnum providerType, string nameOrConnectionString,
            FluentNHibernatePersistenceBuilderOptions options = null)
        {
            options = options ?? new FluentNHibernatePersistenceBuilderOptions();
            var configurer = GetPersistenceConfigurer(providerType, nameOrConnectionString, options);
            return new ConfigurationInfo(configurer, options, providerType);
        }

        private static string GetConnectionString(string nameOrConnectionString)
        {
            if (IsConnectionString(nameOrConnectionString)) return nameOrConnectionString;

            if (IsConnectionStringInConfiguration(nameOrConnectionString))
                return ConfigurationManager.ConnectionStrings[nameOrConnectionString].ConnectionString;

            throw new ArgumentException(
                $"Could not find connection string with name '{nameOrConnectionString}' in application config file");
        }


        private static bool IsConnectionString(string nameOrConnectionString)
        {
            return nameOrConnectionString.Contains(";");
        }

        private static bool IsConnectionStringInConfiguration(string connectionStringName)
        {
            var connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];

            return connectionStringSetting != null;
        }

        /// <summary>
        ///     Return an NHibernate persistence configurer
        ///     that can be accessed using the given connection string or
        ///     its name.
        /// </summary>
        /// <param name="nameOrConnectionString">Connection string or its name</param>
        /// <param name="providerType">Provider type from enumeration</param>
        /// <param name="options"></param>
        public static IPersistenceConfigurer GetPersistenceConfigurer(ProviderTypeEnum providerType,
            string nameOrConnectionString, FluentNHibernatePersistenceBuilderOptions options = null)
        {
            options = options ?? new FluentNHibernatePersistenceBuilderOptions();
            var connectionString = GetConnectionString(nameOrConnectionString);

            if (MappingsDictionary.ContainsKey(providerType))
                return MappingsDictionary[providerType].Create(connectionString, options.DefaultSchema);
            throw new ArgumentException(nameof(providerType));
        }
    }
}