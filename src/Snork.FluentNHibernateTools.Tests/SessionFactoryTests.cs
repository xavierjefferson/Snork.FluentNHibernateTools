using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Xunit;

namespace Snork.FluentNHibernateTools.Tests
{
    public class SessionFactoryTests
    {
        private const string SqlConnectionString = "data source=.;Trusted_Connection=True;";

       

        [Fact]
        public void TestSessionFactoryDeriveSql2000()
        {
            var a = SessionFactoryBuilder.GetFromAssemblyOf<SessionFactoryTests>(ProviderTypeEnum.MsSql2000, SqlConnectionString,
                new FluentNHibernatePersistenceBuilderOptions {UpdateSchema = false});
            Assert.Equal(ProviderTypeEnum.MsSql2000, a.SessionFactory.DeriveProviderType());
        }

        [Fact]
        public void TestSessionFactoryDeriveSql2005()
        {
            var a = SessionFactoryBuilder.GetFromAssemblyOf<SessionFactoryTests>(ProviderTypeEnum.MsSql2005, SqlConnectionString,
                new FluentNHibernatePersistenceBuilderOptions {UpdateSchema = false});
            Assert.Equal(ProviderTypeEnum.MsSql2005, a.SessionFactory.DeriveProviderType());
        }

        [Fact]
        public void TestSessionFactoryDeriveSqlite()
        {
            var a = SessionFactoryBuilder.GetFromAssemblyOf<SessionFactoryTests>(ProviderTypeEnum.SQLite, "data source=aaa",
                new FluentNHibernatePersistenceBuilderOptions {UpdateSchema = false});
            Assert.Equal(ProviderTypeEnum.SQLite, a.SessionFactory.DeriveProviderType());
        }
    }
}