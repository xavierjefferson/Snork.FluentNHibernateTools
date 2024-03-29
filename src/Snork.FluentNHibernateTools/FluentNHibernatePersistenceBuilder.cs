﻿using System;
using System.Configuration;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate.Driver;

namespace Snork.FluentNHibernateTools
{
    public sealed class FluentNHibernatePersistenceBuilder
    {
        private static T ConfigureProvider<T, U>(Func<PersistenceConfiguration<T, U>> createFunc,
            string connectionString, string defaultSchema) where T : PersistenceConfiguration<T, U>
            where U : ConnectionStringBuilder, new()
        {
            var provider = createFunc().ConnectionString(connectionString);

            if (!string.IsNullOrWhiteSpace(defaultSchema)) provider.DefaultSchema(defaultSchema);
            return provider;
        }

        /// <summary>
        ///     Factory method.  Return simple configuration info based on the given provider type, connection string, and default
        ///     schema
        /// </summary>
        /// <param name="nameOrConnectionString">Connection string or its name</param>
        /// <param name="providerType">Provider type from enumeration</param>
        /// <param name="options"></param>
        /// <param name="configuration">
        ///     An IConfiguration instance to read from.  Otherwise connection string will be read with
        ///     System.Configuration.ConfigurationManager
        /// </param>
        public static ConfigurationInfo Build(ProviderTypeEnum providerType, string nameOrConnectionString,
            FluentNHibernatePersistenceBuilderOptions options = null, IConfiguration configuration = null)
        {
            options = options ?? new FluentNHibernatePersistenceBuilderOptions();
            var configurer = GetPersistenceConfigurer(providerType, nameOrConnectionString, options, configuration);
            return new ConfigurationInfo(configurer, options, providerType);
        }


        private static string GetConnectionString(string nameOrConnectionString,
            ConnectionStringImpl connectionStringImpl)
        {
            if (IsConnectionString(nameOrConnectionString)) return nameOrConnectionString;

            if (IsConnectionStringInConfiguration(nameOrConnectionString, connectionStringImpl))
                return connectionStringImpl.GetConnectionString(nameOrConnectionString);

            throw new ArgumentException(
                $"Could not find connection string with name '{nameOrConnectionString}' in application config");
        }


        private static bool IsConnectionString(string nameOrConnectionString)
        {
            return nameOrConnectionString.Contains(";") || nameOrConnectionString.Contains("=");
        }

        private static bool IsConnectionStringInConfiguration(string connectionStringName, ConnectionStringImpl impl)
        {
            return impl.ConnectionStringExists(connectionStringName);
        }


        /// <summary>
        ///     Return an NHibernate persistence configurerTells the bootstrapper to use a FluentNHibernate provider as a job
        ///     storage,
        ///     that can be accessed using the given connection string or
        ///     its name.
        /// </summary>
        /// <param name="providerType">Provider type from enumeration</param>
        /// <param name="nameOrConnectionString">Connection string or its name</param>
        /// <param name="options"></param>
        /// <param name="configuration">An IConfiguration instance from which the connection string should be read</param>
        public static IPersistenceConfigurer GetPersistenceConfigurer(ProviderTypeEnum providerType,
            string nameOrConnectionString, FluentNHibernatePersistenceBuilderOptions options = null,
            IConfiguration configuration = null)
        {
            var connectionStringImpl = configuration == null
                ? new SEConfigurationImpl()
                : (ConnectionStringImpl) new MEConfigurationImpl(configuration);
            var connectionString = GetConnectionString(nameOrConnectionString, connectionStringImpl);

            options = options ?? new FluentNHibernatePersistenceBuilderOptions();


            IPersistenceConfigurer configurer;

            switch (providerType)
            {
                case ProviderTypeEnum.SQLAnywhere9:
                    configurer = ConfigureProvider(() => SQLAnywhereConfiguration.SQLAnywhere9, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.SQLAnywhere10:
                    configurer = ConfigureProvider(() => SQLAnywhereConfiguration.SQLAnywhere10, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.SQLAnywhere11:
                    configurer = ConfigureProvider(() => SQLAnywhereConfiguration.SQLAnywhere11, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.SQLAnywhere12:
                    configurer = ConfigureProvider(() => SQLAnywhereConfiguration.SQLAnywhere12, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.SQLAnywhere17:
                    configurer = ConfigureProvider(() => SQLAnywhereConfiguration.SQLAnywhere17, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.SQLite:
                    configurer = ConfigureProvider(() => SQLiteConfiguration.Standard, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.JetDriver:
                    configurer = ConfigureProvider(() => JetDriverConfiguration.Standard, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.MsSqlCeStandard:
                    configurer = ConfigureProvider(() => MsSqlCeConfiguration.Standard, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.MsSqlCe40:
                    configurer = ConfigureProvider(() => MsSqlCeConfiguration.MsSqlCe40, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.OracleClient10Managed:
                    configurer = ConfigureProvider(() => OracleManagedDataClientConfiguration.Oracle10,
                        connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.OracleClient9Managed:
                    configurer = ConfigureProvider(() => OracleManagedDataClientConfiguration.Oracle9, connectionString,
                        options.DefaultSchema);
                    break;

                case ProviderTypeEnum.OracleClient10:
                    configurer = ConfigureProvider(() => OracleDataClientConfiguration.Oracle10, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.OracleClient9:
                    configurer = ConfigureProvider(() => OracleDataClientConfiguration.Oracle9, connectionString,
                        options.DefaultSchema);
                    break;
                case ProviderTypeEnum.PostgreSQLStandard:
                    configurer = ConfigureProvider(() => PostgreSQLConfiguration.Standard, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.PostgreSQL81:
                    configurer = ConfigureProvider(() => PostgreSQLConfiguration.PostgreSQL81, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.PostgreSQL82:
                    configurer = ConfigureProvider(() => PostgreSQLConfiguration.PostgreSQL82, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.Firebird:
                    configurer = ConfigureProvider(() => new FirebirdConfiguration(), connectionString,
                        options.DefaultSchema);

                    break;

                case ProviderTypeEnum.DB2Informix1150:
                    configurer = ConfigureProvider(() => DB2Configuration.Informix1150, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.DB2Standard:
                    configurer = ConfigureProvider(() => DB2Configuration.Standard, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.MySQL:
                    configurer = ConfigureProvider(() => MySQLConfiguration.Standard, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.MsSql2008:
                    configurer = ConfigureProvider(() => MsSqlConfiguration.MsSql2008, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.MsSql2012:
                    configurer = ConfigureProvider(() => MsSqlConfiguration.MsSql2012, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.MsSql2005:
                    configurer = ConfigureProvider(() => MsSqlConfiguration.MsSql2005, connectionString,
                        options.DefaultSchema);

                    break;
                case ProviderTypeEnum.MsSql2000:
                    configurer = ConfigureProvider(() => MsSqlConfiguration.MsSql2000, connectionString,
                        options.DefaultSchema);

                    break;
                default:
                    throw new ArgumentException(nameof(providerType));
            }

            return configurer;
        }

        /// <summary>
        ///     Uses IConfiguration
        /// </summary>
        public class MEConfigurationImpl : ConnectionStringImpl
        {
            private readonly IConfiguration _config;

            public MEConfigurationImpl(IConfiguration config)
            {
                _config = config;
            }

            public override bool ConnectionStringExists(string name)
            {
                return _config.GetConnectionString(name) != null;
            }

            public override string GetConnectionString(string name)
            {
                return _config.GetConnectionString(name);
            }
        }

        /// <summary>
        ///     Uses System.ConfigurationManager
        /// </summary>
        public class SEConfigurationImpl : ConnectionStringImpl
        {
            public override bool ConnectionStringExists(string name)
            {
                return ConfigurationManager.ConnectionStrings[name] != null;
            }

            public override string GetConnectionString(string name)
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
        }

        public abstract class ConnectionStringImpl
        {
            internal ConnectionStringImpl()
            {
            }

            public abstract bool ConnectionStringExists(string name);
            public abstract string GetConnectionString(string name);
        }
    }
}