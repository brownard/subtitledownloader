using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitleDownloader.Core
{
    /// <summary>
    /// Utility class for subtitle languages
    /// </summary>
    public static class Languages
    {
        private static readonly SubLang DefaultLanguage = new SubLang("eng", "English");

        private static readonly SubLang[] aliases = 
        {
            new SubLang("nld", "Dutch")
        };

        private static readonly SubLang[] languages = 
        { 
            new SubLang("bos", "Bosnian"), 

            new SubLang("slv", "Slovenian"), 
            
            new SubLang("hrv", "Croatian"),
 
            new SubLang("srp", "Serbian"), 
            
            new SubLang("eng", "English"), 
            
            new SubLang("spa", "Spanish"), 
            
            new SubLang("fre", "French"), 
            
            new SubLang("gre", "Greek"), 
            
            new SubLang("ger", "German"), 
            
            new SubLang("rus", "Russian"), 
            
            new SubLang("chi", "Chinese"), 
            
            new SubLang("por", "Portuguese"), 
            
            new SubLang("dut", "Dutch"), 
            
            new SubLang("ita", "Italian"), 
            
            new SubLang("rum", "Romanian"), 
            
            new SubLang("cze", "Czech"), 
            
            new SubLang("ara", "Arabic"), 
            
            new SubLang("pol", "Polish"), 
            
            new SubLang("tur", "Turkish"), 
            
            new SubLang("swe", "Swedish"), 
            
            new SubLang("fin", "Finnish"), 

            new SubLang("hun", "Hungarian"), 
            
            new SubLang("dan", "Danish"), 
            
            new SubLang("heb", "Hebrew"), 
            
            new SubLang("est", "Estonian"), 
            
            new SubLang("slo", "Slovak"), 
           
            new SubLang("ind", "Indonesian"), 
            
            new SubLang("per", "Persian"), 
            
            new SubLang("bul", "Bulgarian"), 
            
            new SubLang("jpn", "Japanese"), 
            
            new SubLang("alb", "Albanian"), 
            
            new SubLang("bel", "Belarusian"), 
            
            new SubLang("hin", "Hindi"), 
            
            new SubLang("gle", "Irish"), 
            
            new SubLang("ice", "Icelandic"), 
            
            new SubLang("cat", "Catalan"), 
            
            new SubLang("kor", "Korean"), 
            
            new SubLang("lav", "Latvian"), 
            
            new SubLang("lit", "Lithuanian"), 
            
            new SubLang("mac", "Macedonian"), 
            
            new SubLang("nor", "Norwegian"), 
            
            new SubLang("tha", "Thai"), 
            
            new SubLang("ukr", "Ukrainian"), 
            
            new SubLang("vie", "Vietnamese")
        };

        /// <summary>
        /// Gets the ISO 639-2 language code for given language
        /// </summary>
        /// <param name="languageName">Name of the language in english, e.g. "finnish", "Finnish"</param>
        /// <returns>ISO 639-2 language code for the language, e.g. "eng"
        /// Returns "eng" if language code cannot be found with given language name</returns>
        public static string GetLanguageCode(string languageName)
        {
            var code = FindLanguageCode(languageName);

            return code ?? DefaultLanguage.Code;
        }

        /// <summary>
        /// Gets the ISO 639-2 language code for given language
        /// </summary>
        /// <param name="languageName">Name of the language in english, e.g. "finnish", "Finnish"</param>
        /// <returns>ISO 639-2 language code for the language, e.g. "eng" or null if not found
        /// Returns null if language code cannot be found with given language name</returns>
        public static string FindLanguageCode(string languageName)
        {
            if (String.IsNullOrEmpty(languageName)) 
                throw new ArgumentException("Language name cannot be null or empty!");

            var lang = languages.Where(l => l.Name.Equals(languageName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            return lang == null ? null : lang.Code;
        }

        /// <summary>
        /// Gets the language name
        /// </summary>
        /// <param name="languageCode">Language code for language, e.g. "eng"</param>
        /// <returns>Language name in english, e.g. "Finnish". 
        /// Returns English if language cannot be found with given language code</returns>
        public static string GetLanguageName(string languageCode)
        {
            if (String.IsNullOrEmpty(languageCode)) 
                throw new ArgumentException("Language code cannot be null or empty!");

            if (languageCode.Count() != 3) throw new ArgumentException("Invalid ISO 639-2 language code!");

            var lang = FindLanguageByLanguageCodeInternal(languageCode);

            return lang == null ? DefaultLanguage.Name : lang.Name;
        }

        /// <summary>
        /// Check if the language code given is supported.
        /// </summary>
        /// <param name="languageCode">ISO 639-2 language code, e.g. "eng"</param>
        /// <returns>True if language code is supported, otherwise false</returns>
        public static bool IsSupportedLanguageCode(string languageCode)
        {
            return FindLanguageByLanguageCodeInternal(languageCode) != null;
        }

        /// <summary>
        /// Check if language given is supported
        /// </summary>
        /// <param name="languageName">Name of the language in english, e.g. "Finnish"</param>
        /// <returns>True if language is supported, otherwise false</returns>
        public static bool IsSupportedLanguageName(string languageName)
        {
            return languages.Any(lang => lang.Name.Equals(languageName, 
                                                          StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get all language names supported
        /// </summary>
        /// <returns>Language names in english</returns>
        public static List<String> GetLanguageNames()
        {
            return languages.Select(lang => lang.Name).ToList();
        }

        private static SubLang FindLanguageByLanguageCodeInternal(string languageCode)
        {
            return languages.Where(l => l.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ??
                       aliases.Where(l => l.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }

    internal class SubLang
    {
        public string Code { get; private set; }

        public string Name { get; private set; }

        public SubLang(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}