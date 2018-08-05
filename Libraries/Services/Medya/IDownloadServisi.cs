using Core.Domain.Medya;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Medya
{
    public partial interface IDownloadServisi
    {
        Download DownloadAlId(int downloadId);

        Download DownloadAlGuid(Guid downloadGuid);

        void DownloadSil(Download download);

        void DownloadEkle(Download download);

        void DownloadGüncelle(Download download);

    }
}
