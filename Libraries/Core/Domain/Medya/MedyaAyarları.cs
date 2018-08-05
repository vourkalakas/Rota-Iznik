using Core.Yapılandırma;

namespace Core.Domain.Medya
{
    public class MedyaAyarları : IAyarlar
    {
        public bool ResimVeritabanındaDepola { get; set; }
        public int AvatarResimBoyutu { get; set; }
        public int IliskilendirilmisResimBoyutu { get; set; }
        public int KategoriThumbResimBoyutu { get; set; }
        public int AutoCompleteAramaThumbResimBoyutu { get; set; }
        public int GörüntüKareResimBoyutu { get; set; }
        public bool VarsayılanResimZoomEtkin { get; set; }
        public int MaksimumResimBoyutu { get; set; }
        public int VarsayılanResimKalitesi { get; set; }
        public bool CokluThumbKlasorleri { get; set; }
        public string AzureOnbellekControlBasligi { get; set; }
    }
}
