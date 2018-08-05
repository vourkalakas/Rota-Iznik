using System;
using System.Collections.Generic;

namespace Services.KimlikDoğrulama.Harici
{
    [Serializable]
    public partial class HariciYetkilendirmeParametreleri
    {
        public HariciYetkilendirmeParametreleri()
        {
            Claims = new List<ExternalAuthenticationClaim>();
        }
        public string ProviderSystemName { get; set; }
        public string ExternalIdentifier { get; set; }
        public string ExternalDisplayIdentifier { get; set; }
        public string AccessToken { get; set; }
        public string Email { get; set; }
        public IList<ExternalAuthenticationClaim> Claims { get; set; }
    }
    [Serializable]
    public class ExternalAuthenticationClaim
    {
        public ExternalAuthenticationClaim(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}