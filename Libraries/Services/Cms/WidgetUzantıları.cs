using Core.Domain.Cms;
using System;

namespace Services.Cms
{
    public static class WidgetUzantıları
    {
        public static bool IsWidgetActive(this IWidgetEklenti widget,
            WidgetAyarları widgetSettings)
        {
            if (widget == null)
                throw new ArgumentNullException(nameof(widget));

            if (widgetSettings == null)
                throw new ArgumentNullException(nameof(widgetSettings));

            if (widgetSettings.AktifWidgetSistemAdları == null)
                return false;
            foreach (var activeMethodSystemName in widgetSettings.AktifWidgetSistemAdları)
                if (widget.EklentiTanımlayıcı.SistemAdı.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
    }
}
