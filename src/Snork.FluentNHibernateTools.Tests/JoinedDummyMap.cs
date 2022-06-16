using FluentNHibernate.Mapping;

namespace Snork.FluentNHibernateTools.Tests
{
    public class JoinedDummyMap : ClassMap<JoinedDummy>
    {
        public const string NameIndex = "ix_name";
        public const string AddressIndex = "ix_address";
        public const string UniqueIndex = "ux_name_address";
        public const string TableName = "GreatBigDummy";

        public JoinedDummyMap()
        {
            Table(TableName);
            Id(i => i.ID);
            Map(i => i.Name, "Name").Length(255).Not.Nullable().UniqueKey(UniqueIndex).Index(NameIndex);
            Map(i => i.Address, "Address").Length(255).Not.Nullable().UniqueKey(UniqueIndex).Index(AddressIndex);
            References(i => i.MyDummy, "MyDummyId");
        }
    }
}