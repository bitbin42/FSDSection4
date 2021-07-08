using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MMStoreServer.Models {
 
  public class MMSProduct {
  [Key] public int? ProductID {get; set;}
  public int? CategoryID {get; set;}
  public string ProductCode {get; set;}
  public string ProductName {get; set;}
  public string ProductInfo {get; set;}
  public decimal? ProductPrice {get; set;}
  public string ProductIsActive {get; set;}

  //public MMSCategory Category {get; set;}

  public override String ToString() {return $"{ProductName} {ProductID}";}
  }


  // derived class
  public class MMSProductExtended {
  public int? ProductID {get; set;}
  public int? CategoryID {get; set;}
  public string ProductCode {get; set;}
  public string ProductName {get; set;}
  public string ProductInfo {get; set;}
  public decimal? ProductPrice {get; set;}
  public string ProductIsActive {get; set;}
  public string CategoryName {get; set;}

  public override string ToString() {return $"{ProductName} {ProductID}";}
  }

}
