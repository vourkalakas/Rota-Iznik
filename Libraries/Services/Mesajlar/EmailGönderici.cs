using System;
using System.Collections.Generic;
using Core.Domain.Mesajlar;
using System.Net.Mail;
using System.Linq;
using System.IO;
using System.Net;
using Services.Medya;

namespace Services.Mesajlar
{
    public partial class EmailGönderici : IEmailGönderici
    {
        private readonly IDownloadServisi _downloadServisi;
        public EmailGönderici(IDownloadServisi downloadServisi)
        {
            this._downloadServisi = downloadServisi;
        }
        public virtual void SendEmail(EmailHesabı emailHesabı, string konu, string gövde, string adresinden, string isminden, string adresine, string ismine, string cevapAdresi = null, string cevapAdı = null, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, string ekDosyaYolu = null, string ekDosyaAdı = null, int ekİndirmeId = 0, IDictionary<string, string> basşlıklar = null)
        {
            var mesaj = new MailMessage();
            //from, to, reply to
            mesaj.From = new MailAddress(adresinden, isminden);
            mesaj.To.Add(new MailAddress(adresine, ismine));
            if (!String.IsNullOrEmpty(cevapAdresi))
            {
                mesaj.ReplyToList.Add(new MailAddress(cevapAdresi, cevapAdı));
            }
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                {
                    mesaj.Bcc.Add(address.Trim());
                }
            }
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                {
                    mesaj.CC.Add(address.Trim());
                }
            }
            mesaj.Subject = konu;
            mesaj.Body = gövde;
            mesaj.IsBodyHtml = true;
            if (basşlıklar != null)
                foreach (var başlık in basşlıklar)
                {
                    mesaj.Headers.Add(başlık.Key, başlık.Value);
                }
            if (!String.IsNullOrEmpty(ekDosyaYolu) &&
                File.Exists(ekDosyaYolu))
            {
                var ek = new Attachment(ekDosyaYolu);
                ek.ContentDisposition.CreationDate = File.GetCreationTime(ekDosyaYolu);
                ek.ContentDisposition.ModificationDate = File.GetLastWriteTime(ekDosyaYolu);
                ek.ContentDisposition.ReadDate = File.GetLastAccessTime(ekDosyaYolu);
                if (!String.IsNullOrEmpty(ekDosyaYolu))
                {
                    ek.Name = ekDosyaYolu;
                }
                mesaj.Attachments.Add(ek);
            }
            if (ekİndirmeId > 0)
            {
                var download = _downloadServisi.DownloadAlId(ekİndirmeId);
                if (download != null)
                {
                    if (!download.UseDownloadUrl)
                    {
                        string fileName = !String.IsNullOrWhiteSpace(download.DosyaAdı) ? download.DosyaAdı : download.Id.ToString();
                        fileName += download.Uzantı;
                        var ms = new MemoryStream(download.DownloadBinary);
                        var ek = new Attachment(ms, fileName);
                        //string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                        //var ek = new Attachment(ms, fileName, contentType);
                        ek.ContentDisposition.CreationDate = DateTime.UtcNow;
                        ek.ContentDisposition.ModificationDate = DateTime.UtcNow;
                        ek.ContentDisposition.ReadDate = DateTime.UtcNow;
                        mesaj.Attachments.Add(ek);
                    }
                }
            }
            //send email
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailHesabı.VarsayılanKimlikBilgileriniKullan;
                smtpClient.Host = emailHesabı.Host;
                smtpClient.Port = emailHesabı.Port;
                smtpClient.EnableSsl = emailHesabı.SslEtkin;
                smtpClient.Credentials = emailHesabı.VarsayılanKimlikBilgileriniKullan ?
                    CredentialCache.DefaultNetworkCredentials :
                    new NetworkCredential(emailHesabı.KullanıcıAdı, emailHesabı.Şifre);
                smtpClient.Send(mesaj);
            }
        }
    }
}
