using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SharpCompress.Common;
using SharpCompress.Readers;
using SubtitleDownloader.Core;

namespace SubtitleDownloader.Util
{
    public class FileUtils
    {
        private static string TEMP_FILE = "subtitledownloader_temp";

        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string GetTempFileName()
        {
            int random = new Random().Next(0, 10);
            return Path.Combine(Path.GetTempPath(), TEMP_FILE + random);
        }

        /// <summary>
        /// Gets the filename and path for Subtitle according to the given video file.
        /// <para>
        /// Subtitle file will be located in the same directory with video file and
        /// it will have the same name but the file extension is Subtitle file's own.
        /// </para>
        /// </summary>
        /// <param name="subtitleFile">e.g. "C:\temp\subtitle.srt"</param>
        /// <param name="languageCode">e.g. "eng"</param>
        /// <param name="videoFile">e.g. "C:\Heroes.S04E03.720p.HDTV.X264-DIMENSION.avi</param>
        /// <returns>e.g. "C:\Heroes.S04E03.720p.HDTV.X264-DIMENSION.English.srt"</returns>
        public static string GetFileNameForSubtitle(string subtitleFile, string languageCode, string videoFile)
        {
            // Episode path + Episode file without extension + Language Code + Subtitle file extension
            // -> Subtitle file name with path
            string episodeFile = videoFile;
            string episodeFileNameWithoutExtension = Path.GetFileNameWithoutExtension(episodeFile);
            string episodePath = Path.GetDirectoryName(episodeFile);
            string subtitleFileExtension = Path.GetExtension(subtitleFile);
            string language = Languages.GetLanguageName(languageCode);

            // Relocate and rename Subtitle file to match _episode
            return episodePath + Path.DirectorySeparatorChar
                   + episodeFileNameWithoutExtension + "." + language + subtitleFileExtension;
        }

        /// <summary>
        /// Extracts files from zip or rar file to destination
        /// </summary>
        /// <param name="archiveFile">Zip or rar archive containing the file</param>
        /// <returns>Filenames and paths to extracted files</returns>
        public static List<FileInfo> ExtractFilesFromZipOrRarFile(string archiveFile)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            using (Stream objStream = File.OpenRead(archiveFile))
            {
                var objReader = ReaderFactory.Open(objStream);
                while (objReader.MoveToNextEntry())
                {
                    if (!objReader.Entry.IsDirectory)
                        objReader.WriteEntryToDirectory(tempDirectory);
                }
            }

            return Directory.GetFiles(tempDirectory, "*", SearchOption.AllDirectories)
                            .Select(fileName => new FileInfo(fileName)).ToList();
        }

        public static void WriteNewFile(string fileNameWithPath, byte[] fileData)
        {
            using (var fStream = new FileStream(fileNameWithPath, FileMode.CreateNew))
            {
                using (var bw = new BinaryWriter(fStream))
                {
                    bw.Write(fileData); 
                }
            }
        }
    }
}