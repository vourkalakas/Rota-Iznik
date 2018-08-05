using System;
using Autofac;
using Core.Data;
using Core.Altyapı.BağımlılıkYönetimi;
using Data;

namespace Web.Framework.Altyapı
{
    public static class DependencyRegistrarUzantıları
    {
        public static void RegisterPluginDataContext<T>(this IDependencyRegistrar dependencyRegistrar,
            ContainerBuilder builder, string contextName, string filePath = null, bool reloadSettings = false) where T : IDbContext
        {
            //data layer
            var dataSettingsManager = new DataAyarlarıYönetici();
            var dataProviderSettings = dataSettingsManager.AyarlarıYükle(filePath, reloadSettings);

            if (dataProviderSettings != null && dataProviderSettings.Geçerli())
            {
                //register named context
                builder.Register(c => (IDbContext)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }))
                    .Named<IDbContext>(contextName)
                    .InstancePerLifetimeScope();

                builder.Register(c => (T)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }))
                    .InstancePerLifetimeScope();
            }
            else
            {
                //register named context
                builder.Register(c => (T)Activator.CreateInstance(typeof(T), new object[] { c.Resolve<DataAyarları>().DataConnectionString }))
                    .Named<IDbContext>(contextName)
                    .InstancePerLifetimeScope();

                builder.Register(c => (T)Activator.CreateInstance(typeof(T), new object[] { c.Resolve<DataAyarları>().DataConnectionString }))
                    .InstancePerLifetimeScope();
            }
        }
    }
}
