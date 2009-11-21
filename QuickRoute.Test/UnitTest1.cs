using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickRoute.BusinessEntities;

namespace QuickRoute.Test
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class LapTest
  {
    public LapTest()
    {
      //
      // TODO: Add constructor logic here
      //
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
      Lap lap = new Lap();
      Assert.AreEqual(lap, lap);
    }

    [TestMethod]
    public void TestDistanceUnit()
    {
      Distance d = new Distance();
      Scale s = Distance.CreateScale(0, 100, 7, false);
      Assert.AreEqual(6, s.Markers.Count);
    }

    [TestMethod]
    public void TestElapsedTimeToString()
    {
      ElapsedTime p = new ElapsedTime(130);
      Assert.AreEqual("2:10", p.ToString());
    }

    [TestMethod]
    public void TestStringToElapsedTime()
    {
      ElapsedTime p = new ElapsedTime();
      p.FromString("2:10");
      Assert.AreEqual(130, p.Value);
    }

    [TestMethod]
    public void TestTimeOfDayScale()
    {
      Scale s = TimeOfDay.CreateScale(DateTime.Parse("2008-08-01 10:22"), DateTime.Parse("2008-08-01 11:30"), 10, true);
      Assert.AreEqual(DateTime.Parse("2008-08-01 10:15"), ((TimeOfDay)s.Start).Value);
      Assert.AreEqual(DateTime.Parse("2008-08-01 11:30"), ((TimeOfDay)s.End).Value);
      Assert.AreEqual(6, s.Markers.Count);
    }

  }
}
