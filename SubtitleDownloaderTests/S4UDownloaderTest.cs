using SubtitleDownloader.Implementations.S4U;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Core;
using System.IO;
using System.Collections.Generic;

namespace SubtitleDownloaderTests
{
    
    
    /// <summary>
    ///This is a test class for S4UDownloaderTest and is intended
    ///to contain all S4UDownloaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class S4UDownloaderTest
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
            S4UDownloader target = new S4UDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("heroes", 1, 1);
            query.LanguageCodes = new string[] { "swe" };
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
            S4UDownloader target = new S4UDownloader();

            SearchQuery query = new SearchQuery("batman");
            query.LanguageCodes = new string[] { "swe" };
            query.Year = 2005;

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void EpisodeSearchQuerySearchSubtitlesTest()
        {
            S4UDownloader target = new S4UDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("heroes", 1, 1);
            query.LanguageCodes = new string[] { "swe" };

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void EpisodeSearchQuerySearchSubtitlesTestWithTvdbId()
        {
            S4UDownloader target = new S4UDownloader();

            // Heroes 79501
            EpisodeSearchQuery query = new EpisodeSearchQuery("", 1, 1, 79501);
            query.LanguageCodes = new string[] { "swe" };

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void EmptyEpisodeSearchQuerySearchSubtitlesTest()
        {
            S4UDownloader target = new S4UDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("foobarnotfound", 1, 1);
            query.LanguageCodes = new string[] { "swe" };

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count == 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void ImdbSearchQuerySearchSubtitlesTest()
        {
            S4UDownloader target = new S4UDownloader();

            ImdbSearchQuery query = new ImdbSearchQuery("0372784");
            query.LanguageCodes = new string[] { "swe" };

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }
    }
}
