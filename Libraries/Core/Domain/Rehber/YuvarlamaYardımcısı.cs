using System;
using Core;
using Core.Altyapı;

namespace Core.Domain.Rehber
{
    public static class YuvarlamaYardımcısı
    {
        public static decimal Yuvarla(this decimal değer, YuvarlamaTipi yuvarlamaTipi)
        {
            var rez = Math.Round(değer, 2);
            decimal t;

            switch (yuvarlamaTipi)
            {
                //rounding with 0.05 or 5 intervals
                case YuvarlamaTipi.YuvarlaYukarı005:
                case YuvarlamaTipi.YuvarlaAşağı005:
                    t = (rez - Math.Truncate(rez)) * 10;
                    t = (t - Math.Truncate(t)) * 10;

                    if (yuvarlamaTipi == YuvarlamaTipi.YuvarlaAşağı005)
                        t = t >= 5 ? 5 - t : t * -1;
                    else
                        t = t >= 5 ? 10 - t : 5 - t;

                    rez += t / 100;
                    break;
                //rounding with 0.10 intervals
                case YuvarlamaTipi.YuvarlaYukarı01:
                case YuvarlamaTipi.YuvarlaAşağı01:
                    t = (rez - Math.Truncate(rez)) * 10;
                    t = (t - Math.Truncate(t)) * 10;

                    if (yuvarlamaTipi == YuvarlamaTipi.YuvarlaAşağı01 && t == 5)
                        t = -5;
                    else
                        t = t < 5 ? t * -1 : 10 - t;

                    rez += t / 100;
                    break;
                //rounding with 0.50 intervals
                case YuvarlamaTipi.Yuvarla05:
                    t = (rez - Math.Truncate(rez)) * 100;
                    t = t < 25 ? t * -1 : t < 50 || t < 75 ? 50 - t : 100 - t;

                    rez += t / 100;
                    break;
                //rounding with 1.00 intervals
                case YuvarlamaTipi.Yuvarla1:
                case YuvarlamaTipi.YuvarlaYukarı1:
                    t = (rez - Math.Truncate(rez)) * 100;

                    if (yuvarlamaTipi == YuvarlamaTipi.YuvarlaYukarı1 && t > 0)
                        rez = Math.Truncate(rez) + 1;
                    else
                        rez = t < 50 ? Math.Truncate(rez) : Math.Truncate(rez) + 1;

                    break;
                case YuvarlamaTipi.Yuvarla001:
                default:
                    break;
            }

            return rez;
        }
    }
}
