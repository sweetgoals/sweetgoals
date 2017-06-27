<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="achievements.aspx.vb" 
    Inherits="achievements" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
        <link rel="stylesheet" href="../Content/achievements.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <div ID="achives" class="pageOutline" runat="server" HorizontalAlign="Center" Width="80%">
        <h1 style="display:inline-block">Achievements</h1>      
<%--        Achivements get created dynamically       --%>
    </div>
</asp:Content>

