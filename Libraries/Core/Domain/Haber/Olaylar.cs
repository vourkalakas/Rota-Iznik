namespace Core.Domain.Haber
{
    public class YeniYorumOnaylandıOlayı
    {
        public YeniYorumOnaylandıOlayı(HaberYorumu haberYorumu)
        {
            this.HaberYorumu = haberYorumu;
        }
        public HaberYorumu HaberYorumu { get; private set; }
    }
}
