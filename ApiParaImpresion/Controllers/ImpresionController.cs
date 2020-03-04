using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using TotalPack.Printing;
using DllPrinter;
using ApiParaImpresion.Properties;
using ApiParaImpresion.Models;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace ApiParaImpresion.Controllers
{
    public class ImpresionController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly DllPrinter.PrinterManager printer = new DllPrinter.PrinterManager();

        [Route("api/Impresion/RealizarImpresion")]
        [HttpPost]
        public PrintResponse RealizarImpresion([FromBody] PrintReq req)
        {
            log.Info("Q: Realizar Impresión: " + Settings.Default.printerName);
            PrintResponse response = new PrintResponse();
            try
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