using System.Collections.Generic;
using Core.Yapılandırma;

namespace Core.Domain.Cms
{
    public class WidgetAyarları : IAyarlar
    {
        public WidgetAyarları()
        {
            AktifWidgetSistemAdları = new List<string>();
        }
        public List<string> AktifWidgetSistemAdları = new List<string>() { "Widgets.NivoSlider" };
    }
}
