using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Core.Domain.Kullanıcılar;

namespace Services.KimlikDoğrulama.Harici
{
    public partial interface IHariciYetkilendirmeServisi
    {
        #region External authentication methods
        
        IList<IHariciYetkilendirmeMetodu> AktifHariciYetkilendirmeMetodlarıYükle(Kullanıcı customer = null, int siteId = 0);
        IHariciYetkilendirmeMetodu AktifHariciYetkilendirmeMetoduYükleSistemAdı(string sistemAdı);
        IList<IHariciYetkilendirmeMetodu> TümAktifHariciYetkilendirmeMetodlarıYükle(Kullanıcı customer = null, int siteId = 0);
        bool HariciYetkilendirmeMetoduErişilebilir(string sistemAdı);

        #endregion

        #region Authentication
        IActionResult Yetkilendir(HariciYetkilendirmeParametreleri parameters, string returnUrl = null);

        #endregion

        void HariciHesabıKullanıcıylaİlişkilendir(Kullanıcı kullanıcı, HariciYetkilendirmeParametreleri parametreler);
        Kullanıcı HariciYetkilendirmeParametresindenKullanıcıAl(HariciYetkilendirmeParametreleri parameters);
        void İlişkilendirmeyiKaldır(HariciYetkilendirmeParametreleri parameters);
        void HariciYetkilendirmeKaydınıSil(HariciKimlikDoğrulamaKaydı externalAuthenticationRecord);
    }
}