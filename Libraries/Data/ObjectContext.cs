using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Core;
using Data.Mapping;

namespace Data
{
    public class ObjectContext : DbContext, IDbContext
    {
        #region Ctor

        public ObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            ((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }

        #endregion

        #region Utilities

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Tüm yapılandırmayı dinamik olarak yükle
            //System.Type configType = typeof(LanguageMap);   //Yapılandırma sınıflarından herhangi biri burada
            //var typesToRegister = Assembly.GetAssembly(configType).GetTypes()

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(TSVarlıkTipiYapılandırması<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }
        protected virtual TEntity ContexteVarlıkEkle<TEntity>(TEntity varlık) where TEntity : TemelVarlık, new()
        {
            var zatenEklendi = Set<TEntity>().Local.FirstOrDefault(x => x.Id == varlık.Id);
            if (zatenEklendi == null)
            {
                //yeni varlık ekl
                Set<TEntity>().Attach(varlık);
                return varlık;
            }

            //varlık zaten yüklü
            return zatenEklendi;
        }

        #endregion

        #region Methods
        public string VeritabanıScriptiOluştur()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }
        public new IDbSet<TEntity> Ayarla<TEntity>() where TEntity : TemelVarlık
        {
            return base.Set<TEntity>();
        }
        public IList<TEntity> StoredProcedureListesiniÇalıştır<TEntity>(string komut, params object[] parametreler) where TEntity : TemelVarlık, new()
        {
            //komuta parametre ekle
            if (parametreler != null && parametreler.Length > 0)
            {
                for (int i = 0; i <= parametreler.Length - 1; i++)
                {
                    var p = parametreler[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Desteklenmeyen parametre tipi");

                    komut += i == 0 ? " " : ", ";

                    komut += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //parametre çıktısı
                        komut += " çıktı";
                    }
                }
            }

            var sonuç = this.Database.SqlQuery<TEntity>(komut, parametreler).ToList();

            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;

                for (int i = 0; i < sonuç.Count; i++)
                    sonuç[i] = ContexteVarlıkEkle(sonuç[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }

            return sonuç;
        }
        public IEnumerable<TElement> SqlSorgusu<TElement>(string sql, params object[] parametreler)
        {
            return this.Database.SqlQuery<TElement>(sql, parametreler);
        }
        public int SqlKomutunuÇalıştır(string sql, bool İşlemiTeyitEt = false, int? zamanAşımı = null, params object[] parametreler)
        {
            int? öncekiZamanAşımı = null;
            if (zamanAşımı.HasValue)
            {
                //site önceki zamanAşımı
                öncekiZamanAşımı = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = zamanAşımı;
            }

            var işlemselDavranış = İşlemiTeyitEt
                ? TransactionalBehavior.DoNotEnsureTransaction
                : TransactionalBehavior.EnsureTransaction;
            var sonuç = this.Database.ExecuteSqlCommand(işlemselDavranış, sql, parametreler);

            if (zamanAşımı.HasValue)
            {
                //Önceki zamanaşımı ayarla
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = öncekiZamanAşımı;
            }

            //sonucu döndür
            return sonuç;
        }
        public void Ayır(object varlık)
        {
            if (varlık == null)
                throw new ArgumentNullException("varlık");

            ((IObjectContextAdapter)this).ObjectContext.Detach(varlık);
        }

        #endregion

        #region Properties
        public virtual bool ProxyOluşturmaEtkin
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }
        public virtual bool DeğişiklikleriOtomatikAlgılamaEtkin
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }

        #endregion
    }
}
