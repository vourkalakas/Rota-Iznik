using System.Collections;

namespace Web.Framework.Kendoui
{
    public class DataSourceSonucu
    {
        public object EkstraData { get; set; }

        public IEnumerable Data { get; set; }

        public object Hatalar { get; set; }

        public int Toplam { get; set; }
    }
}
