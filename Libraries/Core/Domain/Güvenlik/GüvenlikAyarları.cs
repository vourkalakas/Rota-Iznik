using System.Collections.Generic;
using Core.Yapılandırma;

namespace Core.Domain.Güvenlik
{
    public class GüvenlikAyarları : IAyarlar
    {
        public bool TümSayfalarıSslİçinZorla { get; set; }
        public string ŞifrelemeAnahtarı { get; set; }
        public List<string> YöneticiAlanıİzinVerilenIPAdresleri { get; set; }
        public bool YöneticiAlanıiçinXsrfKorumasınıEtkinleştir { get; set; }
        public bool GenelAlaniçinXsrfKorumasınıEtkinleştir { get; set; }
        public bool HoneypotEtkin{ get; set; }
        public string HoneypotGirişAdı { get; set; }
        public string EklentiUzantılarıKaraListesi { get; set; }
    }
}
