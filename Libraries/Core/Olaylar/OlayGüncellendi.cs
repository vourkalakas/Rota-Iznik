namespace Core.Olaylar
{
    public class OlayGüncellendi<T> where T : TemelVarlık
    {
        public OlayGüncellendi(T varlık)
        {
            this.Varlık = varlık;
        }
        public T Varlık { get; private set; }
    }
}
