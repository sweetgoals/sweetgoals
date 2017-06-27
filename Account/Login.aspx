<%@ Page Title="Sweet Goals - For The Cause" 
         MasterPageFile="../Site.master"  
         Language="VB"  
         AutoEventWireup="true" 
         CodeFile="Login.aspx.vb" 
         Inherits="Account_Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/login.css" type="text/css" />
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="FeaturedContent">
        <div style="text-align:center; right: auto; left: auto; width: auto;">
    <section id="loginForm">
        <h2>Logon to make your dreams come true!</h2>
        <asp:Login runat="server" ViewStateMode="Disabled" RenderOuterTable="false">
            <LayoutTemplate>
                <p class="validation-summary-errors">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
                <fieldset>
                    <legend>Log in Form</legend>
                    <ol>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="UserName">User name</asp:Label>
                            <asp:TextBox runat="server" ID="UserName" CssClass="usernameBox" /><br />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName"
                                 CssClass="field-validation-error" />
                        </li>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="" /><br />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                                 CssClass="field-validation-error"  />
                        </li>
                        <li>
                            <asp:CheckBox runat="server" ID="RememberMe" />
                            <asp:Label runat="server" AssociatedControlID="RememberMe" 
                                CssClass="checkbox">Remember me?</asp:Label>
                        </li>
                    </ol>
                    <asp:Button runat="server" CommandName="Login" Text="Log in" />
                </fieldset>
            </LayoutTemplate>
        </asp:Login>
        <p>
            <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register</asp:HyperLink>
            if you don't have an account.<br />
            <asp:HyperLink runat="server" ID="sendPassword" ViewStateMode="Disabled">Send Password</asp:HyperLink>
            if you've forgotten it.
        </p>
    </section>
   </div>
</asp:Content>