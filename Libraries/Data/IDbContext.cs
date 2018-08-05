using System.Collections.Generic;
using System.Data.Entity;
using Core;
namespace Data
{
    public interface IDbContext
    {
        IDbSet<TEntity> Ayarla<TEntity>() where TEntity : TemelVarlık;
        int SaveChanges();
        IList<TEntity> StoredProcedureListesiniÇalıştır<TEntity>(string komut, params object[] parametreler)
            where TEntity : TemelVarlık, new();
        IEnumerable<TElement> SqlSorgusu<TElement>(string sql, params object[] parametreler);
        int SqlKomutunuÇalıştır(string sql, bool İşlemiTeyitEt = false, int? zamanAşımı = null, params object[] parameters);
        void Ayır(object varlık);
        bool ProxyOluşturmaEtkin { get; set; }
        bool DeğişiklikleriOtomatikAlgılamaEtkin { get; set; }
    }
}