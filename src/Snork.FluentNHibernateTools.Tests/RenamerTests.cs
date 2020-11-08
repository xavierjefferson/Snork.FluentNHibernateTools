using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NHibernate;

namespace Snork.FluentNHibernateTools.Tests
{
    [TestClass]
    public class RenamerTests
    {
        private static string _connectionString;
        private static ISessionFactory _sessionFactory;
        private static SessionFactoryInfo info;
        private static FileInfo _fileInfo;

        [TestInitialize]
        public void Initialize()
        {
        }

        [ClassCleanup]
        public static void TestFixtureCleanup()
        {
            _fileInfo.Delete();
        }

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            _fileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".sqlite"));
            _connectionString = string.Format("Data Source={0};Version=3;", _fileInfo.FullName);
            SQLiteConnection.CreateFile(_fileInfo.FullName);

            var persistenceConfigurer = FluentNHibernatePersistenceBuilder.GetPersistenceConfigurer(
                ProviderTypeEnum.SQLite,
                _connectionString,
                new FluentNHibernatePersistenceBuilderOptions());
            info = SessionFactoryBuilder.GetFromAssemblyOf<Dummy>(persistenceConfigurer,
                new FluentNHibernatePersistenceBuilderOptions {ObjectRenamer = new PrefixRenamer()});
            _sessionFactory = info.SessionFactory;

            var uu = UtcDateHelper.GetUtcNow(_sessionFactory, info.ProviderType);
        }

        private ISession GetSession()
        {
            return _sessionFactory.OpenSession();
        }

        private SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            return conn;
        }
     
        [TestMethod]
        public void TestRenamedTables()
        {
            using (var connection = GetConnection())
            {
                using (var dataTable = connection.GetSchema("tables"))
                {
                    PrintDataTable(dataTable);
                    var dataRows = dataTable.Rows.Cast<DataRow>().Where(i =>
                        i["table_name"].ToString() == PrefixRenamer.TablePrefix + DummyMap.TableName).ToList();
                    Assert.AreEqual(1, dataRows.Count, "Table {0}{1} was not found", PrefixRenamer.TablePrefix,
                        DummyMap.TableName);
                }
            }
        }

        private static void PrintDataTable(DataTable dataTable)
        {
            var items = new List<Dictionary<string, object>>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var item = new Dictionary<string, object>();
                foreach (DataColumn dataColumn in dataTable.Columns) item[dataColumn.ColumnName] = dataRow[dataColumn.Ordinal];
                items.Add(item);
            }

            Debug.Print(JsonConvert.SerializeObject(items, Formatting.Indented));
        }

        [TestMethod]
        public void TestRenamedIndexes()
        {
            using (var conn = GetConnection())
            {
                using (var dataTable = conn.GetSchema("indexes"))
                {
                    PrintDataTable(dataTable);
                    var dataRows = dataTable.Rows.Cast<DataRow>().Where(i =>
                        i["table_name"].ToString() == PrefixRenamer.TablePrefix + DummyMap.TableName).ToList();
                    Assert.AreEqual(1,
                        dataRows.Count(i =>
                            i["index_name"].ToString() == PrefixRenamer.IndexPrefix + DummyMap.AddressIndex));
                }
            }
        }
    }
}