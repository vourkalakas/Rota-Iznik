namespace Core.Domain.Kullanıcılar
{
    public partial class HariciKimlikDoğrulamaKaydı : TemelVarlık
    {
        public int KullanıcıId { get; set; }
        public string Email { get; set; }
        public string HariciTanımlayıcı { get; set; }
        public string HariciGörünümTanımlayıcı { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthAccessToken { get; set; }
        public string SağlayıcıSistemAdı { get; set; }
        public virtual Kullanıcı Kullanıcı { get; set; }
    }
}