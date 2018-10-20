using System;
using System.Linq;
using SubtitleDownloader.Implementations.Podnapisi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Core;
using System.Collections.Generic;
using System.IO;

namespace SubtitleDownloaderTests
{
    
    
    /// <summary>
    ///This is a test class for PodnapisiDownloaderTest and is intended
    ///to contain all PodnapisiDownloaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PodnapisiDownloaderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SaveSubtitle
        ///</summary>
        [TestMethod()]
        public void SaveSubtitleTest()
        {
            PodnapisiDownloader target = new PodnapisiDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("heroes", 1, 1);
            List<Subtitle> subtitles = target.SearchSubtitles(query);

            Assert.IsTrue(subtitles.Count > 0);

            List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[0]);

            Assert.IsTrue(fileInfos[0].Exists);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void SearchQuerySearchSubtitlesTest()
        {
            PodnapisiDownloader target = new PodnapisiDownloader();

            SearchQuery query = new SearchQuery("heroes");

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void EpisodeSearchQuerySearchSubtitlesTest()
        {
            PodnapisiDownloader target = new PodnapisiDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("heroes", 1, 1);
            query.LanguageCodes = new string[] { "dut", "eng" };

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
            Assert.IsNotNull(actual.Find(s => s.LanguageCode.Equals("dut")));
            Assert.IsNotNull(actual.Find(s => s.LanguageCode.Equals("eng")));
        }

        [TestMethod()]
        public void EpisodeSearchQuerySearchSubtitlesTest2()
        {
          PodnapisiDownloader target = new PodnapisiDownloader();

          EpisodeSearchQuery query = new EpisodeSearchQuery("Marvel's Agents of S.H.I.E.L.D.", 1, 13);
          query.LanguageCodes = new string[] { "eng" };

          List<Subtitle> actual = target.SearchSubtitles(query);

          Assert.AreEqual(actual.Count, 28);
          Assert.IsNotNull(actual.Find(s => s.LanguageCode.Equals("eng")));
          Assert.IsTrue(actual.All(s => s.FileName.Length < 250));
        }

        [TestMethod()]
        public void EpisodeSearchQuerySearchSubtitlesTest3()
        {
            PodnapisiDownloader target = new PodnapisiDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("Homeland", 4, 5);
            query.LanguageCodes = new string[] { "eng" };

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.AreEqual(actual.Count, 31);
            Assert.IsNotNull(actual.Find(s => s.LanguageCode.Equals("eng")));
            Assert.IsTrue(actual.All(s => s.FileName.Length < 250));
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void EmptyEpisodeSearchQuerySearchSubtitlesTest()
        {
            PodnapisiDownloader target = new PodnapisiDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("foobarnotfound", 1, 1);

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count == 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotImplementedException))]
        public void ImdbSearchQuerySearchSubtitlesTest()
        {
            PodnapisiDownloader target = new PodnapisiDownloader();

            ImdbSearchQuery query = new ImdbSearchQuery("0813715");

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }
    }
}
