using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using ApiParaImpresion.Models;
using ApiParaImpresion.Services;
using ApiParaImpresion.Properties;
using Newtonsoft.Json;

namespace ApiParaImpresion.Services
{
    public class PdfService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public async Task<PrintResponse> ConvertTxtToPdfAndPrint(PrintBonoReq req)
        {
            PrintResponse response = new PrintResponse();
            try
            {
                log.Info("Q: Print Bono Async:: ");
                await Task.Run(() =>
                {
                    log.Info("bodyData: Print Bono Async: " + JsonConvert.SerializeObject(req));
                    //string text = File.ReadAllText((Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Content", "TXT", "TextFromPDF1.txt")));
                    string text = req.bono;
                    log.Info("Q: Convert to PdF: " + text);
                    PdfDocument doc = new PdfDocument();
                    PdfSection section = doc.Sections.Add();
                    section.PageSettings.Height = 300;
                    section.PageSettings.Width = 700;
                    PdfMargins margins = new PdfMargins
                    {
                        Top = 0,
                        Left = 40
                    };
                    section.Document.PageSettings.Margins = margins;
                    section.PageSettings.Margins = margins;
                    //section.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle270;
                    PdfPageBase page = section.Pages.Add();
                    PdfFont font = new PdfFont(PdfFontFamily.Helvetica, 8);
                    PdfStringFormat format = new PdfStringFormat
                    {
                        LineSpacing = 10f
                    };
                    PdfBrush brush = PdfBrushes.Black;
                    PdfTextWidget textwidget = new PdfTextWidget(text, font, brush);
                    float y = 0;
                    PdfTextLayout textLayout = new PdfTextLayout();
                    textLayout.Break = PdfLayoutBreakType.FitPage;
                    textLayout.Layout = PdfLayoutType.OnePage;
                    RectangleF bounds = new RectangleF(new PointF(0, y), page.Canvas.ClientSize);
                    textwidget.StringFormat = format;
                    textwidget.Draw(page, bounds, textLayout);
                    var fileName = "Bono_" + DateTime.Now.ToString("HHmmss") + ".pdf";
                    doc.SaveToFile((Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Content", "PDF", fileName)), FileFormat.PDF);
                    log.Info("R: Save file as PDF: OK");
                    //byte[] docBytes = File.ReadAllBytes((Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Content", "PDF", fileName)));
                    //PRINT
                    PrintService service = new PrintService();
                    var printResp = service.PrintPdfRotated(fileName);
                    response = printResp.Result;
                });
                return response;
            }
            catch (Exception e)
            {
                response.status = false;
                response.code = 804;
                response.message = e.ToString();
                log.Error("E: Print Bono Async: " + JsonConvert.SerializeObject(response));
                return response;
            }
        }
    }
}