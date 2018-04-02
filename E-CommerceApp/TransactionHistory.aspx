<%@ Page Title="Previous Transactions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TransactionHistory.aspx.cs" Inherits="E_CommerceApp.TransactionHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="card">
                    <h3 class="card-header">Previous Transactions</h3>
                    <div class="card-body">
                        <asp:ListView ID="Lvw_transactions" runat="server" OnItemCommand="Lvw_transactions_ItemCommand" OnPagePropertiesChanging="Lvw_transactions_PagePropertiesChanging">
                            <LayoutTemplate>
                                <table runat="server" class="table table-hover text-center">
                                    <thead>
                                        <tr runat="server">
                                            <td runat="server">Transaction Number</td>
                                            <td runat="server">Checkout Date</td>
                                            <td runat="server">Reference Number</td>
                                            <td runat="server">Total Item Count</td>
                                            <td runat="server">Total Price</td>
                                            <td runat="server"></td>
                                        </tr>
                                    </thead>

                                    <tr id="ItemPlaceholder" runat="server">
                                    </tr>
                                </table>
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
                                        <p class="card-text">You do not have any previous transactions.</p>
                                        <asp:Button ID="btn_shpNow" runat="server" CssClass="btn btn-outline-success" PostBackUrl="~/Products.aspx" Text="Shop now" CausesValidation="false" />
                                    </div>
                                </div>
                            </EmptyDataTemplate>

                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text='<%# Eval("TransactionNumber") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("Date","{0:d}") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("RefNum") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("ItemCount") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text='<%# Eval("TotalPrice","{0:c}") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="Btn_view" runat="server" Text="View Transaction" CssClass="btn btn-outline-success"
                                            CommandArgument='<%# Eval("RefNum") %>' UseSubmitBehavior="false" />
                                    </td>
                                </tr>

                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>


    </div>
    <br />
</asp:Content>
