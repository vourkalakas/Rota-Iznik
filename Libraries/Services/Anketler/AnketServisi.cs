using Core;
using Core.Data;
using Core.Domain.Anket;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Anketler
{
    public class AnketServisi : IAnketServisi
    {
        #region Fields

        private readonly IDepo<Anket> _anketDepo;
        private readonly IDepo<AnketCevabı> _anketCevabıDepo;
        private readonly IDepo<AnketOyKaydı> _anketOyKaydıDepo;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;

        #endregion

        #region Ctor

        public AnketServisi(IDepo<Anket> anketDepo,
            IDepo<AnketCevabı> anketCevabıDepo,
            IDepo<AnketOyKaydı> anketOyKaydıDepo,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._anketDepo = anketDepo;
            this._anketCevabıDepo = anketCevabıDepo;
            this._anketOyKaydıDepo = anketOyKaydıDepo;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }

        #endregion

        #region Methods
        public virtual void AnketSil(Anket anket)
        {
            if (anket == null)
                throw new ArgumentNullException("anket");

            _anketDepo.Sil(anket);
            _olayYayınlayıcı.OlaySilindi(anket);
        }
        public virtual Anket AnketAlId(int anketId)
        {
            if (anketId == 0)
                return null;

            return  _anketDepo.AlId(anketId);
        }
        public virtual IList<Anket> TümAnketleriAl(bool sadeceAnasayfadakileriYükle = false, string sistemAnahtarKelime = null,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, bool gizliOlanıGöster = false)
        {
            var sorgu = _anketDepo.Tablo;
            if (!gizliOlanıGöster)
            {
                var utcNow = DateTime.UtcNow;
                sorgu = sorgu.Where(p => p.Yayınlandı);
                sorgu = sorgu.Where(p => !p.BaşlangıçTarihi.HasValue || p.BaşlangıçTarihi <= utcNow);
                sorgu = sorgu.Where(p => !p.BitişTarihi.HasValue || p.BitişTarihi >= utcNow);
            }
            if (sadeceAnasayfadakileriYükle)
            {
                sorgu = sorgu.Where(p => p.AnasayfadaGöster);
            }
            if (!String.IsNullOrEmpty(sistemAnahtarKelime))
            {
                sorgu = sorgu.Where(p => p.SistemAnahtarKelime == sistemAnahtarKelime);
            }
            sorgu = sorgu.OrderBy(p => p.GörüntülenmeSırası).ThenBy(p => p.Id);

            var anketler = new SayfalıListe<Anket>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return anketler;
        }
        public virtual void AnketEkle(Anket anket)
        {
            if (anket == null)
                throw new ArgumentNullException("anket");

            _anketDepo.Ekle(anket);
            _olayYayınlayıcı.OlayEklendi(anket);
        }
        public virtual void AnketGüncelle(Anket anket)
        {
            if (anket == null)
                throw new ArgumentNullException("anket");

            _anketDepo.Güncelle(anket);
            _olayYayınlayıcı.OlayGüncellendi(anket);
        }
        public virtual AnketCevabı AnketCevabıAlId(int anketCevabıId)
        {
            if (anketCevabıId == 0)
                return null;

            return _anketCevabıDepo.AlId(anketCevabıId);
        }
        public virtual void AnketCevabıSil(AnketCevabı anketCevabı)
        {
            if (anketCevabı == null)
                throw new ArgumentNullException("pollAnswer");

            _anketCevabıDepo.Sil(anketCevabı);
            _olayYayınlayıcı.OlaySilindi(anketCevabı);
        }
        public virtual bool ZatenOylandı(int anketId, int kullanıcıId)
        {
            if (anketId == 0 || kullanıcıId == 0)
                return false;

            var sonuç = (from pa in _anketCevabıDepo.Tablo
                          join pvr in _anketOyKaydıDepo.Tablo on pa.Id equals pvr.AnketCevapId
                          where pa.AnketId == anketId && pvr.KullanıcıId == kullanıcıId
                          select pvr).Any();
            return sonuç;
        }

        #endregion
    }
}
