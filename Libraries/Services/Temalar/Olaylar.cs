using System.Collections.Generic;
namespace Services.Temalar
{
    public class TemaYüklendiOlayı
    {
        #region Ctor
        public TemaYüklendiOlayı(IList<TemaAçıklayıcı> yüklenenTema)
        {
            this.YüklenenTema = yüklenenTema;
        }
        #endregion

        #region Properties
        public IList<TemaAçıklayıcı> YüklenenTema { get; private set; }
        #endregion
    }
}
