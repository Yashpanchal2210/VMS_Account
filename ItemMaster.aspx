<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ItemMaster.aspx.cs" Inherits="VMS_1.ItemMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mb-4">Enter Scale of Items</h2>
        <p style="color:yellow; background-color:black; font-size:large;">This is a one-time activity to be updated only when there are revised orders concerning INBR 14. Items and scales can be added or updated exclusively with the permission of the Logistic Officer and the Commanding Officer, using an admin ID login.</p>

        <form id="itemMasterForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
                <asp:LinkButton ID="Receipt" runat="server" Text="Go to Receipt Module" CssClass="btn btn-info" PostBackUrl="~/ItemReceipt.aspx"></asp:LinkButton>
            </div>--%>

            <h3>Basic Items</h3>
            <div class="table-responsive">
                <input type="hidden" id="categoryVal" />
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Category</th>
                            <th class="heading">Basic Item</th>
                            <th class="heading">Denomination</th>
                            <th class="heading">Veg Scale</th>
                            <th class="heading">NonVeg Scale</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <%--<input type="number" class="form-control" name="officerScale" required min="0" step="0.001" />--%>
                                <select class="form-control" name="category" id="CategoryBasic" onchange="changeCategory(this.value)" required>
                                    <option value="">Select</option>
                                    <option value="Officer">Officer</option>
                                    <option value="Sailor">Sailor</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control" name="basicItem" id="basicnameVal" onchange="fetchBasicDenom()">
                                    <option value="">Select</option>
                                </select>
                                <%--<asp:DropDownList ID="basicItem" runat="server" CssClass="form-control" required>
                                    <asp:ListItem Text="Select" Value="" />
                                </asp:DropDownList>--%>
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal" name="denoms" readonly />
                            </td>
                            <td>
                                <input type="number" class="form-control" name="veg" required min="0" step="0.00000001" /></td>
                            <td>
                                <input type="number" class="form-control" name="nonveg" required min="0" step="0.00000001" /></td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <h3>In-lieu Items Details</h3>
            <div class="table-responsive">
                <table class="table" id="Table2">
                    <thead>
                        <tr>
                            <th class="heading">In-lieu Item</th>
                            <th class="heading">Category</th>
                            <th class="heading">Denomination</th>
                            <th class="heading">Veg Scale</th>
                            <th class="heading">NonVeg Scale</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody id="Tbody1" runat="server">
                        <tr>
                            <td>
                                <select class="form-control" name="inlieuItem" id="inlieuItemVal" onchange="changeInlieuItem(this.value)">
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="categoryIlue" id="categoryIlue" readonly /></td>
                            <td>
                                <input type="text" class="form-control" id="denoms" name="denoms" readonly />
                            </td>
                            <td>
                                <input type="number" class="form-control" name="vegscaleIlue" min="0" step="0.00000001" />
                            </td>
                            <td>
                                <input type="number" class="form-control" name="nonvegscaleIlue" min="0" step="0.00000001" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Submit Button -->
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
            <button type="button" class="btn btn-primary" onclick="addAlternativeItem()">Add Alternative Item</button>

            <!-- Alternative Item Fields Container -->
            <div id="alternateItemContainer"></div>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            <div class="mt-2">
                <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False"
                    OnRowEditing="GridView1_RowEditing" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting"
                    OnRowUpdating="GridView1_RowUpdating" DataKeyNames="InlIueId">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="InlIueId" HeaderText="InlIueId" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="BasicItem" HeaderText="Basic Item" ReadOnly="True" />
                        <asp:BoundField DataField="Category" HeaderText="Category" ReadOnly="True" />
                        <asp:BoundField DataField="Denomination" HeaderText="Denomination" ReadOnly="True" />
                        <asp:BoundField DataField="VegScale" HeaderText="Veg Scale" ReadOnly="True" />
                        <asp:BoundField DataField="NonVegScale" HeaderText="NonVeg Scale" ReadOnly="True" />
                        <asp:BoundField DataField="InLieuItem" HeaderText="NonVeg Scale" ReadOnly="True" />

                        <asp:TemplateField HeaderText="VegScale">
                            <ItemTemplate>
                                <asp:Label ID="lblVegScale" runat="server" Text='<%# Eval("InLieuItemVegScale") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtVegScale" runat="server" Text='<%# Bind("InLieuItemVegScale") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NonVeg Scale">
                            <ItemTemplate>
                                <asp:Label ID="lblNonVegScale" runat="server" Text='<%# Eval("InLieuItemNonVegScale") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNonVegScale" runat="server" Text='<%# Bind("InLieuItemNonVegScale") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="EditButton" runat="server" CommandName="Edit" Text="Edit"></asp:LinkButton>
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

        var rowSequence = 1;
        function addAlternativeItem() {
            if (rowSequence == 1) {
                var inlieuItemVal = document.getElementById("inlieuItemVal").value;
                var categoryIlue = document.getElementById("categoryIlue").value;
                var denoms = document.getElementById("denoms").value;
                var vegscaleIlue = document.getElementsByName("vegscaleIlue")[0].value;
                var nonvegscaleIlue = document.getElementsByName("nonvegscaleIlue")[0].value;

                if (inlieuItemVal.trim() === "" || categoryIlue.trim() === "" || denoms.trim() === "" || vegscaleIlue.trim() === "" || nonvegscaleIlue.trim() === "") {
                    alert("Please fill in all fields in the current row before adding a new row.");
                    return;
                }
            } else {
                var rowId = rowSequence - 1;
                var inlieuItemVal = document.getElementById("inlieuItemVal_" + rowId).value;
                var categoryIlue = document.getElementById("categoryIlue_" + rowId).value;
                var denoms = document.getElementById("denoms_" + rowId).value;
                var vegscaleIlue = document.getElementsByName("vegscaleIlue_" + rowId)[0].value;
                var nonvegscaleIlue = document.getElementsByName("nonvegscaleIlue_" + rowId)[0].value;

                if (inlieuItemVal.trim() === "" || categoryIlue.trim() === "" || denoms.trim() === "" || vegscaleIlue.trim() === "" || nonvegscaleIlue.trim() === "") {
                    alert("Please fill in all fields in the current row before adding a new row.");
                    return;
                }
            }
            

            var tableBody = document.getElementById("MainContent_Tbody1");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td>
                                    <select class="form-control" name="inlieuItem" id="inlieuItemVal_${rowSequence}" onchange="changeInlieuItem(this.value)">
                                        <option value="">Select</option>
                                    </select>
                                </td>
                                <td>
                                    <input type="text" class="form-control" name="categoryIlue" id="categoryIlue_${rowSequence}" readonly /></td>
                                <td>
                                    <input type="text" class="form-control" id="denoms_${rowSequence}" name="denoms" readonly/>
                                </td>
                                <td>
                                    <input type="number" class="form-control" name="vegscaleIlue" id="vegscaleIlue_${rowSequence}" min="0" step="0.00000001" />
                                </td>
                                <td>
                                    <input type="number" class="form-control" name="nonvegscaleIlue" id="nonvegscaleIlue_${rowSequence}" min="0" step="0.00000001" />
                                </td>
                                <td>
                                    <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
            fetchInLieuItemsForRow(rowSequence, '');

            var hiddenCategory = document.getElementById("categoryVal");
            var caregoryField = document.getElementById("categoryIlue_" + rowSequence);
            caregoryField.value = hiddenCategory.value;


            rowSequence++;
        }

        function fetchBasicDenom() {
            var basicItemValue = document.getElementById('basicnameVal').value;

            fetch('ItemMaster.aspx/GetBasicDenom', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ basicItem: basicItemValue })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {
                        var denomsDropdown = document.getElementById("denomsVal");
                        denomsDropdown.value = data.d;
                    }
                    fetchInLieuItemsForRow('', '');
                })
                .catch(error => {
                    console.error('Error fetching basic denomination:', error);
                });
        }

        function fetchInLieuDenom(val) {
            fetch('ItemMaster.aspx/GetDenominationForInLieuItem', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ inlieuItem: val })
            })
                .then(response => response.json())
                .then(data => {
                    var idVal = "";
                    if (Id == null || Id == '') {
                        inlieuItemDropdown = document.getElementById(`inlieuItemVal`);
                        inlieuItemDropdown.innerHTML = '<option value="">Select</option>';
                    } else {
                        inlieuItemDropdown = document.getElementById(`inlieuItemVal_${rowSequence}`);
                        inlieuItemDropdown.innerHTML = '<option value="">Select</option>';
                    }

                    if (data.d && Array.isArray(data.d)) {
                        data.d.forEach(function (item) {
                            var option = document.createElement("option");
                            option.value = item;
                            option.text = item;
                            inlieuItemDropdown.add(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching in-lieu items:', error);
                });
        }

        function changeCategory(val) {
            var hiddenCategory = document.getElementById("categoryVal");
            hiddenCategory.value = val;
            var caregoryField = document.getElementById("categoryIlue");
            caregoryField.value = val;

            document.getElementById(`inlieuItemVal`).value = "";
            if (rowSequence > 1) {
                document.getElementById(`inlieuItemVal_` + rowSequence).value = "";
            }

            document.getElementById("denomsVal").value = "";

            var basicVal = document.getElementById("basicnameVal");

            fetch('ItemMaster.aspx/GetCategoryWiseDataItems', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ basicItem: val })
            })
                .then(response => response.json())
                .then(data => {
                    var inlieuItemDropdown = document.getElementById(`basicnameVal`);
                    inlieuItemDropdown.innerHTML = '<option value="">Select</option>';

                    if (data.d && Array.isArray(data.d)) {
                        data.d.forEach(function (item) {
                            var option = document.createElement("option");
                            option.value = item;
                            option.text = item;
                            inlieuItemDropdown.add(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching in-lieu items:', error);
                });

        }

        function fetchInLieuItemsForRow(rowSequence, idValue) {

            var basicItemValue = "";
            var CategoryValue = "";


            if (idValue == "" || idValue == null) {
                basicItemValue = document.getElementById('basicnameVal').value;
                CategoryValue = document.getElementById("CategoryBasic").value;
            } else {
                basicItemValue = idValue;
            }

            fetch('ItemMaster.aspx/GetInLieuItems', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ basicItem: basicItemValue, categoryVal: CategoryValue })
            })
                .then(response => response.json())
                .then(data => {
                    var inlieuItemDropdown = "";
                    if (rowSequence == null || rowSequence == '') {
                        inlieuItemDropdown = document.getElementById(`inlieuItemVal`);
                        inlieuItemDropdown.innerHTML = '<option value="">Select</option>';
                    } else {
                        inlieuItemDropdown = document.getElementById(`inlieuItemVal_${rowSequence}`);
                        inlieuItemDropdown.innerHTML = '<option value="">Select</option>';
                    }

                    if (data.d && Array.isArray(data.d)) {
                        data.d.forEach(function (item) {
                            var option = document.createElement("option");
                            option.value = item;
                            option.text = item;
                            inlieuItemDropdown.add(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching in-lieu items:', error);
                });
        }

        function onRowEdit(rowIndex) {
            var idValue = document.getElementById('GridView1').rows[rowIndex + 1].querySelector("[id$='hfId']").value;
            fetchInLieuItemsForRow(rowIndex, idValue);
        }



        function changeInlieuItem(inlieuItems) {
            fetch('ItemMaster.aspx/GetInLieuBasicDenom', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ basicItem: inlieuItems })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {
                        var InLieudenomsDropdown;
                        if (rowSequence == 1) {
                            InLieudenomsDropdown = document.getElementById("denoms");
                            InLieudenomsDropdown.value = data.d;
                        } else {
                            var valueRow = rowSequence - 1;
                            InLieudenomsDropdown = document.getElementById("denoms_" + valueRow);
                            InLieudenomsDropdown.value = data.d;
                        }
                    }
                })
                .catch(error => {
                    console.error('Error fetching in-lieu items:', error);
                });
        }


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

        function deleteRow(btn) {
            rowSequence - 1;
            var row = btn.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }
    </script>
</asp:Content>
