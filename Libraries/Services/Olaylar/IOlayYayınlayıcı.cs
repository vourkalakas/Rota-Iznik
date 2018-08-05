
namespace Services.Olaylar
{
    public interface IOlayYayınlayıcı
    {
        void Yayınla<T>(T olayMesajı);
    }
}
