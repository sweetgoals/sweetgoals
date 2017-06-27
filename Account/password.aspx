<%@ Page Title="Sweet Goals - For The Cause"
    Language="VB"
    MasterPageFile="../Site.master"
    AutoEventWireup="false"
    CodeFile="password.aspx.vb"
    Inherits="Account_password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
    <link rel="stylesheet" href="../Content/login.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <asp:Panel ID="Panel1" runat="server" Height="170px">
        <asp:PasswordRecovery ID="PasswordRecovery1"
            align="center"
            runat="server"
            UserNameInstructionText="Enter your User Name and we'll email your password to you. ">
        </asp:PasswordRecovery>
    </asp:Panel>
</asp:Content>

