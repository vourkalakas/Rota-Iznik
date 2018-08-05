using Core.Domain.Teklif;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Services.Klasör
{
    public partial interface IPdfServisi
    {
        string TeklifPdfOlustur(Teklif teklif);
        void TeklifPdfOlustur(Stream stream, IList<Teklif> teklifler);
        string TeklifRaporOlustur(Teklif teklif);
        void TeklifRaporOlustur(Stream stream, IList<Teklif> teklifler);
    }
}
