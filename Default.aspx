<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb"
     Inherits="GoalGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link href="Content/Default.css" rel="stylesheet" />
    <link href="Content/tutorial.css" rel="stylesheet" />

    <script type="text/javascript">
        $(document).ready(function () {
            var tour = defaultPageTour();

            $('.tutorial').on('click', function () {
                startTutorial(tour);
            });
        });

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
    <div class="shortVersion">
	  <span class="tTitle">How To Obtain Sweet Goals</span>
	  <div class="steps">
		<div class="step removeLeftBorder">
			<img src="/Images/step1.png" alt="Create Goal" class="tImg">
			<span class="tComment">Create Goal </span>
		</div>
		<div class="step ">
			<img src="/Images/step2.png" alt="Come up with activities" class="tImg">
			<span class="tComment">Brainstorm Activities </span>
		</div>
		<div class="step">
			<img src="/Images/step3.png" alt="Complete activities for the goal" class="tImg">
			<span class="tComment">Complete Activities</span>
		</div>
		<div class="step">
			<img src="/Images/step4.png" alt="Sweet Goal!" class="tImg">
			<span class="tComment">Sweet Goal!</span>
		</div>
	  </div>
  </div>        

    <div id="main" class="main" runat="server">
        <h1 style="display:none;"><asp:label runat="server" ID="GoalTypes"/></h1>
    </div>
</asp:Content>
