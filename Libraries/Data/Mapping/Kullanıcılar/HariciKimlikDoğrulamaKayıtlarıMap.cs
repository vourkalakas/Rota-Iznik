using Core.Domain.Kullanıcılar;

namespace Data.Mapping.Kullanıcılar
{
    public partial class HariciKimlikDoğrulamaKayıtlarıMap : TSVarlıkTipiYapılandırması<HariciKimlikDoğrulamaKaydı>
    {
        public HariciKimlikDoğrulamaKayıtlarıMap()
        {
            this.ToTable("HariciKimlikDoğrulamaKaydı");

            this.HasKey(ear => ear.Id);

            this.HasRequired(ear => ear.Kullanıcı)
                .WithMany(c => c.HariciKimlikDoğrulamaKayıtları)
                .HasForeignKey(ear => ear.KullanıcıId);

        }
    }
}
