using Core.Domain.Anket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Anketler
{
    public partial interface IAnketServisi
    {
        void AnketSil(Anket Anket);
        Anket AnketAlId(int AnketId);
        IList<Anket> TümAnketleriAl(bool sadeceAnasayfadakileriYükle = false, string sistemAnahtarKelime = null, 
            int sayfaIndeksi = 0, int sayfaBüyüklüğü = int.MaxValue, bool gizliOlanıGöster = false);
        void AnketEkle(Anket Anket);
        void AnketGüncelle(Anket Anket);
        AnketCevabı AnketCevabıAlId(int anketCevabıId);
        void AnketCevabıSil(AnketCevabı anketCevabı);
        bool ZatenOylandı(int anketId, int kullanıcıId);
    }
}
