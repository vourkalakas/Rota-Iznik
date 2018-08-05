using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json;
using Core.BileşenModeli;
using Core.Yapılandırma;

namespace Core.Eklentiler
{
    public class EklentiYönetici
    {
        #region Fields

        private static readonly ReaderWriterLockSlim Kilitleyici = new ReaderWriterLockSlim();
        private static DirectoryInfo _gölgeKopyaKlasörü;
        private static readonly List<string> TemelAppKütüphaneleri;

        #endregion

        #region Ctor

        static EklentiYönetici()
        {
            TemelAppKütüphaneleri = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
                .GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name).ToList();
            
            if (!AppDomain.CurrentDomain.BaseDirectory.Equals(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
                TemelAppKütüphaneleri.AddRange(new DirectoryInfo(Environment.CurrentDirectory).GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name));
            
            var refsPathName = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, RefsPathName));
            if (refsPathName.Exists)
                TemelAppKütüphaneleri.AddRange(refsPathName.GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name));
        }

        #endregion

        #region Methods
        public static void Initialize(ApplicationPartManager applicationPartManager, Config config)
        {
            if (applicationPartManager == null)
                throw new ArgumentNullException(nameof(applicationPartManager));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            using (new TekKullanımlıkKilit(Kilitleyici))
            {
                var eklentiKlasörü = new DirectoryInfo(GenelYardımcı.MapPath(EklentiYolu));
                _gölgeKopyaKlasörü = new DirectoryInfo(GenelYardımcı.MapPath(GölgeKopyaYolu));

                var referenslıEklentiler = new List<EklentiTanımlayıcı>();
                var uyumsuzEklentiler = new List<string>();

                try
                {
                    var kuruluEklentiSistemAdları = YüklüEklentilerinAdlarınıAl(GenelYardımcı.MapPath(YüklüEklentilerinDosyaYolu));

                    Debug.WriteLine("Gölge kopya klasörü oluşturma ve dll sorgulama");
                    //klasörlerin oluşturulduğunu doğrula
                    Directory.CreateDirectory(eklentiKlasörü.FullName);
                    Directory.CreateDirectory(_gölgeKopyaKlasörü.FullName);

                    //bin klasöründeki tüm dosyaların listesini al
                    var binDosyaları = _gölgeKopyaKlasörü.GetFiles("*", SearchOption.AllDirectories);
                    if (config.ClearPluginShadowDirectoryOnStartup)
                    {
                        //Gölge kopyalanan eklentileri temizle
                        foreach (var f in binDosyaları)
                        {
                            if (f.Name.Equals("placeholder.txt", StringComparison.InvariantCultureIgnoreCase))
                                continue;
                            Debug.WriteLine("Siliniyor " + f.Name);
                            try
                            {
                                var fileName = Path.GetFileName(f.FullName);
                                if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                                    continue;
                                File.Delete(f.FullName);
                            }
                            catch (Exception exc)
                            {
                                Debug.WriteLine("Silinirken hata oluştu " + f.Name + ". Hata: " + exc);
                            }
                        }
                    }

                    //açıklama dosyalarını kur
                    foreach (var dfd in AçıklamaDosyalarıveTanımlayıcılarıAl(eklentiKlasörü))
                    {
                        var açıklamaDosyası = dfd.Key;
                        var eklentiTanımlayıcı = dfd.Value;

                        //Eklenti sürümünün geçerli olduğundan emin olun
                        if (!eklentiTanımlayıcı.DesteklenenSürümler.Contains(Sürüm.MevcutSürüm, StringComparer.InvariantCultureIgnoreCase))
                        {
                            uyumsuzEklentiler.Add(eklentiTanımlayıcı.SistemAdı);
                            continue;
                        }

                        //doğrulamalar
                        if (String.IsNullOrWhiteSpace(eklentiTanımlayıcı.SistemAdı))
                            throw new Exception(string.Format("'{0}' eklentisinin sistem adı yoktur . Eklentiye benzersiz bir ad atamayı ve yeniden derlemeyi deneyin.", açıklamaDosyası.FullName));
                        if (referenslıEklentiler.Contains(eklentiTanımlayıcı))
                            throw new Exception(string.Format("'{0}' eklenti adı daha önce kullanılmıştır", eklentiTanımlayıcı.SistemAdı));

                        //Kuruldu olarak ayarla
                        eklentiTanımlayıcı.Kuruldu = kuruluEklentiSistemAdları
                            .FirstOrDefault(x => x.Equals(eklentiTanımlayıcı.SistemAdı, StringComparison.InvariantCultureIgnoreCase)) != null;

                        try
                        {
                            if (açıklamaDosyası.Directory == null)
                                throw new Exception(string.Format("'{0}' açıklama dosyası için dizin çözümlenemiyor", açıklamaDosyası.Name));
                            //Eklentilerdeki tüm DLL'lerin listesini alın (bin klasörü içindekiler değil!)
                            var eklentiDosyaları = açıklamaDosyası.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                //gölge kopyalanan eklentileri kaydettirmediğinden emin olun
                                .Where(x => !binDosyaları.Select(q => q.FullName).Contains(x.FullName))
                                .Where(x => PaketEklentiDosyası(x.Directory))
                                .ToList();

                            //Diğer eklenti açıklama bilgileri
                            var anaEklentiDosyası = eklentiDosyaları
                                .FirstOrDefault(x => x.Name.Equals(eklentiTanımlayıcı.EklentiDosyaAdı, StringComparison.InvariantCultureIgnoreCase));
                            eklentiTanımlayıcı.OrijinalAssemblyDosyası = anaEklentiDosyası;
                            if (anaEklentiDosyası == null)
                            {
                                uyumsuzEklentiler.Add(eklentiTanımlayıcı.SistemAdı);
                                continue;
                            }
                            eklentiTanımlayıcı.OrijinalAssemblyDosyası = anaEklentiDosyası;
                            //Gölge kopya ana eklenti dosyası
                            eklentiTanımlayıcı.ReferanslıAssembly = DosyaDağıtımıYap(anaEklentiDosyası, applicationPartManager, config);

                            //Başvurulan tüm derlemeleri şimdi kur
                            foreach (var eklenti in eklentiDosyaları
                                .Where(x => !x.Name.Equals(anaEklentiDosyası.Name, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !ZatenYüklendi(x)))
                                DosyaDağıtımıYap(eklenti, applicationPartManager, config);

                            //Eklenti türünü başlat (assembly başına yalnızca bir eklentiye izin verilir)
                            foreach (var t in eklentiTanımlayıcı.ReferanslıAssembly.GetTypes())
                                if (typeof(IEklenti).IsAssignableFrom(t))
                                    if (!t.IsInterface)
                                        if (t.IsClass && !t.IsAbstract)
                                        {
                                            eklentiTanımlayıcı.EklentiTipi = t;
                                            break;
                                        }

                            referenslıEklentiler.Add(eklentiTanımlayıcı);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            //Bir eklenti adı ekleyin. Bu şekilde problemli bir eklenti kolayca tespit edebiliriz
                            var msg = string.Format("Eklenti '{0}'. ", eklentiTanımlayıcı.KısaAd);
                            foreach (var e in ex.LoaderExceptions)
                                msg += e.Message + Environment.NewLine;

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                        catch (Exception ex)
                        {
                            //Bir eklenti adı ekleyin. Bu şekilde problemli bir eklenti kolayca tespit edebiliriz
                            var msg = string.Format("Eklenti '{0}'. {1}", eklentiTanımlayıcı.KısaAd, ex.Message);

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Empty;
                    for (var e = ex; e != null; e = e.InnerException)
                        msg += e.Message + Environment.NewLine;

                    var fail = new Exception(msg, ex);
                    throw fail;
                }
                ReferenslıEklentiler = referenslıEklentiler;
                UyumsuzEklentiler = uyumsuzEklentiler;
            }
        }
        public static void EklentileriKurulduOlarakİşaretle(string sistemAdı)
        {
            if (String.IsNullOrEmpty(sistemAdı))
                throw new ArgumentNullException("sistemAdı");

            var dosyaYolu = GenelYardımcı.MapPath(YüklüEklentilerinDosyaYolu);
            if (!File.Exists(dosyaYolu))
                using (File.Create(dosyaYolu))
                {
                    //Dosyayı oluşturduktan sonra kapatmak için 'using' özelliğini kullanıyoruz
                }

            var kuruluEklentiSistemAdları = YüklüEklentilerinAdlarınıAl(dosyaYolu);
            bool zatenKuruluOlarakİşaretlendi = kuruluEklentiSistemAdları
                                .FirstOrDefault(x => x.Equals(sistemAdı, StringComparison.InvariantCultureIgnoreCase)) != null;
            if (!zatenKuruluOlarakİşaretlendi)
                kuruluEklentiSistemAdları.Add(sistemAdı);
            YüklenenEklentiAdlarınıKaydet(kuruluEklentiSistemAdları, dosyaYolu);
        }
        public static void EklentileriKaldırıldıOlarakİşaretle(string sistemAdı)
        {
            if (String.IsNullOrEmpty(sistemAdı))
                throw new ArgumentNullException("sistemAdı");

            var dosyaYolu = GenelYardımcı.MapPath(YüklüEklentilerinDosyaYolu);
            if (!File.Exists(dosyaYolu))
                using (File.Create(dosyaYolu))
                {
                    //Dosyayı oluşturduktan sonra kapatmak için 'using' özelliğini kullanıyoruz
                }


            var kuruluEklentiSistemAdları = YüklüEklentilerinAdlarınıAl(dosyaYolu);
            bool zatenKuruluOlarakİşaretlendi = kuruluEklentiSistemAdları
                                .FirstOrDefault(x => x.Equals(sistemAdı, StringComparison.InvariantCultureIgnoreCase)) != null;
            if (zatenKuruluOlarakİşaretlendi)
                kuruluEklentiSistemAdları.Remove(sistemAdı);
            YüklenenEklentiAdlarınıKaydet(kuruluEklentiSistemAdları, dosyaYolu);
        }
        public static void EklentilerinTümüKaldırıldıOlarakİşaretle()
        {
            var dosyaYolu = GenelYardımcı.MapPath(YüklüEklentilerinDosyaYolu);
            if (File.Exists(dosyaYolu))
                File.Delete(dosyaYolu);
        }
        public static EklentiTanımlayıcı EklentiBul(Type typeInAssembly)
        {
            if (typeInAssembly == null)
                throw new ArgumentNullException(nameof(typeInAssembly));

            if (ReferenslıEklentiler == null)
                return null;

            return ReferenslıEklentiler.FirstOrDefault(plugin => plugin.ReferanslıAssembly != null
                && plugin.ReferanslıAssembly.FullName.Equals(typeInAssembly.Assembly.FullName, StringComparison.InvariantCultureIgnoreCase));
        }
        public static EklentiTanımlayıcı DosyadanEklentiTanımlayıcıAl(string dosyaYolu)
        {
            var text = File.ReadAllText(dosyaYolu);

            return TextdenEklentiTanımlayıcıAl(text);
        }
        public static EklentiTanımlayıcı TextdenEklentiTanımlayıcıAl(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new EklentiTanımlayıcı();
            
            var descriptor = JsonConvert.DeserializeObject<EklentiTanımlayıcı>(text);
            
            if (!descriptor.DesteklenenSürümler.Any())
                descriptor.DesteklenenSürümler.Add("2.00");

            return descriptor;
        }
        public static void EklentiTanımlayıcıKaydet(EklentiTanımlayıcı pluginDescriptor)
        {
            if (pluginDescriptor == null)
                throw new ArgumentException(nameof(pluginDescriptor));
            
            if (pluginDescriptor.OrijinalAssemblyDosyası == null)
                throw new Exception($"Cannot load original assembly path for {pluginDescriptor.SistemAdı} plugin.");

            var filePath = Path.Combine(pluginDescriptor.OrijinalAssemblyDosyası.Directory.FullName, PluginDescriptionFileName);
            if (!File.Exists(filePath))
                throw new Exception($"Description file for {pluginDescriptor.SistemAdı} plugin does not exist. {filePath}");

            //save the file
            var text = JsonConvert.SerializeObject(pluginDescriptor, Formatting.Indented);
            File.WriteAllText(filePath, text);
        }
        public static bool EklentiSil(EklentiTanımlayıcı pluginDescriptor)
        {
            if (pluginDescriptor == null)
                return false;
            
            if (pluginDescriptor.Kuruldu)
                return false;

            if (pluginDescriptor.OrijinalAssemblyDosyası.Directory.Exists)
                GenelYardımcı.KlasörSil(pluginDescriptor.OrijinalAssemblyDosyası.DirectoryName);

            return true;
        }

        #endregion

        #region Utilities
        private static IEnumerable<KeyValuePair<FileInfo, EklentiTanımlayıcı>> AçıklamaDosyalarıveTanımlayıcılarıAl(DirectoryInfo eklentiKlasörü)
        {
            if (eklentiKlasörü == null)
                throw new ArgumentNullException("eklentiKlasörü");

            //Liste oluştur (<dosya bilgisi, ayrıştırılmış eklenti tanımlayıcı>)
            var sonuç = new List<KeyValuePair<FileInfo, EklentiTanımlayıcı>>();
            //Listeye görüntülenme sırası ve  yol ekle
            foreach (var açıklamaDosyası in eklentiKlasörü.GetFiles("Açıklama.txt", SearchOption.AllDirectories))
            {
                if (!PaketEklentiDosyası(açıklamaDosyası.Directory))
                    continue;

                //dosyayı ayrıştır
                var eklentiTanımlayıcı = DosyadanEklentiTanımlayıcıAl(açıklamaDosyası.FullName);

                //Listeyi doldur
                sonuç.Add(new KeyValuePair<FileInfo, EklentiTanımlayıcı>(açıklamaDosyası, eklentiTanımlayıcı));
            }

            //Görüntüleme sırasına göre listeyi sıralayın. NOT: En düşük DisplayOrder ilk olacaktır, yani 0, 1, 1, 1, 5, 10
            sonuç.Sort((firstPair, nextPair) => firstPair.Value.GörüntülemeSırası.CompareTo(nextPair.Value.GörüntülemeSırası));
            return sonuç;
        }
        private static IList<string> YüklüEklentilerinAdlarınıAl(string dosyaYolu)
        {
            //check whether file exists
            if (!File.Exists(dosyaYolu))
            {
                dosyaYolu = GenelYardımcı.MapPath(ObsoleteInstalledPluginsFilePath);
                if (!File.Exists(dosyaYolu))
                    return new List<string>();

                var eklentiSistemAdları = new List<string>();
                using (var reader = new StringReader(File.ReadAllText(dosyaYolu)))
                {
                    var eklentiAdı = string.Empty;
                    while ((eklentiAdı = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(eklentiAdı))
                            eklentiSistemAdları.Add(eklentiAdı.Trim());
                    }
                }
                
                YüklenenEklentiAdlarınıKaydet(eklentiSistemAdları, GenelYardımcı.MapPath(YüklüEklentilerinDosyaYolu));
                File.Delete(dosyaYolu);
                return eklentiSistemAdları;
            }

            var text = File.ReadAllText(dosyaYolu);
            if (string.IsNullOrEmpty(text))
                return new List<string>();
            
            return JsonConvert.DeserializeObject<IList<string>>(text);
        }
        private static void YüklenenEklentiAdlarınıKaydet(IList<string> eklentiSistemAdları, string dosyaYolu)
        {
            var text = JsonConvert.SerializeObject(eklentiSistemAdları, Formatting.Indented);
            File.WriteAllText(dosyaYolu, text);
        }

        private static bool ZatenYüklendi(FileInfo dosyaBilgisi)
        {
            if (TemelAppKütüphaneleri.Any(sli => sli.Equals(dosyaBilgisi.Name, StringComparison.InvariantCultureIgnoreCase)))
                return true;
            try
            {
                string uzantısızDosyaAdı = Path.GetFileNameWithoutExtension(dosyaBilgisi.FullName);
                if (uzantısızDosyaAdı == null)
                    throw new Exception(string.Format("{0} Dosya uzantısı bulunamadı.", dosyaBilgisi.Name));
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string assemblyAdı = a.FullName.Split(new[] { ',' }).FirstOrDefault();
                    if (uzantısızDosyaAdı.Equals(assemblyAdı, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Bir derlemenin yüklü olup olmadığını doğrulanamadı. " + exc);
            }
            return false;
        }
        private static Assembly DosyaDağıtımıYap(FileInfo plug, ApplicationPartManager applicationPartManager, Config config)
        {
            if (plug.Directory == null || plug.Directory.Parent == null)
                throw new InvalidOperationException("The plugin directory for the " + plug.Name + " file exists in a folder outside of the allowed nopCommerce folder hierarchy");
            
            var shadowCopyPlugFolder = Directory.CreateDirectory(_gölgeKopyaKlasörü.FullName);
            var shadowCopiedPlug = GölgeKopyaDosyası(plug, shadowCopyPlugFolder);


            //we can now register the plugin definition
            var assemblyName = AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName);
            Assembly shadowCopiedAssembly;
            try
            {
                shadowCopiedAssembly = Assembly.Load(assemblyName);
            }
            catch (FileLoadException)
            {
                if (config.UseUnsafeLoadAssembly)
                {
                    shadowCopiedAssembly = Assembly.UnsafeLoadFrom(shadowCopiedPlug.FullName);
                }
                else
                {
                    throw;
                }
            }

            Debug.WriteLine("Adding to ApplicationParts: '{0}'", shadowCopiedAssembly.FullName);
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(shadowCopiedAssembly));

            return shadowCopiedAssembly;
        }
        private static FileInfo GölgeKopyaDosyası(FileInfo plug, DirectoryInfo gölgeKopyaKlasörü)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(gölgeKopyaKlasörü.FullName, plug.Name));
            
            if (shadowCopiedPlug.Exists)
            {
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= plug.CreationTimeUtc.Ticks;
                if (areFilesIdentical)
                {
                    Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);
                    shouldCopy = false;
                }
                else
                {
                    Debug.WriteLine("New plugin found; Deleting the old file: '{0}'", shadowCopiedPlug.Name);
                    File.Delete(shadowCopiedPlug.FullName);
                }
            }

            if (!shouldCopy)
                return shadowCopiedPlug;

            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException)
            {
                Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                }
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
        }
      
        private static bool PaketEklentiDosyası(DirectoryInfo klasör)
        {
            if (klasör == null) return false;
            if (!klasör.Parent.Name.Equals(PluginsPathName, StringComparison.InvariantCultureIgnoreCase)) return false;
            return true;
        }
        /*
        private static string KuruluEklentilerDosyaYolunuAl()
        {
            return GenelYardımcı.MapPath(KuruluEklentilerYolu);
        }
        */
        #endregion

        #region Properties
        public static string ObsoleteInstalledPluginsFilePath => "~/App_Data/YüklüEklentiler.txt";
        public static string YüklüEklentilerinDosyaYolu => "~/App_Data/YüklüEklentiler.json";
        public static string EklentiYolu => "~/Eklentiler";
        public static string PluginsPathName => "Eklentiler";
        public static string GölgeKopyaYolu => "~/Eklentiler/bin";
        public static string RefsPathName => "refs";
        public static string PluginDescriptionFileName => "eklenti.json";
        public static IEnumerable<EklentiTanımlayıcı> ReferenslıEklentiler { get; set; }
        public static IEnumerable<string> UyumsuzEklentiler { get; set; }

        #endregion
    }
}