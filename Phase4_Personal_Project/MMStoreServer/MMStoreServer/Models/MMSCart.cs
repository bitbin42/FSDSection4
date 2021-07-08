using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MMStoreServer.Models {

  public class MMSCartItem
  {
    public int? UserID { get; set; }  // composite key defined in the dbcontext code
    public int? ProductID { get; set; }
    public int? Quantity { get; set; }

    public override String ToString() { return $"{UserID}:{ProductID}:{Quantity}"; }
  }

  public class MMSCartItemExtended {
    public int? UserID { get; set; }  // composite key defined in the dbcontext code
    public int? ProductID { get; set; }
    public int? Quantity { get; set; }
    public string ProductName { get; set; }
    public decimal? ProductPrice { get; set; }
    public string ProductCode { get; set; }
    public string CategoryName { get; set; }

    public override String ToString() { return $"{UserID}:{ProductID}:{Quantity}"; }
  }

}
