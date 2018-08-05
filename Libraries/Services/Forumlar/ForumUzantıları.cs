using Core.Domain.Forum;
using System;
using System.Linq;

namespace Services.Forumlar
{
    public static class ForumUzantıları
    {
        public static ForumGirdisi İlkGirdiAl(this ForumSayfası forumSayfası, IForumServisi forumServisi)
        {
            if (forumSayfası == null)
                throw new ArgumentNullException("forumSayfası");

            var forumGirdileri = forumServisi.TümGirdileriAl(forumSayfası.Id, 0, string.Empty, 0, 1);
            if (forumGirdileri.Any())
                return forumGirdileri[0];

            return null;
        }
    }
}
