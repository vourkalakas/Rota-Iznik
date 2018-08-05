using System;
using System.Collections.Generic;
using Core.Domain.Sayfalar;
using Core.Data;
using Services.Olaylar;
using System.Linq;

namespace Services.Sayfalar
{
    public partial class SayfaTemaServisi : ISayfaTemaServisi
    {
        private readonly IDepo<SayfaTema> _sayfaTemaDepo;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        public SayfaTemaServisi(IDepo<SayfaTema> sayfaTemaDepo,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._sayfaTemaDepo = sayfaTemaDepo;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }
        public virtual SayfaTema SayfaTemaAlId(int sayfaTemaId)
        {
            if (sayfaTemaId == 0)
                return null;
            return _sayfaTemaDepo.AlId(sayfaTemaId);
        }

        public virtual void SayfaTemaEkle(SayfaTema sayfaTema)
        {
            if (sayfaTema == null)
                throw new ArgumentNullException("sayfaTema");
            _sayfaTemaDepo.Ekle(sayfaTema);
            _olayYayınlayıcı.OlayEklendi(sayfaTema);
        }

        public virtual void SayfaTemaGüncelle(SayfaTema sayfaTema)
        {
            if (sayfaTema == null)
                throw new ArgumentNullException("sayfaTema");
            _sayfaTemaDepo.Güncelle(sayfaTema);
            _olayYayınlayıcı.OlayGüncellendi(sayfaTema);
        }

        public virtual void SayfaTemaSil(SayfaTema sayfaTema)
        {
            if (sayfaTema == null)
                throw new ArgumentNullException("sayfaTema");
            _sayfaTemaDepo.Sil(sayfaTema);
            _olayYayınlayıcı.OlaySilindi(sayfaTema);
        }

        public virtual IList<SayfaTema> TümSayfaTemalar()
        {
            var sorgu = from pt in _sayfaTemaDepo.Tablo
                        orderby pt.GörüntülenmeSırası, pt.Id
                        select pt;
            var temalar = sorgu.ToList();
            return temalar;
        }
    }
}
