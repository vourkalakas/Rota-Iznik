using System.Collections.Generic;
using Core;
using Core.Domain.Genel;

namespace Services.Genel
{
    public partial interface IGenelÖznitelikServisi
    {
        void ÖznitelikSil(GenelÖznitelik öznitelik);
        void ÖznitelikleriSil(IList<GenelÖznitelik> öznitelikler);
        GenelÖznitelik ÖznitelikAlId(int öznitelikId);
        void ÖznitelikEkle(GenelÖznitelik öznitelik);
        void ÖznitelikGüncelle(GenelÖznitelik öznitelik);
        IList<GenelÖznitelik> VarlıkİçinÖznitelikleriAl(int öznitelikId, string keyGroup);
        void ÖznitelikKaydet<TPropType>(TemelVarlık varlık, string key, TPropType value, int siteId = 0);
    }
}