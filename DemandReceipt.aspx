﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DemandReceipt.aspx.cs" Inherits="VMS_1.DemandReceipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Demand Receipt Module</h2>
        <form id="receiptForm" runat="server">           
            <div class="table-responsive">
                <input type="hidden" id="scaleAmount" />
                <table class="table" id="myTable">
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <asp:GridView ID="gvDemandIssue" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chk" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="DemandNo" HeaderText="DemandNo" />
                                        <asp:BoundField DataField="ItemCode" HeaderText="Pattern No" />
                                        <asp:BoundField DataField="iLueItem" HeaderText="Item Name" />
                                        <asp:BoundField DataField="BasicDenom" HeaderText="Deno" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQty" runat="server" Text='<%#Eval("QtyIssued") %>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%-- <asp:BoundField DataField="QtyIssued" HeaderText="Qty Issued" />--%>
                                        <asp:BoundField DataField="DateIssued" HeaderText="Date Issued" DataFormatString="{0:yyyy-MM-dd}" />
                                        <asp:BoundField DataField="IssueRefNo" HeaderText="IssueRefNo" />                                       
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </div>
            <div class="text-center">
                <asp:Button ID="Button2" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />

            </div>


        </form>
    </div>
</asp:Content>
