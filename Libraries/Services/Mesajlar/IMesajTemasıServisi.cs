using Core.Domain.Mesajlar;
using System.Collections.Generic;

namespace Services.Mesajlar
{
    public partial interface IMesajTemasıServisi
    {
        void MesajTemasıSil(MesajTeması mesajTeması);
        void MesajTemasıEkle(MesajTeması mesajTeması);
        void MesajTemasıGüncelle(MesajTeması mesajTeması);
        MesajTeması MesajTemasıAlId(int mesajTemasıId);
        MesajTeması MesajTemasıAlAdı(string mesajTemasıAdı, int siteId);
        IList<MesajTeması> TümMesajTeması(int siteId);
        MesajTeması MesajTemasıKopyası(MesajTeması mesajTeması);
    }
}
