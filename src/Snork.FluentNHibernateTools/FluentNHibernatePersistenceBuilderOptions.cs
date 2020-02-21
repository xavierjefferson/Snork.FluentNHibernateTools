using System.Xml.Serialization;

namespace Snork.FluentNHibernateTools
{
    public class FluentNHibernatePersistenceBuilderOptions
    {
        public FluentNHibernatePersistenceBuilderOptions()
        {
            UpdateSchema = true;
        }

        /// <summary>
        ///     Database schema into which tables will be created. Default is database provider specific ("dbo" for SQL Server,
        ///     "public" for PostgreSQL, etc).
        /// </summary>
        public string DefaultSchema { get; set; }

        /// <summary>
        ///     if set to true, then this provider creates database tables. Default is true
        /// </summary>
        public bool UpdateSchema { get; set; }

        /// <summary>
        ///     Instance of some class that can rename schema objects before creation or use.
        /// </summary>
        [XmlIgnore]
        public virtual IObjectRenamer ObjectRenamer { get; set; }
    }
}