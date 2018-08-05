using Web.Models.Genel;
namespace Web.Fabrika
{
    public partial interface IGenelModelFactory
    {
        /*
        LogoModel PrepareLogoModel();
        LanguageSelectorModel PrepareLanguageSelectorModel();
        CurrencySelectorModel PrepareCurrencySelectorModel();
        TaxTypeSelectorModel PrepareTaxTypeSelectorModel();
        
        */
        AdminHeaderLinksModel PrepareAdminHeaderLinksModel();
        HeaderLinksModel PrepareHeaderLinksModel();
        /*
        SocialModel PrepareSocialModel();
        FooterModel PrepareFooterModel();
        ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties);
        ContactVendorModel PrepareContactVendorModel(ContactVendorModel model, Vendor vendor,
            bool excludeProperties);
        SitemapModel PrepareSitemapModel();
        string PrepareSitemapXml(int? id);
        StoreThemeSelectorModel PrepareStoreThemeSelectorModel();
        FaviconModel PrepareFaviconModel();
        string PrepareRobotsTextFile();
        */
    }
}
