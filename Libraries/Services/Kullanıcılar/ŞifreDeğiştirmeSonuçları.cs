using System.Collections.Generic;
using System.Linq;

namespace Services.Kullanıcılar
{
    public class ŞifreDeğiştirmeSonuçları
    {
        public ŞifreDeğiştirmeSonuçları()
        {
            this.Hatalar = new List<string>();
        }
        public bool Başarılı
        {
            get { return !this.Hatalar.Any(); }
        }
        public void HataEkle(string hata)
        {
            this.Hatalar.Add(hata);
        }
        public IList<string> Hatalar { get; set; }
    }
}