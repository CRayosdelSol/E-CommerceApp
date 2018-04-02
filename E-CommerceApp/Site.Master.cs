using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace E_CommerceApp
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["currUser"] != null)
            {
                Btn_Login.Text = (string)Session["currUser"];
                Btn_Login.PostBackUrl = @"~/TransactionHistory.aspx";

                Btn_SignUp.Text = "Sign Out";
                Btn_SignUp.CssClass = "btn btn-sm btn-outline-danger";
            }
            else
            {
                Btn_Login.PostBackUrl = @"~/Login.aspx";
                Btn_SignUp.PostBackUrl = @"~/Register.aspx";
            }

            UpdateTotalCounters();
        }


        /// <summary>
        /// Updates the values displayed in the my cart link found in the navbar
        /// </summary>
        public void UpdateTotalCounters()
        {
            if (Session["prevID"] != null)
            {
                string[] totals = DBOps.GetUserCartTotals(Convert.ToInt32(Session["prevID"]));
                LBL_Counter.Text = string.Format("Cart ({0} | {1:C})", totals[0], Convert.ToDecimal(totals[1]));
            }
            else
            {
                LBL_Counter.Text = string.Format("Cart ({0} | {1:C})", 0, Convert.ToDecimal(0.ToString()));
            }
        }

        protected void Btn_SignUp_Click(object sender, EventArgs e)
        {
            if (Session["currUser"] != null)
            {
                #region Variable Reset
                Session.Abandon();
                Session.RemoveAll();
                UserCart cart = UserCart.Instance;
                cart.Reset();
                #endregion

                Response.Redirect(@"~/Home.aspx");
            }
        }
    }
}