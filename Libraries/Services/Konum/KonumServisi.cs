using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Data;
using Core.Domain.Konum;
using Core.Önbellek;
using Services.Olaylar;

namespace Services.Konum
{
    public class KonumServisi : IKonumServisi
    {
        private readonly IDepo<Ulke> _ülkeDepo;
        private readonly IDepo<Ilce> _ilceDepo;
        private readonly IDepo<Sehir> _sehirDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        public KonumServisi(IDepo<Ulke> ülkeDepo,
            IDepo<Ilce> ilceDepo,
            IDepo<Sehir> sehirDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            this._ülkeDepo = ülkeDepo;
            this._sehirDepo = sehirDepo;
            this._ilceDepo = ilceDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
        }
        #region Ulke
        public Ulke UlkeAlId(int ülkeId)
        {
            if (ülkeId == 0)
                return null;
            return _ülkeDepo.AlId(ülkeId);
        }

        public void UlkeEkle(Ulke ülke)
        {
            if (ülke == null)
                throw new ArgumentNullException("ülke");

            _ülkeDepo.Ekle(ülke);
            _olayYayınlayıcı.OlayEklendi(ülke);
        }

        public void UlkeGüncelle(Ulke ülke)
        {
            if (ülke == null)
                throw new ArgumentNullException("ülke");

            _ülkeDepo.Güncelle(ülke);
            _olayYayınlayıcı.OlayGüncellendi(ülke);
        }

        public void UlkeSil(Ulke ülke)
        {
            if (ülke == null)
                throw new ArgumentNullException("ülke");

            _ülkeDepo.Sil(ülke);
            _olayYayınlayıcı.OlaySilindi(ülke);
        }

        public IList<Ulke> TümUlkeleriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            var query = _ülkeDepo.Tablo;
            return  query.ToList();
        }
        #endregion
        #region Sehir
        public Sehir SehirAlId(int sehirId)
        {
            if (sehirId == 0)
                return null;
            return _sehirDepo.AlId(sehirId);
        }

        public void SehirEkle(Sehir sehir)
        {
            if (sehir == null)
                throw new ArgumentNullException("sehir");

            _sehirDepo.Ekle(sehir);
            _olayYayınlayıcı.OlayEklendi(sehir);
        }

        public void SehirGüncelle(Sehir sehir)
        {
            if (sehir == null)
                throw new ArgumentNullException("sehir");

            _sehirDepo.Güncelle(sehir);
            _olayYayınlayıcı.OlayGüncellendi(sehir);
        }

        public void SehirSil(Sehir sehir)
        {
            if (sehir == null)
                throw new ArgumentNullException("sehir");

            _sehirDepo.Sil(sehir);
            _olayYayınlayıcı.OlaySilindi(sehir);
        }

        public IList<Sehir> TümSehirleriAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            var query = _sehirDepo.Tablo;
            return  query.ToList();
        }
        public IList<Sehir> SehirlerAlUlkeId(int ulkeId, bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            var ulkeIdSorgu = from s in _ülkeDepo.Tablo
                              select s.Id;
            var query = from fg in _sehirDepo.Tablo
                        where (fg.UlkeId == ulkeId)
                        select fg;
            return query.ToList();
        }
        #endregion
        #region İlçe
        public Ilce IlceAlId(int ilceId)
        {
            if (ilceId == 0)
                return null;

            return  _ilceDepo.AlId(ilceId);
        }

        public void IlceEkle(Ilce ilce)
        {
            if (ilce == null)
                throw new ArgumentNullException("ilce");

            _ilceDepo.Ekle(ilce);
            _olayYayınlayıcı.OlayEklendi(ilce);
        }

        public void IlceGüncelle(Ilce ilce)
        {
            if (ilce == null)
                throw new ArgumentNullException("ilce");

            _ilceDepo.Güncelle(ilce);
            _olayYayınlayıcı.OlayGüncellendi(ilce);
        }

        public void IlceSil(Ilce ilce)
        {
            if (ilce == null)
                throw new ArgumentNullException("ilce");

            _ilceDepo.Sil(ilce);
            _olayYayınlayıcı.OlaySilindi(ilce);
        }

        public IList<Ilce> TümIlceAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            var query = _ilceDepo.Tablo;
            return  query.ToList();
        }
        public IList<Ilce> IlcelerAlSehirId(int sehirid, bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            var sehirIdSorgu = from s in _sehirDepo.Tablo
                               select s.Id;
            var query = from fg in _ilceDepo.Tablo
                        where (fg.SehirId == sehirid)
                        select fg;
            return query.ToList();
        }
        
        #endregion
    }
}
