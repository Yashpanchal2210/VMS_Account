<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DiverReport.aspx.cs" MasterPageFile="~/Site.Master" Inherits="VMS_1.DiverReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Page-1 Report</h2>
        <form id="PageForm" runat="server">
            <div class="container mt-3">
                <h2>Divers</h2>
                <asp:Button ID="ExportDiversStockButton" runat="server" Text="Export to Excel" OnClick="ExportDiversStockButton_Click" CssClass="btn btn-primary" />
                <asp:GridView ID="GridViewExtraIssueDivers" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView>
            </div>
            <asp:Label ID="lblStatus" runat="server"></asp:Label>
        </form>
    </div>
</asp:Content>
