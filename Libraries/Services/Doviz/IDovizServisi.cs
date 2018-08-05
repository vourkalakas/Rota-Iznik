using Core.Domain.Doviz;
using System.Collections.Generic;

namespace Services.DovizServisi
{
    public partial interface IDovizServisi
    {
        List<Doviz> DovizKurları();
        decimal DolarKuruAl();
        decimal EuroKuruAl();
    }
}
