using Core;
using Core.Domain.Mesajlar;
using System;

namespace Services.Mesajlar
{
    public partial interface IBültenAbonelikServisi
    {
        void BültenAboneliğiEkle(BültenAboneliği bültenAboneliği, bool abonelikOlayıYayınla = true);
        void BültenAboneliğiGüncelle(BültenAboneliği bültenAboneliği, bool abonelikOlayıYayınla = true);
        void BültenAboneliğiSil(BültenAboneliği bültenAboneliği, bool abonelikOlayıYayınla = true);
        BültenAboneliği BültenAboneliğiAlId(int bültenAboneliğiId);
        BültenAboneliği BültenAboneliğiAlGuid(Guid bültenAboneliğiGuid);
        BültenAboneliği BültenAboneliğiAlEmailVeSiteId(string email, int siteId);
        ISayfalıListe<BültenAboneliği> TümBültenAbonelikleriniAl(string email = null,
            DateTime? şuTarihden = null, DateTime? şuTarihe = null,
            int siteId = 0, bool? aktif = null, int kullanıcıRolId = 0,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
    }
}

