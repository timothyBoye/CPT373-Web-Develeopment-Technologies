<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/admin.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GoLA2.Admin.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Login</h2>

    <div class="form-horizontal">
        <div class="form-group">
            <asp:label class="control-label col-md-2" for="email" runat="server">Email Address:</asp:label>
            <div class="col-md-10">
                <asp:TextBox type="email" class="form-control" id="Email" name="Email" required="required" runat="server"/>
            </div>
        </div>
        <div class="form-group">
            <asp:label class="control-label col-md-2" for="password" runat="server">Password:</asp:label>
            <div class="col-md-10">
                <asp:TextBox type="password" class="form-control" id="Password" name="Password" required="required" runat="server"/>
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:label id="ValidationError" class="text-danger" runat="server"></asp:label>
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:button id="LoginButton" text="Login" runat="server" class="btn btn-default" OnClick="LoginButton_Click"></asp:button>
            </div>
        </div>
    </div>
</asp:Content>
