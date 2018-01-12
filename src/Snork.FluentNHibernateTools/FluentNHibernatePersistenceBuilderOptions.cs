namespace Snork.FluentNHibernateTools
{
    public class FluentNHibernatePersistenceBuilderOptions
    {
        public FluentNHibernatePersistenceBuilderOptions()
        {
            UpdateSchema = true;
        }

        public string DefaultSchema { get; set; }
        public bool UpdateSchema { get; set; }
    }
}