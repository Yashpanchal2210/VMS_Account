﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ByYardIssue.aspx.cs" Inherits="VMS_1.ByYardIssue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <div>
            <h2 class="mt-4">Demand Issue</h2>
        </div>
        <form id="itemMasterForm" runat="server">
            <input type="hidden" id="scaleAmount" />
            <div class="table-responsive">
            </div>
            <div class="form-group">
                <div class="col-md-12">
                    <table style="width: 100%;">
                        <tr>
                            <div>
                                <h4 class="mt-4">Demand Item Detail</h4>
                            </div>
                            <td style="width: 50%;">
                                <label for="monthYearPicker">Select Month and Year:</label>
                                <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" style="width: 70%" />
                            </td>
                            <td>
                                <asp:Label ID="lblStatus" runat="server" Text="" ForeColor="Green" Font-Bold="true"></asp:Label>
                                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-primary" />
                            </td>
                        </tr>
                        <tr>

                            <td colspan="6">
                                <div>
                                    <asp:GridView ID="GridViewRationScale" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false">
                                        <columns>
                                             <asp:TemplateField HeaderText="">
                                                <itemtemplate>
                                                    <asp:CheckBox ID="chkitem" runat="server"></asp:CheckBox>
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                                            <asp:BoundField DataField="DemandNo" HeaderText="DemandNo" />
                                            <asp:BoundField DataField="ItemCode" HeaderText="Pattern Number" />
                                            <asp:TemplateField HeaderText="Item Name">
                                                <itemtemplate>
                                                    <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ItemDeno" HeaderText="Denomination" />
                                            <asp:BoundField DataField="DemandDate" HeaderText="DemandDate" />
                                            <asp:BoundField DataField="SupplyDate" HeaderText="Supply Date" />
                                            <asp:BoundField DataField="Qty" HeaderText="Requested Qty" />
                                            <asp:TemplateField HeaderText="Issued Qty">
                                                <itemtemplate>
                                                    <asp:Textbox ID="txtIssueQty" runat="server" Text="0"></asp:Textbox>
                                                </itemtemplate>
                                            </asp:TemplateField>
                                        </columns>
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td><asp:label ID="lblRef" runat="server" Text="Issue Reference No" Font-Bold="true" required ></asp:label> </td>
                            <td ><asp:TextBox ID="txtIssueRefNo" runat="server" required ></asp:TextBox> </td>
                            <td>
                                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-primary" />
                            </td>                            
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
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
