namespace Snork.FluentNHibernateTools
{
    public interface IObjectRenamer
    {
        /// <summary>
        /// If an object is to be renamed, calculate its new name and return it to the caller
        /// </summary>
        /// <param name="type">The object type (table, foreign key, unique key)</param>
        /// <param name="name">The object name</param>
        /// <returns>The same object name passed in or null means no change.  A different string means to change the name.</returns>
        string Rename(ObjectTypeEnum type, string name);
    }
}