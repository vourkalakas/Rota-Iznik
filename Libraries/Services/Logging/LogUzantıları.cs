using System;
using Core.Domain.Kullanıcılar;
using Core.Domain.Logging;

namespace Services.Logging
{
    public static class LogUzantıları
    {
        public static void Debug(this ILogger logger, string mesaj, Exception hata = null, Kullanıcı kullanıcı = null)
        {
            LogFiltrele(logger, LogSeviyesi.Debug, mesaj, hata, kullanıcı);
        }
        public static void Bilgi(this ILogger logger, string mesaj, Exception hata = null, Kullanıcı kullanıcı = null)
        {
            LogFiltrele(logger, LogSeviyesi.Bilgi, mesaj, hata, kullanıcı);
        }
        public static void Uyarı(this ILogger logger, string mesaj, Exception hata = null, Kullanıcı kullanıcı = null)
        {
            LogFiltrele(logger, LogSeviyesi.Uyarı, mesaj, hata, kullanıcı);
        }
        public static void Hata(this ILogger logger, string mesaj, Exception hata = null, Kullanıcı kullanıcı = null)
        {
            LogFiltrele(logger, LogSeviyesi.Hata, mesaj, hata, kullanıcı);
        }
        public static void Ölümcül(this ILogger logger, string mesaj, Exception hata = null, Kullanıcı kullanıcı = null)
        {
            LogFiltrele(logger, LogSeviyesi.Ölümcül, mesaj, hata, kullanıcı);
        }

        private static void LogFiltrele(ILogger logger, LogSeviyesi seviye, string mesaj, Exception hata = null, Kullanıcı kullanıcı = null)
        {
            //don't log thread abort hata
            if (hata is System.Threading.ThreadAbortException)
                return;

            if (logger.Etkin(seviye))
            {
                string tamMesaj = hata == null ? string.Empty : hata.ToString();
                logger.LogEkle(seviye, mesaj, tamMesaj, kullanıcı);
            }
        }
    }
}
