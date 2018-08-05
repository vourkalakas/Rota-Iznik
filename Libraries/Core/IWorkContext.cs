using Core.Domain.Kullanıcılar;
using Core.Domain.Localization;

namespace Core
{
    public interface IWorkContext
    {
        Kullanıcı MevcutKullanıcı { get; set; }
        Kullanıcı OrijinalKullanıcıyıTaklitEt { get; }
        Dil MevcutDil { get; set; }
        bool Yönetici { get; set; }
    }
}
