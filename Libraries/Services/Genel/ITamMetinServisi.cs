
namespace Services.Genel
{
    public partial interface ITamMetinServisi
    {
        bool TamMetinDestekli();
        void TamMetinEtkinleştir();
        void TamMetinDevreDışıBırak();
    }
}
