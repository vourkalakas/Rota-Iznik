using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Domain.Siteler
{
    public static class SiteUzantıları
    {
        public static string[] HostDeğerleriniAyrıştır(this Site site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            var parsedValues = new List<string>();
            if (!String.IsNullOrEmpty(site.Hosts))
            {
                string[] hosts = site.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string host in hosts)
                {
                    var tmp = host.Trim();
                    if (!String.IsNullOrEmpty(tmp))
                        parsedValues.Add(tmp);
                }
            }
            return parsedValues.ToArray();
        }
        public static bool HostDeğeriİçerir(this Site site, string host)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            if (String.IsNullOrEmpty(host))
                return false;

            var contains = site.HostDeğerleriniAyrıştır()
                                .FirstOrDefault(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase)) != null;
            return contains;
        }
    }
}
