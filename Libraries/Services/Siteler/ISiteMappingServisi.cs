using Core;
using Core.Domain.Siteler;
using System.Collections.Generic;

namespace Services.Siteler
{
    public partial interface ISiteMappingServisi
    {
        void SiteMappingSil(SiteMapping siteMapping);

        SiteMapping SiteMappingAlId(int siteMappingId);

        IList<SiteMapping> SiteMappingleriAl<T>(T varlık) where T : TemelVarlık, ISiteMappingDestekli;

        void SiteMappingEkle(SiteMapping siteMapping);

        void SiteMappingEkle<T>(T entity, int siteId) where T : TemelVarlık, ISiteMappingDestekli;

        void SiteMappingGüncelle(SiteMapping siteMapping);

        int[] ErişimİleSiteMappingleriAl<T>(T varlık) where T : TemelVarlık, ISiteMappingDestekli;

        bool YetkiVer<T>(T varlık) where T : TemelVarlık, ISiteMappingDestekli;

        bool YetkiVer<T>(T varlık, int siteId) where T : TemelVarlık, ISiteMappingDestekli;
    }
}
