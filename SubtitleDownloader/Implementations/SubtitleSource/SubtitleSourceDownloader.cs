using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using SubtitleDownloader.Core;
using SubtitleDownloader.Util;

namespace SubtitleDownloader.Implementations.SubtitleSource
{
    /// <summary>
    /// Implementation based on XML API
    /// 
    /// Supports:
    /// - SearchSubtitles(SearchQuery query)
    /// - SearchSubtitles(EpisodeSearchQuery query)
    /// - SearchSubtitles(ImdbSearchQuery query)
    /// </summary>
    public class SubtitleSourceDownloader
    {
        private int searchTimeout;

        public string GetName()
        {
            return "SubtitleSource";
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            List<Subtitle> results = new List<Subtitle>();

            foreach (var languageCode in query.LanguageCodes)
            {
                string url = GetQuerySearchUrl(query.Query, Languages.GetLanguageName(languageCode).ToLower());

                results.AddRange(GetResultsFromUrl(languageCode, url, query.Year));
            }
            return results;
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            string episode = "e" + String.Format("{0:00}", query.Episode);
            string season = "s" + String.Format("{0:00}", query.Season);
            string title = "";

            // e.g. "Stargate Universe" -> "Stargate.Universe."
            string[] splittedTitle = query.SerieTitle.Split(' ');

            foreach (var splitted in splittedTitle)
            {
                title += splitted + ".";    
            }

            // e.g. "Stargate.Universe.s01e02"
            string q = title + season + episode;
            
            List<Subtitle> results = new List<Subtitle>();

            foreach (var languageCode in query.LanguageCodes)
            {
                string url = GetQuerySearchUrl(q, Languages.GetLanguageName(languageCode).ToLower());
                results.AddRange(GetResultsFromUrl(languageCode, url, null));
            }
            return results;
        }

        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            List<Subtitle> results = new List<Subtitle>();

            foreach (var languageCode in query.LanguageCodes)
            {
                string url = GetImdbSearchUrl(query.ImdbId);
                results.AddRange(GetResultsFromUrl(languageCode, url, null));
            }
            return results;
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string downloadUrl = GetDownloadUrl(subtitle.Id);

            string zipFile = FileUtils.GetTempFileName();

            WebClient client = new WebClient();
            client.DownloadFile(downloadUrl, zipFile);

            return FileUtils.ExtractFilesFromZipOrRarFile(zipFile);
        }

        public int SearchTimeout
        {
            get { return searchTimeout; }
            set { searchTimeout = value; }
        }

        private List<Subtitle> GetResultsFromUrl(string queryLanguageCode, string url, int? queryYear)
        {
            List<Subtitle> results = new List<Subtitle>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            if (SearchTimeout > 0)
                request.Timeout = SearchTimeout * 1000;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            XmlReader reader = XmlReader.Create(response.GetResponseStream());

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);

            XmlNodeList subtitles = xmlDoc.GetElementsByTagName("sub");

            foreach (XmlNode subtitle in subtitles)
            {
                string id = null;
                string title = null;
                string language = null;
                string releasename = null;
                string year = null;

                foreach (XmlNode node in subtitle.ChildNodes)
                {
                    SetNodeValue(node, "id", ref id);
                    SetNodeValue(node, "title", ref title);
                    SetNodeValue(node, "language", ref language);
                    SetNodeValue(node, "releasename", ref releasename);
                    SetNodeValue(node, "year", ref year);
                }

                string languageCode = Languages.GetLanguageCode(language);
                
                if (languageCode.Equals(queryLanguageCode))
                {
                    Subtitle sub = new Subtitle(id, title, releasename, Languages.GetLanguageCode(language));
                    
                    if (queryYear != null)
                    {
                        // Check if the query year matches
                        if (year != null)
                        {
                            int yearAsInt = Convert.ToInt16(year);
                            
                            if(queryYear.Equals(yearAsInt))
                            {
                                results.Add(sub);
                            }
                        }
                        else
                        {
                            // No year found in results set
                            results.Add(sub);
                        }
                    }
                    else
                    {
                        results.Add(sub);
                    }
                }
            }
            return results;
        }

        private void SetNodeValue(XmlNode node, string name, ref string value)
        {
            if (node.LocalName.Equals(name))
            {
                value = node.InnerText;
            }
        }

        private string GetQuerySearchUrl(string query, string languageName)
        {
            return "http://www.subtitlesource.org/api/xmlsearch/" + query + "/" + languageName + "/0";
        }

        private string GetImdbSearchUrl(string imdbId)
        {
            return "http://www.subtitlesource.org/api/xmlsearch/" + imdbId + "/imdb/0";
        }

        private string GetDownloadUrl(string subtitleId)
        {
            return "http://www.subtitlesource.org/download/zip/" + subtitleId;
        }
    }
}
