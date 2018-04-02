using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace E_CommerceApp
{
    public partial class frm_payment : Page
    {

        #region Global Variables
        string user;
        int cartNum;
        private readonly UserCart _cart = UserCart.Instance;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["refNum"] != null)
            {
                cartNum = Convert.ToInt32(Session["refNum"]);
            }
            else
            {
                Response.Redirect("~/Home.aspx");
            }

            if (Session["currUser"] != null)
            {
                user = (string)Session["currUser"];

                FormView1.DataSource = DBOps.BuildCreditCardDetails(user);
                FormView1.DataBind();
                BindControls(1);
                lvw_totals.DataSource = DBOps.BuildUserCartTotals(cartNum);
                lvw_totals.DataBind();
            }
            else
            {
                FormView1.DataSource = DBOps.BuildCreditCardDetails("-");
                FormView1.DataBind();
                BindControls(0);
                lvw_totals.DataSource = DBOps.BuildUserCartTotals(cartNum);
                lvw_totals.DataBind();
            }

            CompareValidator validator = ((CompareValidator)(FormView1.FindControl("CompareEndTodayValidator")));
            validator.ValueToCompare = DateTime.Now.ToShortDateString();

            SiteMaster master = Page.Master as SiteMaster;
            master.UpdateTotalCounters();
        }

        /// <summary>
        /// Sets the correct properties for the FormView's controls in this page
        /// </summary>
        /// <param name="mode">(0) for any user and (1) for a logged in user</param>
        private void BindControls(int mode)
        {
            TextBox Tbx_cardOwner = (TextBox)FormView1.FindControl("Tbx_cardOwner");
            TextBox Tbx_cardNum = (TextBox)FormView1.FindControl("Tbx_cardNum");
            TextBox Tbx_Expiry = (TextBox)FormView1.FindControl("Tbx_Expiry");
            TextBox Tbx_secCode = (TextBox)FormView1.FindControl("Tbx_secCode");
            TextBox Tbx_Addr = (TextBox)FormView1.FindControl("Tbx_Addr");

            switch (mode)
            {
                case 0:
                    {
                        foreach (Control control in FormView1.Controls)
                        {
                            if (control is TextBox)
                            {
                                TextBox tbx = (TextBox)control;
                                tbx.Enabled = true;
                            }
                        }

                        Tbx_Expiry.TextMode = TextBoxMode.Date;

                        break;
                    }

                case 1:
                    {
                        DataTable dt = (DataTable)FormView1.DataSource;


                        Tbx_cardOwner.Text = /*((string)Eval("name"));*/ dt.Rows[0]["name"].ToString();
                        Tbx_cardNum.Text = /*((string)Eval("number"));*/dt.Rows[0]["number"].ToString();
                        Tbx_Expiry.Text = /*((string)Eval("exp"));*/ Convert.ToDateTime(dt.Rows[0]["exp"]).ToString("d");
                        Tbx_secCode.Text = /*((string)Eval("secCode"));*/ dt.Rows[0]["secCode"].ToString();
                        Tbx_Addr.Text = /*((string)Eval("address"));*/ dt.Rows[0]["address"].ToString();

                        foreach (Control control in FormView1.Controls)
                        {
                            if (control is TextBox)
                            {
                                TextBox tbx = (TextBox)control;
                                tbx.Enabled = false;
                            }
                        }

                        break;
                    }
            }
        }

        protected void btn_checkout_Click(object sender, EventArgs e)
        {
            if (Session["currUser"] != null)
            {
                // Make the application give the user a new cart
                DBOps.ReassignUserCart(user, 0);
            }

            if (Request.Cookies["cartID"] != null)
            {
                HttpCookie myCookie = new HttpCookie("cartID");
                myCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Set(myCookie);
            }


            CartDataSource.Update();




        }

        protected void CartDataSource_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            e.Command.Parameters["@Id"].Value = (Convert.ToInt32(Session["prevID"]));
            e.Command.Parameters["@date"].Value = DateTime.Now;
        }

        protected void CartDataSource_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            // reset the session variables
            // Session.Abandon will erase everything. 
            // Not everything has to be erased.
            // Too lazy to do conditionals.
            Session.Remove("prevID");
            Session.Remove("refNum");

            // empty the cart
            UserCart cart = UserCart.Instance;
            cart.Reset();

            Response.Redirect("~/Confirm.aspx");
        }
    }
}