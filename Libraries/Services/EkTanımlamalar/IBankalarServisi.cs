using Core.Domain.EkTanımlamalar;
using System.Collections.Generic;

namespace Services.EkTanımlamalar
{
    public partial interface IBankalarServisi
    {
        void BankaSil(Banka banka);
        Banka BankaAlId(int bankaId);
        IList<Banka> TümBankalarıAl( bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void BankaEkle(Banka banka);
        void BankaGüncelle(Banka banka);
    }
}
