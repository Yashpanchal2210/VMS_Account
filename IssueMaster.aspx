<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="IssueMaster.aspx.cs" Inherits="VMS_1.IssueMaster" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .input-container {
            position: relative;
        

            .input-container input {
                padding-right: 30px; /* Adjust padding to accommodate the icon */
            }

            .input-container .fa-info-circle {
                position: absolute;
                right: 10px;
                top: 50%;
                transform: translateY(-50%);
            }
    </style>
    <div class="container">

        <h2 class="mt-4">Daily Issue Register</h2>

        <form id="issueForm" runat="server">
            <div class="mb-3">
                <%--<label for="userType">Select User Type:</label>--%>
                <select id="userType" name="userrole" class="form-control" onchange="selectCategory(this.value, '')" style="width: 150px;">
                    <option value="" selected>Select</option>
                    <option value="Wardroom">Wardroom</option>
                    <option value="Galley">Galley</option>
                </select>
                <span class="text-warning" id="selecttype">PLEASE SELECT TYPE FIRST</span>
            </div>
            <input type="hidden" id="ScalAmount_Val" />
            <input type="hidden" id="ItemCategory_Val" />
            <input type="hidden" id="entitledStrength" />
            <input type="hidden" id="EntitledStrength" name="EntitledStrength" />
            <input type="hidden" id="scaleAmount" />
            <input type="hidden" id="Selected_Category" name="Category" />

            <asp:HiddenField ID="DeleteStatus" runat="server" />

            <div class="table-responsive">
                <table class="table" id="issueTable">
                    <thead>
                        <tr>
                            <th class="heading">Date</th>
                            <th class="heading">Item Name</th>
                            <th class="heading">Denomination</th>
                            <th class="heading">Current Stock</th>
                            <th class="heading">Year Mark Stock</th>
                            <th class="heading">Daily Strength</th>
                            <th class="heading">Days</th>
                            <th class="heading">Entitled Strength</th>
                            <th class="heading">Qty Issued</th>
                            <th class="heading"></th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" class="form-control" name="date" /></td>
                            <td>
                                <select class="form-control js-states" id="DropDownList1" name="itemname" onchange="fetchBasicDenom(this.id)" width="130px">
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal" name="denoms" readonly />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomscurrentstock" name="denoms" readonly />
                            </td>
                             <td>
                                <input type="text" class="form-control" id="yearmarkstock" name="yearmarkstock" readonly />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="dailystr" name="dailystr" onchange="attachInputListeners(0)"  />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="days" name="days" onchange="attachInputListeners(0)" />
                            </td>
                            <td>
                                <div class="input-container">
                                    <input type="text" class="form-control" id="Strength" name="Strength" readonly />
                                    <i class="fas fa-info-circle" data-toggle="modal" data-target="#infoModal"></i>
                                </div>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="Qtyissued" /></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <%
                    if (IsUserInRoleRecepit("Store Keeper"))
                    {
                %>
                <button type="button" class="btn btn-success" data-toggle="modal" data-target="#infoModalIssue">
                    Submit
                </button>
                <%
                    }
                    else if (IsUserInRoleRecepit("Logistic Officer"))
                    {
                %>
                <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
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
            <div>
                <a class="btn" href="ExportIssueOfficerandSailor.aspx">Export Issue</a>
            </div>
            <div>

                <asp:GridView ID="GridViewIssue" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" DataKeyNames="Id"
                    OnRowEditing="GridViewIssue_RowEditing"
                    OnRowUpdating="GridViewIssue_RowUpdating"
                    OnRowCancelingEdit="GridViewIssue_RowCancelingEdit"
                    OnRowDeleting="GridViewIssue_RowDeleting"
                    OnRowDataBound="GridViewIssue_RowDataBound" AllowDeleting="true">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="Date" HeaderText="Date" ReadOnly="true" />
                        <asp:BoundField DataField="ItemName" HeaderText="ItemName" ReadOnly="true" />
                        <asp:TemplateField HeaderText="Qty Issued">
                            <ItemTemplate>
                                <asp:Label ID="lblQtyIssued" runat="server" Text='<%# Eval("QtyIssued") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtyIssued" runat="server" Text='<%# Bind("QtyIssued") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>                       
                        <asp:BoundField DataField="Denomination" HeaderText="Denomination" ReadOnly="true" />
                        <asp:TemplateField HeaderText="Strength">
                            <ItemTemplate>
                                <asp:Label ID="lblstrength" runat="server" Text='<%#Eval("EntitledStrength") %>'></asp:Label>
                            </ItemTemplate>                          
                        </asp:TemplateField>
                        <asp:CommandField HeaderText="Action" ShowDeleteButton="True" DeleteText="Delete Row" />
                        <asp:CommandField HeaderText="Action" ShowEditButton="True" EditText="Edit" />
                    </Columns>
                </asp:GridView>
            </div>


            <!-- Modal -->
            <div class="modal fade" id="entitledStrengthModal" tabindex="-1" role="dialog" aria-labelledby="entitledStrengthModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="entitledStrengthModalLabel">Entitled Strength</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">

                            <p>Quantity entitled for given strength:&nbsp<span class="font-weight-bolder" id="entitledStrengthValue"></span> </p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-labelledby="infoModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="infoModalLabel">Info</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Entitled Strength = ("Approximate Daily Strength" * "Number Of Days") 
                       
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="infoModalIssue" tabindex="-1" role="dialog" aria-labelledby="infoModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="infoModalIssueLabel">Daily Issue</h5>
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
    <script>
        $(document).ready(function () {
            $('#DropDownList1').select2({
                placeholder: 'Select an option',
                allowClear: true
            });
        });

        function selectCategory(val, row) {

            document.getElementById('selecttype').style.display = 'none';

            var categoryVal = "";
            if (val != "") {
                categoryVal = document.getElementById("Selected_Category").value;
                categoryVal = val;
            } else {
                categoryVal = document.getElementById("Selected_Category").value;
            }


            fetch('IssueMaster.aspx/GetInLiueItemsByCategory', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ Category: categoryVal })
            })
                .then(response => response.json())
                .then(data => {
                    const items = JSON.parse(data.d);

                    if (row == "" || row == null) {
                        const selectElement = document.getElementById('DropDownList1');
                        selectElement.innerHTML = '<option value="">Select</option>';

                        for (let i = 0; i < items.length; i += 2) {
                            const optionText = items[i];
                            const optionValue = items[i + 1];
                            if (optionText && optionValue) {
                                const option = document.createElement('option');
                                option.text = optionText;
                                option.value = optionValue;
                                selectElement.appendChild(option);
                            }
                        }
                        selectElement.disabled = false;
                    } else {
                        var rowId = rowSequence - 1;
                        const selectElement1 = document.getElementById('DropDownList1_' + rowId);
                        selectElement1.innerHTML = '<option value="">Select</option>';

                        for (let i = 0; i < items.length; i += 2) {
                            const optionText = items[i];
                            const optionValue = items[i + 1];
                            if (optionText && optionValue) {
                                const option = document.createElement('option');
                                option.text = optionText;
                                option.value = optionValue;
                                selectElement1.appendChild(option);
                            }
                        }
                        selectElement1.disabled = false;
                    }

                    // Enable the select element after populating it

                })
                .catch(error => {
                    console.error('Error fetching Items:', error);
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

            fetch('IssueMaster.aspx/GetItemDenom', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ItemVal: ItemValue })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {

                        var result = JSON.parse(data.d);
                        var denomination = result.Denomination;
                        var currentstock = result.CurrentStock;
                        var yearmarkstock = result.YearMarkStock;
                        var vegScale = result.VegScale;
                        var nonvegeScale = result.NonVegScale;

                        var sumofScale = vegScale + nonvegeScale;
                        document.getElementById("scaleAmount").value = sumofScale;

                        if (rowSequence > 1) {
                            var denomsDropdown = document.getElementById("denomsVal_" + part1);
                            denomsDropdown.value = denomination;
                            var CurrentStock = document.getElementById("denomscurrentstock_" + part1);
                            CurrentStock.value = currentstock;
                            var Yearmarkstock = document.getElementById("yearmarkstock_" + part1);
                            Yearmarkstock.value = yearmarkstock;
                        } else {
                            var denomsDropdown = document.getElementById("denomsVal");
                            denomsDropdown.value = denomination;
                            var CurrentStock = document.getElementById("denomscurrentstock");
                            CurrentStock.value = currentstock;
                            var Yearmarkstock = document.getElementById("yearmarkstock");
                            Yearmarkstock.value = yearmarkstock;
                        }
                    }
                })
                .catch(error => {
                    console.error('Error fetching basic denomination:', error);
                });
        }

        $(document).on('change', 'input[name="Strength"]', function () {
            var scaleAmount = parseFloat($('#scaleAmount').val()).toFixed(4);
            var strengthValue = parseFloat($(this).val());

            if (!isNaN(strengthValue)) {
                var entitledStrength = strengthValue * scaleAmount;
                $('#entitledStrengthValue').text(entitledStrength);
                $('#entitledStrengthModal').modal('show');
                //$(this).closest('tr').find('.EntitledStrength').val(entitledStrength);
            } else {
                $('#entitledStrengthValue').text("Please enter a valid strength value.");
                $('#entitledStrengthModal').modal('show');
                //$(this).closest('tr').find('.EntitledStrength').val('');
            }
        });

        document.querySelector('input[name="Strength"]').addEventListener('input', function () {

            var scaleAmount = $('#ScalAmount_Val').val();
            var strengthValue = parseFloat(document.querySelector('input[name="Strength"]').value);

            var strengthValue = parseFloat(this.value);

            if (!isNaN(strengthValue)) {
                var entitledStrength = strengthValue * scaleAmount;
                var id = 'EntitledStrength';
                $('#entitledStrength').val(entitledStrength);

                if (id != 'EntitledStrength' + '_' + rowSequence) {
                    document.getElementById('EntitledStrength').textContent = entitledStrength;
                    $('#EntitledStrength').val(entitledStrength);
                } else {
                    document.getElementById('EntitledStrength' + '_' + rowSequence).textContent = entitledStrength;
                }
            } else {
                document.getElementById('EntitledStrength').textContent = '';
            }
        });

        function setTheme(theme) {
            var gridView = document.getElementById("GridView3");
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

        // Global variable to keep track of sequence number
        var rowSequence = 1;

        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");

            freezDiv();

            var selectedDate = document.querySelector('input[type="date"][name="date"]').value;
            newRow.innerHTML = `
                            <input type="hidden" id="EntitledStrength_${rowSequence}" name="EntitledStrength" />
                            <td>
                                <input type="date" class="form-control" name="date" value="${selectedDate}" disabled required />
                            </td>
                            <td>
                                <select id="DropDownList1_${rowSequence}" class="form-control" onchange="fetchBasicDenom(this.id)" name="itemname" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal_${rowSequence}" name="denoms" readonly />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomscurrentstock_${rowSequence}" name="denoms" readonly />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="yearmarkstock_${rowSequence}" name="yearmarkstock" readonly />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="dailystr_${rowSequence}" onchange="attachInputListeners(${rowSequence})" name="dailystr"  />
                            </td>
                            <td>
                                <input type="text" class="form-control" id="days_${rowSequence}" onchange="attachInputListeners(${rowSequence})" name="days"  />
                            </td>
                            <td>
                                <input type="text" class="form-control strength-input" name="Strength" id="Strength_${rowSequence}" readonly  />
                            </td>
                            <td>
                                <input type="text" class="form-control" name="Qtyissued" id="Qtyissued_${rowSequence}"  />
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                            </td>`;

            tableBody.appendChild(newRow);
            var categoryVal = document.getElementById('userType').value;

            selectCategory(categoryVal, rowSequence);

            var strengthInput = document.getElementById("Strength_" + rowSequence);
            var strengthID = "Strength_" + rowSequence;
            strengthInput.addEventListener('input', function () {
                var scaleAmount = parseFloat($('#ScalAmount_Val').val());
                var strengthValue = parseFloat(this.value);

                if (!isNaN(strengthValue)) {
                    var entitledStrength = strengthValue * scaleAmount;
                    var entitledStrengthElement = document.getElementById(strengthID);
                    var int = rowSequence - 1;
                    document.getElementById('EntitledStrength_' + int).value = entitledStrength;
                } else {
                    document.getElementById('EntitledStrength_' + rowSequence).value = '';
                }
            });

            //fetchItemCategories();
            //fetchItemCategories(newRow);
            //fetchItemNames(getCategory, rowSequence);

            $("#itemcategory_" + rowSequence).append($('#itemcategory').html())
            var getCategory = $("#ItemCategory_Val").val();
            $('.itemcategory').val(getCategory);

            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        function checkReceivedFrom(selectElement) {
            var parentTd = selectElement.parentNode;
            var othersInput = parentTd.querySelector('input[name="otherReceivedFrom"]');
            if (selectElement.value === 'Others') {
                if (!othersInput) {
                    othersInput = document.createElement('input');
                    othersInput.type = 'text';
                    othersInput.name = 'otherReceivedFrom';
                    othersInput.className = 'form-control mt-2';
                    othersInput.placeholder = 'Specify other source';
                    parentTd.appendChild(othersInput);
                }
            } else {
                if (othersInput) {
                    parentTd.removeChild(othersInput);
                }
            }
        }

        var scaleAmountsByCategory = {};

        function populateDropDown(data) {
            var dropdown = $('#itemcategory');
            dropdown.empty();
            $.each(data, function (key, entry) {
                dropdown.append($('<option></option>').attr('value', entry.Value).text(entry.Text));
            });
        }
        var selectedCategoryValue = '';


        function freezDiv() {
            var type = document.getElementById("userType");
            type.disabled = true;
        }

        function showSuccessToast(message) {
            toastr.success(message, 'Success', {
                progressBar: true,
                positionClass: 'toast-bottom-right',
                timeOut: 3000
            });
        }

        function showErrorToast(message) {
            toastr.error(message, 'Error', {
                progressBar: true,
                positionClass: 'toast-bottom-right',
                timeOut: 3000
            });
        }
        //$('#dailystr, #days').on('input', function () {
        //    if (rowSequence == 1) {
        //        var input1 = $('#dailystr').val();
        //        var input2 = $('#days').val();

        //        // Example logic: concatenate the values
        //        var result = input1 * input2;

        //        // Display the result in the third textbox
        //        $('#Strength').val(result);
        //    }
        //    else {
        //        var input1 = parseFloat($('#dailystr_' + rowSequence).val());
        //        var input2 = parseFloat($('#days_' + rowSequence).val());

        //        // Make sure both values are numbers before performing multiplication
        //        if (!isNaN(input1) && !isNaN(input2)) {
        //            var result = input1 * input2;
        //            // Display the result in the Strength textbox of the current row
        //            $('#Strength_' + rowSequence).val(result);
        //        } else {
        //            // Clear the result if one of the inputs is invalid
        //            $('#Strength_' + rowSequence).val('');
        //        }
        //    }
        //});

        //$('#dailystr_' + rowSequence + ', #days_' + rowSequence).on('input', function () {
        //    debugger;
        //    var input1 = parseFloat($('#dailystr_' + rowSequence).val());
        //    var input2 = parseFloat($('#days_' + rowSequence).val());

        //    // Make sure both values are numbers before performing multiplication
        //    if (!isNaN(input1) && !isNaN(input2)) {
        //        var result = input1 * input2;
        //        // Display the result in the Strength textbox of the current row
        //        $('#Strength_' + rowSequence).val(result);
        //    } else {
        //        // Clear the result if one of the inputs is invalid
        //        $('#Strength_' + rowSequence).val('');
        //    }
        //});

        function attachInputListeners(rowSequence) {
            if (rowSequence == 0) {
                // For the first row
                $('#dailystr, #days').on('input', function () {
                    var input1 = parseFloat($('#dailystr').val());
                    var input2 = parseFloat($('#days').val());

                    // Ensure both values are numbers before performing multiplication
                    if (!isNaN(input1) && !isNaN(input2)) {
                        var result = input1 * input2;
                        $('#Strength').val(result);
                    } else {
                        $('#Strength').val('');
                    }
                });
            } else {
                // For dynamic rows (2nd row onwards)
                $('#dailystr_' + rowSequence + ', #days_' + rowSequence).on('input', function () {
                    var input1 = parseFloat($('#dailystr_' + rowSequence).val());
                    var input2 = parseFloat($('#days_' + rowSequence).val());

                    if (!isNaN(input1) && !isNaN(input2)) {
                        var result = input1 * input2;
                        $('#Strength_' + rowSequence).val(result);
                    } else {
                        $('#Strength_' + rowSequence).val('');
                    }
                });
            }
        }
    </script>
</asp:Content>
