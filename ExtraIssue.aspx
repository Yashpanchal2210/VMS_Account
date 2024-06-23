﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExtraIssue.aspx.cs" MasterPageFile="~/Site.Master" Inherits="VMS_1.ExtraIssue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Page-09 Report (Extra Isssue)</h2>
        <form id="PageForm" runat="server">
            <asp:Button ID="Button3" runat="server" Text="Export to Excel" OnClick="ExportExtraIssueButton_Click" CssClass="btn btn-primary" />
            <asp:GridView ID="GridViewExtraIssue" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
            <asp:Label ID="lblStatus" runat="server"></asp:Label>
        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>

