﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="RationIssueOnPayment.aspx.cs" Inherits="VMS_1.RationIssueOnPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <h2 class="">Ration Issue On Payment</h2>
        <form id="limeFreshForm" runat="server">
            <input type="hidden" id="scaleAmount" />
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Date</th>
                            <th class="heading">Item</th>
                            <th class="heading">rate</th>
                            <th class="heading">Qty</th>
                            <th class="heading">Denom</th>
                            <th class="heading">Reference No</th>
                            <th class="heading">Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" name="date" class="form-control" />
                            </td>
                            <td>
                                <select class="form-control item" name="item" id="item" onchange="fetchBasicDenom(this.id)" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td style="width: 10%;">
                                <input type="text" name="rate" id="rate" class="form-control" readonly />
                            </td>
                            <td style="width: 12%;">
                                <input type="text" name="qty" id="qty" class="form-control" />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal" name="denomsVal" readonly />
                            </td>
                            <td>
                                <input type="text" class="form-control" name="refno" id="refno" />
                            </td>
                            <td></td>
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
                <asp:GridView ID="GridViewRationPaymentIssue" runat="server" CssClass="table table-bordered table-striped"
                    AutoGenerateColumns="False" OnRowDeleting="GridViewExtraIssuePayment_RowDeleting" OnRowDataBound="GridViewRIP_RowDataBound" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" />
                        <asp:BoundField DataField="Qty" HeaderText="Qty" />
                        <asp:CommandField ShowDeleteButton="True" DeleteText="Delete Row" />
                    </Columns>
                </asp:GridView>
            </div>
        </form>
    </div>


    <script type="text/javascript">
        $(document).ready(function () {
            fetchItems('item');
        });
        function fetchBasicDenom(id) {
            var ItemValue = document.getElementById(id).value;
            document.getElementById("scaleAmount").value = "";

            if (id != null) {
                var value = id;  // Use 'var' instead of 'string'
                var parts = value.split('_');  // Use JavaScript's 'split' method
            }

            var part1 = parts[1];

            fetch('RationIssueOnPayment.aspx/GetItemDenom', {
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
            newRow.innerHTML = `
                <td>
                    <input type="date" name="date" class="form-control" />
                </td>
                 <td>
                    <select class="form-control item" name="item" id="item_${rowSequence}" onchange="fetchBasicDenom(this.id)" required>
                        <option value="">Select</option>
                    </select>
                </td>
                <td style="width:10%;">
                    <input type="text" name="rate" id="rate_${rowSequence}" class="form-control" readonly />
                </td>
                 <td style="width:12%;">
                     <input type="text" name="qty" id="qty_${rowSequence}" class="form-control" />
                 </td>
                <td>
                     <input type="text" class="form-control" id="denomsVal_${rowSequence}" name="denoms" readonly />
                </td>
                <td>
                    <input type="text" class="form-control" name="refno" id="refno_${rowSequence}"/>
                </td>
                <td>
                    <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                </td>`;
            tableBody.appendChild(newRow);
            fetchItems('item_' + rowSequence);
            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        $(document).on('change', 'select[name="item"]', function () {
            var selectedItem = $(this).val();

            fetch('RationIssueOnPayment.aspx/GetItemDataByItemId', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ Id: selectedItem })
            })
                .then(response => response.json())
                .then(data => {
                    var parsedData = JSON.parse(data.d);
                    var rates = parsedData.Rates[0];
                    //var referenceNos = parsedData.ReferenceNos;
                    var int = rowSequence - 1;
                    var rateVal = $("#rate").val();
                    if (rateVal == null || rateVal == "") {
                        $('#rate').val(rates);
                    }
                    $('#rate_' + int).val(rates);

                    //$('select[name="refno"]').empty();
                    //referenceNos.forEach(function (referenceNo) {
                    //    $('select[name="refno"]').append(`<option value="${referenceNo}">${referenceNo}</option>`);
                    //});
                    //$(`#refno_${rowSequence}`).empty();
                    //referenceNos.forEach(function (referenceNo) {
                    //    $(`#refno_${rowSequence}`).append(`<option value="${referenceNo}">${referenceNo}</option>`);
                    //});
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        });



        function fetchItems(dropdownId) {
            fetch('RationIssueOnPayment.aspx/GetItems', {
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
                            option.value = item.Value;
                            option.textContent = item.Text;
                            option.setAttribute('data-scaleamount', item.ScaleAmount);
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

