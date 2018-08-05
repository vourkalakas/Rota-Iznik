using Core.Domain.Forum;
namespace Data.Mapping.Forumlar
{
    public partial class ForumMap : TSVarlıkTipiYapılandırması<Forum>
    {
        public ForumMap()
        {
            this.ToTable("Forum");
            this.HasKey(f => f.Id);
            this.Property(f => f.Adı).IsRequired().HasMaxLength(200);

            this.HasRequired(f => f.ForumGrubu)
                .WithMany(fg => fg.Forumlar)
                .HasForeignKey(f => f.ForumGrubuId);
        }
    }
}
