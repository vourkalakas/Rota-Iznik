using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BundlerMinifier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Core;
using Core.Önbellek;
using Core.Domain.Seo;
using Services.Seo;

namespace Web.Framework.UI
{
    public partial class SayfaHeadOluşturucu : ISayfaHeadOluşturucu
    {
        #region Fields

        private static readonly object s_lock = new object();

        private readonly SeoAyarları _seoAyarları;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStatikÖnbellekYönetici _cacheManager;
        private BundleFileProcessor _processor;
        private readonly List<string> _baslikParçaları;
        private readonly List<string> _metaDescriptionParçaları;
        private readonly List<string> _metaKeywordParçaları;
        private readonly Dictionary<KaynakKonumu, List<ScriptReferenceMeta>> _scriptParçaları;
        private readonly Dictionary<KaynakKonumu, List<string>> _inlineScriptParçası;
        private readonly Dictionary<KaynakKonumu, List<CssReferenceMeta>> _cssParçaları;
        private readonly List<string> _canonicalUrlParçaları;
        private readonly List<string> _özelHeadParçaları;
        private readonly List<string> _sayfaCssSınıfıParçaları;
        private string _düzenleSayfaUrlsi;
        private string _aktifAdminMenuSistemAdı;
        private const int RecheckBundledFilesPeriod = 120;
        #endregion

        #region Ctor

        public SayfaHeadOluşturucu(SeoAyarları seoAyarları,
            IHostingEnvironment hostingEnvironment,
            IStatikÖnbellekYönetici cacheManager)
        {
            this._seoAyarları = seoAyarları;
            this._baslikParçaları = new List<string>();
            this._metaDescriptionParçaları = new List<string>();
            this._metaKeywordParçaları = new List<string>();
            this._scriptParçaları = new Dictionary<KaynakKonumu, List<ScriptReferenceMeta>>();
            this._cssParçaları = new Dictionary<KaynakKonumu, List<CssReferenceMeta>>();
            this._canonicalUrlParçaları = new List<string>();
            this._özelHeadParçaları = new List<string>();
            this._sayfaCssSınıfıParçaları = new List<string>();
            this._hostingEnvironment = hostingEnvironment;
            this._cacheManager = cacheManager;
            this._processor = new BundleFileProcessor();
        }

        #endregion

        #region Utilities

        protected virtual string PaketSanalYolunuAl(string[] parçalar)
        {
            if (parçalar == null || parçalar.Length == 0)
                throw new ArgumentException("parts");

            //calculate hash
            var hash = "";
            using (SHA256 sha = new SHA256Managed())
            {
                // string concatenation
                var hashInput = "";
                foreach (var part in parçalar)
                {
                    hashInput += part;
                    hashInput += ",";
                }

                var input = sha.ComputeHash(Encoding.Unicode.GetBytes(hashInput));
                hash = WebEncoders.Base64UrlEncode(input);
            }
            //ensure only valid chars
            hash = SeoUzantıları.SeAdıAl(hash);

            return hash;
        }

        #endregion

        #region Methods

        public virtual void BaşlıkParçasıEkle(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _baslikParçaları.Add(parça);
        }
        public virtual void BaşlıkParçasıIlaveEt(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _baslikParçaları.Insert(0, parça);
        }

        public virtual string BaşlıkOluştur(bool varsayılanBaşlığıEkle)
        {
            string sonuç = "";
            
            var özelBaşlık = string.Join(_seoAyarları.SayfaBaşlığıAyırıcısı, _baslikParçaları.AsEnumerable().Reverse().ToArray());
            if (!String.IsNullOrEmpty(özelBaşlık))
            {
                if (varsayılanBaşlığıEkle)
                {
                    //site adı + sayfa başlığı
                    switch (_seoAyarları.SayfaBaşlığıSeoAyarı)
                    {
                        case SayfaBaşlığıSeoAyarı.SayfaAdıSonraSiteAdı:
                            {
                                sonuç = string.Join(_seoAyarları.SayfaBaşlığıAyırıcısı, _seoAyarları.VarsayılanBaşlık, özelBaşlık);
                            }
                            break;
                        case SayfaBaşlığıSeoAyarı.SiteAdıSonraSayfaAdı:
                        default:
                            {
                                sonuç = string.Join(_seoAyarları.SayfaBaşlığıAyırıcısı, özelBaşlık, _seoAyarları.VarsayılanBaşlık);
                            }
                            break;
                    }
                }
                else
                {
                    //sadece sayfa balığı
                    sonuç = özelBaşlık;
                }
            }
            else
            {
                //sadece site adı
                sonuç = _seoAyarları.VarsayılanBaşlık;
            }
            return sonuç;
        }

        public virtual void MetaDescriptionParçasıEkle(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _metaDescriptionParçaları.Add(parça);
        }
        public virtual void MetaDescriptionParçasıIlaveEt(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _metaDescriptionParçaları.Insert(0, parça);
        }
        public virtual string MetaDescriptionOluştur()
        {
            var metaDescription = string.Join(", ", _metaDescriptionParçaları.AsEnumerable().Reverse().ToArray());
            var sonuç = !String.IsNullOrEmpty(metaDescription) ? metaDescription : "";
            return sonuç;
        }


        public virtual void MetaKeywordParçasıEkle(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _metaKeywordParçaları.Add(parça);
        }
        public virtual void MetaKeywordParçasıIlaveEt(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _metaKeywordParçaları.Insert(0, parça);
        }
        public virtual string MetaKeywordsOluştur()
        {
            var metaKeyword = string.Join(", ", _metaKeywordParçaları.AsEnumerable().Reverse().ToArray());
            var sonuç = !String.IsNullOrEmpty(metaKeyword) ? metaKeyword : "";
            return sonuç;
        }


        public virtual void ScriptParçasıEkle(KaynakKonumu konum, string parça, string debugSrc, bool pakettinDışında, bool isAsync)
        {
            if (!_scriptParçaları.ContainsKey(konum))
                _scriptParçaları.Add(konum, new List<ScriptReferenceMeta>());

            if (string.IsNullOrEmpty(parça))
                return;

            if (string.IsNullOrEmpty(debugSrc))
                debugSrc = parça;

            _scriptParçaları[konum].Add(new ScriptReferenceMeta
            {
                pakettinDışında = pakettinDışında,
                IsAsync = isAsync,
                Parça = parça,
                DebugSrc = debugSrc
            });
        }
        public virtual void ScriptParçasıIlaveEt(KaynakKonumu konum, string parça, string debugSrc, bool pakettinDışında, bool isAsync)
        {
            if (!_scriptParçaları.ContainsKey(konum))
                _scriptParçaları.Add(konum, new List<ScriptReferenceMeta>());

            if (string.IsNullOrEmpty(parça))
                return;

            if (string.IsNullOrEmpty(debugSrc))
                debugSrc = parça;

            _scriptParçaları[konum].Insert(0, new ScriptReferenceMeta
            {
                pakettinDışında = pakettinDışında,
                IsAsync = isAsync,
                Parça = parça,
                DebugSrc = debugSrc
            });
        }
        public virtual string ScriptsOluştur(IUrlHelper urlHelper, KaynakKonumu konum, bool? paketDosyaları = null)
        {
            if (!_scriptParçaları.ContainsKey(konum) || _scriptParçaları[konum] == null)
                return "";

            if (!_scriptParçaları.Any())
                return "";

            var debugModel = _hostingEnvironment.IsDevelopment();

            if (!paketDosyaları.HasValue)
            {
                //Herhangi bir değer belirtilmemişse ayarı kullanın
                paketDosyaları = _seoAyarları.JSPaketlemeyeIzinVer;
            }

            if (paketDosyaları.Value)
            {
                var pakettenParçalar = _scriptParçaları[konum]
                    .Where(x => !x.pakettinDışında)
                    .Distinct()
                    .ToArray();
                var paketinDışındakiParçalar = _scriptParçaları[konum]
                    .Where(x => x.pakettinDışında)
                    .Distinct()
                    .ToArray();


                var sonuç = new StringBuilder();

                if (pakettenParçalar.Length > 0)
                {
                    //paket oluştur
                    Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "bundles"));

                    var bundle = new Bundle();
                    foreach (var item in pakettenParçalar)
                    {
                        new PathString(urlHelper.Content(debugModel ? item.DebugSrc : item.Parça))
                            .StartsWithSegments(urlHelper.ActionContext.HttpContext.Request.PathBase, out PathString path);
                        var src = path.Value.TrimStart('/');

                        //check whether this file exists, if not it should be stored into /wwwroot directory
                        if (!File.Exists(Path.Combine(_hostingEnvironment.ContentRootPath, src.Replace("/", "\\"))))
                            src = $"wwwroot/{src}";

                        bundle.InputFiles.Add(src);
                    }
                    //output file
                    var outputFileName = PaketSanalYolunuAl(pakettenParçalar.Select(x => { return debugModel ? x.DebugSrc : x.Parça; }).ToArray());
                    bundle.OutputFileName = "wwwroot/bundles/" + outputFileName + ".js";
                    //save
                    var configFilePath = _hostingEnvironment.ContentRootPath + "\\" + outputFileName + ".json";
                    bundle.FileName = configFilePath;
                    lock (s_lock)
                    {
                        var cacheKey = $"Rota.minification.shouldrebuild.css-{outputFileName}";
                        var shouldRebuild = _cacheManager.Al<bool>(cacheKey, RecheckBundledFilesPeriod, () => true);
                        if (shouldRebuild)
                        {
                            //store json file to see a generated config file (for debugging purposes)
                            //BundleHandler.AddBundle(configFilePath, bundle);

                            //process
                            _processor.Process(configFilePath, new List<Bundle> { bundle });
                            _cacheManager.Ayarla(cacheKey, false, RecheckBundledFilesPeriod);
                        }
                    }
                    sonuç.AppendFormat("<script src=\"{0}\" type=\"{1}\"></script>", urlHelper.Content("~/bundles/" + outputFileName + ".min.js"), MimeTipleri.TextJavascript);
                    sonuç.Append(Environment.NewLine);
                }

                //paketten olmayan parçalar
                foreach (var item in paketinDışındakiParçalar)
                {
                    var src = debugModel ? item.DebugSrc : item.Parça;
                    sonuç.AppendFormat("<script {2}src=\"{0}\" type=\"{1}\"></script>", urlHelper.Content(src), MimeTipleri.TextJavascript, item.IsAsync ? "async " : "");
                    sonuç.Append(Environment.NewLine);
                }
                return sonuç.ToString();
            }
            else
            {
                //bundling is disabled
                var sonuç = new StringBuilder();
                foreach (var item in _scriptParçaları[konum].Distinct())
                {
                    var src = debugModel ? item.DebugSrc : item.Parça;
                    sonuç.AppendFormat("<script {2}src=\"{0}\" type=\"{1}\"></script>", urlHelper.Content(src), MimeTipleri.TextJavascript, item.IsAsync ? "async " : "");
                    sonuç.Append(Environment.NewLine);
                }
                return sonuç.ToString();
            }
        }
        public virtual void InlineScriptParçasıEkle(KaynakKonumu konum, string script)
        {
            if (!_inlineScriptParçası.ContainsKey(konum))
                _inlineScriptParçası.Add(konum, new List<string>());

            if (string.IsNullOrEmpty(script))
                return;

            _inlineScriptParçası[konum].Add(script);
        }
        public virtual void InlineScriptParçasıIlaveEt(KaynakKonumu konum, string script)
        {
            if (!_inlineScriptParçası.ContainsKey(konum))
                _inlineScriptParçası.Add(konum, new List<string>());

            if (string.IsNullOrEmpty(script))
                return;

            _inlineScriptParçası[konum].Insert(0, script);
        }

        public virtual string InlineScriptsOluştur(IUrlHelper urlHelper, KaynakKonumu konum)
        {
            if (!_inlineScriptParçası.ContainsKey(konum) || _inlineScriptParçası[konum] == null)
                return "";

            if (!_inlineScriptParçası.Any())
                return "";

            var sonuç = new StringBuilder();
            foreach (var item in _inlineScriptParçası[konum])
            {
                sonuç.Append(item);
                sonuç.Append(Environment.NewLine);
            }
            return sonuç.ToString();
        }

        public virtual void CssParçasıEkle(KaynakKonumu konum, string parça, string debugSrc, bool pakettinDışında = false)
        {
            if (!_cssParçaları.ContainsKey(konum))
                _cssParçaları.Add(konum, new List<CssReferenceMeta>());

            if (string.IsNullOrEmpty(parça))
                return;

            if (string.IsNullOrEmpty(debugSrc))
                debugSrc = parça;

            _cssParçaları[konum].Add(new CssReferenceMeta
            {
                pakettinDışında = pakettinDışında,
                Parça = parça,
                DebugSrc = debugSrc
            });
        }
        public virtual void CssParçasıIlaveEt(KaynakKonumu konum, string parça, string debugSrc, bool pakettinDışında = false)
        {
            if (!_cssParçaları.ContainsKey(konum))
                _cssParçaları.Add(konum, new List<CssReferenceMeta>());

            if (string.IsNullOrEmpty(parça))
                return;

            if (string.IsNullOrEmpty(debugSrc))
                debugSrc = parça;

            _cssParçaları[konum].Insert(0, new CssReferenceMeta
            {
                pakettinDışında = pakettinDışında,
                Parça = parça,
                DebugSrc = debugSrc
            });
        }
        public virtual string CssOluştur(IUrlHelper urlHelper, KaynakKonumu konum, bool? paketDosyaları = null)
        {
            if (!_cssParçaları.ContainsKey(konum) || _cssParçaları[konum] == null)
                return "";

            if (!_cssParçaları.Any())
                return "";

            var debugModel = _hostingEnvironment.IsDevelopment();

            if (!paketDosyaları.HasValue)
            {
                //Herhangi bir değer belirtilmemişse ayarı kullanın
                paketDosyaları = _seoAyarları.CssPaketlemeyeIzinVer;
            }

            if (urlHelper.ActionContext.HttpContext.Request.PathBase.HasValue)
                paketDosyaları = false;

            if (paketDosyaları.Value)
            {
                var pakettenParçalar = _cssParçaları[konum]
                    .Where(x => !x.pakettinDışında)
                    .Distinct()
                    .ToArray();
                var paketinDışındakiParçalar = _cssParçaları[konum]
                    .Where(x => x.pakettinDışında)
                    .Distinct()
                    .ToArray();


                var sonuç = new StringBuilder();

                if (pakettenParçalar.Length > 0)
                {
                    // ÖNEMLİ: Sanal dizinlerdeki CSS paketlemeyi kullanma
                    Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "bundles"));
                    var bundle = new Bundle();
                    foreach (var item in pakettenParçalar)
                    {
                        var src = debugModel ? item.DebugSrc : item.Parça;
                        src = urlHelper.Content(src);
                        var srcPath = Path.Combine(_hostingEnvironment.ContentRootPath, src.Remove(0, 1).Replace("/", "\\"));
                        if (File.Exists(srcPath))
                        {
                            src = src.Remove(0, 1);
                        }
                        else
                        {
                            src = "wwwroot/" + src;
                        }
                        bundle.InputFiles.Add(src);
                    }
                    //output file
                    var outputFileName = PaketSanalYolunuAl(pakettenParçalar.Select(x => { return debugModel ? x.DebugSrc : x.Parça; }).ToArray());
                    bundle.OutputFileName = "wwwroot/bundles/" + outputFileName + ".css";
                    //save
                    var configFilePath = _hostingEnvironment.ContentRootPath + "\\" + outputFileName + ".json";
                    bundle.FileName = configFilePath;
                    //paket oluştur
                    lock (s_lock)
                    {
                        var cacheKey = $"Rota.minification.shouldrebuild.css-{outputFileName}";
                        var shouldRebuild = _cacheManager.Al<bool>(cacheKey, RecheckBundledFilesPeriod, () => true);
                        if (shouldRebuild)
                        {
                            //process
                            _processor.Process(configFilePath, new List<Bundle> { bundle });
                            _cacheManager.Ayarla(cacheKey, false, RecheckBundledFilesPeriod);
                        }
                    }
                    sonuç.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"{1}\" />", urlHelper.Content("~/bundles/" + outputFileName + ".min.css"), MimeTipleri.TextCss);
                    sonuç.Append(Environment.NewLine);
                }

                foreach (var item in pakettenParçalar)
                {
                    var src = debugModel ? item.DebugSrc : item.Parça;
                    sonuç.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"{1}\" />", urlHelper.Content(src), MimeTipleri.TextCss);
                    sonuç.Append(Environment.NewLine);
                }

                return sonuç.ToString();
            }
            else
            {
                //paketleme devredışı
                var sonuç = new StringBuilder();
                foreach (var item in _cssParçaları[konum].Distinct())
                {
                    var src = debugModel ? item.DebugSrc : item.Parça;
                    sonuç.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"{1}\" />", urlHelper.Content(src), MimeTipleri.TextCss);
                    sonuç.AppendLine();
                }
                return sonuç.ToString();
            }
        }


        public virtual void CanonicalUrlParçasıEkle(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _canonicalUrlParçaları.Add(parça);
        }
        public virtual void CanonicalUrlParçasıIlaveEt(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _canonicalUrlParçaları.Insert(0, parça);
        }
        public virtual string CanonicalUrlOluştur()
        {
            var sonuç = new StringBuilder();
            foreach (var canonicalUrl in _canonicalUrlParçaları)
            {
                sonuç.AppendFormat("<link rel=\"canonical\" href=\"{0}\" />", canonicalUrl);
                sonuç.Append(Environment.NewLine);
            }
            return sonuç.ToString();
        }


        public virtual void ÖzelHeadParçasıEkle(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _özelHeadParçaları.Add(parça);
        }
        public virtual void ÖzelHeadParçasıIlaveEt(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _özelHeadParçaları.Insert(0, parça);
        }
        public virtual string ÖzelHeadOluştur()
        {
            //Sadece farklı satırlar kullan
            var farklıParçalar = _özelHeadParçaları.Distinct().ToList();
            if (!farklıParçalar.Any())
                return "";

            var sonuç = new StringBuilder();
            foreach (var yol in farklıParçalar)
            {
                sonuç.Append(yol);
                sonuç.Append(Environment.NewLine);
            }
            return sonuç.ToString();
        }


        public virtual void SayfaCssSınıfıParçasıEkle(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _sayfaCssSınıfıParçaları.Add(parça);
        }
        public virtual void SayfaCssSınıfıParçasıIlaveEt(string parça)
        {
            if (string.IsNullOrEmpty(parça))
                return;

            _sayfaCssSınıfıParçaları.Insert(0, parça);
        }
        public virtual string SayfaCssSınıfıOluştur()
        {
            string sonuç = string.Join(" ", _sayfaCssSınıfıParçaları.AsEnumerable().Reverse().ToArray());
            return sonuç;
        }
        public virtual void DüzenleSayfaURLsiEkle(string url)
        {
            _düzenleSayfaUrlsi = url;
        }
        public virtual string DüzenleSayfaURLsiAl()
        {
            return _düzenleSayfaUrlsi;
        }
        public virtual void AktifMenuÖğesiSistemAdıBelirle(string systemName)
        {
            _aktifAdminMenuSistemAdı = systemName;
        }
        public virtual string AktifMenuÖğesiSistemAdıAl()
        {
            return _aktifAdminMenuSistemAdı;
        }

        #endregion

        #region Nested classes

        private class ScriptReferenceMeta : IEquatable<ScriptReferenceMeta>
        {
            public bool pakettinDışında { get; set; }

            public bool IsAsync { get; set; }

            public string Parça { get; set; }
            public string DebugSrc { get; set; }
            public bool Equals(ScriptReferenceMeta item)
            {
                if (item == null)
                    return false;
                return this.Parça.Equals(item.Parça) && this.DebugSrc.Equals(item.DebugSrc);
            }
            public override int GetHashCode()
            {
                return Parça == null ? 0 : Parça.GetHashCode();
            }
        }
        


        private class CssReferenceMeta : IEquatable<CssReferenceMeta>
        {
            public bool pakettinDışında { get; set; }

            public string Parça { get; set; }
            public string DebugSrc { get; set; }
            public bool Equals(CssReferenceMeta item)
            {
                if (item == null)
                    return false;
                return this.Parça.Equals(item.Parça) && this.DebugSrc.Equals(item.DebugSrc);
            }
            public override int GetHashCode()
            {
                return Parça == null ? 0 : Parça.GetHashCode();
            }
        }
        #endregion
    }
}
