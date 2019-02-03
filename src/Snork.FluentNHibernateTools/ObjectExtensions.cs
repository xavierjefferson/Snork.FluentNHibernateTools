namespace Snork.FluentNHibernateTools
{
    public static class ObjectExtensions
    {
        public static T Coalesce<T>(this T obj, T fallback) where T : class
        {
            return (obj == null ? fallback : obj);
        }

        public static TTarget Cast<TTarget>(this object source)
        {
            return ((TTarget) source);
        }
    }
}