using System;
using SubtitleDownloader.Implementations.SubtitleSource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Core;
using System.IO;
using System.Collections.Generic;

namespace SubtitleDownloaderTests
{
    
    
    /// <summary>
    ///This is a test class for SubtitleSourceDownloaderV2Test and is intended
    ///to contain all SubtitleSourceDownloaderV2Test Unit Tests
    ///</summary>
    [TestClass()]
    public class SubtitleSourceDownloaderV2Test
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
        //[TestMethod()]
        public void SaveSubtitleTest()
        {
            SubtitleSourceDownloader target = new SubtitleSourceDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("heroes", 1, 1);
            List<Subtitle> subtitles = target.SearchSubtitles(query);

            Assert.IsTrue(subtitles.Count > 0);

            List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[0]);

            Assert.IsTrue(fileInfos[0].Exists);
        }

        /// <summary>
        ///A test for SaveSubtitle
        ///</summary>
        //[TestMethod()]
        public void SaveSubtitleWithMultipleFilesTest()
        {
            SubtitleSourceDownloader target = new SubtitleSourceDownloader();

            List<FileInfo> fileInfos = target.SaveSubtitle(
                new Subtitle("52557", "foo", "foo", "eng"));

            Assert.IsTrue(fileInfos[0].Exists);
            Assert.IsTrue(fileInfos[1].Exists);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        //[TestMethod()]
        public void SearchQuerySearchSubtitlesTest()
        {
            SubtitleSourceDownloader target = new SubtitleSourceDownloader();

            SearchQuery query = new SearchQuery("heroes");

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        //[TestMethod()]
        public void EpisodeSearchQuerySearchSubtitlesTest()
        {
            SubtitleSourceDownloader target = new SubtitleSourceDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("heroes", 1, 1);

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        //[TestMethod()]
        public void EmptyEpisodeSearchQuerySearchSubtitlesTest()
        {
            SubtitleSourceDownloader target = new SubtitleSourceDownloader();

            EpisodeSearchQuery query = new EpisodeSearchQuery("foobarnotfound", 1, 1);

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count == 0);
        }

        /// <summary>
        ///A test for SearchSubtitles
        ///</summary>
        //[TestMethod()]
        public void ImdbSearchQuerySearchSubtitlesTest()
        {
            SubtitleSourceDownloader target = new SubtitleSourceDownloader();

            ImdbSearchQuery query = new ImdbSearchQuery("0813715");

            List<Subtitle> actual = target.SearchSubtitles(query);

            Assert.IsTrue(actual.Count > 0);
        }
    }
}
