
using Core;
using Core.Domain.Mesajlar;
using Services.Olaylar;
using System.Collections.Generic;

namespace Services.Mesajlar
{
    public static class OlayYayınlayıcıUzantıları
    {
        public static void BültenAboneliğiYayınla(this IOlayYayınlayıcı olayYayınlayıcı,BültenAboneliği bültenAboneliği)
        {
            olayYayınlayıcı.Yayınla(new EmailAboneOlduOlayı(bültenAboneliği));
        }
        public static void BültenAboneliğindenAyrıldıYayınla(this IOlayYayınlayıcı olayYayınlayıcı, BültenAboneliği bültenAboneliği)
        {
            olayYayınlayıcı.Yayınla(new EmailAboneliktenAyrıldıOlayı(bültenAboneliği));
        }
        public static void VarlıkTokenEklendi<T, U>(this IOlayYayınlayıcı olayYayınlayıcı, T varlık, IList<U> token) where T : TemelVarlık
        {
            olayYayınlayıcı.Yayınla(new VarlıkTokenEklendiOlayı<T, U>(varlık, token));
        }

        public static void MesajTokenEklendi<U>(this IOlayYayınlayıcı olayYayınlayıcı, MesajTeması mesaj, IList<U> token)
        {
            olayYayınlayıcı.Yayınla(new MesajTokenEklendiOlayı<U>(mesaj, token));
        }
    }
}
