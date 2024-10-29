<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DemandIssueList.aspx.cs" Inherits="VMS_1.DemandIssueList" %>
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
                        <asp:BoundField DataField="DemandNo" HeaderText="DemandNo" />
                        <asp:BoundField DataField="ItemCode" HeaderText="Pattern Number" />
                        <asp:TemplateField HeaderText="Item Name">
                            <itemtemplate>
                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("iLueItem") %>'></asp:Label>
                            </itemtemplate>                            
                        </asp:TemplateField>
                         <asp:BoundField DataField="BasicDenom" HeaderText="Denomination" /> 
                        <asp:BoundField DataField="Qty" HeaderText="Requested Qty" />
                        <asp:BoundField DataField="QtyIssued" HeaderText="Issued Qty" />                          
                        <asp:BoundField DataField="ReceiptQty" HeaderText="Received Qty" />
                        <asp:BoundField DataField="DateIssued" HeaderText="Date Issued" />
                        <asp:BoundField DataField="IssueRefNo" HeaderText="IssueRefNo" />
                        <asp:BoundField DataField="UnitName" HeaderText="Unit Name" />
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
