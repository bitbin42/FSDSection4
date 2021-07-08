using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using json=System.Text.Json.JsonSerializer;

namespace MMStoreServer.Controllers {

  public class ProductsController : ControllerBase {

    private readonly ILogger<AuthorizeController> moLogger;
    private readonly Repositories.IMMStoreRepository moMMStoreRepo;
    private Support.Helper moHelper;

    /// <summary>
    /// Initialize controller for /Products requests
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="MMStoreRepository">DB Repository</param>
    /// <param name="Helper">Support Helper object</param>
    public ProductsController(ILogger<AuthorizeController> logger,Repositories.IMMStoreRepository MMStoreRepository,Support.Helper Helper) {
    moLogger = logger;
    moMMStoreRepo=MMStoreRepository;
    moHelper=Helper;
    }

    /// <summary>
    /// Get list of Products
    /// </summary>
    /// <param name="Offset">Starting offset into list</param>
    /// <param name="PageSize">Number of Products to return</param>
    /// <param name="Sort">Sort key (Name|ID)</param>
    /// <param name="CategoryID">Category ID</param>
    /// <param name="Search">Text search</param>
    /// <returns>Product list</returns>
    [HttpGet][Route("api/products/list")]
    public async Task<IActionResult> GetProducts(string Offset="0",string PageSize="25",string Sort="Name",string CategoryID="",string Search="") {
    IActionResult r=BadRequest();
    try {
      Int32 nOffset=0;
      Int32 nPageSize=25;
      Int32 nCatID=0;
      bool b=Int32.TryParse(Offset,out nOffset);
      if (!Int32.TryParse(PageSize,out nPageSize)) nPageSize=25;
      b=Int32.TryParse(CategoryID,out nCatID); // if invald we will just ignore it
      Sort=Sort.ToLower();
      Search=(Search??"").Trim();
      if (nOffset>=0 && nPageSize>0 && nPageSize<=500 && (Sort=="name" || Sort=="id" || Sort=="code")) {
        bool bInactive = await moHelper.IsAdmin();
        Int32 nCount =moMMStoreRepo.GetProductCount(nCatID,Search,bInactive);
        IEnumerable<Models.MMSProductExtended> cProducts=await moMMStoreRepo.GetProductsAsync(nOffset,nPageSize,Sort,nCatID,Search,bInactive);
        string sOpt=$"?Sort={Sort}"+((nCatID>0)?$"&CategoryID={nCatID}":"")+((string.IsNullOrEmpty(Search)?"":$"&Search={System.Web.HttpUtility.UrlEncode(Search)}"));
        Models.MMSPagedResponse oResp = new Models.MMSPagedResponse(nOffset, nPageSize, nCount, moHelper.CurrentURL(HttpContext.Request) + sOpt);
        oResp.Products=cProducts.ToList();
        r=Ok(oResp);
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,"GET /api/Products/List",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Return info for selected Product by ID
    /// </summary>
    /// <param name="Product">ID or name or code</param>
    /// <returns>Product object</returns>
    [HttpGet][Route("api/products/{product}")]
    public async Task<IActionResult> GetProduct(string Product) {
    IActionResult r=BadRequest();
    try {
      Int32 nID=0;
      Models.MMSProduct oProduct=null;
      Product=(Product??"").Trim();
      if (Int32.TryParse(Product,out nID)) 
        oProduct=await moMMStoreRepo.GetProductAsync(nID);
      else
        oProduct=moMMStoreRepo.GetProduct(Product);
      if (oProduct!=null) 
        r=Ok(oProduct);
      else
        r=NotFound();
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"GET /api/Products/{Product??""}",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Add new product
    /// </summary>
    /// <param name="Product">Product object</param>
    /// <returns>Confirmed Product</returns>
    [HttpPost][Route("api/products")]
    public async Task<IActionResult> AddProduct([FromBody]Models.MMSProduct Product) {
    IActionResult r=BadRequest();
    try {
      if (await moHelper.IsAdmin()) {
        if (Product.CategoryID!=null) { // must be valid category ID
          Models.MMSCategory oCat=await moMMStoreRepo.GetCategoryAsync((int)Product.CategoryID);
          if (oCat!=null) {
            if (Product!=null 
            && (Product.ProductPrice!=null && Product.ProductPrice>=0)
            && string.IsNullOrEmpty(Product.ProductName)==false 
            && string.IsNullOrEmpty(Product.ProductCode)==false 
            && string.IsNullOrEmpty(Product.ProductInfo)==false) {
              Product.ProductName=(Product.ProductName??"").Trim();
              Product.ProductCode=(Product.ProductCode??"").Trim();
              Models.MMSProduct oCurrent=moMMStoreRepo.GetProduct(Product.ProductName);
              Models.MMSProduct oCurrent2=(oCurrent!=null)?null:moMMStoreRepo.GetProduct(Product.ProductCode);
              if (oCurrent==null && oCurrent2==null) {
                Product.ProductID=null;
                Product.ProductIsActive="Y";
                moMMStoreRepo.AddProduct(Product);
                moMMStoreRepo.SaveChanges();
                oCurrent=moMMStoreRepo.GetProduct(Product.ProductName);
                if (oCurrent!=null) {
                  r=Ok(oCurrent);
                  }
                }
              }
            }
          }
        }
      else
        r=Unauthorized();
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,"POST /api/Products",(Product!=null)?json.Serialize(Product):"no product");
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Update info for selected product by ID (admin only)
    /// </summary>
    /// <param name="ProductID">Product ID</param>
    /// <returns>Product object</returns>
    [HttpPatch][Route("api/products/{productid}")]
    public async Task<IActionResult> UpdateProduct(string ProductID,[FromBody]Models.MMSProduct NewInfo) {
    IActionResult r=BadRequest();
    try {
      if (NewInfo!=null // do we have anything to do
      && (string.IsNullOrEmpty(NewInfo.ProductName)==false || string.IsNullOrEmpty(NewInfo.ProductInfo)==false
      || (NewInfo.ProductPrice!=null && NewInfo.ProductPrice<0)
      || string.IsNullOrEmpty(NewInfo.ProductIsActive)==false || string.IsNullOrEmpty(NewInfo.ProductCode)==false)) {
        if (await moHelper.IsAdmin()) {
          Models.MMSProduct oProd=null;
          Int32 nID=0;
          if (Int32.TryParse(ProductID,out nID))
            oProd=await moMMStoreRepo.GetProductAsync(nID); // get existing record
          if (oProd!=null) {
            if (NewInfo.CategoryID!=null) { // verify category if specified
              Models.MMSCategory oCat=await moMMStoreRepo.GetCategoryAsync((int)NewInfo.CategoryID);
              if (oCat==null)
                oProd=null;
              }
            if (oProd!=null && (NewInfo.ProductID==null || NewInfo.ProductID==oProd.ProductID)) { // is same ID in body/url
            if ((!string.IsNullOrEmpty(NewInfo.ProductInfo) && NewInfo.ProductInfo!=oProd.ProductInfo) // any field changed?
            || (!string.IsNullOrEmpty(NewInfo.ProductName) && NewInfo.ProductInfo!=oProd.ProductName)
            || (!string.IsNullOrEmpty(NewInfo.ProductCode) && NewInfo.ProductInfo!=oProd.ProductCode)
            || (NewInfo.ProductPrice!=null && NewInfo.ProductPrice!=oProd.ProductPrice)
            || (!string.IsNullOrEmpty(NewInfo.ProductIsActive) && NewInfo.ProductIsActive.ToUpper()!=oProd.ProductIsActive)) {
              // copy properties to user object where specified
              if (!string.IsNullOrEmpty(NewInfo.ProductInfo)) oProd.ProductInfo=NewInfo.ProductInfo;
              if (NewInfo.ProductPrice!=null) oProd.ProductPrice=NewInfo.ProductPrice;
              if (NewInfo.CategoryID!=null) oProd.CategoryID=NewInfo.CategoryID;
              if (!string.IsNullOrEmpty(NewInfo.ProductIsActive)) oProd.ProductIsActive=NewInfo.ProductIsActive.ToUpper();
              if (!string.IsNullOrEmpty(NewInfo.ProductName)) {
                if (!NewInfo.ProductName.Equals(oProd.ProductName,StringComparison.InvariantCultureIgnoreCase)) { // Product name OK?
                  Models.MMSProduct oTest= moMMStoreRepo.GetProduct(NewInfo.ProductName);
                  if (oTest!=null) // Product found
                    oProd=null;
                  else
                    oProd.ProductName=NewInfo.ProductName;
                  }
                }
              if (oProd!=null && string.IsNullOrEmpty(NewInfo.ProductCode)==false) {
                if (!NewInfo.ProductCode.Equals(oProd.ProductCode,StringComparison.InvariantCultureIgnoreCase)) { // Product code OK?
                  Models.MMSProduct oTest= moMMStoreRepo.GetProduct(NewInfo.ProductCode);
                  if (oTest!=null) // Product found
                    oProd=null;
                  else
                    oProd.ProductCode=NewInfo.ProductCode;
                  }
                }
              if (oProd!=null) {
                moMMStoreRepo.UpdateProduct(oProd);
                moMMStoreRepo.SaveChanges();
                r=Ok(oProd); 
                }
              }
            else
              r=Ok(oProd); // no change -- skip DB update
            }
            }
          }
        else
          r=Unauthorized();
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"PATCH /api/Categories/{ProductID??""}",Request.QueryString,(NewInfo==null)?"No body":json.Serialize(NewInfo));
      r=StatusCode(500);
      }
    return r;
    }
    
    
  }
}
