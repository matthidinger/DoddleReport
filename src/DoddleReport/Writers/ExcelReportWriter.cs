using System.Text;

namespace DoddleReport.Writers
{
    public class ExcelReportWriter : HtmlReportWriter
    {
        protected override void RenderHeader(ReportTextFieldCollection textFields, RenderHintsCollection hints)
        {
            Html.AppendLine(ExcelHeaderHtml(textFields));
            WrapHeadAndBody = true;
            IgnoreTitleImage = true;

            base.RenderHeader(textFields, hints);

            Html.Replace("<hr />", "<br /><br />");
        }

        protected override string GetCellStyle(ReportRow row, RowField field)
        {
            string html = base.GetCellStyle(row, field);

            if (field.DataType == typeof(int))
            {
                html += @"mso-number-format:\@;";
            }
            return html;
        }

        public string ExcelHeaderHtml(ReportTextFieldCollection textFields)
        {
            var sb = new StringBuilder();
            sb.Append("<html xmlns:o='urn:schemas-microsoft-com:office:office'\n" +

                      "xmlns:x='urn:schemas-microsoft-com:office:excel'\n" +
                      "xmlns='http://www.w3.org/TR/REC-html40'>\n" +

                      "<head>\n");
            sb.Append("<style>\n");

            sb.Append("@page");
            sb.Append("{margin:.5in .75in .5in .75in;\n");

            sb.Append("mso-header-margin:.5in;\n");
            sb.Append("mso-footer-margin:.5in;\n");

            sb.Append("mso-page-orientation:landscape;}\n");

            sb.Append("@print { mso-print-title-row:\"$1:$7\"; }");

            sb.Append("thead { display:table-header-group; }");
            sb.Append("tbody { display:table-body-group; }");
            sb.Append("tfoot { display:table-footer-group; }");

            sb.Append("</style>\n");

            sb.Append("<!--[if gte mso 9]><xml>\n");
            sb.Append("<x:ExcelWorkbook>\n");

            sb.Append("<x:ExcelWorksheets>\n");
            sb.Append("<x:ExcelWorksheet>\n");

            sb.AppendFormat("<x:Name>{0}</x:Name>\n", textFields.Title);
            sb.Append("<x:WorksheetOptions>\n");

            int numberOfHeaderLines = textFields.Title.NumberOfLines() + textFields.SubTitle.NumberOfLines();

            // Header contains a <hr /> which is converted into <br/><br/> so it has 1 extra line break if it isn't empty
            if (!string.IsNullOrEmpty(textFields.Header))
            {
                numberOfHeaderLines += textFields.Header.NumberOfLines() + 1;
            }

            sb.AppendFormat(@"
                <x:FreezePanes/>
                <x:FrozenNoSplit/>
                <x:SplitHorizontal>{0}</x:SplitHorizontal>
                <x:TopRowBottomPane>{0}</x:TopRowBottomPane>",
                        numberOfHeaderLines + 1);


            sb.Append("<x:FitToPage/>\n");

            sb.Append("<x:Print>\n");
            sb.Append("<x:FitHeight>999</x:FitHeight>\n");
            sb.Append("<x:ValidPrinterInfo/>\n");
            sb.Append("<x:PaperSizeIndex>9</x:PaperSizeIndex>\n");
            sb.Append("<x:Scale>67</x:Scale>\n");
            sb.Append("<x:HorizontalResolution>600</x:HorizontalResolution\n");
            sb.Append("<x:VerticalResolution>600</x:VerticalResolution\n");
            sb.Append("</x:Print>\n");

            sb.Append("<x:Selected/>\n");
            sb.Append("<x:DoNotDisplayGridlines/>\n");

            sb.Append("<x:ProtectContents>False</x:ProtectContents>\n");
            sb.Append("<x:ProtectObjects>False</x:ProtectObjects>\n");

            sb.Append("<x:ProtectScenarios>False</x:ProtectScenarios>\n");
            sb.Append("</x:WorksheetOptions>\n");

            sb.Append("</x:ExcelWorksheet>\n");
            sb.Append("</x:ExcelWorksheets>\n");

            sb.Append("<x:WindowHeight>12780</x:WindowHeight>\n");
            sb.Append("<x:WindowWidth>19035</x:WindowWidth>\n");

            sb.Append("<x:WindowTopX>0</x:WindowTopX>\n");
            sb.Append("<x:WindowTopY>15</x:WindowTopY>\n");

            sb.Append("<x:ProtectStructure>False</x:ProtectStructure>\n");
            sb.Append("<x:ProtectWindows>False</x:ProtectWindows>\n");

            sb.Append("</x:ExcelWorkbook>\n");
            sb.Append("</xml><![endif]-->\n");

            sb.Append("</head><body>\n");


            return sb.ToString();

        }
    }
}