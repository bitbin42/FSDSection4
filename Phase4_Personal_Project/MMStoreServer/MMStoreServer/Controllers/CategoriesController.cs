using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using json=System.Text.Json.JsonSerializer;
using Microsoft.AspNetCore.Authorization;

namespace MMStoreServer.Controllers {

  public class CategoriesController : ControllerBase {

    private readonly ILogger<AuthorizeController> moLogger;
    private readonly Repositories.IMMStoreRepository moMMStoreRepo;
    private Support.Helper moHelper;

    /// <summary>
    /// Initialize controller for /categories requests
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="MMStoreRepository">DB Repository</param>
    /// <param name="Helper">Support Helper object</param>
    public CategoriesController(ILogger<AuthorizeController> logger,Repositories.IMMStoreRepository MMStoreRepository,Support.Helper Helper) {
    moLogger = logger;
    moMMStoreRepo=MMStoreRepository;
    moHelper=Helper;
    }

    /// <summary>
    /// Get list of categories
    /// </summary>
    /// <param name="Offset">Starting offset into list</param>
    /// <param name="PageSize">Number of Products to return</param>
    /// <param name="Sort">Sort key (Name|ID)</param>
    /// <returns>List of categories</returns>
    [HttpGet][Route("api/categories/list")]
    public async Task<IActionResult> GetCategories(string Offset="0",string PageSize="25",string Sort="Name") {
    IActionResult r=BadRequest();
    try {
      Int32 nOffset=0;
      Int32 nPageSize=25;
      bool b=Int32.TryParse(Offset,out nOffset);
      if (!Int32.TryParse(PageSize,out nPageSize)) nPageSize=25;
      Sort=Sort.ToLower();
      if (nOffset>=0 && nPageSize>0 && nPageSize<=500 && (Sort=="name" || Sort=="id")) {
        bool bInactive =await moHelper.IsAdmin();
        Int32 nCount=moMMStoreRepo.GetCategoryCount(bInactive);
        IEnumerable<Models.MMSCategory> cCats=await moMMStoreRepo.GetCategoriesAsync(nOffset,nPageSize,Sort, bInactive);
        Models.MMSPagedResponse oResp=new Models.MMSPagedResponse(nOffset,nPageSize,nCount,moHelper.CurrentURL(HttpContext.Request)+ "?sort={Sort}");
        oResp.Categories=cCats.ToList();
        r=Ok(oResp);
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,"GET /api/Categories/List",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Return info for selected Category by ID or name
    /// </summary>
    /// <param name="Category">ID or name</param>
    /// <returns>Category object</returns>
    [HttpGet][Route("api/categories/{category}")]
    public async Task<IActionResult> GetCategory(string Category) {
    IActionResult r=BadRequest();
    try {
      Int32 nID=0;
      Models.MMSCategory oCategory=null;
      if (Int32.TryParse(Category,out nID)) 
        oCategory=await moMMStoreRepo.GetCategoryAsync(nID);
      else
        oCategory=moMMStoreRepo.GetCategory(Category);
      if (oCategory!=null) 
        r=Ok(oCategory);
      else
        r=NotFound();
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"GET /api/Categories/{Category??""}",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Add new category
    /// </summary>
    /// <param name="Category">Category object</param>
    /// <returns>Confirmed Category</returns>
    [HttpPost][Route("api/categories")]
    public async Task<IActionResult> AddCategory([FromBody]Models.MMSCategory Category) {
    IActionResult r=BadRequest();
    try {
      if (await moHelper.IsAdmin()) {
        if (Category!=null 
        && string.IsNullOrEmpty(Category.CategoryName)==false 
        && string.IsNullOrEmpty(Category.CategoryInfo)==false) {
          Category.CategoryName=(Category.CategoryName??"").Trim();
          Models.MMSCategory oCurrent=moMMStoreRepo.GetCategory(Category.CategoryName);
          if (oCurrent==null) {
            Category.CategoryID=null;
            Category.CategoryIsActive="Y";
            moMMStoreRepo.AddCategory(Category);
            moMMStoreRepo.SaveChanges();
            oCurrent=moMMStoreRepo.GetCategory(Category.CategoryName);
            if (oCurrent!=null) {
              r=Ok(oCurrent);
              }
            }
          }
        }
      else
        r=Unauthorized();
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,"POST /api/Categories",(User!=null)?json.Serialize(Category):"no category");
      r=StatusCode(500);
      }
    return r;
    }
    
    /// <summary>
    /// Update info for selected category by ID (admin only)
    /// </summary>
    /// <param name="CategoryID">Category ID</param>
    /// <returns>Category object</returns>
    [HttpPatch][Route("api/categories/{categoryid}")]
    public async Task<IActionResult> PatchCategory(string CategoryID,[FromBody]Models.MMSCategory NewInfo) {
    IActionResult r=BadRequest();
    try {
      if (NewInfo!=null // do we have anything to do
      && (string.IsNullOrEmpty(NewInfo.CategoryName)==false || string.IsNullOrEmpty(NewInfo.CategoryInfo)==false
      || string.IsNullOrEmpty(NewInfo.CategoryIsActive)==false)) {
        if (await moHelper.IsAdmin()) {
          Models.MMSCategory oCat=null;
          Int32 nID=0;
          if (Int32.TryParse(CategoryID,out nID))
            oCat=await moMMStoreRepo.GetCategoryAsync(nID);
          else
            oCat=moMMStoreRepo.GetCategory(CategoryID);
          if (oCat!=null && (NewInfo.CategoryID==null || NewInfo.CategoryID==oCat.CategoryID)) { // is same ID in body/url
            if ((!string.IsNullOrEmpty(NewInfo.CategoryInfo) && NewInfo.CategoryInfo!=oCat.CategoryInfo) // any field changed?
            || (!string.IsNullOrEmpty(NewInfo.CategoryName) && NewInfo.CategoryInfo!=oCat.CategoryName)
            || (!string.IsNullOrEmpty(NewInfo.CategoryIsActive) && NewInfo.CategoryIsActive.ToUpper()!=oCat.CategoryIsActive)) {
              bool bActiveChanged=(NewInfo.CategoryIsActive!=oCat.CategoryIsActive);
              // copy properties to user object where specified
              if (!string.IsNullOrEmpty(NewInfo.CategoryInfo)) oCat.CategoryInfo=NewInfo.CategoryInfo;
              if (!string.IsNullOrEmpty(NewInfo.CategoryIsActive)) oCat.CategoryIsActive=NewInfo.CategoryIsActive.ToUpper();
              if (!string.IsNullOrEmpty(NewInfo.CategoryName)) {
                if (!NewInfo.CategoryName.Equals(oCat.CategoryName,StringComparison.InvariantCultureIgnoreCase)) { // category name OK?
                  Models.MMSCategory oTest= moMMStoreRepo.GetCategory(NewInfo.CategoryName);
                  if (oTest!=null) // category found
                    oCat=null;
                  else
                    oCat.CategoryName=NewInfo.CategoryName;
                  }
                }
              if (oCat!=null) {
                moMMStoreRepo.UpdateCategory(oCat);
                moMMStoreRepo.SaveChanges();
                r=Ok(oCat); 
                if (bActiveChanged==true) {
                  moMMStoreRepo.UpdateProductCategoryState((int)oCat.CategoryID,(oCat.CategoryIsActive=="Y"));
                  }
                }
              }
            else
              r=Ok(oCat); // no change -- skip DB update
            }
          }
        else 
          r=Unauthorized();
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"PATCH /api/Categories/{CategoryID??""}",Request.QueryString,(NewInfo==null)?"No body":json.Serialize(NewInfo));
      r=StatusCode(500);
      }
    return r;
    }

    
  }
}
