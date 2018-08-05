using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Core;
using Core.Önbellek;
using Core.Domain.Kullanıcılar;
using Core.Altyapı;
using Services.Genel;
using Services.Kullanıcılar.Önbellek;
//using Services.Kullanıcılar.Önbellek;

namespace Services.Kullanıcılar
{
    public static class KullanıcıUzantıları
    {
        public static string TamAdAl(this Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");
            var adı = kullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.Adı);
            var soyadı = kullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.Soyadı);

            string tamAd = "";
            if (!String.IsNullOrWhiteSpace(adı) && !String.IsNullOrWhiteSpace(soyadı))
                tamAd = string.Format("{0} {1}", adı, soyadı);
            else
            {
                if (!String.IsNullOrWhiteSpace(adı))
                    tamAd = adı;

                if (!String.IsNullOrWhiteSpace(soyadı))
                    tamAd = soyadı;
            }
            return tamAd;
        }
        public static string KullanıcıAdıFormatı(this Kullanıcı kullanıcı, bool şeritÇokUzun = false, int maksUzunluk = 0)
        {
            if (kullanıcı == null)
                return string.Empty;

            if (kullanıcı.IsGuest())
            {
                //return EngineContext.Current.Resolve<ILocalizationService>().GetResource("Kullanıcı.Guest");
            }

            string sonuç = string.Empty;
            switch (EngineContext.Current.Resolve<KullanıcıAyarları>().KullanıcıAdıFormatı)
            {
                case Core.Domain.Kullanıcılar.KullanıcıAdıFormatı.EmailGöster:
                    sonuç = kullanıcı.Email;
                    break;
                case Core.Domain.Kullanıcılar.KullanıcıAdıFormatı.KullanıcıAdıGöster:
                    sonuç = kullanıcı.KullanıcıAdı;
                    break;
                case Core.Domain.Kullanıcılar.KullanıcıAdıFormatı.TamAdıGöster:
                    sonuç = kullanıcı.TamAdAl();
                    break;
                case Core.Domain.Kullanıcılar.KullanıcıAdıFormatı.SadeceAdıGöster:
                    sonuç = kullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.Adı);
                    break;
                default:
                    break;
            }

            if (şeritÇokUzun && maksUzunluk > 0)
            {
                sonuç = GenelYardımcı.MaksimumUzunlukKontrol(sonuç, maksUzunluk);
            }

            return sonuç;
        }
        public static string[] UygulananKupoKodlarınıAyrıştır(this Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var genelÖznitelikServisi = EngineContext.Current.Resolve<IGenelÖznitelikServisi>();
            var mevcutKuponKodları = kullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.İndirimKuponuKodu,
                genelÖznitelikServisi);

            var kuponKodları = new List<string>();
            if (String.IsNullOrEmpty(mevcutKuponKodları))
                return kuponKodları.ToArray();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(mevcutKuponKodları);

                var nodeList1 = xmlDoc.SelectNodes(@"//İndirimKuponları/KuponKodu");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["Kod"] != null)
                    {
                        string code = node1.Attributes["Kod"].InnerText.Trim();
                        kuponKodları.Add(code);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return kuponKodları.ToArray();
        }
        public static void İndirimKuponuKoduUygula(this Kullanıcı kullanıcı, string kuponKodu)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var genelÖznitelikServisi = EngineContext.Current.Resolve<IGenelÖznitelikServisi>();
            string sonuç = string.Empty;
            try
            {
                var mevcutKuponKodları = kullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.İndirimKuponuKodu,
                    genelÖznitelikServisi);

                kuponKodu = kuponKodu.Trim().ToLower();

                var xmlDoc = new XmlDocument();
                if (String.IsNullOrEmpty(mevcutKuponKodları))
                {
                    var element1 = xmlDoc.CreateElement("İndirimKuponuKodları");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(mevcutKuponKodları);
                }
                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//İndirimKuponları");

                XmlElement gcElement = null;
                //varolanı bul
                var nodeList1 = xmlDoc.SelectNodes(@"//İndirimKuponları/KuponKodu");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["Code"] != null)
                    {
                        string kuponKoduÖzniteliği = node1.Attributes["Code"].InnerText.Trim();
                        if (kuponKoduÖzniteliği.ToLower() == kuponKodu.ToLower())
                        {
                            gcElement = (XmlElement)node1;
                            break;
                        }
                    }
                }

                //yoksa yeni oluştur
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("KuponKodu");
                    gcElement.SetAttribute("Code", kuponKodu);
                    rootElement.AppendChild(gcElement);
                }

                sonuç = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            //yeni değer uygula
            genelÖznitelikServisi.ÖznitelikKaydet(kullanıcı, SistemKullanıcıÖznitelikAdları.İndirimKuponuKodu, sonuç);
        }
        public static void İndirimKuponuKoduSil(this Kullanıcı kullanıcı, string kuponKodu)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            //uygulanmış kupon kodlarını al
            var mevcutKuponKodları = kullanıcı.UygulananKupoKodlarınıAyrıştır();

            //sil
            var genelÖznitelikServisi = EngineContext.Current.Resolve<IGenelÖznitelikServisi>();
            genelÖznitelikServisi.ÖznitelikKaydet<string>(kullanıcı, SistemKullanıcıÖznitelikAdları.İndirimKuponuKodu, null);

            //kaldırılanlar haricindekileri tekrar kaydedin
            foreach (string mevcutKuponKodu in mevcutKuponKodları)
                if (!mevcutKuponKodu.Equals(kuponKodu, StringComparison.InvariantCultureIgnoreCase))
                    kullanıcı.İndirimKuponuKoduUygula(mevcutKuponKodu);
        }
        public static bool ŞifreKurtarmaSimgesiGeçerli(this Kullanıcı kullanıcı, string token)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var cPrt = kullanıcı.ÖznitelikAl<string>(SistemKullanıcıÖznitelikAdları.ŞifreKurtarmaKodu);
            if (String.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }
        public static bool ŞifreKurtarmaLinkiSüresiDoldu(this Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException(nameof(kullanıcı));

            if (kullanıcı.IsGuest())
                return false;

            if (!kullanıcı.KullanıcıRolleri.Any(role => role.Aktif && role.ParolaÖmrünüEtkinleştir))
                return false;

            var kullanıcıAyarları = EngineContext.Current.Resolve<KullanıcıAyarları>();
            if (kullanıcıAyarları.ŞifreÖmrü == 0)
                return false;

            var cacheManager = EngineContext.Current.Resolve<IStatikÖnbellekYönetici>();
            var cacheKey = string.Format(KullanıcıÖnbellekOlayTüketici.KULLANICI_ŞİFRE_ÖMRÜ, kullanıcı.Id);

            //get current password usage time
            var currentLifetime = cacheManager.Al(cacheKey, () =>
            {
                var customerPassword = EngineContext.Current.Resolve<IKullanıcıServisi>().MevcutŞifreAl(kullanıcı.Id);
                if (customerPassword == null)
                    return int.MaxValue;

                return (DateTime.UtcNow - customerPassword.OluşturulmaTarihi).Days;
            });

            return currentLifetime >= kullanıcıAyarları.ŞifreÖmrü;
        }
    
        public static int[] KullanıcıRolIdleri(this Kullanıcı kullanıcı, bool gizliGöster = false)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            var kullanıcıRolIdleri = kullanıcı.KullanıcıRolleri
               .Where(cr => gizliGöster || cr.Aktif)
               .Select(cr => cr.Id)
               .ToArray();

            return kullanıcıRolIdleri;
        }
        public static bool ParolaSüresiDoldu(this Kullanıcı kullanıcı)
        {
            if (kullanıcı == null)
                throw new ArgumentNullException("kullanıcı");

            //Ziyaretçilerin şifresi yok
            if (kullanıcı.IsGuest())
                return false;

            //kullanıcılar için şifre ömrü devre dışı
            if (!kullanıcı.KullanıcıRolleri.Any(role => role.Aktif && role.ParolaÖmrünüEtkinleştir))
                return false;

            //ayar tamamı için devre dışı bırakıldı
            var kullanıcıAyarları = EngineContext.Current.Resolve<KullanıcıAyarları>();
            if (kullanıcıAyarları.ŞifreÖmrü == 0)
                return false;

            //var geneatedDate = Convert.ToDateTime((DateTime.Now.Year - 1));
            var geneatedDate = kullanıcı.ÖznitelikAl<DateTime?>(SistemKullanıcıÖznitelikAdları.ParolaKurtarmaTokenOluşturulmaTarihi);
            if (!geneatedDate.HasValue)
                return false;

            var daysPassed = (DateTime.UtcNow - geneatedDate.Value).TotalDays;
            if (daysPassed > kullanıcıAyarları.ŞifreÖmrü)
                return true;

            return false;
        }
    }
}
