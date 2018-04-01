<%@ Page Title="My Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="E_CommerceApp.FrmCheckout1" EnableViewState="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container my-4">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" RenderMode="Block">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btn_checkout" />
                <asp:AsyncPostBackTrigger ControlID="lvw_items" />
            </Triggers>
            <ContentTemplate>
                <div class="container">
                    <div class="row">
                        <div class="col col-sm-8">
                            <div class="container">
                                <asp:ListView ID="lvw_items" runat="server" OnItemCommand="lvw_items_ItemCommand" OnPagePropertiesChanging="lvw_items_PagePropertiesChanging">
                                    <LayoutTemplate>
                                        <tr id="itemPlaceholderContainer" runat="server">
                                            <td id="itemPlaceholder" runat="server"></td>
                                        </tr>
                                        <div class="container">
                                            <asp:DataPager ID="DataPager1" runat="server" PageSize="3">
                                                <Fields>
                                                    <asp:NextPreviousPagerField
                                                        ButtonCssClass="btn btn-outline-primary"
                                                        ShowPreviousPageButton="true"
                                                        ShowNextPageButton="false"
                                                        PreviousPageText="&laquo; Prev." />
                                                    <asp:NumericPagerField
                                                        NumericButtonCssClass="NumericButtonCSS"
                                                        NextPreviousButtonCssClass="btn btn-outline-primary" />
                                                    <asp:NextPreviousPagerField
                                                        ButtonCssClass="btn btn-outline-primary"
                                                        ShowNextPageButton="true"
                                                        ShowPreviousPageButton="false"
                                                        NextPageText="Next &raquo;" />
                                                </Fields>
                                            </asp:DataPager>
                                        </div>
                                    </LayoutTemplate>
                                    <EmptyDataTemplate>
                                        <div class="card text-center">
                                            <img class="card-img-top" src="Content/Images/dino.jpg" alt="Card image cap">
                                            <div class="card-body">
                                                <p class="card-text">Your cart is empty.</p>
                                                <asp:Button ID="btn_shpNow" runat="server" CssClass="btn btn-outline-success" PostBackUrl="~/Products.aspx" Text="Shop now" CausesValidation="false" />
                                            </div>
                                        </div>
                                    </EmptyDataTemplate>
                                    <ItemTemplate>
                                        <td runat="server">
                                            <div class="card">
                                                <div class="row align-items-center">
                                                    <div class="col col-md-4">
                                                        <img src='<%# RenderImage(Eval("sku"))%>' class="img-thumbnail" />
                                                    </div>
                                                    <div class="col col-md-4">
                                                        <asp:Label ID="lbl_item" runat="server" Text='<%# Eval("item") %>' CssClass="h5"></asp:Label>
                                                        <br />
                                                        <asp:Label ID="lbl_sku" runat="server" Text='<%# Eval("sku") %>' CssClass="h6 text-muted"></asp:Label>
                                                        <br />
                                                        <asp:Label ID="lbl_price" runat="server" Text='<%# Eval("price","{0:c}") %>' CssClass="h6 text-muted"></asp:Label>
                                                    </div>
                                                    <div class="col-md">
                                                        <div class="container">
                                                            <asp:Label ID="Label5" runat="server" Text="Quantity (Max: 99)" CssClass="h6 text-muted"></asp:Label>
                                                            <div class="input-group my-3">
                                                                <div class="input-group-append">
                                                                    <asp:Button ID="Btn_add" runat="server" Text="+" CssClass="btn btn-outline-primary" OnClick="Btn_add_Click" />
                                                                </div>
                                                                <asp:TextBox ID="tbx_qty" runat="server" CssClass="form-control text-center" TextMode="Number" Text='<%# Eval("quantity") %>' min="1" max="99" step="1" OnTextChanged="tbx_qty_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                <div class="input-group-append">
                                                                    <asp:Button ID="Btn_sub" runat="server" Text="-" CssClass="btn btn-outline-primary" OnClick="Btn_sub_Click" />
                                                                </div>
                                                            </div>
                                                            <asp:Button ID="btn_remove" runat="server" CssClass="btn btn-outline-danger btn-block" Text="Remove from cart" CommandArgument='<%#Eval("sku")+","+ Eval("price")+","+ Eval("quantity")%>' OnClientClick="return confirm('Are you sure you want to delete this item?')" UseSubmitBehavior="true" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </td>

                                        <br />
                                    </ItemTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                        <div class="col-6 col-sm-4 align-self-start">
                            <div class="card">
                                <h3 class="card-header">Totals</h3>
                                <div class="card-body">
                                    <asp:ListView ID="lvw_totals" runat="server">
                                        <LayoutTemplate>
                                            <table runat="server" class="table table-hover">
                                                <tr id="ItemPlaceholder" runat="server"></tr>
                                            </table>
                                        </LayoutTemplate>
                                        <EmptyDataTemplate>
                                            <p>Empty.</p>
                                        </EmptyDataTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <th scope="row">Quantity</th>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("totalQuantity") %>'></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th scope="row">Price</th>
                                                <td>
                                                    <asp:Label ID="Label3" runat="server" Text='<%# Eval("totalPrice", "{0:c}") %>'></asp:Label>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                            <br />
                            <asp:Button ID="btn_checkout" runat="server" CssClass="btn btn-success btn-block" Text="Proceed to Checkout" ClientIDMode="Static" OnClick="Button1_Click" CausesValidation="True" />
                            <br />
                            <div class="card">
                                <h5 class="card-header">View Previous Transactions</h5>
                                <div class="card-body">
                                    <asp:Panel ID="Panel1" runat="server" DefaultButton="btn_goRefCart">
                                        <div class="form-group">
                                            <asp:TextBox ID="tbx_refNum" runat="server" CssClass="form-control" Placeholder="Reference Numer" ClientIDMode="Static" CausesValidation="true" ValidationGroup="refNum" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="This field is required in order to view a previous transaction" ForeColor="#FF5050" ControlToValidate="tbx_refNum" Display="Dynamic" ValidationGroup="refNum"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="This field may only contain numbers" Display="Dynamic" ForeColor="#FF5050" ControlToValidate="tbx_refNum" ValidationExpression="[0-9]+" ValidationGroup="refNum"></asp:RegularExpressionValidator>
                                        </div>
                                        <asp:Button ID="btn_goRefCart" runat="server" CssClass="btn btn-outline-success btn-block" Text="View Transaction" ClientIDMode="Static" OnClick="btn_goRefCart_Click" CausesValidation="true" ValidationGroup="refNum" />
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
    <%-- Data source --%>
    <asp:SqlDataSource ID="cartDatasource" runat="server" ConnectionString="<%$ ConnectionStrings:CartConnectionString %>" DeleteCommand="DELETE FROM [purchases] WHERE [Id] = @Id" InsertCommand="INSERT INTO [purchases] ([customer], [items], [prices], [quants], [totalCount], [totalPrice]) VALUES (@customer, @items, @prices, @quants, @totalCount, @totalPrice)" SelectCommand="SELECT * FROM [purchases]" UpdateCommand="UPDATE [purchases] SET [customer] = @customer, [items] = @items, [prices] = @prices, [quants] = @quants, [totalCount] = @totalCount, [totalPrice] = @totalPrice WHERE [Id] = @Id" OnUpdating="cartDatasource_Updating" OnDeleting="cartDatasource_Deleting">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="customer" Type="String" />
            <asp:Parameter Name="items" Type="String" />
            <asp:Parameter Name="prices" Type="String" />
            <asp:Parameter Name="quants" Type="String" />
            <asp:Parameter Name="totalCount" Type="Int32" />
            <asp:Parameter Name="totalPrice" Type="Decimal" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="customer" Type="String" />
            <asp:Parameter Name="items" Type="String" />
            <asp:Parameter Name="prices" Type="String" />
            <asp:Parameter Name="quants" Type="String" />
            <asp:Parameter Name="totalCount" Type="Int32" />
            <asp:Parameter Name="totalPrice" Type="Decimal" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ProductsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ProductsConnectionString %>" SelectCommand="SELECT [img_url] FROM [Products] WHERE ([sku] = @sku)" UpdateCommand="UPDATE Products SET [qty] = @qty WHERE [sku] = @sku" OnUpdating="ProductsDataSource_Updating">
        <SelectParameters>
            <asp:Parameter Name="sku" Type="String" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="sku" Type="String" />
            <asp:Parameter Name="qty" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
</asp:Content>
