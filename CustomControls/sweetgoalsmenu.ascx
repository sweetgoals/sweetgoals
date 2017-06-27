<%@ Control Language="VB" AutoEventWireup="false" CodeFile="sweetgoalsmenu.ascx.vb" Inherits="sweetgoalsmenu" %>

<script type="text/javascript">
    $(document).ready(function () {
        // Start the tour
        $('#tutorial').on('click', function () {
            tour.init();
            tour.start();
            tour.restart();
        });
    });
</script>

<div class="topSection">
	<ul id="menubar" class="menuBar" runat="server"/>
<%--    <a id="donate" class="donate" target="_blake" 
        href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=donate@sweetgoals.com&
                item_name=Sweet Goals&no_note=0&bn=PP-DonationsBF:btn_donate_LG.gif:NonHostedGuest&
                currency_code=USD">
        <img id="donateImg" 
                src="https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif"
                border="0" 
                alt="Donate Now Using PayPal" />
    </a>--%>
</div>