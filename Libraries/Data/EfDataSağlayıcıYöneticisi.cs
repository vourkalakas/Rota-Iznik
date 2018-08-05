using System;
using Core;
using Core.Data;

namespace Data
{
    public partial class EfDataSağlayıcıYöneticisi : TemelVeriSağlayıcıYöneticisi
    {
        public EfDataSağlayıcıYöneticisi(DataAyarları ayarlar) : base(ayarlar)
        {
        }

        public override IDataSağlayıcı DataSağlayıcıYükle()
        {

            var sağlayıcıAdı = Ayarlar.DataSağlayıcı;
            if (String.IsNullOrWhiteSpace(sağlayıcıAdı))
                throw new Hata("Data Ayarları sağlayıcıAdı içermiyor");

            switch (sağlayıcıAdı.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                case "sqlce":
                    return new SqlCeDataProvider();
                default:
                    throw new Hata(string.Format("Desteklenmeyen dataSağlayıcı adı: {0}", sağlayıcıAdı));
            }
        }

    }

}

