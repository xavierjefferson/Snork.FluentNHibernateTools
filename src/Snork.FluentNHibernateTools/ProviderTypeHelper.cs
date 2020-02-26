using System.Linq;
using FluentNHibernate.Cfg.Db;

namespace Snork.FluentNHibernateTools
{
    public static class ProviderTypeHelper
    {
        public static ProviderTypeEnum InferProviderType(IPersistenceConfigurer config)
        {
            return Constants.ProviderMappings.Where(i => i.ConfigurerType == config.GetType())
                .Select(i => i.ProviderType).FirstOrDefault();
        }
    }
}