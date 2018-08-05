using Core;
using Core.Domain.Mesajlar;
using System;
using System.Collections.Generic;

namespace Services.Mesajlar
{
    public partial interface IMesajlarServisi
    {
        void MesajlarSil(Mesaj mesajlar);
        Mesaj MesajlarAlId(int mesajlarId);
        IList<Mesaj> MesajlarAlKullanıcıId(int kullanıcıId);
        IList<Mesaj> TümMesajlarAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        ISayfalıListe<Mesaj> MesajAra(string baslik, string msj,
           DateTime? tarihi, bool enYeniler, int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        void MesajlarEkle(Mesaj mesajlar);
        void MesajlarGüncelle(Mesaj mesajlar);
    }
}
