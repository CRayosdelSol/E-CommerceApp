using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace E_CommerceApp
{
    public partial class FrmCheckout1 : Page
    {
        #region Global Variables
        readonly UserCart _cart = UserCart.Instance;
        string _user = "-";
        int _userCartId = -1;
        int _itemQuant = 0;
        string _itemSKU = string.Empty;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            // if the user is logged in, obtain his current cart if it's not zero.
            // otherwise, use the ID stored in the session variable.
            if (Session["currUser"] != null)
            {
                _user = (string)Session["currUser"];

                _userCartId = DBOps.GetLatestEntry(DBOps.GetUserID(_user)) != 0 ? DBOps.GetLatestEntry(DBOps.GetUserID(_user)) :
                    Convert.ToInt32(Session["prevID"]);
                _cart.cartID = _userCartId;

                if (Session["sync"] != null)
                {
                    _cart.SyncCart(Convert.ToInt32(Session["prevID"]), _user);
                    cartDatasource.Update();
                    Session["prevID"] = _userCartId;
                    Session.Remove("sync");
                }
            }
            else
            {
                _user = "-";
                _userCartId = Convert.ToInt32(Session["prevID"]);
            }

            if (Session["prevID"] == null)
            {
                _userCartId = Convert.ToInt32(Session["prevID"]);
            }
            else
            {
                Session["prevID"] = _userCartId;
            }

            _cart.cartID = _userCartId;
            //Session["prevID"] = _userCartId;

            lvw_items.DataSource = DBOps.BuildUserCart(_userCartId);
            lvw_items.DataBind();
            lvw_totals.DataSource = DBOps.BuildUserCartTotals(_userCartId);
            lvw_totals.DataBind();

            Button button = (Button)UpdatePanel1.FindControl("btn_checkout");

            if (DBOps.BuildUserCart(_userCartId).Rows.Count == 0)
            {
                button.Enabled = false;
                button.CssClass = "btn btn-outline-secondary btn-block";
            }
            else
            {
                button.Enabled = true;
            }

            SiteMaster master = Page.Master as SiteMaster;
            master.UpdateTotalCounters();
        }

        protected void lvw_items_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            string[] productDetails = ((String)e.CommandArgument).Split(',');

            try
            {
                _cart.RemoveItem(productDetails[0].Trim(), Convert.ToDecimal(productDetails[1].Trim()), Convert.ToInt32(productDetails[2].Trim()));
                cartDatasource.Update();
                _itemSKU = productDetails[0].Trim();
                _itemQuant = DBOps.GetProductQuantity(_itemSKU) + Convert.ToInt32(productDetails[2].Trim());
                ProductsDataSource.Update();
            }
            catch (Exception)
            {
                // ignored
            }

            Button button = (Button)UpdatePanel1.FindControl("btn_checkout");
            if (DBOps.BuildUserCart(_userCartId).Rows.Count < 1)
            {
                if (_user != "-")
                {
                    //cartDatasource.Delete();
                    //DataOps.reassignUserCart(user);
                }
                button.Enabled = false;
                button.CssClass = "btn btn-outline-secondary btn-block";
            }

            DataPager dp = (DataPager)lvw_items.FindControl("DataPager1");
            if (lvw_items.Items.Count <= 1)
            {
                dp.SetPageProperties(0, dp.MaximumRows, false);
            }

            lvw_items.DataSource = DBOps.BuildUserCart(_userCartId);
            lvw_totals.DataSource = DBOps.BuildUserCartTotals(_userCartId);
            lvw_items.DataBind();
            lvw_totals.DataBind();

            SiteMaster master = Page.Master as SiteMaster;
            master.UpdateTotalCounters();
        }

        protected void cartDatasource_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            e.Command.Parameters["@Id"].Value = _userCartId;
            e.Command.Parameters["@customer"].Value = _user;
            e.Command.Parameters["@items"].Value = _cart.lastInsertedItem;
            e.Command.Parameters["@prices"].Value = _cart.lastInsertedPrice;
            e.Command.Parameters["@quants"].Value = _cart.lastInsertedQuant;
            e.Command.Parameters["@totalCount"].Value = _cart.totalItemQuantity;
            e.Command.Parameters["@totalPrice"].Value = _cart.totalCartPrice;
        }

        protected void Btn_add_Click(object sender, EventArgs e)
        {
            Button Btn_add = (Button)sender;
            ListViewDataItem item = (ListViewDataItem)Btn_add.NamingContainer;
            Button a_Btn_add = (Button)item.FindControl("Btn_add");
            TextBox tb = (TextBox)item.FindControl("tbx_qty"); //get the textbox in the proper listview item

            int tb_content = Convert.ToInt32(tb.Text);
            tb.Text = (tb_content + 1).ToString();

            if (Convert.ToInt32(tb.Text) <= 0 || tb.Text == string.Empty || string.IsNullOrEmpty(tb.Text) || string.IsNullOrWhiteSpace(tb.Text) || (tb.Text == DBNull.Value.ToString(CultureInfo.InvariantCulture)))
            {
                tb.Text = "1";
            }
            else if (Convert.ToInt32(tb.Text) > 99)
            {
                tb.Text = "99";
            }

            Label lblSku = (Label)item.FindControl("lbl_sku");
            Label lblPrice = (Label)item.FindControl("lbl_price");

            //int t_itemStock = DBOps.GetProductQuantity(lblSku.Text);
            int t_itemStock = Convert.ToInt32(Session[lblSku.Text]);
            int t_cartQuantity = Convert.ToInt32(tb.Text);
            int t_currCartQuantity = DBOps.GetItemQuantity(_userCartId, lblSku.Text);

            try
            {
                _itemSKU = lblSku.Text;

                /// user adds a specified amount of the item to the cart
                if (t_currCartQuantity - t_cartQuantity < 0)
                {
                    if (t_itemStock >= t_cartQuantity /* || t_sessionQuant >= t_cartQuantity*/)
                    {
                        _cart.UpdateItem(lblSku.Text, Decimal.Parse(lblPrice.Text, NumberStyles.Currency), Convert.ToInt32(tb.Text));
                        cartDatasource.Update();

                        _itemQuant = DBOps.GetProductQuantity(_itemSKU) - Math.Abs(t_currCartQuantity - t_cartQuantity);
                        ProductsDataSource.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "notif",
                            string.Format("alert('ITEM NOT ADDED. The specified quantity of {0} is more than the available stock of the item.')",
                            t_cartQuantity), true);
                    }
                }
                else
                {
                    _cart.UpdateItem(lblSku.Text, Decimal.Parse(lblPrice.Text, NumberStyles.Currency), Convert.ToInt32(tb.Text));
                    cartDatasource.Update();
                    _itemQuant = DBOps.GetProductQuantity(_itemSKU) + Math.Abs(t_currCartQuantity - t_cartQuantity);
                    ProductsDataSource.Update();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            lvw_items.DataSource = DBOps.BuildUserCart(_userCartId);
            lvw_totals.DataSource = DBOps.BuildUserCartTotals(_userCartId);
            lvw_items.DataBind();
            lvw_totals.DataBind();

            SiteMaster master = Page.Master as SiteMaster;
            master.UpdateTotalCounters();

        }

        protected void Btn_sub_Click(object sender, EventArgs e)
        {
            Button Btn_sub = (Button)sender;
            ListViewDataItem item = (ListViewDataItem)Btn_sub.NamingContainer;
            Button a_Btn_sub = (Button)item.FindControl("Btn_sub");
            TextBox tb = (TextBox)item.FindControl("tbx_qty"); //get the textbox in the proper listview item

            int tb_content = Convert.ToInt32(tb.Text);
            tb.Text = (tb_content - 1).ToString();

            if (Convert.ToInt32(tb.Text) <= 0 || tb.Text == string.Empty || string.IsNullOrEmpty(tb.Text) || string.IsNullOrWhiteSpace(tb.Text) || (tb.Text == DBNull.Value.ToString(CultureInfo.InvariantCulture)))
            {
                tb.Text = "1";
            }
            else if (Convert.ToInt32(tb.Text) > 99)
            {
                tb.Text = "99";
            }

            Label lblSku = (Label)item.FindControl("lbl_sku");
            Label lblPrice = (Label)item.FindControl("lbl_price");

            int t_itemStock = Convert.ToInt32(Session[lblSku.Text]);
            int t_cartQuantity = Convert.ToInt32(tb.Text);
            int t_currCartQuantity = DBOps.GetItemQuantity(_userCartId, lblSku.Text);

            try
            {
                _itemSKU = lblSku.Text;

                /// user adds a specified amount of the item to the cart
                if (t_currCartQuantity - t_cartQuantity < 0)
                {
                    if (t_itemStock >= t_cartQuantity /* || t_sessionQuant >= t_cartQuantity*/)
                    {
                        _cart.UpdateItem(lblSku.Text, Decimal.Parse(lblPrice.Text, NumberStyles.Currency), Convert.ToInt32(tb.Text));
                        cartDatasource.Update();

                        _itemQuant = DBOps.GetProductQuantity(_itemSKU) - Math.Abs(t_currCartQuantity - t_cartQuantity);
                        ProductsDataSource.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "notif",
                            string.Format("alert('ITEM NOT ADDED. The specified quantity of {0} is more than the available stock of the item.')",
                            t_cartQuantity), true);
                    }
                }
                else
                {
                    _cart.UpdateItem(lblSku.Text, Decimal.Parse(lblPrice.Text, NumberStyles.Currency), Convert.ToInt32(tb.Text));
                    cartDatasource.Update();
                    _itemQuant = DBOps.GetProductQuantity(_itemSKU) + Math.Abs(t_currCartQuantity - t_cartQuantity);
                    ProductsDataSource.Update();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            lvw_items.DataSource = DBOps.BuildUserCart(_userCartId);
            lvw_totals.DataSource = DBOps.BuildUserCartTotals(_userCartId);
            lvw_items.DataBind();
            lvw_totals.DataBind();

            SiteMaster master = Page.Master as SiteMaster;
            master.UpdateTotalCounters();
        }

        protected void tbx_qty_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox1 = (TextBox)sender;
            ListViewDataItem item = (ListViewDataItem)textBox1.NamingContainer;
            TextBox tb = (TextBox)item.FindControl("tbx_qty"); //get the textbox in the proper listview item
            if (Convert.ToInt32(tb.Text) <= 0 || tb.Text == string.Empty || string.IsNullOrEmpty(tb.Text) || string.IsNullOrWhiteSpace(tb.Text) || (tb.Text == DBNull.Value.ToString(CultureInfo.InvariantCulture)))
            {
                tb.Text = "1";
            }
            else if (Convert.ToInt32(tb.Text) > 99)
            {
                tb.Text = "99";
            }

            Label lblSku = (Label)item.FindControl("lbl_sku");
            Label lblPrice = (Label)item.FindControl("lbl_price");

            //int t_itemStock = DBOps.GetProductQuantity(lblSku.Text);
            int t_itemStock = Convert.ToInt32(Session[lblSku.Text]);
            int t_cartQuantity = Convert.ToInt32(tb.Text);
            int t_currCartQuantity = DBOps.GetItemQuantity(_userCartId, lblSku.Text);

            #region Old validation code
            //int t_sessionQuant = 0;

            //if (Session[lblSku.Text] != null)
            //{
            //    t_sessionQuant = Convert.ToInt32(Session[lblSku.Text]);
            //}
            ////else
            ////{
            ////    Session[lblSku.Text] = t_currCartQuantity;
            ////} 
            #endregion


            try
            {
                _itemSKU = lblSku.Text;

                /// user adds a specified amount of the item to the cart
                if (t_currCartQuantity - t_cartQuantity < 0)
                {
                    if (t_itemStock >= t_cartQuantity /* || t_sessionQuant >= t_cartQuantity*/)
                    {
                        _cart.UpdateItem(lblSku.Text, Decimal.Parse(lblPrice.Text, NumberStyles.Currency), Convert.ToInt32(tb.Text));
                        cartDatasource.Update();

                        _itemQuant = DBOps.GetProductQuantity(_itemSKU) - Math.Abs(t_currCartQuantity - t_cartQuantity);
                        ProductsDataSource.Update();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "notif",
                            string.Format("alert('ITEM NOT ADDED. The specified quantity of {0} is more than the available stock of the item.')",
                            t_cartQuantity), true);
                    }
                }
                else
                {
                    _cart.UpdateItem(lblSku.Text, Decimal.Parse(lblPrice.Text, NumberStyles.Currency), Convert.ToInt32(tb.Text));
                    cartDatasource.Update();
                    _itemQuant = DBOps.GetProductQuantity(_itemSKU) + Math.Abs(t_currCartQuantity - t_cartQuantity);
                    ProductsDataSource.Update();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            lvw_items.DataSource = DBOps.BuildUserCart(_userCartId);
            lvw_totals.DataSource = DBOps.BuildUserCartTotals(_userCartId);
            lvw_items.DataBind();
            lvw_totals.DataBind();

            SiteMaster master = Page.Master as SiteMaster;
            master.UpdateTotalCounters();

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if ((string)Session["currUser"] == null)
            {
                Session["loginRedirect"] = "yes";
                Session["refNum"] = _userCartId;
                //Response.Redirect(@"~/frm_payment.aspx?mode=unlogged");
            }
            else
            {
                Session["refNum"] = _userCartId;
                //Response.Redirect(@"~/frm_payment.aspx?mode=logged");
            }

            Response.Redirect(@"~/Payment.aspx");

        }


        #region DataSource Methods
        protected void lvw_items_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            // Prevent the pager from showing an empty page
            // in case the user deletes all the items in a
            // single page
            DataPager dp = (DataPager)lvw_items.FindControl("DataPager1");
            dp.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            lvw_items.DataSource = DBOps.BuildUserCart(_userCartId);
            lvw_items.DataBind();
        }

        protected void cartDatasource_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            e.Command.Parameters["@Id"].Value = _userCartId;
        }

        protected void btn_goRefCart_Click(object sender, EventArgs e)
        {
            Session["refNum"] = tbx_refNum.Text;
            Response.Redirect("~/RefCart.aspx");
        }

        protected void ProductsDataSource_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            e.Command.Parameters["@sku"].Value = _itemSKU;
            e.Command.Parameters["@qty"].Value = _itemQuant;
        }

        #endregion

        // Handles image src logic
        protected string RenderImage(object sku)
        {
            ProductsDataSource.SelectParameters[0].DefaultValue = sku.ToString();
            var result = ProductsDataSource.Select(DataSourceSelectArguments.Empty) as DataView;
            Debug.Assert(result != null, nameof(result) + " != null");
            string path = result[0]["img_url"].ToString();

            // Get all png and jpg files in current dir only
            var images = Directory.GetFiles(Server.MapPath(path) ?? throw new InvalidOperationException(), "*", SearchOption.TopDirectoryOnly)
                .Where(file => file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg"));

            // Resolve physical paths to server-relative paths
            List<string> files = images.Select(img => path + "/" + Path.GetFileName(img)).ToList();
            return files[0];
        }


    }
}