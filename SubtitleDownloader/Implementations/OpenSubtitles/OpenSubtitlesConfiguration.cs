using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SubtitleDownloader.Implementations.OpenSubtitles
{
    [XmlRoot("Configuration")]
    public class OpenSubtitlesConfiguration
    {
        [XmlElement("Username")]
        public string Username  = "";

        [XmlElement("Password")] 
        public string Password = "";

        [XmlElement("Language")]
        public string Language = "";
    }
}
