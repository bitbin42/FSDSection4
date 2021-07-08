using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MMStoreServer.Models {
 
  public class MMSCategory {
  [Key] public int? CategoryID {get; set;}
  public string CategoryName {get; set;}
  public string CategoryInfo {get; set;}
  public string CategoryIsActive {get; set;}

  //public List<MMSProduct> Products;

  public override String ToString() {return $"{CategoryName} {CategoryID}";}
  }

}
