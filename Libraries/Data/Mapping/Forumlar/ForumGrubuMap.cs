using Core.Domain.Forum;

namespace Data.Mapping.Forumlar
{
    public partial class ForumGrubuMap : TSVarlıkTipiYapılandırması<ForumGrubu>
    {
        public ForumGrubuMap()
        {
            this.ToTable("ForumGrubu");
            this.HasKey(f => f.Id);
            this.Property(f => f.Adı).IsRequired().HasMaxLength(200);
        }
    }
}
