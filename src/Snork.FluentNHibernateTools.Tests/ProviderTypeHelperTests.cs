using System;
using System.ComponentModel;
using FluentNHibernate.Cfg.Db;
using Xunit;

namespace Snork.FluentNHibernateTools.Tests
{
    public class ProviderTypeHelperTests
    {
        private void TestConfig(Antlr.Runtime.Misc.Func<IPersistenceConfigurer> func, ProviderTypeEnum expected)
        {
            if (!Enum.IsDefined(typeof(ProviderTypeEnum), expected))
                throw new InvalidEnumArgumentException(nameof(expected), (int) expected, typeof(ProviderTypeEnum));
            Assert.Equal(expected, ProviderTypeHelper.InferProviderType(func()));
        }

        [Fact]
        public void TestInferDB2Informix1150()
        {
            TestConfig(() => DB2Configuration.Informix1150, ProviderTypeEnum.DB2Informix1150);
        }

        [Fact]
        public void TestInferDB2Standard()
        {
            TestConfig(() => DB2Configuration.Standard, ProviderTypeEnum.DB2Standard);
        }

        [Fact]
        public void TestInferFirebird()
        {
            TestConfig(() => new FirebirdConfiguration(), ProviderTypeEnum.Firebird);
        }

        [Fact]
        public void TestInferJet()
        {
            TestConfig(() => JetDriverConfiguration.Standard, ProviderTypeEnum.JetDriver);
        }

        [Fact]
        public void TestInferMsSql2000()
        {
            TestConfig(() => MsSqlConfiguration.MsSql2000, ProviderTypeEnum.MsSql2000);
        }

        [Fact]
        public void TestInferMsSql2005()
        {
            TestConfig(() => MsSqlConfiguration.MsSql2005, ProviderTypeEnum.MsSql2005);
        }

        [Fact]
        public void TestInferMsSql2008()
        {
            TestConfig(() => MsSqlConfiguration.MsSql2008, ProviderTypeEnum.MsSql2008);
        }

        [Fact]
        public void TestInferMsSql2012()
        {
            TestConfig(() => MsSqlConfiguration.MsSql2012, ProviderTypeEnum.MsSql2012);
        }

        [Fact]
        public void TestInferMySql()
        {
            TestConfig(() => MySQLConfiguration.Standard, ProviderTypeEnum.MySQL);
        }

        [Fact]
        public void TestInferOracleDataClient10()
        {
            TestConfig(() => OracleDataClientConfiguration.Oracle10, ProviderTypeEnum.OracleClient10);
        }

        [Fact]
        public void TestInferOracleDataClient9()
        {
            TestConfig(() => OracleDataClientConfiguration.Oracle9, ProviderTypeEnum.OracleClient9);
        }

        [Fact]
        public void TestInferOracleManagedDataClient10()
        {
            TestConfig(() => OracleManagedDataClientConfiguration.Oracle10, ProviderTypeEnum.OracleClient10Managed);
        }

        [Fact]
        public void TestInferOracleManagedDataClient9()
        {
            TestConfig(() => OracleManagedDataClientConfiguration.Oracle9, ProviderTypeEnum.OracleClient9Managed);
        }

        [Fact]
        public void TestInferPostgres()
        {
            TestConfig(() => PostgreSQLConfiguration.Standard, ProviderTypeEnum.PostgreSQLStandard);
        }

        [Fact]
        public void TestInferPostgres81()
        {
            TestConfig(() => PostgreSQLConfiguration.PostgreSQL81, ProviderTypeEnum.PostgreSQL81);
        }

        [Fact]
        public void TestInferPostgres82()
        {
            TestConfig(() => PostgreSQLConfiguration.PostgreSQL82, ProviderTypeEnum.PostgreSQL82);
        }

        [Fact]
        public void TestInferSqlAnywhere10()
        {
            TestConfig(() => SQLAnywhereConfiguration.SQLAnywhere10, ProviderTypeEnum.SQLAnywhere10);
        }

        [Fact]
        public void TestInferSqlAnywhere11()
        {
            TestConfig(() => SQLAnywhereConfiguration.SQLAnywhere11, ProviderTypeEnum.SQLAnywhere11);
        }

        [Fact]
        public void TestInferSqlAnywhere12()
        {
            TestConfig(() => SQLAnywhereConfiguration.SQLAnywhere12, ProviderTypeEnum.SQLAnywhere12);
        }

        [Fact]
        public void TestInferSqlAnywhere17()
        {
            TestConfig(() => SQLAnywhereConfiguration.SQLAnywhere17, ProviderTypeEnum.SQLAnywhere17);
        }

        [Fact]
        public void TestInferSqlAnywhere9()
        {
            TestConfig(() => SQLAnywhereConfiguration.SQLAnywhere9, ProviderTypeEnum.SQLAnywhere9);
        }

        [Fact]
        public void TestInferSqlCe40()
        {
            TestConfig(() => MsSqlCeConfiguration.MsSqlCe40, ProviderTypeEnum.MsSqlCe40);
        }

        [Fact]
        public void TestInferSqlCeStandard()
        {
            TestConfig(() => MsSqlCeConfiguration.Standard, ProviderTypeEnum.MsSqlCeStandard);
        }

        [Fact]
        public void TestInferSqlite()
        {
            TestConfig(() => SQLiteConfiguration.Standard, ProviderTypeEnum.SQLite);
        }

        [Fact]
        public void TestInferThrowsExceptionOnNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => { TestConfig(() => null, ProviderTypeEnum.None); });
        }

        [Fact]
        public void TestInferUnknownThrowsArgumentRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                TestConfig(() => IfxDRDAConfiguration.Informix, ProviderTypeEnum.None);
            });
        }
    }
}