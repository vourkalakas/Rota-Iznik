using System.Collections.Generic;

namespace Services.Olaylar
{
    public interface IAbonelikServisi
    {
        IList<IMüşteri<T>> AbonelikleriAl<T>();
    }
}
