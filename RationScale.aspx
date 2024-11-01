﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="RationScale.aspx.cs" Inherits="VMS_1.RationScale" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .input-group-text {
            background-color: #fff; /* Match the background color with the input field */
            border: 1px solid #ced4da; /* Match the border with the input field */
        }

        .input-group-prepend .input-group-text {
            border-right: 0; /* Remove the right border of the prepend to blend with the input */
        }

        .input-group .form-control {
            border-left: 0; /* Remove the left border of the input to blend with the prepend */
        }
    </style>

    <div class="container">
        <h2 class="mt-4">Item Rate</h2>

        <form id="itemMasterForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
            </div>--%>
            <input type="hidden" id="scaleAmount" />
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Name</th>
                            <th class="heading">Rate</th>
                            <th class="heading">Denomination</th>
                            <th class="heading">Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <select class="form-control itemname" id="itemname" name="itemname" onchange="fetchBasicDenom(this.id)" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <div class="input-group-prepend" style="margin-bottom: -34px; margin-top: 3px;">
                                    <span class="input-group-text"><i class="fas fa-rupee-sign"></i></span>
                                </div>
                                <input type="text" class="form-control" name="rate" required pattern="^\d+(\.\d+)?$" style="padding-left: 50px;" />Default Value 0</td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal" name="denoms" readonly />
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>
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
                <asp:GridView ID="GridViewRationScale" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" AutoPostBack="true" DataKeyNames="ID" OnRowEditing="GridViewRationScale_RowEditing" OnRowUpdating="GridViewRationScale_RowUpdating" OnRowCancelingEdit="GridViewRationScale_RowCancelingEdit" OnRowDeleting="GridViewRationScale_RowDeleting" OnRowDataBound="GridViewRation_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />

                        <asp:TemplateField HeaderText="Item Name">
                            <ItemTemplate>
                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlItemName" runat="server" CssClass="form-control itemname" AppendDataBoundItems="true">
                                    <asp:ListItem Text="Select" Value="" />
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Rate" HeaderText="Rate" />
                        <asp:BoundField DataField="Denomination" HeaderText="Denomination" />
                        <asp:CommandField HeaderText="Action" ShowEditButton="true" ShowDeleteButton="true" />
                    </Columns>
                </asp:GridView>

            </div>
        </form>
    </div>

    <script>
        $(document).ready(function () {
            fetchItems('');
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

        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td>

                    <select class="form-control itemname" id="itemname_${rowSequence}" name="itemname" onchange="fetchBasicDenom(this.id)" required>
                        <option value="">Select</option>
                    </select>
                    </td>

    <td>
     <div class="input-group-prepend" style="margin-bottom: -34px;margin-top: 3px;">
     <span class="input-group-text"><i class="fas fa-rupee-sign"></i></span>
 </div>
    <input type="number" class="form-control" name="rate" required min="0" step="0.01" style="padding-left:50px;" />Default Value 0</td>
   <td>
    <input type="text" class="form-control" id="denomsVal_${rowSequence}" name="denoms" readonly />
</td>
    <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
            fetchItems(newRow);
            rowSequence++;
        }

        function fetchBasicDenom(id) {
            var ItemValue = document.getElementById(id).value;
            document.getElementById("scaleAmount").value = "";

            if (id != null) {
                var value = id;  // Use 'var' instead of 'string'
                var parts = value.split('_');  // Use JavaScript's 'split' method
            }

            var part1 = parts[1];

            fetch('RationScale.aspx/GetItemDenom', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ItemVal: ItemValue })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {
                        if (id == "itemname") {
                            var denomsDropdown = document.getElementById("denomsVal");
                            denomsDropdown.value = data.d;
                        } else {
                            var denomsDropdown = document.getElementById("denomsVal_" + part1);
                            denomsDropdown.value = data.d;
                        }
                    }
                })
                .catch(error => {
                    console.error('Error fetching basic denomination:', error);
                });
        }

        function deleteRow(btn) {
            var row = btn.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }


        function fetchItems(row) {
            fetch('RationScale.aspx/GetItemNames', {
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
    </script>
</asp:Content>
