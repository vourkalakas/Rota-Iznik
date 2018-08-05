using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using Core.Data;
using Data.Initializers;

namespace Data
{
    public class SqlCeDataProvider : IDataSağlayıcı
    {
        public virtual void BağlantıFabrikasıBaşlat()
        {
            var connectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            //TODO fix compilation warning (below)
#pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;
        }
        public virtual void VeritabanınıBaşlat()
        {
            BağlantıFabrikasıBaşlat();
            VeritabanıBaşlatıcıyıAyarla();
        }
        public virtual void VeritabanıBaşlatıcıyıAyarla()
        {
            var initializer = new CreateCeDatabaseIfNotExists<ObjectContext>();
            Database.SetInitializer(initializer);
        }
        public virtual bool StoredProceduredDestekli
        {
            get { return false; }
        }
        public bool BackupDestekli
        {
            get { return false; }
        }
        public virtual DbParameter ParametreAl()
        {
            return new SqlParameter();
        }
        public int DesteklenenBinaryHashUzunluğu()
        {
            return 0; //HASHBYTES functions is missing in SQL CE
        }
    }
}