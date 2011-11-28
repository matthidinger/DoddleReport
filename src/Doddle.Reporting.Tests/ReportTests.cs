using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Doddle.Reporting.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Doddle.Reporting.Writers;

namespace Doddle.Reporting.Tests
{
    /// <summary>
    /// Summary description for ReportTests
    /// </summary>
    [TestClass]
    public class ReportTests
    {
        public ReportTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("Html", Config.Report.DefaultWriter);
            Assert.AreEqual("DataRowStyle", Config.Report.DataRowStyleName);

            //List<Product> products = new List<Product>
            //                             {
            //                                 new Product {ProductID = 1, CategoryID = 1, ProductName = "Chai"},
            //                                 new Product {ProductID = 2, CategoryID = 1, ProductName = "Wheat"},
            //                                 new Product {ProductID = 3, CategoryID = 1, ProductName = "Taco"},
            //                             };



            //IReportSource source = products.ToReportSource();
            //Assert.AreEqual(source.GetFields().Count(), 3);


            //Report report = new Report(source);
            


            //IReportWriter writer = new HtmlReportWriter();
            ////writer.WriteReport(report, null);



        }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
    }
}
