using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MMStoreServer.Models {
 
  public class MMSUser {
  [Key] public int? UserID {get; set;}
  public string UserEMail {get; set;}
  public string UserPassword {get; set;}
  public string UserFirstName {get; set;}
  public string UserLastName {get; set;}
  public string UserIsAdmin {get; set;}
  public string UserIsActive {get; set;}

  public override string ToString() {return $"{UserEMail} [{UserID}]";}
  }


  public class MMSUserExtended {
  public int UserID { get; set; }
  public string UserEMail {get; set;}
  public string UserFirstName {get; set;}
  public string UserLastName {get; set;}
  public string UserIsAdmin {get; set;}
  public string AccessToken {get; set;}

  public override string ToString() {return $"{UserEMail}";}
  }

}
