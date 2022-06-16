namespace Snork.FluentNHibernateTools.Tests
{
    public class JoinedDummy
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }

        public virtual Dummy MyDummy { get; set; }
    }
}