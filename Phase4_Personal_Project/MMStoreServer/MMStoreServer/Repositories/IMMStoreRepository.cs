using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMStoreServer.Models;

namespace MMStoreServer.Repositories {
  public interface IMMStoreRepository {

    /// <summary>
    /// Update the database
    /// </summary>
    /// <returns>Nothing</returns>
    public void SaveChanges();

    //===============================================================================================  Categories

    /// <summary>
    /// Get a count of all categories
    /// </summary>
    /// <returns>Number of categories</returns>
    public int GetCategoryCount(bool IncludeInactive=false);

    /// <summary>
    /// Get a sorted listed of categories
    /// <param name="Offset">Number of records to skip</param>
    /// <param name="PageSize">Number of records to return</param>
    /// <param name="SortField">Field to sort on</param>
    /// </summary>
    /// <returns>MMSCategory objects</returns>
    public Task<IEnumerable<MMSCategory>> GetCategoriesAsync(Int32 Offset=0,Int32 PageSize=25,
                string SortField="Name", bool IncludeInactive = false);

    /// <summary>
    /// Get a specific Category by ID
    /// </summary>
    /// <param name="CategoryID">Category ID</param>
    /// <returns>Category object or null</returns>
    public Task<MMSCategory> GetCategoryAsync(int CategoryID);

    /// <summary>
    /// Get a specific Category by name
    /// </summary>
    /// <param name="CategoryName">Category name</param>
    /// <returns>Category object or null</returns>
    public MMSCategory GetCategory(string CategoryName);

    /// <summary>
    /// Add a new category to the database
    /// </summary>
    /// <param name="NewItem">Category object</param>
    public void AddCategory(MMSCategory NewItem);

    /// <summary>
    /// Replace a category in the database
    /// </summary>
    /// <param name="MMSCategory">Updated Category</param>
    public void UpdateCategory(MMSCategory Category);

    //===============================================================================================  Products

    /// <summary>
    /// Get a count of all products
    /// </summary>
    /// <returns>Number of products</returns>
    public int GetProductCount(Int32 CategoryID=0,string Search="", bool IncludeInactive = false);

    /// <summary>
    /// Get a sorted listed of products
    /// <param name="Offset">Number of records to skip</param>
    /// <param name="PageSize">Number of records to return</param>
    /// <param name="SortField">Field to sort on</param>
    /// <param name="CategoryID">Category to include</param>
    /// <param name="Search">Text to search for in name and/or code</param>
    /// </summary>
    /// <returns>MMSProduct objects</returns>
    public Task<IEnumerable<MMSProductExtended>> GetProductsAsync(Int32 Offset=0,Int32 PageSize=25,
                  string SortField="Name",Int32 CategoryID=0,string Search="", bool IncludeInactive = false);

    /// <summary>
    /// Get a specific Product by ID
    /// </summary>
    /// <param name="ProductID">Product ID</param>
    /// <returns>Product object or null</returns>
    public Task<MMSProduct> GetProductAsync(int ProductID);

    /// <summary>
    /// Get a specific Product by name or code
    /// </summary>
    /// <param name="Product">Product ID, Name, or Code</param>
    /// <returns>Product object or null</returns>
    public MMSProduct GetProduct(string ProductTarget);

    /// <summary>
    /// Add a new product to the database
    /// </summary>
    /// <param name="NewItem">Product object</param>
    public void AddProduct(MMSProduct NewItem);

    /// <summary>
    /// Updates a product in the database
    /// </summary>
    /// <param name="UpdatedItem">Product object</param>
    public void UpdateProduct(MMSProduct UpdatedItem);

    /// <summary>
    /// Update active status for all products in a category
    /// </summary>
    /// <param name="CategoryID">Category to update</param>
    /// <param name="CategoryIsActive">true to enable, false to disable</param>
    public void UpdateProductCategoryState(int CategoryID, bool CategoryIsActive);

    //===============================================================================================  Cart Items

    /// <summary>
    /// Get items from the cart
    /// </summary>
    /// <param name="UserID">User ID for cart</param>
    /// <param name="ProductID">Product ID for item (0 for all)</param>
    /// <returns>Cart item list</returns>
    public IEnumerable<Models.MMSCartItem> GetCartItems(int UserID, int ProductID = 0);

    /// <summary>
    /// Get items from the cart including product/category info
    /// </summary>
    /// <param name="UserID">User ID for cart</param>
    /// <param name="ProductID">Product ID for item (0 for all)</param>
    /// <returns>Cart item list</returns>
    public IEnumerable<Models.MMSCartItemExtended> GetCartItemsExtended(int UserID, int ProductID = 0);

    /// <summary>
    /// Add an item to the cart
    /// </summary>
    /// <param name="CartItem">Item to add</param>
    /// <returns>Cart item</returns>
    public void AddCartItem(Models.MMSCartItem CartItem);

    /// <summary>
    /// Update an item in the cart
    /// </summary>
    /// <param name="CartItem">Item to update</param>
    /// <returns>Cart item</returns>
    public void UpdateCartItem(Models.MMSCartItem CartItem);

    /// <summary>
    /// Delete an item from the cart
    /// </summary>
    /// <param name="CartItem">Item to delete</param>
    /// <returns>Cart item</returns>
    public void DeleteCartItem(Models.MMSCartItem CartItem);


    //===============================================================================================  Orders

    /// <summary>
    /// Add an order to the database
    /// </summary>
    /// <param name="Order">Order Header item</param>
    public void AddOrder(Models.MMSOrder Order);

    /// <summary>
    /// Add an order item to the database
    /// </summary>
    /// <param name="OrderItem">Order Detail item</param>
    public void AddOrderItem(Models.MMSOrderDetail OrderItem);

    /// <summary>
    /// Get order header by ID
    /// </summary>
    /// <param name="OrderID">Order to retrieve</param>
    /// <returns>Order header</returns>
    public Models.MMSOrder GetOrder(int OrderID);

    /// <summary>
    /// Get order detail line
    /// </summary>
    /// <param name="OrderID">Order to retrieve</param>
    /// <param name="ProductID">Product ID to retrieve</param>
    /// <returns>Order header</returns>
    public Models.MMSOrderDetail GetOrderDetail(int OrderID,int ProductID);

    /// <summary>
    /// Get order details
    /// </summary>
    /// <param name="OrderID">Order ID number</param>
    /// <returns>List of order items</returns>
    public IEnumerable<Models.MMSOrderDetailExtended> GetOrderDetails(int OrderID);

    /// <summary>
    /// Get count of orders for user
    /// </summary>
    /// <param name="UserID">User ID</param>
    /// <param name="Status">Order status filter</param>
    /// <returns>Count of orders</returns>
    public int GetOrderCount(Int32 UserID,string Status="");

    /// <summary>
    /// Get list of orders for user
    /// </summary>
    /// <param name="UserID">User ID</param>
    /// <param name="Offset">Records to skip</param>
    /// <param name="PageSize">Records to return</param>
    /// <param name="Status">Status filter</param>
    /// <returns>List of orders</returns>
    public IEnumerable<MMSOrder> GetOrders(Int32 UserID,Int32 Offset=0,Int32 PageSize=25,string Status="");


    //===============================================================================================  Users

    /// <summary>
    /// Get a count of all users
    /// </summary>
    /// <param name="Search">Search target</param>
    /// <returns>Number of users</returns>
    public Int32 GetUserCount(string Search="");

    /// <summary>
    /// Get a partial list of users, sorted
    /// </summary>
    /// <param name="Offset">Starting offset into list</param>
    /// <param name="PageSize">Number of records to return</param>
    /// <param name="SortField">Field to sort on (ID|Name)</param>
    /// <param name="Search">Search target</param>
    /// <returns>MMSUser objects</returns>
    public Task<IEnumerable<MMSUser>> GetUsersAsync(Int32 Offset=0,Int32 PageSize=25,string SortField="Name",string Search="");

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    /// <param name="UserID">User ID</param>
    /// <returns>User object or null</returns>
    public Task<MMSUser> GetUserAsync(int UserID);

    /// <summary>
    /// Get a specific user by email and optional password
    /// </summary>
    /// <param name="UserEMail">EMail address</param>
    /// <param name="UserPassword">Password</param>
    /// <returns>User object or null</returns>
    public MMSUser GetUser(string UserEMail,string UserPassword="");

    /// <summary>
    /// Replace a user in the database
    /// </summary>
    /// <param name="MMSUser">Updated user</param>
    public void UpdateUser(MMSUser User);

    /// <summary>
    /// Convert password to hashed value
    /// </summary>
    /// <param name="Password">Password text</param>
    /// <returns>Password hash</returns>
    public string PasswordHash(string Password);

    /// <summary>
    /// Ad a new user to the database
    /// </summary>
    /// <param name="MMSUser">New user</param>
    public void AddUser(MMSUser User);


  }
}
