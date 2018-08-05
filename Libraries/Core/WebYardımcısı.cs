using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Core.Data;
using Core.Altyapı;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.Net.Http.Headers;
using Core.Yapılandırma;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http.Features;

namespace Core
{
    public partial class WebYardımcısı : IWebYardımcısı
    {
        #region Fields 
        private const string NullIpAddress = "::1";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HostingAyarları _hostingAyarları;
        //private readonly HttpContextBase _httpContextAccessor;
        //private readonly string[] _sabitDosyaUzantıları;

        #endregion

        #region Constructor
        public WebYardımcısı(HostingAyarları hostingAyarları, IHttpContextAccessor httpContextAccessor)
        {
            this._hostingAyarları = hostingAyarları;
            this._httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Utilities

        protected virtual Boolean İstekErişilebilir()
        {
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                return false;

            try
            {
                if (_httpContextAccessor.HttpContext.Request == null)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        protected virtual bool WebConfigYazmayıDene()
        {
            try
            {
                // Orta güvenlikte, "UnloadAppDomain" desteklenmez.
                // AppDomaini yeniden başlatmayı deneyin.
                File.SetLastWriteTimeUtc(GenelYardımcı.MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected virtual bool IpAdresiAyarlandı(IPAddress address)
        {
            return address != null && address.ToString() != NullIpAddress;
        }

        #endregion

        #region Methods
        public virtual bool İstekYönlendirildi
        {
            get
            {
                var response = _httpContextAccessor.HttpContext.Response;
                //ASP.NET 4 style - return response.IsRequestBeingRedirected;
                int[] redirectionStatusCodes = { StatusCodes.Status301MovedPermanently, StatusCodes.Status302Found };
                return redirectionStatusCodes.Contains(response.StatusCode);
            }
        }

        public bool PostTamamlandı
        {
            get
            {
                if (_httpContextAccessor.HttpContext.Items["Rota.PostTamamlandı"] == null)
                    return false;
                return Convert.ToBoolean(_httpContextAccessor.HttpContext.Items["Rota.PostTamamlandı"]);
            }
            set
            {
                _httpContextAccessor.HttpContext.Items["Rota.PostTamamlandı"] = value;
            }
        }

        public virtual void AppDomainYenidenBaşlat(bool yönlendir = false)
        {

            //Orta sevşye güvenlik
            bool başarılı = WebConfigYazmayıDene();
            if (!başarılı)
            {
                throw new Hata("Yapılandırma değişikliği yüzünden yeniden başlatılmalı ancak bunu yapamadı." + Environment.NewLine +
                    "Gelecekte bu sorunu önlemek için web sunucusu yapılandırmasında bir değişiklik yapılması gerekiyor:" + Environment.NewLine +
                    "- Uygulamayı tam güven ortamında çalıştırın veya" + Environment.NewLine +
                    "- 'web.config' dosyasına uygulama yazma erişimi verin.");
            }
        }

        public virtual bool MevcutBağlantıGüvenli()
        {
            if (!İstekErişilebilir())
                return false;

            //check whether hosting uses a load balancer
            //use HTTP_CLUSTER_HTTPS?
            if (_hostingAyarları.UseHttpClusterHttps)
                return _httpContextAccessor.HttpContext.Request.Headers["HTTP_CLUSTER_HTTPS"].ToString().Equals("on", StringComparison.OrdinalIgnoreCase);

            //use HTTP_X_FORWARDED_PROTO?
            if (_hostingAyarları.UseHttpXForwardedProto)
                return _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-Proto"].ToString().Equals("https", StringComparison.OrdinalIgnoreCase);

            return _httpContextAccessor.HttpContext.Request.IsHttps;
        }

        public string MevcutIpAdresiAl()
        {
            if (!İstekErişilebilir())
                return string.Empty;

            var sonuç = "";
            try
            {
                if (_httpContextAccessor.HttpContext.Request.Headers != null)
                {
                    // Bir istemcinin kaynak IP adresini tanımlamak için
                    // Bir HTTP proxy veya yük dengeleyici aracılığıyla bir web sunucusuna bağlanmak gerekir.
                    var yönlendirilmişHttpHeader = "X-FORWARDED-FOR";
                    if (!String.IsNullOrEmpty(_hostingAyarları.ForwardedHttpHeader))
                    {
                        //Ancak bazı durumlarda sunucu, diğer HTTP header kullanır
                        //Bu durumlarda yönetici özel bir Yönlendirilmiş HTTP header belirleyebilir
                        //CF-Connecting-IP, X-FORWARDED-PROTO
                        yönlendirilmişHttpHeader = _hostingAyarları.ForwardedHttpHeader;
                    }
                    var forwardedHeader = _httpContextAccessor.HttpContext.Request.Headers[yönlendirilmişHttpHeader];
                    if (!StringValues.IsNullOrEmpty(forwardedHeader))
                        sonuç = forwardedHeader.FirstOrDefault();
                }
                if (string.IsNullOrEmpty(sonuç) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                    sonuç = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch
            {
                return sonuç;
            }

            //Bazı doğrulamalar
            if (sonuç == "::1")
                sonuç = "127.0.0.1";
            //portu sil
            if (!String.IsNullOrEmpty(sonuç))
            {
                int index = sonuç.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                    sonuç = sonuç.Substring(0, index);
            }
            return sonuç;
        }

        public bool SabitKaynak()
        {
            if (!İstekErişilebilir())
                return false;

            string yol = _httpContextAccessor.HttpContext.Request.Path;
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            return contentTypeProvider.TryGetContentType(yol, out string _);
        }

        public string SayfanınUrlsiniAl(bool sorguİçerir)
        {
            bool useSsl = MevcutBağlantıGüvenli();
            return SayfanınUrlsiniAl(sorguİçerir, useSsl);
        }

        public virtual string SayfanınUrlsiniAl(bool sorguİçerir, bool? SslKullan=null, bool lowercaseUrl = false)
        {
            if (!İstekErişilebilir())
                return string.Empty;

            if (!SslKullan.HasValue)
                SslKullan = MevcutBağlantıGüvenli();

            //Ana bilgisayarın SSL kullanmayı düşünmesini sağlayın
            var url = SiteHostAl(SslKullan.Value).TrimEnd('/');

            //Tam Urlyi al sorgu ile veya sorgusuz
            url += sorguİçerir ? HamUrlAl(_httpContextAccessor.HttpContext.Request)
                : $"{_httpContextAccessor.HttpContext.Request.PathBase}{_httpContextAccessor.HttpContext.Request.Path}";

            if(lowercaseUrl)
                url = url.ToLowerInvariant();

            return url.ToLowerInvariant();
        }

        public virtual string HamUrlAl(HttpRequest request)
        {
            var rawUrl = request.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;
            
            if (string.IsNullOrEmpty(rawUrl))
                rawUrl = $"{request.PathBase}{request.Path}{request.QueryString}";

            return rawUrl;
        }

        public string SiteHostAl(bool SslKullan)
        {
            var sonuç = "";
            var hostHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
            if (!StringValues.IsNullOrEmpty(hostHeader))
                sonuç = "http://" + hostHeader.FirstOrDefault();

            if (DataAyarlarıYardımcısı.DatabaseYüklendi())
            {
                #region Database yüklendi

                //let's resolve IWorkContext  here.
                //Do not inject it via constructor  because it'll cause circular references
                var siteContext = EngineContext.Current.Resolve<ISiteContext>();
                var mevcutSite = siteContext.MevcutSite;
                if (mevcutSite == null)
                    throw new Exception("Mevcut site yüklenemedi");

                if (String.IsNullOrWhiteSpace(sonuç))
                {
                    //HTTP_HOST değişkeni erişilemez durumda
                    //Bu senaryo, yalnızca HttpContext kullanılabilir olmadığında (örneğin, bir zamanlama görevinde çalışırken) mümkündür.
                    //Bu durumda yönetici alanında yapılandırılmış bir site öğesinin URL'sini kullanın
                    sonuç = mevcutSite.Url;
                }

                if (SslKullan)
                {
                    sonuç = !String.IsNullOrWhiteSpace(mevcutSite.GüvenliUrl) ?
                        mevcutSite.GüvenliUrl :
                        sonuç.Replace("http:/", "https:/");
                }
                else
                {
                    if (mevcutSite.SslEtkin && !String.IsNullOrWhiteSpace(mevcutSite.GüvenliUrl))
                    {
                        sonuç = mevcutSite.Url;
                    }
                }
                #endregion
            }
            else
            {
                #region Database yüklenmedi
                if (!string.IsNullOrEmpty(sonuç) && SslKullan)
                {
                    sonuç = sonuç.Replace("http:/", "https:/");
                }
                #endregion
            }

            if (!sonuç.EndsWith("/"))
                sonuç += "/";
            return sonuç.ToLowerInvariant();
        }

        public string SiteKonumuAl(bool? SslKullan)
        {
            if (!SslKullan.HasValue)
                SslKullan = MevcutBağlantıGüvenli();
            var host = SiteHostAl(SslKullan.Value).TrimEnd('/');
            if (İstekErişilebilir())
                host += _httpContextAccessor.HttpContext.Request.PathBase;

            if (!host.EndsWith("/"))
                host += "/";

            return host;
        }

        public T Sorgu<T>(string ad)
        {
            if (!İstekErişilebilir())
                return default(T);

            if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Query[ad]))
                return default(T);

            return GenelYardımcı.To<T>(_httpContextAccessor.HttpContext.Request.Query[ad].ToString());
        }

        public string SorguDeğiştir(string url, string sorguDeğiştirme, string anchor)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (sorguDeğiştirme == null)
                sorguDeğiştirme = string.Empty;
            sorguDeğiştirme = sorguDeğiştirme.ToLowerInvariant();

            if (anchor == null)
                anchor = string.Empty;
            anchor = anchor.ToLowerInvariant();


            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(sorguDeğiştirme))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2)
                            {
                                if (!dictionary.ContainsKey(strArray[0]))
                                {
                                    dictionary[strArray[0]] = strArray[1];
                                }
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in sorguDeğiştirme.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            string[] strArray2 = str4.Split(new[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = sorguDeğiştirme;
                }
            }
            if (!string.IsNullOrEmpty(anchor))
            {
                str2 = anchor;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2))).ToLowerInvariant();
        }

        public string SorguSil(string url, string sorgu)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (sorgu == null)
                sorgu = string.Empty;
            sorgu = sorgu.ToLowerInvariant();


            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(sorgu))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(sorgu);

                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }

        public string UrlYönlendiriciAl()
        {
            if (!İstekErişilebilir())
                return string.Empty;
            return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
        }

        public virtual bool LocalIstek(HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (IpAdresiAyarlandı(connection.RemoteIpAddress))
            {
                return IpAdresiAyarlandı(connection.LocalIpAddress)
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return true;
        }
        #endregion
    }
}
