<%@ Page Language="VB" AutoEventWireup="false" CodeFile="srequest.aspx.vb" Inherits="backend_srequest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align:center; right: auto; left: auto; width: auto;">
            <asp:HyperLink ID="homePage" runat="server" NavigateUrl="~/Default.aspx">Home Page</asp:HyperLink>
            <h1> SWEET GOALS REQUEST FOR SUPPORT </h1>
            <br />
            <asp:Label ID="msgLabel" runat="server" Text="Thanks for the Support You'll be hearing from me."></asp:Label>

        </div>
    </form>
</body>
</html>
