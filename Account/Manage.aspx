<%@ Page Title="Manage Account" Language="VB" MasterPageFile="../Site.Master" AutoEventWireup="false" 
    CodeFile="Manage.aspx.vb" Inherits="Account_Manage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="../Content/login.css" type="text/css" />
</asp:Content>

<asp:Content ContentPlaceHolderID="FeaturedContent" Runat="Server">
    <section id="passwordForm">
        <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
            <p class="message-success"><%: SuccessMessageText %></p>
        </asp:PlaceHolder>

        <p>You're logged in as <strong><%: User.Identity.Name %></strong>.</p>

        <asp:PlaceHolder runat="server" ID="setPassword" Visible="false">
            <p>
                You do not have a local password for this site. Add a local
                password so you can log in without an external login.
            </p>
            <ul>
                <li>
                    <asp:Label runat="server" AssociatedControlID="password">Password</asp:Label>
                    <asp:TextBox runat="server" ID="password" TextMode="Password" CssClass="" /><br />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="password"
                        CssClass="field-validation-error" ErrorMessage="The password field is required."
                        Display="Dynamic" ValidationGroup="SetPassword" />                        
                    <asp:Label runat="server" ID="newPasswordMessage" CssClass="field-validation-error"
                        AssociatedControlID="password" />                        
                </li>
                <li>
                    <asp:Label runat="server" AssociatedControlID="confirmPassword">Confirm password</asp:Label>
                    <asp:TextBox runat="server" ID="confirmPassword" TextMode="Password" CssClass="" /><br />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="confirmPassword"
                        CssClass="field-validation-error" Display="Dynamic" 
                        ErrorMessage="The confirm password field is required."
                        ValidationGroup="SetPassword" /><br />
                    <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="confirmPassword"
                        CssClass="field-validation-error" Display="Dynamic" 
                        ErrorMessage="The password and confirmation password do not match."
                        ValidationGroup="SetPassword" />
                </li>
                <li>
                </li>
            </ul>                          
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="changePassword" Visible="false">
            <h3>Change password</h3>
            <asp:ChangePassword runat="server" CancelDestinationPageUrl="~/" ViewStateMode="Disabled" 
                                RenderOuterTable="false" SuccessPageUrl="~/Default.aspx">
                <ChangePasswordTemplate>
                    <p class="validation-summary-errors">
                        <asp:Literal runat="server" ID="FailureText" />
                    </p>
                    <ul style="list-style-type:none">
                        <li>
                            <div style="display:block">
                                <asp:Label runat="server" ID="CurrentPasswordLabel" 
                                    AssociatedControlID="CurrentPassword">Current password</asp:Label><br />
                                <asp:TextBox runat="server" ID="CurrentPassword" TextMode="Password" CssClass="" /><br />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="CurrentPassword"
                                        ErrorMessage="The current password field is required."
                                    ValidationGroup="ChangePassword" />
                            </div>
                        </li>
                        <li>
                            <div style="display:block">
                                <asp:Label runat="server" ID="NewPasswordLabel" 
                                    AssociatedControlID="NewPassword">New password</asp:Label><br />
                                <asp:TextBox runat="server" ID="NewPassword" TextMode="Password" CssClass="" /><br />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="NewPassword"
                                    CssClass="field-validation-error" ErrorMessage="The new password is required."
                                    ValidationGroup="ChangePassword" />
                            </div>
                        </li>
                        <li>
                            <div style="display:block">
                                <asp:Label runat="server" ID="ConfirmNewPasswordLabel" 
                                    AssociatedControlID="ConfirmNewPassword">Confirm new password</asp:Label><br />
                                <asp:TextBox runat="server" ID="ConfirmNewPassword"  TextMode="Password" CssClass=""/><br />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmNewPassword"
                                        Display="Dynamic" ErrorMessage="Confirm new password is required."
                                    ValidationGroup="ChangePassword" /><br />
                                <asp:CompareValidator runat="server" ControlToCompare="NewPassword" 
                                    ControlToValidate="ConfirmNewPassword"
                                    Display="Dynamic" 
                                    ErrorMessage="The new password and confirmation password do not match."
                                    ValidationGroup="ChangePassword" /><br />
                            </div>
                        </li>
                        <li>                        
                            <asp:Button runat="server" CommandName="ChangePassword" Text="Change password" 
                                ValidationGroup="ChangePassword" />
                        </li>
                    </ul>
                </ChangePasswordTemplate>
            </asp:ChangePassword>
        </asp:PlaceHolder>
    </section>
</asp:Content>
