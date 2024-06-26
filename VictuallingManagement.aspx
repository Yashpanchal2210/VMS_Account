<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VictuallingManagement.aspx.cs" Inherits="VMS_1.VictuallingManagement" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /*table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 5px;
            margin-bottom: 5px;
        }

        th, td {
            border: 1px solid black;
            padding: 8px;
            margin-bottom: 5px;
            text-align: left;
        }

        th {
            background-color: #f2f2f2;
        }*/
    </style>
    <div class="container">
        <h2 class="mt-4">Victualling Account</h2>
        <form id="PageForm" runat="server">
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" required class="form-control date-picker" style="width: 21%" />

                <label class="col-form-label">Commanding Officer:</label>
                <input type="text" class="form-control" id="coVal" name="coVal" style="width: 50%;" />

                <label class="col-form-label">Logistic Officer:</label>
                <input type="text" class="form-control" id="loVal" name="loVal" style="width: 50%;" />

                <label class="col-form-label">Accounting Officer:</label>
                <input type="text" class="form-control" id="aoVal" name="aoVal" style="width: 50%;" />
            </div>
            <asp:Button ID="GenerateHTMLViewButton" CssClass="btn btn-primary" runat="server" Text="Generate HTML View" OnClick="GenerateHTMLViewButton_Click" />
            <asp:Button ID="btnSendToLogistic" CssClass="btn btn-success" runat="server" Text="Send to Logistic" Visible="false"/>
            <asp:Button ID="btnApprove1" CssClass="btn btn-success" runat="server" Text="Approve" Visible="false" />
            <asp:Button ID="btnReject1" CssClass="btn btn-danger" runat="server" Text="Reject" Visible="false" />
            <asp:Button ID="btnApprove2" CssClass="btn btn-success" runat="server" Text="Approve" Visible="false" />
            <asp:Button ID="btnReject2" CssClass="btn btn-danger" runat="server" Text="Reject" Visible="false" />
            <asp:Label ID="lblMessage" CssClass="text-success" runat="server"></asp:Label>
            <div id="htmlViewContainer">
                <%--Start Page02--%>
                <asp:Literal ID="HTMLContentLiteralP2" runat="server"></asp:Literal>
                <%--End Page02--%>

                <%--Start Page2to7--%>
                <div id="tablesContainerPage2to7" runat="server">
                </div>
                <%--End Page2to7--%>

                <%--Start Page8--%>
                <asp:Literal ID="HTMLContentLiteralP8" runat="server"></asp:Literal>
                <%--End Page8--%>

                <%--Start Page14--%>
                <asp:Literal ID="HTMLContentLiteralP14" runat="server"></asp:Literal>
                <%--End Page14--%>
            </div>
        </form>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>
