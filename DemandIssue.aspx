<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DemandIssue.aspx.cs" Inherits="VMS_1.DemandIssue" %>

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
                            <div>
                                <h4 class="mt-4"></h4>
                            </div>
                        </tr>
                    </table>
                </div>
            </div>
            <div>
                <asp:GridView ID="GridViewRationScale" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false">
                    <columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="DemandNo" HeaderText="DemandNo" />
                        <asp:BoundField DataField="ItemCode" HeaderText="Pattern Number" />
                        <asp:TemplateField HeaderText="Item Name">
                            <itemtemplate>
                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                            </itemtemplate>                            
                        </asp:TemplateField>
                        <asp:BoundField DataField="Qty" HeaderText="Requested Qty" />
                        <asp:BoundField DataField="IssuedQty" HeaderText="Issued Qty" />
                        <asp:BoundField DataField="ItemDeno" HeaderText="Denomination" />
                        <asp:BoundField DataField="DemandDate" HeaderText="Demand Date" />
                        <asp:BoundField DataField="SupplyDate" HeaderText="Supply Date" />
                    </columns>
                </asp:GridView>
            </div>
            <div class="text-center">
            </div>
            <div>
            </div>
        </form>
    </div>
</asp:Content>
