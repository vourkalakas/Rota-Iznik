using System;
using System.Collections.Generic;
using Core;
using Core.Domain.Kullanıcılar;
using Core.Domain.Logging;

namespace Services.Logging
{
    public partial interface ILogger
    {
        bool Etkin(LogSeviyesi seviye);
        void LogSil(Log log);
        void LoglarıSil(IList<Log> logs);
        void LogTemizle();
        ISayfalıListe<Log> TümLoglarıAl(DateTime? ŞuTarihten = null, DateTime? ŞuTarihe = null,
            string mesaj = "", LogSeviyesi? logseviyesi = null,
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue);
        Log LogAlId(int logId);
        IList<Log> LoglarıAlId(int[] logIds);
        Log LogEkle(LogSeviyesi logseviyesi, string kısaMesaj, string tamMesaj = "", Kullanıcı kullanıcı = null);
    }
}
