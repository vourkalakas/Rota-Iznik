using Core.Domain.Anket;

namespace Data.Mapping.Anketler
{
    public partial class AnketCevabıMap : TSVarlıkTipiYapılandırması<AnketCevabı>
    {
        public AnketCevabıMap()
        {
            this.ToTable("AnketCevabı");
            this.HasKey(p => p.Id);
            this.Property(p => p.Adı).IsRequired();
            this.HasRequired(pa => pa.Anket)
               .WithMany(p => p.AnketCevabı)
               .HasForeignKey(pa => pa.AnketId).WillCascadeOnDelete(true);
        }
    }
}
