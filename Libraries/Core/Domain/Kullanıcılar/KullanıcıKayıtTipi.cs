using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Kullanıcılar
{
    public enum KullanıcıKayıtTipi
    {
        Standart = 1,
        EmailDoğrulaması = 2,
        YöneticiOnayı = 3,
        Engelli = 4,
    }
}
