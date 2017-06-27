<%@ Page Language="VB" AutoEventWireup="false" CodeFile="usercontrol.aspx.vb" Inherits="userControl_usercontrol" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Table ID="userTable" runat="server" Caption="<h1> Good Users</h1>" HorizontalAlign="Center"/>

        <asp:Table ID="userLockedTable" runat="server" Caption="<h1> Locked Out Users</h1>" HorizontalAlign="Center"/>

        <asp:Table ID="pictureTable" runat="server" Caption="<h1> Pictures</h1>" HorizontalAlign="Center"/>
    </div>
    </form>
</body>
</html>
