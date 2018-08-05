using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Data;
using Core.Domain.Test;
using Core.Önbellek;
using Services.Olaylar;

namespace Services.Testler
{
    public class TestServisi : ITestServisi
    {
        private const string TEST_ALL_KEY = "test.all-{0}-{1}";
        private const string TEST_BY_ID_KEY = "test.id-{0}";
        private const string TEST_PATTERN_KEY = "test.";
        private readonly IDepo<Test> _testDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public TestServisi(IDepo<Test> testDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._testDepo = testDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Test TestAlId(int testId)
        {
            if (testId == 0)
                return null;

            string key = string.Format(TEST_BY_ID_KEY, testId);
            return _önbellekYönetici.Al(key, () => _testDepo.AlId(testId));
        }

        public void TestEkle(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            _testDepo.Ekle(test);
            _önbellekYönetici.KalıpİleSil(TEST_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(test);
        }

        public void TestGüncelle(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            _testDepo.Güncelle(test);
            _önbellekYönetici.KalıpİleSil(TEST_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(test);
        }

        public void TestSil(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            _testDepo.Sil(test);
            _önbellekYönetici.KalıpİleSil(TEST_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(test);
        }

        public IList<Test> TümTestAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TEST_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _testDepo.Tablo;
                query = query.OrderByDescending(x => x.Id);
                return query.ToList();
            });
        }
        public ISayfalıListe<Test> TestAra(DateTime? tarihinden = null,
            DateTime? tarihine = null, int hazırlayanId = 0, string adı = "",
            string Konumu = "", string açıklama = "", string durumu = "", 
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue)
        {
            var sorgu = _testDepo.Tablo;

            var testler = new SayfalıListe<Test>(sorgu, sayfaIndeksi, sayfaBüyüklüğü);
            return testler;
        }
    }
}
