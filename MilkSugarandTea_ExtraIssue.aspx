<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="MilkSugarandTea_ExtraIssue.aspx.cs" Inherits="VMS_1.MilkSugarandTea_ExtraIssue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <h2 class="">Milk, Sugar and Tea - Extra Issue</h2>
        <span class="text-danger">*From this only Milk, Sugar and Tea will be issue</span>
        <form id="milkSugarTeaForm" runat="server">
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Date</th>
                            <th class="heading">Strength</th>
                            <th class="heading">Tea Qty</th>
                            <th class="heading">Milk Fresh Qty</th>
                            <th class="heading">Sugar Qty</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" name="date" class="form-control" />
                            </td>
                            <td>
                                <input type="number" name="strength" id="strength" class="form-control" />
                            </td>
                            <td>
                                <input type="number" name="tea" id="tea" class="form-control" readonly />
                            </td>
                            <td>
                                <input type="number" name="milk" id="milk" class="form-control" readonly />
                            </td>
                            <td>
                                <input type="number" name="sugar" id="sugar" class="form-control" readonly />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <%
                    if (IsUserInRoleRecepit("Store Keeper"))
                    {
                %>
                <button type="button" class="btn btn-success" data-toggle="modal" data-target="#infoModalMST">
                    Submit
                </button>
                <%
                    }
                    else if (IsUserInRoleRecepit("Logistic Officer"))
                    {
                %>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />


                <%
                    }%>

                <%
                    bool IsUserLoggedIn()
                    {
                        // Check if the user is logged in
                        return HttpContext.Current.Session["Role"] != null;
                    }

                    bool IsUserInRoleRecepit(string role)
                    {
                        // Check if the user is in the specified role
                        return HttpContext.Current.Session["Role"] != null && HttpContext.Current.Session["Role"].ToString() == role;
                    }

                    string GetUserName()
                    {
                        return HttpContext.Current.Session["UserName"] != null ? HttpContext.Current.Session["UserName"].ToString() : "Admin";
                    }
                %>
            </div>
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>

            <div>
                <h2 class="mt-4">Entered Data</h2>
            </div>
            <div>
                <asp:GridView ID="GridViewExtraIssueCategory5" runat="server" AutoGenerateColumns="false" OnRowDeleting="GridViewExtraIssueMST_RowDeleting" OnRowDataBound="GridViewMST_RowDataBound" CssClass="table table-bordered table-striped" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="Strength" HeaderText="Strength" ReadOnly="true"/>
                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" />
                        <asp:BoundField DataField="Qty" HeaderText="Qty" />
                        <asp:CommandField ShowDeleteButton="True" DeleteText="Delete Row" />
                    </Columns>
                </asp:GridView>
            </div>
            <div class="modal fade" id="infoModalMST" tabindex="-1" role="dialog" aria-labelledby="infoModalMSTLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="infoModalMSTLabel">Extra Issue: Milk,Sugar and Tea</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <p>Data once submitted cannot be changed</p>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Back</button>
                        </div>
                    </div>
                </div>
            </div>

        </form>
    </div>
    <script type="text/javascript">

        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `
                 <td>
                    <input type="date" name="date" class="form-control" />
                </td>
                <td>
                    <input type="number" name="strength" id="strength_${rowSequence}" class="form-control" />
                </td>
                <td>
                    <input type="number" name="tea" id="tea_${rowSequence}" class="form-control" readonly />
                </td>
                <td>
                    <input type="number" name="milk" id="milk_${rowSequence}" class="form-control" readonly />
                </td>
                <td>
                    <input type="number" name="sugar" id="sugar_${rowSequence}" class="form-control" readonly />
                </td>
                <td>
                    <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                </td>`;
            tableBody.appendChild(newRow);

            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        $(document).on('input', 'input[name="strength"]', function () {

            var strengthValue = parseFloat($(this).val());

            if (!isNaN(strengthValue)) {
                var calTea = (strengthValue * 0.007).toFixed(3);
                var calMilk = (strengthValue * 0.070).toFixed(3);
                var calSugar = (strengthValue * 0.030).toFixed(3);

                if (rowSequence == 0) {
                    $('#tea').val(calTea);
                    $('#milk').val(calMilk);
                    $('#sugar').val(calSugar);
                }
                if (rowSequence > 0) {
                    var rowInt = rowSequence - 1;
                    $('#tea_' + rowInt).val(calTea);
                    $('#milk_' + rowInt).val(calMilk);
                    $('#sugar_' + rowInt).val(calSugar);
                }

            }
        });

    </script>
</asp:Content>
