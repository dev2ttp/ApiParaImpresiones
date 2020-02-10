using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using TotalPack.Tesoreria.Printing;
using DllPrinter;

namespace ApiParaImpresion.Controllers
{
    public class ImpresionController : ApiController
    {
        //private readonly PrinterBase printer = PrinterManager.GetPrinter("Star TUP900 Presenter (TUP992)");
        DllPrinter.PrinterManager printer = new DllPrinter.PrinterManager();
        DllPrinter.PrintResponseModel res = new PrintResponseModel();
        // GET api/values
        [HttpPost]
        [Route("api/Impresion/RealizarImpresion")]
        public IEnumerable<string> RealizarImpresion([FromBody] String Docuemnto)
        {
           printer.PrintTicket("TUP900", Docuemnto, "Star TUP900 Presenter (TUP992)");
            return new string[] { "value1", "value2" };
        }
    }
}