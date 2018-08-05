using Core.Domain.Klasör;
using System.Collections.Generic;

namespace Services.Klasör
{
    public partial interface IÜlkeServisi
    {
        void ÜlkeSil(Ülke ülke);
        IList<Ülke> TümÜlkeleriAl(bool gizliOlanıGöster = false);
        Ülke ÜlkeAlId(int ülkeId);
        IList<Ülke> ÜlkeAlIdler(int[] ülkeIdleri);
        Ülke ÜlkeAlİkiHarfIsoKodu(string ikiHarfIsoKodu);
        Ülke ÜlkeAlÜçHarfIsoKodu(string üçHarfIsoKodu);
        void ÜlkeEkle(Ülke ülke);
        void ÜlkeGüncelle(Ülke ülke);
    }
}

