<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ReasonsGoal.aspx.vb" 
    Inherits="goalreasons" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/reasons.css" type="text/css" />

    <script type="text/javascript">
        function saveReasons() {
            $("#lockScreen").show();
            sendData = "";
            sendData = formatReasons();
            var msg;
            $.ajax({
                type: "POST",
                url: "../Goals/ReasonsGoal.aspx/saveList",
                data: JSON.stringify({
                    "gReasons": sendData,
                    "goalNum": '<%=Session("gN")%>',
                                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) { onSuccess(msg.d)},
                error: errormsg,
            })
        }

        function formatReasons() {
            var reasonStr = "";
            var eachReason = [];

            //eachReason = $("#reasonList").find('.dSpan');
            eachReason = $("#reasonList").find('.reasonClass');
            eachReason.each(function () {
                reasonStr = reasonStr + this.innerText + "|||";
            })
            return reasonStr;
        };

        function onSuccess(result) {
            if (result.length > 5)
                showAchievementPopup(result);
            $("#lockScreen").hide();
        }
        function errormsg(result) {
            alert("Error - Yeah... Not very descriptive is it? ");
            $("#lockScreen").hide();
        }

        function showAchievementPopup(message) {
            $(function () {
                $("#achPopup").html(message);
                $("#achPopup").dialog({
                    title: "New Achievement!",
                    modal: true,
                    buttons: {
                        Close: function () {
                            $(this).dialog('close');
                        }
                    }
                });
            });
        };

        $(document).ready(function () {
            var counter = 0;
            var disableSort = '<%=Session("disableSort")%>';
            var tour = goalReasonsPageTour();

            $('.tutorial').on('click', function () {
                startTutorial(tour);
            });

            if (disableSort == 0)
                $("#reasonList").sortable();

            $("#nreason").keypress(function (e) {
                if (e.keyCode==10 || e.keyCode == 13) {                    
                    e.preventDefault();
                    $("#addButton").click();
                }
            });
            $("#addButton").click(function (e) {
                e.preventDefault();
                counter++;
                var reasonText = "";
                reasonText =$("input[name='newReason']").val()

                if (reasonText.length > 5) {
                    var tSpan = $(document.createElement('span'))
                       .attr("id", 'rt' + counter)
                       .attr("value", $("input[name='newReason']").val())
                       .attr("class", 'dSpan');
                    tSpan.text(reasonText);

                    var img = $('<img />',
                                 {
                                     id: 'rd' + counter,
                                     src: '../Images/closepic.png',
                                     class: 'closeImg'
                                 });
                    var $li = $("<li class='reasonClass'/>")
                    $li.append(img);
                    $li.append(tSpan);

                    $("#reasonList").append($li);
                    $("#reasonList").sortable('refresh');
                    $("#nreason").val("");
                }
            });

            // removes the reason 
            $("body").on('click', '#reasonList .closeImg', function () {
                $(this).closest(".reasonClass").remove();
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <div id="content" style="text-align:center; right: auto; left: auto; width: auto;">
        <div id="reasonsDiv" class="reasonDiv">      
            <asp:Label ID="goalReasonLabel" runat="server" CssClass="labelClass" Text=""/>
            <ul id="reasonList" runat="server" ClientIDMode="Static">
                <%--Holds the users goal reasons. These get pulled from the server.--%>
            </ul>
            <div id='TextBoxesGroup' runat="server" style="text-align:center;">
                <input type="text" placeholder="Why are you doing this?" name="newReason" id="nreason" style="width:80%;" />
                <br />
                <input type="button" value="Add Reason" id="addButton">
                <input type="button" id="saveReasons" runat="server" value="Save Reasons" OnClick="saveReasons()" />
	        </div>
        </div>
    </div>
    <div id="lockScreen" class="ajaxCallLock" style="display:none; position:fixed; height:100%; width:100%">
        <asp:Image ID="Image1" runat="server" ImageUrl="../Images/ajaxload.gif" />
    </div>
</asp:Content>
