using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using json=System.Text.Json.JsonSerializer;

namespace MMStoreServer.Controllers {

  public class OrdersController : ControllerBase {

    private readonly ILogger<AuthorizeController> moLogger;
    private readonly Repositories.IMMStoreRepository moMMStoreRepo;
    private readonly Support.Helper moHelper;

    /// <summary>
    /// Initialize controller for handing /orders requests
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="MMStoreRepository">DB Repository</param>
    /// <param name="Helper">Support Helper object</param>
    public OrdersController(ILogger<AuthorizeController> logger,Repositories.IMMStoreRepository MMStoreRepository,Support.Helper Helper) {
    moLogger = logger;
    moMMStoreRepo=MMStoreRepository;
    moHelper=Helper;
    }

    /// <summary>
    /// Get order header
    /// </summary>
    /// <param name="OrderID">Order ID number</param>
    /// <param name="UserID">User ID number</param>
    /// <returns>Order object</returns>
    [HttpGet][Route("/api/orders/{OrderID}")]
    public async Task<IActionResult> GetOrder(string OrderID,string UserID) {
    IActionResult r=Unauthorized();
    try {
      Models.MMSUser oMe=await moHelper.GetCurrentUser();
      Int32 nOrderID=0;
      bool b=Int32.TryParse(OrderID,out nOrderID);
      if (nOrderID<1)
        r=BadRequest();
      else {
        Int32 nUserID=0;
        if (string.IsNullOrEmpty(UserID))
          nUserID=(int)oMe.UserID;
        else {
          b=Int32.TryParse(UserID,out nUserID);
          }
        if (oMe!=null && (nUserID==oMe.UserID || oMe.UserIsAdmin=="Y")) {
          Models.MMSOrder oOrder=moMMStoreRepo.GetOrder(nOrderID);
          if (oOrder!=null && oOrder.UserID==nUserID)
            r=Ok(oOrder);
          else
            r=NotFound();
          }
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"GET /api/orders/{(OrderID??"").ToString()}",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Get order header
    /// </summary>
    /// <param name="OrderID">Order ID number</param>
    /// <param name="UserID">User ID number</param>
    /// <returns>Order object</returns>
    [HttpGet][Route("/api/orders/{OrderID}/details")]
    public async Task<IActionResult> GetOrderDetails(string OrderID,string UserID) {
    IActionResult r=Unauthorized();
    try {
      Models.MMSUser oMe=await moHelper.GetCurrentUser();
      Int32 nOrderID=0;
      bool b=Int32.TryParse(OrderID,out nOrderID);
      if (nOrderID<1)
        r=BadRequest();
      else {
        Int32 nUserID=0;
        if (string.IsNullOrEmpty(UserID))
          nUserID=(int)oMe.UserID;
        else {
          b=Int32.TryParse(UserID,out nUserID);
          }
        if (oMe!=null && (nUserID==oMe.UserID || oMe.UserIsAdmin=="Y")) {
          Models.MMSOrder oOrder=moMMStoreRepo.GetOrder(nOrderID);
          if (oOrder!=null && oOrder.UserID==nUserID) {
            List<Models.MMSOrderDetailExtended> cDetails=moMMStoreRepo.GetOrderDetails(nOrderID).ToList();
            if (cDetails!=null)
              r=Ok(new {details=cDetails});
            else
              r=NotFound();
            }
          }
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"GET /api/orders/{(OrderID??"").ToString()}",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }


    /// <summary>
    /// Get all orders for the user
    /// </summary>
    /// <param name="Offset">Records to skip</param>
    /// <param name="PageSize">Records to return</param>
    /// <param name="Status">Status filter</param>
    /// <param name="UserID">Order owner (admins only)</param>
    /// <returns>List of order objects</returns>
    [HttpGet][Route("/api/orders")]
    public async Task<IActionResult> GetOrders(string Offset,string PageSize,string Status,string UserID) {
    IActionResult r=Unauthorized();
    try {
      Int32 nOffset=0;
      Int32 nPageSize=25;
      Int32 nUserID=0;
      string sStatus=(Status??"").Trim().ToLower();
      if (sStatus == "all") {sStatus = "";}
      bool b=Int32.TryParse(Offset,out nOffset);
      if (!Int32.TryParse(PageSize,out nPageSize)) nPageSize=25;
      Models.MMSUser oMe=await moHelper.GetCurrentUser();
      if (string.IsNullOrEmpty(UserID))
        nUserID=(int)oMe.UserID;
      else
        b=Int32.TryParse(UserID,out nUserID);
      if (oMe!=null && (nUserID==oMe.UserID || oMe.UserIsAdmin=="Y")) {
        if (nOffset>=0 && nPageSize>0
        && (sStatus.Length==0 || ",processing,shipped,complete,cancelled,".IndexOf(sStatus)>=0)) {
          Int32 nCount=moMMStoreRepo.GetOrderCount(nUserID,sStatus);
          IEnumerable<Models.MMSOrder> cOrders=null;
          if (nOffset<nCount)
            cOrders=moMMStoreRepo.GetOrders(nUserID,nOffset,nPageSize,sStatus).ToList();
          else
            cOrders=new List<Models.MMSOrder>();
          Models.MMSPagedResponse oResp=new Models.MMSPagedResponse(nOffset, nPageSize, nCount, "&status={sStatus}");
          oResp.Orders=cOrders.ToList();
          r=Ok(oResp);
          }
        else
          r=BadRequest();
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"GET /api/orders",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }



}
}