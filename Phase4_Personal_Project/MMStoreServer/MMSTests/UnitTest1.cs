using NUnit.Framework;

namespace MMSTests {
  [TestFixture]
  public class Tests {

  [Test]
  public void Test1() {
  MMStoreServer.Models.MMSPagedResponse oResp=new MMStoreServer.Models.MMSPagedResponse(100,50,500,"testbase");
  Assert.That(oResp.Offset,Is.EqualTo(100));
  Assert.That(oResp.PageSize,Is.GreaterThan(49)); // just to show different tests
  Assert.That(oResp.PageSize,Is.LessThan(51)); 
  Assert.That(oResp.TotalRecords,Is.InRange(450,550));
  Assert.That(oResp.NextPage,Is.EqualTo("testbase?offset=150&pagesize=50"));
  Assert.NotNull(oResp.PrevPage);
  oResp=new MMStoreServer.Models.MMSPagedResponse(0,50,500,"testbase");
  Assert.That(oResp.PrevPage,Is.Null);
  oResp=new MMStoreServer.Models.MMSPagedResponse(501,50,500,"testbase");
  Assert.IsNull(oResp.NextPage); // alternate syntax
  }


  }
}