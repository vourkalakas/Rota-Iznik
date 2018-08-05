using System.Collections.Generic;
using Core.Domain.Kullanıcılar;

namespace Services.Cms
{
    public partial interface IWidgetServisi
    {
        IList<IWidgetEklenti> AktifWidgetleriYükle(Kullanıcı kullanıcı = null, int siteId = 0);
        IList<IWidgetEklenti> BölgedenAktifWidgetleriYükle(string widgetZone, Kullanıcı kullanıcı = null, int siteId = 0);
        IWidgetEklenti WidgetleriYükleSistemAdı(string sistemAdı);
        IList<IWidgetEklenti> TümWidgetleriYükle(Kullanıcı kullanıcı = null, int siteId = 0);
    }
}