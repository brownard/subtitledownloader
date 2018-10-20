using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using SubtitleDownloader.Core;
using SubtitleDownloader.Util;

namespace SubtitleDownloader.Implementations.MovieSubtitles
{
    /// <summary>
    /// Implementation based on XPath parsing.
    /// 
    /// Supports:
    /// - SearchSubtitles(SearchQuery query)
    /// </summary>
    public class MovieSubtitlesDownloader : ISubtitleDownloader
    {
        private int searchTimeout;

        public string GetName()
        {
            return "MovieSubtitles";
        }

        private const string MovieSubtitlesUrl = "http://www.moviesubtitles.org/";

        private const string SearchParameters = "search.php?q=";
        
        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            string moviePage = ParseMovieLinkFromSearchResultsPage(query);

            if (moviePage != null)
                return ParseSubtitlesForMovie(moviePage, query);

            return new List<Subtitle>();
        }

        [Obsolete("Not supported by current implementation")] 
        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Not supported by current implementation")] 
        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            throw new NotSupportedException();
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string downloadUrl = MovieSubtitlesUrl + "download-" + subtitle.Id + ".html";
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

        private bool OnPreRequest(HttpWebRequest request)
        {
            if (SearchTimeout > 0)
                request.Timeout = SearchTimeout * 1000;

            return true;
        }

        private string ParseMovieLinkFromSearchResultsPage(SearchQuery query)
        {
            HtmlWeb web = new HtmlWeb();
            web.PreRequest = new HtmlWeb.PreRequestHandler(OnPreRequest);
            HtmlDocument searchResultsPage = 
                web.Load(MovieSubtitlesUrl + SearchParameters + query.Query.ToLowerInvariant());

            // <a href="/movie-51.html">Batman Begins (2005)</a
            // -> Match <b>Batman Begins (2005)</b>
            // -> Get href
            HtmlNodeCollection resultLinks = searchResultsPage.DocumentNode.SelectNodes("//a");

            if (resultLinks == null)
                return null;

            var queryWithYearIfGiven = query.Year == null ? query.Query : query.Query + " " + query.Year;

            HtmlNode titleNode = FindTitleNode(queryWithYearIfGiven, resultLinks);
            
            if (titleNode != null)
            {
                if (query.Year != null)
                {
                    if (titleNode.InnerText.Contains(query.Year.ToString()))
                    {
                        // Title and year matches
                        return titleNode.GetAttributeValue("href", string.Empty);
                    }
                    // Title matches but the year doesn't..
                    return null;
                }

                // Get link to movie page
                return titleNode.GetAttributeValue("href", string.Empty);
            }
            return null;
        }

        private List<Subtitle> ParseSubtitlesForMovie(string moviePage, SearchQuery query)
        {
            List<Subtitle> results = new List<Subtitle>();

            HtmlWeb web = new HtmlWeb();
            web.PreRequest = new HtmlWeb.PreRequestHandler(OnPreRequest);
            HtmlDocument subtitlesListingPage = web.Load(MovieSubtitlesUrl + moviePage);

            // a href="/subtitle-100.html" title="Download english subtitles">
            // -> Get href for parsing subtitle id
            // -> Get language from title for subtitle language
            HtmlNodeCollection subtitleLinks = subtitlesListingPage.DocumentNode.SelectNodes("//a");

            foreach (var subtitleLink in subtitleLinks)
            {
                string href = subtitleLink.GetAttributeValue("href", string.Empty);

                if (href.StartsWith("/subtitle"))
                {
                    string id = ParseSubtitleId(href);
                    string language = ParseSubtitleLanguageFromTitleAttribute(
                        subtitleLink.GetAttributeValue("title", string.Empty));

                    if (language != null && SubtitleLanguageMatchesQueried(language.ToLower(), query.LanguageCodes))
                    {
                        string filename = ParseSubtitleFilenameFromSubtitlePage(href);
                        Subtitle sub = new Subtitle(id, query.Query, filename, Languages.GetLanguageCode(language));
                        results.Add(sub);
                    }
                }
            }
            return results;
        }

        private bool SubtitleLanguageMatchesQueried(string subtitleLang, string[] queriedLangs)
        {
            foreach (var lang in queriedLangs)
            {
                string queriedLang = Languages.GetLanguageName(lang).ToLower();

                if (subtitleLang.Equals(queriedLang))
                {
                    return true;
                }
            }
            return false;
        }

        private string ParseSubtitleId(string href)
        {
            // /subtitle-84813.html
            string[] splitted = href.Split('-');

            // 84813.html
            string[] splitted2 = splitted[1].Split('.');

            // 84813
            return splitted2[0];
        }

        private string ParseSubtitleFilenameFromSubtitlePage(string link)
        {
            HtmlWeb web = new HtmlWeb();
            web.PreRequest = new HtmlWeb.PreRequestHandler(OnPreRequest);
            HtmlDocument subtitleInfoPage = web.Load(MovieSubtitlesUrl + link);

            // <td width=70% class="filename">Batman Begins.TRD.part1.en.srt<br>Batman Begins.TRD.part2.en.srt</td>
            HtmlNodeCollection bTags = subtitleInfoPage.DocumentNode.SelectNodes("//b");

            foreach (var tag in bTags)
            {
                if (tag.InnerText.Equals("filename:"))
                {
                    // Get parent <td>
                    HtmlNode parentTd = tag.ParentNode;

                    // Get parent <td>'s sibling <td width=70% class="filename">Batman Begins.TRD.part1.en.srt<br>Batman Begins.TRD.part2.en.srt</td>
                    HtmlNode filenameTd = parentTd.NextSibling;

                    string fileName = filenameTd.InnerHtml.Replace("<br>", " / ");

                    return fileName;
                }
            }
            return null;
        }

        private string ParseSubtitleLanguageFromTitleAttribute(string title)
        {
            // "Download spanish subtitles" -> spanish
            string[] splitted = title.Split(' ');

            if (splitted.Length == 3)
                return splitted[1];

            return null;
        }

        public static HtmlNode FindTitleNode(string key, HtmlNodeCollection candidates)
        {
            double biggest = 0;
            double currentResult;
            HtmlNode selected = null;

            foreach (var node in candidates)
            {
                currentResult = SimilarityUtils.CompareStrings(key.ToLowerInvariant(), node.InnerText.ToLowerInvariant());

                if (currentResult > biggest)
                {
                    biggest = currentResult;
                    selected = node;
                }
            }
            return selected;
        }
    }
}