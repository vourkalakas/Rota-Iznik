namespace Services.Olaylar
{
    public interface IMüşteri<T>
    {
        void Olay(T olayMesajı);
    }
}
