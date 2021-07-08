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

  public class CartController : ControllerBase {

    private readonly ILogger<AuthorizeController> moLogger;
    private readonly Repositories.IMMStoreRepository moMMStoreRepo;
    private Support.Helper moHelper;

    /// <summary>
    /// Initialize controller for /categories requests
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="MMStoreRepository">DB Repository</param>
    /// <param name="Helper">Support Helper object</param>
    public CartController(ILogger<AuthorizeController> logger,Repositories.IMMStoreRepository MMStoreRepository,Support.Helper Helper) {
    moLogger = logger;
    moMMStoreRepo=MMStoreRepository;
    moHelper=Helper;
    }

    /// <summary>
    /// Get the cart items for a user
    /// </summary>
    /// <param name="ProductID">Optional product ID</param>
    /// <param name="UserID">Optional User ID (admin only)</param>
    /// <returns>Cart items list</returns>
    [HttpGet][Route("/api/cart")]
    public async Task<IActionResult> GetCart(string ProductID,string UserID) {
    IActionResult r=BadRequest();
    try {
      Models.MMSUser oMe=await moHelper.GetCurrentUser();
      if (oMe!=null && oMe.UserIsActive=="Y") {
        Int32 nUserID=0;
        bool b=Int32.TryParse(UserID,out nUserID);
        if (string.IsNullOrEmpty(UserID) || nUserID==oMe.UserID || oMe.UserIsAdmin=="Y") {
          if (string.IsNullOrEmpty(UserID)) nUserID=(int)oMe.UserID;
          Int32 nProductID=0;
          b=Int32.TryParse(ProductID,out nProductID);
          IEnumerable<Models.MMSCartItemExtended> cItems=moMMStoreRepo.GetCartItemsExtended(nUserID,nProductID);
          r=Ok(new {cart=cItems});
          }
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"GET /api/cart",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Add an entry to the current user's shopping cart
    /// </summary>
    /// <param name="CartItem">CartItem object</param>
    /// <returns>OK</returns>
    [HttpPost][Route("api/cart")]
    public async Task<IActionResult> AddToCart([FromBody]Models.MMSCartItem CartItem) {
    IActionResult r=BadRequest();
    try {
      if (CartItem!=null && CartItem.ProductID!=null && (CartItem.Quantity==null || CartItem.Quantity>=0)) {
        Models.MMSUser oMe=await moHelper.GetCurrentUser();
        if (oMe!=null && oMe.UserIsActive=="Y" && (CartItem.UserID==null || CartItem.UserID==oMe.UserID)) {
          if (CartItem.Quantity==null) CartItem.Quantity=0; // validate quantity
          Models.MMSProduct oProd=await moMMStoreRepo.GetProductAsync((int)CartItem.ProductID);
          if (oProd!=null && oProd.ProductIsActive=="Y") {
            IEnumerable<Models.MMSCartItem> cItems=moMMStoreRepo.GetCartItems((int)oMe.UserID,(int)oProd.ProductID);
            Models.MMSCartItem oItem=(cItems==null || cItems.Count<Models.MMSCartItem>()!=1)?null:cItems.First();
            if (oItem==null && CartItem.Quantity>0) {
              oItem=new Models.MMSCartItem() {ProductID=oProd.ProductID,UserID=oMe.UserID,Quantity=CartItem.Quantity};
              moMMStoreRepo.AddCartItem(oItem);
              }
            else if (oItem!=null && CartItem.Quantity>0 && oItem.Quantity!=CartItem.Quantity) {
              oItem.Quantity=CartItem.Quantity;
              moMMStoreRepo.UpdateCartItem(oItem);
              }
            else if (oItem!=null && CartItem.Quantity==0) {
              moMMStoreRepo.DeleteCartItem(oItem);
              }
            moMMStoreRepo.SaveChanges();
            r=Ok(new { productID=oProd.ProductID, quantity=CartItem.Quantity });
            }
          }
        }
      else
        r=Unauthorized();
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"POST /api/cart",Request.QueryString,(CartItem==null)?"No body":json.Serialize(CartItem));
      r=StatusCode(500);
      }
    return r;
    }

    /// <summary>
    /// Submit the cart as an order
    /// <param name="UserID">Optional User ID (admin only)</param>
    /// </summary>
    /// <returns>Order object</returns>
    [HttpGet][Route("/api/cart/submit")]
    public async Task<IActionResult> SubmitCart(string UserID) {
    IActionResult r=BadRequest();
    try {
      Models.MMSUser oMe=await moHelper.GetCurrentUser();
      if (oMe!=null && oMe.UserIsActive=="Y") {
        Int32 nUserID=0;
        bool b=Int32.TryParse(UserID,out nUserID);
        if (string.IsNullOrEmpty(UserID) || nUserID==oMe.UserID || oMe.UserIsAdmin=="Y") {
          if (string.IsNullOrEmpty(UserID)) nUserID=(int)oMe.UserID;
          Models.MMSUser oUser=null;
          if (nUserID!=oMe.UserID) // verify user
            oUser=await moMMStoreRepo.GetUserAsync(nUserID);
          else
            oUser=oMe;
          if (oUser!=null) {
            List<Models.MMSCartItem> cItems=moMMStoreRepo.GetCartItems(nUserID).ToList();
            if (cItems.Count>0) {
              bool bOK=true; // make sure all products are still available
                Dictionary<int, decimal> cPrices = new Dictionary<int, decimal>();
              foreach (Models.MMSCartItem oItem in cItems) {
                if (bOK) {
                  Models.MMSProduct oProd=await moMMStoreRepo.GetProductAsync((int)oItem.ProductID);
                  if (oProd.ProductIsActive != "Y")
                    bOK = false;
                  else if (!cPrices.ContainsKey((int)oProd.ProductID))
                    cPrices.Add((int)oProd.ProductID, (oProd.ProductPrice==null)?0:(decimal)oProd.ProductPrice);
                  }
                }
              if (bOK) { // we have a good order
                Models.MMSOrder oOrder=new Models.MMSOrder() { OrderDate=DateTime.UtcNow, OrderStatus="Processing", UserID=nUserID};
                moMMStoreRepo.AddOrder(oOrder);
                moMMStoreRepo.SaveChanges();
                foreach (Models.MMSCartItem oItem in cItems) {
                  Models.MMSOrderDetail oLIneItem=new Models.MMSOrderDetail() { OrderID=oOrder.OrderID, ProductID=oItem.ProductID, 
                            OrderQuantity=(int)oItem.Quantity, OrderPrice=cPrices[(int)oItem.ProductID]};
                  moMMStoreRepo.AddOrderItem(oLIneItem);
                  }
                foreach (Models.MMSCartItem oItem in cItems) // flush the cart
                  moMMStoreRepo.DeleteCartItem(oItem);
                moMMStoreRepo.SaveChanges();
                r=Ok(oOrder);
                }
              }
            }
          }
        }
      }
    catch (Exception ex) {
      moLogger.LogError(ex.Message,$"GET /api/cart/submit",Request.QueryString);
      r=StatusCode(500);
      }
    return r;
    }


}
}