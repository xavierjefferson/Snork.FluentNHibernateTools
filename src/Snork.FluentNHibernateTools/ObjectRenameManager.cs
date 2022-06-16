using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Mapping;
using Snork.FluentNHibernateTools.Logging;

namespace Snork.FluentNHibernateTools
{
    internal class ObjectRenameManager : IDisposable
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly Queue<Action> _recoveryActions = new Queue<Action>();

        public void Dispose()
        {
            RestoreOriginalNames();
        }

        private void RenameObject<T>(IEnumerable<T> items, ObjectTypeEnum objectType,
            FluentNHibernatePersistenceBuilderOptions options, Func<T, string> nameGetter, Action<T, string> nameSetter)

        {
            foreach (var item in items)
            {
                var originalName = nameGetter(item);
                var newName = options.ObjectRenamer.Rename(objectType, originalName);
                if (!string.IsNullOrWhiteSpace(newName) && newName != originalName)
                {
                    _recoveryActions.Enqueue(() =>
                    {
                        Logger.DebugFormat("Renaming {0} '{1}' back to '{2}'", typeof(T).Name.ToLower(), newName,
                            originalName);
                        nameSetter(item, originalName);
                    });
                    Logger.DebugFormat("Temporarily renamed {2} {0} to {1}", originalName, newName, typeof(T).Name.ToLower());
                    nameSetter(item, newName);
                }
            }
        }

        public void RenameObjects(FluentNHibernatePersistenceBuilderOptions options, Configuration cfg)
        {
            if (options.ObjectRenamer == null) return;

            foreach (var table in cfg.ClassMappings.Select(i => i.Table))
            {
                RenameObject(table.IndexIterator, ObjectTypeEnum.Index, options, i => i.Name,
                    (index, name) => index.Name = name);
                RenameObject(table.UniqueKeyIterator, ObjectTypeEnum.UniqueKey, options, i => i.Name,
                    (uniqueKey, name) => uniqueKey.Name = name);
                RenameObject(table.ForeignKeyIterator, ObjectTypeEnum.ForeignKey, options, i => i.Name,
                    (foreignKey, name) => foreignKey.Name = name);
                RenameObject(new List<Table> {table}, ObjectTypeEnum.Table, options, i => i.Name,
                    (table1, name) => table1.Name = name);
            }
        }


        public void RestoreOriginalNames()
        {
            foreach (var action in _recoveryActions) action();
        }
    }
}