using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace E_CommerceApp
{
    public partial class TransactionHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["currUser"] == null)
            {
                Response.Redirect("~/Home.aspx");
            }
            else
            {
                Lvw_transactions.DataSource = DBOps.UserTransactionsTable(((string)(Session["currUser"])));
                Lvw_transactions.DataBind();
            }
        }

        protected void Lvw_transactions_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            string RefNum = ((String)e.CommandArgument);
            Session["refNum"] = RefNum;

            Response.Redirect(@"~/RefCart.aspx");

        }

        protected void Lvw_transactions_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            DataPager dp = (DataPager)Lvw_transactions.FindControl("DataPager1");
            dp.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            Lvw_transactions.DataSource = DBOps.UserTransactionsTable(((string)(Session["currUser"])));
            Lvw_transactions.DataBind();
        }
    }
}