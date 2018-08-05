using Core.Domain.Doviz;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Services.DovizServisi
{
    public class DovizServisi:IDovizServisi
    {
        public DovizServisi()
        {
            
        }

        public List<Doviz> DovizKurları()
        {
            XmlTextReader xtr = new XmlTextReader("http://www.tcmb.gov.tr/kurlar/today.xml");
            XmlDocument document = new XmlDocument();
            document.Load(xtr);
            XmlNode tarih = document.SelectSingleNode("/Tarih_Date/@Tarih");
            XmlNodeList mylist = document.SelectNodes("/Tarih_Date/Currency");
            XmlNodeList adi = document.SelectNodes("/Tarih_Date/Currency/Isim");
            XmlNodeList kod = document.SelectNodes("/Tarih_Date/Currency/@Kod");
            XmlNodeList doviz_alis = document.SelectNodes("/Tarih_Date/Currency/ForexBuying");
            XmlNodeList doviz_satis = document.SelectNodes("/Tarih_Date/Currency/ForexSelling");
            XmlNodeList efektif_alis = document.SelectNodes("/Tarih_Date/Currency/BanknoteBuying");
            XmlNodeList efektif_satis = document.SelectNodes("/Tarih_Date/Currency/BanknoteSelling");
            //19 satır eklenmesini sağlıyor. çünkü xml dökümanında 19. node dan sonra güncel kur bilgileri değil Euro dönüşüm kurları var
            int x = 10;
            List<Doviz> d = new List<Doviz>();
            for (int i = 0; i < x; i++)
            {
                var Doviz = new Doviz
                {
                    Adı = adi.Item(i).InnerText.ToString(),
                    Kodu = kod.Item(i).InnerText.ToString(),
                    DovizAlış= Convert.ToDecimal(doviz_alis.Item(i).InnerText.Replace(".",",")),
                    DovizSatış= Convert.ToDecimal(doviz_satis.Item(i).InnerText.Replace(".", ",")),
                    EfektifAlış= Convert.ToDecimal(efektif_alis.Item(i).InnerText.Replace(".", ",")),
                    EfektifSatış =Convert.ToDecimal(efektif_satis.Item(i).InnerText.Replace(".", ",")),
                    Tarih = Convert.ToDateTime(tarih.InnerText)
                };
                d.Add(Doviz);
            }
            return d;
        }
        public decimal DolarKuruAl()
        {
            return DovizKurları()[0].EfektifSatış;
        }
        public decimal EuroKuruAl()
        {
            return DovizKurları()[3].EfektifSatış;
        }
    }
}
