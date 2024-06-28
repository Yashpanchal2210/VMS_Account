<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Strength.aspx.cs" Inherits="VMS_1.Strength" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Strength Module</h2>
        <form id="strengthForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
            </div>--%>
            <div class="table-responsive">
                <table class="table" id="myTable" style="width: 1440px">
                    <thead>
                        <tr>
                            <th class="heading date" style="width: 123px"></th>
                            <th class="heading veg-officer" colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Officer</th>
                            <th class="heading rik-officer" colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Officer RIK</th>
                            <th class="heading veg-staff" colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Sailor</th>
                            <th class="heading veg-staff" colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Sailor RIK</th>
                            <th class="heading veg-staff" colspan="2">&nbsp;Non Entitled Officer</th>
                            <th class="heading veg-staff" colspan="2">&nbsp;Non Entitled Sailor</th>
                            <th class="heading veg-staff" colspan="2">&nbsp;Civilian</th>
                        </tr>
                        <tr>
                            <th class="heading date" style="width: 123px">Date <i class="fa fa-info-circle" data-toggle="modal" data-target="#exampleModal"></i></th>
                            <th class="heading veg-officer" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading non-veg-officer" style="width: 123px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading rik-officer" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading rik-officer" style="width: 123px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading veg-staff" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading non-veg-staff" style="width: 123px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading rik-staff" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading rik-staff" style="width: 123px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading non-entitled-officer" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading non-entitled-officer" style="width: 147px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading non-entitled-staff" style="width: 124px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading non-entitled-staff" style="width: 124px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading civilian" style="width: 124px"></th>
                            <th style="width: 124px">&nbsp;&nbsp; Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td style="width: 123px">
                                <input type="date" class="form-control" id="dateVal" name="date" style="width: 98%" onchange="fetchDate(this.id)" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="VegOfficer" min="0" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="NonVegOfficer" min="0" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="VegrikOfficer" min="0" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="NonVegRikOfficer" min="0" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="vegSailor" min="0" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="nonVegSailor" min="0" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="VegSailorRik" min="0" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="NonVegSailorRik" min="0" /></td>
                            <td style="width: 123px">
                                <input type="text" class="form-control" name="VegNonEntitledOfficer" min="0" /></td>
                            <td style="width: 147px">
                                <input type="text" class="form-control" name="NonVegNonEntitledOfficer" min="0" /></td>
                            <td style="width: 124px">
                                <input type="text" class="form-control" name="VegNonEntitledSailor" min="0" /></td>
                            <td style="width: 124px">
                                <input type="text" class="form-control" name="NonVegNonEntitledSailor" min="0" /></td>
                            <td style="width: 124px">
                                <input type="text" class="form-control" name="Civilian" min="0" style="width: 96%" /></td>
                            <td style="width: 124px">
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </div>
            <div style="color: red; background-color: yellow; padding: 10px; text-align: center; font-weight: bold;">
                Data entered for the same date will be overwritten.
            </div>

            <div class="text-center">
                <br />
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />

            </div>
            <div>
                <h2 class="mt-4">Strength Data</h2>

            </div>
            <%--  <div class="form-group">
                <label for="monthYear">Select Month and Year:</label>
                <input type="month" id="monthYear" name="monthYear" class="form-control" onchange="fetchFilteredData()" /><br />
                &nbsp;
            </div>--%>
            <div class="table-responsive">
                <asp:TextBox ID="txtDate" type="month" runat="server" CssClass="form-control" placeholder="Enter date (MM/DD/YYYY)"></asp:TextBox>
                <asp:Button ID="btnFilter" runat="server" CssClass="btn btn-primary" Text="Filter" OnClick="btnFilter_Click" />

                <asp:GridView ID="GridViewStrength" runat="server" CssClass="table table-bordered table-striped"
                    AutoGenerateColumns="False" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" Visible="false" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="dates" HeaderText="Date" ReadOnly="True" />
                        <asp:BoundField DataField="vegOfficers" HeaderText="vegOfficers" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegOfficers" HeaderText="nonVegOfficers" ReadOnly="True" />
                        <asp:BoundField DataField="vegrikOfficers" HeaderText="vegrikOfficers" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegrikOfficers" HeaderText="nonVegrikOfficers" ReadOnly="True" />
                        <asp:BoundField DataField="vegSailor" HeaderText="vegSailor" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegSailor" HeaderText="nonVegSailor" ReadOnly="True" />
                        <asp:BoundField DataField="vegSailorRik" HeaderText="vegSailorRik" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegSailorRik" HeaderText="nonVegSailorRik" ReadOnly="True" />
                        <asp:BoundField DataField="vegNonEntitledOfficer" HeaderText="vegNonEntitledOfficer" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegNonEntitledOfficer" HeaderText="nonVegNonEntitledOfficer" ReadOnly="True" />
                        <asp:BoundField DataField="vegNonEntitledSailor" HeaderText="vegNonEntitledSailor" ReadOnly="True" />
                        <asp:BoundField DataField="NonVegNonEntitledSailor" HeaderText="NonVegNonEntitledSailor" ReadOnly="True" />
                        <asp:BoundField DataField="civilians" HeaderText="civilians" ReadOnly="True" />
                        <asp:BoundField DataField="IsApproved" HeaderText="Status" ReadOnly="True" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnDelete" runat="server" Text="Reject" CssClass="btn btn-danger"
                                    CommandName="DeleteRecord" CommandArgument='<%# Eval("Id") %>' OnClick="btnAction_Click"
                                    Visible='<%# IsRegulatingOfficer() %>' />
                                <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-success"
                                    CommandName="ApproveRecord" CommandArgument='<%# Eval("Id") %>' OnClick="btnAction_Click"
                                    Visible='<%# IsRegulatingOfficer() %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </div>

            <div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalLabel">Strength</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Data entered for the same date will be overwritten.
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function setTheme(theme) {
            var gridView = document.getElementById("GridView2");

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
            newRow.innerHTML = `<td style="width: 123px">
                                    <input type="date" class="form-control" name="date" id="dateVal_${rowSequence}" required style="width: 98%" onchange="fetchDate(this.id)" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="VegOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="NonVegOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="VegrikOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="NonVegRikOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="vegSailor" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="nonVegSailor" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="VegSailorRik" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="NonVegSailorRik" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="VegNonEntitledOfficer" required min="0" /></td>
                                <td style="width: 147px">
                                    <input type="text" class="form-control" name="NonVegNonEntitledOfficer" required min="0" /></td>
                                <td style="width: 124px">
                                    <input type="text" class="form-control" name="VegNonEntitledSailor" required min="0" /></td>
                                <td style="width: 124px">
                                    <input type="text" class="form-control" name="NonVegNonEntitledSailor" required min="0" /></td>
                                <td style="width: 124px">
                                    <input type="text" class="form-control" name="Civilian" required min="0" style="width: 96%" /></td>
                                <td style="width: 124px">
                                    <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
            rowSequence++;
        }

        function deleteRow(btn) {
            var row = btn.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }



    </script>
</asp:Content>
