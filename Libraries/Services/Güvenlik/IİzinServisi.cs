using System.Collections.Generic;
using Core.Domain.Kullanıcılar;
using Core.Domain.Güvenlik;

namespace Services.Güvenlik
{
    public partial interface IİzinServisi
    {
        void İzinKaydınıSil(İzinKaydı izin);
        İzinKaydı İzinKaydıAlId(int izinId);
        İzinKaydı İzinKaydıAlSistemAdı(string sistemAdı);
        IList<İzinKaydı> TümİzinKayıtlarınıAl();
        void İzinKaydıEkle(İzinKaydı izin);
        void İzinKaydıGüncelle(İzinKaydı izin);
        void İzinleriKur(IİzinSağlayıcı permissionProvider);
        void İzinleriKaldır(IİzinSağlayıcı permissionProvider);
        bool YetkiVer(İzinKaydı izin);
        bool YetkiVer(İzinKaydı izin, Kullanıcı kullanıcı);
        bool YetkiVer(string izinKaydıSistemAdı);
        bool YetkiVer(string izinKaydıSistemAdı, Kullanıcı kullanıcı);
    }
}
