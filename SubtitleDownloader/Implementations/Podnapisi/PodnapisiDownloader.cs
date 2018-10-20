using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using HtmlAgilityPack;
using SubtitleDownloader.Core;
using SubtitleDownloader.Util;

namespace SubtitleDownloader.Implementations.Podnapisi
{
    public class PodnapisiDownloader : ISubtitleDownloader
    {
        private readonly string baseUrl = "https://www.podnapisi.net";

        private readonly string searchUrlBase = "https://www.podnapisi.net/ppodnapisi/search?sXML=1";

        private XmlDocument xmlDoc;

        private int searchTimeout;

        public string GetName()
        {
            return "Podnapisi";
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            string url = searchUrlBase
                         + "&sK=" + query.Query;

            if (query.Year.HasValue)
            {
                url += "&sY=" + query.Year;
            }
            return Search(url, query);
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            string url = searchUrlBase
                         + "&sK=" + query.SerieTitle
                         + "&sTS=" + query.Season
                         + "&sTE=" + query.Episode;

            return Search(url, query);
        }

        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            throw new NotImplementedException();
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string url = subtitle.Id;

            string archiveFile = FileUtils.GetTempFileName();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument seriesListingPage = web.Load(url);

            HtmlNodeCollection links = seriesListingPage.DocumentNode.SelectNodes("//form");

            foreach (HtmlNode node in links)
            {
                string href = node.GetAttributeValue("action", string.Empty);

                // http://www.podnapisi.net/subtitles/heroes-2006/rZcC/download
                if (href.Contains("/download"))
                {
                    WebClient client = new WebClient();
                    client.DownloadFile(baseUrl + href, archiveFile);

                    return FileUtils.ExtractFilesFromZipOrRarFile(archiveFile);
                }
            }

            throw new Exception("No download link found for subtitle!");
        }

        public int SearchTimeout
        {
            get { return searchTimeout;  }
            set { searchTimeout = value; }
        }

        private List<Subtitle> Search(string baseUrl, SubtitleSearchQuery query)
        {
            Dictionary<string, string> languages = ParseLanguageOptions();

            List<Subtitle> results = new List<Subtitle>();

            foreach (string languageName in languages.Keys)
            {
                if (!query.HasLanguageCode(Languages.FindLanguageCode(languageName)))
                    continue;

                string languageId = languages[languageName];

                string url = baseUrl + "&sJ=" + languageId;

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);

                if (SearchTimeout > 0)
                    request.Timeout = SearchTimeout * 1000;

                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ProhibitDtd = false;
                settings.ValidationType = ValidationType.None;

                XmlReader reader = XmlReader.Create(response.GetResponseStream(), settings);

                xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                XmlNodeList subtitleNodes = xmlDoc.GetElementsByTagName("subtitle");

                foreach (XmlNode subtitleNode in subtitleNodes)
                {
                    string subtitleId = null;
                    string releaseName = null;

                    for (int i = 0; i < subtitleNode.ChildNodes.Count; i++)
                    {
                        if (subtitleNode.ChildNodes[i].Name == "url")
                        {
                            subtitleId = subtitleNode.ChildNodes[i].InnerText;
                        }
                        else if (subtitleNode.ChildNodes[i].Name == "release")
                        {
                            releaseName = subtitleNode.ChildNodes[i].InnerText.TrimEnd().TrimStart();
                        }
                    }

                    if (!String.IsNullOrEmpty(releaseName))
                    {
                        if (releaseName.Contains(" "))
                        {
                            var releases = releaseName.Split(' ');

                            foreach (var release in releases)
                            {
                                Subtitle subtitle = new Subtitle(subtitleId, release, release,
                                                                 Languages.FindLanguageCode(languageName));
                                results.Add(subtitle);
                            }
                        }
                        else
                        {
                            Subtitle subtitle = new Subtitle(subtitleId, releaseName, releaseName,
                                                             Languages.FindLanguageCode(languageName));
                            results.Add(subtitle);
                        }
                    }
                }
            }
            return results;
        }

        private Dictionary<string, string> ParseLanguageOptions()
        {
            return new Dictionary<string, string>()
                       {
                            { "Albanian", "29"},
                            { "Arabic", "12" },
                            { "Argentino", "14" },
                            { "Belarus", "50" },
                            { "Bosnian", "10" },
                            { "Brazilian", "48" },
                            { "Bulgarian", "33" },
                            { "Catalan", "53" },
                            { "Chinese", "17" },
                            { "Croatian", "38" },
                            { "Czech", "7" },
                            { "Danish", "24" },
                            { "Dutch", "23" },
                            { "English", "2" },
                            { "Estonian", "20" },
                            { "Farsi", "52" },
                            { "Finnish", "31" },
                            { "French", "8" },
                            { "German", "5" },
                            { "Greek", "16" },
                            { "Hebrew", "22" },
                            { "Hindi", "42" },
                            { "Hungarian", "15" },
                            { "Icelandic", "6" },
                            { "Indonesian", "54" },
                            { "Irish", "49" },
                            { "Italian", "9" },
                            { "Japanese", "11" },
                            { "Korean", "4" },
                            { "Latvian", "21" },
                            { "Lithuanian", "19" },
                            { "Macedonian", "35" },
                            { "Malay", "55" },
                            { "Mandarin", "40" },
                            { "Norwegian", "3" },
                            { "Polish", "26" },
                            { "Portuguese", "32" },
                            { "Romanian", "13" },
                            { "Russian", "27" },
                            { "Serbian", "36" },
                            { "Slovak", "37" },
                            { "Slovenian", "1" },
                            { "Spanish", "28" },
                            { "Swedish", "25" },
                            { "Thai", "44" },
                            { "Turkish", "30" },
                            { "Ukrainian", "46" },
                            { "Vietnamese", "51" },
                       };
        }
    }
}
