using System.Collections.Generic;

namespace Services.Temalar
{
    public partial interface ITemaSağlayıcı
    {
        #region Methods
        TemaAçıklayıcı YazıdanTemaAçıklayıcıAl(string text);
        IList<TemaAçıklayıcı> TemalarıAl();
        TemaAçıklayıcı TemaAlSistemAdı(string sistemAdı);
        bool TemaMevcut(string sistemAdı);
        #endregion

        #region Properties
        string TemalarınYolu { get; }
        string TemaAçıklamasıDosyaAdı { get; }
        #endregion
    }
}
