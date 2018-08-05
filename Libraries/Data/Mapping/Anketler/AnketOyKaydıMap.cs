using Core.Domain.Anket;

namespace Data.Mapping.Anketler
{
    public partial class AnketOyKaydıMap : TSVarlıkTipiYapılandırması<AnketOyKaydı>
    {
        public AnketOyKaydıMap()
        {
            this.ToTable("AnketOyKaydı");
            this.HasKey(p => p.Id);
            this.HasRequired(pvr => pvr.AnketCevabı)
                .WithMany(pa => pa.AnketOyKaydı)
                .HasForeignKey(pvr => pvr.AnketCevapId);

            this.HasRequired(cc => cc.Kullanıcı)
                .WithMany()
                .HasForeignKey(cc => cc.KullanıcıId);
        }
    }
}
