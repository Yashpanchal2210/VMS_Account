<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CurrentStockList.aspx.cs" Inherits="VMS_1.CurrentStockList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function PrintGrid(html, css) {
            var printWin = window.open('', '', 'left=0,top=0,width=400,height=300,scrollbars=1');
            printWin.document.write('<style type = "text/css">' + css + '</style>');
            printWin.document.write(html);
            printWin.document.close();
            printWin.focus();
            printWin.print();
            printWin.close();
        };
    </script>
    <style id="gridStyles" runat="server" type="text/css">
        body {
            font-family: Arial;
            font-size: 10pt;
        }

        table {
            border: 1px solid #ccc;
            border-collapse: collapse;
            width: 100%;
        }

        tableth {
            background-color: #F7F7F7;
            color: #333;
            font-weight: bold;
        }

        tableth, tabletd {
            padding: 5px;
            border: 1px solid #ccc;
        }

        table, table table td {
            border: 0px solid #ccc;
        }
    </style>
    <form id="frm" runat="server">
        <div class="container">
            <div class="nk-block">
                <div class="row g-gs">
                    <div class="col-lg-12">
                        <div class="card card-bordered h-100">
                            <div class="card-inner">
                                <div class="row row-align col-md-12">
                                    <div class="card-title-group align-start mb-3">
                                        <div class="card-title">
                                            <h5 class="title">Curret Stock List</h5>
                                        </div>
                                    </div>
                                </div>
                                <div>
                                </div>
                                <div class="row row-abglign col-md-12">
                                    <div class="col-md-4">
                                        
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Button ID="btnPrintCurrent" CssClass="btn btn-warning" runat="server"  Text="Print" OnClick="PrintGridView" CommandArgument="All" />
                                        <asp:GridView ID="gvStock" runat="server" CssClass="table table-bordered table-striped"
                                            AutoGenerateColumns="False" PageSize="30" Width="100%">
                                            <Columns>
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="ItemName" HeaderText="ItemName" />
                                                <asp:BoundField DataField="Qty" HeaderText="Qty" />
                                                <asp:BoundField DataField="Denos" HeaderText="Denos" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <div class="col-md-4">
                                    </div>
                                </div>
                            </div>
                            <asp:LinkButton ID="lnkPrint" runat="server" ToolTip="Click to Print All Records"
                                Text="Print Data" OnClientClick="PrintPage()">
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
