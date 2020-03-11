using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using ApiParaImpresion.Models;
using ApiParaImpresion.Properties;
using Newtonsoft.Json;

namespace ApiParaImpresion.Services
{
    public class PrintService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly DllPrinter.PrinterManager printer = new DllPrinter.PrinterManager();

        public async Task<PrintResponse> RealizarImpresion(PrintReq req)
        {
            log.Info("Q: Realizar Impresión: " + Settings.Default.printerName);
            PrintResponse response = new PrintResponse();
            try
            {
                await Task.Run(() =>
                {
                    var printResp = printer.PrintTicket(Settings.Default.printerType, req.document, Settings.Default.printerName);
                    if (printResp.getResponse())
                    {
                        response.status = true;
                        response.code = 200;
                        response.message = "Impresión OK";
                        log.Info("R: Realizar Impresión: " + JsonConvert.SerializeObject(response));
                    }
                    else
                    {
                        response.status = false;
                        response.code = 400;
                        response.message = "Error al imprimir";
                        log.Info("E: Realizar Impresión: " + JsonConvert.SerializeObject(response));
                    }
                });
                return response;
            }
            catch (Exception e)
            {
                response.status = false;
                response.code = 804;
                response.message = e.ToString();
                log.Error("E: Realizar Impresión: " + JsonConvert.SerializeObject(response));
                return response;
            }
        }

        public async Task<PrintResponse> PrintPdfRotated(string fileName)
        {
            PrintResponse response = new PrintResponse();
            try
            {
                await Task.Run(() =>
                {
                    byte[] docBytes = File.ReadAllBytes(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Content", "PDF", fileName));
                    var printResp = printer.ImprimirPdf(docBytes, false, Settings.Default.printerName, Settings.Default.printerType);
                    File.Delete(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Content", "PDF", fileName));
                    if (printResp.getResponse())
                    {
                        var cutResp = printer.SendCut(Settings.Default.printerName);
                        if (cutResp)
                        {
                            log.Info("R: Realizar Impresión: Corte OK");
                            response.status = true;
                            response.code = 200;
                            response.message = printResp.getGlosa();
                            log.Info("R: Realizar Impresión: " + JsonConvert.SerializeObject(response));
                        }
                        else
                        {
                            log.Info("E: Realizar Impresión: Corte NOK");
                            response.status = true;
                            response.code = 201;
                            response.message = printResp.getGlosa();
                            log.Info("R: Realizar Impresión: " + JsonConvert.SerializeObject(response));
                        }
                    }
                    else
                    {
                        response.status = false;
                        response.code = 400;
                        response.message = printResp.getGlosa();
                        log.Info("E: Realizar Impresión: " + JsonConvert.SerializeObject(response));
                    }
                });
                return response;
            }
            catch (Exception e)
            {
                response.status = false;
                response.code = 804;
                response.message = e.ToString();
                log.Error("E: Realizar Impresión: " + JsonConvert.SerializeObject(response));
                return response;
            }
        }
    }
}