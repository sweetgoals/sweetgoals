<%@ Page Title="Sweet Goals - For The Cause" 
    Language="VB" 
    MasterPageFile="~/Site.master" 
    AutoEventWireup="false" 
    CodeFile="SummaryGoal.aspx.vb" 
    Inherits="summary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/summary.css" type="text/css" />
    <link rel="stylesheet" href="../Content/ThirdParty/prettyPhoto.css" type="text/css" media="screen" 
          title="prettyPhoto main stylesheet"/>  

    <script src="../Scripts/ThirdParty/jquery.prettyPhoto.js" type="text/javascript" charset="utf-8"></script>	

    <script type="text/javascript" charset="utf-8">
        $(document).ready(function () {
            $("a[rel^='prettyPhoto']").prettyPhoto();

            $(".gallery:first a[rel^='prettyPhoto']").prettyPhoto({
                animation_speed: 'normal',
                overlay_gallery: false,
                theme: 'pp_default',
                social_tools: false,
            });
        });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <div id="summary" class="pageOutline">
        <asp:Label ID="pageTitle" runat="server" Text="" />
        <div id="goalPics" class="gPics" runat="server">
            <%--        Pictures --%>
        </div>
        <asp:Table ID="statisticsTable" runat="server" HorizontalAlign="Center"/>
    </div>
</asp:Content>

