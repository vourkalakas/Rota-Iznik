using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Data;
using Core.Domain.EkTanımlamalar;
using Core.Önbellek;
using Services.Olaylar;

namespace Services.EkTanımlamalar
{
    public class TedarikciSektorServisi : ITedarikciSektorServisi
    {
        private const string TEDARIKCISEKTOR_ALL_KEY = "tedarikcisektor.all-{0}-{1}";
        private const string TEDARIKCISEKTOR_BY_ID_KEY = "tedarikcisektor.id-{0}";
        private const string TEDARIKCISEKTOR_PATTERN_KEY = "tedarikcisektor.";
        private readonly IDepo<TedarikciSektor> _tedarikciDepo;
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        public TedarikciSektorServisi(IDepo<TedarikciSektor> tedarikciDepo,
            IWorkContext workContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IÖnbellekYönetici önbellekYönetici)
        {
            this._tedarikciDepo = tedarikciDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public TedarikciSektor TedarikciSektorAlId(int tedarikciId)
        {
            if (tedarikciId == 0)
                return null;

            string key = string.Format(TEDARIKCISEKTOR_BY_ID_KEY, tedarikciId);
            return _önbellekYönetici.Al(key, () => _tedarikciDepo.AlId(tedarikciId));
        }

        public void TedarikciSektorEkle(TedarikciSektor tedarikci)
        {
            if (tedarikci == null)
                throw new ArgumentNullException("tedarikci");

            _tedarikciDepo.Ekle(tedarikci);
            _önbellekYönetici.KalıpİleSil(TEDARIKCISEKTOR_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(tedarikci);
        }

        public void TedarikciSektorGüncelle(TedarikciSektor tedarikci)
        {
            if (tedarikci == null)
                throw new ArgumentNullException("tedarikci");

            _tedarikciDepo.Güncelle(tedarikci);
            _önbellekYönetici.KalıpİleSil(TEDARIKCISEKTOR_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(tedarikci);
        }

        public void TedarikciSektorSil(TedarikciSektor tedarikci)
        {
            if (tedarikci == null)
                throw new ArgumentNullException("tedarikci");

            _tedarikciDepo.Sil(tedarikci);
            _önbellekYönetici.KalıpİleSil(TEDARIKCISEKTOR_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(tedarikci);
        }

        public IList<TedarikciSektor> TümTedarikciSektorleriAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TEDARIKCISEKTOR_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _tedarikciDepo.Tablo;
                return query.ToList();
            });
        }
    }
}
