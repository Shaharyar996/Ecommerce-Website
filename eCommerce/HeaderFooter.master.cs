using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//

public partial class HeaderFooter : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request.QueryString["notifyTitle"] != null || Request.QueryString["notificationDescription"] != null)
        {
            string title = Request.QueryString["notifyTitle"];
            string message = Request.QueryString["notificationDescription"];
            string jsFunction = string.Format("showNotification('{0}','{1}')",title,message);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "notify", jsFunction, true);
        }
        
        using (ecommerceEntities context = new ecommerceEntities())
        {
            //Check if some user is logged in.
            if (Session["CustomerID"] != null)
            {
                //Getting user info based on session variable
                int id = (int)Session["CustomerID"];
                Customer cust = context.Customers.Where(i => i.CustomerID == id).FirstOrDefault();
                //Change the links
                lnkLogInLogOut.Text = "Logout";
                lnkLogInLogOut.PostBackUrl = "logout.aspx";

                lnkSignUpProfile.Text = "Welcome: " + cust.CustomerName;
                lnkSignUpProfile.PostBackUrl = "~/profile.aspx?CustomerID=" + cust.CustomerID;


                //Update Cart count
                var cart = (from c in context.ProductOrderStatus
                            join p in context.Products
                                on c.ProductId equals p.ProductID
                                where c.CustomerId==id && c.StatusId==1//in cart
                            select new { p.ProductName, p.ProductPrice, p.ProductImageURL });
                lblCartCount.Text = cart.Count().ToString();
                var Wish = (from c in context.ProductOrderStatus
                            join p in context.Products
                                on c.ProductId equals p.ProductID
                            where c.CustomerId == id && c.StatusId == 3//in wish
                            select new { p.ProductName, p.ProductPrice, p.ProductImageURL });
                lblWishCount.Text = Wish.Count().ToString();
                var PlacedOrder = (from c in context.ProductOrderStatus
                            join p in context.Products
                                on c.ProductId equals p.ProductID
                            where c.CustomerId == id && c.StatusId == 2//Placed Order
                            select new { p.ProductName, p.ProductPrice, p.ProductImageURL });
                lblPlaceOrder.Text = PlacedOrder.Count().ToString();
                divUserList.Visible = true;
            }
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/search.aspx?product=" + txtSearch.Text);
    }


}
