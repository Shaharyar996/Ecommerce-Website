using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class order : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["CustomerID"] == null)
        {
            string redirectToURL = "~/order.aspx";
            Response.Redirect("~/login.aspx?redirect=" + redirectToURL);
        }

        if (!IsPostBack)
        {
            //Get logged in id
            int CustID = Convert.ToInt16(Session["CustomerID"]);
            using (ecommerceEntities context = new ecommerceEntities())
            {
                if (Request.QueryString["addToCart"] != null )
                {
                    if(Request.QueryString["addToCart"] != "1")
                    ProductOrderStatus(CustID, 1, 1/*Cart*/, context, "addToCart");

                    bool isCartEmpty = AddProducts(CustID, 1, context);
                    //If cart is empty. No ID label means, no cart item
                    if (isCartEmpty)
                    {
                        Wizard1.Visible = false;
                        lblMessage.Visible = true;
                    }
                }
                else if (Request.QueryString["addToWishlist"] != null )
                {
                    if( Request.QueryString["addToWishlist"] != "1")
                    ProductOrderStatus(CustID, 1, 3, context, "addToWishlist");

                    bool isCartEmpty = AddProducts(CustID, 3/*Wish*/, context);
                    //If cart is empty. No ID label means, no cart item
                    if (isCartEmpty)
                    {
                        Wizard1.Visible = false;
                        lblMessage.Visible = true;
                    }

                }
                
            }
        }
    }

    protected void lnkRemoveItem_Click(object sender, EventArgs e)
    {
        using (ecommerceEntities context = new ecommerceEntities())
        {
            //Get the reference of the clicked button.
            LinkButton button = (sender as LinkButton);
            //Get the Repeater Item reference
            RepeaterItem item = button.NamingContainer as RepeaterItem;
            //Get the repeater item index
            int index = item.ItemIndex;
            string id = ((Label)(Repeater1.Items[index].FindControl("lblHiddenCartID"))).Text;
            int cartid = Convert.ToInt16(id);
            ProductOrderStatu cr = context.ProductOrderStatus.Where(i => i.ProductOrderStatusId == cartid && i.StatusId==1).FirstOrDefault();

            context.ProductOrderStatus.Remove(cr);
            context.SaveChanges();

            string notifyTitle = "One item removed";
            string message = "One item was removed from your cart!";
            string notification = string.Format("?notifyTitle={0}&notificationDescription={1}", notifyTitle, message);
            
            Response.Redirect("~/order.aspx" + notification);
        }
    }
    protected void Wizard1_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        Response.Redirect("~/order.aspx");
    }
    protected void btnPay_Click(object sender, EventArgs e)
    {
        using (ecommerceEntities context = new ecommerceEntities())
        {
            int custID = Convert.ToInt16(Session["CustomerID"]);
            if (string.IsNullOrEmpty(txtAmount.Text))
            {
                lblStatus.Text = "Please select mode of payment Debit/Credit card";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                List<ProductOrderStatu> cart = context.ProductOrderStatus.Where(i => i.CustomerId == custID && i.StatusId==1).ToList();

                foreach (var i in cart)
                {
                    //Fill order table
                    context.Orders.Add(new Order
                    {
                        CustomerID = custID,
                        ProductID = i.ProductId,
                        DateOrdered = DateTime.Now
                    });

                    //Product is bought so, empty the cart.
                    context.ProductOrderStatus.Remove(i);
                }
                context.SaveChanges();

                lblStatus.Text = "Your order has been placed. Happy shopping!";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                string notifyTitle = "Payment Successful!";
                string message = "The order has been placed. You will receive your shipment sonn.";
                string notification = string.Format("?notifyTitle={0}&notificationDescription={1}", notifyTitle, message);

                Response.Redirect("~/order.aspx" + notification);
            }
        }
    }
    protected void rdoDebitCard_CheckedChanged(object sender, EventArgs e)
    {
        addAmount();
        lblStatus.Text = "";
    }

    protected void rdoCredit_CheckedChanged(object sender, EventArgs e)
    {
        addAmount();
        lblStatus.Text = "";
    }
    private void addAmount()
    {
        int CustomerID = Convert.ToInt16(Session["CustomerID"].ToString());
        using (ecommerceEntities context = new ecommerceEntities())
        {
            var cart = (from c in context.ProductOrderStatus
                        join p in context.Products
                            on c.ProductId equals p.ProductID
                            where c.CustomerId == CustomerID
                            && c.StatusId==1
                        select new { p.ProductPrice, c.Quantity });
            decimal? amt = 0;
            foreach (var i in cart)
            {
                amt += (i.ProductPrice * i.Quantity);
            }
            txtAmount.Text = amt.ToString();
        }
    }

    private void ProductOrderStatus(int CustID, int Quantity,int StatusId, ecommerceEntities context,string Querystring)
    {
        int ProdID = Convert.ToInt16(Request.QueryString[Querystring]);

        //Check if product is already in cart
        ProductOrderStatu cr = context.ProductOrderStatus.Where(i => i.ProductId == ProdID && i.CustomerId == CustID && i.StatusId==1/*is already in cart*/).FirstOrDefault();
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

}