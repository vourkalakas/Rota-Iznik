using System;
using System.Linq;
using Core;
using Core.Altyapı;
using Data;


namespace Services.Genel
{
    public static class GenelÖznitelikUzantıları
    {
        public static TPropType ÖznitelikAl<TPropType>(this TemelVarlık varlık, string key, int siteId = 0)
        {
            var genelÖznitelikServisi = EngineContext.Current.Resolve<IGenelÖznitelikServisi>();
            return ÖznitelikAl<TPropType>(varlık, key, genelÖznitelikServisi, siteId);
        }
        public static TPropType ÖznitelikAl<TPropType>(this TemelVarlık varlık,
            string key, IGenelÖznitelikServisi genelÖznitelikServisi, int siteId = 0)
        {
            if (varlık == null)
                throw new ArgumentNullException("varlık");

            string keyGroup = varlık.GetUnproxiedEntityType().Name;

            var props = genelÖznitelikServisi.VarlıkİçinÖznitelikleriAl(varlık.Id, keyGroup);
            if (props == null)
                return default(TPropType);
            props = props.Where(x => x.SiteId == siteId).ToList();
            if (!props.Any())
                return default(TPropType);

            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); 

            if (prop == null || string.IsNullOrEmpty(prop.Value))
                return default(TPropType);

            return GenelYardımcı.To<TPropType>(prop.Value);
        }
    }
}
