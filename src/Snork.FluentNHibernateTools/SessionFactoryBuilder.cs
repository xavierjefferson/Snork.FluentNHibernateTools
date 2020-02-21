using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Newtonsoft.Json;
using NHibernate.Tool.hbm2ddl;
using Snork.FluentNHibernateTools.Logging;

namespace Snork.FluentNHibernateTools
{
    public static class SessionFactoryBuilder
    {
        private static readonly object Mutex = new object();

        private static readonly Dictionary<string, SessionFactoryInfo> SessionFactoryInfos =
            new Dictionary<string, SessionFactoryInfo>();

        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();


        private static string CalculateSHA512Hash(string input)

        {
            // step 1, calculate Sha512 hash from input

            var sha512 = SHA512.Create();

            var inputBytes = Encoding.ASCII.GetBytes(input);

            var hash = sha512.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            var sb = new StringBuilder();

            foreach (var t in hash)
                sb.Append(t.ToString("X2"));

            return sb.ToString();
        }

        public static SessionFactoryInfo GetByKey(string key)
        {
            lock (Mutex)
            {
                return SessionFactoryInfos[key];
            }
        }

        public static SessionFactoryInfo GetFromAssemblyOf<T>(IPersistenceConfigurer configurer,
            FluentNHibernatePersistenceBuilderOptions options = null)
        {
            return GetFromAssemblies(new List<Assembly> {typeof(T).Assembly}, configurer, options);
        }

        public static SessionFactoryInfo GetFromAssemblies(List<Assembly> sourceAssemblies,
            IPersistenceConfigurer configurer,
            FluentNHibernatePersistenceBuilderOptions options = null)
        {
            options = options ?? new FluentNHibernatePersistenceBuilderOptions();
            var providerType = ProviderTypeHelper.InferProviderType(configurer);
            var derivedInfo = PersistenceConfigurationHelper.GetDerivedConnectionInfo(configurer);
            if (derivedInfo == null)
                throw new ArgumentException("Could not derive connection string info from configurer");
            options.DefaultSchema = derivedInfo.DefaultSchema;
            Func<ConfigurationInfo> configFunc = () => new ConfigurationInfo(configurer, options, providerType);
            var keyInfo = new KeyInfo
            {
                ProviderType = providerType,
                NameOrConnectionString = derivedInfo.ConnectionString,
                Options = options,
                AssemblyNames = string.Join(",", sourceAssemblies.Distinct().OrderBy(i => i.FullName))
            };
            return GetSessionFactoryInfo(sourceAssemblies, options, keyInfo, providerType, configFunc);
        }

        private static SessionFactoryInfo GetSessionFactoryInfo(List<Assembly> sourceAssemblies,
            FluentNHibernatePersistenceBuilderOptions options,
            KeyInfo keyInfo,
            ProviderTypeEnum providerType, Func<ConfigurationInfo> configFunc)
        {
            var key = CalculateSHA512Hash(JsonConvert.SerializeObject(keyInfo));

            lock (Mutex)
            {
                if (SessionFactoryInfos.ContainsKey(key))
                    return SessionFactoryInfos[key];
                var configurationInfo = configFunc();
                var configurationInfoPersistenceConfigurer = configurationInfo.PersistenceConfigurer;
                var configuration = Fluently.Configure().Database(configurationInfoPersistenceConfigurer);
                foreach (var assembly in sourceAssemblies)
                    configuration = configuration.Mappings(x => x.FluentMappings.AddFromAssembly(assembly));

                var objectNameStore = new ObjectRenameManager();


                //make sure we only execute ExposeConfiguration once.  It will try to execute again when we build the session factory
                var exposed = false;
                configuration.ExposeConfiguration(cfg =>
                {
                    if (exposed) return;
                    exposed = true;

                    objectNameStore.RenameObjects(options, cfg);


                    if (options.UpdateSchema)
                    {
                        var schemaUpdate = new SchemaUpdate(cfg);
                        using (LogProvider.OpenNestedContext("Schema update"))
                        {
                            Logger.Debug("Starting schema update");
                            schemaUpdate.Execute(i => Logger.Debug(i), true);
                            Logger.Debug("Done with schema update");
                        }


                        if (schemaUpdate.Exceptions != null && schemaUpdate.Exceptions.Any())
                            throw new SchemaException(
                                string.Format("Schema update failed with {1} exceptions.  See {0} property",
                                    nameof(SchemaException.Exceptions), schemaUpdate.Exceptions.Count),
                                schemaUpdate.Exceptions);
                    }
                });

                //configuration.BuildConfiguration();


                SessionFactoryInfos[key] = new SessionFactoryInfo(key, configuration.BuildSessionFactory(),
                    providerType, options);

                //restore object names
                objectNameStore.RestoreOriginalNames();


                return SessionFactoryInfos[key];
            }
        }


        public static SessionFactoryInfo GetFromAssemblyOf<T>(ProviderTypeEnum providerType,
            string nameOrConnectionString,
            FluentNHibernatePersistenceBuilderOptions options = null)
        {
            return GetFromAssemblies(new List<Assembly> {typeof(T).Assembly}, providerType, nameOrConnectionString,
                options);
        }

        public static SessionFactoryInfo GetFromAssemblies(List<Assembly> sourceAssemblies,
            ProviderTypeEnum providerType,
            string nameOrConnectionString,
            FluentNHibernatePersistenceBuilderOptions options = null)
        {
            options = options ?? new FluentNHibernatePersistenceBuilderOptions();
            Func<ConfigurationInfo> configFunc = () => FluentNHibernatePersistenceBuilder.Build(providerType,
                nameOrConnectionString, options);
            var keyInfo = new KeyInfo
            {
                ProviderType = providerType,
                NameOrConnectionString = nameOrConnectionString,
                Options = options,
                AssemblyNames = string.Join(",", sourceAssemblies.Distinct().OrderBy(i => i.FullName))
            };
            return GetSessionFactoryInfo(sourceAssemblies, options, keyInfo, providerType, configFunc);
        }


        private class KeyInfo
        {
            public ProviderTypeEnum ProviderType { get; set; }
            public string NameOrConnectionString { get; set; }
            public string AssemblyNames { get; set; }
            public FluentNHibernatePersistenceBuilderOptions Options { get; set; }
        }
    }
}