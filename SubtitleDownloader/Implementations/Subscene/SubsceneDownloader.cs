using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using SubtitleDownloader.Core;
using SubtitleDownloader.Util;

namespace SubtitleDownloader.Implementations.Subscene
{
    /// <summary>
    /// Implementation based on XPath parsing.
    /// 
    /// Supports:
    /// - SearchSubtitles(SearchQuery query)
    /// - SearchSubtitles(EpisodeSearchQuery query)
    /// </summary>
    public class SubsceneDownloader : ISubtitleDownloader
    {
        private const string SubsceneUrl = "http://subscene.com";

        private const string SubsceneUrlV2 = "http://v2.subscene.com";

        private const string SubsceneQueryUrl = SubsceneUrlV2 + "/s.aspx?q=";

        private string[] languageCodes;

        private int searchTimeout;

        public string GetName()
        {
            return "Subscene";
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            languageCodes = query.LanguageCodes;

            List<Subtitle> results = new List<Subtitle>();

            HtmlWeb web = new HtmlWeb();
            web.UseCookies = true;
            web.PreRequest = new HtmlWeb.PreRequestHandler(OnPreRequest);
            HtmlDocument searchResults = web.Load(SubsceneQueryUrl + query.Query);

            // Perform search and find all download links from results page.
            // Extract subtitle id (e.g. "68855") from link.

            // <a class="a1" id=s68855 href="javascript:Subtitle(68855, 'zip', '0', '66250');">
            HtmlNodeCollection subtitleLinks = searchResults.DocumentNode.SelectNodes("//a");

            if (subtitleLinks == null) 
                return new List<Subtitle>(0);

            foreach (var subtitleLink in subtitleLinks)
            {
                string href = subtitleLink.GetAttributeValue("href", string.Empty);

                if (href.Contains("subtitle-"))
                {
                    // href="/danish/The-4400-Fourth-Season/subtitle-120640.aspx"
                    //string[] splittedHref1 = href.Split('/');
                    //string[] splittedHref2 = splittedHref1[0].Split('(');
                    //string id = splittedHref2[1];
                    //int start = href.IndexOf("subtitle-") + "subtitle-".Length;
                    //string page = href.Substring(start);    // 120640.aspx
                    //string[] splitted = page.Split('.');    // [120640][aspx]
                    //string id = splitted[0];                // 120640

                    string languageCode = "";
                    string subtitleName = "";

                    // Parse language name and subtitle name
                    if (ParseLanguageAndSubtitleName(subtitleLink, ref languageCode, ref subtitleName))
                    {
                        if (query.HasLanguageCode(languageCode))
                        {
                            Subtitle subtitle = new Subtitle(href, subtitleName, subtitleName, languageCode);
                            results.Add(subtitle);   
                        }
                    }
                }
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

            SearchQuery searchQuery = new SearchQuery(q);
            searchQuery.LanguageCodes = query.LanguageCodes;

            List<Subtitle> firstResults = SearchSubtitles(searchQuery);

            // e.g. "heroes 409"
            q = query.SerieTitle + " " + query.Season + String.Format("{0:00}", query.Episode);

            searchQuery = new SearchQuery(q);
            searchQuery.LanguageCodes = query.LanguageCodes;

            List<Subtitle> secondResults = SearchSubtitles(searchQuery);

            firstResults.AddRange(secondResults);
            return firstResults;
        }

        [Obsolete("Not supported by current implementation")] 
        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            throw new NotSupportedException("IMDB ID search is not supported by SubsceneDownloader");
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string file = DownloadSubtitle(subtitle.Id);

            if (file != null)
            {
                return FileUtils.ExtractFilesFromZipOrRarFile(file);
            }
            
            throw new Exception("Subtitle download failed! Downloading returned unexpected result!");
        }

        public int SearchTimeout
        {
            get { return searchTimeout; }
            set { searchTimeout = value; }
        }

        private string DownloadSubtitle(string subtitleUrl)
        {
            try
            {
                var downloadUrl = SubsceneUrlV2 + subtitleUrl;

                HtmlWeb web = new HtmlWeb();
                HtmlDocument downloadPage = web.Load(downloadUrl);

                HtmlNodeCollection subtitleLinks = downloadPage.DocumentNode.SelectNodes("//a");

                if (subtitleLinks.Count == 0)
                    throw new Exception("Subtitle download failed! No download link available");

                foreach (var subtitleLink in subtitleLinks)
                {
                    string href = subtitleLink.GetAttributeValue("href", string.Empty);

                    if (href.StartsWith("/subtitle/download"))
                    {
                        string fileDownloadUrl = SubsceneUrl + href;

                        string archiveFile = FileUtils.GetTempFileName();

                        WebClient client = new WebClient();
                        client.DownloadFile(fileDownloadUrl, archiveFile);

                        return archiveFile;
                    }
                }
            }
            catch (Exception e)
            {
                // Unable to do anything..
            }

            return null;
        }

        //<a class="a1" id=s218628 href="javascript:Subtitle(218628, 'zip', '9', '73786');"> 
        //  <span class="r100" >Arabic</span>
		//	<span id="r218628">Heroes.S03E23.HDTV.XviD-LOL  Heroes.S03E23.720p.HDTV.X264-DIMENSION (By Mohamed El Mansoura)</span> 
		//</a> 
        private bool ParseLanguageAndSubtitleName(HtmlNode subtitleLink, ref string languageCode, ref string subtitleName)
        {
            HtmlNodeCollection childs = subtitleLink.ChildNodes;

            bool langFound = false;

            foreach (var child in childs)
            {
                if (child.Name.Equals("span"))
                {
                    string childText = child.InnerText.Trim();

                    if (Languages.IsSupportedLanguageName(childText))
                    {
                        // e.g. "Arabic"
                        languageCode = Languages.GetLanguageCode(childText);
                        langFound = true;
                    }
                    else if (langFound)
                    {
                        // e.g. "Heroes.S03E23.HDTV.XviD-LOL (some comments)", don't include comments
                        string[] splitted = childText.Split('(');
                        subtitleName = splitted[0].TrimEnd(' ');
                    }
                }
            }
            return langFound;
        }

        private bool OnPreRequest(HttpWebRequest request)
        {
            if (searchTimeout > 0)
                request.Timeout = searchTimeout * 1000;

            string cookieValue = "";

            foreach (var languageCode in languageCodes)
            {
                string languageName = Languages.GetLanguageName(languageCode);
                
                try
                {
                    cookieValue += LanguageIds[languageName];
                    cookieValue += "-"; 
                }
                catch
                {
                    // Probably language is not supported
                    // throw new Exception("Could not found language id for language " + languageName);
                }
            }

            // Remove last "-"
            if (cookieValue.Length > 0)
            {
                cookieValue = cookieValue.Remove(cookieValue.Length-1);
            }

            Cookie languageIds = new Cookie("subscene_sLanguageIds", cookieValue, "/", "subscene.com");
            request.CookieContainer.Add(languageIds);

            return true;
        }

        public static Dictionary<string, int> LanguageIds
        {
            get
            {
                return new Dictionary<string, int>()
                       {
                           { "Albanian", 1 },
                           { "Arabic", 2 },
                           { "Bengali", 54 },
                           { "Bulgarian", 5 },
                           { "Catalan", 49 },
                           { "Chinese", 7 },
                           { "Croatian", 8 },
                           { "Czech", 9 },
                           { "Danish", 10 },
                           { "Dutch", 11 },
                           { "English", 13 },
                           { "Estonian", 16 },
                           { "Farsi", 46 },
                           { "Persian", 46 },
                           { "Finnish", 17 },
                           { "French", 18 },
                           { "German",19 },
                           { "Greek", 21 },
                           { "Hebrew", 22 },
                           { "Hindi", 51  },
                           { "Hungarian", 23 },
                           { "Icelandic", 25 },
                           { "Indonesian", 44 },
                           { "Italian", 26 },
                           { "Japanese", 27 },
                           { "Korean", 28 },
                           { "Kurdish", 52 },
                           { "Latvian", 29 },
                           { "Lithuanian", 43 },
                           { "Macedonian", 48 },
                           { "Malay", 50 },
                           { "Norwegian", 30 },
                           { "Polish", 31 },
                           { "Portuguese", 32 },
                           { "Romanian", 33 },
                           { "Russian", 34 },
                           { "Serbian", 35 },
                           { "Slovak", 36 },
                           { "Slovenian", 37 },
                           { "Spanish", 38 },
                           { "Swedish", 39 },
                           { "Tagalog", 53 },
                           { "Thai", 40 },
                           { "Turkish", 41 },
                           { "Ukranian", 56 },
                           { "Ukrainian", 56 },
                           { "Urdu", 42 },
                           { "Vietnamese", 45 }
                       };
            }
        }
    }
}
