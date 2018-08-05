using Core.Domain.Güvenlik;
namespace Data.Mapping.Güvenlik
{
    public partial class IzinKaydıMap : TSVarlıkTipiYapılandırması<İzinKaydı>
    {
        public IzinKaydıMap()
        {
            this.ToTable("İzinKaydı");
            this.HasKey(pr => pr.Id);
            this.Property(pr => pr.Adı).IsRequired();
            this.Property(pr => pr.SistemAdı).IsRequired().HasMaxLength(255);
            this.Property(pr => pr.Kategori).IsRequired().HasMaxLength(255);

            this.HasMany(pr => pr.KullanıcıRolleri)
                .WithMany(cr => cr.İzinKayıtları)
                .Map(m => m.ToTable("İzinKaydıKullanıcıRolü"));
        }
    }
}
