<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="supporters.aspx.vb" 
    Inherits="supporters" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/supporters.css" type="text/css" />
    <script type="text/javascript" charset="utf-8">
        $(document).ready(function () {
            var tour = supportPageTour();

            $('.tutorial').on('click', function () {
                startTutorial(tour);
            });

            $('.dAccount').on('click', function () {
                $("#dasupemail").val(event.target.text);
                $("#deleteAccount").dialog('open');
                return false;
            });

            $('.dGoal').on('click', function () {
                $("#dgsupemail").val(event.target.text);
                $("#deleteGoal").dialog('open');
                return false;
            });

            $("#deleteGoal").dialog({
                autoOpen: false,
            });

            $("#deleteGoal").dialog({
                autoOpen: false,
                width: 425,
                height: 300,
                buttons: [
                  {
                      text: "OK",
                      click: function () {
                          $("#lockScreen").show();
                          $.ajax({
                              type: "POST",
                              url: "supporters.aspx/removeGoalSupport",
                              data: JSON.stringify({
                                  "supemail": $("#dgsupemail").val(),
                                  "supmsg": $("#dgsupmsg").val(),
                                  "gn": '<%=Session("gN")%>',
                              }),
                              contentType: "application/json; charset=utf-8",
                              dataType: "json",
                              success: onDeleteSuccess,
                              error: errormsg,
                          })
                          $("#dgsupemail").val("");
                          $("#dgsupmsg").val("");
                          $(this).dialog("close");
                      }
                  }
                ]
            });

            function onDeleteSuccess(result) {
                // The response from the function is in the attribute d
                // alert("yay it worked");
                $("#lockScreen").hide();
                window.location.reload(true);
            }
            function errormsg(result) {
                $("#lockScreen").hide();
                alert("Error - Yeah... Not very descriptive is it? ");
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <div style="display:inline-block; margin-top:2%; background-color:#8DA5ED;">
        <asp:Table ID="goalSupportTable" runat="server" HorizontalAlign="Center" Visible="false" CssClass="sTable"/>
    </div>
    <div id="addGoal" style="font-size:medium;" title="Add Supporter To Goal">
        <asp:Table ID="Table1" runat="server" HorizontalAlign="Center">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label ID="Label1" runat="server" Text="Support Email"/>
                </asp:TableCell>
                <asp:TableCell Width="98%">
                    <input id="agsupemail" type="email" name="email" style="width:100%; box-sizing:border-box;"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label ID="Label2" runat="server" Text="Support Message"/>
                </asp:TableCell>
                <asp:TableCell Width="98%">
                    <textarea  id="agsupmsg" rows="4" cols="25" style="width:100%; box-sizing:border-box;"></textarea>
                </asp:TableCell>
            </asp:TableRow>
        </asp:table>
    </div>
    <div id="deleteGoal" style="font-size:medium;"; title="Delete Supporter From Goal">
        <asp:Table ID="Table3" runat="server" HorizontalAlign="Center">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label ID="Label5" runat="server" Text="Support Email"/>
                </asp:TableCell>
                <asp:TableCell Width="98%">
                    <input id="dgsupemail" type="email" name="email" style="width:100%; box-sizing:border-box;">
                </asp:TableCell>
            </asp:TableRow>        
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label ID="Label6" runat="server" Text="Support Message"/>
                </asp:TableCell>
                <asp:TableCell Width="98%">
                    <textarea  id="dgsupmsg" rows="4" cols="25" style="width:100%; box-sizing:border-box;"></textarea>
                </asp:TableCell>
            </asp:TableRow>
        </asp:table>
    </div>
    <div id="lockScreen" class="ajaxCallLock" style="display:none;">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/ajaxload.gif" />
    </div>
</asp:Content>

