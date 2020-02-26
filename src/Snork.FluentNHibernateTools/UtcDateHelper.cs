using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Caching;
using NHibernate;

namespace Snork.FluentNHibernateTools
{
    public static class UtcDateHelper
    {
        private static readonly object DateOffsetMutex = new object();

        private static readonly Dictionary<ISessionFactory, string> OffsetKeys =
            new Dictionary<ISessionFactory, string>();


        private static readonly MemoryCache Cache = new MemoryCache(nameof(UtcDateHelper));

        public static DateTime GetUtcNow(this ISession session, ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            return GetUtcNow(session.SessionFactory, providerType);
        }


        public static DateTime GetUtcNow(this ISessionFactory sessionFactory,
            ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            lock (DateOffsetMutex)
            {
                string key;
                if (!OffsetKeys.ContainsKey(sessionFactory))
                {
                    key = Guid.NewGuid().ToString();
                    OffsetKeys[sessionFactory] = key;
                }
                else
                {
                    key = OffsetKeys[sessionFactory];
                }

                var offset = Cache.Get(key) as TimeSpan?;
                if (offset == null)
                {
                    offset = RefreshUtcOffset(sessionFactory, providerType);
                    Cache.Add(key, offset, DateTimeOffset.Now.AddMinutes(5));
                }

                var utcNow = DateTime.UtcNow.Add(offset.Value);
                return utcNow;
            }
        }

        private static TimeSpan RefreshUtcOffset(ISessionFactory sessionWrapper, ProviderTypeEnum providerType)
        {
            lock (DateOffsetMutex)
            {
                Func<object, DateTime> conversionFunc = a => (DateTime) a;
                using (var session = sessionWrapper.OpenStatelessSession())
                {
                    IQuery query;
                    switch (providerType == ProviderTypeEnum.None
                        ? sessionWrapper.DeriveProviderType()
                        : providerType)
                    {
                        case ProviderTypeEnum.OracleClient10Managed:
                        case ProviderTypeEnum.OracleClient9Managed:
                        case ProviderTypeEnum.OracleClient10:
                        case ProviderTypeEnum.OracleClient9:
                            query = session.CreateSQLQuery("select systimestamp at time zone 'UTC' from dual");
                            break;
                        case ProviderTypeEnum.PostgreSQLStandard:
                        case ProviderTypeEnum.PostgreSQL81:
                        case ProviderTypeEnum.PostgreSQL82:
                            query = session.CreateSQLQuery("SELECT NOW() at time zone 'utc'");
                            break;
                        case ProviderTypeEnum.MySQL:
                            query = session.CreateSQLQuery("select utc_timestamp()");
                            break;
                        case ProviderTypeEnum.SQLite:
                            query = session.CreateSQLQuery("select datetime('now')");
                            break;
                        case ProviderTypeEnum.DB2Informix1150:
                            query = session.CreateSQLQuery("select dbinfo('utc_current')");
                            conversionFunc = o =>
                            {
                                var seconds = Convert.ToInt32(o);
                                return new DateTime(1970, 1, 1).AddSeconds(seconds);
                            };
                            break;
                        case ProviderTypeEnum.Ingres:
                            query = session.CreateSQLQuery("select date_gmt('now')");
                            break;
                        case ProviderTypeEnum.SQLAnywhere10:
                        case ProviderTypeEnum.SQLAnywhere9:
                        case ProviderTypeEnum.SQLAnywhere11:
                        case ProviderTypeEnum.SQLAnywhere12:
                            query = session.CreateSQLQuery("select CURRENT UTC TIMESTAMP");
                            break;
                        case ProviderTypeEnum.MsSql2000:
                        case ProviderTypeEnum.MsSql2005:
                        case ProviderTypeEnum.MsSql2008:
                        case ProviderTypeEnum.MsSql2012:
                        case ProviderTypeEnum.JetDriver:
                            query = session.CreateSQLQuery("select getutcdate()");
                            break;
                        case ProviderTypeEnum.DB2Standard:
                            query = session.CreateSQLQuery("select CURRENT TIMESTAMP - CURRENT TIME ZONE");
                            break;
                        case ProviderTypeEnum.Firebird:
                        case ProviderTypeEnum.MsSqlCe40:
                        default:
                            throw new NotImplementedException(
                                string.Format("This feature is not supported for provider {0}",
                                    providerType == ProviderTypeEnum.None
                                        ? sessionWrapper.DeriveProviderType()
                                        : providerType));
                    }

                    var stopwatch = new Stopwatch();
                    var current = DateTime.UtcNow;
                    stopwatch.Start();
                    var serverUtc = conversionFunc(query.UniqueResult());
                    stopwatch.Stop();
                    return serverUtc.Subtract(current).Subtract(stopwatch.Elapsed);
                }
            }
        }
    }
}