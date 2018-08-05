using Core;
using Core.Domain.Test;
using System;
using System.Collections.Generic;

namespace Services.Testler
{
    public partial interface ITestServisi
    {
        void TestSil(Test Test);
        Test TestAlId(int TestId);
        IList<Test> TümTestAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void TestEkle(Test Test);
        void TestGüncelle(Test Test);
        ISayfalıListe<Test> TestAra(DateTime? tarihinden = null,
            DateTime? tarihine = null, int hazırlayanId = 0, string adı = "",
            string Konumu = "", string açıklama = "", string durumu = "",
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
    }
}
