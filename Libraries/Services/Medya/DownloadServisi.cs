using Core.Data;
using Core.Domain.Medya;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Medya
{
    public partial class DownloadServisi:IDownloadServisi
    {
        #region Fields

        private readonly IDepo<Download> _downloadDepo;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;

        #endregion

        #region Ctor

        public DownloadServisi(IDepo<Download> downloadDepo,
            IOlayYayınlayıcı olayYayınlayıcı)
        {
            _downloadDepo = downloadDepo;
            _olayYayınlayıcı = olayYayınlayıcı;
        }

        #endregion

        #region Methods

        public virtual Download DownloadAlId(int downloadId)
        {
            if (downloadId == 0)
                return null;

            return _downloadDepo.AlId(downloadId);
        }

        public virtual Download DownloadAlGuid(Guid downloadGuid)
        {
            if (downloadGuid == Guid.Empty)
                return null;

            var sorgu = from o in _downloadDepo.Tablo
                        where o.DownloadGuid == downloadGuid
                        select o;

            return sorgu.FirstOrDefault();
        }

        public virtual void DownloadSil(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadDepo.Sil(download);

            _olayYayınlayıcı.OlayEklendi(download);
        }

        public virtual void DownloadEkle(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadDepo.Ekle(download);

            _olayYayınlayıcı.OlayEklendi(download);
        }

        public virtual void DownloadGüncelle(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadDepo.Güncelle(download);

            _olayYayınlayıcı.OlayEklendi(download);
        }

        #endregion
    }
}
