using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Impl;

namespace Snork.FluentNHibernateTools
{
    public static class ProviderMappingHelper
    {
        internal static readonly List<NameProviderMatcher> ProviderMappings;

        static ProviderMappingHelper()
        {
            var forJetDriver = PersistenceConfigurationHelper.GetDerivedConnectionInfo(JetDriverConfiguration.Standard);

            ProviderMappings =
                new List<NameProviderMatcher>
                {
                    new NameProviderMatcher(ProviderTypeEnum.JetDriver, forJetDriver.Dialect, forJetDriver.Driver),
                    new TypedProviderMatcher<MySQLDialect>(ProviderTypeEnum.MySQL),
                    new TypedProviderMatcher<PostgreSQL81Dialect>(ProviderTypeEnum.PostgreSQL81),
                    new TypedProviderMatcher<PostgreSQL82Dialect>(ProviderTypeEnum.PostgreSQL82),
                    new TypedProviderMatcher<PostgreSQLDialect>(ProviderTypeEnum.PostgreSQLStandard),
                    new TypedProviderMatcher<MsSql2008Dialect>(ProviderTypeEnum.MsSql2008),
                    new TypedProviderMatcher<MsSql2012Dialect>(ProviderTypeEnum.MsSql2012),
                    new TypedProviderMatcher<MsSqlCeDialect>(ProviderTypeEnum.MsSqlCeStandard),
                    new TypedProviderMatcher<MsSqlCe40Dialect>(ProviderTypeEnum.MsSqlCe40),
                    new TypedProviderMatcher<MsSql2000Dialect>(ProviderTypeEnum.MsSql2000),
                    new TypedProviderMatcher<MsSql2005Dialect>(ProviderTypeEnum.MsSql2005),
                    new TypedProviderMatcher<MsSql2008Dialect>(ProviderTypeEnum.MsSql2008),
                    new TypedProviderMatcher<MsSql2012Dialect>(ProviderTypeEnum.MsSql2012),
                    new TypedProviderMatcher<SQLiteDialect>(ProviderTypeEnum.SQLite),

                    new TypedProviderMatcher<FirebirdDialect>(ProviderTypeEnum.Firebird),
                    //oracle unmanaged drivers
                    new TypedProviderMatcher<Oracle9iDialect>(ProviderTypeEnum.OracleClient9,
                        typeof(OracleDataClientDriver)),
                    new TypedProviderMatcher<Oracle10gDialect>(ProviderTypeEnum.OracleClient10,
                        typeof(OracleDataClientDriver)),

                    //oracle managed drivers
                    new TypedProviderMatcher<Oracle9iDialect>(ProviderTypeEnum.OracleClient9Managed,
                        typeof(OracleManagedDataClientDriver)),
                    new TypedProviderMatcher<Oracle10gDialect>(ProviderTypeEnum.OracleClient10Managed,
                        typeof(OracleManagedDataClientDriver)),
                    new TypedProviderMatcher<SybaseASA9Dialect>(ProviderTypeEnum.SQLAnywhere9),
                    new TypedProviderMatcher<SybaseSQLAnywhere10Dialect>(ProviderTypeEnum.SQLAnywhere10),
                    new TypedProviderMatcher<SybaseSQLAnywhere11Dialect>(ProviderTypeEnum.SQLAnywhere11),
                    new TypedProviderMatcher<SybaseSQLAnywhere12Dialect>(ProviderTypeEnum.SQLAnywhere12),
                    new TypedProviderMatcher<SapSQLAnywhere17Dialect>(ProviderTypeEnum.SQLAnywhere17),
                    new TypedProviderMatcher<InformixDialect1000>(ProviderTypeEnum.DB2Informix1150),
                    new TypedProviderMatcher<DB2Dialect>(ProviderTypeEnum.DB2Standard)
                };
        }


        public static ProviderTypeEnum DeriveProviderType(string dialectAssemblyQualifiedName,
            string driverAssemblyQualifiedName)
        {
            var mapping = ProviderMappings.FirstOrDefault(i =>
                i.Matches(dialectAssemblyQualifiedName, driverAssemblyQualifiedName));
            return mapping?.ProviderType ?? ProviderTypeEnum.None;
        }

        public static ProviderTypeEnum DeriveProviderType(this ISessionFactory sessionFactory)
        {
            var sessionFactoryImpl = sessionFactory as SessionFactoryImpl;
            if (sessionFactoryImpl?.Dialect == null)
                return ProviderTypeEnum.None;


            var mapping = DeriveProviderType(sessionFactoryImpl.Dialect.GetType().AssemblyQualifiedName,
                sessionFactoryImpl.ConnectionProvider?.Driver?.GetType().AssemblyQualifiedName);
            return mapping;
        }


        internal class NameProviderMatcher
        {
            public NameProviderMatcher(ProviderTypeEnum providerType, string dialectTypeName, string driverTypeName)
            {
                ProviderType = providerType;
                DialectTypeName = dialectTypeName;
                DriverTypeName = driverTypeName;
            }

            public ProviderTypeEnum ProviderType { get; }

            public string DriverTypeName { get; }

            public string DialectTypeName { get; }

            public bool Matches(string dialectAssemblyQualifiedName, string driverTypeAssemblyQualifiedName)
            {
                if (string.IsNullOrWhiteSpace(DriverTypeName)) return DialectTypeName == dialectAssemblyQualifiedName;

                return DialectTypeName == dialectAssemblyQualifiedName &&
                       DriverTypeName == driverTypeAssemblyQualifiedName;
            }
        }

        internal class TypedProviderMatcher<T> : NameProviderMatcher
        {
            public TypedProviderMatcher(ProviderTypeEnum providerType, Type driverType = null) : base(providerType,
                typeof(T).AssemblyQualifiedName, driverType == null ? null : driverType.AssemblyQualifiedName)
            {
            }
        }
    }
}