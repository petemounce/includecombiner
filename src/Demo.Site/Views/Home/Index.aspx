<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>IncludeCombiner</h2>
    <p>
        This page is intentionally unstyled and lacking in JS.  This is so the first request doesn't have the side-effect of populating the include-combiner while I benchmark.  It also means the first-request JIT compilation doesn't get included in my benchmarking.
    </p>
    <p><%= Html.RouteLink("index", new { controller = "include", action = "index" })%></p>
    
    <h3>Benchmarking</h3>
    <p>Before each test:</p>
    <ol>
			<li>Ensure Compilation Debug="false" in web.config</li>
			<li>Recycle IIS app-pool</li>
			<li>Clear browser cache</li>
		</ol>
		<ol>
			<li>Request each of the benchmark pages in order; note down timing result from Firebug/Net</li>
			<li>Run Hammerhead against each benchmark page (now that the browser cache is primed)</li>
    </ol>
</asp:Content>
