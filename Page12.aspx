﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page12.aspx.cs" MasterPageFile="~/Site.master" Inherits="VMS_1.Page12" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Page 12 Report</h2>
        <form id="PageForm" runat="server">
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" required class="form-control date-picker" style="width: 21%" />
            </div>
            <asp:Button ID="PaymentExportToExcelButton" runat="server" Text="Download Report" OnClick="PaymentExportToExcelButton_Click" CssClass="btn btn-primary" BackColor="#009900" />
        </form>

        <div>

        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>
