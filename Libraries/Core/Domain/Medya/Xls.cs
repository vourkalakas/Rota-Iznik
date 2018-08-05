namespace Core.Domain.Medya
{
    public partial class Xls : TemelVarlık
    {
        public byte[] XlsBinary { get; set; }
        public string MimeTipi { get; set; }
        public string SeoDosyaAdı { get; set; }
        public string AltÖznitelik { get; set; }
        public string BaşlıkÖznitelik { get; set; }
        public bool Yeni { get; set; }
    }
}
