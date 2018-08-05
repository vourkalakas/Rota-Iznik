using System;
using System.Linq;
using Core.Altyapı;
using Core.Eklentiler;
using Services.Logging;

namespace Services.Olaylar
{
    public class OlayYayınlayıcı : IOlayYayınlayıcı
    {
        private readonly IAbonelikServisi _abonelikServisi;
        public OlayYayınlayıcı(IAbonelikServisi abonelikServisi)
        {
            _abonelikServisi = abonelikServisi;
        }
        protected virtual void MüşteriİçinYayınla<T>(IMüşteri<T> x, T olayMesajı)
        {
            //Yüklenmemiş eklentileri görmezden gel
            var eklenti = EklentiBul(x.GetType());
            if (eklenti != null && !eklenti.Kuruldu)
                return;

            try
            {
                x.Olay(olayMesajı);
            }
            catch (Exception exc)
            {
                //günlük hatası
                var logger = EngineContext.Current.Resolve<ILogger>();
                try
                {
                    logger.Hata(exc.Message, exc);
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
        }
        protected virtual EklentiTanımlayıcı EklentiBul(Type sağlayıcıTipi)
        {
            if (sağlayıcıTipi == null)
                throw new ArgumentNullException("sağlayıcıTipi");

            if (EklentiYönetici.ReferenslıEklentiler == null)
                return null;

            foreach (var eklenti in EklentiYönetici.ReferenslıEklentiler)
            {
                if (eklenti.ReferanslıAssembly == null)
                    continue;

                if (eklenti.ReferanslıAssembly.FullName == sağlayıcıTipi.Assembly.FullName)
                    return eklenti;
            }

            return null;
        }
        public virtual void Yayınla<T>(T olayMesajı)
        {
            var abonelikler = _abonelikServisi.AbonelikleriAl<T>();
            abonelikler.ToList().ForEach(x => MüşteriİçinYayınla(x, olayMesajı));
        }

    }
}
