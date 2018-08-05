using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Cms;
using Core.Domain.Kullanıcılar;
using Core.Eklentiler;

namespace Services.Cms
{
    public partial class WidgetServisi : IWidgetServisi
    {
        #region Fields

        private readonly IEklentiBulucu _eklentiBulucu;
        private readonly WidgetAyarları _widgetAyarları;

        #endregion

        #region Ctor
        public WidgetServisi(IEklentiBulucu eklentiBulucu,
            WidgetAyarları widgetAyarları)
        {
            this._eklentiBulucu = eklentiBulucu;
            this._widgetAyarları = widgetAyarları;
        }

        #endregion

        #region Methods
        public virtual IList<IWidgetEklenti> AktifWidgetleriYükle(Kullanıcı kullanıcı = null, int siteId = 0)
        {
            return TümWidgetleriYükle(kullanıcı, siteId).ToList();
        }
        public virtual IList<IWidgetEklenti> BölgedenAktifWidgetleriYükle(string widgetZone, Kullanıcı kullanıcı = null, int siteId = 0)
        {
            if (String.IsNullOrWhiteSpace(widgetZone))
                return new List<IWidgetEklenti>();

            return AktifWidgetleriYükle(kullanıcı, siteId)
                .Where(x => x.WidgetBölgeleriniAl().Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }
        public virtual IWidgetEklenti WidgetleriYükleSistemAdı(string sistemAdı)
        {
            var tanımlayıcı = _eklentiBulucu.EklentiTanımlayıcıAlSistemAdı<IWidgetEklenti>(sistemAdı);
            if (tanımlayıcı != null)
                return tanımlayıcı.Instance<IWidgetEklenti>();

            return null;
        }
        public virtual IList<IWidgetEklenti> TümWidgetleriYükle(Kullanıcı kullanıcı = null, int siteId = 0)
        {
            return _eklentiBulucu.EklentileriAl<IWidgetEklenti>(kullanıcı: kullanıcı, siteId: siteId).ToList();
        }

        #endregion
    }
}
