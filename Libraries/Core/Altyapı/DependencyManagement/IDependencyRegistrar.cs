using Autofac;
using Core.Yapılandırma;

namespace Core.Altyapı.BağımlılıkYönetimi
{
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, Config config);
        int Order { get; }
    }
}
