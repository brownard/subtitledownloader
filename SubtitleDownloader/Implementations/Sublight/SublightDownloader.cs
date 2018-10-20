using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SubtitleDownloader.Core;
using SubtitleDownloader.Implementations.SublightApi;
using SubtitleDownloader.Util;
using Subtitle=SubtitleDownloader.Core.Subtitle;

namespace SubtitleDownloader.Implementations.Sublight
{
    /// <summary>
    /// Implementation uses Sublight Web Service API
    /// 
    /// Supports:
    /// - SearchSubtitles(SearchQuery query)
    /// - SearchSubtitles(EpisodeSearchQuery query)
    /// </summary>
    public class SublightDownloader : ISubtitleDownloader
    {
        private readonly string ClientId = Configuration.SubLightClientId;

        private readonly string ApiKey = Configuration.SublightApiKey;

        private SublightApi.Sublight client = new SublightApi.Sublight();

        private Guid guid;

        private int searchTimeout;

        public string GetName()
        {
            return "Sublight";
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            string error;
            string[] settings;

            Login(out settings);

            SublightApi.Subtitle[] subtitles;
            Release[] releases;
            bool isLimited;

            if (searchTimeout > 0)
                client.Timeout = searchTimeout * 1000;

            client.SearchSubtitles3(guid, null, query.Query, query.Year, null, null,
                                    GetLanguages(query),
                                    new[] { Genre.Serial, Genre.Cartoon, Genre.Documentary, Genre.Movie, Genre.Other },
                                    null, null,
                                    out subtitles, out releases, out isLimited, out error);
            ProcessError(error, "Error occurred when performing subtitle search");
            Logout();

            return CreateSubtitleResults(subtitles);
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            string error;
            string[] settings;

            Login(out settings);

            SublightApi.Subtitle[] subtitles;
            Release[] releases;
            bool isLimited;

            byte? season = (byte) query.Season;
            int? episode = Convert.ToInt16(query.Episode);

            try
            {
                if (searchTimeout > 0)
                    client.Timeout = searchTimeout * 1000;

                client.SearchSubtitles3(guid, null, query.SerieTitle, null, season, episode,
                                        GetLanguages(query),
                                        new[] {Genre.Serial},
                                        null, null,
                                        out subtitles, out releases, out isLimited, out error);
                ProcessError(error, "Error occurred when performing subtitle search");
                Logout();

                return CreateSubtitleResults(subtitles);
            }
            catch(UnsupportedLanguageException)
            {
                return new List<Subtitle>(0);
            }
        }

        [Obsolete("Not supported by current implementation")] 
        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            throw new NotSupportedException("IMDB ID search not supported by SublightDownloader");
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string error;
            string[] settings;

            string data;
            string ticket;
            short que;

            Login(out settings);
            client.GetDownloadTicket(guid, ClientId, ClientId, out ticket, out que, out error);
            ProcessError(error, "Unable to get download ticket for subtitle with ID '" + subtitle.Id + "'");

            Guid subtitleGuid = new Guid(subtitle.Id);
            double points;

            // Wait if we are not allowed to download immediately
            Thread.Sleep(que * 1000);

            client.DownloadByID4(guid, subtitleGuid, 1, false, ticket, out data, out points, out error);
            ProcessError(error, "Unable to download subtitle with ID '" + subtitle.Id + "'");
            Logout();
            
            if (data != null)
            {
                // Create decoded file
                byte[] decoded = Decoder.Decode(data);
                string tempFile = FileUtils.GetTempFileName();
                File.WriteAllBytes(tempFile, decoded);

                return FileUtils.ExtractFilesFromZipOrRarFile(tempFile);
            }
            throw new Exception("Subtitle not found with ID '" + subtitle.Id + "'");
        }

        public int SearchTimeout
        {
            get { return searchTimeout; }
            set { searchTimeout = value; }
        }

        private List<Subtitle> CreateSubtitleResults(SublightApi.Subtitle[] subtitles)
        {
            List<Subtitle> results = new List<Subtitle>();

            if (subtitles == null) return results;

            foreach(SublightApi.Subtitle subtitle in subtitles)
            {
                string subtitleFileName = GenerateSubtitleFileName(subtitle);

                Subtitle result = new Subtitle(
                    subtitle.SubtitleID.ToString(), subtitle.Title, subtitleFileName, 
                    Languages.GetLanguageCode(subtitle.Language.ToString()));

                results.Add(result);
            }
            return results;
        }

        private void Login(out string[] settings)
        {
            string error;
            ClientInfo clientInfo = new ClientInfo();
            clientInfo.ClientId = ClientId;
            clientInfo.ApiKey = ApiKey;
            guid = client.LogInAnonymous4(clientInfo, new string[0], out settings, out error);
            ProcessError(error, "Unable to log in");
        }

        private void Logout()
        {
            string error;
            client.LogOut(guid, out error);
            ProcessError(error, "Unable to log out");
        }

        private string GenerateSubtitleFileName(SublightApi.Subtitle subtitle)
        {
            string filename = subtitle.Title.Replace("\"", "");

            string extension = ".sub";

            if (subtitle.SubtitleType == SubtitleType.Srt || subtitle.SubtitleType == SubtitleType.Sub)
            {
                extension = "." + subtitle.SubtitleType.ToString().ToLower();
            }

            if (subtitle.Episode != null && subtitle.Season != null)
            {
                filename += "." + "S" + String.Format("{0:00}", subtitle.Season) + "." + "E" + String.Format("{0:00}", subtitle.Episode);
            }

            return filename + extension;
        }

        private SubtitleLanguage[] GetLanguages(SubtitleSearchQuery query)
        {
            List<SubtitleLanguage> languages = new List<SubtitleLanguage>();

            foreach (var languageCode in query.LanguageCodes)
            {
                SubtitleLanguage lang = GetLanguage(languageCode);
                languages.Add(lang);
            }
            return languages.ToArray();
        }

        private SubtitleLanguage GetLanguage(string languageCode)
        {
            string languageName = Languages.GetLanguageName(languageCode);
            string[] names = Enum.GetNames(typeof(SubtitleLanguage));

            for (int i = 0; i < names.Length; i++)
            {
                string lan = ConvertLanguage(names[i]);

                if (lan.ToLower().Equals(languageName.ToLower()))
                {
                    return (SubtitleLanguage)Enum.Parse(typeof(SubtitleLanguage), names[i]);
                }
            }
            throw new UnsupportedLanguageException();
        }

        private string ConvertLanguage(string languageToConvert)
        {
            if (languageToConvert.Equals("SerbianLatin"))
            {
                return "Serbian";
            }
            if (languageToConvert.Equals("SpanishArgentina"))
            {
                return "Spanish";
            }
            if (languageToConvert.Equals("PortugueseBrazil"))
            {
                return "Portuguese";
            }
            if (languageToConvert.Equals("BosnianLatin"))
            {
                return "Bosnian";
            }
            if (languageToConvert.Equals("Unknown"))
            {
                return "English";
            }

            return languageToConvert;
        }

        private void ProcessError(string error, string message)
        {
            if (error != null)
            {
                throw new Exception(message + " reason: " + error);
            }
        }
    }

    internal class UnsupportedLanguageException : Exception
    {
    }
}