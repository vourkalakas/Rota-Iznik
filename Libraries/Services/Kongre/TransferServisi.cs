using Core;
using Core.Data;
using Core.Domain.Kongre;
using Core.Önbellek;
using Services.Olaylar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Kongre
{
    public class TransferServisi : ITransferServisi
    {
        private const string TRANSFER_ALL_KEY = "transfer.all-{0}-{1}";
        private const string TRANSFER_BY_ID_KEY = "transfer.id-{0}";
        private const string TRANSFER_PATTERN_KEY = "transfer.";
        private readonly IWorkContext _workContext;
        private readonly IOlayYayınlayıcı _olayYayınlayıcı;
        private readonly IÖnbellekYönetici _önbellekYönetici;
        private readonly IDepo<Transfer> _transferDepo;
        public TransferServisi(IDepo<Transfer> transferDepo,
        IWorkContext workContext,
        IOlayYayınlayıcı olayYayınlayıcı,
        IÖnbellekYönetici önbellekYönetici)
        {
            this._transferDepo = transferDepo;
            this._workContext = workContext;
            this._olayYayınlayıcı = olayYayınlayıcı;
            this._önbellekYönetici = önbellekYönetici;
        }
        public Transfer TransferAlId(int transferId)
        {
            if (transferId == 0)
                return null;

            string key = string.Format(TRANSFER_BY_ID_KEY, transferId);
            return _önbellekYönetici.Al(key, () => _transferDepo.AlId(transferId));
        }

        public void TransferEkle(Transfer transfer)
        {
            if (transfer == null)
                throw new ArgumentNullException("transfer");

            _transferDepo.Ekle(transfer);
            _önbellekYönetici.KalıpİleSil(TRANSFER_PATTERN_KEY);
            _olayYayınlayıcı.OlayEklendi(transfer);
        }

        public void TransferGüncelle(Transfer transfer)
        {
            if (transfer == null)
                throw new ArgumentNullException("transfer");

            _transferDepo.Güncelle(transfer);
            _önbellekYönetici.KalıpİleSil(TRANSFER_PATTERN_KEY);
            _olayYayınlayıcı.OlayGüncellendi(transfer);
        }

        public void TransferSil(Transfer transfer)
        {
            if (transfer == null)
                throw new ArgumentNullException("transfer");

            _transferDepo.Sil(transfer);
            _önbellekYönetici.KalıpİleSil(TRANSFER_PATTERN_KEY);
            _olayYayınlayıcı.OlaySilindi(transfer);
        }

        public IList<Transfer> TümTransferAl(bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TRANSFER_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _transferDepo.Tablo;
                return query.ToList();
            });
        }
        public IList<Transfer> TransferAlKongreId(int kongreId,bool AclYoksay = false, bool gizliOlanlarıGöster = false)
        {
            string key = string.Format(TRANSFER_ALL_KEY, AclYoksay, gizliOlanlarıGöster);
            return _önbellekYönetici.Al(key, () =>
            {
                var query = _transferDepo.Tablo.Where(x => x.KongreId == kongreId);
                return query.ToList();
            });
        }
    }

}
