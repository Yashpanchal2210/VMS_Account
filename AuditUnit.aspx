<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AuditUnit.aspx.cs" Inherits="VMS_1.Audit.AuditUnit" %>

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
                                <div class="row row-align col-md-12">
                                    <div class="card-title-group align-start mb-3">
                                        <div class="card-title">
                                            <h5 class="title">Audit Module</h5>
                                        </div>
                                    </div>
                                </div>
                                <div class="row row-align col-md-12">
                                    <div class="col-md-2">
                                        <h6 class="title">Unit</h6>
                                    </div>
                                    <div class="col-md-4">
                                        <select class="form-control" name="basicItem" id="basicnameVal" onchange="fetchBasicDenom()">
                                            <option value="">Select</option>
                                            <option value="InsHamla">INS Hamla</option>
                                        </select>
                                    </div>
                                    <div class="col-md-5"></div>
                                </div>
                                <div class="row row-align col-md-12">
                                    <br />
                                </div>
                                <div class="row row-align col-md-12">
                                    <div class="col-md-2">
                                        <h6 class="title">Month</h6>
                                    </div>
                                    <div class="col-md-10">                                      
                                        <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" style="width: 21%" required />
                                    </div>
                                </div>
                                <div class="row row-align col-md-12">
                                    <br />
                                </div>
                                <div class="row row-align col-md-12">
                                    <div class="col-md-2"></div>
                                    <div class="col-md-10">
                                        <asp:Button ID="btnView" class="btn btn-primary" runat="server" Text="View Victualling Account" OnClick="btnView_Click" />
                                    </div>
                                </div>
                                <div class="row row-align col-md-12">
                                    <br />
                                </div>
                                <div class="row row-align col-md-12">
                                    <div class="col-md-2">
                                        <h6 class="title">Observation</h6>
                                    </div>
                                    <div class="col-md-10">
                                        <asp:TextBox ID="txtObservation" runat="server" TextMode="MultiLine" Height="100px" Width="400px"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row row-align col-md-12">
                                    <br />
                                </div>
                                <div class="row row-align col-md-12">
                                    <div class="col-md-2"></div>
                                    <div class="col-md-1">
                                        <asp:Button ID="btnApprove" class="btn btn-success" runat="server" Text="Approve" OnClick="btnApprove_Click" />
                                    </div>
                                    <div>
                                        <asp:Button ID="btnReturn" class="btn btn-warning" runat="server" Text="Return to unit with observations" OnClick="btnReturn_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
