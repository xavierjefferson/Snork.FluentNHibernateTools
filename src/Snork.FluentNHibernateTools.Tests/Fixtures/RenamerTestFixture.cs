using System;
using System.Data.SQLite;
using System.IO;
using NHibernate;

namespace Snork.FluentNHibernateTools.Tests
{
    public class RenamerTestFixture : IDisposable
    {
        private readonly FileInfo _fileInfo;
        private readonly SessionFactoryInfo info;

        public RenamerTestFixture()
        {
            _fileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".sqlite"));
            ConnectionString = string.Format("Data Source={0};Version=3;", _fileInfo.FullName);
            SQLiteConnection.CreateFile(_fileInfo.FullName);

            var persistenceConfigurer = FluentNHibernatePersistenceBuilder.GetPersistenceConfigurer(
                ProviderTypeEnum.SQLite,
                ConnectionString,
                new FluentNHibernatePersistenceBuilderOptions());
            info = SessionFactoryBuilder.GetFromAssemblyOf<Dummy>(persistenceConfigurer,
                new FluentNHibernatePersistenceBuilderOptions {ObjectRenamer = new PrefixRenamer()});
            SessionFactory = info.SessionFactory;

            var uu = SessionFactory.GetUtcNow(info.ProviderType);
        }

        public string ConnectionString { get; }
        public ISessionFactory SessionFactory { get; }

        public void Dispose()
        {
            SessionFactory?.Dispose();
            _fileInfo.Delete();
        }
    }
}