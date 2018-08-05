using System;

namespace Core.Domain.Medya
{
    public partial class Download : TemelVarlık
    {
        public Guid DownloadGuid { get; set; }

        public bool UseDownloadUrl { get; set; }

        public string DownloadUrl { get; set; }

        public byte[] DownloadBinary { get; set; }

        public string İçerikTipi { get; set; }

        public string DosyaAdı { get; set; }

        public string Uzantı { get; set; }

        public bool Yeni { get; set; }
    }
}
