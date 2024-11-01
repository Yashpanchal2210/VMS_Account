﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportsGenerate.aspx.cs" Inherits="VMS_1.ReportsGenerate" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Strength Report</h2>
        <form id="strengthForm" runat="server">
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" required class="form-control date-picker" style="width: 21%" />
            </div>
            <asp:Button ID="ViewReport" runat="server" Text="View Report" OnClick="ViewToExcelButton_Click" CssClass="btn btn-primary" />
            <asp:Button ID="ExportToExcelButton" runat="server" Text="Download Report" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" BackColor="#009900" />

            <h2 class="mt-4">Strength Data</h2>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-bordered"
                DataKeyNames="Id">
                <Columns>
                    <asp:BoundField DataField="dates" HeaderText="Dates" SortExpression="dates" />
                    <asp:TemplateField HeaderText="Veg Officers">
                        <ItemTemplate>
                            <asp:Label ID="lblVegOfficers" runat="server" Text='<%# Eval("vegOfficers") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtVegOfficers" runat="server" Text='<%# Bind("vegOfficers") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="NonVeg Officers">
                        <ItemTemplate>
                            <asp:Label ID="lblNonVegOfficers" runat="server" Text='<%# Eval("nonVegOfficers") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtNonVegOfficers" runat="server" Text='<%# Bind("nonVegOfficers") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Veg Sailor">
                        <ItemTemplate>
                            <asp:Label ID="lblVegSailor" runat="server" Text='<%# Eval("vegSailor") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtVegSailor" runat="server" Text='<%# Bind("vegSailor") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="NonVeg Sailor">
                        <ItemTemplate>
                            <asp:Label ID="lblNonVegSailor" runat="server" Text='<%# Eval("nonVegSailor") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtNonVegSailor" runat="server" Text='<%# Bind("nonVegSailor") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </form>
    </div>
</asp:Content>
