using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api
{
    public static class ReportPieces
    {
        public static readonly string Part1 = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title>MyWineDb Inventory Report</title>" +
            "<style type=\"text/css\" media=\"screen\">" +
            "div.blueTable {border: 1px solid #1C6EA4; border-bottom: 0px; background-color: white; width: 100%; text-align: left; border-collapse: collapse; }" +
            ".divTable.blueTable .divTableRow .divTableCell.Value {text-align: right;}" +
            ".divTable.blueTable .divTableRow .divTableCell {border: 1px solid #AAAAAA; vertical-align: middle; padding-top: .2em; padding-bottom: .2em; }" +
            ".divTable.blueTable .divTableRow:nth-child(even) {background-color: white; }" +
            ".divTable.blueTable .divTableHead {border: 1px solid #AAAAAA; vertical-align: middle; padding: 3px 01px; }" +
            ".divTable.blueTable .divTableBody .divTableCell { font-size: 12px; color: black; }" +
            ".divTable.blueTable .divTableRow:nth-child(even) { background: #ebecee; }" +
            ".divTable.blueTable .divTableHeading { background: #1C6EA4; background: -moz-linear-gradient(top, #5592bb 0%, #327cad 66%, #1C6EA4 100%); " +"" +
                                                    "background: -webkit-linear-gradient(top, #5592bb 0%, #327cad 66%, #1C6EA4 100%);" +
                                                    "background: linear-gradient(to bottom, #5592bb 0%, #327cad 66%, #1C6EA4 100%);" +
                                                    "border-bottom: 2px solid #444444;}" +
            ".divTable.blueTable .divTableHeading .divTableHead { font-size: .75em; font-weight: bold; padding-left: .5em; color: #FFFFFF; border-left: 2px solid #D0E4F5; }" +
            ".divTable.blueTable .divTableHeading .divTableHead.Qty { border-left: none; width: 5em; }" +
            ".divTable.blueTable .divTableHeading .divTableHead.Size { width: 5em; }" +
            ".divTable.blueTable .divTableHeading .divTableHead.Value { width: 8em; }" +
            ".blueTable .tableHeadStyle { font-size: 1.5em; font-weight: bold; color: black; background: white; display: table-cell; padding-right: .5em;  margin-left: .2em; }" +
            ".divTableBody .divTableCell.Value { padding-right: 1em; }" +
            ".blueTable .tableFootStyle { font-size: 14px; font-weight: bold; color: #FFFFFF; background: #D0E4F5; background: -moz-linear-gradient(top, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%);" +
                                            "background: -webkit-linear-gradient(top, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%); background: linear-gradient(to bottom, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%);" +
                                            "border-top: 2px solid #444444;}" +
            ".blueTable .tableFootStyle { font-size: 14px; }" +
            ".blueTable .tableFootStyle .links { text-align: right; }" +
            ".blueTable .tableFootStyle .links a { display: inline-block; background: #1C6EA4; color: #FFFFFF; padding: 2px 8px; border-radius: 5px; }" +
            ".blueTable.outerTableHeader { margin-right: 100px; }" +
            ".blueTable.outerTableHeader .tableHeadStyle .divTableRow .divTableCell { font-size: small; font-weight: normal; padding-left: .2em; }" +
            ".blueTable.outerTableHeader .tableHeadStyle .divTableRow .divTableCell span { font-size: larger; font-weight: bold; }" +
            ".blueTable.outerTableHeader .tableHeadStyle .divTableRow .divTableCell span.metaDetail { font-size: small; font-weight: normal }" +
            ".blueTable.outerTableFooter { border-top: none; border-bottom: 1px solid #444444; padding-top: .2em; }" +
            ".blueTable.outerTableFooter .divTableRow .divTableCell { height: 3em; vertical-align: middle; padding-right: .5em; border-right: 1px solid #444444; " +
                                                                    "text-align: right; background-color: white; color: black; font-size: larger; border-top: 6px solid #444444; }" +
            ".blueTable.outerTableFooter .divTableRow .divTableCell.Value { width: 5.9em; padding-left: .3em; font-weight: bold; border-top: 6px double #444444; }" +
            ".blueTable.outerTableFooter .tableFootStyle { adding: .5em .5em; }" +
            "/* Thanks - DivTable.com! */" + "" +
            ".divTable { display: table; }" +
            ".divTableRow { display: table-row; }" +
            ".divTableHeading { display: table-header-group; }" +
            ".divTableCell, .divTableHead { display: table-cell; }" +
            ".divTableHeadingRow1 { display: table-cell; column-span: 4; }" +
            ".divTableFoot { display: table-footer-group; }" +
            ".divTableBody { display: table-row-group; }" +
            "body { background-color: white; font-family: Arial, Helvetica, sans-serif; }" +
            "p { color:black; margin: 0,0,0,0; line-height: .75em; font-size: 1.1em; }" +
            "p span { font-size: larger; font-weight: bold}" +
            "h1 { color: black; font-size: 1.2em; }" +
            "h1 span { font-size: smaller; font-weight: normal; }" +
            "div { display: table; }</style>" +
            "<style type=\"text/css\" media=\"print\">" +
            "div.blueTable {border: 1px solid #1C6EA4; border-bottom: 0px; background-color: white; width: 100%; text-align: left; border-collapse: collapse; }" +
            ".divTable.blueTable .divTableRow .divTableCell.Value {text-align: right;}" +
            ".divTable.blueTable .divTableRow .divTableCell {border: 1px solid #AAAAAA; vertical-align: middle; padding-top: .2em; padding-bottom: .2em; }" +
            ".divTable.blueTable .divTableRow:nth-child(even) {background-color: white; }" +
            ".divTable.blueTable .divTableHead {border: 1px solid #AAAAAA; vertical-align: middle; padding: 3px 01px; }" +
            ".divTable.blueTable .divTableBody .divTableCell { font-size: 12px; color: black; }" +
            ".divTable.blueTable .divTableRow:nth-child(even) { background: #ebecee; }" +
            ".divTable.blueTable .divTableHeading { background: #CCCCCC; border-bottom: 2px solid #444444;}" +
            ".divTable.blueTable .divTableHeading .divTableHead { font-size: .75em; font-weight: bold; padding-left: .5em; color: black; border-left: 2px solid #D0E4F5; }" +
            ".divTable.blueTable .divTableHeading .divTableHead.Qty { border-left: none; width: 5em; }" +
            ".divTable.blueTable .divTableHeading .divTableHead.Size { width: 5em; }" +
            ".divTable.blueTable .divTableHeading .divTableHead.Value { width: 8em; }" +
            ".blueTable .tableHeadStyle { font-size: 1.5em; font-weight: bold; color: black; background: white; display: table-cell; padding-right: .5em;  margin-left: .2em; }" +
            ".divTableBody .divTableCell.Value { padding-right: 1em; }" +
            ".blueTable .tableFootStyle { font-size: 14px; font-weight: bold; color: #FFFFFF; background: #D0E4F5; background: -moz-linear-gradient(top, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%);" +
                                            "background: -webkit-linear-gradient(top, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%); background: linear-gradient(to bottom, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%);" +
                                            "border-top: 2px solid #444444;}" +
            ".blueTable .tableFootStyle { font-size: 14px; }" +
            ".blueTable .tableFootStyle .links { text-align: right; }" +
            ".blueTable .tableFootStyle .links a { display: inline-block; background: #1C6EA4; color: #FFFFFF; padding: 2px 8px; border-radius: 5px; }" +
            ".blueTable.outerTableHeader { margin-right: 100px; }" +
            ".blueTable.outerTableHeader .tableHeadStyle .divTableRow .divTableCell { font-size: small; font-weight: normal; padding-left: .2em; }" +
            ".blueTable.outerTableHeader .tableHeadStyle .divTableRow .divTableCell span { font-size: larger; font-weight: bold; }" +
            ".blueTable.outerTableHeader .tableHeadStyle .divTableRow .divTableCell span.metaDetail { font-size: small; font-weight: normal }" +
            ".blueTable.outerTableFooter { border-top: none; border-bottom: 1px solid #444444; padding-top: .2em; }" +
            ".blueTable.outerTableFooter .divTableRow .divTableCell { height: 3em; vertical-align: middle; padding-right: .5em; border-right: 1px solid #444444; " +
                                                                    "text-align: right; background-color: white; color: black; font-size: larger; border-top: 6px solid #444444; }" +
            ".blueTable.outerTableFooter .divTableRow .divTableCell.Value { width: 5.9em; padding-left: .3em; font-weight: bold; border-top: 6px double #444444; }" +
            ".blueTable.outerTableFooter .tableFootStyle { adding: .5em .5em; }" +
            "/* Thanks - DivTable.com! */" + "" +
            ".divTable { display: table; }" +
            ".divTableRow { display: table-row; }" +
            ".divTableHeading { display: table-header-group; }" +
            ".divTableCell, .divTableHead { display: table-cell; }" +
            ".divTableHeadingRow1 { display: table-cell; column-span: 4; }" +
            ".divTableFoot { display: table-footer-group; }" +
            ".divTableBody { display: table-row-group; }" +
            "body { background-color: white; font-family: Arial, Helvetica, sans-serif; }" +
            "p { color:black; margin: 0,0,0,0; line-height: .75em; font-size: 1.1em; }" +
            "p span { font-size: larger; font-weight: bold}" +
            "h1 { color: black; font-size: 1.2em; }" +
            "h1 span { font-size: smaller; font-weight: normal; }" +
            "div { display: table; }" +
            "</style></head><body>";

        public static readonly string EndOfPage = "</body></html>";

        public static string SummaryData(string DateOfReport, string TotalValue, string TotalNumberOfBottles, string AverageBottleValue)
        {
               return $"<h1>Inventory<span> as of: {DateOfReport}</ span ></ h1><p>Total Value: <span>$ {TotalValue}</span>" +
                        $"<p> Total Number of Bottles: <span>{TotalNumberOfBottles}</span></p>" +            
                        $"<p>Average Value per Bottle: <span>$ {AverageBottleValue}</span></p>";
        }

        public static string CellarHeader(string CellarName, string Capacity, string PctOfCapacity)
        {
            return $"<div class=\"blueTable outerTableHeader\"><div class=\"tableHeadStyle\">" +
                $"<div class=\"divTableRow\"><div class=\"divTableCell\">Cellar: <span>" +
                $"{CellarName}</span><br><span class=\"metaDetail\">Capacity: <span>" +
                $"{Capacity}</span > - % of Capacity: <span>{PctOfCapacity}</span ></span></div>" +
                "</div></div></div>";
        }

        public static string VintageHeader(string vintageYear)
        {
            return $"<div class=\"blueTable outerTableHeader\"><div class=\"tableHeadStyle\">" +
                    "<div class=\"divTableRow\"><div class=\"divTableCell\">Vintage: <span>" +
                    $"{vintageYear}</span></div></div></div></div>";
        }

        public static string CellarLineHeader()
        {
            return  "<div class=\"divTable blueTable\"><div class=\"divTableHeading\">" +
                    "<div class=\"divTableRow\"><div class=\"divTableHead Qty\">Qty</div>" +
                    "<div class=\"divTableHead Size\">Size (ml)</div><div class=\"divTableHead\">" +
                    "Description</div><div class=\"divTableHead Value\">Unit Value</div>" +
                    "<div class=\"divTableHead Value\">Total Value</div></div></div>" + 
                    "<div class=\"divTableBody\">";
        }

        public static string CellarLineItem(string Quantity, string Size, string WineryName, string Region, string Country,
            string WineName, string Color, string VarietalType, string UnitValue, string TotalValue)
        {
            return "<div class=\"divTableRow\"><div class=\"divTableCell Value\">" +
                    $"{Quantity}</div><div class=\"divTableCell Value\">{Size}</div>" +
                    $"<div class=\"divTableCell\">{WineryName} - {Region},{Country}: {WineName}<br/>" +
                    $"Color: {Color} Varietal Type: {VarietalType}</div>" +
                    $"<div class= \"divTableCell Value\">{UnitValue}</div>" +
                    $"<div class= \"divTableCell Value\">{TotalValue}</div></div>";
        }
        public static string CellarFooter(string CellarTotalValue)
        {
            return "<div class=\"blueTable outerTableFooter\"><div class=\"divTableRow\">" +
                    "<div class=\"divTableCell\">CellarTotal</div>" +
                    $"<div class=\"divTableCell Value\">$ {CellarTotalValue}</div></div></div>";
        }
    }
}
