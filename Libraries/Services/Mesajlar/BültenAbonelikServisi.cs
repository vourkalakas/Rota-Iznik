using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Mesajlar;
using Core.Data;
using Services.Olaylar;
using Data;
using Core.Domain.Kullanıcılar;
using Services.Kullanıcılar;

namespace Services.Mesajlar
{
    public partial class BültenAbonelikServisi : IBültenAbonelikServisi
    {
        private readonly IDepo<BültenAboneliği> _abonelikDepo;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IDbContext _context;
        private readonly IDepo<Kullanıcı> _kullanıcıDepo;
        private readonly IKullanıcıServisi _kullanıcıServisi;

        public BültenAbonelikServisi(
            IDepo<BültenAboneliği> abonelikDepo,
            IOlayYayınlayıcı olayYayınlayıcı,
            IDbContext context,
            IDepo<Kullanıcı> kullanıcıDepo,
            IKullanıcıServisi kullanıcıServisi
            )
        {
            this._abonelikDepo = abonelikDepo;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._context = context;
            this._kullanıcıDepo = kullanıcıDepo;
            this._kullanıcıServisi = kullanıcıServisi;
        }
        private void AbonelikOlayıYayınla(BültenAboneliği bültenAboneliği, bool abone, bool abonelikOlayıYayınla)
        {
            if (abonelikOlayıYayınla)
            {
                if (abone)
                {
                    _olayYayınlayıcı.BültenAboneliğiYayınla(bültenAboneliği);
                }
                else
                {
                    _olayYayınlayıcı.BültenAboneliğindenAyrıldıYayınla(bültenAboneliği);
                }
            }
        }
        public BültenAboneliği BültenAboneliğiAlEmailVeSiteId(string email, int siteId)
        {
            if (!GenelYardımcı.GeçerliMail(email))
                return null;
            email = email.Trim();
            var bültenAbonelikleri = from nls in _abonelikDepo.Tablo
                                          where nls.Email == email && nls.SiteId == siteId
                                          orderby nls.Id
                                          select nls;

            return bültenAbonelikleri.FirstOrDefault();
        }

        public BültenAboneliği BültenAboneliğiAlGuid(Guid bültenAboneliğiGuid)
        {
            if (bültenAboneliğiGuid == Guid.Empty) return null;
            var bültenAbonelikleri = from nls in _abonelikDepo.Tablo
                                     where nls.BültenAboneliğiGuid == bültenAboneliğiGuid
                                     orderby nls.Id
                                     select nls;

            return bültenAbonelikleri.FirstOrDefault();
        }

        public BültenAboneliği BültenAboneliğiAlId(int bültenAboneliğiId)
        {
            if (bültenAboneliğiId == 0) return null;
            return _abonelikDepo.AlId(bültenAboneliğiId);
        }

        public void BültenAboneliğiEkle(BültenAboneliği bültenAboneliği, bool abonelikOlayıYayınla = true)
        {
            if (bültenAboneliği == null)
            {
                throw new ArgumentNullException("bültenAboneliği");
            }
            bültenAboneliği.Email = GenelYardımcı.AboneMailAdresindenEminOl(bültenAboneliği.Email);
            _abonelikDepo.Ekle(bültenAboneliği);
            if (bültenAboneliği.Aktif)
            {
                AbonelikOlayıYayınla(bültenAboneliği, true, abonelikOlayıYayınla);
            }
            _olayYayınlayıcı.OlayEklendi(bültenAboneliği);
        }

        public void BültenAboneliğiGüncelle(BültenAboneliği bültenAboneliği, bool abonelikOlayıYayınla = true)
        {
            if (bültenAboneliği == null)
            {
                throw new ArgumentNullException("bültenAboneliği");
            }
            bültenAboneliği.Email = GenelYardımcı.AboneMailAdresindenEminOl(bültenAboneliği.Email);
            var orijinalAbonelik = _context.LoadOriginalCopy(bültenAboneliği);
            _abonelikDepo.Güncelle(bültenAboneliği);
            if ((!orijinalAbonelik.Aktif && bültenAboneliği.Aktif) && (bültenAboneliği.Email != orijinalAbonelik.Email))
            {
                AbonelikOlayıYayınla(bültenAboneliği, true, abonelikOlayıYayınla);
            }
            if ((orijinalAbonelik.Aktif && bültenAboneliği.Aktif) || (bültenAboneliği.Aktif && (bültenAboneliği.Email != orijinalAbonelik.Email)))
            {
                AbonelikOlayıYayınla(orijinalAbonelik, false, abonelikOlayıYayınla);
            }
            if ((orijinalAbonelik.Aktif && !bültenAboneliği.Aktif))
            {
                AbonelikOlayıYayınla(orijinalAbonelik, false, abonelikOlayıYayınla);
            }
            _olayYayınlayıcı.OlayGüncellendi(bültenAboneliği);
        }

        public void BültenAboneliğiSil(BültenAboneliği bültenAboneliği, bool abonelikOlayıYayınla = true)
        {
            if (bültenAboneliği == null)
            {
                throw new ArgumentNullException("bültenAboneliği");
            }
            _abonelikDepo.Sil(bültenAboneliği);
            AbonelikOlayıYayınla(bültenAboneliği, false, abonelikOlayıYayınla);
            _olayYayınlayıcı.OlaySilindi(bültenAboneliği);
        }

        public ISayfalıListe<BültenAboneliği> TümBültenAbonelikleriniAl(string email = null, DateTime? şuTarihden = default(DateTime?), DateTime? şuTarihe = default(DateTime?), int siteId = 0, bool? aktif = default(bool?), int kullanıcıRolId = 0, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            if (kullanıcıRolId == 0)
            {
                var sorgu = _abonelikDepo.Tablo;
                if (!String.IsNullOrEmpty(email))
                    sorgu = sorgu.Where(nls => nls.Email.Contains(email));
                if (şuTarihden.HasValue)
                    sorgu = sorgu.Where(nls => nls.OluşturulmaTarihi >= şuTarihden.Value);
                if (şuTarihe.HasValue)
                    sorgu = sorgu.Where(nls => nls.OluşturulmaTarihi <= şuTarihe.Value);
                if (siteId > 0)
                    sorgu = sorgu.Where(nls => nls.SiteId == siteId);
                if (aktif.HasValue)
                    sorgu = sorgu.Where(nls => nls.Aktif == aktif.Value);
                sorgu = sorgu.OrderBy(nls => nls.Email);

                var abonelikler = new SayfalıListe<BültenAboneliği>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
                return abonelikler;
            }
            else
            {
                var ziyaretçiRolü = _kullanıcıServisi.KullanıcıRolüAlSistemAdı(SistemKullanıcıRolAdları.Ziyaretçi);
                if (ziyaretçiRolü == null)
                    throw new Hata("'Ziyaretçi' rolü yüklenemedi");

                if (ziyaretçiRolü.Id == kullanıcıRolId)
                {
                    var sorgu = _abonelikDepo.Tablo;
                    if (!String.IsNullOrEmpty(email))
                        sorgu = sorgu.Where(nls => nls.Email.Contains(email));
                    if (şuTarihden.HasValue)
                        sorgu = sorgu.Where(nls => nls.OluşturulmaTarihi >= şuTarihden.Value);
                    if (şuTarihe.HasValue)
                        sorgu = sorgu.Where(nls => nls.OluşturulmaTarihi <= şuTarihe.Value);
                    if (siteId > 0)
                        sorgu = sorgu.Where(nls => nls.SiteId == siteId);
                    if (aktif.HasValue)
                        sorgu = sorgu.Where(nls => nls.Aktif == aktif.Value);
                    sorgu = sorgu.Where(nls => !_kullanıcıDepo.Tablo.Any(c => c.Email == nls.Email));
                    sorgu = sorgu.OrderBy(nls => nls.Email);

                    var abonelikler = new SayfalıListe<BültenAboneliği>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
                    return abonelikler;
                }
                else
                {
                    var sorgu = _abonelikDepo.Tablo.Join(_kullanıcıDepo.Tablo,
                        nls => nls.Email,
                        c => c.Email,
                        (nls, c) => new
                        {
                            BültenAbonesi = nls,
                            Kullanıcı = c
                        });
                    sorgu = sorgu.Where(x => x.Kullanıcı.KullanıcıRolleri.Any(cr => cr.Id == kullanıcıRolId));
                    if (!String.IsNullOrEmpty(email))
                        sorgu = sorgu.Where(x => x.BültenAbonesi.Email.Contains(email));
                    if (şuTarihden.HasValue)
                        sorgu = sorgu.Where(x => x.BültenAbonesi.OluşturulmaTarihi >= şuTarihden.Value);
                    if (şuTarihe.HasValue)
                        sorgu = sorgu.Where(x => x.BültenAbonesi.OluşturulmaTarihi <= şuTarihe.Value);
                    if (siteId > 0)
                        sorgu = sorgu.Where(x => x.BültenAbonesi.SiteId == siteId);
                    if (aktif.HasValue)
                        sorgu = sorgu.Where(x => x.BültenAbonesi.Aktif == aktif.Value);
                    sorgu = sorgu.OrderBy(x => x.BültenAbonesi.Email);

                    var subscriptions = new SayfalıListe<BültenAboneliği>(sorgu.Select(x => x.BültenAbonesi), sayfaIndeksi, sayfaBüyüklüğü);
                    return subscriptions;
                }
            }
        }
    }
}
