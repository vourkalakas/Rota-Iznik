using Microsoft.AspNetCore.Authentication;

namespace Services.KimlikDoğrulama.Harici
{
    public interface IHariciYetkilendirmeRegistrar
    {
        void Yapılandır(AuthenticationBuilder builder);
    }
}
