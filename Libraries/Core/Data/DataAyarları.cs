using System;
using System.Collections.Generic;

namespace Core.Data
{
    public partial class DataAyarları
    {
        public DataAyarları()
        {
            HamDataAyarları = new Dictionary<string, string>();
        }
        public string DataSağlayıcı { get; set; }
        public string DataConnectionString { get; set; }
        public IDictionary<string, string> HamDataAyarları { get; private set; }
        public bool Geçerli()
        {
            return !String.IsNullOrEmpty(this.DataSağlayıcı) && !String.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
