using System;
using SubtitleDownloader.Implementations.Subscene;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Core;
using System.IO;
using System.Collections.Generic;

namespace SubtitleDownloaderTests
{
    
    
    /// <summary>
    ///This is a test class for SubsceneDownloaderTest and is intended
    ///to contain all SubsceneDownloaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubsceneDownloaderTest
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
        public void SaveSubtitleTestRar()
        {
            SubsceneDownloader target = new SubsceneDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("heroes", 4, 6);
            query.LanguageCodes = new string[]  { "dan" };
            List<Subtitle> subtitles = target.SearchSubtitles(query);

            Assert.IsTrue(subtitles.Count > 0);

            List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[0]);

            Assert.IsTrue(fileInfos[0].Exists);
        }

        /// <summary>
        ///A test for SaveSubtitle
        ///</summary>
        [TestMethod()]
        public void SaveSubtitleTestZip()
        {
            SubsceneDownloader target = new SubsceneDownloader();

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
            SubsceneDownloader target = new SubsceneDownloader();

            SearchQuery query = new SearchQuery("terminator salvation");
            query.LanguageCodes = new string[] { "eng", "dan" }; 

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void SearchQuerySearchSubtitlesTest2()
        {
            SubsceneDownloader target = new SubsceneDownloader();

            SearchQuery query = new SearchQuery("batman");
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
            SubsceneDownloader target = new SubsceneDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("heroes", 4, 12);
            query.LanguageCodes = new string[] {"dan"}; 

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void EpisodeSearchQuerySearchSubtitlesTest2()
        {
            SubsceneDownloader target = new SubsceneDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("no ordinary family", 1, 15);

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);

            List<FileInfo> fileInfos = target.SaveSubtitle(actual[0]);

            Assert.IsTrue(fileInfos[0].Exists);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        public void EmptyEpisodeSearchQuerySearchSubtitlesTest()
        {
            SubsceneDownloader target = new SubsceneDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("foobarnotfound", 1, 1);
            query.LanguageCodes = new string[] { "hin" };

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count == 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void ImdbSearchQuerySearchSubtitlesTest()
        {
            SubsceneDownloader target = new SubsceneDownloader();

            ImdbSearchQuery query = new ImdbSearchQuery("0813715");

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }
    }
}
