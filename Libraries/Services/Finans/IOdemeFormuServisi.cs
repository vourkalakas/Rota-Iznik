using Core;
using Core.Domain.Finans;
using Core.Domain.Kullanıcılar;
using System;
using System.Collections.Generic;
namespace Services.Finans
{
    public partial interface IOdemeFormuServisi
    {
        void OdemeFormuSil(OdemeFormu odemeFormu);
        OdemeFormu OdemeFormuAlId(int odemeFormuId);
        IList<OdemeFormu> TümOdemeFormuAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        ISayfalıListe<OdemeFormu> OdemeFormuAra(string firma,
           int kongreGunu, int kongreAyı, int odemeGunu, int odemeAyı, string aciklama, string alisFatura, string satisFatura,
           bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        void OdemeFormuEkle(OdemeFormu odemeFormu);
        void OdemeFormuGüncelle(OdemeFormu odemeFormu);
    }

}
