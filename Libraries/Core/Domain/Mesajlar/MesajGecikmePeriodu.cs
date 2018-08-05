using System;

namespace Core.Domain.Mesajlar
{
    public enum MesajGecikmePeriodu
    {
        Saatler = 0,
        Günler = 1
    }
    public static class MesajGecikmePerioduUzantıları
    {
        public static int Saatler(this MesajGecikmePeriodu period, int değer)
        {
            switch (period)
            {
                case MesajGecikmePeriodu.Saatler:
                    return değer;
                case MesajGecikmePeriodu.Günler:
                    return değer * 24;
                default:
                    throw new ArgumentOutOfRangeException("MesajGecikmePeriodu");
            }
        }
    }
}