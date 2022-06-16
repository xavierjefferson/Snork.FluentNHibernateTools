using FluentNHibernate.Mapping;

namespace Snork.FluentNHibernateTools.Tests
{
    public class DummyMap : ClassMap<Dummy>
    {
        public const string NameIndex = "ix_name1";
        public const string AddressIndex = "ix_address1";
        public const string UniqueIndex = "ux_name_address1";
        public const string TableName = "JoinedDummy";

        public DummyMap()
        {
            Table(TableName);
            Id(i => i.ID);
            Map(i => i.Name, "Name").Length(255).Not.Nullable().UniqueKey(UniqueIndex).Index(NameIndex);
            Map(i => i.Address, "Address").Length(255).Not.Nullable().UniqueKey(UniqueIndex).Index(AddressIndex);
        }
    }
}