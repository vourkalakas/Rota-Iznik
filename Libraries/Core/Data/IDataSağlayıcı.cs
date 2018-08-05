using System.Data.Common;

namespace Core.Data
{
    public interface IDataSağlayıcı
    {
        void BağlantıFabrikasıBaşlat();
        void VeritabanıBaşlatıcıyıAyarla();
        void VeritabanınıBaşlat();
        bool StoredProceduredDestekli { get; }
        bool BackupDestekli { get; }
        DbParameter ParametreAl();
        int DesteklenenBinaryHashUzunluğu();
    }
}
