using FluentNHibernate.Cfg.Db;

namespace Snork.FluentNHibernateTools
{
    public static class ProviderTypeHelper
    {
        public static ProviderTypeEnum InferProviderType(IPersistenceConfigurer config)
        {
            if (config is SQLAnywhereConfiguration)
            {
                return ProviderTypeEnum.SQLAnywhere9;
            }

            if (config is MsSqlConfiguration)
            {
                return ProviderTypeEnum.MsSql2000;
            }

            if (config is PostgreSQLConfiguration)
            {
                return ProviderTypeEnum.PostgreSQLStandard;
            }

            if (config is JetDriverConfiguration)
            {
                return ProviderTypeEnum.JetDriver;
            }

            if (config is SQLiteConfiguration)
            {
                return ProviderTypeEnum.SQLite;
            }

            if (config is MsSqlCeConfiguration)
            {
                return ProviderTypeEnum.MsSqlCe40;
            }

            if (config is DB2Configuration)
            {
                return ProviderTypeEnum.DB2Standard;
            }

            if (config is OracleClientConfiguration)
            {
                return ProviderTypeEnum.OracleClient9;
            }

            if (config is FirebirdConfiguration)
            {
                return ProviderTypeEnum.Firebird;
            }

            return ProviderTypeEnum.None;
        }
    }
}