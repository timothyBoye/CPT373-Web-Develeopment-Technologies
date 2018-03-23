<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/admin.Master" AutoEventWireup="true" CodeBehind="UploadTemplate.aspx.cs" Inherits="GoLA2.Admin.UploadTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Upload Templates</h2>
    <p>
        A well formed file is in the format "height [newline] width [newline] grid of cells(x or o)". For example:
    </p>
    <pre>5
5
XXXXX
XXXXX
XOOOX
XXXXX
XXXXX
</pre>
    <asp:FileUpload ID="fileUpload" runat="server" />
    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_click"/>
    <asp:Label ID="Validation" runat="server" Text="" CssClass=""/>
</asp:Content>
