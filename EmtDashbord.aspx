<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmtDashbord.aspx.cs" Inherits="VMS_1.EmtDashbord" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <form id="usersForm" runat="server">
        <div class="container" id="conversation" runat="server">
            <div class="nk-block">
                <div class="row g-gs">
                    <div class="col-lg-12">
                        <div class="card card-bordered h-100">
                            <div class="card-inner">
                                <div class="card-title-group align-start mb-3">
                                    <%--<div class="card-title">
                                        <h6 class="title">Victualling Status</h6>
                                    </div>--%>
                                    <div>
                                        <h4>Victualling Status</h4>
                                    </div>
                                </div>
                                <div class="nk-order-ovwg mt-5">
                                    <asp:GridView ID="gvvalist" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-striped">
                                        <Columns>
                                            <asp:BoundField HeaderText="AccountID" DataField="AccountID" />
                                            <asp:TemplateField HeaderText="Victualling Account">
                                                <ItemTemplate>
                                                    <a id="alink" href='VictuallingManagement.aspx?date=<%# Eval("VA") %>&id=<%#Eval("AccountID") %>'>
                                                        <asp:Label ID="lblVA" runat="server" Text='<%# Eval("VA") %>'></asp:Label>
                                                    </a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="ForwardedBy" DataField="ForwardedBy" />
                                            <asp:BoundField HeaderText="Role" DataField="ForwardedByRole" />
                                            <asp:BoundField HeaderText="ForwardedTo" DataField="ForwardedTo" />
                                            <asp:BoundField HeaderText="Role" DataField="ForwardedToRole" />
                                            <asp:BoundField HeaderText="Remark" DataField="Remark" />
                                            <asp:BoundField HeaderText="Date" DataField="Date" />
                                            <asp:BoundField HeaderText="UnitName" DataField="UnitName" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="container" id="admin" runat="server">
            <div class="nk-block">
                <div class="row g-gs">
                    <div class="col-lg-12">
                        <div class="card card-bordered h-100">
                            <div class="card-inner">
                                <div class="card-title-group align-start mb-3">
                                    <div class="card-title">
                                        <h2 class="mt-4">Users List</h2>
                                    </div>
                                </div>
                                <div class="nk-order-ovwg mt-5">
                                    <asp:GridView ID="GridViewUser" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" Width="100%">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("name") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Rank">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRank" runat="server" Text='<%# Eval("rank") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Designation">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDesignation" runat="server" Text='<%# Eval("designation") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Nuid">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNuid" runat="server" Text='<%# Eval("NudId") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Password">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPassword" runat="server" Text="********"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Role">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRole" runat="server" Text='<%# Eval("role") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
        <div class="container" id="audit" runat="server">
            <div>
                <h4>Demand Status</h4>
            </div>
            <%--<form id="itemMasterForm" runat="server">--%>
            <input type="hidden" id="scaleAmount" />
            <div class="table-responsive">
            </div>
            <div class="form-group">
                <div class="col-md-12">
                    <table style="width: 100%;">
                        <tr>
                            <div>
                                <h4 class="mt-4"></h4>
                            </div>
                        </tr>
                    </table>
                </div>
            </div>
            <div>
                <asp:GridView ID="GridViewRationScale" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="DemandNo" HeaderText="DemandNo" />
                        <asp:BoundField DataField="ItemCode" HeaderText="Pattern Number" />
                        <asp:TemplateField HeaderText="Item Name">
                            <ItemTemplate>
                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("iLueItem") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="BasicDenom" HeaderText="Denomination" />
                        <asp:BoundField DataField="QtyIssued" HeaderText="Issued Qty" />
                        <asp:BoundField DataField="DateIssued" HeaderText="Date Issued" />
                        <asp:BoundField DataField="IssueRefNo" HeaderText="IssueRefNo" />
                        <asp:BoundField DataField="UnitName" HeaderText="Unit Name" />
                    </Columns>
                </asp:GridView>
            </div>
            <div class="text-center">
            </div>
            <div>
            </div>

        </div>
    </form>
</asp:Content>
