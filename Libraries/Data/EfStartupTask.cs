using Core;
using Core.Data;
using Core.Altyapı;

namespace Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var ayarlar = EngineContext.Current.Resolve<DataAyarları>();
            if (ayarlar != null && ayarlar.Geçerli())
            {
                var sağlayıcı = EngineContext.Current.Resolve<IDataSağlayıcı>();
                if (sağlayıcı == null)
                    throw new Hata("IDataSağlayıcı bulunamadı");
                sağlayıcı.VeritabanıBaşlatıcıyıAyarla();
            }
        }
        public int Order
        {
            get { return -1000; }
        }
    }
}
