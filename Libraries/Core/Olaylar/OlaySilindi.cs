namespace Core.Olaylar
{
    public class OlaySilindi<T> where T : TemelVarlık
    {
        public OlaySilindi(T varlık)
        {
            this.Varlık = varlık;
        }
        public T Varlık { get; private set; }
    }
}
