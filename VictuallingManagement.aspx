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
            <div id="view" runat="server">
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" style="width: 21%" required />

                <label class="col-form-label">Please enter Name and Rank of Commanding Officer:</label>
                <input type="text" class="form-control" id="coVal" name="coVal" style="width: 50%;" />

                <label class="col-form-label">Please enter Name and Rank of Logistic Officer:</label>
                <input type="text" class="form-control" id="loVal" name="loVal" style="width: 50%;" />

                <label class="col-form-label">Please enter Name and Rank of Accounting Officer:</label>
                <input type="text" class="form-control" id="aoVal" name="aoVal" style="width: 50%;" />
            </div>
            <asp:Button ID="GenerateHTMLViewButton" CssClass="btn btn-primary" runat="server" Text="Generate View" OnClick="GenerateHTMLViewButton_Click" />
            <asp:Button ID="btnSendToLogistic" CssClass="btn btn-success" runat="server" Text="Send to Logistic" OnClick="SendToLogoButton_Click" Visible="false" />
            <asp:Button ID="btnApprove1" CssClass="btn btn-success" runat="server" Text="Approve" Visible="false" />
            <asp:Button ID="btnReject1" CssClass="btn btn-danger" runat="server" Text="Reject" Visible="false" />
            <asp:Button ID="btnApprove2" CssClass="btn btn-success" runat="server" Text="Approve" Visible="false" />
            <asp:Button ID="btnReject2" CssClass="btn btn-danger" runat="server" Text="Reject" Visible="false" />
            <asp:Label ID="lblMessage" CssClass="text-success" runat="server"></asp:Label>
            </div>
            <div id="htmlViewContainer">
                <%--Start Page02--%>
                <asp:Literal ID="HTMLContentLiteralP2" runat="server"></asp:Literal>
                <%--End Page02--%>

                <%--Start Page2to7--%>
                <div id="tablesContainerPage2to7" runat="server">
                </div>
                <%--End Page2to7--%>

                <%--Start Officer Worksheet--%>
                <h3 class="mt-4 mb-4">Officer WorkSheet</h3>
                <div id="tablesContainerPageOfficerWorksheet" runat="server">
                </div>
                <%--End Officer Worksheet--%>

                <%--Start Sailor Worksheet--%>
                <h3 class="mt-4 mb-4">Sailor WorkSheet</h3>
                <div id="tablesContainerPageSailorWorksheet" runat="server">
                </div>
                <%--End Sailor Worksheet--%>

                <%--Start Page8--%>
                <asp:Literal ID="HTMLContentLiteralP8" runat="server"></asp:Literal>
                <%--End Page8--%>
                <asp:Literal ID="HTMLContentLiteralP9" runat="server"></asp:Literal>
                <asp:Literal ID="HTMLContentLiteralP10" runat="server"></asp:Literal>
                <asp:Literal ID="HTMLContentLiteralP11" runat="server"></asp:Literal>
                <asp:Literal ID="HTMLContentLiteralP12" runat="server"></asp:Literal>
                <asp:Literal ID="HTMLContentLiteralP13" runat="server"></asp:Literal>
                <h6 class="mt-4" style="text-align:center">Page 13</h6>
                <h4 class="mt-6" style="text-align:center">Strength Data</h4>
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
