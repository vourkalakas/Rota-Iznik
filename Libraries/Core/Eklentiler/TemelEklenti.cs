namespace Core.Eklentiler
{
    public abstract class TemelEklenti : IEklenti
    {
        public virtual EklentiTanımlayıcı EklentiTanımlayıcı { get; set; }
        public virtual void Yükle()
        {
            EklentiYönetici.EklentileriKurulduOlarakİşaretle(this.EklentiTanımlayıcı.SistemAdı);
        }
        public virtual void Sil()
        {
            EklentiYönetici.EklentileriKaldırıldıOlarakİşaretle(this.EklentiTanımlayıcı.SistemAdı);
        }
        public virtual string SayfaYapılandırmaUrlsiniAl()
        {
            return null;
        }
    }
}
