using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Core.Altyapı;
using Newtonsoft.Json;

namespace Core.Eklentiler
{
    public class EklentiTanımlayıcı : IDescriptor, IComparable<EklentiTanımlayıcı>
    {
        public EklentiTanımlayıcı()
        {
            this.DesteklenenSürümler = new List<string>();
            this.KısıtlıSiteler = new List<int>();
            this.KısıtlıMüsteriRolleriListesi = new List<int>();
        }
        public EklentiTanımlayıcı(Assembly referanslıAssembly) : this()
        {
            this.ReferanslıAssembly = referanslıAssembly;
        }
        public virtual string EklentiDosyaAdı { get; set; }
        [JsonIgnore]
        public virtual Type EklentiTipi { get; set; }
        [JsonIgnore]
        public virtual Assembly ReferanslıAssembly { get; internal set; }
        [JsonIgnore]
        public virtual FileInfo OrijinalAssemblyDosyası { get; internal set; }
        [JsonProperty(PropertyName = "Grup")]
        public virtual string Grup { get; set; }
        [JsonProperty(PropertyName = "KısaAd")]
        public virtual string KısaAd { get; set; }
        [JsonProperty(PropertyName = "SistemAdı")]
        public virtual string SistemAdı { get; set; }
        [JsonProperty(PropertyName = "Sürüm")]
        public virtual string Sürüm { get; set; }
        [JsonProperty(PropertyName = "DesteklenenSürümler")]
        public virtual IList<string> DesteklenenSürümler { get; set; }
        [JsonProperty(PropertyName = "Yazar")]
        public virtual string Yazar { get; set; }
        [JsonProperty(PropertyName = "Açıklama")]
        public virtual string Açıklama { get; set; }
        [JsonProperty(PropertyName = "GörüntülemeSırası")]
        public virtual int GörüntülemeSırası { get; set; }
        [JsonProperty(PropertyName = "KısıtlıSiteler")]
        public virtual IList<int> KısıtlıSiteler { get; set; }
        [JsonProperty(PropertyName = "KısıtlıMüsteriRolleriListesi")]
        public virtual IList<int> KısıtlıMüsteriRolleriListesi { get; set; }
        [JsonIgnore]
        public virtual bool Kuruldu { get; set; }

        public IEklenti Instance()
        {
            return Instance<IEklenti>();
        }
        public virtual T Instance<T>() where T : class, IEklenti
        {
            object instance = null;
            try
            {
                instance = EngineContext.Current.Resolve(EklentiTipi);
            }
            catch
            {
                //try resolve
            }
            if (instance == null)
            {
                instance = EngineContext.Current.ResolveUnregistered(EklentiTipi);
            }
            var typedInstance = instance as T;
            if (typedInstance != null)
                typedInstance.EklentiTanımlayıcı = this;
            return typedInstance;
        }

        public int CompareTo(EklentiTanımlayıcı diğer)
        {
            if (GörüntülemeSırası != diğer.GörüntülemeSırası)
                return GörüntülemeSırası.CompareTo(diğer.GörüntülemeSırası);

            return KısaAd.CompareTo(diğer.KısaAd);
        }

        public override string ToString()
        {
            return KısaAd;
        }

        public override bool Equals(object obj)
        {
            var diğer = obj as EklentiTanımlayıcı;
            return diğer != null &&
                SistemAdı != null &&
                SistemAdı.Equals(diğer.SistemAdı);
        }

        public override int GetHashCode()
        {
            return SistemAdı.GetHashCode();
        }
    }
}

