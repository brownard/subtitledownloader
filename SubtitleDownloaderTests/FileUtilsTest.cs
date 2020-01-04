using System.Collections.Generic;
using System.IO;
using SubtitleDownloader.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SubtitleDownloaderTests
{
    
    
    /// <summary>
    ///This is a test class for FileUtilsTest and is intended
    ///to contain all FileUtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileUtilsTest
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
        ///A test for GetFileNameForSubtitle
        ///</summary>
        [TestMethod()]
        public void GetFileNameForSubtitleTest()
        {
            string subtitleFile = @"C:\temp\subtitle.srt";
            string languageCode = "dut";
            string videoFile = @"C:\foo.release.avi";

            string expected = @"C:\\foo.release.Dutch.srt";
            string actual= FileUtils.GetFileNameForSubtitle(subtitleFile, languageCode, videoFile);
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetFileNameForSubtitle
        ///</summary>
        [TestMethod()]
        public void GetFileNameForSubtitleTest2()
        {
            string subtitleFile = @"C:\temp\subtitle.srt";
            string languageCode = "nld";
            string videoFile = @"C:foo.release.avi";

            string expected = @"C:\foo.release.Dutch.srt";
            string actual = FileUtils.GetFileNameForSubtitle(subtitleFile, languageCode, videoFile);

            Assert.AreEqual(expected, actual);
        }
    }
}
