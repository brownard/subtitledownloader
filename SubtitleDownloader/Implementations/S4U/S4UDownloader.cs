using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using SubtitleDownloader.Core;
using SubtitleDownloader.Util;

namespace SubtitleDownloader.Implementations.S4U
{
    /// <summary>
    /// Implementation based on XML API
    /// 
    /// Supports:
    /// - SearchSubtitles(SearchQuery query)
    /// - SearchSubtitles(EpisodeSearchQuery query)
    /// - SearchSubtitles(ImdbSearchQuery query)
    /// </summary>
    public class S4UDownloader : ISubtitleDownloader
    {
        private readonly string ApiKey = Configuration.S4UApiKey;

        private int searchTimeout;

        public string GetName()
        {
            return "S4U.se";
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var results = new List<Subtitle>();

            if (!query.HasLanguageCode("swe"))
                return results;

            string url = GetMovieQuerySearchUrl(query.Query, query.Year);
            results.AddRange(GetResultsFromUrl(url));

            return results;
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            var results = new List<Subtitle>();

            if (!query.HasLanguageCode("swe"))
                return results;

            string url = query.TvdbId.HasValue 
                ? GetSerieQueryTvdbIdSearchUrl(query.TvdbId.Value, query.Season, query.Episode) 
                : GetSerieQuerySearchUrl(query.SerieTitle, query.Season, query.Episode);

            results.AddRange(GetResultsFromUrl(url));

            return results;
        }

        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            var results = new List<Subtitle>();

            if (!query.HasLanguageCode("swe"))
                return results;

            var url = GetImdbSearchUrl(query.ImdbId);
            results.AddRange(GetResultsFromUrl(url));

            return results;
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            var downloadUrl = subtitle.Id;

            var zipFile = FileUtils.GetTempFileName();

            var client = new WebClient();
            client.DownloadFile(downloadUrl, zipFile);

            var files = FileUtils.ExtractFilesFromZipOrRarFile(zipFile);

            var subtitleFiles = new List<FileInfo>();

            foreach (var file in files)
            {
                if (file.Extension.Equals(".srt") || file.Extension.Equals(".sub"))
                    subtitleFiles.Add(file);
            }
            return subtitleFiles;
        }

        public int SearchTimeout
        {
            get { return searchTimeout; }
            set { searchTimeout = value; }
        }

        private IEnumerable<Subtitle> GetResultsFromUrl(string url)
        {
            var results = new List<Subtitle>();

            var request = (HttpWebRequest)WebRequest.Create(url);

            if (SearchTimeout > 0)
                request.Timeout = SearchTimeout * 1000;

            var response = (HttpWebResponse)request.GetResponse();

            var reader = XmlReader.Create(response.GetResponseStream());

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);

            var subtitles = xmlDoc.GetElementsByTagName("sub");

            foreach (XmlNode subtitle in subtitles)
            {
                string id = null;
                string title = null;
                string releasename = null;

                foreach (XmlNode node in subtitle.ChildNodes)
                {
                    SetNodeValue(node, "download_zip", ref id);
                    SetNodeValue(node, "ep_title", ref title);
                    SetNodeValue(node, "file_name", ref releasename);
                }

                results.Add(new Subtitle(id, title, releasename, Languages.GetLanguageCode("Swedish")));
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

        private string GetSerieQueryTvdbIdSearchUrl(int tvdbid, int season, int episode)
        {
            return "http://api.s4u.se/1.0/"
                + ApiKey + "/xml" + "/serie/tvdb" + "/" + tvdbid + "/season=" + season + "&episode=" + episode;
        }

        private string GetSerieQuerySearchUrl(string query, int season, int episode)
        {
            return "http://api.s4u.se/1.0/" 
                + ApiKey + "/xml" + "/serie/title" + "/" + query + "/season=" + season + "&episode=" + episode;
        }

        private string GetMovieQuerySearchUrl(string query, int? year)
        {
            var url = "http://api.s4u.se/1.0/" + ApiKey + "/xml" + "/movie/title" + "/" + query;

            if (year.HasValue)
                url += "/year=" + year;

            return url;
        }

        private string GetImdbSearchUrl(string imdbId)
        {
            return "http://api.s4u.se/1.0/" 
                + ApiKey + "/xml" + "/movie/imdb" + "/" + imdbId;
        }
    }
}