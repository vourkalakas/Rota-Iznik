using System.Collections.Generic;
using Core.Domain.Siteler;

namespace Services.Siteler
{
    public partial interface ISiteServisi
    {
        void SiteSil(Site Site);
        IList<Site> TümSiteler();
        Site SiteAlId(int SiteId);
        void SiteEkle(Site Site);
        void SiteGüncelle(Site Site);
    }
}
