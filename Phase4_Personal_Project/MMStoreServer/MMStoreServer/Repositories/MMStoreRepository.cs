using MMStoreServer.Repositories;
using MMStoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMStoreServer.Repositories {

  public class MMStoreRepository : IMMStoreRepository {

    /// <summary>
    /// Connection to EntityFramework
    /// </summary>
    private MMStoreDBContext moDBContext;

    /// <summary>
    /// Initialize repository
    /// </summary>
    /// <param name="DBContext">DB context specified in startup</param>
    public MMStoreRepository(MMStoreDBContext DBContext) {
    moDBContext = DBContext ?? throw new ArgumentNullException(nameof(DBContext));
    }

    /// <inheritdoc>
    public void SaveChanges() {
    moDBContext.SaveChanges();
    }



    //===============================================================================================  Categories

    /// <inheritdoc>
    public int GetCategoryCount(bool IncludeInactive = false) {
    IQueryable<Models.MMSCategory> q = moDBContext.MMSCategories;
    if (!IncludeInactive)
      q = q.Where(c => c.CategoryIsActive == "Y");
    return q.Count();
    }

    /// <inheritdoc>
    public async Task<IEnumerable<MMSCategory>> GetCategoriesAsync(Int32 Offset=0,Int32 PageSize=25,
              string SortField="Name", bool IncludeInactive = false) {
    IQueryable<Models.MMSCategory> q;
    IOrderedQueryable<Models.MMSCategory> q1;
    q = moDBContext.MMSCategories;
    if (!IncludeInactive)
      q = q.Where(c => c.CategoryIsActive == "Y");
    if (SortField.Equals("ID",StringComparison.InvariantCultureIgnoreCase))
      q1=q.OrderBy(c => c.CategoryID);
    else
      q1=q.OrderBy(c => c.CategoryName);
    IEnumerable<MMSCategory> cCats= await Task.FromResult<IEnumerable<MMSCategory>>(q.Skip(Offset).Take(PageSize).ToList());
    return cCats;
    }

    /// <inheritdoc>
    public async Task<MMSCategory> GetCategoryAsync(int CategoryID) {
    MMSCategory oCategory= await moDBContext.MMSCategories.FindAsync(CategoryID);
    return oCategory;
    }

    /// <inheritdoc>
    public MMSCategory GetCategory(string CategoryName) {
    MMSCategory oCategory= moDBContext.MMSCategories.Where(c => c.CategoryName==CategoryName).FirstOrDefault();
    return oCategory;
    }

    /// <inheritdoc>
    public void AddCategory(MMSCategory NewItem) {
    moDBContext.Add(NewItem);
    }

    /// <inheritdoc>
    public void UpdateCategory(MMSCategory Category) {
    moDBContext.Update(Category);
    }


    //===============================================================================================  Products

    /// <inheritdoc>
    public int GetProductCount(Int32 CategoryID=0,string Search="", bool IncludeInactive = false) {
    IQueryable<Models.MMSProduct> q=moDBContext.MMSProducts;
    if (!IncludeInactive)
      q=q.Where(p => p.ProductIsActive=="Y");
    if (CategoryID>0)
      q=q.Where(p => p.CategoryID==CategoryID);
    if (Search.Length>0)
      q=q.Where(p => p.ProductCode.IndexOf(Search)>=0 || p.ProductName.IndexOf(Search)>=0);
    return q.Count();
    }

    /// <inheritdoc>
    public async Task<IEnumerable<MMSProductExtended>> GetProductsAsync(Int32 Offset=0,Int32 PageSize=25,
        string SortField="Name",Int32 CategoryID=0,string Search="",bool IncludeInactive=false) {
    //IQueryable<Models.MMSProduct> q=moDBContext.MMSProducts;
    IQueryable<MMSProductExtended> q = from p in moDBContext.MMSProducts
            join c in moDBContext.MMSCategories on p.CategoryID equals c.CategoryID into x 
            from pc in x.DefaultIfEmpty()
            select new MMSProductExtended() {
                ProductID = p.ProductID,
                CategoryID = p.CategoryID,
                ProductName=p.ProductName,
                ProductCode=p.ProductCode,
                ProductInfo=p.ProductInfo,
                CategoryName=pc.CategoryName,
                ProductPrice=p.ProductPrice,
                ProductIsActive=p.ProductIsActive};
    if (CategoryID>0)
      q=q.Where(p => p.CategoryID==CategoryID);
    if (!IncludeInactive)
      q=q.Where(p => p.ProductIsActive=="Y");
    if (Search.Length>0)
      q=q.Where(p => p.ProductCode.IndexOf(Search)>=0 || p.ProductName.IndexOf(Search)>=0);
    switch (SortField.ToLower()) {
      case "id":
        q=q.OrderBy(p => p.ProductID);
        break;
      case "code":
        q=q.OrderBy(p => p.ProductCode);
        break;
      default:
        q=q.OrderBy(p => p.ProductName);
        break;
        }
    IEnumerable<MMSProductExtended> cProducts= await Task.FromResult<IEnumerable<MMSProductExtended>>(q.Skip(Offset).Take(PageSize).ToList());
    return cProducts;
    }

    /// <inheritdoc>
    public async Task<MMSProduct> GetProductAsync(int ProductID) {
    MMSProduct oProduct= await moDBContext.MMSProducts.FindAsync(ProductID);
    return oProduct;
    }

    /// <inheritdoc>
    public MMSProduct GetProduct(string ProductTarget) {
    MMSProduct oProduct= moDBContext.MMSProducts.Where(p => p.ProductName==ProductTarget || p.ProductCode==ProductTarget).FirstOrDefault();
    return oProduct;
    }

    /// <inheritdoc>
    public void AddProduct(MMSProduct NewItem) {
    moDBContext.Add(NewItem);
    }

    /// <inheritdoc>
    public void UpdateProduct(MMSProduct UpdatedItem) {
    moDBContext.Update(UpdatedItem);
    }

    /// <inheritdoc>
    public void UpdateProductCategoryState(int CategoryID, bool CategoryIsActive) {
    string sNewState = CategoryIsActive ? "Y" : "C"; // if category is inactive we set all active products to C
    string sOldState = CategoryIsActive ? "C" : "Y"; // so we can undo the change if categoyr is made active again
    // this will be slow for categories with many items
    List<MMSProduct> cProducts= moDBContext.MMSProducts.Where(p => p.CategoryID == CategoryID && p.ProductIsActive == sOldState).ToList();
    foreach (MMSProduct p in cProducts) {
      p.ProductIsActive = sNewState;
      moDBContext.Update(p);
      }
    moDBContext.SaveChanges();
    }

    //===============================================================================================  Cart Items

    /// <inheritdoc>
    public IEnumerable<Models.MMSCartItem> GetCartItems(int UserID, int ProductID = 0) {
      IEnumerable<Models.MMSCartItem> cItems = null;
      if (ProductID == 0)
        cItems = moDBContext.MMSCarts.Where(i => i.UserID == UserID).ToList();
      else
        cItems = moDBContext.MMSCarts.Where(i => i.UserID == UserID && i.ProductID == ProductID).ToList();
      return cItems;
    }

    /// <inheritdoc>
    public IEnumerable<Models.MMSCartItemExtended> GetCartItemsExtended(int UserID, int ProductID = 0) {
      IEnumerable<Models.MMSCartItemExtended> cItems = null;
      IQueryable<MMSCartItemExtended> q = from c in moDBContext.MMSCarts
                                             join p in moDBContext.MMSProducts on c.ProductID equals p.ProductID 
                                             join pc in moDBContext.MMSCategories on p.CategoryID equals pc.CategoryID
                                             join u in moDBContext.MMSUsers on c.UserID equals u.UserID 
                                             select new MMSCartItemExtended() {
                                               UserID = c.UserID,
                                               ProductID = c.ProductID,
                                               ProductName = p.ProductName,
                                               ProductCode = p.ProductCode,
                                               ProductPrice = p.ProductPrice,
                                               CategoryName = pc.CategoryName,
                                               Quantity = c.Quantity
                                             };
      if (ProductID == 0)
        cItems =q.Where(i => i.UserID == UserID).ToList();
      else
        cItems = q.Where(i => i.UserID == UserID && i.ProductID == ProductID).ToList();
      return cItems;
    }

    /// <inheritdoc>
    public void AddCartItem(Models.MMSCartItem CartItem) {
    moDBContext.Add(CartItem);
    }

    /// <inheritdoc>
    public void UpdateCartItem(Models.MMSCartItem CartItem) {
    moDBContext.Update(CartItem);
    }

    /// <inheritdoc>
    public void DeleteCartItem(Models.MMSCartItem CartItem) {
    moDBContext.Remove(CartItem);
    }


    //===============================================================================================  Orders

    /// <inheritdoc>
    public void AddOrder(Models.MMSOrder Order) {
    moDBContext.Add(Order);
    }

    /// <inheritdoc>
    public void AddOrderItem(Models.MMSOrderDetail OrderItem) {
    moDBContext.Add(OrderItem);
    }

    /// <inheritdoc>
    public Models.MMSOrder GetOrder(int OrderID) {
    MMSOrder oOrder=moDBContext.MMSOrders.Find(OrderID);
    return oOrder;
    }

    /// <inheritdoc>
    public Models.MMSOrderDetail GetOrderDetail(int OrderID,int ProductID) {
    MMSOrderDetail oOrderItem=moDBContext.MMSOrderDetails.Find(OrderID,ProductID);
    return oOrderItem;
    }

    /// <inheritdoc>
    public IEnumerable<Models.MMSOrderDetailExtended> GetOrderDetails(int OrderID) {
    IQueryable<MMSOrderDetailExtended> q = from d in moDBContext.MMSOrderDetails
            join p in moDBContext.MMSProducts on d.ProductID equals p.ProductID into dp
            from x in dp.DefaultIfEmpty()
            select new MMSOrderDetailExtended() {
                OrderID = d.OrderID,
                ProductID = d.ProductID,
                ProductName=x.ProductName,
                ProductCode=x.ProductCode,
                OrderPrice=d.OrderPrice,
                OrderQuantity=d.OrderQuantity};
    return q.ToList();
    }

    /// <inheritdoc>
    public int GetOrderCount(Int32 UserID,string Status="") {
    IQueryable<Models.MMSOrder> q=moDBContext.MMSOrders;
    Status=(Status??"").Trim().ToLower();
    if (Status.Length>0)
      q=q.Where(o => o.UserID==UserID && o.OrderStatus==Status);
    else
      q=q.Where(o => o.UserID==UserID);
    return q.Count();
    }

    /// <inheritdoc>
    public IEnumerable<MMSOrder> GetOrders(Int32 UserID,Int32 Offset=0,Int32 PageSize=25,string Status="") {
    IQueryable<Models.MMSOrder> q=moDBContext.MMSOrders;
    if (Status.Length>0)
      q=q.Where(o => o.UserID==UserID && o.OrderStatus==Status);
    else
      q=q.Where(o => o.UserID==UserID);
    IEnumerable<MMSOrder> cOrders=q.OrderBy(o => o.OrderDate).Skip(Offset).Take(PageSize);
    return cOrders;
    }


    //===============================================================================================  Users

    /// <inheritdoc>
    public int GetUserCount(string Search="") {
    IQueryable<MMSUser> q=moDBContext.MMSUsers;
    if (Search.Length > 0)
      q = q.Where(u => u.UserEMail.IndexOf(Search) >= 0 || u.UserFirstName.IndexOf(Search) >= 0 || u.UserLastName.IndexOf(Search) >= 0);
    return q.Count();
    }

    /// <inheritdoc>
    public async Task<IEnumerable<MMSUser>> GetUsersAsync(Int32 Offset=0,Int32 PageSize=25,string SortField="Name", string Search="") {
    IQueryable<Models.MMSUser> q= moDBContext.MMSUsers;
    if (Search.Length>0)
      q = q.Where(u => u.UserEMail.IndexOf(Search) >= 0 || u.UserFirstName.IndexOf(Search) >= 0 || u.UserLastName.IndexOf(Search) >= 0);
    if (SortField.Equals("ID",StringComparison.InvariantCultureIgnoreCase))
      q=q.OrderBy(u => u.UserID);
    else
      q=q.OrderBy(u => u.UserLastName+','+u.UserFirstName);
    IEnumerable<MMSUser> cUsers= await Task.FromResult<IEnumerable<MMSUser>>(q.Skip(Offset).Take(PageSize).ToList());
    return cUsers;
    }

    /// <inheritdoc>
    public async Task<MMSUser> GetUserAsync(int UserID) {
    MMSUser oUser= await moDBContext.MMSUsers.FindAsync(UserID);
    return oUser;
    }

    /// <inheritdoc>
    public MMSUser GetUser(string UserEMail,string UserPassword="") {
    MMSUser oUser=null;
    if (UserPassword.Length==0)
      oUser=moDBContext.MMSUsers.Where<MMSUser>(u => u.UserEMail==UserEMail).SingleOrDefault<MMSUser>();
    else {
      string sHash=PasswordHash(UserPassword);
      oUser=moDBContext.MMSUsers.Where<MMSUser>(u => u.UserEMail==UserEMail && u.UserPassword==sHash).SingleOrDefault<MMSUser>();
      }
    return oUser;
    }

    /// <inheritdoc>
    public void UpdateUser(MMSUser User) {
    moDBContext.Update(User);
    }

    /// <inheritdoc>
    public string PasswordHash(string Password) {
    string sOut=string.IsNullOrEmpty(Password)?"":Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Password)).Replace("=","");
    return sOut;
    }

    /// <inheritdoc>
    public void AddUser(MMSUser User) {
    moDBContext.Add(User);
    }

  }
}
