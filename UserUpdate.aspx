﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="UserUpdate.aspx.cs" Inherits="VMS_1.UserUpdate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Update Profile</h2>

        <form id="usersForm" runat="server">
            <asp:GridView ID="GridViewUser" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" OnRowEditing="GridViewUser_RowEditing" OnRowCancelingEdit="GridViewUser_RowCancelingEdit" OnRowUpdating="GridViewUser_RowUpdating" Width="100%">
                <columns>
                    <asp:TemplateField HeaderText="Name">
                        <itemtemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("name") %>'></asp:Label>
                        </itemtemplate>
                        <edititemtemplate>
                            <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("name") %>'></asp:TextBox>
                        </edititemtemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Rank">
                        <itemtemplate>
                            <asp:Label ID="lblRank" runat="server" Text='<%# Eval("rank") %>'></asp:Label>
                        </itemtemplate>
                        <edititemtemplate>
                            <asp:TextBox ID="txtRank" runat="server" Text='<%# Bind("rank") %>'></asp:TextBox>
                        </edititemtemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Designation">
                        <itemtemplate>
                            <asp:Label ID="lblDesignation" runat="server" Text='<%# Eval("designation") %>'></asp:Label>
                        </itemtemplate>
                        <edititemtemplate>
                            <asp:TextBox ID="txtDesignation" runat="server" Text='<%# Bind("designation") %>'></asp:TextBox>
                        </edititemtemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Nuid">
                        <itemtemplate>
                            <asp:Label ID="lblNuid" runat="server" Text='<%# Eval("NudId") %>'></asp:Label>
                        </itemtemplate>
                        <edititemtemplate>
                            <asp:TextBox ID="txtNuid" runat="server" Text='<%# Bind("NudId") %>'></asp:TextBox>
                        </edititemtemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Password">
                        <itemtemplate>
                            <asp:Label ID="lblPassword" runat="server" Text="****"></asp:Label>
                        </itemtemplate>
                        <edititemtemplate>
                            <asp:TextBox ID="txtPassword" runat="server" Text='<%# Bind("Password") %>'></asp:TextBox>
                        </edititemtemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowEditButton="True" HeaderText="Action" ControlStyle-CssClass="btn" ButtonType="Link" />
                </columns>
            </asp:GridView>
        </form>
    </div>

    <script type="text/javascript">
        function setTheme(theme) {
            var gridView = document.getElementById("GridView1");

            if (theme === "blue") {
                document.body.style.backgroundColor = '#3498db';
                document.body.classList.remove('dark-theme');
                gridView.classList.remove('dark-theme-text');
                document.querySelectorAll('.heading').forEach(function (element) {
                    element.classList.remove('dark-theme');
                });
                document.querySelector('.table').classList.remove('dark-theme');
            } else if (theme === "dark") {
                document.body.style.backgroundColor = '#333';
                document.body.classList.add('dark-theme');
                gridView.classList.add('dark-theme-text');
                document.querySelectorAll('.heading').forEach(function (element) {
                    element.classList.add('dark-theme');
                });
                document.querySelector('.table').classList.add('dark-theme');
            }
        }
    </script>
</asp:Content>
