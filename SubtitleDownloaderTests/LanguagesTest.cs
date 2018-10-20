using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Core;

namespace SubtitleDownloaderTests
{
    
    
    /// <summary>
    ///This is a test class for LanguagesTest and is intended
    ///to contain all LanguagesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LanguagesTest
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
        ///A test for IsSupportedLanguageName
        ///</summary>
        [TestMethod()]
        public void IsSupportedLanguageNameTest()
        {
            Assert.IsTrue(Languages.IsSupportedLanguageName("english"));

            Assert.IsTrue(Languages.IsSupportedLanguageName("fInnIsH"));

            Assert.IsFalse(Languages.IsSupportedLanguageName("NotExistingLanguage"));
        }

        /// <summary>
        ///A test for IsSupportedLanguageCode
        ///</summary>
        [TestMethod()]
        public void IsSupportedLanguageCodeTest()
        {
            Assert.IsTrue(Languages.IsSupportedLanguageCode("dut"));

            Assert.IsTrue(Languages.IsSupportedLanguageCode("nld"));

            Assert.IsTrue(Languages.IsSupportedLanguageCode("fin"));

            Assert.IsFalse(Languages.IsSupportedLanguageCode("foo"));
        }

        /// <summary>
        ///A test for GetLanguageName
        ///</summary>
        [TestMethod()]
        public void GetLanguageNameTest()
        {
            Assert.IsTrue(Languages.GetLanguageName("eNG").Equals("English"));

            Assert.IsTrue(Languages.GetLanguageName("dut").Equals("Dutch"));

            Assert.IsTrue(Languages.GetLanguageName("nld").Equals("Dutch"));

            Assert.IsTrue(Languages.GetLanguageName("fin").Equals("Finnish"));
        }

        /// <summary>
        ///A test for FindLanguageCode
        ///</summary>
        [TestMethod()]
        public void FindLanguageCodeTest()
        {
            Assert.IsTrue(Languages.FindLanguageCode("ENGLISH").Equals("eng"));

            Assert.IsTrue(Languages.FindLanguageCode("Foobar") == null);
        }

        /// <summary>
        ///A test for GetLanguageCode
        ///</summary>
        [TestMethod()]
        public void GetLanguageCodeTest()
        {
            Assert.IsTrue(Languages.GetLanguageCode("ENGLISH").Equals("eng"));

            Assert.IsTrue(Languages.GetLanguageCode("Foobar").Equals("eng"));            // Default
        }
    }
}
