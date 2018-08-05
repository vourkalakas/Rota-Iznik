using Core.Domain.Blogs;
using System;
using System.Collections.Generic;

namespace Services.Blog
{
    public static class BlogUzantıları
    {
        public static string[] TaglarıBirleştir(this BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            var birleştirilmişTaglar = new List<string>();
            if (!String.IsNullOrEmpty(blogPost.Taglar))
            {
                string[] taglar2 = blogPost.Taglar.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string tag2 in taglar2)
                {
                    var tmp = tag2.Trim();
                    if (!String.IsNullOrEmpty(tmp))
                        birleştirilmişTaglar.Add(tmp);
                }
            }
            return birleştirilmişTaglar.ToArray();
        }
    }
}
