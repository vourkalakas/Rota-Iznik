using Core;
using Core.Domain.Kullanıcılar;
using Core.Domain.Logging;
using System;
using System.Collections.Generic;

namespace Services.Logging
{
    public partial interface IKullanıcıİşlemServisi
    {
        void İşlemTipiEkle(İşlemTipi işlemTipi);
        void İşlemTipiGüncelle(İşlemTipi işlemTipi);
        void İşlemTipiSil(İşlemTipi işlemTipi);
        IList<İşlemTipi> TümİşlemTipleriAl();
        İşlemTipi İşlemTipiAlId(int işlemTipiId);
        İşlem İşlemEkle(string sistemAnahtarKelimleri, string yorum, params object[] yorumDeğerleri);
        İşlem İşlemEkle(Kullanıcı kullanıcı, string sistemAnahtarKelimleri, string yorum, params object[] yorumDeğerleri);
        void İşlemSil(İşlem işlem);
        ISayfalıListe<İşlem> TümİşlemleriAl(DateTime? şuTarihden = null,
            DateTime? şuTarihe = null, int? kullanıcıId = null, int işlemTipiId = 0,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, string ipAdresi = null);
        İşlem İşlemAlId(int işlemTipiId);
        void TümİşlemleriTemizle();
    }
}
