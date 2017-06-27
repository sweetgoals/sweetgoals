<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="settings.aspx.vb"
     Inherits="settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/settings.css" type="text/css" />
    <script type="text/javascript" charset="utf-8">
        $(document).ready(function () {
            var tour = settingsPageTour();

            $('.tutorial').on('click', function () {
                startTutorial(tour);
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <div class="contentDiv" style="">
        <asp:Table ID="settingsTable" runat="server" Caption="<h1><u>Account Settings</u></h1>" CaptionAlign="Top"
             HorizontalAlign="Center" CellSpacing="20">
            <asp:TableRow ID="unsubRow" runat="server">
                <asp:TableCell>Unsubscribe To All Goals</asp:TableCell>
                <asp:TableCell><asp:CheckBox ID="unsubscribeBox" runat="server" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="emailRow">
                <asp:TableCell>Email Address</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="emailBox" runat="server" cssClass="emailBox"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>        
        <asp:HyperLink runat="server" NavigateUrl="~/Account/Manage.aspx" CssClass="passChange">Change Password </asp:HyperLink>
        <br />
        <asp:Button ID="updateButton" runat="server" Text="Update Settings" /><br />
    </div>
    <asp:Label ID="errorLabel" runat="server" Visible="false" ForeColor="Red" />
</asp:Content>

