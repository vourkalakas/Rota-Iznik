using System.Collections.Generic;
using Core.Eklentiler;


namespace Services.Cms
{
    public partial interface IWidgetEklenti : IEklenti
    {
        IList<string> WidgetBölgeleriniAl();
        void PublicViewBileşeniAl(string widgetZone, out string viewComponentName);
    }
}
