<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ItemReceipt.aspx.cs" Inherits="VMS_1.ItemReceipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />


    <div class="container">
        <h2 class="mt-4">Receipt Module</h2>

        <form id="receiptForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
            </div>--%>
            <div class="table-responsive">
                <input type="hidden" id="scaleAmount" />
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading ref">Reference/CRV No</th>
                            <th class="heading itemname">Item Name</th>
                            <th class="heading qty">Quantity</th>
                            <th class="heading denom">Denomination</th>
                            <th class="heading rcvdfrom">Received From</th>
                            <th class="heading date">Date</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="text" class="form-control" name="ref" required />
                            </td>
                            <td>
                                <select class="form-control itemname js-states" id="itemname" name="itemname" onchange="fetchBasicDenom(this.id)" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="qty" pattern="^\d+(\.\d+)?$" required />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal" name="denoms" readonly required />
                            </td>
                            <td>
                                <select class="form-control" name="rcvdfrom" onchange="checkReceivedFrom(this)" required>
                                    <option value="">Select </option>
                                    <option value="BV Yard">BV Yard</option>
                                    <option value="Local Purchase">Local Purchase</option>
                                    <option value="Other Ship">Other Ship</option>
                                    <option value="Others">Others</option>
                                </select>
                            </td>

                            <td>
                                <input type="date" class="form-control" name="date" required />
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="text-left">
                <h5>Upload CRV</h5>
                <label class="col-form-label" for="fileDate">Date</label>
                <input type="date" class="form-control " name="fileDate" id="fileDate" style="width: auto;" required />

                <label class="col-form-label" for="FileUpload1">File</label>
                <asp:FileUpload CssClass="form-control mt-2" Width="20%" ID="FileUpload1" runat="server" ToolTip="Select Only Excel File" required />
                <div>
                    <asp:Label ID="lblStatus" runat="server" Text="" Font-Bold="true"></asp:Label>
                </div>
                <%--<asp:Button CssClass="btn btn-dark mt-2" Width="10%" ID="Button1" runat="server" Text="Upload" OnClick="UploadFileButton_Click" />--%>
            </div>

            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <%
                    if (IsUserInRoleRecepit("Store Keeper"))
                    {
                %>
                <button type="button" class="btn btn-success" data-toggle="modal" data-target="#infoModal">
                    Submit
                </button>
                <%
                    }
                    else if (IsUserInRoleRecepit("Logistic Officer"))
                    {
                %>
                <asp:Button ID="Button2" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
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
            <div class="form-group">
                <label for="monthYear">Files</label>
                <asp:GridView ID="FilesGridView" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" OnRowCommand="FilesGridView_RowCommand" OnRowDataBound="GridViewFileReceipt_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Date" HeaderText="Date" ReadOnly="true" />
                        <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="true" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="DeleteButton" runat="server" CommandName="DeleteFile" CommandArgument='<%# Eval("Id") %>' Text="Delete"></asp:LinkButton>
                                <%--<asp:Button runat="server" CommandName="ViewFile" CommandArgument='<%# Eval("Id") %>' Text="View" CssClass="btn btn-info btn-sm" />--%>
                                <%--<asp:Button runat="server" CommandName="DeleteFile" CommandArgument='<%# Eval("Id") %>' Text="Delete" CssClass="btn btn-danger btn-sm" OnClientClick="return confirm('Are you sure you want to delete this file?');" />--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div>
                <h2 class="mt-4">Search Receipt</h2>
            </div>
            <div class="form-group">
                <label for="monthYear">Select Month and Year:</label>
                <input type="month" id="monthYear" name="monthYear" class="form-control" onchange="filterData()" /><br />
                &nbsp;
            </div>
            <div>
                <asp:GridView ID="GridView" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" OnRowEditing="GridView_RowEditing" OnRowUpdating="GridView_RowUpdating" OnRowCancelingEdit="GridView_RowCancelingEdit"
                    OnRowDeleting="GridView_RowDeleting" OnRowDataBound="GridViewReceipt_RowDataBound" DataKeyNames="itemid">
                    <Columns>
                        <asp:BoundField DataField="itemid" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:TemplateField HeaderText="Date">
                            <ItemTemplate>
                                <asp:Label ID="lblDate" runat="server" Text='<%# Eval("Dates") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="lblDate" runat="server" Text='<%# Bind("Dates") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Reference No">
                            <ItemTemplate>
                                <asp:Label ID="lblreferenceNos" runat="server" Text='<%# Eval("referenceNos") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="lblreferenceNos" runat="server" Text='<%# Bind("referenceNos") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Received From">
                            <ItemTemplate>
                                <asp:Label ID="lblreceivedFrom" runat="server" Text='<%# Eval("receivedFrom") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtreceivedFrom" runat="server" Text='<%# Bind("receivedFrom") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item Name">
                            <ItemTemplate>
                                <asp:Label ID="lblitemnames" runat="server" Text='<%# Eval("itemnames") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtitemnames" runat="server" Text='<%# Bind("itemnames") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Denomination">
                            <ItemTemplate>
                                <asp:Label ID="lblDenomination" runat="server" Text='<%# Eval("denominations") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDenomination" runat="server" Text='<%# Bind("denominations") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quantities">
                            <ItemTemplate>
                                <asp:Label ID="lblquantities" runat="server" Text='<%# Eval("quantities") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtquantities" runat="server" Text='<%# Bind("quantities") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <%--<asp:LinkButton ID="EditButton" runat="server" CommandName="Edit" Text="Edit"></asp:LinkButton>--%>
                                <asp:LinkButton ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete"></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="UpdateButton" runat="server" CommandName="Update" Text="Update"></asp:LinkButton>
                                <asp:LinkButton ID="CancelButton" runat="server" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-labelledby="infoModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="infoModalLabel">Item Receipt</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <p>Data once submitted cannot be changed</p>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Back</button>
                        </div>
                    </div>
                </div>
            </div>

        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.1/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.1/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script>
        $(document).ready(function () {
            loadItemNamesForRow('');
            $('#itemname').select2({
                placeholder: 'Select an option',
                allowClear: true
            });
        });

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

        function fetchBasicDenom(id) {
            var ItemValue = document.getElementById(id).value;
            document.getElementById("scaleAmount").value = "";

            if (id != null) {
                var value = id;  // Use 'var' instead of 'string'
                var parts = value.split('_');  // Use JavaScript's 'split' method
            }

            var part1 = parts[1];

            fetch('ItemReceipt.aspx/GetItemDenom', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ItemVal: ItemValue })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {
                        if (rowSequence >= 1) {
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

        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            var selectedRefNo = document.querySelector('input[type="text"][name="ref"]').value;
            var selectedRcvdFrom = document.querySelector('select[name="rcvdfrom"]').value;
            var selectedDate = document.querySelector('input[type="date"][name="date"]').value;
            newRow.innerHTML = `
                                <td><input type="text" class="form-control" value="${selectedRefNo}" disabled /></td>
                                 <td>
                                    <select class="form-control itemname js-states" id="itemname_${rowSequence}" name="itemname" onchange="fetchBasicDenom(this.id)" required>
                                        <option value="">Select</option>
                                    </select>
                                 </td>
                                <td><input type="text" class="form-control" name="qty" required pattern="^\\d+(\\.\\d+)?$" /></td>
                                <td>
                                    <input type="text" class="form-control" id="denomsVal_${rowSequence}" name="denoms" readonly />
                                </td>
                                <td>
                                    <select class="form-control" name="rcvdfrom" required onchange="checkReceivedFrom(this)" disabled>
                                        <option value="">Select</option>
                                        <option value="BV Yard" ${selectedRcvdFrom === 'BV Yard' ? 'selected' : ''}>BV Yard</option>
                                        <option value="Local Purchase" ${selectedRcvdFrom === 'Local Purchase' ? 'selected' : ''}>Local Purchase</option>
                                        <option value="Other Ship" ${selectedRcvdFrom === 'Other Ship' ? 'selected' : ''}>Other Ship</option>
                                        <option value="Others" ${selectedRcvdFrom === 'Others' ? 'selected' : ''}>Others</option>
                                    </select>
                                </td>
                                <td><input type="date" class="form-control" value="${selectedDate}" name="date" required disabled /></td>
                                <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
            loadItemNamesForRow(newRow);

            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        function loadItemNamesForRow(row) {
            fetch('ItemReceipt.aspx/GetItemNames', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {
                        var itemSelect = '';
                        var int = rowSequence - 1;
                        if (rowSequence > 0) {
                            itemSelect = document.getElementById('itemname_' + int);
                        } else {
                            itemSelect = document.getElementById('itemname');
                        }
                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item;
                            option.textContent = item;
                            itemSelect.appendChild(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching item names:', error);
                });
        }

        function checkReceivedFrom(selectElement) {
            if (selectElement.value === 'Others') {
                var parentTd = selectElement.parentNode;
                var othersInput = document.createElement('input');
                othersInput.type = 'text';
                othersInput.name = 'otherReceivedFrom';
                othersInput.className = 'form-control mt-2';
                othersInput.placeholder = 'Specify other source';
                parentTd.appendChild(othersInput);
            } else {
                var parentTd = selectElement.parentNode;
                var othersInput = parentTd.querySelector('input[name="otherReceivedFrom"]');
                if (othersInput) {
                    parentTd.removeChild(othersInput);
                }
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            var rows = document.querySelectorAll("#tableBody tr");
            rows.forEach(loadItemNamesForRow);
        });

        function filterData() {
            var monthYearInput = document.getElementById('monthYear');
            var monthYearValue = monthYearInput.value;

            if (monthYearValue) {
                var form = document.getElementById('receiptForm');
                var formData = new FormData(form);

                fetch('ItemReceipt.aspx/GetFilteredData', {
                    method: 'POST',
                    body: JSON.stringify({ monthYear: monthYearValue }),
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.d && data.d.length) {
                            var gridView = document.getElementById('<%= GridView.ClientID %>');
                            gridView.innerHTML = '';

                            var table = document.createElement('table');
                            table.className = 'table table-bordered table-striped';

                            var thead = document.createElement('thead');
                            var theadRow = document.createElement('tr');
                            ['Item Name', 'Quantity', 'Denomination', 'Received From', 'Reference No', 'Date'].forEach(function (heading) {
                                var th = document.createElement('th');
                                th.textContent = heading;
                                theadRow.appendChild(th);
                            });
                            thead.appendChild(theadRow);
                            table.appendChild(thead);

                            var tbody = document.createElement('tbody');
                            data.d.forEach(function (row) {
                                var tr = document.createElement('tr');
                                row.forEach(function (cell) {
                                    var td = document.createElement('td');
                                    td.textContent = cell;
                                    tr.appendChild(td);
                                });
                                tbody.appendChild(tr);
                            });
                            table.appendChild(tbody);

                            gridView.appendChild(table);
                        }
                    })
                    .catch(error => {
                        console.error('Error fetching filtered data:', error);
                    });
            }
        }
    </script>

</asp:Content>
