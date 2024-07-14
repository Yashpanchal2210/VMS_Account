﻿<%@ Page Title="Dashboard" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Dashboard.aspx.cs" Inherits="VMS_1.Dashboard" %>

<%@ Import Namespace="System.Web.Security" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="wwwroot/assets/css/dashlite.css" />
    <link id="skin-default" rel="stylesheet" href="wwwroot/assets/css/theme.css" />

    <style>
        .issue-row {
            color: red;
        }

        .row-align {
            display: flex;
            align-items: center;
        }

        .form-control {
            flex: 1;
            margin-right: 10px;
        }

            .form-control:last-child {
                margin-right: 0;
            }
    </style>
    <form id="form1" runat="server">
        <div class="container">
            <div class="nk-block">
                <div class="row g-gs">
                    <div class="col-lg-12">
                        <div class="card card-bordered h-100">
                            <div class="card-inner">
                                <div class="card-title-group align-start mb-3">
                                    <div class="card-title">
                                        <h6 class="title">Status</h6>
                                    </div>
                                </div>
                                <div class="row row-align col-md-6">
                                    <input type="month" class="form-control col-md-4" id="vmsDate" />
                                    <button class="btn btn-primary form-control col-md-2" onclick="getVMSStatus()">Check</button>
                                </div>
                                <div class="nk-order-ovwg mt-5">
                                    <div class="row g-4 align-end">
                                        <div class="col-xxl-8">
                                            <div class="nk-order-ovwg-ck">
                                                <div class="row row-align col-md-12">
                                                    <p style="" class="col-md-4 align-content-around">Store Keeper</p>
                                                    <p style="text-align: center;" class="col-md-4">Logistic Officer</p>
                                                    <p style="text-align: right;" class="col-md-4">Commanding Officer</p>
                                                    <p style=""></p>
                                                </div>
                                                <div class="progress">
                                                    <div class="progress-bar bg-info" role="progressbar" aria-valuemin="0" aria-valuemax="100"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="container">
            <div class="nk-block">
                <div class="row g-gs">
                    <div class="col-lg-12">
                        <div class="card card-bordered h-100">
                            <div class="card-inner">
                                <div class="card-title-group align-start mb-3">
                                    <div class="card-title">
                                        <h6 class="title">Present Stock Status</h6>
                                        <%--<p>Ration received from BVY in last 10 days</p>--%>
                                    </div>
                                </div>
                                <!-- .card-title-group -->
                                <div class="nk-order-ovwg">
                                    <div class="row g-4 align-end">
                                        <div class="col-xxl-8">
                                            <div class="nk-order-ovwg-ck">
                                                <div style="width: 100%; overflow-x: auto;">
                                                    <canvas class="order-overview-chart" id="orderOverviewPresent"></canvas>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- .col -->
                                    </div>
                                </div>
                                <!-- .nk-order-ovwg -->
                            </div>
                            <!-- .card-inner -->
                        </div>
                        <!-- .card -->
                    </div>
                    <div class="col-lg-8">
                        <div class="card card-bordered h-100">
                            <div class="card-inner">
                                <div class="card-title-group align-start mb-3">
                                    <div class="card-title">
                                        <h6 class="title">Issue Status</h6>
                                        <p>Ration Issue in last 10 days</p>
                                    </div>
                                </div>
                                <!-- .card-title-group -->
                                <div class="nk-order-ovwg">
                                    <div class="row g-4 align-end">
                                        <div class="col-xxl-8">
                                            <div class="nk-order-ovwg-ck">
                                                <canvas class="order-overview-chart" id="orderOverviewIssue" width="891" height="225"></canvas>
                                            </div>
                                        </div>
                                        <!-- .col -->
                                    </div>
                                </div>
                                <!-- .nk-order-ovwg -->
                            </div>
                            <!-- .card-inner -->
                        </div>
                        <!-- .card -->
                    </div>
                    <!-- .col Reference -->
                    <div class="col-lg-4">
                        <div class="card card-bordered h-100">
                            <div class="card-inner-group">
                                <div class="card-inner card-inner-md">
                                    <div class="card-title-group">
                                        <div class="card-title">
                                            <h6 class="title">Reference</h6>
                                        </div>
                                        <div class="card-tools mr-n1">
                                        </div>
                                    </div>
                                </div>
                                <!-- .card-inner -->
                                <div class="card-inner">
                                    <div class="nk-wg-action">
                                        <div class="nk-wg-action-content">
                                            <p class="title"><a href="wwwroot/PDFFile/Revised INBR 14.pdf" download>Recent Policy Letter</a></p>
                                        </div>
                                    </div>
                                </div>
                                <!-- .card-inner -->
                                <div class="card-inner">
                                    <div class="nk-wg-action">
                                        <div class="nk-wg-action-content">
                                            <p class="title"><a href="wwwroot/PDFFile/Revised INBR 14.pdf">Book of Reference</a></p>
                                        </div>
                                    </div>
                                </div>

                                <!-- .card-inner -->
                            </div>
                            <!-- .card-inner-group -->
                        </div>
                        <!-- .card -->
                    </div>
                    <!-- .col RECEIPT Status -->
                    <div class="col-lg-12">
                        <div class="card card-bordered h-100">
                            <div class="card-inner">
                                <div class="card-title-group align-start mb-3">
                                    <div class="card-title">
                                        <h6 class="title">Receipt Status</h6>
                                        <p>Ration received from BVY in last 10 days</p>
                                    </div>
                                </div>
                                <!-- .card-title-group -->
                                <div class="nk-order-ovwg">
                                    <div class="row g-4 align-end">
                                        <div class="col-xxl-8">
                                            <div class="nk-order-ovwg-ck">
                                                <canvas class="order-overview-chart" id="orderOverviewReceipt"></canvas>
                                            </div>
                                        </div>
                                        <!-- .col -->
                                    </div>
                                </div>
                                <!-- .nk-order-ovwg -->
                            </div>
                            <!-- .card-inner -->
                        </div>
                        <!-- .card -->
                    </div>



                    <!-- .card Strength -->
                    <div class="col-lg-12 mb-5">
                        <div class="card card-bordered h-100">
                            <div class="card-inner">
                                <div class="card-title-group align-start mb-3">
                                    <div class="card-title">
                                        <h6 class="title">Daily Strength</h6>
                                    </div>
                                </div>
                                <!-- .card-title-group -->
                                <div class="nk-order-ovwg">
                                    <div class="row g-4 align-end">
                                        <div class="col-xxl-8">
                                            <div class="nk-order-ovwg-ck">
                                                <div class="chartjs-size-monitor">
                                                    <div class="chartjs-size-monitor-expand">
                                                        <div class=""></div>
                                                    </div>
                                                    <div class="chartjs-size-monitor-shrink">
                                                        <div class=""></div>
                                                    </div>
                                                </div>
                                                <canvas class="order-overview-chart chartjs-render-monitor" id="orderOverviewStrength" width="891" height="225" style="display: block; height: 180px; width: 713px;"></canvas>
                                            </div>
                                        </div>
                                        <!-- .col -->
                                    </div>
                                </div>
                                <!-- .nk-order-ovwg -->
                            </div>
                            <!-- .card-inner -->
                        </div>
                        <!-- .card -->
                    </div>
                </div>

                <div class="modal" id="myModal">
                    <div class="modal-dialog modal-dialog-scrollable">
                        <div class="modal-content">

                            <!-- Modal Header -->
                            <div class="modal-header">
                                <h2 class="modal-title">Recent Policy Letter</h2>
                                <button type="button" class="close" data-dismiss="modal">×</button>
                            </div>

                            <!-- Modal body -->
                            <div class="modal-body">
                                <h3>Some text to enable scrolling..</h3>
                                <p>Some text to enable scrolling.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p>
                                <p>Some text to enable scrolling.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p>

                                <p>Some text to enable scrolling.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p>
                            </div>

                            <!-- Modal footer -->
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                            </div>

                        </div>
                    </div>
                </div>

                <h2>Month Stock</h2>
                <div class="form-group row">
                    <label class="col-sm-2 col-form-label" for="ddlMonths">Select Month:</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlMonths" runat="server" CssClass="form-control">
                            <asp:ListItem Text="January" Value="1"></asp:ListItem>
                            <asp:ListItem Text="February" Value="2"></asp:ListItem>
                            <asp:ListItem Text="March" Value="3"></asp:ListItem>
                            <asp:ListItem Text="April" Value="4"></asp:ListItem>
                            <asp:ListItem Text="May" Value="5"></asp:ListItem>
                            <asp:ListItem Text="June" Value="6"></asp:ListItem>
                            <asp:ListItem Text="July" Value="7"></asp:ListItem>
                            <asp:ListItem Text="August" Value="8"></asp:ListItem>
                            <asp:ListItem Text="September" Value="9"></asp:ListItem>
                            <asp:ListItem Text="October" Value="10"></asp:ListItem>
                            <asp:ListItem Text="November" Value="11"></asp:ListItem>
                            <asp:ListItem Text="December" Value="12"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="Button1" runat="server" Text="Filter" CssClass="btn btn-primary" OnClick="btnFilter_Click" />
                    </div>
                </div>
                <br />
                <asp:GridView ID="GridViewMonthStock" runat="server" CssClass="table table-bordered table-striped" OnRowDataBound="GridViewMonthStock_RowDataBound">
                </asp:GridView>
            </div>
        </div>
        <div class="container">
            <%--<h2>Page 2 To 7</h2>--%>
            <%-- <asp:GridView ID="GridViewP2to7" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>--%>
            <%--<div style="height: 355px;">
                <canvas id="myPieChartP2to7" width="60" height="60"></canvas>
                <asp:HiddenField ID="hfChartDataPage2to7" runat="server" />
            </div>--%>
            <%--<div style="height: 355px;">
                <canvas id="myPieChartP2to7" width="60" height="60"></canvas>
                <asp:HiddenField ID="HiddenField1" runat="server" />
            </div>--%>
        </div>

        <%--<div class="container">
            <h2>Present Stock</h2>
            <asp:GridView ID="GridViewPresentStock" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
            <asp:Button ID="ExportPresentStockButton" runat="server" Text="Export Present Stock to Excel" OnClick="ExportPresentStockButton_Click" CssClass="btn btn-primary" />
        </div>--%>

        <%--<div class="container">
            <h2 class="mt-4">Export Issue</h2>
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" />
            </div>
            <asp:Button ID="ExportToExcelButton" runat="server" Text="Export to Excel" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" />
        </div>--%>
        <asp:Label ID="lblStatus" runat="server"></asp:Label>

        <%--<div class="container mt-3">
            <h2>Divers</h2>
            <asp:Button ID="ExportDiversStockButton" runat="server" Text="Export to Excel" OnClick="ExportDiversStockButton_Click" CssClass="btn btn-primary" />
            <asp:GridView ID="GridViewExtraIssueDivers" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </div>--%>

        <%--<div class="container mt-3">
            <h2>Officers</h2>
            <asp:Button ID="Button2" runat="server" Text="Export to Excel" OnClick="ExportOfficersButton_Click" CssClass="btn btn-primary" />
            <asp:GridView ID="GridViewOfficersSheet" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </div>

        <div class="container mt-3">
            <h2>Extra Issue</h2>
            <asp:Button ID="Button3" runat="server" Text="Export to Excel" OnClick="ExportExtraIssueButton_Click" CssClass="btn btn-primary" />
            <asp:GridView ID="GridViewExtraIssue" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </div>--%>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="wwwroot/js/js/bundle.js"></script>
    <script src="wwwroot/js/js/scripts.js"></script>
    <script src="wwwroot/js/js/charts/gd-default.js"></script>
    <script src="wwwroot/js/js/example-chart.js"></script>
    <script src="wwwroot/js/js/charts/chart-crm.js"></script>
    <script>
        window.onload = function () {
            var ctx = document.getElementById('orderOverviewStrength').getContext('2d');
            ctx.canvas.width = 2400;
            ctx.canvas.height = 400;
            // Create the chart
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: chartDataST,
                options: {
                    scales: {
                        y: {
                            beginAtZero: true // Start y-axis at zero
                        }
                    }
                }
            });

            var ctx = document.getElementById('orderOverviewPresent').getContext('2d');
            ctx.canvas.width = 2400;
            ctx.canvas.height = 400;
            // Create the chart
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: chartDataP,
                options: {
                    scales: {
                        y: {
                            beginAtZero: true // Start y-axis at zero
                        }
                    },

                    plugins: {
                        legend: {
                            position: 'top' // Example: adjust legend position if needed
                        }
                    }
                }
            });

            var ctx = document.getElementById('orderOverviewReceipt').getContext('2d');
            ctx.canvas.width = 2400;
            ctx.canvas.height = 400;
            // Create the chart
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: chartDataR,
                options: {
                    scales: {
                        y: {
                            beginAtZero: true // Start y-axis at zero
                        }
                    }
                }
            });


            var ctx = document.getElementById('orderOverviewIssue').getContext('2d');

            // Create the chart
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: chartDataI,
                options: {
                    scales: {
                        y: {
                            beginAtZero: true // Start y-axis at zero
                        }
                    }
                }
            });
        };

        function addAlternativeItem() {
            var tableBody = document.getElementById("Table2");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td><input type="text" class="form-control" name="alternateitemname" /></td>
                                <td><input type="number" class="form-control" name = "equivalentofficerScale" required min="0" step="0.001"/></td>
                                <td><input type="number" class="form-control" name="equivalentsailorScale" required min="0" step="0.001"/></td>
                                <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
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
            var row = btn.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        function getVMSStatus() {
            var selectedDate = document.getElementById("vmsDate").value;

            fetch('Dashboard.aspx/GetVMSStatus', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ selectedDate: selectedDate })
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(data => {
                    console.log(data); // Inspect data structure
                    if (data && data.d) {
                        updateProgressBar(data.d);
                    } else {
                        console.error('No data found in response or incorrect data structure');
                    }
                })
                .catch(error => {
                    console.error('Error fetching VMS status:', error);
                });
        }

        function updateProgressBar(data) {
            let rows = JSON.parse(data);
            if (rows.length > 0) {
                var item = rows[0];

                var percentage = 0;
                if (item.SK === true && item.Logo === false && item.LogoReject === false && item.LogoApprove === false && item.LogotoCo === false && item.Co === false && item.CoReject === false && item.CoApprove === false0) {
                    percentage = 25;
                } else if (item.Logo === true && item.SK === false && item.LogoReject === false && item.LogoApprove === false && item.LogotoCo === false && item.Co === false && item.CoReject === false && item.CoApprove === false) {
                    percentage = 50;
                } else if (item.Co === true && item.SK === false && item.Logo === false && item.LogoReject === false && item.LogoApprove === false && item.LogotoCo === false && item.CoReject === false && item.CoApprove === false) {
                    percentage = 75;
                } else if (item.IsApproved === true) {
                    percentage = 100;
                }

                var progressBar = document.querySelector('.progress-bar');
                progressBar.style.width = percentage + '%';
                progressBar.setAttribute('aria-valuenow', percentage);
            }
        }

    </script>
</asp:Content>

