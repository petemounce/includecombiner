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
    <h3>JS tests</h3>
    <ul>
			<li><%= Html.RouteLink("jquery + jquery-ui", new { action = "jsjquery" })%></li>
			<li><%= Html.RouteLink("mootools", new { action = "jsmootools" })%></li>
			<li><%= Html.RouteLink("scriptaculous", new { action = "jsscriptaculous" })%></li>
			<li><%= Html.RouteLink("many js includes", new { action = "jsmany" })%></li>
    </ul>
    
    <h3>CSS tests</h3>
    <ul>
			<li><%= Html.RouteLink("one include", new { action = "cssone" })%></li>
			<li><%= Html.RouteLink("many includes", new { action = "cssmany" })%></li>
    </ul>
    
</asp:Content>
