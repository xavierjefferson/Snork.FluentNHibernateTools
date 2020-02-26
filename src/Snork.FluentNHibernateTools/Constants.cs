using System.Collections.Generic;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;
using NHibernate.Driver;
using Snork.FluentNHibernateTools.Mapping;

namespace Snork.FluentNHibernateTools
{
    internal class Constants
    {
        internal static readonly List<ConfigurationMappingBase> ProviderMappings = new List<ConfigurationMappingBase>
        {
            new ConfigurationMappingWithDialect<SQLAnywhereConfiguration, SybaseSQLAnywhereConnectionStringBuilder,
                SybaseASA9Dialect>(ProviderTypeEnum.SQLAnywhere9, () => SQLAnywhereConfiguration.SQLAnywhere9),

            new ConfigurationMappingWithDialect<SQLAnywhereConfiguration, SybaseSQLAnywhereConnectionStringBuilder,
                SybaseSQLAnywhere10Dialect>(ProviderTypeEnum.SQLAnywhere10,
                () => SQLAnywhereConfiguration.SQLAnywhere10),

            new ConfigurationMappingWithDialect<SQLAnywhereConfiguration, SybaseSQLAnywhereConnectionStringBuilder,
                SybaseSQLAnywhere11Dialect>(ProviderTypeEnum.SQLAnywhere11,
                () => SQLAnywhereConfiguration.SQLAnywhere11),

            new ConfigurationMappingWithDialect<SQLAnywhereConfiguration, SybaseSQLAnywhereConnectionStringBuilder,
                SybaseSQLAnywhere12Dialect>(ProviderTypeEnum.SQLAnywhere12,
                () => SQLAnywhereConfiguration.SQLAnywhere12),

            new ConfigurationMappingWithDialect<SQLiteConfiguration, ConnectionStringBuilder, SQLiteDialect>(
                ProviderTypeEnum.SQLite, () => SQLiteConfiguration.Standard),

            new ConfigurationMappingWithLazyDialect<JetDriverConfiguration, JetDriverConnectionStringBuilder>(
                ProviderTypeEnum.JetDriver, () => JetDriverConfiguration.Standard,
                "NHibernate.JetDriver.JetDialect, NHibernate.JetDriver"),

            new ConfigurationMappingWithDialect<MsSqlCeConfiguration, ConnectionStringBuilder, MsSqlCe40Dialect>(
                ProviderTypeEnum.MsSqlCe40, () => MsSqlCeConfiguration.MsSqlCe40),

            new ConfigurationMappingWithDialectAndDriver<OracleManagedDataClientConfiguration,
                OracleConnectionStringBuilder,
                Oracle10gDialect,
                OracleManagedDataClientDriver>(
                ProviderTypeEnum.OracleClient10Managed,
                () => OracleManagedDataClientConfiguration.Oracle10),

            new ConfigurationMappingWithDialectAndDriver<OracleManagedDataClientConfiguration,
                OracleConnectionStringBuilder,
                Oracle9iDialect,
                OracleManagedDataClientDriver>(
                ProviderTypeEnum.OracleClient9Managed,
                () => OracleManagedDataClientConfiguration.Oracle9),

            new ConfigurationMappingWithDialect<OracleClientConfiguration, OracleConnectionStringBuilder,
                Oracle10gDialect>(
                ProviderTypeEnum.OracleClient10, () => OracleClientConfiguration.Oracle10),

            new ConfigurationMappingWithDialect<OracleClientConfiguration, OracleConnectionStringBuilder,
                Oracle9iDialect>(
                ProviderTypeEnum.OracleClient9Managed, () => OracleClientConfiguration.Oracle9),

            new ConfigurationMappingWithDialect<PostgreSQLConfiguration, PostgreSQLConnectionStringBuilder,
                PostgreSQLDialect>(
                ProviderTypeEnum.PostgreSQLStandard, () => PostgreSQLConfiguration.Standard),

            new ConfigurationMappingWithDialect<PostgreSQLConfiguration, PostgreSQLConnectionStringBuilder,
                PostgreSQL81Dialect>(
                ProviderTypeEnum.PostgreSQL81, () => PostgreSQLConfiguration.PostgreSQL81),

            new ConfigurationMappingWithDialect<PostgreSQLConfiguration, PostgreSQLConnectionStringBuilder,
                PostgreSQL82Dialect>(
                ProviderTypeEnum.PostgreSQL82, () => PostgreSQLConfiguration.PostgreSQL82),

            new ConfigurationMappingWithDialect<FirebirdConfiguration, ConnectionStringBuilder, FirebirdDialect>(
                ProviderTypeEnum.Firebird, () => new FirebirdConfiguration()),

            new ConfigurationMappingWithDialect<DB2400Configuration, DB2400ConnectionStringBuilder, DB2400Dialect>(
                ProviderTypeEnum.DB2400, () => DB2400Configuration.Standard),

            new ConfigurationMappingWithDialect<DB2Configuration, DB2ConnectionStringBuilder, InformixDialect1000>(
                ProviderTypeEnum.DB2Informix1150, () => DB2Configuration.Informix1150),

            new ConfigurationMappingWithDialect<DB2Configuration, DB2ConnectionStringBuilder, DB2Dialect>(
                ProviderTypeEnum.DB2Standard, () => DB2Configuration.Standard),

            new ConfigurationMappingWithDialect<MySQLConfiguration, MySQLConnectionStringBuilder, MySQLDialect>(
                ProviderTypeEnum.MySQL, () => MySQLConfiguration.Standard),

            new ConfigurationMappingWithDialect<MsSqlConfiguration, MsSqlConnectionStringBuilder, MsSql2008Dialect>(
                ProviderTypeEnum.MsSql2008, () => MsSqlConfiguration.MsSql2008),

            new ConfigurationMappingWithDialect<MsSqlConfiguration, MsSqlConnectionStringBuilder, MsSql2012Dialect>(
                ProviderTypeEnum.MsSql2012, () => MsSqlConfiguration.MsSql2012),

            new ConfigurationMappingWithDialect<MsSqlConfiguration, MsSqlConnectionStringBuilder, MsSql2005Dialect>(
                ProviderTypeEnum.MsSql2005, () => MsSqlConfiguration.MsSql2005),

            new ConfigurationMappingWithDialect<MsSqlConfiguration, MsSqlConnectionStringBuilder, MsSql2000Dialect>(
                ProviderTypeEnum.MsSql2000, () => MsSqlConfiguration.MsSql2000),

            new ConfigurationMappingWithDialect<IngresConfiguration, IngresConnectionStringBuilder, IngresDialect>(
                ProviderTypeEnum.Ingres, () => IngresConfiguration.Standard)
        };
    }
}