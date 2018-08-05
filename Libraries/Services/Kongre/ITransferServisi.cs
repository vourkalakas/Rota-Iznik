using Core.Domain.Kongre;
using System.Collections.Generic;

namespace Services.Kongre
{
    public partial interface ITransferServisi
    {
        void TransferSil(Transfer transfer);
        Transfer TransferAlId(int transferId);
        IList<Transfer> TümTransferAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        IList<Transfer> TransferAlKongreId(int kongreId,bool AclYoksay = false, bool gizliOlanlarıGöster = false);
        void TransferEkle(Transfer transfer);
        void TransferGüncelle(Transfer transfer);
    }

}
