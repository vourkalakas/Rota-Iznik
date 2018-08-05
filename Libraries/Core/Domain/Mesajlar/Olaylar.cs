using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Mesajlar
{
    #region EmailAboneOlduOlayı
    public class EmailAboneOlduOlayı
    {
        private readonly BültenAboneliği _bültenAboneliği;
        public EmailAboneOlduOlayı(BültenAboneliği bültenAboneliği)
        {
            this._bültenAboneliği = bültenAboneliği;
        }
        public BültenAboneliği AboneOl
        {
            get { return _bültenAboneliği; }
        }
        public bool Equals(EmailAboneOlduOlayı diğer)
        {
            if (ReferenceEquals(null, diğer)) return false;
            if (ReferenceEquals(this, diğer)) return true;
            return Equals(diğer._bültenAboneliği, _bültenAboneliği);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(EmailAboneOlduOlayı)) return false;
            return Equals((EmailAboneOlduOlayı)obj);
        }
        public override int GetHashCode()
        {
            return (_bültenAboneliği != null ? _bültenAboneliği.GetHashCode() : 0);
        }
    }
    #endregion
    #region EmailAboneliktenAyrıldıOlayı
    public class EmailAboneliktenAyrıldıOlayı
    {
        private readonly BültenAboneliği _bültenAboneliği;
        public EmailAboneliktenAyrıldıOlayı(BültenAboneliği bültenAboneliği)
        {
            this._bültenAboneliği = bültenAboneliği;
        }
        public BültenAboneliği AboneliktenAyrıl
        {
            get { return _bültenAboneliği; }
        }
        public bool Equals(EmailAboneliktenAyrıldıOlayı diğer)
        {
            if (ReferenceEquals(null, diğer)) return false;
            if (ReferenceEquals(this, diğer)) return true;
            return Equals(diğer._bültenAboneliği, _bültenAboneliği);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(EmailAboneliktenAyrıldıOlayı)) return false;
            return Equals((EmailAboneliktenAyrıldıOlayı)obj);
        }
        public override int GetHashCode()
        {
            return (_bültenAboneliği != null ? _bültenAboneliği.GetHashCode() : 0);
        }
    }
    #endregion
    #region VarlıkTokenEklendiOlayı
    public class VarlıkTokenEklendiOlayı<T, U> where T : TemelVarlık
    {
        private readonly T _varlık;
        private readonly IList<U> _token;

        public VarlıkTokenEklendiOlayı(T varlık, IList<U> token)
        {
            _varlık = varlık;
            _token = token;
        }

        public T Varlık { get { return _varlık; } }
        public IList<U> Token { get { return _token; } }
    }
    #endregion
    #region MesajTokenEklendiOlayı
    public class MesajTokenEklendiOlayı<U>
    {
        private readonly MesajTeması _mesaj;
        private readonly IList<U> _token;

        public MesajTokenEklendiOlayı(MesajTeması mesaj, IList<U> token)
        {
            _mesaj = mesaj;
            _token = token;
        }

        public MesajTeması Mesaj { get { return _mesaj; } }
        public IList<U> Token { get { return _token; } }
    }
    #endregion
    #region EkTokenEklendiOlayı
    public class EkTokenEklendiOlayı
    {
        private readonly IList<string> _token;

        public EkTokenEklendiOlayı()
        {
            this._token = new List<string>();
        }

        public void TokenEkle(params string[] ekTokenler)
        {
            foreach (var ekToken in ekTokenler)
            {
                this._token.Add(ekToken);
            }
        }

        public IList<string> ekTokenler { get { return _token; } }
    }
    #endregion
    #region KampanyaEkTokenEklendiOlayı
    public class KampanyaEkTokenEklendiOlayı : EkTokenEklendiOlayı
    {
    }
    #endregion
}
