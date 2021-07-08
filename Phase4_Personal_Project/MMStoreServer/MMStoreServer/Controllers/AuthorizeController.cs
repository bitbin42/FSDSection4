using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using json=System.Text.Json.JsonSerializer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
//!!using Microsoft.AspNetCore.Cors;

namespace MMStoreServer.Controllers {

  public class AuthorizeController : ControllerBase {

    private readonly ILogger<AuthorizeController> moLogger;
    private readonly IConfiguration moConfig;
    private readonly Repositories.IMMStoreRepository moMMStoreRepo;
    private Support.Helper moHelper;

    /// <summary>
    /// Initialize controller for /authorize requests
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="MMStoreRepository">DB Repository</param>
    /// <param name="Helper">Support Helper object</param>
    public AuthorizeController(ILogger<AuthorizeController> Logger,IConfiguration Config,
                               Repositories.IMMStoreRepository MMStoreRepository,Support.Helper Helper) {
    moLogger = Logger;
    moConfig=Config;
    moMMStoreRepo=MMStoreRepository;
    moHelper=Helper;
    }

    /// <summary>
    /// Handle logon request
    /// </summary>
    /// <param name="User">User object (UserEMail and UserPassword fields)</param>
    /// <returns>Full user object</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("api/authorize/login")]
    public IActionResult Login([FromBody]Models.MMSUser User) {
    IActionResult r=Unauthorized();
    try {
      if (User!=null && string.IsNullOrEmpty(User.UserEMail)==false && string.IsNullOrEmpty(User.UserPassword)==false) {
        User=moMMStoreRepo.GetUser(User.UserEMail,User.UserPassword);
        if (User!=null && User.UserID!=null && User.UserID>0) {
          r=TokenResponse(User);
          }
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,"POST /api/Authorize/Login",(User!=null)?json.Serialize(User):"no user");
      r=StatusCode(500);
      }
    return r;
    }

    private IActionResult TokenResponse(Models.MMSUser User) {
    string sToken = GenerateJSONWebToken(User);
    Models.MMSUserExtended oResp=new Models.MMSUserExtended() {UserID=(int)User.UserID,UserEMail=User.UserEMail,
      UserFirstName = User.UserFirstName, UserLastName =User.UserLastName, UserIsAdmin=User.UserIsAdmin, AccessToken=sToken};
    return Ok(oResp);
    }

    private string GenerateJSONWebToken(Models.MMSUser User) {
    SymmetricSecurityKey oKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(moConfig["Jwt:Key"]));
    SigningCredentials oCred = new SigningCredentials(oKey, SecurityAlgorithms.HmacSha256);
    System.Security.Claims.Claim oClaim=new System.Security.Claims.Claim("UserID",User.UserID.ToString());
    List<System.Security.Claims.Claim> cClaims=new List<System.Security.Claims.Claim>() {oClaim};
    JwtSecurityToken oToken = new JwtSecurityToken(moConfig["Jwt:Issuer"],moConfig["Jwt:Issuer"],cClaims,
                                                  expires: DateTime.Now.AddDays(1), signingCredentials: oCred);
    string sToken=new JwtSecurityTokenHandler().WriteToken(oToken);
    return sToken;
    }


    /// <summary>
    /// Handle registration request
    /// </summary>
    /// <param name="User">User object</param>
    /// <returns>Confirmed user</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("api/authorize/register")]
    public IActionResult Register([FromBody]Models.MMSUser User) {
    IActionResult r=BadRequest();
    try {
      if (User!=null 
      && string.IsNullOrEmpty(User.UserEMail)==false 
      && string.IsNullOrEmpty(User.UserPassword)==false
      && string.IsNullOrEmpty(User.UserFirstName)==false
      && string.IsNullOrEmpty(User.UserLastName)==false) {
        Models.MMSUser oCurrent=moMMStoreRepo.GetUser(User.UserEMail);
        if (oCurrent==null) {
          User.UserID=null;
          User.UserIsAdmin="N";
          User.UserIsActive="Y";
          User.UserPassword=moMMStoreRepo.PasswordHash(User.UserPassword);
          moMMStoreRepo.AddUser(User);
          moMMStoreRepo.SaveChanges();
          oCurrent=moMMStoreRepo.GetUser(User.UserEMail);
          if (oCurrent!=null) {
            r=TokenResponse(oCurrent);
            }
          }
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,"POST /api/Authorize/Register",(User!=null)?json.Serialize(User):"no user");
      r=StatusCode(500);
      }
    return r;
    }

 
  }
}
