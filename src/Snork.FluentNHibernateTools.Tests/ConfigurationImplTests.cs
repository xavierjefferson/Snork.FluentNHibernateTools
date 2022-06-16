using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Xunit;

namespace Snork.FluentNHibernateTools.Tests
{
    [Collection(Constants.OneTestAtATimeFixtureCollectionName)]
    public class ConfigurationImplTests
    {
        public ConfigurationImplTests(OneTestAtATimeFixture fixture)
        {
        }

        [Fact]
        public void TestReadConfig()
        {
            var memoryConfigurationSource = new MemoryConfigurationSource();
            const string value = "test2";
            const string keyName = "test";
            memoryConfigurationSource.InitialData = new[]
                {new KeyValuePair<string, string>($"ConnectionStrings:{keyName}", value)};
            var config = new ConfigurationBuilder().Add(memoryConfigurationSource).Build();
            var impl = new FluentNHibernatePersistenceBuilder.MEConfigurationImpl(config);
            Assert.True(impl.ConnectionStringExists(keyName));
            Assert.Equal(value, impl.GetConnectionString(keyName));

            Assert.False(impl.ConnectionStringExists("test123"));
        }


        [Fact]
        public void TestReadSEConfigWhenMissing()
        {
            const string value = "test2";
            const string keyName = "test";
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var file = new FileInfo(config.FilePath);
            if (file.Exists) file.Delete();

            var impl = new FluentNHibernatePersistenceBuilder.SEConfigurationImpl();
            Assert.False(impl.ConnectionStringExists(keyName));
            Assert.Throws<NullReferenceException>(() => { Assert.Equal(value, impl.GetConnectionString(keyName)); });
        }

        [Fact]
        public void TestReadSEConfigWhenPresent()
        {
            const string value = "test2";
            const string keyName = "test";
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var file = new FileInfo(config.FilePath);
            if (file.Exists) file.Delete();
            var xml =
                $"<configuration>\r\n<connectionStrings>\r\n<add name=\"{keyName}\" connectionString=\"{value}\" />\r\n</connectionStrings>\r\n</configuration>";
            var xd = new XmlDocument();
            xd.LoadXml(xml);
            xd.Save(file.FullName);
            var impl = new FluentNHibernatePersistenceBuilder.SEConfigurationImpl();
            Assert.True(impl.ConnectionStringExists(keyName));
            Assert.Equal(value, impl.GetConnectionString(keyName));
            Assert.False(impl.ConnectionStringExists("test123"));
        }
    }
}