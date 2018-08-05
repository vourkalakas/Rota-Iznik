using Core.Eklentiler;

namespace Services.KimlikDoğrulama.Harici
{
    public partial interface IHariciYetkilendirmeMetodu : IEklenti
    {
        void PublicViewBileşeniAl(out string viewComponentName);
    }
}
