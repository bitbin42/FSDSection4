using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MMStoreServer.Support {
  public class Helper {

  private readonly IHttpContextAccessor moContext;
  private Repositories.IMMStoreRepository moMMStoreRepo {get; set;}

    /// <param name="HTTPContext">HTTP context for Request/Response objects</param>
  public Helper(IHttpContextAccessor HTTPContext,Repositories.IMMStoreRepository MMStoreRepository) {
  moMMStoreRepo=MMStoreRepository;
  moContext=HTTPContext;
  }

  /// <summary>
  /// Get user record for logged on user
  /// </summary>
  /// <returns>User object or null</returns>
  public async Task<Models.MMSUser> GetCurrentUser() {
  Models.MMSUser oUser=null;
  ClaimsIdentity oJWT = moContext.HttpContext.User.Identity as ClaimsIdentity;
  if (oJWT != null && oJWT.Claims!=null) {
    System.Security.Claims.Claim oClaim=oJWT.Claims.Where(c => c.Type=="UserID").FirstOrDefault();
    if (oClaim!=null) {
      Int32 nID=0;
      if (Int32.TryParse(oClaim.Value,out nID)) 
        oUser=await moMMStoreRepo.GetUserAsync(nID);
      } 
    }
  return oUser;
  }

  /// <summary>
  /// Check if the current user has admin rights
  /// </summary>
  /// <returns>True if admin user</returns>
  public async Task<bool> IsAdmin() {
  Models.MMSUser oUser=await GetCurrentUser();
  return (oUser!=null && oUser.UserIsAdmin=="Y" && oUser.UserIsActive=="Y");
  }

  /// <summary>
  /// Check if the current user is valid
  /// </summary>
  /// <returns>True if active user</returns>
  public async Task<bool> IsValid() {
  Models.MMSUser oUser=await GetCurrentUser();
  return (oUser!=null && oUser.UserIsActive=="Y");
  }

  /// <summary>
  /// Get the current action URL
  /// </summary>
  /// <param name="Request">Http.HttpRequest object</param>
  /// <returns>URL of current request</returns>
  public string CurrentURL(Microsoft.AspNetCore.Http.HttpRequest Request) {
  string sURL=Request.Scheme+"://"+Request.Host.ToString()+Request.Path;
  return sURL;
  }

  }
}
