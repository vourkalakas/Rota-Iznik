namespace Core.Domain.Blogs
{
    public class YeniYorumOnaylandıOlayı
    {
        public YeniYorumOnaylandıOlayı(BlogYorumu blogYorumu)
        {
            this.BlogYorumu = blogYorumu;
        }
        public BlogYorumu BlogYorumu { get; private set; }
    }
}
