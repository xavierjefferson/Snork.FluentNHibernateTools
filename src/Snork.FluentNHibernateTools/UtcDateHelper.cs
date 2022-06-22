using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using NHibernate;

namespace Snork.FluentNHibernateTools
{
    public static class UtcDateHelper
    {
        private static readonly object DateOffsetMutex = new object();

        private static readonly Dictionary<ISessionFactory, TimeSpan> Offsets =
            new Dictionary<ISessionFactory, TimeSpan>();


        public static DateTime GetUtcNow(this ISession session, ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            return GetUtcNow(session.SessionFactory, providerType);
        }

        public static TimeSpan GetUtcOffset(this ISession session,
            ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            return GetUtcOffset(session.SessionFactory, providerType);
        }

        public static TimeSpan GetUtcOffset(this ISessionFactory sessionFactory,
            ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            lock (DateOffsetMutex)
            {
                if (!Offsets.ContainsKey(sessionFactory))
                    Offsets[sessionFactory] = RefreshUtcOffset(sessionFactory, providerType);
                return Offsets[sessionFactory];
            }
        }

        public static DateTime GetUtcNow(this ISessionFactory sessionFactory,
            ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            return DateTime.UtcNow.Add(GetUtcOffset(sessionFactory, providerType));
        }

        private static bool IsLocalIpAddress(string host)
        {
            try
            {
                // get host IP addresses
                var hostAddresses = Dns.GetHostAddresses(host);
                // get local IP addresses
                var localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (var address in hostAddresses)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(address)) return true;
                    // is local address
                    foreach (var localIP in localIPs)
                        if (address.Equals(localIP))
                            return true;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }

        private static TimeSpan RefreshUtcOffset(ISessionFactory sessionFactory, ProviderTypeEnum providerType)
        {
            if (providerType == ProviderTypeEnum.None) providerType = sessionFactory.DeriveProviderType();

            lock (DateOffsetMutex)
            {
                //desktop databases don't have an offset
                switch (providerType)
                {
                    case ProviderTypeEnum.JetDriver:
                    case ProviderTypeEnum.MsSqlCe40:
                    case ProviderTypeEnum.SQLite:
                        var tmp = DateTime.UtcNow;
                        return tmp.Subtract(tmp.ToLocalTime());
                }

                Func<object, DateTime> conversionFunc = a => (DateTime) a;
                using (var session = sessionFactory.OpenStatelessSession())
                {
                    if (IsLocalIpAddress(session.Connection.DataSource)) return TimeSpan.Zero;

                    IQuery query = null;

                    switch (providerType)
                    {
                        case ProviderTypeEnum.Firebird:
                            query = session.CreateSQLQuery("select CURRENT_TIMESTAMP from rdb$database");
                            break;
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
                        case ProviderTypeEnum.SQLAnywhere17:
                            query = session.CreateSQLQuery("select CURRENT UTC TIMESTAMP");
                            break;
                        case ProviderTypeEnum.MsSql2000:
                        case ProviderTypeEnum.MsSql2005:
                        case ProviderTypeEnum.MsSql2008:
                        case ProviderTypeEnum.MsSql2012:
                            if (session.Connection.DataSource != null &&
                                session.Connection.DataSource.StartsWith(".")) return TimeSpan.Zero;
                            query = session.CreateSQLQuery("select getutcdate()");
                            break;
                        case ProviderTypeEnum.DB2Standard:
                            query = session.CreateSQLQuery("select CURRENT TIMESTAMP - CURRENT TIME ZONE");
                            break;

                        default:
                            var dialect = sessionFactory.GetDialect();
                            if (dialect != null && dialect.SupportsCurrentUtcTimestampSelection)
                                query = session.CreateSQLQuery(dialect.CurrentUtcTimestampSelectString);

                            if (query == null)
                                throw new NotImplementedException(
                                    $"This feature is not supported for provider {providerType}");

                            break;
                    }

                    var stopwatch = new Stopwatch();
                    var current = DateTime.UtcNow;
                    stopwatch.Start();
                    var serverUtc = DateTime.SpecifyKind(conversionFunc(query.UniqueResult()), DateTimeKind.Utc);
                    stopwatch.Stop();
                    return serverUtc.Subtract(current).Subtract(stopwatch.Elapsed);
                }
            }
        }
    }
}