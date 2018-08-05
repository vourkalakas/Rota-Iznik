using Core.Data;
using Core.Domain.Genel;
using Data;
using System;
using System.Linq;

namespace Services.Genel
{
    public partial class TamMetinServisi : ITamMetinServisi
    {
        private readonly IDataSağlayıcı _dataSağlayıcı;
        private readonly IDbContext _dbContext;
        private readonly GenelAyarlar _genelAyarlar;
        public TamMetinServisi(IDataSağlayıcı dataSağlayıcı,
            IDbContext dbContext,
            GenelAyarlar genelAyarlar)
        {
            this._dataSağlayıcı = dataSağlayıcı;
            this._dbContext = dbContext;
            this._genelAyarlar = genelAyarlar;
        }
        public bool TamMetinDestekli()
        {
            if (_genelAyarlar.StoredProcedureKullanDestekliyse && _dataSağlayıcı.StoredProceduredDestekli)
            {
                var result = _dbContext.SqlSorgusu<int>("EXEC [TamMetin_Destekli]");
                return result.FirstOrDefault() > 0;
            }
            return false;
        }

        public void TamMetinDevreDışıBırak()
        {
            if (_genelAyarlar.StoredProcedureKullanDestekliyse && _dataSağlayıcı.StoredProceduredDestekli)
            {
                _dbContext.SqlKomutunuÇalıştır("EXEC [TamMetin_Devredışı]", true);
            }
            else
            {
                throw new Exception("Stored procedures veritabanında desteklenmiyor");
            }
        }

        public void TamMetinEtkinleştir()
        {
            if (_genelAyarlar.StoredProcedureKullanDestekliyse && _dataSağlayıcı.StoredProceduredDestekli)
            {
                _dbContext.SqlKomutunuÇalıştır("EXEC [TamMetin_Etkin]", true);
            }
            else
            {
                throw new Exception("Stored procedures veritabanında desteklenmiyor");
            }
        }
    }
}
