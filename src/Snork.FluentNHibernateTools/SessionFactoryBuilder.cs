using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;

namespace Snork.FluentNHibernateTools
{
    public static class SessionFactoryBuilder
    {
        private static readonly object Mutex = new object();

        private static readonly Dictionary<string, SessionFactoryInfo> SessionFactoryInfos =
            new Dictionary<string, SessionFactoryInfo>();

        private static string CalculateMD5Hash(string input)

        {
            // step 1, calculate MD5 hash from input

            var md5 = MD5.Create();

            var inputBytes = Encoding.ASCII.GetBytes(input);

            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            var sb = new StringBuilder();

            for (var i = 0; i < hash.Length; i++)

                sb.Append(hash[i].ToString("X2"));

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
                options = options,
                AssemblyNames = string.Join(",", sourceAssemblies.Distinct().OrderBy(i => i.FullName))
            };
            return GetSessionFactoryInfo(sourceAssemblies, options, keyInfo, providerType, configFunc);
        }

        private static SessionFactoryInfo GetSessionFactoryInfo(List<Assembly> sourceAssemblies,
            FluentNHibernatePersistenceBuilderOptions options,
            KeyInfo keyInfo,
            ProviderTypeEnum providerType, Func<ConfigurationInfo> configFunc)
        {
            var key = CalculateMD5Hash(new JavaScriptSerializer().Serialize(keyInfo));

            lock (Mutex)
            {
                if (SessionFactoryInfos.ContainsKey(key))
                    return SessionFactoryInfos[key];
                var configurationInfo = configFunc();
                var configurationInfoPersistenceConfigurer = configurationInfo.PersistenceConfigurer;
                var configuration = Fluently.Configure().Database(configurationInfoPersistenceConfigurer);
                foreach (var assembly in sourceAssemblies)
                    configuration = configuration.Mappings(x => x.FluentMappings.AddFromAssembly(assembly));
                configuration.ExposeConfiguration(cfg =>
                {
                    if (options.UpdateSchema)
                    {
                        var schemaUpdate = new SchemaUpdate(cfg);
                        using (var stringWriter = new StringWriter())
                        {
                            try
                            {
                                schemaUpdate.Execute(i => stringWriter.WriteLine(i), true);
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                            var d = stringWriter.ToString();
                        }
                    }
                });
                configuration.BuildConfiguration();
                SessionFactoryInfos[key] = new SessionFactoryInfo(key, configuration.BuildSessionFactory(),
                    providerType, options);
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
                options = options,
                AssemblyNames = string.Join(",", sourceAssemblies.Distinct().OrderBy(i => i.FullName))
            };
            return GetSessionFactoryInfo(sourceAssemblies, options, keyInfo, providerType, configFunc);
        }

        private class KeyInfo
        {
            public ProviderTypeEnum ProviderType { get; set; }
            public string NameOrConnectionString { get; set; }
            public string AssemblyNames { get; set; }
            public FluentNHibernatePersistenceBuilderOptions options { get; set; }
        }
    }
}