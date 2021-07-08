using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using json=System.Text.Json.JsonSerializer;

namespace MMStoreServer.Controllers {

  public class UsersController : ControllerBase {

    private readonly ILogger<AuthorizeController> moLogger;
    private readonly Repositories.IMMStoreRepository moMMStoreRepo;
    private readonly Support.Helper moHelper;

    /// <summary>
    /// Initialize controller for handing /users requests
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="MMStoreRepository">DB Repository</param>
    /// <param name="Helper">Support Helper object</param>
    public UsersController(ILogger<AuthorizeController> logger,Repositories.IMMStoreRepository MMStoreRepository,Support.Helper Helper) {
    moLogger = logger;
    moMMStoreRepo=MMStoreRepository;
    moHelper=Helper;
    }

    /// <summary>
    /// Return the info for the current user
    /// </summary>
    /// <returns>User object</returns>
    [HttpGet][Route("api/users/me")]
    public async Task<IActionResult> GetMe() {
    IActionResult r=Unauthorized();
    try {
      Models.MMSUser oUser=await moHelper.GetCurrentUser();
      if (oUser!=null) {
        oUser.UserPassword=null;
        r=Ok(oUser);
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,"GET /api/Users/Me",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Return info for selected user by ID (admin only)
    /// </summary>
    /// <param name="UserID">User ID</param>
    /// <returns>User object</returns>
    [HttpGet][Route("api/users/{userid}")]
    public async Task<IActionResult> GetUser(string UserID) {
    IActionResult r=Unauthorized();
    try {
      if (await moHelper.IsAdmin()) {
        Int32 nID=0;
        Models.MMSUser oUser=null;
        if (Int32.TryParse(UserID,out nID))
          oUser=await moMMStoreRepo.GetUserAsync(nID);
        else
          oUser=moMMStoreRepo.GetUser(UserID);
        if (oUser!=null) {
          oUser.UserPassword=null;
          r=Ok(oUser);
          }
        else
          r=NotFound();
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"GET /api/Users/{UserID??""}",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Update info for selected user by ID (admin only)
    /// </summary>
    /// <param name="UserID">User ID</param>
    /// <returns>User object</returns>
    [HttpPatch][Route("api/users/{userid}")]
    public async Task<IActionResult> UpdateUser(string UserID,[FromBody]Models.MMSUser NewInfo) {
    IActionResult r=BadRequest();
    try {
      if (NewInfo!=null // do we have anything to do
      && (string.IsNullOrEmpty(NewInfo.UserEMail)==false || string.IsNullOrEmpty(NewInfo.UserFirstName)==false
      || string.IsNullOrEmpty(NewInfo.UserLastName)==false || string.IsNullOrEmpty(NewInfo.UserIsActive)==false
      || string.IsNullOrEmpty(NewInfo.UserIsAdmin)==false || string.IsNullOrEmpty(NewInfo.UserPassword)==false)) {
        Models.MMSUser oMe=await moHelper.GetCurrentUser();
        if (oMe==null || oMe.UserIsActive!="Y" || oMe.UserIsAdmin!="Y") 
          r=Unauthorized();
        else {
          Models.MMSUser oUser=null;
          Int32 nID=0;
          if (Int32.TryParse(UserID,out nID))
            oUser=await moMMStoreRepo.GetUserAsync(nID);
          else
            oUser=oMe;
          if (oUser!=null && oUser.UserID!=oMe.UserID && (NewInfo.UserID==null || NewInfo.UserID==oUser.UserID)) { // is updating self or different ID in body/url
            if ((!string.IsNullOrEmpty(NewInfo.UserFirstName) && NewInfo.UserFirstName!=oUser.UserFirstName) // any field changed?
            || (!string.IsNullOrEmpty(NewInfo.UserLastName) && NewInfo.UserLastName!=oUser.UserLastName)
            || (!string.IsNullOrEmpty(NewInfo.UserPassword) && NewInfo.UserPassword!=oUser.UserPassword)
            || (!string.IsNullOrEmpty(NewInfo.UserEMail) && NewInfo.UserEMail!=oUser.UserEMail)
            || (!string.IsNullOrEmpty(NewInfo.UserIsActive) && NewInfo.UserIsActive.ToUpper()!=oUser.UserIsActive)
            || (!string.IsNullOrEmpty(NewInfo.UserIsAdmin) && NewInfo.UserIsAdmin.ToUpper()!=oUser.UserIsAdmin)) {
              // copy properties to user object where specified
              if (!string.IsNullOrEmpty(NewInfo.UserFirstName)) oUser.UserFirstName=NewInfo.UserFirstName;
              if (!string.IsNullOrEmpty(NewInfo.UserLastName)) oUser.UserLastName=NewInfo.UserLastName;
              if (!string.IsNullOrEmpty(NewInfo.UserIsActive)) oUser.UserIsActive=NewInfo.UserIsActive.ToUpper();
              if (!string.IsNullOrEmpty(NewInfo.UserIsAdmin)) oUser.UserIsAdmin=NewInfo.UserIsAdmin.ToUpper();
              if (!string.IsNullOrEmpty(NewInfo.UserPassword)) oUser.UserPassword=moMMStoreRepo.PasswordHash(NewInfo.UserPassword);
              if (!string.IsNullOrEmpty(NewInfo.UserEMail)) {
                if (!NewInfo.UserEMail.Equals(oUser.UserEMail,StringComparison.InvariantCultureIgnoreCase)) { // newemail OK?
                  Models.MMSUser oTest= moMMStoreRepo.GetUser(NewInfo.UserEMail);
                  if (oTest!=null) // email found
                    oUser=null;
                  else
                    oUser.UserEMail=NewInfo.UserEMail;
                  }
                }
              if (oUser!=null) {
                moMMStoreRepo.UpdateUser(oUser);
                moMMStoreRepo.SaveChanges();
                r=Ok(oUser); 
                }
              else
                r=Ok(oUser); // no change -- skip DB update
              }
            }
          }
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"PATCH /api/Users/{UserID??""}",Request.QueryString,(NewInfo==null)?"No body":json.Serialize(NewInfo));
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Get list of users
    /// </summary>
    /// <param name="Offset">Starting offset into list</param>
    /// <param name="PageSize">Number of users to return</param>
    /// <param name="Search">User to search for</param>
    /// <param name="Sort">Sort key (Name|ID)</param>
    /// <returns></returns>
    [HttpGet][Route("api/users/list")]
    public async Task<IActionResult> GetUsers(string Offset="0",string PageSize="25",string Sort="Name", string Search = "") {
    IActionResult r=BadRequest();
    try {
      Int32 nOffset=0;
      Int32 nPageSize=25;
      bool b=Int32.TryParse(Offset,out nOffset);
      if (!Int32.TryParse(PageSize,out nPageSize)) nPageSize=25;
      Sort=Sort.ToLower();
      Search=(Search??"").Trim();
      if (nOffset>=0 && nPageSize>0 && nPageSize<=500 && (Sort=="name" || Sort=="id")) {
        Int32 nCount=moMMStoreRepo.GetUserCount(Search);
        IEnumerable<Models.MMSUser> cUsers=await moMMStoreRepo.GetUsersAsync(nOffset,nPageSize,Sort, Search);
        Models.MMSPagedResponse oResp = new Models.MMSPagedResponse(nOffset, nPageSize, nCount, moHelper.CurrentURL(HttpContext.Request) + "?sort={Sort}");
        foreach (Models.MMSUser u in cUsers)
          u.UserPassword=null;
        oResp.Users=cUsers.ToList();
        r=Ok(oResp);
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,"GET /api/Users/List",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

  }
}