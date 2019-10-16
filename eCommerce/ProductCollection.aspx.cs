using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProductCollection : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["CustomerID"] == null)
        {
            string redirectToURL = "~/ProductCollection.aspx";
            Response.Redirect("~/login.aspx?redirect=" + redirectToURL);
        }

        if (!IsPostBack)
        {
            //Get logged in id
            int CustID = Convert.ToInt16(Session["CustomerID"]);
            using (ecommerceEntities context = new ecommerceEntities())
            {
                if (Request.QueryString["addToCart"] != null)
                {
                    Session["PageMode"] = "addToCart";
                    if (Request.QueryString["addToCart"] != "1")
                        ProductOrderStatus(CustID, 1, 1/*Cart*/, context, "addToCart");

                    bool isCartEmpty = AddProducts(CustID, 1, context);
                    //If cart is empty. No ID label means, no cart item
                    if (isCartEmpty)
                    {
                        //Wizard1.Visible = false;
                        lblMessage.Visible = true;
                    }
                }
                else if (Request.QueryString["addToWishlist"] != null)
                {
                    Session["PageMode"] = "addToWishlist";
                    if (Request.QueryString["addToWishlist"] != "1")
                        ProductOrderStatus(CustID, 1, 3, context, "addToWishlist");

                    bool isCartEmpty = AddProducts(CustID, 3/*Wish*/, context);
                    //If cart is empty.No ID label means, no cart item
                    if (isCartEmpty)
                    {
                        // Wizard1.Visible = false;
                        lblMessage.Visible = true;
                    }

                }
                else if (Request.QueryString["PlaceorderList"] != null)
                {
                    Session["PageMode"] = "PlaceorderList";
                    //if (Request.QueryString["PlaceorderList"] != "1")
                    //    ProductOrderStatus(CustID, 1, 3, context, "addToWishlist");

                    bool isCartEmpty = AddProducts(CustID, 2/*Wish*/, context);
                    //If cart is empty.No ID label means, no cart item
                    if (isCartEmpty)
                    {
                        // Wizard1.Visible = false;
                        lblMessage.Visible = true;
                    }
                    Button1.Visible = false;
                    //lnkRemoveItem.Visible = false;
                }

            }
        }

    }

    private bool AddProducts(int CustID, int StatusId, ecommerceEntities context)
    {
        var cart = (from c in context.ProductOrderStatus
                    join p in context.Products
                        on c.ProductId equals p.ProductID
                    where c.CustomerId == CustID && c.StatusId == StatusId//In Cart
                    select new { p.ProductName, p.ProductPrice, p.ProductImageURL, c.ProductOrderStatusId }).ToList();
        Repeater1.DataSource = cart;
        Repeater1.DataBind();

        Boolean isCartEmpty = context.ProductOrderStatus.Where(i => i.CustomerId == CustID && i.StatusId == StatusId).FirstOrDefault() == null;
        return isCartEmpty;

    }
    protected void lnkRemoveItem_Click(object sender, EventArgs e)
    {
        using (ecommerceEntities context = new ecommerceEntities())
        {
            string pagemode = Session["PageMode"].ToString();
            int Status = 1;
            if (pagemode == "addToWishlist")
                Status = 3;
            else
                Status = 1;
            //Get the reference of the clicked button.
            LinkButton button = (sender as LinkButton);
            //Get the Repeater Item reference
            RepeaterItem item = button.NamingContainer as RepeaterItem;
            //Get the repeater item index
            int index = item.ItemIndex;
            string id = ((Label)(Repeater1.Items[index].FindControl("lblHiddenCartID"))).Text;
            int cartid = Convert.ToInt16(id);
            ProductOrderStatu cr = context.ProductOrderStatus.Where(i => i.ProductOrderStatusId == cartid && i.StatusId == Status).FirstOrDefault();

            context.ProductOrderStatus.Remove(cr);
            context.SaveChanges();

            string notifyTitle = "One item removed";
            
            string message = "One item was removed from your ";
            if (Status == 1)
                message = message + "cart!";
            else
                message = message + "wish list!";
            string notification = string.Format("?notifyTitle={0}&notificationDescription={1}", notifyTitle, message);

            Response.Redirect("~/ProductCollection.aspx" + notification);
        }
    }
    private void ProductOrderStatus(int CustID, int Quantity, int StatusId, ecommerceEntities context, string Querystring)
    {
        int ProdID = Convert.ToInt16(Request.QueryString[Querystring]);

        //Check if product is already in cart
        ProductOrderStatu cr = context.ProductOrderStatus.Where(i => i.ProductId == ProdID && i.CustomerId == CustID && i.StatusId==3).FirstOrDefault();
        //If not in the DB add it.
        if (cr == null)
        {
            context.ProductOrderStatus.Add(new ProductOrderStatu
            {
                CustomerId = CustID,
                ProductId = ProdID,
                StatusId = StatusId,//Add to cart
                Quantity = Quantity,
                CreatedDate = DateTime.Now
            });
            context.SaveChanges();
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        int CustID = Convert.ToInt16(Session["CustomerID"]);
        using (ecommerceEntities context = new ecommerceEntities())
        {
            List<ProductOrderStatu> cr = context.ProductOrderStatus.Where(i =>  i.CustomerId == CustID && i.StatusId == 1).ToList();

            foreach (ProductOrderStatu order in cr)
            {

                order.StatusId = 2;
               


            }
            context.SaveChanges();

            Response.Redirect("~/ProductCollection.aspx?PlaceorderList=1");
        }
    }
}