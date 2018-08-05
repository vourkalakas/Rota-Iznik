using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Core;
using Core.Data;


namespace Data
{
    public partial class EfDepo<T> : IDepo<T> where T : TemelVarlık
    {
        #region Fields

        private readonly IDbContext _context;
        private IDbSet<T> _varlıklar;

        #endregion

        #region Ctor
        public EfDepo(IDbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Utilities
        protected string TamHataMesajınıAl(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach (var DoğrulamaHataları in exc.EntityValidationErrors)
                foreach (var error in DoğrulamaHataları.ValidationErrors)
                    msg += string.Format("Property: {0} Error: {1}", error.PropertyName, error.ErrorMessage) + Environment.NewLine;
            return msg;
        }

        #endregion

        #region Methods
        public virtual T AlId(object id)
        {
            return this.Varlıklar.Find(id);
        }
        public virtual void Ekle(T varlık)
        {
            try
            {
                if (varlık == null)
                    throw new ArgumentNullException("varlık");

                this.Varlıklar.Add(varlık);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(TamHataMesajınıAl(dbEx), dbEx);
            }
        }
        public virtual void Ekle(IEnumerable<T> varlıklar)
        {
            try
            {
                if (varlıklar == null)
                    throw new ArgumentNullException("varlıklar");

                foreach (var varlık in varlıklar)
                    this.Varlıklar.Add(varlık);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(TamHataMesajınıAl(dbEx), dbEx);
            }
        }
        public virtual void Güncelle(T varlık)
        {
            try
            {
                if (varlık == null)
                    throw new ArgumentNullException("varlık");

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(TamHataMesajınıAl(dbEx), dbEx);
            }
        }
        public virtual void Güncelle(IEnumerable<T> varlıklar)
        {
            try
            {
                if (varlıklar == null)
                    throw new ArgumentNullException("varlıklar");

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(TamHataMesajınıAl(dbEx), dbEx);
            }
        }
        public virtual void Sil(T varlık)
        {
            try
            {
                if (varlık == null)
                    throw new ArgumentNullException("varlık");

                this.Varlıklar.Remove(varlık);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(TamHataMesajınıAl(dbEx), dbEx);
            }
        }
        public virtual void Sil(IEnumerable<T> varlıklar)
        {
            try
            {
                if (varlıklar == null)
                    throw new ArgumentNullException("varlıklar");

                foreach (var varlık in varlıklar)
                    this.Varlıklar.Remove(varlık);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(TamHataMesajınıAl(dbEx), dbEx);
            }
        }

        #endregion

        #region Properties
        public virtual IQueryable<T> Tablo
        {
            get
            {
                return this.Varlıklar;
            }
        }
        public virtual IQueryable<T> Tabloİzlemesiz
        {
            get
            {
                return this.Varlıklar.AsNoTracking();
            }
        }
        protected virtual IDbSet<T> Varlıklar
        {
            get
            {
                if (_varlıklar == null)
                    _varlıklar = _context.Ayarla<T>();
                return _varlıklar;
            }
        }

        #endregion
    }
}
