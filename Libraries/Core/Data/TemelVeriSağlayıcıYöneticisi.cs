using System;

namespace Core.Data
{
    public abstract class TemelVeriSağlayıcıYöneticisi
    {
        protected TemelVeriSağlayıcıYöneticisi(DataAyarları ayarlar)
        {
            if (ayarlar == null)
                throw new ArgumentNullException("ayarlar");
            this.Ayarlar = ayarlar;
        }
        protected DataAyarları Ayarlar { get; private set; }
        public abstract IDataSağlayıcı DataSağlayıcıYükle();
    }
}
