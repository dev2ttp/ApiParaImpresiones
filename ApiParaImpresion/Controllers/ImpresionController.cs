using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Web.Http;
using ApiParaImpresion.Properties;
using ApiParaImpresion.Models;
using ApiParaImpresion.Services;
using Newtonsoft.Json;
using System.Text;
using System.Web.Hosting;
using System.Threading.Tasks;

namespace ApiParaImpresion.Controllers
{
    public class ImpresionController : ApiController
    {

        [Route("api/v1/RealizarImpresion")]
        [HttpPost]
        public async Task<PrintResponse> RealizarImpresion([FromBody] PrintReq req)
        {
            PrintService service = new PrintService();
            var response = await service.RealizarImpresion(req);
            return response;
        }

        [Route("api/v1/ImprimirPdfRotado")]
        [HttpGet]
        public async Task<PrintResponse> PrintPdfRotated(string fileName)
        {
            PrintService service = new PrintService();
            var response = await service.PrintPdfRotated(fileName);
            return response;
        }

        [Route("api/v1/PrintBonoAsync")]
        [HttpPost]
        public async Task<PrintResponse> PrintBonoAsync([FromBody] PrintBonoReq req)
        {
            PdfService service = new PdfService();
            var response = await service.ConvertTxtToPdfAndPrint(req);
            return response;
        }
    }
}