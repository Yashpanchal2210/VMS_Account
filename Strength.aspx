<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Strength.aspx.cs" Inherits="VMS_1.Strength" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Strength Module</h2>
        <form id="strengthForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
            </div>--%>
            <div class="table-responsive">
                <table class="table" id="myTable" style="width: 1600px;">
                    <thead>
                        <tr>
                            <th class="heading date" style="width: 123px"></th>
                            <th class="heading veg-officer" colspan="3">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Officer</th>
                            <th class="heading rik-officer" colspan="3">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Officer RIK</th>
                            <th class="heading veg-staff" colspan="3">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Sailor</th>
                            <th class="heading veg-staff" colspan="3">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Sailor RIK</th>
                            <th class="heading veg-staff" colspan="3">&nbsp;Non Entitled Officer</th>
                            <th class="heading veg-staff" colspan="3">&nbsp;Non Entitled Sailor</th>
                            <th class="heading veg-staff" colspan="3">&nbsp;Civilian</th>
                        </tr>
                        <tr>
                            <th class="heading date" style="width: 123px">Date <i class="fa fa-info-circle" data-toggle="modal" data-target="#exampleModal"></i></th>
                            <th class="heading veg-officer" style="width: 125px">&nbsp;&nbsp;&nbsp;&nbsp; Std</th>
                            <th class="heading veg-officer" style="width: 125px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading non-veg-officer" style="width: 125px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading veg-officer" style="width: 125px">&nbsp;&nbsp;&nbsp;&nbsp; Std</th>
                            <th class="heading rik-officer" style="width: 125px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading rik-officer" style="width: 125px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading veg-officer" style="width: 125px">&nbsp;&nbsp;&nbsp;&nbsp; Std</th>
                            <th class="heading veg-staff" style="width: 125px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading non-veg-staff" style="width: 125px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading veg-officer" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Std</th>
                            <th class="heading rik-staff" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading rik-staff" style="width: 123px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading veg-officer" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Std</th>
                            <th class="heading non-entitled-officer" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading non-entitled-officer" style="width: 147px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading veg-officer" style="width: 123px">&nbsp;&nbsp;&nbsp;&nbsp; Std</th>
                            <th class="heading non-entitled-staff" style="width: 124px">&nbsp;&nbsp;&nbsp;&nbsp; Veg</th>
                            <th class="heading non-entitled-staff" style="width: 124px">&nbsp;&nbsp; NonVeg</th>
                            <th class="heading civilian" style="width: 124px"></th>
                            <th style="width: 124px">&nbsp;&nbsp; Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td style="width: 123px"><input type="date" class="form-control" id="dateVal" name="date" style="width: 98%" onchange="fetchDate(this.id)" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="StdOfficer" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="VegOfficer" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="NonVegOfficer" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="StdrikOfficer" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="VegrikOfficer" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="NonVegRikOfficer" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="StdSailor" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="vegSailor" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="nonVegSailor" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="StdSailorRik" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="VegSailorRik" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="NonVegSailorRik" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="StdNonEntitledOfficer" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="VegNonEntitledOfficer" min="0" /></td>
                            <td style="width: 147px"><input type="text" class="form-control" name="NonVegNonEntitledOfficer" min="0" /></td>
                            <td style="width: 125px"><input type="text" class="form-control" name="StdNonEntitledSailor" min="0" /></td>
                            <td style="width: 124px"><input type="text" class="form-control" name="VegNonEntitledSailor" min="0" /></td>
                            <td style="width: 124px"><input type="text" class="form-control" name="NonVegNonEntitledSailor" min="0" /></td>
                            <td style="width: 124px"><input type="text" class="form-control" name="Civilian" min="0" style="width: 96%" /></td>
                            <td style="width: 124px">
                                <%--<button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>--%>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div>
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
                <h2 class="mt-4">Approved Strength Data</h2>

            </div>
            <div class="table-responsive">
                <div style="width: 50%">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <asp:TextBox ID="txtDate" type="month" runat="server" CssClass="form-control" placeholder="Enter date (MM/DD/YYYY)"></asp:TextBox></td>
                            <td>
                                <asp:Button ID="btnFilter" runat="server" CssClass="btn btn-primary" Text="Filter" OnClick="btnFilter_Click" />
                            </td>
                            <td>
                                <asp:Label ID="lblStatus" runat="server" Text="" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:GridView ID="GridViewStrength" runat="server" CssClass="table table-bordered table-striped"
                    AutoGenerateColumns="False" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" Visible="false" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="dates" HeaderText="Date" ReadOnly="True" />

                        <asp:BoundField DataField="StdOfficers" HeaderText="Std Officers" ReadOnly="True" />
                        <asp:BoundField DataField="vegOfficers" HeaderText="Veg Officers" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegOfficers" HeaderText="Non Veg Officers" ReadOnly="True" />

                         <asp:BoundField DataField="StdrikOfficers" HeaderText="Std Vegrik Officers" ReadOnly="True" />
                        <asp:BoundField DataField="vegrikOfficers" HeaderText="Vegrik Officers" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegrikOfficers" HeaderText="NonVegrik Officers" ReadOnly="True" />

                         <asp:BoundField DataField="StdSailor" HeaderText="Std Sailor" ReadOnly="True" />
                        <asp:BoundField DataField="vegSailor" HeaderText="Veg Sailor" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegSailor" HeaderText="Non Veg Sailor" ReadOnly="True" />

                        <asp:BoundField DataField="StdSailorRik" HeaderText="Std Sailor Rik" ReadOnly="True" />
                        <asp:BoundField DataField="vegSailorRik" HeaderText="Veg Sailor Rik" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegSailorRik" HeaderText="Non Veg Sailor Rik" ReadOnly="True" />

                         <asp:BoundField DataField="StdNonEntitledOfficer" HeaderText="Std Non Entitled Officer" ReadOnly="True" />
                        <asp:BoundField DataField="vegNonEntitledOfficer" HeaderText="Veg Non Entitled Officer" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegNonEntitledOfficer" HeaderText="Non Veg Non Entitled Officer" ReadOnly="True" />

                        <asp:BoundField DataField="StdNonEntitledSailor" HeaderText="Std Non Entitled Sailor" ReadOnly="True" />
                        <asp:BoundField DataField="vegNonEntitledSailor" HeaderText="Veg Non Entitled Sailor" ReadOnly="True" />
                        <asp:BoundField DataField="NonVegNonEntitledSailor" HeaderText="Non Veg Non Entitled Sailor" ReadOnly="True" />

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

            <div>
                <h2 class="mt-4">
                    <asp:Label ID="lblUnapproved" runat="server" Text="Unapproved Strength Data" Visible="false"></asp:Label>
                </h2>
            </div>
            <div class="table-responsive">
                <asp:GridView ID="gvUnapprovedSt" runat="server" CssClass="table table-bordered table-striped"
                    AutoGenerateColumns="False" DataKeyNames="Id">
                    <Columns>
                                                <asp:BoundField DataField="Id" Visible="false" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="dates" HeaderText="Date" ReadOnly="True" />

                        <asp:BoundField DataField="StdOfficers" HeaderText="Std Officers" ReadOnly="True" />
                        <asp:BoundField DataField="vegOfficers" HeaderText="Veg Officers" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegOfficers" HeaderText="Non Veg Officers" ReadOnly="True" />

                         <asp:BoundField DataField="StdrikOfficers" HeaderText="Std Vegrik Officers" ReadOnly="True" />
                        <asp:BoundField DataField="vegrikOfficers" HeaderText="Vegrik Officers" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegrikOfficers" HeaderText="NonVegrik Officers" ReadOnly="True" />

                         <asp:BoundField DataField="StdSailor" HeaderText="Std Sailor" ReadOnly="True" />
                        <asp:BoundField DataField="vegSailor" HeaderText="Veg Sailor" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegSailor" HeaderText="Non Veg Sailor" ReadOnly="True" />

                        <asp:BoundField DataField="StdSailorRik" HeaderText="Std Sailor Rik" ReadOnly="True" />
                        <asp:BoundField DataField="vegSailorRik" HeaderText="Veg Sailor Rik" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegSailorRik" HeaderText="Non Veg Sailor Rik" ReadOnly="True" />

                         <asp:BoundField DataField="StdNonEntitledOfficer" HeaderText="Std Non Entitled Officer" ReadOnly="True" />
                        <asp:BoundField DataField="vegNonEntitledOfficer" HeaderText="Veg Non Entitled Officer" ReadOnly="True" />
                        <asp:BoundField DataField="nonVegNonEntitledOfficer" HeaderText="Non Veg Non Entitled Officer" ReadOnly="True" />

                        <asp:BoundField DataField="StdNonEntitledSailor" HeaderText="Std Non Entitled Sailor" ReadOnly="True" />
                        <asp:BoundField DataField="vegNonEntitledSailor" HeaderText="Veg Non Entitled Sailor" ReadOnly="True" />
                        <asp:BoundField DataField="NonVegNonEntitledSailor" HeaderText="Non Veg Non Entitled Sailor" ReadOnly="True" />

                        <asp:BoundField DataField="civilians" HeaderText="civilians" ReadOnly="True" />
                        <asp:BoundField DataField="IsApproved" HeaderText="Status" ReadOnly="True" />

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
                                    <input type="text" class="form-control" name="StdOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="VegOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="NonVegOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="StdrikOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="VegrikOfficer" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="NonVegRikOfficer" required min="0" /></td>
                                 <td style="width: 123px">
                                    <input type="text" class="form-control" name="StdSailor" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="vegSailor" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="nonVegSailor" required min="0" /></td>
                                <td style="width: 125px"><input type="text" class="form-control" name="StdSailorRik" min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="VegSailorRik" required min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="NonVegSailorRik" required min="0" /></td>
                                <td style="width: 125px"><input type="text" class="form-control" name="StdNonEntitledOfficer" min="0" /></td>
                                <td style="width: 123px">
                                    <input type="text" class="form-control" name="VegNonEntitledOfficer" required min="0" /></td>
                                <td style="width: 147px">
                                    <input type="text" class="form-control" name="NonVegNonEntitledOfficer" required min="0" /></td>
                                <td style="width: 125px"><input type="text" class="form-control" name="StdNonEntitledSailor" min="0" /></td>
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
