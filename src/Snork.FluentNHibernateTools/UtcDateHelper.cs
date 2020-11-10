using System;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate;
using Snork.FluentNHibernateTools.Logging;

namespace Snork.FluentNHibernateTools
{
    public static class UtcDateHelper
    {
        private static readonly object DateOffsetMutex = new object();
        private static DateTime LastRefresh = DateTime.UtcNow;

        private static readonly Dictionary<ISessionFactory, TimeSpan> Offsets =
            new Dictionary<ISessionFactory, TimeSpan>();

        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public static TimeSpan GetUtcOffset(ISession session, ProviderTypeEnum providerType)
        {
            try
            {
                switch (providerType)
                {
                    case ProviderTypeEnum.SQLite:
                    case ProviderTypeEnum.JetDriver:
                        return TimeSpan.Zero;
                }

                if (!Offsets.ContainsKey(session.SessionFactory) ||
                    DateTime.UtcNow.Subtract(LastRefresh).TotalMinutes > 5)
                {
                    RefreshUtcOffset(session.SessionFactory, providerType, session);
                    LastRefresh = DateTime.UtcNow;
                }

                return Offsets[session.SessionFactory];
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Can't update Utc offset", ex);
                return TimeSpan.Zero;
            }
        }

        public static DateTime GetUtcNow(ISession session, ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            lock (DateOffsetMutex)
            {
                var offset = GetUtcOffset(session, providerType);
                return DateTime.UtcNow.Add(offset);
            }
        }

        public static DateTime GetUtcNow(this ISessionFactory sessionFactory,
            ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return GetUtcNow(session, providerType);
            }
        }

        public static void RefreshUtcOffset(ISessionFactory sessionFactory, ProviderTypeEnum providerType,
            ISession session)
        {
            lock (DateOffsetMutex)
            {
                IQuery query;
                Func<object, DateTime> conversionFunc = a => (DateTime) a;
                var deriveProviderType = providerType == ProviderTypeEnum.None
                    ? sessionFactory.DeriveProviderType()
                    : providerType;
                switch (deriveProviderType)
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
                    case ProviderTypeEnum.JetDriver:
                    case ProviderTypeEnum.SQLite:
                        query = null;
                        break;
                    case ProviderTypeEnum.DB2Informix1150:
                        query = session.CreateSQLQuery("select dbinfo('utc_current')");
                        conversionFunc = o =>
                        {
                            var seconds = Convert.ToInt32(o);
                            return new DateTime(1970, 1, 1).AddSeconds(seconds);
                        };
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
                                deriveProviderType));
                }

                TimeSpan offset;
                if (query != null)
                {
                    var stopwatch = new Stopwatch();
                    var current = DateTime.UtcNow;
                    stopwatch.Start();
                    var serverUtc = conversionFunc(query.UniqueResult());
                    stopwatch.Stop();
                    offset = serverUtc.Subtract(current).Subtract(stopwatch.Elapsed);
                }
                else
                {
                    offset = TimeSpan.Zero;
                }

                Offsets[sessionFactory] = offset;
            }
        }
    }
}