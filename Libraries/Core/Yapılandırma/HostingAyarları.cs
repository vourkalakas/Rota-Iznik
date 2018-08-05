namespace Core.Yapılandırma
{
    public partial class HostingAyarları
    {
        public string ForwardedHttpHeader { get; set; }
        public bool UseHttpClusterHttps { get; set; }
        public bool UseHttpXForwardedProto { get; set; }
    }
}
