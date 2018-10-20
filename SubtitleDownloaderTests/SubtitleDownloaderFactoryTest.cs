﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Core;

namespace SubtitleDownloaderTests
{
    /// <summary>
    ///This is a test class for SubtitleDownloaderFactoryTest and is intended
    ///to contain all SubtitleDownloaderFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubtitleDownloaderFactoryTest
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
        ///A test for GetSubtitleDownloader
        ///</summary>
        [TestMethod()]
        public void GetSubtitleDownloaderTest()
        {
            var names = SubtitleDownloaderFactory.GetSubtitleDownloaderNames();

            foreach (var name in names)
            {
                var downloader = SubtitleDownloaderFactory.GetSubtitleDownloader(name);

                Assert.IsNotNull(downloader);
            }
        }

        /// <summary>
        ///A test for GetSubtitleDownloaderNames
        ///</summary>
        [TestMethod()]
        public void GetSubtitleDownloaderNamesTest()
        {
            var names = SubtitleDownloaderFactory.GetSubtitleDownloaderNames();

            Assert.AreEqual(7, names.Count);
        }
    }
}
