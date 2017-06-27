<%@ Page Title="" Language="VB" AutoEventWireup="false" CodeFile="response.aspx.vb" Inherits="backend_response"%>
<%@ Register Src="~/CustomControls/sweetgoalsmenu.ascx" TagPrefix="uc1" TagName="sweetgoalsmenu" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Sweet Goals - For the Cause</title>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <link rel="stylesheet" href="../Content/prettyPhoto.css" type="text/css" media="screen"/>
    <link rel="stylesheet" href="../Content/ThirdParty/bootstrap.css" type="text/css" />
    <link rel="stylesheet" href="../Content/response.css" type="text/css" />
    <link rel="stylesheet" href="../Content/sweetgoalsmenu.css" type="text/css"/>
    <link rel="stylesheet" href="../Content/master.css" type="text/css"/>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/ThirdParty/jquery.prettyPhoto.js" type="text/javascript" charset="utf-8"></script>
    <script src="../Scripts/ThirdParty/bootstrap.js" type="text/javascript" charset="utf-8"></script>
    <script type="text/javascript" charset="utf-8">
        $("a[rel^='prettyPhoto']").prettyPhoto();
        $(".gallery:first a[rel^='prettyPhoto']").prettyPhoto({
            animation_speed: 'normal', theme: 'light_square',
            slideshow: 3000, autoplay_slideshow: true
        });
        $(".gallery:gt(0) a[rel^='prettyPhoto']").prettyPhoto({ animation_speed: 'fast', slideshow: 10000, hideflash: true });

        $("#custom_content a[rel^='prettyPhoto']:first").prettyPhoto({
            custom_markup: '<div id="map_canvas" style="width:260px; height:265px"></div>',
            changepicturecallback: function () { initialize(); }
        });
        $("#custom_content a[rel^='prettyPhoto']:last").prettyPhoto({
            custom_markup: '<div id="bsap_1259344" class="bsarocks bsap_d49a0984d0f377271ccbf01a33f2b6d6"></div>' +
                '<div id="bsap_1237859" class="bsarocks bsap_d49a0984d0f377271ccbf01a33f2b6d6" style="height:260px">' +
                '</div><div id="bsap_1251710" class="bsarocks bsap_d49a0984d0f377271ccbf01a33f2b6d6"></div>',
            changepicturecallback: function () { _bsap.exec(); }
        });
	</script>
</head>
<body>   
    <div id="desktopSection" class="containerdiv hidden-sm hidden-xs visible-md visible-lg">
        <uc1:sweetgoalsmenu runat="server" ID="sweetgoalsmenu" />
    </div>
    <div id="mobileSection" class="containerdiv visible-sm visible-xs hidden-md hidden-lg">
        <uc1:sweetgoalsmenu runat="server" ID="sweetgoalsmenu1" />
    </div>
    <form runat="server">
        <div id="mainDetail" runat="server" class="groupOutline">
            <h1> Sweet Goals Response</h1><br />
            <asp:Table ID="goalDetailTable"  
                class="table table-condensed" 
                runat="server" 
                HorizontalAlign="Center" 
                Visible="true" 
                Width="40%"/>
            <asp:Table ID="picTable"  
                runat="server" 
                CaptionAlign="Top" 
                HorizontalAlign="Center" 
                Width="40%"
                Visible="false" 
                CssClass="gallery clearfix"/>
            <asp:Table ID="actTable" 
                runat="server" 
                Caption="<h1>Past Activities</h1>" 
                CaptionAlign="Top" 
                HorizontalAlign="Center" 
                Visible="true"
                class="table table-condensed"
                Width="80%"/>
            <br />
            <asp:Label ID="msgLabel" 
                runat="server" 
                Text="Based on the Evidence Presented did they complete the Activity?"></asp:Label>    
            <br />
            <asp:Panel ID="inputPanel" runat="server">
                <asp:TextBox ID="supMsgText" runat="server" cssClass="textboxes" Visible="true" TextMode="MultiLine"/>
                <br />
                <asp:Button ID="yesButton" runat="server" Text="Yes" Visible="true" />
                <asp:Button ID="noButton" runat="server" Text="No" Visible="true" />
                <br />
                <br />
                <asp:Button ID="unsubscribeButton" runat="server" Text="Unsubscribe" Visible="true" /><br />
                <asp:label id="unsubLabel" runat="server" 
                    Text="Unsubscribed. You will not recieve any more emails unless you create an account" Visible="False" />
            </asp:Panel>
        </div>
        <div id="simpleResponse" runat="server" class="groupOutline">
            <asp:Label ID="simpleMsg" runat="server"></asp:Label>
        </div>
    </form>    
</body>
</html>
