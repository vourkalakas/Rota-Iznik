using System.Collections.Generic;
using System.Linq;

namespace Core.Data
{
    public partial interface IDepo<T> where T : TemelVarlık
    {
        T AlId(object id);
        void Ekle(T varlık);
        void Ekle(IEnumerable<T> varlıklar);
        void Güncelle(T varlık);
        void Güncelle(IEnumerable<T> varlıklar);
        void Sil(T varlık);
        void Sil(IEnumerable<T> varlıklar);
        IQueryable<T> Tablo { get; }
        IQueryable<T> Tabloİzlemesiz { get; }
    }
}
