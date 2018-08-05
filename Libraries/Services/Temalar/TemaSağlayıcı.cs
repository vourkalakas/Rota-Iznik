using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Services.Temalar
{
    public partial class TemaSağlayıcı : ITemaSağlayıcı
    {
        #region Fields

        private IList<TemaAçıklayıcı> _temaAçıklayıcı;

        #endregion

        #region Methods
        public TemaAçıklayıcı YazıdanTemaAçıklayıcıAl(string text)
        {
            var temaAçıklayıcı = JsonConvert.DeserializeObject<TemaAçıklayıcı>(text);
            
            if (_temaAçıklayıcı?.Any(descriptor => descriptor.SistemAdı.Equals(temaAçıklayıcı?.SistemAdı, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                throw new Exception($"'{temaAçıklayıcı.SistemAdı}' sistemadı zaten belirlendi");

            return temaAçıklayıcı;
        }
        public IList<TemaAçıklayıcı> TemalarıAl()
        {
            if (_temaAçıklayıcı == null)
            {
                var temaKlasörü = new DirectoryInfo(GenelYardımcı.MapPath(TemalarınYolu));
                _temaAçıklayıcı = new List<TemaAçıklayıcı>();
                foreach (var açıklamaDosyası in temaKlasörü.GetFiles(TemaAçıklamasıDosyaAdı, SearchOption.AllDirectories))
                {
                    var text = File.ReadAllText(açıklamaDosyası.FullName);
                    if (string.IsNullOrEmpty(text))
                        continue;
                    
                    var temaAçıklayıcı = YazıdanTemaAçıklayıcıAl(text);
                    
                    if (string.IsNullOrEmpty(temaAçıklayıcı?.SistemAdı))
                        throw new Exception($"'{açıklamaDosyası.FullName}' tema açıklayıcısında sistem adı mevcut değil");

                    _temaAçıklayıcı.Add(temaAçıklayıcı);
                }
            }

            return _temaAçıklayıcı;
        }
        public TemaAçıklayıcı TemaAlSistemAdı(string sistemAdı)
        {
            if (string.IsNullOrEmpty(sistemAdı))
                return null;

            return TemalarıAl().SingleOrDefault(açıklayıcı => açıklayıcı.SistemAdı.Equals(sistemAdı, StringComparison.InvariantCultureIgnoreCase));
        }
        public bool TemaMevcut(string sistemAdı)
        {
            if (string.IsNullOrEmpty(sistemAdı))
                return false;

            return TemalarıAl().Any(açıklayıcı => açıklayıcı.SistemAdı.Equals(sistemAdı, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion

        #region Properties
        public string TemalarınYolu => "~/Temalar";
        public string TemaAçıklamasıDosyaAdı => "tema.json";

        #endregion
    }
}
