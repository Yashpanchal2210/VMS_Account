<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OfficerWorksheet.aspx.cs" Inherits="VMS_1.OfficerWorksheet" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Officer Work Sheet Report</h2>
        <form id="PageForm" runat="server">
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" required class="form-control date-picker" style="width: 21%" />
            </div>
            <%--<asp:Button ID="ExportToExcelButton" runat="server" Text="Download Report" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" BackColor="#009900" />--%>
            <asp:Label ID="lblStatus" runat="server" CssClass="text-danger"></asp:Label>
            <div>
                <asp:GridView ID="GridViewOfficersSheet" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView>
            </div>
        </form>



    </div>
</asp:Content>
