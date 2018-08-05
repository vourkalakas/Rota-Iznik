using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Core;
using Core.Data;
using Data.Initializers;


namespace Data
{
    public class SqlServerDataProvider : IDataSağlayıcı
    {
        #region Utilities

        protected virtual string[] KomutlarıAyrıştır(string dosyaYolu, bool mevcutDeğilseHata)
        {
            if (!File.Exists(dosyaYolu))
            {
                if (mevcutDeğilseHata)
                    throw new ArgumentException(string.Format("Belirtilen dosya mevcut değil - {0}", dosyaYolu));

                return new string[0];
            }


            var ifadeler = new List<string>();
            using (var stream = File.OpenRead(dosyaYolu))
            using (var reader = new StreamReader(stream))
            {
                string ifade;
                while ((ifade = SonrakiİfadeyiStreamdenOku(reader)) != null)
                {
                    ifadeler.Add(ifade);
                }
            }

            return ifadeler.ToArray();
        }

        protected virtual string SonrakiİfadeyiStreamdenOku(StreamReader reader)
        {
            var sb = new StringBuilder();

            while (true)
            {
                var satır = reader.ReadLine();
                if (satır == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (satır.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(satır + Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion

        #region Methods

        public virtual void BağlantıFabrikasıBaşlat()
        {
            var connectionFactory = new SqlConnectionFactory();
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
            var DoğrulananTablolar = new[] { "Kullanıcılar", "İndirim", "Sipariş" };

            //custom commands (stored procedures, indexes)

            var özelKomutlar = new List<string>();
            özelKomutlar.AddRange(KomutlarıAyrıştır(GenelYardımcı.MapPath("~/App_Data/Install/SqlServer.Indexes.sql"), false));
            özelKomutlar.AddRange(KomutlarıAyrıştır(GenelYardımcı.MapPath("~/App_Data/Install/SqlServer.StoredProcedures.sql"), false));

            var başlatıcı = new CreateTablesIfNotExist<ObjectContext>(DoğrulananTablolar, özelKomutlar.ToArray());
            Database.SetInitializer(başlatıcı);
        }
        public virtual bool StoredProceduredDestekli
        {
            get { return true; }
        }
        public virtual bool BackupDestekli
        {
            get { return true; }
        }
        public virtual DbParameter ParametreAl()
        {
            return new SqlParameter();
        }
        public int DesteklenenBinaryHashUzunluğu()
        {
            return 8000; //for SQL Server 2008 and above HASHBYTES function has a limit of 8000 characters.
        }

        #endregion
    }
}