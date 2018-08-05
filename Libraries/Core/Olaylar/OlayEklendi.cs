namespace Core.Olaylar
{
    public class OlayEklendi<T> where T : TemelVarlık
    {
        public OlayEklendi(T varlık)
        {
            this.Varlık = varlık;
        }
        public T Varlık { get; private set; }
    }
}
