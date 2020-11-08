using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Mapping;

namespace Snork.FluentNHibernateTools
{
    internal class ObjectRenameManager
    {
        private readonly RestorableList<ForeignKey> _foreignKeys = new RestorableList<ForeignKey>();
        private readonly RestorableList<Index> _indexes = new RestorableList<Index>();
        private readonly RestorableList<Table> _tables = new RestorableList<Table>();
        private readonly RestorableList<UniqueKey> _uniqueKeys = new RestorableList<UniqueKey>();

        public void RenameObjects(FluentNHibernatePersistenceBuilderOptions options, Configuration cfg)
        {
            if (options.ObjectRenamer == null) return;

            foreach (var table in cfg.ClassMappings.Select(i => i.Table))
            {
                foreach (var index in table.IndexIterator)
                {
                    var newIndexName = options.ObjectRenamer.Rename(ObjectTypeEnum.Index, index.Name);
                    if (!string.IsNullOrWhiteSpace(newIndexName) && newIndexName != index.Name)
                    {
                        _indexes.Add(new IndexNameRestorer(index, index.Name));
                        index.Name = newIndexName;
                    }
                }

                foreach (var foreignKey in table.ForeignKeyIterator)
                {
                    var newForeignKeyName = options.ObjectRenamer.Rename(ObjectTypeEnum.ForeignKey, foreignKey.Name);
                    if (!string.IsNullOrWhiteSpace(newForeignKeyName) && newForeignKeyName != foreignKey.Name)
                    {
                        _foreignKeys.Add(new ForeignKeyNameRestorer(foreignKey, foreignKey.Name));
                        foreignKey.Name = newForeignKeyName;
                    }
                }

                foreach (var uniqueKey in table.UniqueKeyIterator)
                {
                    var newUniqueKeyName = options.ObjectRenamer.Rename(ObjectTypeEnum.UniqueKey, uniqueKey.Name);
                    if (!string.IsNullOrWhiteSpace(newUniqueKeyName) && newUniqueKeyName != uniqueKey.Name)
                    {
                        _uniqueKeys.Add(new UniqueKeyNameRestorer(uniqueKey, uniqueKey.Name));
                        uniqueKey.Name = newUniqueKeyName;
                    }
                }

                var newTableName = options.ObjectRenamer.Rename(ObjectTypeEnum.Table, table.Name);
                if (!string.IsNullOrWhiteSpace(newTableName) && newTableName != table.Name)
                {
                    _tables.Add(new TableNameRestorer(table, table.Name));


                    table.Name = newTableName;
                }
            }
        }


        public void RestoreOriginalNames()
        {
            _tables.RestoreOriginalName();
            _foreignKeys.RestoreOriginalName();
            _uniqueKeys.RestoreOriginalName();
            _indexes.RestoreOriginalName();
        }

        private class ForeignKeyNameRestorer : NameRestorer<ForeignKey>
        {
            public ForeignKeyNameRestorer(ForeignKey foreignKey, string originalName) : base(foreignKey, originalName)
            {
            }

            public override void Restore()
            {
                Item.Name = OriginalName;
            }
        }

        private class IndexNameRestorer : NameRestorer<Index>
        {
            public IndexNameRestorer(Index index, string originalName) : base(index, originalName)
            {
            }

            public override void Restore()
            {
                Item.Name = OriginalName;
            }
        }

        private class UniqueKeyNameRestorer : NameRestorer<UniqueKey>
        {
            public UniqueKeyNameRestorer(UniqueKey uniqueKey, string originalName) : base(uniqueKey, originalName)
            {
            }

            public override void Restore()
            {
                Item.Name = OriginalName;
            }
        }

        private class TableNameRestorer : NameRestorer<Table>
        {
            public TableNameRestorer(Table table, string originalName) : base(table, originalName)
            {
            }

            public override void Restore()
            {
                Item.Name = OriginalName;
            }
        }

        abstract class NameRestorer<T>
        {
            public NameRestorer(T input, string originalName)
            {
                Item = input;
                OriginalName = originalName;
            }

            public T Item { get; }
            public string OriginalName { get; }
            public abstract void Restore();
        }

        private class RestorableList<T> : List<NameRestorer<T>>
        {
            public void RestoreOriginalName()
            {
                if (this.Any())
                    foreach (var tuple in this)
                        tuple.Restore();
            }
        }
    }
}