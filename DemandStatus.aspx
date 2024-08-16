<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DemandStatus.aspx.cs" Inherits="VMS_1.DemandStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <div>
            <h2 class="mt-4">Demand Status</h2>
        </div>
        <form id="itemMasterForm" runat="server">
            <input type="hidden" id="scaleAmount" />
            <div class="table-responsive">
            </div>
            <div class="form-group">
                <div class="col-md-12">
                    <table style="width: 100%;">
                        <tr>

                            <td style="width: 50%;">
                                <label for="monthYearPicker">Select Month and Year:</label>
                                <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" style="width: 70%" />
                            </td>
                            <td class="text-left">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-primary" />
                            </td>
                            <td style="width: 60%;">
                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </div>

            <div>
                <div class="form-group">
                    <h4 class="mt-4">Demand Pending With LOGO</h4>
                </div>
                <asp:GridView ID="GridViewRationScale" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" AutoPostBack="true" DataKeyNames="ID">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="DemandNo" HeaderText="DemandNo" />
                        <asp:BoundField DataField="ItemCode" HeaderText="Pattern Number" />
                        <asp:TemplateField HeaderText="Item Name">
                            <ItemTemplate>
                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlItemName" runat="server" CssClass="form-control itemname" AppendDataBoundItems="true">
                                    <asp:ListItem Text="Select" Value="" />
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Qty" HeaderText="Qty" />
                        <asp:BoundField DataField="ItemDeno" HeaderText="Denomination" />
                        <asp:BoundField DataField="DemandDate" HeaderText="Demand Date" />
                        <asp:BoundField DataField="SupplyDate" HeaderText="Supply Date" />
                    </Columns>
                </asp:GridView>
            </div>
            <div class="text-center">
            </div>
            <div>
            </div>
            <div class="form-group">
                <div class="col-md-12">
                    <table style="width: 100%;">
                        <tr>
                            <div>
                                <h4 class="mt-4">Demand Pending With BVY</h4>
                            </div>
                            <%-- <td style="width: 50%;">
                                <label for="monthYearPicker">Select Month and Year:</label>
                                <input type="month" id="monthapp" runat="server" class="form-control date-picker" style="width: 70%" />
                            </td>
                            <td class="text-left">
                                <asp:Button ID="btnApprovedSearch" runat="server" Text="Search" OnClick="btnApprovedSearch_Click" CssClass="btn btn-primary" />
                            </td>--%>
                            <td style="width: 30%;">
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label></td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </div>

            <div>
                <asp:GridView ID="gvapproved" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" DataKeyNames="ID">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="DemandNo" HeaderText="DemandNo" />
                        <asp:BoundField DataField="ItemCode" HeaderText="Pattern Number" />
                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" />
                        <asp:BoundField DataField="Qty" HeaderText="Requested Qty" />
                        <asp:BoundField DataField="IssuedQty" HeaderText="Issued Qty" />
                        <asp:BoundField DataField="ItemDeno" HeaderText="Denomination" />
                        <asp:BoundField DataField="DemandDate" HeaderText="Demand Date" />
                        <asp:BoundField DataField="SupplyDate" HeaderText="Supply Date" />
                    </Columns>
                </asp:GridView>
            </div>
        </form>
    </div>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>
