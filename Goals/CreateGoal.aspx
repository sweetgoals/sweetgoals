<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="CreateGoal.aspx.vb" 
    Inherits="creategoal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/creategoal.css" type="text/css" />
    <script src="../Scripts/createGoal.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <div id='createGoalTable' class='createGoalTable' runat="server">
        <asp:Label ID="pageTitle" runat="server" Text="Create Goal" Font-Size="X-Large" />
        <div>
            <asp:TextBox ID="goalTitleText" runat="server" TextMode="SingleLine" ToolTip="What are you doing?" 
                            placeholder="Goal Title" Width="500px" ValidateRequestMode="disabled" ClientIDMode="Static" 
                            MaxLength="25">
            </asp:TextBox>
            <br />
            <asp:TextBox ID="goalDescText" runat="server" TextMode="SingleLine" ToolTip="Why are you doing this?" 
                            placeholder="Goal Description" Width="500px" ValidateRequestMode="disabled"
                            ClientIDMode="Static" MaxLength="150">
            </asp:TextBox>
            <br />
            <div id="datepicker-wrap" class="datePicker">
                <span style="font-size:X-Large;">Completion Date</span>
                <div id="datepicker">
                    <asp:HiddenField ID="dueDate" runat="server" ClientIDMode="Static"/>
                </div>
            </div>
        </div>
        <br />
        <div id="actSetup">
            <h2>Activity Setup</h2>
            <asp:TextBox ID="freqBox" placeholder="3" tooltip="Freqency (Whole Number) ex: 5" runat="server" 
                Width="50px" Height="30px" ClientIDMode="Static" MaxLength="2">                  
            </asp:TextBox>
            <asp:Label ID="timeWord" Font-Bold="True" runat="server" Text="Times" ClientIDMode="Static"/>
            <b>Per</b>
            <asp:DropDownList ID="freqDropDownSingle" runat="server" tooltip="times per: day/week/month" 
                              Height="40px" style="display:none;" ClientIDMode="Static">
                <asp:ListItem>Day</asp:ListItem>
                <asp:ListItem>Week</asp:ListItem>
                <asp:ListItem>Month</asp:ListItem>
                <asp:ListItem>Year</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="freqDropDownPlural" runat="server" tooltip="times per: days/weeks/months" 
                              Height="40px" ClientIDMode="Static">
                <asp:ListItem>Days</asp:ListItem>
                <asp:ListItem>Weeks</asp:ListItem>
                <asp:ListItem>Months</asp:ListItem>
                <asp:ListItem>Years</asp:ListItem>
            </asp:DropDownList>

            <b> For</b>
            <asp:TextBox ID="timeText" runat="server" placeholder="7" ToolTip="Duration (whole number) ex: 3" 
                         Width="50px" Height="30px" ClientIDMode="Static" MaxLength="2"></asp:TextBox> 
            <asp:DropDownList ID="timeUnitDropPlural" runat="server" ToolTip="Minutes or Hours" Height="40px"
                              ClientIDMode="Static">
                <asp:ListItem>Minutes</asp:ListItem>
                <asp:ListItem>Hours</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="timeUnitDropSingle" runat="server" ToolTip="Minutes or Hours" Height="40px" 
                              style="display:none;" ClientIDMode="Static">
                <asp:ListItem>Minute</asp:ListItem>
                <asp:ListItem>Hour</asp:ListItem>
            </asp:DropDownList>
            <br />
        </div>
        <asp:HiddenField ID="timeUnitSelect" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="freqDropDownSelect" runat="server" ClientIDMode="Static"/>
        <asp:Button ID="submitButton" runat="server"  OnClientClick="return checkCreateGoalPageForErrors();" 
            Text="Create Goal" />
        <br />
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red" Font-Bold="True" ClientIDMode="Static" />
    </div>
    <br />
    <div id="modifyNotice" class="modifyNotice">
        <div>
            Modifying a goal is a very delicate process. You need to be careful that you do not change the goal and allow goal 
            creep. When changing a goal you just want to make it more descriptive of what you are doing or change the goal
            guidelines. You need to be very careful that you do not change the definition. If this goal has started out as one
            thing and is morphing into something else then just complete this goal and create a new one. For example, The goal 
            is to go to the gym three days a week and working out for one hour. Then you decide that you want to be able to do
            ten pull ups then that is a completely different goal. You can only modify a goal every 24 hours.
        </div>
        <input id="noticeButton" type="button" value="Close" />
    </div>
    <div id="screen"></div>
</asp:Content>