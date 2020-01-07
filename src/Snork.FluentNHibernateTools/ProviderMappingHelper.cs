using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Impl;

namespace Snork.FluentNHibernateTools
{
    public static class ProviderMappingHelper
    {
        internal static readonly List<ProviderMatcher> ProviderMappings =
            new List<ProviderMatcher>
            {
                new NameProviderMatcher(ProviderTypeEnum.JetDriver,
                    "NHibernate.JetDriver.JetDialect, NHibernate.JetDriver"),

                new TypedProviderMatcher<MySQLDialect>(ProviderTypeEnum.MySQL),
                new TypedProviderMatcher<PostgreSQL81Dialect>(ProviderTypeEnum.PostgreSQL81),
                new TypedProviderMatcher<PostgreSQL82Dialect>(ProviderTypeEnum.PostgreSQL82),
                new TypedProviderMatcher<PostgreSQLDialect>(ProviderTypeEnum.PostgreSQLStandard),
                new TypedProviderMatcher<MsSql2008Dialect>(ProviderTypeEnum.MsSql2008),
                new TypedProviderMatcher<MsSql2012Dialect>(ProviderTypeEnum.MsSql2012),
                new TypedProviderMatcher<MsSqlCe40Dialect>(ProviderTypeEnum.MsSqlCe40),
                new TypedProviderMatcher<MsSql2000Dialect>(ProviderTypeEnum.MsSql2000),
                new TypedProviderMatcher<MsSql2005Dialect>(ProviderTypeEnum.MsSql2005),
                new TypedProviderMatcher<MsSql2008Dialect>(ProviderTypeEnum.MsSql2008),
                new TypedProviderMatcher<MsSql2012Dialect>(ProviderTypeEnum.MsSql2012),
                new TypedProviderMatcher<SQLiteDialect>(ProviderTypeEnum.SQLite),
                new TypedProviderMatcher<Oracle10gDialect>(ProviderTypeEnum.OracleClient10),
                new TypedProviderMatcher<FirebirdDialect>(ProviderTypeEnum.Firebird),
                new TypedProviderMatcher<Oracle9iDialect>(ProviderTypeEnum.OracleClient9),
                new TypedProviderMatcher<Oracle10gDialect>(ProviderTypeEnum.OracleClient10Managed,
                    typeof(OracleManagedDataClientDriver)),
                new TypedProviderMatcher<Oracle9iDialect>(ProviderTypeEnum.OracleClient9Managed,
                    typeof(OracleManagedDataClientDriver)),
                new TypedProviderMatcher<SybaseSQLAnywhere10Dialect>(ProviderTypeEnum.SQLAnywhere10),
                new TypedProviderMatcher<SybaseSQLAnywhere11Dialect>(ProviderTypeEnum.SQLAnywhere11),
                new TypedProviderMatcher<SybaseSQLAnywhere12Dialect>(ProviderTypeEnum.SQLAnywhere12),
                new TypedProviderMatcher<InformixDialect1000>(ProviderTypeEnum.DB2Informix1150),
                new TypedProviderMatcher<DB2Dialect>(ProviderTypeEnum.DB2Standard)
            };

        public static ProviderTypeEnum DeriveProviderType(this ISessionFactory sessionFactory)
        {
            var sessionFactoryImpl = sessionFactory as SessionFactoryImpl;
            if (sessionFactoryImpl?.Dialect == null)
                return ProviderTypeEnum.None;


            var mapping = ProviderMappings.FirstOrDefault(i => i.Matches(sessionFactoryImpl.Dialect.GetType(),
                sessionFactoryImpl.ConnectionProvider?.Driver?.GetType()));
            return mapping?.ProviderType ?? ProviderTypeEnum.None;
        }

        public abstract class ProviderMatcher
        {
            public ProviderTypeEnum ProviderType { get; protected set; }
            public abstract bool Matches(Type dialectType, Type driverType);
        }

        internal class NameProviderMatcher : ProviderMatcher
        {
            public NameProviderMatcher(ProviderTypeEnum providerType, string dialectName)
            {
                ProviderType = providerType;
                DialectName = dialectName;
            }

            public string DialectName { get; set; }

            public override bool Matches(Type dialectType, Type driverType)
            {
                return dialectType.FullName.IndexOf(DialectName, StringComparison.InvariantCulture) > 0;
            }
        }

        internal class TypedProviderMatcher<T> : ProviderMatcher
        {
            public TypedProviderMatcher(ProviderTypeEnum providerType, Type driverType = null)
            {
                ProviderType = providerType;
                MatchingTypes = typeof(T);
                DriverType = driverType;
            }


            public Type DriverType { get; }

            public Type MatchingTypes { get; }

            public override bool Matches(Type dialectType, Type driverType)
            {
                return (MatchingTypes == dialectType || dialectType.IsSubclassOf(MatchingTypes)) & (DriverType == null || DriverType == driverType);
            }
        }
    }
}