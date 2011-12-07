<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>DoddleReport - turn any IEnumerable set of data into pluggable reports!</h2>
    
    <h3><a href="http://doddlereport.codeplex.com" target="_blank">Find it on CodePlex</a></h3>

    <h3>Usage</h3>
    <div style="color:Black;background-color:White;"><pre style="font-size: 12px">
<span style="color:Blue;">public</span> ReportResult ProductReport()
{
    <span style="color:Green;">// Get the data for the report (any IEnumerable will work)</span>
    <span style="color:Blue;">var</span> query = ProductRepository.GetAll();
    <span style="color:Blue;">var</span> totalProducts = query.Count;
    <span style="color:Blue;">var</span> totalOrders = query.Sum(p =&gt; p.OrderCount);


    <span style="color:Green;">// Create the report and turn our query into a ReportSource</span>
    <span style="color:Blue;">var</span> report = <span style="color:Blue;">new</span> Report(query.ToReportSource());


    <span style="color:Green;">// Customize the Text Fields</span>
    report.TextFields.Title = <span style="color:#A31515;">&quot;Products Report&quot;</span>;
    report.TextFields.SubTitle = <span style="color:#A31515;">&quot;This is a sample report showing how Doddle Report works&quot;</span>;
    report.TextFields.Footer = <span style="color:#A31515;">&quot;Copyright 2011 &amp;copy; The Doddle Project&quot;</span>;
    report.TextFields.Header = <span style="color:Blue;">string</span>.Format(<span style="color:#A31515;">@&quot;
        Report Generated: {0}
        Total Products: {1}
        Total Orders: {2}
        Total Sales: {3:c}&quot;</span>, DateTime.Now, totalProducts, totalOrders, totalProducts * totalOrders);


    <span style="color:Green;">// Render hints allow you to pass additional hints to the reports as they are being rendered</span>
    report.RenderHints.BooleanCheckboxes = <span style="color:Blue;">true</span>;


    <span style="color:Green;">// Customize the data fields</span>
    report.DataFields[<span style="color:#A31515;">&quot;Id&quot;</span>].Hidden = <span style="color:Blue;">true</span>;
    report.DataFields[<span style="color:#A31515;">&quot;Price&quot;</span>].DataFormatString = <span style="color:#A31515;">&quot;{0:c}&quot;</span>;
    report.DataFields[<span style="color:#A31515;">&quot;LastPurchase&quot;</span>].DataFormatString = <span style="color:#A31515;">&quot;{0:d}&quot;</span>;



    <span style="color:Green;">// Return the ReportResult</span>
    <span style="color:Green;">// the type of report that is rendered will be determined by the extension in the URL (.pdf, .xls, .html, etc)</span>
    <span style="color:Blue;">return</span> <span style="color:Blue;">new</span> ReportResult(report);
}
</pre></div>

    <h3>Samples!</h3>

    <table width="100%">
        <tr>
            <td>
                <h3>Excel Report - with automatic frozen headers!</h3>
                <%: Html.ActionLink("See it Live!", "ProductReport", "Doddle", new { extension = "xls"}, null) %>
                
            </td>
        
            <td>
                <h3>PDF Report - headers repeat on every page</h3>
                <%: Html.ActionLink("See Live!", "ProductReport", "Doddle", new { extension = "pdf" }, null)%>
            </td>
        </tr>
        <tr>
            <td>
                <a href="<%= ResolveUrl("~/Content/DoddleXlsReport.png") %>">
                    <img src="<%= ResolveUrl("~/Content/DoddleXlsReport.png") %>" width="500"  alt="Excel Report" />
                </a>
            </td>

            <td>
                <a href="<%= ResolveUrl("~/Content/DoddlePdfReport.png") %>">
                    <img src="<%= ResolveUrl("~/Content/DoddlePdfReport.png") %>" width="500"  alt="PDF Report" />
                </a>
            </td>
        </tr>
        <tr>
            <td>
                <h3>HTML Report</h3>
                <%: Html.ActionLink("See Live!", "ProductReport", "Doddle", new { extension = "html" }, null)%>
            </td>
            <td>
                <h3>CSV/Delimited Output</h3>
                <%: Html.ActionLink("See Live!", "ProductReport", "Doddle", new { extension = "txt" }, null)%>
            </td>
        </tr>
        <tr>
            <td>
                 <a href="<%= ResolveUrl("~/Content/DoddleHtmlReport.png") %>">
                    <img src="<%= ResolveUrl("~/Content/DoddleHtmlReport.png") %>" width="500" alt="HTML Report" />
                </a>
            </td>

            <td>
                 <a href="<%= ResolveUrl("~/Content/DoddleTxtReport.png") %>">
                    <img src="<%= ResolveUrl("~/Content/DoddleTxtReport.png") %>" width="500" alt="CSV Report" />
                </a>
            </td>
        </tr>
    </table>
 
</asp:Content>
