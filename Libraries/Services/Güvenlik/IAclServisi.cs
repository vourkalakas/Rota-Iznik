using Core;
using Core.Domain.Güvenlik;
using Core.Domain.Kullanıcılar;
using System.Collections.Generic;

namespace Services.Güvenlik
{
    public partial interface IAclServisi
    {
        void AclKaydıSil(AclKaydı aclKaydı);

        AclKaydı AclKaydıAlId(int aclRecordId);

        IList<AclKaydı> TümAclKayıtları<T>(T varlık) where T : TemelVarlık, IAclDestekli;

        void AclKaydıEkle(AclKaydı aclKaydı);

        void AclKaydıEkle<T>(T varlık, int kullanıcıRolId) where T : TemelVarlık, IAclDestekli;

        void AclKaydıGüncelle(AclKaydı aclKaydı);

        int[] ErişimİleKullanıcıRolIdleriAl<T>(T varlık) where T : TemelVarlık, IAclDestekli;

        bool YetkiVer<T>(T varlık) where T : TemelVarlık, IAclDestekli;

        bool YetkiVer<T>(T varlık, Kullanıcı kullanıcı) where T : TemelVarlık, IAclDestekli;
    }
}
