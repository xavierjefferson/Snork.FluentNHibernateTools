namespace Snork.FluentNHibernateTools.Tests
{
    internal class PrefixRenamer : IObjectRenamer
    {
        public const string TablePrefix = "TablePrefix_";
        public const string IndexPrefix = "IndexPrefix_";
        public const string UniqueKeyPrefix = "UniqueKeyPrefix_";
        public const string ForeignKeyPrefix = "ForeignKeyPrefix_";


        public string Rename(ObjectTypeEnum type, string name)
        {
            var prefix = "";
            switch (type)
            {
                case ObjectTypeEnum.Index:
                    prefix = IndexPrefix;
                    break;
                case ObjectTypeEnum.ForeignKey:
                    prefix = ForeignKeyPrefix;
                    break;
                case ObjectTypeEnum.Table:
                    prefix = TablePrefix;
                    break;
                case ObjectTypeEnum.UniqueKey:
                    prefix = UniqueKeyPrefix;
                    break;
            }

            return string.Concat(prefix, name);
        }
    }
}