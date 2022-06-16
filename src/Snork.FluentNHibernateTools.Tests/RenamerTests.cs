using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using NHibernate;
using Xunit;

namespace Snork.FluentNHibernateTools.Tests
{
    [Collection(Constants.RenamerTestFixtureCollectionName)]
    public class RenamerTests
    {
        public RenamerTests(RenamerTestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        private readonly RenamerTestFixture _testFixture;


        private ISession GetSession()
        {
            return _testFixture.SessionFactory.OpenSession();
        }

        private SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(_testFixture.ConnectionString);
            conn.Open();
            return conn;
        }

        private static void PrintDataTable(DataTable dataTable)
        {
            var items = new List<Dictionary<string, object>>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var item = new Dictionary<string, object>();
                foreach (DataColumn dataColumn in dataTable.Columns)
                    item[dataColumn.ColumnName] = dataRow[dataColumn.Ordinal];
                items.Add(item);
            }

            Debug.Print(JsonConvert.SerializeObject(items, Formatting.Indented));
        }

        private static List<DataRow> GetDataRowList(DataTable dataTable)
        {
            var list = new List<DataRow>();
            foreach (var m in dataTable.Rows) list.Add(m as DataRow);

            return list;
        }

        [Fact]
        public void TestRenamedIndexes()
        {
            using (var conn = GetConnection())
            {
                using (var dataTable = conn.GetSchema("indexes"))
                {
                    PrintDataTable(dataTable);

                    var dataRows = GetDataRowList(dataTable).Where(i =>
                        i["table_name"].ToString() == PrefixRenamer.TablePrefix + DummyMap.TableName).ToList();
                    Assert.Equal(1,
                        dataRows.Count(i =>
                            i["index_name"].ToString() == PrefixRenamer.IndexPrefix + DummyMap.AddressIndex));
                }
            }
        }

        [Fact]
        public void TestRenamedTables()
        {
            using (var connection = GetConnection())
            {
                using (var dataTable = connection.GetSchema("tables"))
                {
                    PrintDataTable(dataTable);
                    var dataRows = GetDataRowList(dataTable).Where(i =>
                        i["table_name"].ToString() == PrefixRenamer.TablePrefix + DummyMap.TableName).ToList();
                    Assert.Single(dataRows);
                }
            }
        }
    }
}