﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Divers_ExtraIssue.aspx.cs" Inherits="VMS_1.Divers_ExtraIssue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Extra Issue - Divers</h2>
        <span class="text-danger-">WORK IN PROGRESS</span>
        <form id="extraIssueForm" runat="server">
            <div class="table-responsive">
                <table class="table" id="extraissueTable">
                    <thead>
                        <tr>
                            <th class="heading">Date</th>
                            <th class="heading">Name</th>
                            <th class="heading">Rank</th>
                            <th class="heading">P.No.</th>
                            <th class="heading">NO. OF DAYS ENTILED</th>
                            <th class="heading">Item 1</th>
                            <th class="heading">Item 2</th>
                            <th class="heading">Qty</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" class="form-control" id="dateVal" name="date" required />
                            </td>
                            <td style="width: 10%;">
                                <input type="text" class="form-control" id="nameVal" name="name" required style="text-transform: capitalize;" />
                            </td>
                            <td>
                                <select class="form-control rank" id="rank" name="rank" width="200px" required>
                                    <option value="" selected>Select</option>
                                    <option value="Sub Lieutenant">Sub Lieutenant</option>
                                    <option value="Lieutenant">Lieutenant</option>
                                    <option value="Lieutenant Commander">Lieutenant Commander</option>
                                    <option value="Commander">Commander</option>
                                    <option value="Captain">Captain</option>
                                    <option value="Commodore">Commodore</option>
                                    <option value="Rear Admiral">Rear Admiral</option>
                                    <option value="Vice Admiral">Vice Admiral</option>
                                    <option value="Admiral">Admiral</option>
                                    <option value="Seaman 2nd Class">Seaman 2nd Class</option>
                                    <option value="Seaman Ist Class">Seaman Ist Class</option>
                                    <option value="Leading Rate">Leading</option>
                                    <option value="Petty Officer">Petty Officer</option>
                                    <option value="Chief Petty Officer">Chief Petty Officer</option>
                                    <option value="Master Chief Petty Officer IInd Class">Master Chief Petty Officer IInd Class</option>
                                    <option value="Master Chief Petty Officer Ist Class">Master Chief Petty Officer Ist Class</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control pno" id="pno" name="pno" required style="text-transform: capitalize;" />
                            </td>
                            <td>
                                <input type="text" class="form-control days" id="days" name="days" required />
                            </td>
                            <td>
                                <select class="form-control" name="itemname1" id="itemname1" required>
                                    <option value="">Select</option>
                                    <option value="Milk Fresh">Milk Fresh (200 ml)</option>
                                    <option value="Milk Tinned">Milk Tinned (80 gms)</option>
                                    <option value="Milk Powder">Milk Powder (28 gms)</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control" name="itemname2" id="itemname2" required>
                                    <option value="">Select</option>
                                    <option value="Eggs">Eggs (2 Nos)</option>
                                    <option value="Milk Fresh">Milk Fresh (150 ml)</option>
                                    <option value="Milk Tinned">Milk Tinned (55 gms)</option>
                                    <option value="Milk Powder">Milk Powder (20 gms)</option>
                                    <option value="Cheese Tinned">Cheese Tinned (50 gms)</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" id="qty" name="qty" required />
                            </td>
                            <%--<td>
                    <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>--%>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
            </div>
            <div class="mt-3">
                <%--<asp:GridView ID="GridViewExtraIssueDivers" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView>--%>
                <asp:GridView ID="GridViewExtraIssueDivers" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" OnRowEditing="GridView_RowEditing" OnRowUpdating="GridView_RowUpdating" OnRowCancelingEdit="GridView_RowCancelingEdit"
                    OnRowDeleting="GridView_RowDeleting" OnRowDataBound="GridView_RowDataBound" DataKeyNames="Id">
                    <Columns>
                        <asp:TemplateField HeaderText="Date">
                            <ItemTemplate>
                                <asp:Label ID="lblDate" runat="server" Text='<%# Eval("Date") %>'></asp:Label>
                            </ItemTemplate>
                            <%--   <EditItemTemplate>
                                <asp:TextBox ID="lblDate" runat="server" Text='<%# Bind("Date") %>'></asp:TextBox>
                            </EditItemTemplate>--%>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:Label ID="lblname" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="lblname" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rank">
                            <ItemTemplate>
                                <asp:Label ID="lblrank" runat="server" Text='<%# Eval("Rank") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtrank" runat="server" Text='<%# Bind("Rank") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="PNo">
                            <ItemTemplate>
                                <asp:Label ID="lblpno" runat="server" Text='<%# Eval("PNo") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtpno" runat="server" Text='<%# Bind("PNo") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Days">
                            <ItemTemplate>
                                <asp:Label ID="lbldays" runat="server" Text='<%# Eval("Days") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtdays" runat="server" Text='<%# Bind("Days") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ItemName">
                            <ItemTemplate>
                                <asp:Label ID="lblitemname" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlitemname" runat="server" CssClass="itemname"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty">
                            <ItemTemplate>
                                <asp:Label ID="lblqty" runat="server" Text='<%# Eval("Qty") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtqty" runat="server" Text='<%# Bind("Qty") %>'></asp:TextBox>
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
        </form>
    </div>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        $(document).ready(function () {
            fetchItems('');
        });
        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            var selectedDate = document.getElementById("dateVal").value;
            var selectedName = document.getElementById("nameVal").value;
            var selectedRank = document.getElementById("rank").value;
            var selectedPno = document.getElementById("pno").value;
            var selectedDays = document.getElementById("days").value;
            newRow.innerHTML = `
                        <td>
                            <input type="date" class="form-control" name="date" value="${selectedDate}" required disabled />
                        </td>
                         <td>
                             <input type="text" class="form-control" name="name" value="${selectedName}" required style="text-transform: capitalize;" disabled />
                         </td>
                         <td>
                             <select class="form-control rank" id="rank" name="rank" value="${selectedRank}" width="130px" required disabled>
                                  <option value="">Select</option>
                                  <option value="Sub Lieutenant" ${selectedRank === "Sub Lieutenant" ? "selected" : ""}>Sub Lieutenant</option>
                                  <option value="Lieutenant" ${selectedRank === "Lieutenant" ? "selected" : ""}>Lieutenant</option>
                                  <option value="Lieutenant Commander" ${selectedRank === "Lieutenant Commander" ? "selected" : ""}>Lieutenant Commander</option>
                                  <option value="Commander" ${selectedRank === "Commander" ? "selected" : ""}>Commander</option>
                                  <option value="Captain" ${selectedRank === "Captain" ? "selected" : ""}>Captain</option>
                                  <option value="Commodore" ${selectedRank === "Commodore" ? "selected" : ""}>Commodore</option>
                                  <option value="Rear Admiral" ${selectedRank === "Rear Admiral" ? "selected" : ""}>Rear Admiral</option>
                                  <option value="Vice Admiral" ${selectedRank === "Vice Admiral" ? "selected" : ""}>Vice Admiral</option>
                                  <option value="Admiral" ${selectedRank === "Admiral" ? "selected" : ""}>Admiral</option>
                                  <option value="Seaman 2nd Class" ${selectedRank === "Seaman 2nd Class" ? "selected" : ""}>Seaman 2nd Class</option>
                                  <option value="Seaman Ist Class" ${selectedRank === "Seaman Ist Class" ? "selected" : ""}>Seaman Ist Class</option>
                                  <option value="Leading Rate" ${selectedRank === "Leading Rate" ? "selected" : ""}>Leading</option>
                                  <option value="Petty Officer" ${selectedRank === "Petty Officer" ? "selected" : ""}>Petty Officer</option>
                                  <option value="Chief Petty Officer" ${selectedRank === "Chief Petty Officer" ? "selected" : ""}>Chief Petty Officer</option>
                                  <option value="Master Chief Petty Officer IInd Class" ${selectedRank === "Master Chief Petty Officer IInd Class" ? "selected" : ""}>Master Chief Petty Officer IInd Class</option>
                                  <option value="Master Chief Petty Officer Ist Class" ${selectedRank === "Master Chief Petty Officer Ist Class" ? "selected" : ""}>Master Chief Petty Officer Ist Class</option>
                             </select>
                         </td>
                         <td>
                             <input type="text" class="form-control pno" name="pno" value="${selectedPno}" required style="text-transform: capitalize;" disabled/>
                         </td>
                         <td>
                            <input type="text" class="form-control days" id="days" value="${selectedDays}" name="days" required disabled/>
                        </td>
                         <td>
                            <select class="form-control itemname" name="itemname" id="itemname_${rowSequence}" required>
                                <option value="">Select</option>
                            </select>
                        </td>
                        <td>
                            <input type="number" class="form-control" id="qty" name="qty" required />
                        </td>
                        <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
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

    </script>
</asp:Content>

