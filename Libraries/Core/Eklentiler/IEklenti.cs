namespace Core.Eklentiler
{
    public interface IEklenti
    {
        string SayfaYapılandırmaUrlsiniAl();
        EklentiTanımlayıcı EklentiTanımlayıcı { get; set; }
        void Yükle();
        void Sil();
    }
}