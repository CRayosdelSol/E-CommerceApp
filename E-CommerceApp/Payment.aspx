<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="E_CommerceApp.frm_payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="margin-top: 2rem; margin-bottom: 2rem;">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="container">
                    <div class="row">
                        <div class="col col-sm-8">
                            <div class="card">
                                <h3 class="card-header">Payment Details</h3>
                                <div class="card-block">
                                    <div class="container">
                                        <asp:FormView ID="FormView1" runat="server" CssClass="container">
                                            <ItemTemplate>
                                                <div class="form-group">
                                                    <asp:Label ID="Label1" runat="server" CssClass="form-label" Text="Name on Card"></asp:Label>
                                                    <asp:TextBox ID="Tbx_cardOwner" runat="server" CssClass="form-control form-control-lg"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="This field is required" ControlToValidate="Tbx_cardOwner" ForeColor="#FF5050" Display="Dynamic"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="This field may only contain letters and spaces." ControlToValidate="Tbx_cardOwner" ForeColor="#FF5050" ValidationExpression="^[a-z A-Z]+$" Display="Dynamic"></asp:RegularExpressionValidator>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label ID="Label4" runat="server" CssClass="form-label" Text="Credit Card Number"></asp:Label>
                                                    <asp:TextBox ID="Tbx_cardNum" runat="server" CssClass="form-control form-control-lg"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="This field is required" ControlToValidate="Tbx_cardNum" ForeColor="#FF5050" Display="Dynamic"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ErrorMessage="Invalid credit card number." ControlToValidate="Tbx_cardNum" ForeColor="#FF5050" ValidationExpression="^[\d]+$" Display="Dynamic"></asp:RegularExpressionValidator>
                                                </div>
                                                </div>
                                                <div class="form-row">
                                                    <div class="form-group col-sm-6">
                                                        <asp:Label ID="Label5" runat="server" CssClass="form-label" Text="Valid Until"></asp:Label>
                                                        <asp:TextBox ID="Tbx_Expiry" runat="server" CssClass="form-control form-control-lg"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="This field is required or the date entered is invalid." ControlToValidate="Tbx_Expiry" ForeColor="#FF5050" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:CompareValidator ID="CompareEndTodayValidator" Operator="GreaterThan" Type="Date" ControlToValidate="Tbx_Expiry" ErrorMessage="The card you will use should not be expired." runat="server" Display="Dynamic" ForeColor="#FF5050" />

                                                    </div>
                                                    <div class="form-group col-sm-6">
                                                        <asp:Label ID="Label6" runat="server" CssClass="form-label" Text="Credit Card Security Code"></asp:Label>
                                                        <asp:TextBox ID="Tbx_secCode" runat="server" CssClass="form-control form-control-lg"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="This field is required" ControlToValidate="Tbx_secCode" ForeColor="#FF5050" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ErrorMessage="Invalid security code." ControlToValidate="Tbx_secCode" ForeColor="#FF5050" ValidationExpression="^([0-9]{3,4})$" Display="Dynamic"></asp:RegularExpressionValidator>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label ID="Label7" runat="server" CssClass="form-label" Text="Shipping Address"></asp:Label>
                                                    <asp:TextBox ID="Tbx_Addr" runat="server" CssClass="form-control form-control-lg" TextMode="MultiLine"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="This field is required" ControlToValidate="Tbx_Addr" ForeColor="#FF5050" Display="Dynamic"></asp:RequiredFieldValidator>
                                                </div>
                                            </ItemTemplate>
                                        </asp:FormView>
                                    </div>
                                </div>

                            </div>

                        </div>
                        <div class="col-6 col-sm-4 align-self-start">
                            <div class="card">
                                <h3 class="card-header">Totals</h3>
                                <div class="card-block">
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
                            <asp:Button ID="btn_checkout" runat="server" CssClass="btn btn-outline-success btn-block" Text="Checkout" ClientIDMode="Static" OnClick="btn_checkout_Click" CausesValidation="true" />
                        </div>
                        <br />

                    </div>
                    <br />
                    <asp:SqlDataSource ID="CartDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CartConnectionString %>" DeleteCommand="DELETE FROM [purchases] WHERE [Id] = @Id" InsertCommand="INSERT INTO [purchases] ([date]) VALUES (@date)" OnUpdated="CartDataSource_Updated" OnUpdating="CartDataSource_Updating" SelectCommand="SELECT [date], [Id] FROM [purchases]" UpdateCommand="UPDATE [purchases] SET [date] = @date WHERE [Id] = @Id">
                        <DeleteParameters>
                            <asp:Parameter Name="Id" Type="Int32" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter DbType="Date" Name="date" />
                        </InsertParameters>
                        <UpdateParameters>
                            <asp:Parameter DbType="Date" Name="date" />
                            <asp:Parameter Name="Id" Type="Int32" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</asp:Content>
