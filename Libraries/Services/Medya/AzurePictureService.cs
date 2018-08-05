using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Core;
using Core.Önbellek;
using Core.Yapılandırma;
using Core.Data;
using Core.Domain.Katalog;
using Core.Domain.Medya;
using Data;
using Services.Yapılandırma;
using Services.Olaylar;
using Services.Logging;

namespace Services.Medya
{
    public partial class AzurePictureService : ResimServisi
    {
        #region Constants
        private const string THUMB_EXISTS_KEY = "Rota.azure.thumb.exists-{0}";
        private const string THUMBS_PATTERN_KEY = "Rota.azure.thumb";
        #endregion

        #region Fields
        private static CloudBlobContainer _container;
        private readonly IStatikÖnbellekYönetici _önbellekYönetici;
        private readonly MedyaAyarları _medyaAyarları;
        private readonly Config _config;
        #endregion

        #region Ctor
        public AzurePictureService(IDepo<Resim> resimDepo,
            IAyarlarServisi ayarlarServisi,
            IWebYardımcısı webYardımcısı,
            ILogger logger,
            IDbContext dbContext,
            IOlayYayınlayıcı olayYayınlayıcı,
            IStatikÖnbellekYönetici önbellekYönetici,
            MedyaAyarları medyaAyarları,
            Config config,
            IDataSağlayıcı dataSağlayıcı,
            IHostingEnvironment hostingEnvironment)
            : base(resimDepo,
                ayarlarServisi,
                webYardımcısı,
                logger,
                dbContext,
                olayYayınlayıcı,
                medyaAyarları,
                dataSağlayıcı,
                hostingEnvironment)
        {
            this._önbellekYönetici = önbellekYönetici;
            this._medyaAyarları = medyaAyarları;
            this._config = config;

            if (string.IsNullOrEmpty(_config.AzureBlobStorageConnectionString))
                throw new Exception("Azure connection string for BLOB is not specified");

            if (string.IsNullOrEmpty(_config.AzureBlobStorageContainerName))
                throw new Exception("Azure container name for BLOB is not specified");

            if (string.IsNullOrEmpty(_config.AzureBlobStorageEndPoint))
                throw new Exception("Azure end point for BLOB is not specified");

            CreateCloudBlobContainer();
        }

        #endregion

        #region Utilities
        protected virtual async void CreateCloudBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(_config.AzureBlobStorageConnectionString);
            if (storageAccount == null)
                throw new Exception("Azure connection string for BLOB is not wrong");

            var blobClient = storageAccount.CreateCloudBlobClient();

            _container = blobClient.GetContainerReference(_config.AzureBlobStorageContainerName);

            await _container.CreateIfNotExistsAsync();
            await _container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
        }
        protected override async void ResimThumbSil(Resim resim)
        {
            await ResimThumbSilAsync(resim);
        }
        protected override string ThumbYoluAl(string thumbDosyaAdı)
        {
            return $"{_config.AzureBlobStorageEndPoint}{_config.AzureBlobStorageContainerName}/{thumbDosyaAdı}";
        }
        protected override string ThumbUrlAl(string thumbDosyaAdı, string siteKonumu = null)
        {
            return $"{_config.AzureBlobStorageEndPoint}{_config.AzureBlobStorageContainerName}/{thumbDosyaAdı}";
        }
        protected override bool ThumbZatenMevcut(string thumbDosyayolu, string thumbDosyaAdı)
        {
            return OluşturulanThumbMevcutAsync(thumbDosyayolu, thumbDosyaAdı).Result;
        }
        protected override async void ThumbKaydet(string thumbDosyaYolu, string thumbDosyaAdı, string mimeTipi, byte[] binary)
        {
            await ThumbAsyncKaydet(thumbDosyaYolu, thumbDosyaAdı, mimeTipi, binary);
        }
        protected virtual async Task ResimThumbSilAsync(Resim resim)
        {
            //create a string containing the blob name prefix
            var prefix = $"{resim.Id:0000000}";

            BlobContinuationToken continuationToken = null;
            do
            {
                var resultSegment = await _container.ListBlobsSegmentedAsync(prefix, true, BlobListingDetails.All, null, continuationToken, null, null);

                //delete files in result segment
                await Task.WhenAll(resultSegment.Results.Select(blobItem => ((CloudBlockBlob)blobItem).DeleteAsync()));

                //get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            _önbellekYönetici.KalıpİleSil(THUMBS_PATTERN_KEY);
        }
        protected virtual async Task<bool> OluşturulanThumbMevcutAsync(string thumbDosyaYolu, string thumbDosyaAdı)
        {
            try
            {
                var key = string.Format(THUMB_EXISTS_KEY, thumbDosyaAdı);
                return await _önbellekYönetici.Al(key, async () =>
                {
                    var blockBlob = _container.GetBlockBlobReference(thumbDosyaAdı);
                    return await blockBlob.ExistsAsync();
                });
            }
            catch { return false; }
        }
        protected virtual async Task ThumbAsyncKaydet(string thumbDosyaYolu, string thumbDosyaAdı, string mimeType, byte[] binary)
        {
            var blockBlob = _container.GetBlockBlobReference(thumbDosyaAdı);

            if (!string.IsNullOrEmpty(mimeType))
                blockBlob.Properties.ContentType = mimeType;

            if (!string.IsNullOrEmpty(_medyaAyarları.AzureOnbellekControlBasligi))
                blockBlob.Properties.CacheControl = _medyaAyarları.AzureOnbellekControlBasligi;

            await blockBlob.UploadFromByteArrayAsync(binary, 0, binary.Length);

            _önbellekYönetici.KalıpİleSil(THUMBS_PATTERN_KEY);
        }

        #endregion
    }
}
