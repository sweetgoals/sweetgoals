<%@ Page Title="" Language="VB" MasterPageFile="../Site.master" AutoEventWireup="false" 
    CodeFile="regcomplete.aspx.vb" Inherits="Account_regcomplete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
        <asp:Label ID="registrationMessage" runat="server" Text="Label"></asp:Label><br />
        <asp:hyperlink ID="logonButton" runat="server" NavigateUrl="~/Account/Login.aspx">Logon</asp:hyperlink>
</asp:Content>

