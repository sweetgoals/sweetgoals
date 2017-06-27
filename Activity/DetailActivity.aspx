<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="DetailActivity.aspx.vb" 
    Inherits="actdetail" ValidateRequest="false" enableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/actdetail.css" type="text/css" />
    <script src="../Scripts/activityDetail.js"></script>
    <script src="../Scripts/ThirdParty/autosize.js"></script>
    <script>
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

<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server" ValidateRequestMode="disabled">
    <div id="goalTitleDiv" runat="server" style="font-size: xx-large;"/>
    <div id="statusDiv" style="width: 80%; background-color:white; text-align: center; 
            margin-right: auto; margin-left: auto;"> 
        <div id="activityStatusDiv" runat="server"></div>
        <div id="timeSection" style="margin-right: auto; margin-left: auto; font-size:x-large">
            <span id="actStartTime" runat="server" style="color:green; padding:10px;"></span>
            <span id="actCompleteTime" runat="server" style="color:red; padding:10px;"></span>
            <span id="actDifferenceTime" runat="server" style="color:black; padding:10px;"></span>
        </div>
    </div>
    <asp:Panel ID="picDisplayPanel" runat="server" CssClass="block-picture" ClientIDMode="Static"/>
    <div id="controlSection" style="width: 80%; text-align: center; margin-right: auto; margin-left: auto;
                                    margin-bottom:100px; background-color: white;">
        <div id="mainSection" runat="server" style="margin-bottom: 25px; border-bottom-color: black; border-bottom-width: 1px; 
                                     border-bottom-style: solid;">
            <div id="sectionMenuColumn" class="menu"> 
                <div id="sectionMenuColumnRow2">
                    <span id="menuPicture" class="menuItem">P</span>
                    <span id="menuTitle" class="menuItem" onclick="createTextControl();">T</span>
                </div>
            </div>
            <div id="activitySections" style="width:100%; display:inline-block">
                <div id="sectionItemColumn"></div>
            </div>
        </div>
        <div id="startSection" class="button-block" runat="server" ClientIDMode="Static">
            <asp:Button ID="createActButton" cssClass="actButtonClass" runat="server" Text="Create" ClientIDMode="Static"
                ValidateRequestMode="disabled" OnClientClick="return submitButtonClick();" Visible="false"/>
            <asp:Button ID="startActButton" cssClass="actButtonClass" runat="server" Text="Start" ValidateRequestMode="disabled"
                OnClientClick="return submitButtonClick();" ClientIDMode="Static" Visible="False"/>
            <asp:Button ID="deleteActButton" cssClass="actButtonClass" runat="server" Text="Delete" 
                ValidateRequestMode="disabled" Visible="False" />
            <asp:Button ID="editActButton" cssClass="actButtonClass" runat="server" Text="Edit" 
                ValidateRequestMode="disabled" Visible="False" ClientIDMode="Static"/>
        </div>
        <div id="endSection" class="button-block" runat="server" visible="false" ClientIDMode="Static">
            <asp:Button ID="manualTime" runat="server" Text="Manual Time" OnClientClick="return false;"
                    ValidateRequestMode="disabled" ClientIDMode="Static" cssClass="actButtonClass"/>
            <div id="manualTimeDiv" class="titleTextControl">
                <img id="closePicTimeDiv" class="closeImg" src="../Images/closepic.png" title="Close"/>
                <asp:label cssClass="timeLabel" runat="server">Time Spent</asp:label>
                <br />
                <asp:DropDownList ID="timeDropDown" cssClass="timeDropDownClass" runat="server" ClientIDMode="Static"
                                    onchange="setHidden(this.options[this.selectedIndex].text);" >
                </asp:DropDownList>
                <asp:TextBox ID="manualBox" runat="server" placeholder="0:15 (hh:mm)" 
                                title="Manual Time Spent. ex: 0:15 (15 minutes)" ClientIDMode="Static">
                </asp:TextBox><br />
                <asp:HiddenField ID="timeDropDownValue" runat="server" ClientIDMode="Static" />
                <asp:Button ID="stopActButtonDialog" runat="server" Text="Stop Activity" cssClass="actButtonClass"
                    ClientIDMode="Static" OnClientClick="return submitButtonClickDialog();" />                      
            </div>
            <asp:Button ID="stopActButton" runat="server" Text="Stop Activity" cssClass="iniHidden" ClientIDMode="Static"
                OnClientClick="return submitButtonClick();" />                      
        </div>
        <div id="commentGroup" CssClass="commentGroup" runat="server">
            <span style="margin: 10px; font-size: x-large; float: left;">Comments, Suggestions, Ideas?</span>
            <div id="aComments" style="margin:30px 30px 100px 30px; " runat="server">
                <asp:TextBox ID="commentInsert" runat="server" TextMode="MultiLine" ValidateRequestMode="Disabled"
                                placeholder="Share Your Wisdom!" CssClass="commentInsertClass"/>
                <br />
                <asp:Button ID="commentButton" cssClass="commentButtonClass" runat="server" Text="Post" 
                            ClientIDMode="Static" />
            </div>
            <asp:Panel ID="actCommentPanel" runat="server" CssClass="actCommentPanel" ClientIDMode="Static"/>
        </div>
    </div>
    <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red" Font-Bold="True" ClientIDMode="Static" />
    <asp:LinkButton ID="previousActivity" CssClass="previousActivity" runat="server" ToolTip="Previous Activity"/>
    <asp:LinkButton ID="nextActivity" CssClass="nextsActivity" runat="server" ToolTip="Next Activity"/>
    <div ID="addPicture" class="titleTextControl">
        <img id="closeImg" class="closeImg" src="../Images/closepic.png" title="Close"/>
        <label class="file-upload">
            <span id="fileUploadTitle">Select Pictures</span>
            <asp:FileUpload ID="picFile" runat="server" AllowMultiple="true" ClientIDMode="Static"
                            title="Pick Pictures. Max 10 at a time" onchange="showFile();"/>
        </label>
        <br />
        <div id="fileList"></div>
        <asp:DropDownList ID="picShareAll" runat="server" CssClass="shareSettingsDropdown" ClientIDMode="Static">
            <asp:ListItem>Share Settings</asp:ListItem>
            <asp:ListItem>Private</asp:ListItem>
            <asp:ListItem>Supporters</asp:ListItem>
            <asp:ListItem>Public</asp:ListItem>
        </asp:DropDownList>
        <br />
        <asp:Button ID="submitPicButton" cssClass="DialogButtons" runat="server" 
            Text="Upload Pictures" ClientIDMode="Static" OnClientClick="return checkPics();" />
    </div>
    <div id="picSettings" class="titleTextControl">
        <img id="closePicSettings" class="closeImg" src="../Images/closepic.png" title="Close"/>
        <h2>Picture Settings</h2>
        <asp:HiddenField id="pictureNumberHidden" ClientIDMode="Static" runat="server" />
        <asp:DropDownList ID="picShareSettingsSingle" runat="server" CssClass="shareSettingsDropdown" ClientIDMode="Static">
            <asp:ListItem>Share Settings</asp:ListItem>
            <asp:ListItem>Private</asp:ListItem>
            <asp:ListItem>Supporters</asp:ListItem>
            <asp:ListItem>Public</asp:ListItem>
        </asp:DropDownList>
        <br />
        <input id="picSettingsButton" class="DialogButtons" type="button" value="Save" onclick="savePictureSettings();" />
        <asp:Button ID="picDeleteButton" cssClass="DialogButtons" runat="server" 
                    Text="Delete" ClientIDMode="Static" OnClientClick="submitButtonClickDialog();" />
    </div>
    <div id="titleDialog" class="titleTextControl">
        <img id="closeTitleDialog" class="closeImg" src="../Images/closepic.png" title="Close"/>
        <input id="titleControlId" type="hidden" />
        <input id="titleControl" type="text" style="width:80%;" onkeypress="return renameTextControlEnter(event);" /><br />
        <input id="titleControlButton" class="DialogButtons" type="button" value="Change Title" 
               onclick="return renameTextControl();" />
    </div>
    <div id="screen"></div>
    <div id="errorDialog" class="errorDialogClass">
        <span id="errorDialogText"> </span><br />
        <input id="errorDialogButton" class="errorButton" type="button" value="Ok" 
               onkeypress="closeErrorDialogButton();" onclick="closeErrorDialogButton();" />
    </div>
    <div id="hiddenItems">
        <asp:HiddenField ID="textControlServerData" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="textControlDataHidden" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="activityTitleHidden" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="activityEditEnableHidden" runat="server" ClientIDMode="Static"/>
    </div>
</asp:Content>