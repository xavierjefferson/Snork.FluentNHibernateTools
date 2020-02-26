using System.Linq;
using NHibernate;
using NHibernate.Impl;

namespace Snork.FluentNHibernateTools
{
    public static class ProviderMappingHelper
    {
        public static ProviderTypeEnum DeriveProviderType(this ISessionFactory sessionFactory)
        {
            var sessionFactoryImpl = sessionFactory as SessionFactoryImpl;
            if (sessionFactoryImpl?.Dialect == null)
                return ProviderTypeEnum.None;

            var mapping = Constants.ProviderMappings.FirstOrDefault(i =>
                i.DialectAndDriverMatches(sessionFactoryImpl.Dialect.GetType(),
                    sessionFactoryImpl.ConnectionProvider?.Driver?.GetType()));
            return mapping?.ProviderType ?? ProviderTypeEnum.None;
        }
    }
}