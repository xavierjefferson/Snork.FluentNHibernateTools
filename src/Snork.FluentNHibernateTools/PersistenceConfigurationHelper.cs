﻿using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg.Db;
using NHibernate.Mapping.ByCode;

namespace Snork.FluentNHibernateTools
{
    public class PersistenceConfigurationHelper
    {
        /// <summary>
        ///     This method uses some reflection tricks to get the connection string and default schema from an instance of
        ///     IPersistenceConfigurer
        /// </summary>
        /// <param name="configurer"></param>
        /// <returns></returns>
        public static DerivedInfo GetDerivedConnectionInfo(IPersistenceConfigurer configurer)
        {
            var toPropertiesMethodName =
                nameof(Tmp.ToProperties);
            IDictionary<string, string> properties = null;

            var genericBaseType = configurer.GetType().GetBaseTypes().FirstOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(PersistenceConfiguration<,>));
            if (genericBaseType != null)
            {
                var genericType =
                    typeof(PersistenceConfiguration<,>).MakeGenericType(genericBaseType.GetGenericArguments());

                var methodInfo = genericType.GetMethod(toPropertiesMethodName);
                if (methodInfo != null && methodInfo.GetParameters().Length == 0)
                    properties = methodInfo.Invoke(configurer, null) as IDictionary<string, string>;
            }

            if (properties != null)
            {
                var result = new DerivedInfo();
                if (properties.ContainsKey(Tmp.ConnectionStringKey))
                    result.ConnectionString = properties[Tmp.ConnectionStringKey];
                if (properties.ContainsKey(Tmp.DefaultSchemaKey))
                    result.DefaultSchema = properties[Tmp.DefaultSchemaKey];
                if (properties.ContainsKey(Tmp.DialectKey)) result.Dialect = properties[Tmp.DialectKey];
                if (properties.ContainsKey(Tmp.DriverClassKey)) result.Driver = properties[Tmp.DriverClassKey];
                return result;
            }

            return null;
        }


        private class Tmp : PersistenceConfiguration<SQLAnywhereConfiguration,
            SybaseSQLAnywhereConnectionStringBuilder>
        {
            public new const string ConnectionStringKey =
                PersistenceConfiguration<SQLAnywhereConfiguration, SybaseSQLAnywhereConnectionStringBuilder>
                    .ConnectionStringKey;

            public new const string DefaultSchemaKey =
                PersistenceConfiguration<SQLAnywhereConfiguration, SybaseSQLAnywhereConnectionStringBuilder>
                    .DefaultSchemaKey;

            public new const string DialectKey =
                PersistenceConfiguration<SQLAnywhereConfiguration, SybaseSQLAnywhereConnectionStringBuilder>.DialectKey;

            public new const string DriverClassKey =
                PersistenceConfiguration<SQLAnywhereConfiguration, SybaseSQLAnywhereConnectionStringBuilder>
                    .DriverClassKey;
        }

      
    }
}