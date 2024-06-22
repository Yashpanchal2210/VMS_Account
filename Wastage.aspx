﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Wastage.aspx.cs" Inherits="VMS_1.Wastage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="">Wastage</h2>
        <form id="wastageForm" runat="server">
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
                                <select name="itemname" id="itemname" class="form-control">
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="number" name="qty" id="qty" class="form-control" />
                            </td>
                            <td>
                                <select class="form-control" name="denom" id="denom">
                                    <option value="">Select</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Other">Other</option>
                                </select>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
            </div>
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>

            <div>
                <h2 class="mt-4">Entered Data</h2>
            </div>
            <div>
                <asp:GridView ID="GridViewWastage" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView>
            </div>
        </form>
    </div>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            fetchItems('itemname');
        });

        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `
         <td>
            <input type="date" name="date" class="form-control" />
        </td>
        <td>
            <select name="itemname" id="itemname_${rowSequence}" class="form-control">
                <option value="">Select</option>
            </select>
        </td>
        <td>
            <input type="number" name="qty" id="qty" class="form-control" />
        </td>
        <td>
            <select class="form-control" name="denom" id="denom">
                <option value="">Select</option>
                <option value="Kgs">Kgs</option>
                <option value="Ltr">Ltr</option>
                <option value="Nos">Nos</option>
                <option value="Other">Other</option>
            </select>
        </td>
        <td>
            <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
        </td>`;
            tableBody.appendChild(newRow);
            fetchItems('itemname_' + rowSequence);
            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        function fetchItems(dropdownId) {
            fetch('Wastage.aspx/GetItems', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && Array.isArray(data.d)) {
                        var dropdown = document.getElementById(dropdownId);
                        dropdown.innerHTML = '<option value="">Select</option>';
                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item.Text;
                            option.textContent = item.Text;
                            dropdown.appendChild(option);
                        });
                    } else {
                        console.error('Invalid data format:', data.d);
                    }
                })
                .catch(error => {
                    console.error('Error fetching items:', error);
                });
        }

    </script>
</asp:Content>
