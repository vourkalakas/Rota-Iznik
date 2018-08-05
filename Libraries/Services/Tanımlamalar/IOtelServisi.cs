using Core.Domain.Tanımlamalar;
using System.Collections.Generic;

namespace Services.Tanımlamalar
{
    public partial interface IOtelServisi
    {
        void OtelSil(Otel otel);
        Otel OtelAlId(int otelId);
        IList<Otel> TümOtelAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void OtelEkle(Otel otel);
        void OtelGüncelle(Otel otel);
    }

}
