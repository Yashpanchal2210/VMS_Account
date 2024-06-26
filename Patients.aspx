<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Patients.aspx.cs" Inherits="VMS_1.Patients" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="">Patients</h2>
        <form id="patientsForm" runat="server">
            <input type="hidden" id="scaleAmount" />
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Date</th>
                            <th class="heading">Item Name</th>
                            <th class="heading">Qty</th>
                            <th class="heading">Denom</th>
                            <th class="heading">Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" name="date" class="form-control" />
                            </td>
                            <td>
                                <select class="form-control" name="itemname" id="itemname" onchange="fetchBasicDenom(this.id)" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="number" name="qty" id="qty" class="form-control" />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal" name="denoms" readonly />
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
                <button type="button" class="btn btn-success" data-toggle="modal" data-target="#infoModalPatient">
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
                <asp:GridView ID="GridViewPatients" runat="server" CssClass="table table-bordered table-striped"
                    AutoGenerateColumns="False" OnRowDeleting="GridViewExtraPatients_RowDeleting" OnRowDataBound="GridViewP_RowDataBound" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" />
                        <asp:BoundField DataField="Qty" HeaderText="Qty" />
                        <asp:CommandField ShowDeleteButton="True" DeleteText="Delete Row" />
                    </Columns>
                </asp:GridView>
            </div>

            <div class="modal fade" id="infoModalPatient" tabindex="-1" role="dialog" aria-labelledby="infoModalPatientLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="infoModalPatientLabel">Extra Issue: Patients</h5>
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
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            fetchItems('');
        });

        var rowSequence = 1;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            var selectedDate = document.querySelector('input[type="date"][name="date"]').value;
            newRow.innerHTML = `
         <td>
            <input type="date" name="date" class="form-control" value="${selectedDate}" readonly />
        </td>
        <td>
             <select class="form-control itemname" name="itemname" id="itemname_${rowSequence}" onchange="fetchBasicDenom(this.id)" required>
                <option value="">Select</option>
            </select>
        </td>
        <td>
            <input type="number" name="qty" id="qty" class="form-control" />
        </td>
        <td>
            <input type="text" class="form-control" id="denomsVal_${rowSequence}" name="denoms" readonly />
        </td>
        <td>
            <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
        </td>`;
            tableBody.appendChild(newRow);
            fetchItems(newRow);
            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        function fetchItems(row) {
            fetch('Divers_ExtraIssue.aspx/GetItemNames', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {
                        if (row) {
                            itemSelect = row.querySelector('.itemname');
                        } else {
                            itemSelect = document.getElementById('itemname');
                        }
                        // Clear existing options
                        itemSelect.innerHTML = '<option value="">Select</option>';

                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item.Value;
                            option.textContent = item.Text;
                            itemSelect.appendChild(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching item names:', error);
                });
        }

        function fetchBasicDenom(id) {
            var ItemValue = document.getElementById(id).value;
            document.getElementById("scaleAmount").value = "";

            if (id != null) {
                var value = id;  // Use 'var' instead of 'string'
                var parts = value.split('_');  // Use JavaScript's 'split' method
            }

            var part1 = parts[1];

            fetch('Wastage.aspx/GetItemDenom', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ItemVal: ItemValue })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {

                        if (rowSequence > 1) {
                            var denomsDropdown = document.getElementById("denomsVal_" + part1);
                            denomsDropdown.value = data.d;
                        } else {
                            var denomsDropdown = document.getElementById("denomsVal");
                            denomsDropdown.value = data.d;
                        }
                    }
                })
                .catch(error => {
                    console.error('Error fetching basic denomination:', error);
                });
        }

    </script>
</asp:Content>
