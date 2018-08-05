using Core.Domain.Güvenlik;
using System.Collections.Generic;

namespace Services.Güvenlik
{
    public interface IİzinSağlayıcı
    {
        IEnumerable<İzinKaydı> İzinleriAl();
        IEnumerable<VarsayılanİzinKaydı> VarsayılanİzinleriAl();
    }
}