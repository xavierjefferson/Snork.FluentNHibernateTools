using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg.Db;
using NHibernate.Mapping.ByCode;

namespace Snork.FluentNHibernateTools
{
    public class PersistenceConfigurationHelper
    {
        public static DerivedInfo GetDerivedConnectionInfo(IPersistenceConfigurer configurer)
        {
            var toPropertiesMethodName =
                nameof(Tmp.ToProperties);


            var genericBaseType = configurer.GetType().GetBaseTypes().FirstOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(PersistenceConfiguration<,>));
            if (genericBaseType != null)
            {
                var genericType =
                    typeof(PersistenceConfiguration<,>).MakeGenericType(genericBaseType.GetGenericArguments());

                var methodInfo = genericType.GetMethod(toPropertiesMethodName);
                if (methodInfo != null && methodInfo.GetParameters().Length == 0)
                {
                    var result = new DerivedInfo();
                    var connectionString = methodInfo.Invoke(configurer, null) as IDictionary<string, string>;
                    if (connectionString != null)
                    {
                        result.ConnectionString = connectionString[Tmp.ConnectionStringKey];
                        if (connectionString.ContainsKey(Tmp.DefaultSchemaKey))
                        {
                            result.DefaultSchema = connectionString[Tmp.DefaultSchemaKey];
                        }
                    }

                    return result;
                }
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
        }
    }
}