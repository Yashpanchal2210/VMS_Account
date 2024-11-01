﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="VMS_1.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="wwwroot/css/font-awesome.min.css" rel="stylesheet" />
    <style type="text/css">
        body {
            font-family: Arial, Helvetica, sans-serif;
        }
        
        .navbar {
            overflow: hidden;
            background-color: #7091E6;
        }
        .navbarh {
            overflow: hidden;
            background-color: #3D52A0;
        }

            .navbar a {
                float: left;
                font-size: 16px;
                color: white;
                text-align: center;
                padding: 14px 16px;
                text-decoration: none;
            }

        .dropdown {
            float: left;
            overflow: hidden;
        }

        .dropdownright {
            float: right;
            overflow: hidden;
            margin-right: 42px;
        }

            .dropdown .dropbtn, .dropdownright .dropbtn {
                font-size: 16px;
                border: none;
                outline: none;
                color: white;
                padding: 14px 16px;
                background-color: inherit;
                font-family: inherit;
                margin: 0;
            }

            .navbar a:hover, .dropdown:hover .dropbtn, .dropdownright:hover .dropbtn {
                background-color: red;
            }

        .dropdown-content, .dropdownright-content {
            display: none;
            position: absolute;
            background-color: #f9f9f9;
            min-width: 160px;
            box-shadow: 0px 8px 16px 0px rgba(0, 0, 0, 0.2);
            z-index: 1;
        }

            .dropdown-content a, .dropdownright-content a {
                float: none;
                color: black;
                padding: 12px 16px;
                text-decoration: none;
                display: block;
                text-align: left;
            }

                .dropdown-content a:hover, .dropdownright-content a:hover {
                    background-color: #ddd;
                }

        .dropdown:hover .dropdown-content, .dropdownright:hover .dropdownright-content {
            display: block;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div class="navbarh">
            <div style="font-size: 30px; font-weight: 500; font-weight: bolder; height: 60px;text-align: center;margin-top: 17px;color: white;">
                VICTUALLING MANAGEMENT SYSTEM
            </div>
        </div>
        <div class="navbar">
            <a href="#home">Home</a>
            <a href="#services">Services</a>

            <div class="dropdown">
                <button class="dropbtn">
                    Dropdown 1 
               
                    <i class="fa fa-caret-down"></i>
                </button>
                <div class="dropdown-content">
                    <a href="#">Link 1</a>
                    <a href="#">Link 2</a>
                    <a href="#">Link 3</a>
                </div>
            </div>

            <!-- New Right Corner Menu -->
            <div class="dropdownright">
                <button class="dropbtn">
                    Logout
               
                    <i class="fa fa-caret-down"></i>
                </button>
                <div class="dropdownright-content">
                    <a href="#">Reset Pssword</a>
                    <a href="#">Logout</a>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
