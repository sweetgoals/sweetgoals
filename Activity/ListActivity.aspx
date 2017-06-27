<%@ Page Title="Sweet Goals - For the Cause" 
         Language="VB" 
         MasterPageFile="~/Site.master" 
         AutoEventWireup="false" 
         CodeFile="ListActivity.aspx.vb" 
         Inherits="goaldetail" 
         EnableViewState="True" 
         ViewStateMode="Enabled" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/activities.css" type="text/css" />

    <script type="text/javascript">
        var sendData = "";

        $(document).ready(function () {
            var tour = listActivityPageTour();

            $('.tutorial').on('click', function () {
                startTutorial(tour);
            });
        });

        $(function () {
            var gtl = window.location.search.indexOf("goalTypeList=supporting")
            if (gtl < 0)
            {
                $("#sortable").sortable();
                $("#sortable").disableSelection();
            }
        });

        function saveCreateList() {
            $("#lockScreen").show();
            sendData = "";
            senddata = loadids();
            $.ajax({
                type: "POST",
                url: "ListActivity.aspx/reOrderList",
                data: "{ 'input' : '" + senddata + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: onSuccess,
                error: errormsg,
            })
        }

        function loadids() {
            var ids = "";
            $("#sortable").children().each(function () {
                ids = ids + this.id + ",";
            })
            return ids;
        };

        function onSuccess(result) {
            $("#lockScreen").hide();
            window.location.reload(true);
        }
        function errormsg(result) {
            alert("Error - Yeah... Not very descriptive is it? ");
            $("#lockScreen").hide();
        }

        //Google Tracking
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');
        ga('create', 'UA-63033431-1', 'auto');
        ga('send', 'pageview');

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <div id="main" style="text-align:center; right: auto; left: auto; width: auto;">
        <asp:Panel ID="goalDetailSection" class="goalDetailSection" runat="server"/>
        <div id="activitySection">
            <div id="sortableTable" runat="server" style="text-align:center; display:none; width:100%">
                <div style="margin-right:90%">
                    <input id="orderButton" type="button" runat="server" style="display:inline" value="Save Order" 
                        onclick="saveCreateList()"/>                 
                </div>
                <ul id="sortable" runat="server" style="width:95%" ClientIDMode="Static"></ul>   
            </div>
            <asp:Panel ID="goalDetailPanel" runat="server" Visible="true" CssClass="detailPanel"></asp:Panel>
        </div>
    </div>
    <div id="footer"></div>
    <div id="lockScreen" class="ajaxCallLock lockScreen">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/ajaxload.gif" />
    </div>
</asp:Content>