using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMStoreServer.Models {

  public class MMSPagedResponse {
  public int? Offset {get; set;}
  public int? PageSize {get; set;}
  public int? TotalRecords {get; set;}
  public string NextPage { get; set; }
  public string PrevPage { get; set; }
  public List<MMSUser> Users {get; set;}
  public List<MMSCategory> Categories {get; set;}
  public List<MMSProductExtended> Products {get; set;}
  public List<MMSOrder> Orders {get; set;}

  /// <summary>
  /// Initialize response header
  /// </summary>
  /// <param name="Offset">Starting offset</param>
  /// <param name="PageSize">Current page size</param>
  /// <param name="TotalRecords">Number of records</param>
  /// <param name="BaseURL">URL for Next/Previous links</param>
  public MMSPagedResponse(Int32 Offset = 0, Int32 PageSize = 25,Int32 TotalRecords= 0, string BaseURL="") {
  this.Offset = Offset;
  this.PageSize = PageSize;
  this.TotalRecords = TotalRecords;
  BaseURL+=BaseURL.Contains("?")?"&":"?";
  if (Offset > 0)
    this.PrevPage = BaseURL + $"offset={((Offset < PageSize) ? 0 : Offset - PageSize)}&pagesize={PageSize}";
  if (Offset + PageSize < TotalRecords)
    this.NextPage = BaseURL + $"offset={Offset + PageSize}&pagesize={PageSize}";
  }

  }

}
