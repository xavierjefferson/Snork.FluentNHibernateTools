﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate;

namespace Snork.FluentNHibernateTools
{
    public static class UtcDateHelper
    {
        public const int DefaultSyncRefreshIntervalMinutes = 5;

        private static readonly object DateOffsetMutex = new object();
        private static readonly DateTime LastRefresh = DateTime.Now;

        private static Dictionary<ISessionFactory, TimeSpan> _offsets =
            new Dictionary<ISessionFactory, TimeSpan>();


        public static DateTime GetUtcNow(this ISession session, ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            return GetUtcNow(session.SessionFactory, providerType);
        }


        public static DateTime GetUtcNow(this ISessionFactory sessionFactory,
            ProviderTypeEnum providerType = ProviderTypeEnum.None)
        {
            lock (DateOffsetMutex)
            {
                switch (providerType)
                {
                    case ProviderTypeEnum.SQLite:
                    case ProviderTypeEnum.JetDriver:
                        return DateTime.UtcNow;
                }

                if (DateTime.Now.Subtract(LastRefresh).TotalMinutes >= DefaultSyncRefreshIntervalMinutes)
                    _offsets = new Dictionary<ISessionFactory, TimeSpan>();
                if (!_offsets.ContainsKey(sessionFactory))
                    _offsets[sessionFactory] = RefreshUtcOffset(sessionFactory, providerType);
                var utcNow = DateTime.UtcNow.Add(_offsets[sessionFactory]);
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
                    IQuery query = null;
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
                                    providerType == ProviderTypeEnum.None
                                        ? sessionWrapper.DeriveProviderType()
                                        : providerType));
                    }

                    if (query != null)
                    {
                        var stopwatch = new Stopwatch();
                        var current = DateTime.UtcNow;
                        stopwatch.Start();
                        var serverUtc = conversionFunc(query.UniqueResult());
                        stopwatch.Stop();
                        return serverUtc.Subtract(current).Subtract(stopwatch.Elapsed);
                    }

                    return TimeSpan.Zero;
                }
            }
        }
    }
}