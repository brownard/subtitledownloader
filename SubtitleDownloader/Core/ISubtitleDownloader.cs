using System.Collections.Generic;
using System.IO;

namespace SubtitleDownloader.Core
{
    public interface ISubtitleDownloader
    {
        /// <summary>
        /// Get the name of this downloader implementation, e.g "OpenSubtitles" or "Bierdopje".
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// Search for subtitles by query string and optionally a year
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<Subtitle> SearchSubtitles(SearchQuery query);

        /// <summary>
        /// Search for subtitles by title of the serie, episode number and season number
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<Subtitle> SearchSubtitles(EpisodeSearchQuery query);

        /// <summary>
        /// Search for subtitles by IMDB identifier
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<Subtitle> SearchSubtitles(ImdbSearchQuery query);

        /// <summary>
        /// Fetches and saves the subtitle file(s)
        /// </summary>
        /// <param name="subtitle">Subtitle to download</param>
        /// <returns>Handles to downloaded subtitle files</returns>
        List<FileInfo> SaveSubtitle(Subtitle subtitle);

        /// <summary>
        /// Sets/gets the timeout in seconds for the search requests
        /// </summary>
        int SearchTimeout { get; set; }
    }
}