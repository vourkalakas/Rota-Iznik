﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Core;

namespace Services.Yardımcılar
{
    public class BrowscapXmlHelper
    {
        private readonly List<string> _crawlerUserAgentsRegexp;

        public BrowscapXmlHelper(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath)
        {
            _crawlerUserAgentsRegexp = new List<string>();

            Initialize(userAgentStringsPath, crawlerOnlyUserAgentStringsPath);
        }

        private void Initialize(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath)
        {
            List<XElement> crawlerItems = null;

            if (!string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) && File.Exists(crawlerOnlyUserAgentStringsPath))
            {
                //try to load crawler list from crawlers only file
                using (var sr = new StreamReader(crawlerOnlyUserAgentStringsPath))
                {
                    crawlerItems = XDocument.Load(sr).Root?.Elements("browscapitem").ToList();
                }
            }

            if (crawlerItems == null)
            {
                //try to load crawler list from full user agents file
                using (var sr = new StreamReader(userAgentStringsPath))
                {
                    crawlerItems = XDocument.Load(sr).Root?.Elements("browscapitem")
                        //only crawlers
                        .Where(IsBrowscapItemIsCrawler).ToList();
                }
            }

            if (crawlerItems == null)
                throw new Exception("Incorrect file format");

            _crawlerUserAgentsRegexp.AddRange(crawlerItems
                //get only user agent names
                .Select(e => e.Attribute("name"))
                .Where(e => e != null && !string.IsNullOrEmpty(e.Value))
                .Select(e => e.Value)
                .Select(ToRegexp));

            if (string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) || File.Exists(crawlerOnlyUserAgentStringsPath))
                return;

            //try to write crawlers file
            using (var sw = new StreamWriter(crawlerOnlyUserAgentStringsPath))
            {
                var root = new XElement("browsercapitems");

                foreach (var crawler in crawlerItems)
                {
                    foreach (var element in crawler.Elements().ToList())
                    {
                        if ((element.Attribute("name")?.Value.ToLower() ?? string.Empty) == "crawler")
                            continue;
                        element.Remove();
                    }

                    root.Add(crawler);
                }
                root.Save(sw);
            }
        }

        private static bool IsBrowscapItemIsCrawler(XElement browscapItem)
        {
            var el = browscapItem.Elements("item").FirstOrDefault(e => e.Attribute("name")?.Value == "Crawler");

            return el != null && el.Attribute("value")?.Value.ToLower() == "true";
        }

        private static string ToRegexp(string str)
        {
            var sb = new StringBuilder(Regex.Escape(str));
            sb.Replace("&amp;", "&").Replace("\\?", ".").Replace("\\*", ".*?");
            return string.Format("^{0}$", sb);
        }
        public bool IsCrawler(string userAgent)
        {
            return _crawlerUserAgentsRegexp.Any(p => Regex.IsMatch(userAgent, p));
        }
    }
}
