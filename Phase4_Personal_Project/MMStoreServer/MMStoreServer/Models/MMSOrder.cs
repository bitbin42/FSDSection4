using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MMStoreServer.Models {
 
  public class MMSOrder {
  [Key] public int? OrderID {get; set;}
  public int? UserID {get; set;}
  public DateTime? OrderDate {get; set;}
  public string OrderStatus {get; set;}

  //public List<MMSOrderDetail> Details;

  public override string ToString() {return $"{UserID}:{OrderID}:{((DateTime)OrderDate).ToString("yyyy-MM-dd")}:{OrderStatus}";}
  }



  public class MMSOrderDetail {
  public int? OrderID {get; set;}   // composite key defined in the dbcontext code
  public int? ProductID {get; set;}
  public int? OrderQuantity { get; set; }
  public decimal? OrderPrice { get; set;}

  public override string ToString() {return $"{OrderID}:{ProductID}:{OrderQuantity}:{OrderPrice}";}
  }


  // derived class
  public class MMSOrderDetailExtended {
  public int? OrderID {get; set;}
  public int? ProductID {get; set;}
  public string ProductName {get; set;}
  public string ProductCode {get; set;}
  public decimal? OrderPrice { get; set;}
  public int? OrderQuantity { get; set;}

  public override String ToString() {return ProductName.ToString();}
  }

}
