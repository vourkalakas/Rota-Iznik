using Core.Data;
using Core.Domain.Hint;
using Core.Önbellek;
using System;
using System.Linq;

namespace Services.Hint
{
    public partial class HintServisi:IHintServisi
    {
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "lsr.{0}";
        private readonly IÖnbellekYönetici _önbellekYöneticisi;
        private readonly IDepo<Hints> _lsrDepo;
        public HintServisi(IÖnbellekYönetici önbellekYöneticisi,
            IDepo<Hints> lsrDepo)
        {
            this._önbellekYöneticisi = önbellekYöneticisi;
            this._lsrDepo = lsrDepo;
        }
        public virtual string GetResource(string resourceKey)
        {
            string result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
                //gradual loading
                string key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, resourceKey);
                string lsr = _önbellekYöneticisi.Al(key, () =>
                {
                    var query = from l in _lsrDepo.Tablo
                                where l.KaynakAdı == resourceKey
                                select l.KaynakDegeri;
                    return query.FirstOrDefault();
                });

                if (lsr != null)
                    result = lsr;
            
            if (String.IsNullOrEmpty(result))
            {
                result = "";
            }
            return result;
        }
    }
}
