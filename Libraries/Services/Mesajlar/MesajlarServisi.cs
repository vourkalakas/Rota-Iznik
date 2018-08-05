using Core;
using Core.Data;
using Core.Domain.Mesajlar;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Mesajlar
{
    public class MesajlarServisi : IMesajlarServisi
    {
        private const string MESAJLAR_ALL_KEY = "mesajlar.all-{0}-{1}";
        private const string MESAJLAR_BY_ID_KEY = "mesajlar.id-{0}";
        private const string MESAJLAR_PATTERN_KEY = "mesajlar.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Mesaj> _mesajlarDepo;
        public MesajlarServisi(IDepo<Mesaj> mesajlarDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._mesajlarDepo = mesajlarDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Mesaj MesajlarAlId(int mesajlarId)
        {
            if (mesajlarId == 0)
                return null;

            string key = string.Format(MESAJLAR_BY_ID_KEY, mesajlarId);
            return _önbellekYönetici.Al(key, () => _mesajlarDepo.AlId(mesajlarId));
        }
        public IList<Mesaj> MesajlarAlKullanıcıId(int kullanıcıId)
        {
            if (kullanıcıId == 0)
                return null;

            string key = string.Format(MESAJLAR_BY_ID_KEY, kullanıcıId);
            return _önbellekYönetici.Al(key, () => _mesajlarDepo.Tablo.Where(x=>x.KullanıcıId==kullanıcıId).ToList());
        }

        public void MesajlarEkle(Mesaj mesajlar)
        {
            if (mesajlar == null)
                throw new ArgumentNullException("mesajlar");

            _mesajlarDepo.Ekle(mesajlar);
            _önbellekYönetici.KalıpİleSil(MESAJLAR_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(mesajlar);
        }

        public void MesajlarGüncelle(Mesaj mesajlar)
        {
            if (mesajlar == null)
                throw new ArgumentNullException("mesajlar");

            _mesajlarDepo.Güncelle(mesajlar);
            _önbellekYönetici.KalıpİleSil(MESAJLAR_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(mesajlar);
        }

        public void MesajlarSil(Mesaj mesajlar)
        {
            if (mesajlar == null)
                throw new ArgumentNullException("mesajlar");

            _mesajlarDepo.Sil(mesajlar);
            _önbellekYönetici.KalıpİleSil(MESAJLAR_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(mesajlar);
        }

        public IList<Mesaj> TümMesajlarAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(MESAJLAR_ALL_KEY,  AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _mesajlarDepo.Tablo;
                query = query.OrderByDescending(qe => qe.OlusmaTarihi);
                return query.ToList();
            });
        }
        public ISayfalıListe<Mesaj> MesajAra(string baslik, string msj,
           DateTime? tarihi,bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _mesajlarDepo.Tablo;
            if (!String.IsNullOrEmpty(baslik))
                sorgu = sorgu.Where(qe => qe.Baslik.Contains(baslik));
            if (!String.IsNullOrEmpty(msj))
                sorgu = sorgu.Where(qe => qe.Msj.Contains(msj));
            if (tarihi.HasValue)
                sorgu = sorgu.Where(qe => qe.OlusmaTarihi == tarihi);
            sorgu = enYeniler ?
                sorgu.OrderByDescending(qe => qe.OlusmaTarihi) :
                sorgu.OrderByDescending(qe => qe.OlusmaTarihi).ThenBy(qe => qe.OlusmaTarihi);

            var mesajlar = new SayfalıListe<Mesaj>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return mesajlar;
        }
    }

}
