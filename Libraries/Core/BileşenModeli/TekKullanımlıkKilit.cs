using System.Threading;
using System;
namespace Core.BileşenModeli
{
    public class TekKullanımlıkKilit : IDisposable
    {
        private readonly ReaderWriterLockSlim _rwKilit;
        public TekKullanımlıkKilit(ReaderWriterLockSlim rwKilit)
        {
            _rwKilit = rwKilit;
            _rwKilit.EnterWriteLock();
        }
        void IDisposable.Dispose()
        {
            _rwKilit.ExitWriteLock();
        }
    }
}
